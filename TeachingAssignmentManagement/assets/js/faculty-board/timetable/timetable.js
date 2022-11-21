var rootUrl = $('#loader').data('request-url');

$(function () {
    // Display message when table have no data
    var rowCount = $('#tblAssign tbody tr').length;
    if (rowCount == 0) {
        $('#assignLecturerDiv').append('<h4 class="text-center mt-2">Học kỳ này chưa có dữ liệu <i class="feather feather-help-circle"></i></h4>');
    } else {      
        // Waves Effect
        Waves.init();
        Waves.attach(".btn-assign", ['waves-float', 'waves-light']);
        // Trigger tooltip for card when document on ready
        $('[data-bs-toggle="tooltip"]').tooltip({
            trigger: 'hover'
        });
    }
});

$('table .form-select option:selected').each(function () {
    var selectedLecturer = $(this);
    if (selectedLecturer.val() != '') {
        // Add previous selected value to jquery data
        selectedLecturer.closest('.form-select').attr('data-preval', selectedLecturer.val());
        selectedLecturer.closest('.form-select').attr('data-pretext', selectedLecturer.text());

        var lecturerName = splitString(selectedLecturer.text());
        $(this).text(lecturerName);
    } else {
        selectedLecturer.closest('.assign-card').addClass('bg-opacity-50');
    }
});

$('table .form-select').on('select2:select select2:unselecting', function () {
    var $this = $(this);

    // Call function to change lecturer
    changeLecturer($this);
});

$('.btn-assign').on('click', function () {
    $this = $(this);

    // Get values
    var id = $this.closest('.assign-card').attr('id');
    var lecturerId = $this.parent().find('.select2 :selected').val();

    // Send ajax request to assign lecturer
    $.ajax({
        type: 'POST',
        url: rootUrl + 'FacultyBoard/Timetable/Assign',
        data: { id, lecturerId },
        success: function (data) {
            if (data.success) {
                var assignCard = $this.closest('.assign-card');
                var opacityClass = 'bg-opacity-50';
                // Add class to assign card if value is empty and remove if assigned
                "" == lecturerId ? assignCard.addClass(opacityClass) : assignCard.hasClass(opacityClass) && assignCard.removeClass(opacityClass);

                // Display success message
                toastr.options.positionClass = 'toast-bottom-right';
                toastr.success('Thành công!');
            }
        }
    });
});

$('.btn-delete').on('click', function () {
    $this = $(this);

    // Get values
    var id = $this.closest('.assign-card').attr('id');

    // Show confirm message
    Swal.fire({
        title: 'Thông báo',
        text: 'Bạn có chắc muốn xoá lớp học phần này?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Xoá',
        cancelButtonText: 'Huỷ',
        customClass: {
            confirmButton: 'btn btn-primary',
            cancelButton: 'btn btn-outline-danger ms-1'
        },
        buttonsStyling: false
    }).then((result) => {
        if (result.isConfirmed) {
            // Send ajax request to delete class
            $.ajax({
                type: 'POST',
                url: rootUrl + 'FacultyBoard/Timetable/Delete',
                data: { id },
                success: function (data) {
                    if (data.success) {
                        // Remove element when delete succeeded
                        toastr.options.positionClass = 'toast-bottom-right';
                        toastr.success('Xoá lớp thành công!');
                    }
                }
            });
        }
    })
});

function splitString(lecturerName) {
    // Split lecture name
    var removeLastWord = lecturerName.substring(0, lecturerName.lastIndexOf(' '));
    var splitName = removeLastWord.split(' ').map(function (item) { return item[0] }).join('.');
    var result = splitName + " " + lecturerName.split(' ').pop();
    return result;
}

function changeLecturer($this) {
    // Destroy select2 to update option text
    $this.select2('destroy').unwrap();

    // Split new selected lecturer name
    var newSelected = $this.find(':selected');
    var currentVal = newSelected.val();
    var currentText = newSelected.text();
    var newSelectedText = splitString(currentText);
    "" != currentVal && newSelected.text(newSelectedText);

    // Change previous selected option value and text
    var preVal = $this.data('preval');
    var preText = $this.data('pretext');
    $this.children('option[value = "' + preVal + '"]').text(preText);

    // Set data for preval and pretext
    $this.data('preval', currentVal);
    $this.data('pretext', currentText);

    // Populate select2 dropdown again
    populateSelect($this);
}
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

        // Initialize Tooltip
        $('[data-bs-toggle="tooltip"]').tooltip({
            trigger: 'hover'
        });

        // Initialize Popover
        var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
        var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
            return new bootstrap.Popover(popoverTriggerEl, {
                html: true,
                sanitize: false
            });
        });
    }
});

// Split lecturerName
$('.assign-card').each(function () {
    var $this = $(this);
    var lecturerId = $this.data('lecturerid');
    var lecturerName = $this.text();
    if (lecturerId != '') {
        $this.text(splitString(lecturerName));
    } else {
        $this.removeClass('btn-success btn-warning').addClass('btn-secondary');
    }
});

$(document).off('click', '.btn-assign').on('click', '.btn-assign', function () {
    $this = $(this);

    // Get values
    var id = $this.data('id'),
        termId = $('#term').val(),
        lecturerSelect = $this.parent().find('.select2 :selected'),
        lecturerId = lecturerSelect.val(),
        lecturerName = lecturerSelect.text();

    // Send ajax request to check state of lecturer
    $.ajax({
        type: 'GET',
        url: rootUrl + 'FacultyBoard/Timetable/CheckState',
        data: { id, termId, lecturerId },
        success: function (data) {
            if (data.success) {
                // Call function to assign lecturer
                assignLecturer(id, lecturerId, lecturerName);
            } else {
                let errorMessage = data.message + '<div class="table-responsive mt-2"><table class="table"><thead><tr><th>Mã LHP</th><th>Tên HP</th><th>Ngành</th></tr></thead><tbody>';
                data.classList.forEach(function (item, index) {
                    errorMessage += '<tr><td>' + item.classId + '</td><td>' + item.curriculumName + '</td><td>' + item.majorName + '</td></tr>';
                });
                errorMessage += '</tbody></table></div>';
                // Show message when assign failed
                Swal.fire({
                    title: 'Thông báo',
                    width: 600,
                    html: errorMessage,
                    icon: 'error',
                    customClass: {
                        confirmButton: 'btn btn-primary'
                    },
                    buttonsStyling: false
                })
            }
        }
    });
});

$('.assign-card').on('click', function () {
    // Hide other popovers when a popover is clicked
    $('[data-bs-toggle="popover"]').not(this).popover('hide');
});

$('.assign-card').on('show.bs.popover', function () {
    setTimeout(() => {
        var formSelect = $('.popover-body .form-select');
        if (!$(formSelect).hasClass("select2-hidden-accessible")) {
            // Apply select2
            populateSelect(formSelect);
            // Add focus to search field
            formSelect.on('select2:open', () => {
                document.querySelector('.select2-search__field').focus();
            });
        } else {
            // Set selected value for dropdown
            var lecturerId = $(this).data('lecturerid');
            formSelect.val(lecturerId);
        }
    }, 0);
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

function assignLecturer(id, lecturerId, lecturerName) {
    // Send ajax request to assign lecturer
    $.ajax({
        type: 'POST',
        url: rootUrl + 'FacultyBoard/Timetable/Assign',
        data: { id, lecturerId },
        success: function (data) {
            if (data.success) {
                // Update assign card data
                var assignCard = $('#' + id);
                updateClass(assignCard, lecturerId, lecturerName);

                // Display success message
                toastr.options.positionClass = 'toast-bottom-right';
                toastr.success('Thành công!');
            }
        }
    });
}
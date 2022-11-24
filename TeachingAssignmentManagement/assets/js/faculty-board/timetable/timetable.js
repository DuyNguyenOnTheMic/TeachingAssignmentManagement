﻿var rootUrl = $('#loader').data('request-url');

$(function () {
    // Display message when table have no data
    var rowCount = $('#tblAssign tbody tr').length;
    if (rowCount == 0) {
        $('#assignLecturerDiv').append('<h4 class="text-center mt-2">Học kỳ này chưa có dữ liệu <i class="feather feather-help-circle"></i></h4>');
    } else {
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
                // Populate error message into table
                let errorMessage = data.message + '<div class="table-responsive mt-2"><table class="table"><thead><tr><th>Mã LHP</th><th>Tên HP</th><th>Ngành</th></tr></thead><tbody>';
                data.classList.forEach(function (item, index) {
                    errorMessage += '<tr class="font-small-3"><td>' + item.classId + '</td><td>' + item.curriculumName + '</td><td>' + item.majorName + '</td></tr>';
                });
                errorMessage += '</tbody></table></div>';
                // Show message when assign failed
                Swal.fire({
                    title: 'Thông báo',
                    width: 800,
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

$(document).off('click', '.btn-delete').on('click', '.btn-delete', function () {
    $this = $(this);

    // Get values
    var id = $this.data('id');
    var curriculumClassId = $this.parents().find('.popover-header').text();

    // Show confirm message
    Swal.fire({
        title: 'Hãy nhập lại tên lớp học phần ' + curriculumClassId + ' để xác nhận xoá',
        input: 'text',
        inputAttributes: {
            autocapitalize: 'off'
        },
        showCancelButton: true,
        confirmButtonText: 'Look up',
        showLoaderOnConfirm: true,
        preConfirm: (classId) => {
            toastr.options.positionClass = 'toast-bottom-right';
            if (classId === curriculumClassId) {
                // Send ajax request to delete class
                $.ajax({
                    type: 'POST',
                    url: rootUrl + 'FacultyBoard/Timetable/Delete',
                    data: { id },
                    success: function (data) {
                        if (data.success) {
                            // Remove element when delete succeeded
                            toastr.success('Xoá lớp thành công!');
                        }
                    }
                });
            } else {
                toastr.warning('Xác nhận xoá thất bại!');
                return false;
            }
        },
        allowOutsideClick: () => !Swal.isLoading()
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire({
                title: `${result.value.login}'s avatar`,
                imageUrl: result.value.avatar_url
            })
        }
    })
});

$('.assign-card').on('click', function () {
    // Hide other popovers when a popover is clicked
    $('[data-bs-toggle="popover"]').not(this).popover('hide');
}).on('show.bs.popover', function () {
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
            formSelect.val(lecturerId).trigger('change');
        }
    }, 0);
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
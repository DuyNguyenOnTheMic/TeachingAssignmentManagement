var termId = $('#term').val(),
    majorId = $('#major').val(),
    rootUrl = $('#loader').data('request-url');

$(function () {
    // Update count on document ready
    updateCount();
});

// Populate select2 for curriculum filter
var curriculumSelect = $('#curriculum'); 
curriculumSelect.wrap('<div class="position-relative my-50"></div>');
curriculumSelect.select2({
    language: 'vi',
    dropdownAutoWidth: true,
    dropdownParent: curriculumSelect.parent(),
    placeholder: curriculumSelect[0][0].innerHTML,
}).on('select2:select', function (e) {
    var curriculumId = $('#' + e.params.data.id);
    curriculumId.show(); 
    curriculumSelect.parent().find('.select2-search__field').attr('placeholder', 'Đã chọn ' + curriculumSelect.val().length);
}).on('select2:unselect', function (e) {
    var curriculumId = $('#' + e.params.data.id);
    curriculumId.hide();
    curriculumSelect.parent().find('.select2-search__field').attr('placeholder', 'Đã chọn ' + curriculumSelect.val().length);
});

// Display message when table have no data
var rowCount = $('#tblAssign tbody tr').length;
if (rowCount == 0) {
    $('#assignLecturerDiv').empty().append('<h4 class="text-center mt-2">Học kỳ này chưa có dữ liệu <i class="feather feather-help-circle"></i></h4><div class="card-body"><img class="mx-auto p-3 d-block w-50" alt="Welcome" src="' + rootUrl + 'assets/images/img_no_data.svg"></div>');
} else {
    // Initialize Tooltip
    $('#tblAssign [data-bs-toggle="tooltip"]').tooltip({
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

// Split lecturerName
$('.assign-card').each(function () {
    var $this = $(this);
    var lecturerId = $this.data('lecturerid');
    var lecturerName = $this.text();
    if (lecturerId != '') {
        $this.text(splitString(lecturerName));
    } else {
        if ($this.hasClass('btn-success')) {
            $this.removeClass('btn-success').addClass('btn-secondary unassigned-theory');
        } else {
            $this.removeClass('btn-warning').addClass('btn-secondary unassigned-practical');
        }
    }
});

$(document).off('click', '.btn-assign').on('click', '.btn-assign', function () {
    $this = $(this);

    // Get values
    var id = $this.data('id'),
        lecturerSelect = $this.parent().find('.select2 :selected'),
        lecturerId = lecturerSelect.val();

    // Send ajax request to check state of lecturer
    $.ajax({
        type: 'GET',
        url: rootUrl + 'Timetable/CheckState',
        data: { id, termId, lecturerId },
        success: function (data) {
            if (data.success) {
                // Call function to assign lecturer
                assignLecturer(id, lecturerId);
            } else {
                // Populate error message into table
                let errorMessage = data.message + '<div class="table-responsive mt-2"><table class="table table-sm"><thead class="text-nowrap"><tr><th></th><th>Mã LHP</th><th>Tên HP</th><th>Thứ</th><th>Tiết</th><th>Phòng</th><th>Ngành</th></tr></thead><tbody>';
                data.classList.forEach(function (item, index) {
                    errorMessage += '<tr class="font-small-3"><td>' + (index + 1) + '</td><td>' + item.classId + '</td><td>' + item.curriculumName + '</td><td class="text-nowrap">' + item.classDay + '</td><td class="text-nowrap">' + item.lessonTime + '</td><td>' + item.roomId + '</td><td>' + item.majorName + '</td></tr>';
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
    var curriculumClassId = $this.parents().find('.popover-header').find('.class-id').text();

    // Show confirm message
    Swal.fire({
        title: 'Thông báo',
        html: '<p class="text-danger mb-0">Hãy nhập lại mã lớp học phần, ' + curriculumClassId + ' để xác nhận xoá.</p>',
        icon: 'warning',
        input: 'text',
        inputAttributes: {
            autocapitalize: 'off'
        },
        showCancelButton: true,
        cancelButtonText: 'Huỷ',
        confirmButtonText: 'Xoá',
        customClass: {
            confirmButton: 'btn btn-primary',
            cancelButton: 'btn btn-outline-danger ms-1'
        },
        buttonsStyling: false,
        preConfirm: (classId) => {
            if (classId === curriculumClassId) {
                // Send ajax request to delete class
                $.ajax({
                    type: 'POST',
                    url: rootUrl + 'Timetable/Delete',
                    data: { id },
                    success: function (data) {
                        if (data.success) {
                            // Show success message
                            toastr.options.positionClass = 'toast-bottom-right';
                            toastr.success('Xoá lớp thành công!');
                        }
                    }
                });
            } else {
                Swal.showValidationMessage('Xác nhận xoá thất bại!');
                return false;
            }
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

// Hide other popovers when user click on table
$('#tblAssign').on('click', function () {
    $('#tblAssign [data-bs-toggle="popover"]').popover('hide');
});

$('.btn-export').on('click', function () {
    var url = rootUrl + 'TimeTable/Export?termId=' + termId + '&majorId=' + majorId;
    window.open(url, '_blank').focus();
});

function splitString(lecturerName) {
    // Split lecture name
    var removeLastWord = lecturerName.substring(0, lecturerName.lastIndexOf(' '));
    var splitName = removeLastWord.split(' ').map(function (item) { return item[0] }).join('.');
    var result = splitName + " " + lecturerName.split(' ').pop();
    return result;
}

function assignLecturer(id, lecturerId) {
    // Send ajax request to assign lecturer
    $.ajax({
        type: 'POST',
        url: rootUrl + 'Timetable/Assign',
        data: { id, lecturerId },
        success: function (data) {
            if (data.success) {
                // Display success message
                toastr.options.positionClass = 'toast-bottom-right';
                toastr.success('Thành công!');
            } else {
                // Show error message
                Swal.fire({
                    title: 'Thông báo',
                    html: data.message,
                    icon: 'error',
                    customClass: {
                        confirmButton: 'btn btn-primary'
                    },
                    buttonsStyling: false
                })
            }
        }
    });
}
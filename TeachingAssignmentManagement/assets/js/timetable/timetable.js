var termId = $('#term').val(),
    majorId = $('#major').val(),
    rootUrl = $('#loader').data('request-url'),
    curriculumFilter = $('#curriculumFilter'),
    lecturerFilter = $('#lecturerFilter');

$(function () {
    // Update count on document ready
    updateCount();

    // Update focus for select2 inside popover
    $(document).on('select2:open', () => {
        document.querySelector('.select2-container--open .select2-search__field').focus();
    });
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

if (majorId == -1) {
    // Append abbreviation text of major name to curriculum name
    $('#tblAssign tbody tr').each(function () {
        var $this = $(this);
        var curriculumId = $this.attr('id');
        var majorAbb = $('.abb-' + curriculumId).first().data('abb');
        $this.find('td:first').append(' (' + majorAbb + ')');
        curriculumFilter.find('option[value="' + curriculumId + '"]').append(' (' + majorAbb + ')');
    });
}

$.fn.select2.amd.define('select2/selectAllAdapter', [
    'select2/utils',
    'select2/dropdown',
    'select2/dropdown/attachBody'
], function (Utils, Dropdown, AttachBody) {

    function SelectAll() { }
    SelectAll.prototype.render = function (decorated) {
        var self = this,
            $rendered = decorated.call(this),
            $selectAll = $(
                '<button class="btn btn-sm text-start" type="button"><i class="feather feather-check-square"></i> Chọn tất cả</button>'
            ),
            $unselectAll = $(
                '<button class="btn btn-sm text-start" type="button"><i class="feather feather-square"></i> Bỏ chọn tất cả</button>'
            ),
            $btnContainer = $('<div class="d-grid"></div>').append($selectAll).append($unselectAll);
        if (!this.$element.prop("multiple")) {
            // this isn't a multi-select -> don't add the buttons!
            return $rendered;
        }
        $rendered.find('.select2-dropdown').prepend($btnContainer);
        $selectAll.on('click', function () {
            hidePopover();
            curriculumFilter.find('option').prop('selected', 'selected').trigger('change'); // Select All Options
            self.trigger('close');
            $('#tblAssign').find('tbody tr').show();
            FilterCount(curriculumFilter);
        });
        $unselectAll.on('click', function () {
            hidePopover();
            curriculumFilter.val(null).trigger('change'); // Unselect All Options
            self.trigger('close');
            $('#tblAssign').find('tbody tr').hide();
            curriculumFilter.parent().find('.select2-search__field').attr('placeholder', 'Lọc môn học');
        });
        return $rendered;
    };

    return Utils.Decorate(
        Utils.Decorate(
            Dropdown,
            AttachBody
        ),
        SelectAll
    );

});

// Populate select2 for curriculum filter
curriculumFilter.find('option').prop('selected', 'selected');
curriculumFilter.wrap('<div class="position-relative my-50 me-1"></div>');
curriculumFilter.select2({
    language: 'vi',
    dropdownAutoWidth: true,
    dropdownParent: curriculumFilter.parent(),
    placeholder: 'Lọc môn học',
    dropdownAdapter: $.fn.select2.amd.require('select2/selectAllAdapter')
})
curriculumFilter.parent().find('.select2-search__field').attr('placeholder', 'Lọc môn học');
curriculumFilter.on('select2:select', function (e) {
    // Show table row on select
    var curriculumId = $('#' + e.params.data.id);
    curriculumId.show();
    FilterCount(curriculumFilter);
}).on('select2:unselect', function (e) {
    // Hide table row on unselect
    var curriculumId = $('#' + e.params.data.id);
    curriculumId.hide();
    FilterCount(curriculumFilter);
});

// Populate select2 for lecturer filter
lecturerFilter.wrap('<div class="position-relative my-50"></div>');
lecturerFilter.select2({
    language: 'vi',
    dropdownAutoWidth: true,
    dropdownParent: lecturerFilter.parent(),
    placeholder: 'Lọc giảng viên',
    allowClear: true
}).on('select2:select', function () {
    // Show all class to filter again
    showAllClass();
    // Filter for curriculum classes which has lecturer
    var $this = $(this),
        tableRow = $('#tblAssign tbody tr'),
        lecturerId = $this.val(),
        lecturerClass = tableRow.find('[data-lecturerid=' + lecturerId + ']'),
        curriculumClass = tableRow.find('.assign-card'),
        curriculumRow = lecturerClass.closest('tr');
    curriculumClass.not(lecturerClass).hide();
    tableRow.not(curriculumRow).hide();
}).on('select2:unselect', function () {
    // Show all class when unselect
    showAllClass();
});

function FilterCount(element) {
    element.parent().find('.select2-search__field').attr('placeholder', 'Đã chọn ' + curriculumFilter.val().length);
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
        lecturerId = lecturerSelect.val()
        warning = true;

    // Send ajax request to check state of lecturer
    $.ajax({
        type: 'GET',
        url: rootUrl + 'Timetable/CheckState',
        data: { id, termId, lecturerId, warning },
        success: function (data) {
            if (data.success) {
                // Call function to assign lecturer
                assignLecturer(id, lecturerId, warning);
            } else if (data.warning) {
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
                    icon: 'warning',
                    showCancelButton: true,
                    cancelButtonText: 'Huỷ',
                    confirmButtonText: 'Phân công',
                    customClass: {
                        confirmButton: 'btn btn-primary',
                        cancelButton: 'btn btn-outline-danger ms-1'
                    },
                    buttonsStyling: false
                }).then((result) => {
                    if (result.isConfirmed) {
                        warning = false;
                        assignLecturer(id, lecturerId, warning);
                    }
                });            
            } else {
                // Populate error message into table
                let errorMessage = data.message + '<div class="table-responsive mt-2"><table class="table table-sm"><thead class="text-nowrap"><tr><th></th><th>Mã LHP</th><th>Tên HP</th><th>Thứ</th><th>Tiết</th><th>Ngành</th></tr></thead><tbody>';
                data.classList.forEach(function (item, index) {
                    errorMessage += '<tr class="font-small-3"><td>' + (index + 1) + '</td><td>' + item.classId + '</td><td>' + item.curriculumName + '</td><td class="text-nowrap">' + item.classDay + '</td><td class="text-nowrap">' + item.lessonTime + '</td><td>' + item.majorName + '</td></tr>';
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
        }
        var lecturerId = $(this).attr('data-lecturerid');
        formSelect.val(lecturerId).trigger('change');
    }, 0);
});

// Hide other popovers when user click on table
$('#tblAssign').on('click', function () {
    hidePopover();
});

$('.btn-export').on('click', function () {
    var url = rootUrl + 'TimeTable/Export?termId=' + termId + '&majorId=' + majorId;
    window.open(url, '_blank').focus();
});

function showAllClass() {
    hidePopover();
    $('#tblAssign tbody tr').show();
    $('.assign-card').show();
}

function hidePopover() {
    $('#tblAssign [data-bs-toggle="popover"]').popover('hide');
}

function splitString(lecturerName) {
    // Split lecture name
    var removeLastWord = lecturerName.substring(0, lecturerName.lastIndexOf(' '));
    var splitName = removeLastWord.split(' ').map(function (item) { return item[0] }).join('.');
    var result = splitName + " " + lecturerName.split(' ').pop();
    return result;
}

function assignLecturer(id, lecturerId, warning) {
    // Send ajax request to assign lecturer
    $.ajax({
        type: 'POST',
        url: rootUrl + 'Timetable/Assign',
        data: { id, lecturerId, warning },
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
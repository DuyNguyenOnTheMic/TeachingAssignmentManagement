﻿var termId = $('#term').val(),
  majorId = $('#major').val(),
  rootUrl = $('#loader').data('request-url'),
  termStatus = $('#termData').data('status'),
  subjectFilter = $('#subjectFilter'),
  lecturerFilter = $('#lecturerFilter'),
  rowCount = $('#tblAssign tbody tr').length

$(function () {
  // Update count on document ready
  updateClassCount()
  updateTotalCount()
  $('#subjectCount').text(rowCount)

  // Update focus for select2 inside popover
  $(document).on('select2:open', () => {
    document.querySelector('.select2-container--open .select2-search__field').focus()
  })
})

// Display message when table have no data
if (rowCount == 0) {
  showNoData(assignLecturerDiv, '<i class="feather feather-help-circle"></i>')
} else {
  // Initialize Tooltip
  $('#tblAssign [data-bs-toggle="tooltip"]').tooltip({
    trigger: 'hover'
  })

  // Initialize Popover
  var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'))
  var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
    return new bootstrap.Popover(popoverTriggerEl, {
      html: true,
      sanitize: false
    })
  })
}

if (majorId == -1) {
  // Append abbreviation text of major name to subject name
  $('#tblAssign tbody tr').each(function () {
    var $this = $(this)
    var subjectId = $this.attr('id')
    var majorAbb = $this.data('abb')
    $this.find('td:first').append(' (' + majorAbb + ')')
    subjectFilter.find('option[value="' + subjectId + '"]').append(' (' + majorAbb + ')')
  })
}

$.fn.select2.amd.define(
  'select2/selectAllAdapter',
  ['select2/utils', 'select2/dropdown', 'select2/dropdown/attachBody'],
  function (Utils, Dropdown, AttachBody) {
    function SelectAll() {}
    SelectAll.prototype.render = function (decorated) {
      var self = this,
        $rendered = decorated.call(this),
        $selectAll = $(
          '<button class="btn btn-sm text-start filter-button" type="button"><i class="feather feather-check-square"></i> Chọn tất cả</button>'
        ),
        $unselectAll = $(
          '<button class="btn btn-sm text-start filter-button" type="button"><i class="feather feather-square"></i> Bỏ chọn tất cả</button>'
        ),
        $btnContainer = $('<div class="d-grid"></div>').append($selectAll).append($unselectAll)
      if (!this.$element.prop('multiple')) {
        // this isn't a multi-select -> don't add the buttons!
        return $rendered
      }
      $rendered.find('.select2-dropdown').prepend($btnContainer)
      $selectAll.on('click', function () {
        hidePopover()
        subjectFilter.find('option').prop('selected', 'selected').trigger('change') // Select All Options of subject filter
        filterCount(subjectFilter)
        if (self.$element.attr('id') == 'lecturerFilter') {
          lecturerFilter.find('option').prop('selected', 'selected').trigger('change') // Select All Options of lecturer filter
          filterCount(lecturerFilter)
          $('.assign-card').show()
        }
        self.trigger('close')
        $('#tblAssign tbody tr').show()
        updateClassCount()
      })
      $unselectAll.on('click', function () {
        hidePopover()
        if (self.$element.attr('id') == 'subjectFilter') {
          subjectFilter.val(null).trigger('change') // Unselect All Options of subject filter
          subjectFilter.parent().find('.select2-search__field').attr('placeholder', 'Lọc môn học')
        } else if (self.$element.attr('id') == 'lecturerFilter') {
          lecturerFilter.val('-1').trigger('change') // Unselect All Options of lecturer filter
          lecturerFilter.parent().find('.select2-search__field').attr('placeholder', 'Lọc giảng viên')
        }
        self.trigger('close')
        $('#tblAssign tbody tr').hide()
        updateClassCount()
      })
      return $rendered
    }

    return Utils.Decorate(Utils.Decorate(Dropdown, AttachBody), SelectAll)
  }
)

// Populate select2 for subject filter
subjectFilter.find('option').prop('selected', 'selected')
subjectFilter.wrap('<div class="position-relative my-50 me-1"></div>')
subjectFilter.select2({
  language: 'vi',
  dropdownAutoWidth: true,
  dropdownParent: subjectFilter.parent(),
  placeholder: 'Lọc môn học',
  dropdownAdapter: $.fn.select2.amd.require('select2/selectAllAdapter')
})
subjectFilter.parent().find('.select2-search__field').attr('placeholder', 'Lọc môn học')
subjectFilter
  .on('select2:select', function (e) {
    // Show table row on select
    var subjectId = $('#' + e.params.data.id)
    subjectId.show()
    filterCount(subjectFilter)
    updateClassCount()
  })
  .on('select2:unselect', function (e) {
    // Hide table row on unselect
    var subjectId = $('#' + e.params.data.id)
    subjectId.hide()
    filterCount(subjectFilter)
    updateClassCount()
  })

// Populate select2 for lecturer filter
lecturerFilter.find('option').prop('selected', 'selected')
lecturerFilter.wrap('<div class="position-relative my-50"></div>')
lecturerFilter.select2({
  language: 'vi',
  dropdownAutoWidth: true,
  dropdownParent: lecturerFilter.parent(),
  dropdownAdapter: $.fn.select2.amd.require('select2/selectAllAdapter')
})
lecturerFilter.parent().find('.select2-search__field').attr('placeholder', 'Lọc giảng viên')
lecturerFilter
  .on('select2:select', function (e) {
    var tableRow = $('#tblAssign tbody tr'),
      lecturerId = e.params.data.id,
      lecturerClass = tableRow.find('[data-lecturerid="' + lecturerId + '"]')
    hidePopover()
    lecturerClass.show()
    filterCount(lecturerFilter)
    // Show error message when lecturer class is not found
    if (!lecturerClass.length) {
      toastr.info('Giảng viên chưa được phân giảng lớp nào!')
    } else {
      if ($('#tblAssign tbody tr:visible').length > 0) {
        // Filter for subject classes which has lecturer
        tableRow.show()
        updateRow(tableRow)
      } else {
        // Filter from beginning when user deselects all options
        tableRow.show()
        $('#tblAssign .assign-card').not(lecturerClass).hide()
        tableRow.not(lecturerClass.closest('tr')).hide()
        updateRow(tableRow)
      }
    }
    updateClassCount()
  })
  .on('select2:unselect', function (e) {
    var tableRow = $('#tblAssign tbody tr'),
      lecturerId = e.params.data.id,
      lecturerClass = tableRow.find('[data-lecturerid="' + lecturerId + '"]')
    hidePopover()
    filterCount(lecturerFilter)
    if (lecturerClass.length) {
      lecturerClass.hide()
      updateRow(tableRow)
    }
    updateClassCount()
  })

// Split lecturerName
$('.assign-card').each(function () {
  var $this = $(this)
  var lecturerId = $this.data('lecturerid')
  var lecturerName = $this.text()
  if (lecturerId != '') {
    $this.text(splitString(lecturerName))
  } else {
    if ($this.hasClass('btn-success')) {
      $this.removeClass('btn-success').addClass('btn-secondary unassigned-theory')
    } else {
      $this.removeClass('btn-warning').addClass('btn-secondary unassigned-practical')
    }
  }
})

$(document)
  .off('click', '.btn-assign')
  .on('click', '.btn-assign', function () {
    $this = $(this)

    // Get values
    var id = $this.data('id'),
      lecturerSelect = $this.parent().find('.select2 :selected'),
      lecturerId = lecturerSelect.val(),
      warning = true

    // Send ajax request to check state of lecturer
    $.ajax({
      type: 'GET',
      url: rootUrl + 'Timetable/CheckState',
      data: { id, termId, lecturerId, warning },
      success: function (data) {
        if (data.success) {
          // Call function to assign lecturer
          assignLecturer(id, lecturerId, warning)
        } else if (data.warning) {
          // Populate error message into table
          let errorMessage =
            data.message +
            ' Bạn có chắc muốn phân công?' +
            '<div class="table-responsive mt-2"><table class="table table-sm"><thead class="text-nowrap"><tr><th></th><th>Mã LHP</th><th>Tên HP</th><th>Thứ</th><th>Tiết</th><th>Phòng</th><th>Ngành</th></tr></thead><tbody>'
          data.classList.forEach(function (item, index) {
            errorMessage +=
              '<tr class="font-small-3"><td>' +
              (index + 1) +
              '</td><td>' +
              item.classId +
              '</td><td>' +
              item.subjectName +
              '</td><td class="text-nowrap">' +
              item.classDay +
              '</td><td class="text-nowrap">' +
              item.lessonTime +
              '</td><td>' +
              item.roomId +
              '</td><td>' +
              item.majorName +
              '</td></tr>'
          })
          errorMessage += '</tbody></table></div>'
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
          }).then(result => {
            if (result.isConfirmed) {
              warning = false
              assignLecturer(id, lecturerId, warning)
            }
          })
        } else {
          // Populate error message into table
          let errorMessage =
            data.message +
            '<div class="table-responsive mt-2"><table class="table table-sm"><thead class="text-nowrap"><tr><th></th><th>Mã LHP</th><th>Tên HP</th><th>Thứ</th><th>Tiết</th><th>Ngành</th></tr></thead><tbody>'
          data.classList.forEach(function (item, index) {
            errorMessage +=
              '<tr class="font-small-3"><td>' +
              (index + 1) +
              '</td><td>' +
              item.classId +
              '</td><td>' +
              item.subjectName +
              '</td><td class="text-nowrap">' +
              item.classDay +
              '</td><td class="text-nowrap">' +
              item.lessonTime +
              '</td><td>' +
              item.majorName +
              '</td></tr>'
          })
          errorMessage += '</tbody></table></div>'
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
    })
  })

$(document)
  .off('click', '.btn-delete')
  .on('click', '.btn-delete', function () {
    $this = $(this)

    // Get values
    var id = $this.data('id')
    var subjectClassId = $this.parents().find('.popover-header').find('.class-id').text()

    // Show confirm message
    Swal.fire({
      title: 'Thông báo',
      html: '<p class="text-danger mb-0">Hãy nhập lại mã lớp học phần, ' + subjectClassId + ' để xác nhận xoá.</p>',
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
      preConfirm: classId => {
        if (classId === subjectClassId) {
          // Send ajax request to delete class
          $.ajax({
            type: 'POST',
            url: rootUrl + 'Timetable/Delete',
            data: { id },
            success: function (data) {
              if (data.success) {
                // Show success message
                toastr.success('Xoá lớp thành công!')
              }
            }
          })
        } else {
          Swal.showValidationMessage('Xác nhận xoá thất bại!')
          return false
        }
      }
    })
  })

$('.assign-card')
  .on('click', function () {
    // Hide other popovers when a popover is clicked
    $('[data-bs-toggle="popover"]').not(this).popover('hide')
  })
  .on('show.bs.popover', function () {
    setTimeout(() => {
      var formSelect = $('.popover-body .form-select')
      if (!$(formSelect).hasClass('select2-hidden-accessible')) {
        // Apply select2
        populateSelect(formSelect)
      }
      var lecturerId = $(this).attr('data-lecturerid')
      formSelect.val(lecturerId).trigger('change')

      // Check if term status is false
      if (termStatus == 'False') {
        var formButton = $('.popover-body button')

        // Disable form select and buttons
        formSelect.prop('disabled', true)

        // Add on click event in case user remove disabled attribute
        formButton.off('click').on('click', function () {
          toastr.warning('Học kỳ này đã được khoá phân công!')
          return false
        })
      }
    }, 0)
  })

// Hide other popovers when user click on table
$('#tblAssign').on('click', function () {
  hidePopover()
})

$('.btn-export').on('click', function () {
  var url = rootUrl + 'TimeTable/Export?termId=' + termId + '&majorId=' + majorId
  window.open(url, '_blank').focus()
})

function updateRow(tableRow) {
  tableRow.each(function () {
    var $this = $(this)
    if ($this.find('.assign-card:visible').length == 0) {
      $this.closest('tr').hide()
    } else {
      $this.closest('tr').show()
    }
  })
}

function filterCount(element) {
  var filterText
  if (element.attr('id') == 'subjectFilter') {
    filterText = 'môn'
  } else if (element.attr('id') == 'lecturerFilter') {
    filterText = 'GV'
  }
  element
    .parent()
    .find('.select2-search__field')
    .attr('placeholder', 'Đã chọn ' + element.val().length + ' ' + filterText)
}

function hidePopover() {
  $('#tblAssign [data-bs-toggle="popover"]').popover('hide')
}

function splitString(lecturerName) {
  // Split lecture name
  var removeLastWord = lecturerName.substring(0, lecturerName.lastIndexOf(' '))
  var splitName = removeLastWord
    .split(' ')
    .map(function (item) {
      return item[0]
    })
    .join('.')
  var result = splitName + ' ' + lecturerName.split(' ').pop()
  return result
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
        toastr.success('Thành công!')
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
  })
}

﻿var select = $('.select2'),
  dropzone = $('#dpz-single-file'),
  isUpdate = $('#isUpdate'),
  isCheckStudentNumber = $('#isCheckStudentNumber'),
  rootUrl = $('#loader').data('request-url'),
  errorLecturersSection = $('#errorLecturers-section'),
  differentCampusSection = $('#differentCampus-section')

// Populate select2 for choosing term and major
select.each(function () {
  var $this = $(this)
  $this.wrap('<div class="position-relative"></div>')
  $this.select2({
    language: 'vi',
    dropdownAutoWidth: true,
    dropdownParent: $this.parent(),
    placeholder: $this[0][0].innerHTML
  })
})

// Populate Dropzone
dropzone.dropzone({
  url: rootUrl + 'Timetable/Import',
  autoProcessQueue: false,
  paramName: 'postedFile',
  timeout: null,
  maxFiles: 1,
  maxFilesize: 50,
  acceptedFiles: '.xlsx, .xls',
  dictFileTooBig: 'Tệp quá lớn ({{filesize}}MB). Kích thước tối đa: {{maxFilesize}}MB.',
  dictInvalidFileType: 'Tệp tin sai định dạng, chỉ được upload file excel',
  success: function (response, data) {
    // Do actions after Dropzone import Succeeded
    importSucceeded(data)
  },
  init: function () {
    // Using a closure
    var myDropzone = this

    this.on('addedfile', function (file) {
      dropzone.find('.dz-progress').addClass('d-none')
    })

    this.on('maxfilesexceeded', function (file) {
      // Remove file and add again if user input more than 1
      myDropzone.removeAllFiles()
      myDropzone.addFile(file)
    })

    $('#submit-all').click(function (e) {
      // Prevent dropzone from auto submit file
      e.preventDefault()
      e.stopPropagation()

      var count = myDropzone.getQueuedFiles().length

      var termId = $('#term').val()
      var majorId = $('#major').val()

      // Validation for form
      if (termId == '' && majorId == '') {
        toastr.warning('Bạn chưa chọn học kỳ và ngành')
      } else if (termId == '') {
        toastr.warning('Bạn chưa chọn học kỳ')
      } else if (majorId == '') {
        toastr.warning('Bạn chưa chọn ngành')
      } else if (count == 0) {
        toastr.warning('File chưa được upload hoặc sai định dạng ')
      } else {
        dropzone.find('.dz-progress').removeClass('d-none')
        // Begin to import file
        myDropzone.processQueue()
      }
    })

    this.on('sending', function (data, xhr, formData) {
      $('.form-data').each(function () {
        // Send form data along with submit
        formData.append($(this).attr('name'), $(this).val())
      })

      Swal.fire({
        title: 'Vui lòng đợi...',
        allowEscapeKey: false,
        allowOutsideClick: false,
        html: '<div class="progress progress-bar-primary my-2" style="height: 30px"><div id="myProgress" class="progress-bar progress-bar-striped progress-bar-animated" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100"></div></div>',
        didOpen: () => {
          Swal.showLoading()
        }
      })

      startProgressBar()

      // Show confirmation message when user closes tab
      window.onbeforeunload = function () {
        return 'Bạn có chắc muốn thoát, tiến trình import có thể sẽ không được lưu?'
      }
    })

    this.on('error', function (data, errorMessage, xhr) {
      Swal.close()
      isCheckStudentNumber.val(true)
      errorLecturersSection.hide()
      differentCampusSection.hide()
      window.onbeforeunload = null

      if (xhr) {
        var errorDisplay = document.querySelectorAll('[data-dz-errormessage]')
        errorDisplay[errorDisplay.length - 1].innerHTML = 'Đã xảy ra lỗi! Vui lòng kiểm tra và chọn lại tệp tin.'
        if (xhr.status == 417) {
          isUpdate.val(false)
          Swal.fire({
            title: 'Thông báo',
            html: errorMessage,
            icon: 'error',
            customClass: {
              confirmButton: 'btn btn-primary'
            },
            buttonsStyling: false
          })
        } else if (xhr.status == 409) {
          isUpdate.val(false)
          Swal.fire({
            title: 'Thông báo',
            text: errorMessage,
            icon: 'question',
            showDenyButton: true,
            showCancelButton: true,
            confirmButtonText: 'Cập nhật',
            denyButtonText: 'Thay thế',
            cancelButtonText: 'Huỷ',
            customClass: {
              confirmButton: 'btn btn-primary',
              denyButton: 'btn btn-success ms-1',
              cancelButton: 'btn btn-outline-danger ms-1'
            },
            buttonsStyling: false
          }).then(result => {
            if (result.isConfirmed) {
              // Update timetable
              importAgain(myDropzone, true)
            } else if (result.isDenied) {
              // Delete and import timetable again
              deleteAndImport(myDropzone)
            }
          })
        } else if (xhr.status == 400) {
          Swal.fire({
            title: 'Thông báo',
            text: errorMessage,
            icon: 'warning',
            showCancelButton: true,
            cancelButtonText: 'Huỷ',
            confirmButtonText: 'Import tiếp',
            customClass: {
              confirmButton: 'btn btn-primary',
              cancelButton: 'btn btn-outline-danger ms-1'
            },
            buttonsStyling: false
          }).then(result => {
            if (result.isConfirmed) {
              // Keep importing the timetable
              isCheckStudentNumber.val(false)
              if (isUpdate.val() === 'true') {
                importAgain(myDropzone, true)
              } else {
                deleteAndImport(myDropzone)
              }
            }
          })
        }
      }
    })
  }
})

function startProgressBar() {
  // Reference the auto-generated proxy for the hub.
  var progress = $.connection.progressHub

  // Create a function that the hub can call back to display messages.
  progress.client.addProgress = function (message, percentage) {
    if (percentage >= 100) {
      return
    } else {
      $('#myProgress')
        .attr({ 'aria-valuenow': percentage })
        .text(message + ' ' + percentage + ' %')
        .width(percentage)
    }
  }

  $.connection.hub.start()
}

function importSucceeded(data) {
  // Reset state of isUpdate
  isUpdate.val(false)

  if (data.length) {
    var errorLecurersFilter = data.filter(x => x.Item7 === false),
      differentCampusFilter = data.filter(x => x.Item7 === true)
    // If has data, then show section
    if (errorLecurersFilter.length) {
      errorLecturersSection.show()
      populateErrorLecturersDatatable(errorLecurersFilter)
    }
    if (differentCampusFilter.length) {
      differentCampusSection.show()
      populateDifferentCampusDatatable(differentCampusFilter)
    }

    var message
    if (data[0].Item6) {
      // Show assign lecturer error list
      message =
        'Đã import dữ liệu! \nCó một số giảng viên có lịch giảng dạy bị trùng, vui lòng xem chi tiết ở cuối trang.'
      setVisibleColumn(true)
      setTimeout(function () {
        errorLecturersSection.find('.alert').hide()
        errorLecturersSection.find('.importUser').hide()
      }, 100)
    } else {
      // Show lecturer that hasn't been in the system yet
      errorLecturersSection.show()
      populateErrorLecturersDatatable(data)
      message = 'Đã import dữ liệu! \nCó một số giảng viên chưa có trong hệ thống, vui lòng xem chi tiết ở cuối trang.'
      setVisibleColumn(false)
      setTimeout(function () {
        errorLecturersSection.find('.alert').show()
        errorLecturersSection.find('.importUser').show()
      }, 100)
    }

    Swal.fire({
      title: 'Thông báo',
      text: message,
      icon: 'error',
      customClass: {
        confirmButton: 'btn btn-primary'
      },
      buttonsStyling: false
    })
  } else {
    Swal.fire({
      title: 'Thông báo',
      text: 'Import thời khoá biểu thành công!',
      icon: 'success',
      customClass: {
        confirmButton: 'btn btn-primary'
      },
      buttonsStyling: false
    })
    // Hide section
    errorLecturersSection.hide()
    differentCampusSection.hide()
  }
  window.onbeforeunload = null
}

function setVisibleColumn(state) {
  var table = $('#tblErrorLecturers').DataTable()
  for (var i = 3; i <= 6; i++) {
    table.column(i).visible(state, state)
  }
  table.columns.adjust().draw(state) // adjust column sizing and redraw
}

function importAgain(myDropzone, state) {
  $.each(myDropzone.files, function (i, file) {
    // Add file to Dropzone again
    file.status = Dropzone.QUEUED
    file.previewElement.classList.remove('dz-error')
    return file.previewElement.classList.add('dz-success')
  })
  // Set state for isUpdate
  isUpdate.val(state)
  // Process import
  myDropzone.processQueue()
}

function deleteAndImport(myDropzone) {
  // Show waiting message while delete
  Swal.fire({
    title: 'Đang xoá dữ liệu...',
    allowEscapeKey: false,
    allowOutsideClick: false,
    didOpen: () => {
      Swal.showLoading()
    }
  })

  var term = $('#term').val()
  var major = $('#major').val()

  // Send ajax request to delete all classes
  $.ajax({
    type: 'POST',
    url: rootUrl + 'Timetable/DeleteAll',
    data: { term, major },
    success: function (data) {
      if (data.success) {
        Swal.close()

        // Import timetable again
        importAgain(myDropzone, false)
      }
    }
  })
}

function populateErrorLecturersDatatable(data) {
  var dataTable
  var fileName = $('#lblErrorLecturers').text()

  if (!$.fn.DataTable.isDataTable('#tblErrorLecturers')) {
    // Populate Error lecturers datatable
    dataTable = $('#tblErrorLecturers').DataTable({
      columns: [
        { data: '', defaultContent: '' },
        { data: 'Item1' },
        { data: 'Item2' },
        { data: 'Item3', defaultContent: '' },
        { data: 'Item4', defaultContent: '' },
        { data: 'Item5', defaultContent: '' },
        { data: 'Item6', defaultContent: '' }
      ],

      columnDefs: [
        {
          searchable: false,
          orderable: false,
          className: 'text-center',
          width: '5%',
          targets: 0
        }
      ],
      order: [[1, 'asc']],
      dom: '<"d-flex justify-content-between align-items-center header-actions mx-2 row mt-75"<"col-sm-12 col-lg-4 d-flex justify-content-center justify-content-lg-start" l><"col-sm-12 col-lg-8 ps-xl-75 px-0"<"dt-action-buttons d-flex align-items-center justify-content-center justify-content-lg-end flex-lg-nowrap flex-wrap"<"me-1"f>B>>>t<"d-flex justify-content-between mx-2 row mb-1"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>',
      displayLength: 10,
      lengthMenu: [
        [10, 25, 50, -1],
        [10, 25, 50, 'tất cả']
      ],
      buttons: [
        {
          extend: 'collection',
          className: 'btn btn-outline-secondary dropdown-toggle me-2',
          text: feather.icons['share'].toSvg({ class: 'font-small-4 me-50' }) + 'Export',
          buttons: [
            {
              extend: 'print',
              className: 'dropdown-item',
              title: fileName
            },
            {
              extend: 'excel',
              className: 'dropdown-item',
              title: fileName
            },
            {
              extend: 'pdf',
              className: 'dropdown-item',
              title: fileName
            },
            {
              extend: 'copy',
              className: 'dropdown-item',
              title: fileName
            }
          ],
          init: function (api, node, config) {
            $(node).removeClass('btn-secondary')
            $(node).parent().removeClass('btn-group')
            setTimeout(function () {
              $(node).closest('.dt-buttons').removeClass('btn-group').addClass('d-inline-flex mt-50')
            }, 50)
          }
        },
        {
          text: feather.icons['plus'].toSvg({ class: 'me-50 font-small-4' }) + 'Import vào hệ thống',
          className: 'importUser btn btn-primary',
          attr: {
            onclick: 'importUsers()'
          }
        }
      ],

      language: {
        url: rootUrl + 'app-assets/language/datatables/vi.json'
      }
    })
  }

  // Update current datatable
  dataTable = $('#tblErrorLecturers').DataTable()
  dataTable.clear().rows.add(data).draw()

  // Create Index column datatable
  dataTable.on('draw.dt', function () {
    dataTable
      .column(0, { search: 'applied', order: 'applied' })
      .nodes()
      .each(function (cell, i) {
        cell.innerHTML = i + 1
        dataTable.cell(cell).invalidate('dom')
      })
  })
}

function populateDifferentCampusDatatable(data) {
  var dataTable
  var fileName = $('#lblDifferentCampus').text()

  if (!$.fn.DataTable.isDataTable('#tblDifferentCampus')) {
    // Populate different campus datatable
    dataTable = $('#tblDifferentCampus').DataTable({
      columns: [
        { data: '', defaultContent: '' },
        { data: 'Item1' },
        { data: 'Item2' },
        { data: 'Item3', defaultContent: '' },
        { data: 'Item4', defaultContent: '' },
        { data: 'Item5', defaultContent: '' }
      ],

      columnDefs: [
        {
          searchable: false,
          orderable: false,
          className: 'text-center',
          width: '5%',
          targets: 0
        }
      ],
      order: [[1, 'asc']],
      dom: '<"d-flex justify-content-between align-items-center header-actions mx-2 row mt-75"<"col-sm-12 col-lg-4 d-flex justify-content-center justify-content-lg-start" l><"col-sm-12 col-lg-8 ps-xl-75 px-0"<"dt-action-buttons d-flex align-items-center justify-content-center justify-content-lg-end flex-lg-nowrap flex-wrap"<"me-1"f>B>>>t<"d-flex justify-content-between mx-2 row mb-1"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>',
      displayLength: 10,
      lengthMenu: [
        [10, 25, 50, -1],
        [10, 25, 50, 'tất cả']
      ],
      buttons: [
        {
          extend: 'collection',
          className: 'btn btn-outline-secondary dropdown-toggle me-2',
          text: feather.icons['share'].toSvg({ class: 'font-small-4 me-50' }) + 'Export',
          buttons: [
            {
              extend: 'print',
              className: 'dropdown-item',
              title: fileName
            },
            {
              extend: 'excel',
              className: 'dropdown-item',
              title: fileName
            },
            {
              extend: 'pdf',
              className: 'dropdown-item',
              title: fileName
            },
            {
              extend: 'copy',
              className: 'dropdown-item',
              title: fileName
            }
          ],
          init: function (api, node, config) {
            $(node).removeClass('btn-secondary')
            $(node).parent().removeClass('btn-group')
            setTimeout(function () {
              $(node).closest('.dt-buttons').removeClass('btn-group').addClass('d-inline-flex mt-50')
            }, 50)
          }
        }
      ],

      language: {
        url: rootUrl + 'app-assets/language/datatables/vi.json'
      }
    })
  }

  // Update current datatable
  dataTable = $('#tblDifferentCampus').DataTable()
  dataTable.clear().rows.add(data).draw()

  // Create Index column datatable
  dataTable.on('draw.dt', function () {
    dataTable
      .column(0, { search: 'applied', order: 'applied' })
      .nodes()
      .each(function (cell, i) {
        cell.innerHTML = i + 1
        dataTable.cell(cell).invalidate('dom')
      })
  })
}

function importUsers() {
  // Show waiting message while import
  Swal.fire({
    title: 'Đang import giảng viên...',
    allowEscapeKey: false,
    allowOutsideClick: false,
    html: '<div class="progress progress-bar-primary my-2" style="height: 30px"><div id="myProgress" class="progress-bar progress-bar-striped progress-bar-animated" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100"></div></div>',
    didOpen: () => {
      Swal.showLoading()
    }
  })

  startProgressBar()

  // Show confirmation message when user closes tab
  window.onbeforeunload = function () {
    return 'Bạn có chắc muốn thoát, tiến trình import có thể sẽ không được lưu?'
  }

  // Get data from datatables
  var data = $('#tblErrorLecturers').DataTable().rows().data().toArray()
  var lecturerId = $(data)
    .map(function () {
      return this.Item1
    })
    .get()
  var lecturerName = $(data)
    .map(function () {
      return this.Item2
    })
    .get()

  // Send ajax request to import users
  $.ajax({
    type: 'POST',
    url: rootUrl + 'User/Import',
    data: { lecturerId, lecturerName },
    success: function (data) {
      window.onbeforeunload = null

      if (data.success) {
        // Hide error lecturers section
        errorLecturersSection.hide()
        differentCampusSection.hide()

        // Show confirm message
        Swal.fire({
          title: 'Thông báo',
          text: 'Import giảng viên thành công, bạn có muốn import lại file không?',
          icon: 'question',
          showCancelButton: true,
          confirmButtonText: 'Import lại',
          cancelButtonText: 'Huỷ',
          customClass: {
            confirmButton: 'btn btn-primary',
            cancelButton: 'btn btn-outline-danger ms-1'
          },
          buttonsStyling: false
        }).then(result => {
          if (result.isConfirmed) {
            // Delete and import timetable again
            var myDropzone = Dropzone.forElement('#dpz-single-file')
            deleteAndImport(myDropzone)
          }
        })
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

﻿var popup, dataTable

// Setup data
var dataLoader = $('#data-loader'),
  termId = dataLoader.data('termid')

$(function () {
  // Populate LecturerRank datatable
  dataTable = $('#tblLecturerRank').DataTable({
    ajax: {
      url: rootUrl + 'LecturerRank/GetData?termId=' + termId,
      type: 'GET',
      dataType: 'json',
      dataSrc: ''
    },
    deferRender: true,
    columns: [
      { data: '', defaultContent: '' },
      { data: 'StaffId' },
      { data: 'FullName' },
      { data: 'AcademicDegreeRankId' },
      {
        data: 'Id',
        render: function (data, type, full, meta) {
          // Set url action to be create or edit lecturer rank
          var actionUrl
          var $lecturerId = full['LecturerId']
          if (data) {
            actionUrl = rootUrl + 'LecturerRank/Edit/' + data
          } else {
            actionUrl = rootUrl + 'LecturerRank/Create?termId=' + termId + '&lecturerId=' + $lecturerId
          }
          return (
            "<a class='editRow text-success p-0' data-original-title='Chỉnh sửa' title='Chỉnh sửa' onclick=popupForm('" +
            actionUrl +
            "')><i class='feather feather-edit font-medium-3 me-1'></i></a>"
          )
        }
      }
    ],

    columnDefs: [
      {
        searchable: false,
        orderable: false,
        targets: [0, 4]
      },
      { className: 'text-center', targets: [0, 3, 4] },
      { width: '5%', targets: 0 },
      { width: '10%', targets: 4 }
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
        text: feather.icons['edit-2'].toSvg({ class: 'me-50 font-small-4' }) + 'Sửa tất cả',
        className: 'editAll btn btn-primary',
        attr: {
          onclick: "popupForm('" + rootUrl + 'LecturerRank/EditAll?termId=' + termId + "')"
        },
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

  // Create Index column datatable
  dataTable.on('draw.dt', function () {
    dataTable
      .column(0, { search: 'applied', order: 'applied' })
      .nodes()
      .each(function (cell, i) {
        cell.innerHTML = i + 1
      })

    // Prevent user from add edit delete while dialog is populated
    if ($('.ui-dialog-content').dialog('isOpen') === true) {
      disableButtons(true)
    } else {
      disableButtons(false)
    }
  })
})

function disableButtons(state) {
  if (state === true) {
    // disable buttons
    $('.editAll').prop('disabled', true)
    $('.editRow').each(function () {
      this.style.pointerEvents = 'none'
    })
  } else {
    // enable buttons
    $('.editAll').prop('disabled', false)
    $('.editRow').each(function () {
      this.style.pointerEvents = 'auto'
    })
  }
}

// Show Edit form
function popupForm(url) {
  var formDiv = $('<div/>')
  $.get(url).done(function (response) {
    formDiv.html(response)

    popup = formDiv.dialog({
      autoOpen: true,
      resizable: false,
      title: 'Quản lý cấp bậc giảng viên',
      width: 550,
      open: function () {
        // Add close icon class
        $(this).closest('.ui-dialog').find('.ui-dialog-titlebar-close').addClass('btn-close')

        // Prevent user from add edit delete while dialog is populated
        disableButtons(true)
      },
      close: function () {
        popup.dialog('destroy').remove()

        // Re-enable buttons when user closes the dialog
        disableButtons(false)
      }
    })
  })
}

function submitForm(form) {
  $.validator.unobtrusive.parse(form)

  if ($(form).valid()) {
    $.ajax({
      type: 'POST',
      url: form.action,
      data: $(form).serialize(),
      success: function (data) {
        if (data.success) {
          popup.dialog('close')
          dataTable.ajax.reload(null, false)

          // Show message when create or edit succeeded
          toastr['success'](data.message)
        } else {
          // Show message when create failed
          Swal.fire({
            title: 'Thông báo',
            text: data.message,
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
  return false
}

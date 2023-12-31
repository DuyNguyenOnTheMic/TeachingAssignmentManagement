﻿var popup, dataTable
var rootUrl = $('#loader').data('request-url')

$(function () {
  'use strict'

  var groupColumn = 1
  // Populate Academic Degree Rank datatable
  dataTable = $('#tblAcademicDegreeRank').DataTable({
    ajax: {
      url: rootUrl + 'AcademicDegreeRank/GetData',
      type: 'GET',
      dataType: 'json',
      dataSrc: ''
    },
    deferRender: true,
    columns: [
      { data: 'id' },
      { data: 'group' },
      {
        data: 'id',
        render: function (data) {
          return (
            "<a class='editRow text-success p-0' data-original-title='Chỉnh sửa' title='Chỉnh sửa' onclick=popupForm('" +
            rootUrl +
            'AcademicDegreeRank/Edit/' +
            data +
            "')><i class='feather feather-edit font-medium-3 me-1'></i></a><a class='deleteRow text-danger p-0' data-original-title='Xoá' title='Xoá' onclick=deleteAcademicDegreeRank('" +
            data +
            "') ><i class='feather feather-trash-2 font-medium-3 me-1'></i></a>"
          )
        }
      }
    ],

    columnDefs: [
      {
        searchable: false,
        orderable: false,
        className: 'text-center',
        targets: 2
      },
      { visible: false, targets: groupColumn },
      { className: 'ps-5', targets: 0 },
      { width: '15%', targets: 2 }
    ],
    order: [[groupColumn, 'desc']],
    dom: '<"d-flex justify-content-between align-items-center header-actions mx-2 row mt-75"<"col-sm-12 col-lg-4 d-flex justify-content-center justify-content-lg-start" l><"col-sm-12 col-lg-8 ps-xl-75 px-0"<"dt-action-buttons d-flex align-items-center justify-content-center justify-content-lg-end flex-lg-nowrap flex-wrap"<"me-1"f>B>>>t<"d-flex justify-content-between mx-2 row mb-1"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>',
    displayLength: 25,
    lengthMenu: [
      [25, 50, -1],
      [25, 50, 'tất cả']
    ],
    buttons: [
      {
        text: feather.icons['plus'].toSvg({ class: 'me-50 font-small-4' }) + 'Thêm cấp bậc mới',
        className: 'createNew btn btn-primary',
        attr: {
          onclick: "popupForm('" + rootUrl + "AcademicDegreeRank/Create')"
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
    drawCallback: function (settings) {
      var api = this.api()
      var rows = api.rows({ page: 'current' }).nodes()
      var last = null

      api
        .column(groupColumn, { page: 'current' })
        .data()
        .each(function (group, i) {
          if (last !== group) {
            $(rows)
              .eq(i)
              .before('<tr class="group"><td class="fw-bold" colspan="2">' + group + '</td></tr>')

            last = group
          }
        })
    },

    language: {
      url: rootUrl + 'app-assets/language/datatables/vi.json'
    }
  })

  // Create Index column datatable
  dataTable.on('draw.dt', function () {
    // Prevent user from add edit delete while dialog is populated
    if ($('.ui-dialog-content').dialog('isOpen') === true) {
      disableButtons(true)
    } else {
      disableButtons(false)
    }
  })

  // Order by the grouping
  $('#tblAcademicDegreeRank tbody').on('click', 'tr.group', function () {
    var currentOrder = dataTable.order()[0]
    if (currentOrder[0] === groupColumn && currentOrder[1] === 'asc') {
      dataTable.order([groupColumn, 'desc']).draw()
    } else {
      dataTable.order([groupColumn, 'asc']).draw()
    }
  })
})

function refreshTable() {
  dataTable.ajax.reload(null, false)
}

function disableButtons(state) {
  if (state === true) {
    // disable buttons
    $('.createNew').prop('disabled', true)
    $('.editRow').each(function () {
      this.style.pointerEvents = 'none'
    })
    $('.deleteRow').each(function () {
      this.style.pointerEvents = 'none'
    })
  } else {
    // enable buttons
    $('.createNew').prop('disabled', false)
    $('.editRow').each(function () {
      this.style.pointerEvents = 'auto'
    })
    $('.deleteRow').each(function () {
      this.style.pointerEvents = 'auto'
    })
  }
}

// Show Create and Edit form
function popupForm(url) {
  var formDiv = $('<div/>')
  $.get(url).done(function (response) {
    formDiv.html(response)

    popup = formDiv.dialog({
      autoOpen: true,
      resizable: false,
      title: 'Quản lý cấp bậc',
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
          refreshTable()

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

function deleteAcademicDegreeRank(id) {
  Swal.fire({
    title: 'Thông báo',
    text: 'Bạn có chắc muốn xoá cấp bậc này?',
    icon: 'warning',
    showCancelButton: true,
    cancelButtonText: 'Huỷ',
    confirmButtonText: 'Xoá',
    customClass: {
      confirmButton: 'btn btn-primary',
      cancelButton: 'btn btn-outline-danger ms-1'
    },
    buttonsStyling: false
  }).then(result => {
    if (result.isConfirmed) {
      // Delete item
      $.ajax({
        type: 'POST',
        url: rootUrl + 'AcademicDegreeRank/Delete/' + id,
        success: function (data) {
          if (data.success) {
            refreshTable()

            // Show message when delete succeeded
            toastr['success'](data.message)
          } else {
            // Show message when delete failed
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
  })
}

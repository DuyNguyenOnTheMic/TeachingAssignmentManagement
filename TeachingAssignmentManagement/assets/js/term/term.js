var popup, dataTable;
var rootUrl = $('#loader').data('request-url');

$(function () {
    'use strict';

    // Populate Term datatable
    dataTable = $('#tblTerm').DataTable(
        {
            ajax: {
                url: rootUrl + 'Term/GetData',
                type: 'GET',
                dataType: 'json',
                dataSrc: ''
            },
            deferRender: true,
            columns: [
                { 'data': 'id' },
                { 'data': 'start_year' },
                { 'data': 'end_year' },
                { 'data': 'start_week' },
                { 'data': 'start_date' },
                { 'data': 'max_lesson' },
                { 'data': 'max_class' },
                { 'data': 'status' },
                {
                    'data': 'id', 'render': function (data) {
                        return "<a class='editRow text-success p-0' data-original-title='Chỉnh sửa' title='Chỉnh sửa' onclick=popupForm('" + rootUrl + "Term/Edit/" + data + "')><i class='feather feather-edit font-medium-3 me-1'></i></a><a class='deleteRow text-danger p-0' data-original-title='Xoá' title='Xoá' onclick=deleteTerm('" + data + "')><i class='feather feather-trash-2 font-medium-3 me-1'></i></a>";
                    }
                }
            ],

            columnDefs: [
                {
                    // Term status
                    targets: 7,
                    render: function (data, type, row) {
                        var $status = data;
                        var isChecked = $status ? 'checked' : '';
                        return type === 'display' ? "<div class='form-check form-check-primary form-switch d-flex justify-content-center'><input type='checkbox' class='form-check-input user-status' aria-label='Trạng thái người dùng' onchange=editStatus('" + row.id + "','" + !$status + "') " + isChecked + "></div>" : data;
                    }
                },
                {
                    searchable: false,
                    orderable: false,
                    width: '10%',
                    targets: 8
                },
                { className: 'text-center', targets: '_all' },
                { render: DataTable.render.date(), targets: 4 }
            ],
            order: [[0, 'desc']],
            dom: '<"d-flex justify-content-between align-items-center header-actions mx-2 row mt-75"<"col-sm-12 col-lg-4 d-flex justify-content-center justify-content-lg-start" l><"col-sm-12 col-lg-8 ps-xl-75 px-0"<"dt-action-buttons d-flex align-items-center justify-content-center justify-content-lg-end flex-lg-nowrap flex-wrap"<"me-1"f>B>>>t<"d-flex justify-content-between mx-2 row mb-1"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>',
            displayLength: 10,
            lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "tất cả"]],
            buttons: [
                {
                    text: feather.icons['plus'].toSvg({ class: 'me-50 font-small-4' }) + 'Thêm học kỳ mới',
                    className: 'createNew btn btn-primary',
                    attr: {
                        'onclick': "popupForm('" + rootUrl + "Term/Create')"
                    },
                    init: function (api, node, config) {
                        $(node).removeClass('btn-secondary');
                        $(node).parent().removeClass('btn-group');
                        setTimeout(function () {
                            $(node).closest('.dt-buttons').removeClass('btn-group').addClass('d-inline-flex mt-50');
                        }, 50);
                    }
                }
            ],

            language: {
                'url': rootUrl + 'app-assets/language/datatables/vi.json'
            }
        });

    dataTable.on('draw.dt', function () {
        // Prevent user from add edit delete while dialog is populated
        if ($('.ui-dialog-content').dialog("isOpen") === true) {
            disableButtons(true);
        } else {
            disableButtons(false);
        }
    });
});

function refreshTable() {
    dataTable.ajax.reload(null, false);
}

function disableButtons(state) {
    if (state === true) {
        // disable buttons
        $('.createNew').prop('disabled', true);
        $('.editRow').each(function () {
            this.style.pointerEvents = 'none';
        });
        $('.deleteRow').each(function () {
            this.style.pointerEvents = 'none';
        });
    } else {
        // enable buttons
        $('.createNew').prop('disabled', false);
        $('.editRow').each(function () {
            this.style.pointerEvents = 'auto';
        });
        $('.deleteRow').each(function () {
            this.style.pointerEvents = 'auto';
        });
    }
}

// Edit term status
function editStatus(id, status) {
    $.ajax({
        type: 'POST',
        url: rootUrl + 'Term/EditStatus/',
        data: { id, status },
        success: function (data) {
            if (data.success) {
                refreshTable();

                // Show message when delete succeeded
                toastr["success"](data.message);
            }
        }
    });
}

// Show Create and Edit form
function popupForm(url) {
    var formDiv = $('<div />')
    $.get(url)
        .done(function (response) {
            formDiv.html(response);

            popup = formDiv.dialog({
                autoOpen: true,
                resizable: false,
                title: 'Quản lý học kỳ',
                width: 550,
                open: function () {
                    // Add close icon class
                    $(this).closest(".ui-dialog")
                        .find(".ui-dialog-titlebar-close")
                        .addClass("btn-close");

                    // Prevent user from add edit delete while dialog is populated
                    disableButtons(true);
                },
                close: function () {
                    popup.dialog('destroy').remove();

                    // Re-enable buttons when user closes the dialog
                    disableButtons(false);
                }
            });
        });
}

function submitForm(form) {
    $.validator.unobtrusive.parse(form);

    if ($(form).valid()) {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: $(form).serialize(),
            success: function (data) {
                if (data.success) {
                    popup.dialog('close');
                    refreshTable();

                    // Show message when create or edit succeeded
                    toastr["success"](data.message);
                }
                else {
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
        });
    }
    return false;
}

function deleteTerm(id) {
    Swal.fire({
        title: 'Thông báo',
        text: 'Bạn có chắc muốn xoá học kỳ này?',
        icon: 'warning',
        showCancelButton: true,
        cancelButtonText: 'Huỷ',
        confirmButtonText: 'Xoá',
        customClass: {
            confirmButton: 'btn btn-primary',
            cancelButton: 'btn btn-outline-danger ms-1'
        },
        buttonsStyling: false
    }).then((result) => {
        if (result.isConfirmed) {
            // Delete item
            $.ajax({
                type: 'POST',
                url: rootUrl + 'Term/Delete/' + id,
                success: function (data) {
                    if (data.success) {
                        refreshTable();

                        // Show message when delete succeeded
                        toastr["success"](data.message);
                    }
                    else {
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
            });
        }
    })
}
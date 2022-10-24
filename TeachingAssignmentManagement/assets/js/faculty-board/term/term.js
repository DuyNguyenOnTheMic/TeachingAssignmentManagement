﻿var popup, dataTable;

$(function () {
    'use strict';

    // Populate Term datatable
    dataTable = $('#tblTerm').DataTable(
        {
            ajax: {
                url: 'Term/GetData',
                type: 'GET',
                dataType: 'json',
                dataSrc: ''
            },
            deferRender: true,
            columns: [
                { 'data': '', defaultContent: '' },
                { 'data': 'id' },
                { 'data': 'start_year' },
                { 'data': 'end_year' },
                { 'data': 'start_week' },
                { 'data': 'start_date' },
                {
                    'data': 'id', 'render': function (data) {
                        return "<a class='editRow text-success p-0' data-original-title='Edit' title='Edit' onclick=popupForm('Term/Edit/" + data + "')><i class='feather feather-edit font-medium-3 me-1'></i></a> <a class='deleteRow text-danger p-0' data-original-title='Delete' title='Delete' onclick=deleteTerm('" + data + "')><i class='feather feather-trash-2 font-medium-3 me-1'></i></a>";
                    }
                }
            ],

            columnDefs: [
                {
                    searchable: false,
                    orderable: false,
                    className: 'text-center',
                    targets: [0, 6]
                },
                { width: '5%', targets: 0 },
                { width: '10%', targets: 6 },
                { render: DataTable.render.date(), targets: 5 }
            ],
            order: [[1, 'asc']],
            dom: '<"d-flex justify-content-between align-items-center header-actions mx-2 row mt-75"<"col-sm-12 col-lg-4 d-flex justify-content-center justify-content-lg-start" l><"col-sm-12 col-lg-8 ps-xl-75 ps-0"<"dt-action-buttons d-flex align-items-center justify-content-center justify-content-lg-end flex-lg-nowrap flex-wrap"<"me-1"f>B>>>t<"d-flex justify-content-between mx-2 row mb-1"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>',
            displayLength: 7,
            lengthMenu: [7, 10, 25, 50, 75, 100],
            buttons: [
                {
                    text: feather.icons['plus'].toSvg({ class: 'me-50 font-small-4' }) + 'Thêm học kỳ mới',
                    className: 'createNew btn btn-primary',
                    attr: {
                        'onclick': "popupForm('Term/Create')"
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
                'url': '/app-assets/language/vi.json'
            }
        });

    // Create Index column datatable
    dataTable.on('draw.dt', function () {
        dataTable.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    });
});


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
                    $('.createNew').prop('disabled', true);
                    $('.editRow').each(function () {
                        this.style.pointerEvents = 'none';
                    });
                    $('.deleteRow').each(function () {
                        this.style.pointerEvents = 'none';
                    });
                },
                close: function () {
                    popup.dialog('destroy').remove();

                    // Re-enable buttons when user closes the dialog
                    $('.createNew').prop('disabled', false);
                    $('.editRow').each(function () {
                        this.style.pointerEvents = 'auto';
                    });
                    $('.deleteRow').each(function () {
                        this.style.pointerEvents = 'auto';
                    });
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
                    dataTable.ajax.reload();

                    // Show message when create or edit succeeded
                    toastr.options.positionClass = 'toast-bottom-right';
                    toastr["success"](data.message);
                }
                else {
                    // Show message when create failed
                    Swal.fire({
                        title: 'Thông báo',
                        text: 'Mã ngành này đã tồn tại!',
                        icon: 'error',
                        customClass: {
                            confirmButton: 'btn btn-primary'
                        }
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
        }
    }).then((result) => {
        if (result.isConfirmed) {
            // Delete item
            $.ajax({
                type: 'POST',
                url: 'Term/Delete/' + id,
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();

                        // Show message when delete succeeded
                        toastr.options.positionClass = 'toast-bottom-right';
                        toastr["success"](data.message);
                    }
                    else {
                        // Show message when delete failed
                        Swal.fire({
                            title: 'Thông báo',
                            text: 'Không thể xoá do đã có sinh viên học ngành này!',
                            icon: 'error',
                            customClass: {
                                confirmButton: 'btn btn-primary'
                            }
                        })
                    }
                }
            });
        }
    })
}
var popup, dataTable;
var rootUrl = $('#loader').data('request-url');
var typeBadgeObj = {
    'CH': { title: 'Cơ hữu', class: 'badge-light-success' },
    'TG': { title: 'Thỉnh giảng', class: 'badge-light-warning' }
};

$(function () {
    'use strict';

    // Populate User datatable
    dataTable = $('#tblUser').DataTable(
        {
            ajax: {
                url: rootUrl + 'User/GetData',
                type: 'GET',
                dataType: 'json',
                dataSrc: ''
            },
            deferRender: true,
            columns: [
                { 'data': '', defaultContent: '' },
                { 'data': 'staff_id' },
                { 'data': 'full_name' },
                { 'data': 'email' },
                { 'data': 'type' },
                { 'data': 'role' },
                {
                    'data': 'id', 'render': function (data, type, row) {
                        return "<a class='editRow text-success p-0' data-original-title='Chỉnh sửa' title='Chỉnh sửa' onclick=popupForm('" + rootUrl + "User/Edit/" + data + "')><i class='feather feather-edit font-medium-3 me-1'></i></a><a class='deleteRow text-danger p-0' data-original-title='Xoá' title='Xoá' onclick=deleteUser('" + data + "','" + row.email + "')><i class='feather feather-trash-2 font-medium-3 me-1'></i></a>";
                    }
                }
            ],
            columnDefs: [
                {
                    // User type
                    targets: 4,
                    render: function (data) {
                        var $status = data;
                        if ($status) {
                            return '<span class="badge rounded-pill ' + typeBadgeObj[$status].class + ' text-capitalized">' + typeBadgeObj[$status].title + '</span>';
                        } else {
                            return null;
                        }
                    }
                },
                {
                    // User Role
                    targets: 5,
                    render: function (data, type, full, meta) {
                        var $role = full['role'];
                        var roleBadgeObj = {
                            'BCN khoa': feather.icons['command'].toSvg({ class: 'font-medium-3 text-primary me-50' }),
                            'Bộ môn': feather.icons['book'].toSvg({ class: 'font-medium-3 text-danger me-50' }),
                            'Giảng viên': feather.icons['edit-2'].toSvg({ class: 'font-medium-3 text-success me-50' }),
                            'Chưa phân quyền': feather.icons['help-circle'].toSvg({ class: 'font-medium-3 text-warning me-50' })
                        };
                        return '<span class="text-truncate align-middle">' + roleBadgeObj[$role] + $role + '</span>';
                    }
                },
                {
                    searchable: false,
                    orderable: false,
                    className: 'text-center',
                    targets: [0, 6]
                },
                { width: '5%', targets: 0 },
                { width: '10%', targets: 6 }
            ],

            order: [[3, 'asc']],
            dom: '<"d-flex justify-content-between align-items-center header-actions mx-2 row mt-75"<"col-sm-12 col-lg-4 d-flex justify-content-center justify-content-lg-start" l><"col-sm-12 col-lg-8 ps-xl-75 px-0"<"dt-action-buttons d-flex align-items-center justify-content-center justify-content-lg-end flex-lg-nowrap flex-wrap"<"me-1"f>B>>>t<"d-flex justify-content-between mx-2 row mb-1"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>',
            lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "tất cả"]],
            buttons: [
                {
                    text: 'Thêm người dùng',
                    className: 'createNew btn btn-primary',
                    attr: {
                        'onclick': "popupForm('" + rootUrl + "User/Create')"
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
            },
            initComplete: function () {
                // Adding user type filter once table initialized
                this.api()
                    .columns(4)
                    .every(function () {
                        var column = this;
                        var select = $(
                            '<select id="UserType" class="form-select text-capitalize mb-md-0 mb-2xx"><option value=""> Chọn loại GV để lọc </option></select>'
                        )
                            .appendTo('.user_type')
                            .on('change', function () {
                                var val = $.fn.dataTable.util.escapeRegex($(this).val());
                                column.search(val ? '^' + val + '$' : '', true, false).draw();
                            });

                        column
                            .data()
                            .unique()
                            .sort()
                            .each(function (d, j) {
                                if (d) {
                                    select.append(
                                        '<option value="' +
                                        typeBadgeObj[d].title +
                                        '" class="text-capitalize">' +
                                        typeBadgeObj[d].title +
                                        '</option>'
                                    );
                                }
                            });
                    });

                // Adding role filter once table initialized
                this.api()
                    .columns(5)
                    .every(function () {
                        var column = this;
                        var select = $(
                            '<select id="UserRole" class="form-select text-capitalize mb-md-0 mb-2"><option value=""> Chọn role để lọc </option></select>'
                        )
                            .appendTo('.user_role')
                            .on('change', function () {
                                var val = $.fn.dataTable.util.escapeRegex($(this).val());
                                column.search(val ? '^' + val + '$' : '', true, false).draw();
                            });

                        column
                            .data()
                            .unique()
                            .sort()
                            .each(function (d, j) {
                                select.append('<option value="' + d + '" class="text-capitalize">' + d + '</option>');
                            });
                    });
            }
        });

    // Create Index column datatable
    dataTable.on('draw.dt', function () {
        dataTable.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
        // Get user count
        var countFBoard = dataTable.column(5).data().filter(function (value, index) { return value === "BCN khoa" ? true : false; }).length;
        var countDHead = dataTable.column(5).data().filter(function (value, index) { return value === "Bộ môn" ? true : false; }).length;
        var countLecturer = dataTable.column(5).data().filter(function (value, index) { return value === "Giảng viên" ? true : false; }).length;
        var countUnassigned = dataTable.column(5).data().filter(function (value, index) { return value === "Chưa phân quyền" ? true : false; }).length;
        $('#totalFBoard').text(countFBoard);
        $('#totalDHead').text(countDHead);
        $('#totalLecturer').text(countLecturer);
        $('#totalUnassigned').text(countUnassigned);

        // Prevent user from add edit delete while dialog is populated
        if ($('.ui-dialog-content').dialog("isOpen") === true) {
            disableButtons(true);
        } else {
            disableButtons(false);
        }
    });
});

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

// Show Edit form
function popupForm(url) {
    var formDiv = $('<div />')
    $.get(url)
        .done(function (response) {
            formDiv.html(response);

            popup = formDiv.dialog({
                autoOpen: true,
                resizable: false,
                title: 'Quản lý người dùng',
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
                    dataTable.ajax.reload(null, false);

                    // Show message when edit succeeded
                    toastr["success"](data.message);
                }
                else {
                    // Show message when edit failed
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

function deleteUser(id, email) {
    "null" == email && (email = "");
    Swal.fire({
        title: 'Thông báo',
        text: 'Bạn có chắc muốn xoá người dùng ' + email + ' này?',
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
                url: rootUrl + 'User/Delete/' + id,
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload(null, false);

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
var popup, dataTable;

// Setup data
var dataLoader = $('#data-loader'),
    termId = dataLoader.data('termid'),
    majorId = dataLoader.data('majorid');

$(function () {
    // Populate Subject datatable
    dataTable = $('#tblSubject').DataTable(
        {
            ajax: {
                url: rootUrl + 'Remuneration/GetSubjectData?termId=' + termId + '&majorId=' + majorId,
                type: 'GET',
                dataType: 'json',
                dataSrc: ''
            },
            deferRender: true,
            columns: [
                { 'data': '', defaultContent: '' },
                { 'data': 'subject_id' },
                { 'data': 'name' },
                { 'data': 'credits' },
                { 'data': 'major', defaultContent: '' },
                { 'data': 'is_vietnamese' },
                { 'data': 'theoretical_coefficient' },
                { 'data': 'practice_coefficient' },
                {
                    'data': 'id', 'render': function (data) {
                        return "<a class='editRow text-success p-0' data-original-title='Chỉnh sửa' title='Chỉnh sửa' onclick=popupForm('" + rootUrl + "Remuneration/EditSubject/" + data + "')><i class='feather feather-edit font-medium-3 me-1'></i></a>";
                    }
                }
            ],

            columnDefs: [
                {
                    // Subject language
                    targets: 5,
                    render: function (data) {
                        return data ? '<i class="flag-icon flag-icon-vn me-50"></i>Việt' : '<i class="flag-icon flag-icon-us me-50"></i>Anh';
                    }
                },
                {
                    searchable: false,
                    orderable: false,
                    targets: [0, 8]
                },
                { className: 'text-center', targets: [0, 3, 5, 6, 7, 8] },
                { width: '5%', targets: 0 },
                { width: '10%', targets: 8 }
            ],
            order: [[1, 'asc']],
            dom: '<"d-flex justify-content-between align-items-center header-actions mx-2 row mt-75"<"col-sm-12 col-lg-4 d-flex justify-content-center justify-content-lg-start" l><"col-sm-12 col-lg-8 ps-xl-75 px-0"<"dt-action-buttons d-flex align-items-center justify-content-center justify-content-lg-end flex-lg-nowrap flex-wrap"<"me-1"f>B>>>t<"d-flex justify-content-between mx-2 row mb-1"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>',
            displayLength: 10,
            lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "tất cả"]],
            buttons: [
                {
                    text: feather.icons['edit-2'].toSvg({ class: 'me-50 font-small-4' }) + 'Sửa tất cả',
                    className: 'editAll btn btn-primary',
                    attr: {
                        'onclick': "popupForm('" + rootUrl + "Remuneration/EditAllSubjects?termId=" + termId + "&majorId=" + majorId + "')"
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

            initComplete: function () {
                majorId == '-1' ? setVisibleColumn(true) : setVisibleColumn(false);
            },
            language: {
                'url': rootUrl + 'app-assets/language/datatables/vi.json'
            }
        });

    // Create Index column datatable
    dataTable.on('draw.dt', function () {
        dataTable.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });

        // Prevent user from add edit delete while dialog is populated
        if ($('.ui-dialog-content').dialog("isOpen") === true) {
            disableButtons(true);
        } else {
            disableButtons(false);
        }
    });
});

function setVisibleColumn(state) {
    var table = $('#tblSubject').DataTable();
    table.column(4).visible(state, state);
    table.columns.adjust().draw(state); // adjust column sizing and redraw
}

function disableButtons(state) {
    if (state === true) {
        // disable buttons
        $('.editAll').prop('disabled', true);
        $('.editRow').each(function () {
            this.style.pointerEvents = 'none';
        });
    } else {
        // enable buttons
        $('.editAll').prop('disabled', false);
        $('.editRow').each(function () {
            this.style.pointerEvents = 'auto';
        });
    }
}

// Show Edit form
function popupForm(url) {
    var formDiv = $('<div/>')
    $.get(url)
        .done(function (response) {
            formDiv.html(response);

            popup = formDiv.dialog({
                autoOpen: true,
                resizable: false,
                title: 'Quản lý thù lao môn học',
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
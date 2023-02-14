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
                { 'data': 'id' },
                { 'data': 'name' },
                { 'data': 'abbreviation' },
                {
                    'data': 'id', 'render': function (data) {
                        return "<a class='editRow text-success p-0' data-original-title='Chỉnh sửa' title='Chỉnh sửa' onclick=popupForm('" + rootUrl + "Remuneration/EditSubject/" + data + "')><i class='feather feather-edit font-medium-3 me-1'></i></a>";
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
                { width: '10%', targets: 6 }
            ],
            order: [[1, 'asc']],
            dom: '<"d-flex justify-content-between align-items-center header-actions mx-2 row mt-75"<"col-sm-12 col-lg-4 d-flex justify-content-center justify-content-lg-start" l><"col-sm-12 col-lg-8 ps-xl-75 px-0"<"dt-action-buttons d-flex align-items-center justify-content-center justify-content-lg-end flex-lg-nowrap flex-wrap"<"me-1"f>B>>>t<"d-flex justify-content-between mx-2 row mb-1"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>',
            displayLength: 10,
            lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "tất cả"]],

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

// Show Edit form
function popupForm(url) {
    var formDiv = $('<div/>')
    $.get(url)
        .done(function (response) {
            formDiv.html(response);

            popup = formDiv.dialog({
                autoOpen: true,
                resizable: false,
                title: 'Quản lý môn học',
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

function disableButtons(state) {
    if (state === true) {
        // disable buttons
        $('.editRow').each(function () {
            this.style.pointerEvents = 'none';
        });
    } else {
        // enable buttons
        $('.editRow').each(function () {
            this.style.pointerEvents = 'auto';
        });
    }
}
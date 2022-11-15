var popup, dataTable;
var rootUrl = $('#loader').data('request-url');

$(function () {
    'use strict';

    // Populate User datatable
    dataTable = $('#tblAssign').DataTable(
        {
            ajax: {
                url: rootUrl + 'FacultyBoard/Timetable/GetData',
                type: 'GET',
                dataType: 'json',
                dataSrc: ''
            },
            paging: false,
            ordering: false,
            info: false,
            columns: [
                { 'data': 'staff_id' },
                { 'data': 'full_name' },
                { 'data': 'email' },
                { 'data': 'role' },
                {
                    'data': 'id', 'render': function (data, type, row) {
                        return "<a class='editRow text-success p-0' data-original-title='Edit' title='Edit' onclick=popupForm('" + rootUrl + "FacultyBoard/User/Edit/" + data + "')><i class='feather feather-edit font-medium-3 me-1'></i></a> <a class='deleteRow text-danger p-0' data-original-title='Delete' title='Delete' onclick=deleteUser('" + data + "','" + row.email + "')><i class='feather feather-trash-2 font-medium-3 me-1'></i></a>";
                    }
                }
            ],
            columnDefs: [
                {
                    // User Role
                    targets: 4,
                    render: function (data, type, full, meta) {
                    }
                }
            ],
            dom: '<"d-flex justify-content-between align-items-center header-actions mx-2 row mt-75"<"col-sm-12 col-lg-4 d-flex justify-content-center justify-content-lg-start" l><"col-sm-12 col-lg-8 ps-xl-75 ps-0"<"dt-action-buttons d-flex align-items-center justify-content-center justify-content-lg-end flex-lg-nowrap flex-wrap"<"me-1"f>B>>>t<"d-flex justify-content-between mx-2 row mb-1"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>',
            language: {
                'url': rootUrl + 'app-assets/language/datatables/vi.json'
            }
        });

    // Create Index column datatable
    dataTable.on('draw.dt', function () {
        dataTable.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    });
});
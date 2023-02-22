﻿var popup, dataTable;

// Setup data
var dataLoader = $('#data-loader'),
    termId = dataLoader.data('termid');

$(function () {
    // Populate LecturerRank datatable
    dataTable = $('#tblRemuneration').DataTable(
        {
            ajax: {
                url: rootUrl + 'Remuneration/GetRemunerationData?termId=' + termId,
                type: 'GET',
                dataType: 'json',
                dataSrc: ''
            },
            deferRender: true,
            columns: [
                { 'data': '', defaultContent: '' },
                { 'data': 'StaffId' },
                { 'data': 'FullName' },
                { 'data': 'AcademicDegreeRankId' }
            ],

            columnDefs: [
                {
                    searchable: false,
                    orderable: false,
                    targets: [0, 3]
                },
                { className: 'text-center', targets: [0, 3, 3] },
                { width: '5%', targets: 0 },
                { width: '10%', targets: 3 }
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
                        'onclick': "popupForm('" + rootUrl + "Remuneration/EditAllLecturerRanks?termId=" + termId + "')"
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

    // Create Index column datatable
    dataTable.on('draw.dt', function () {
        dataTable.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    });
});
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
            searching: false,
            paging: false,
            ordering: false,
            info: false,
            columns: [
                { 'data': 'name' },
                { 'data': 'mon1' },
                { 'data': 'mon4' },
                { 'data': 'mon7' },
                { 'data': 'mon10' },
                { 'data': 'mon13' },
                { 'data': 'tues1' },
                { 'data': 'tues4' },
                { 'data': 'tues7' },
                { 'data': 'tues10' },
                { 'data': 'tues13' },
                { 'data': 'wed1' },
                { 'data': 'wed4' },
                { 'data': 'wed7' },
                { 'data': 'wed10' },
                { 'data': 'wed13' },
                { 'data': 'thurs1' },
                { 'data': 'thurs4' },
                { 'data': 'thurs7' },
                { 'data': 'thurs10' },
                { 'data': 'thurs13' },
                { 'data': 'fri1' },
                { 'data': 'fri4' },
                { 'data': 'fri7' },
                { 'data': 'fri10' },
                { 'data': 'fri13' },
                { 'data': 'sat1' },
                { 'data': 'sat4' },
                { 'data': 'sat7' },
                { 'data': 'sat10' },
                { 'data': 'sat13' }
            ],
            dom: '<"d-flex justify-content-between align-items-center header-actions mx-2 row mt-75"<"col-sm-12 col-lg-4 d-flex justify-content-center justify-content-lg-start" l><"col-sm-12 col-lg-8 ps-xl-75 px-0"<"dt-action-buttons d-flex align-items-center justify-content-center justify-content-lg-end flex-lg-nowrap flex-wrap"<"me-1"f>B>>>t<"d-flex justify-content-between mx-2 row mb-1"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>',
            buttons: [
                {
                    extend: 'collection',
                    className: 'btn btn-outline-secondary dropdown-toggle',
                    text: feather.icons['share'].toSvg({ class: 'font-small-4 me-50' }) + 'Export',
                    buttons: [
                        {
                            extend: 'print',
                            className: 'dropdown-item'
                        },
                        {
                            extend: 'excel',
                            className: 'dropdown-item',
                            title: '',
                            customize: function (xlsx) {
                                var sheet = xlsx.xl.worksheets['sheet1.xml'];
                                $('row:first c', sheet).attr('s', '42');
                            }
                        },
                        {
                            extend: 'pdf',
                            className: 'dropdown-item'
                        },
                        {
                            extend: 'copy',
                            className: 'dropdown-item'
                        }
                    ],
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
});
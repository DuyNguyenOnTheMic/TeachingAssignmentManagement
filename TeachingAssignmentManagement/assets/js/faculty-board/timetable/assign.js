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
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' },
                { 'data': 'curriculum_class_id' }
            ],
            dom: '<"d-flex justify-content-between align-items-center header-actions mx-2 row mt-75"<"col-sm-12 col-lg-4 d-flex justify-content-center justify-content-lg-start" l><"col-sm-12 col-lg-8 ps-xl-75 ps-0"<"dt-action-buttons d-flex align-items-center justify-content-center justify-content-lg-end flex-lg-nowrap flex-wrap"<"me-1"f>B>>>t<"d-flex justify-content-between mx-2 row mb-1"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>',
            language: {
                'url': rootUrl + 'app-assets/language/datatables/vi.json'
            }
        });
});
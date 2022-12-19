// Display message when table have no data
var rowCount = $('#tblStatistics tbody tr').length;
if (rowCount == 0) {
    $('#timetableStatisticsDiv').empty().append('<h4 class="text-center mt-2">Học kỳ này chưa có dữ liệu <i class="feather feather-help-circle"></i></h4><div class="card-body"><img class="mx-auto p-3 d-block w-50" alt="Welcome" src="' + rootUrl + 'assets/images/img_no_data.svg"></div>');
} else {
    // Initialize Tooltip
    $('#tblStatistics [data-bs-toggle="tooltip"]').tooltip({
        trigger: 'hover'
    });

    // Initialize Popover
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl, {
            html: true,
            sanitize: false
        });
    });
}
var rootUrl = $('#loader').data('request-url'),
    statisticsDiv = $('#statisticsDiv'),
    url = rootUrl + 'Statistics/GetChart';

$(function () {
    var termId = 223;
    $.get(url, function (data) {
        if (!data.error) {
            // Populate personal timetable
            statisticsDiv.html(data);
        } else {
            // Return not found error message
            personalTimetableDiv.html('<h4 class="text-center mt-2">' + data.message + '</h4><div class="card-body"><img class="mx-auto p-3 d-block w-50" alt="No data" src="' + rootUrl + 'assets/images/img_no_data.svg"></div>');
            weekSelect.parent().find('.select2-selection__placeholder').text('không khả dụng');
        }
    });
});
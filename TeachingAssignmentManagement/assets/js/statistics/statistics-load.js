var rootUrl = $('#loader').data('request-url'),
    personalTimetableDiv = $('#statisticsDiv'),
    url = rootUrl + 'Statistics/GetChart';

$(window).on('load', function () {
    // Detect Dark Layout and change color
    $('.nav-link-style').on('click', function () {
        var lightColor = '#b4b7bd',
            darkColor = '#6e6b7b',
            color;
        if ($('html').hasClass('dark-layout')) {
            color = lightColor;
        } else {
            color = darkColor;
        }
        chart.options.scales.xAxis.ticks.color = color;
        chart.options.scales.yAxis.ticks.color = color;
        chart.options.plugins.datalabels.color = color;
        chart.options.plugins.legend.labels.color = color;
        chart.update();
    });
});

$(function () {
    var termId = 223;
    $.get(url, function (data) {
        if (!data.error) {
            // Populate personal timetable
            personalTimetableDiv.html(data);
        } else {
            // Return not found error message
            personalTimetableDiv.html('<h4 class="text-center mt-2">' + data.message + '</h4><div class="card-body"><img class="mx-auto p-3 d-block w-50" alt="No data" src="' + rootUrl + 'assets/images/img_no_data.svg"></div>');
            weekSelect.parent().find('.select2-selection__placeholder').text('không khả dụng');
        }
    });
});
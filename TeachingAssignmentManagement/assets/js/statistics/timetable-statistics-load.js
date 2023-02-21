var formSelect = $('.form-select'),
    termSelect = $('#term'),
    weekSelect = $('#week'),
    rootUrl = $('#loader').data('request-url'),
    timetableStatisticsDiv = $('#timetableStatisticsDiv'),
    url = rootUrl + 'Statistics/GetTimetable';

$(function () {
    // Set latest term option
    var termId = $('#term option:eq(1)').val(),
        week = 0;
    termSelect.val(termId);

    // Populate select2 for choosing term and major
    formSelect.each(function () {
        var $this = $(this);
        $this.wrap('<div class="position-relative"></div>');
        $this.select2({
            language: 'vi',
            dropdownAutoWidth: true,
            dropdownParent: $this.parent(),
            placeholder: $this[0][0].innerHTML
        });
    })

    // Fetch data on form load
    if (termId) {
        fetchData(termId, week);
    } else {
        showNoData(timetableStatisticsDiv, 'học kỳ');
    }
});

termSelect.change(function () {
    // Empty week select to re-populate weeks
    weekSelect.empty();

    var termId = $(this).val(),
        week = 0;
    fetchData(termId, week);
});

weekSelect.change(function () {
    var termId = termSelect.val(),
        week = $(this).val();
    fetchData(termId, week);
});

function fetchData(termId, week) {
    // Dispose all tooltips and popovers
    $("#tblStatistics [data-bs-toggle='tooltip']").tooltip('dispose');

    if (termId) {
        // Display loading message while fetching data
        showLoading(timetableStatisticsDiv);

        // Get Partial View timetable data
        $.get(url, { termId, week }, function (data) {
            timetableStatisticsDiv.html(data);
        });
    }
}
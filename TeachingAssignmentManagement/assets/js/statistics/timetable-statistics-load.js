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
        timetableStatisticsDiv.html('<h4 class="text-center mt-2">Chưa có dữ liệu học kỳ</h4><div class="card-body"><img class="mx-auto p-3 d-block w-50" alt="No data" src="' + rootUrl + 'assets/images/img_no_data.svg"></div>');
    }
});

termSelect.change(function () {
    // Empty week select to re-populate weeks
    weekSelect.empty();

    var termId = $(this).val(),
        week = 0;
    loading();
    fetchData(termId, week);
});

weekSelect.change(function () {
    var termId = termSelect.val(),
        week = $(this).val();
    loading();
    fetchData(termId, week);
});

function loading() {
    timetableStatisticsDiv.html('<div class="d-flex justify-content-center mt-2"><div class="spinner-border text-primary me-1" role="status"><span class="visually-hidden">Loading...</span></div><p class="my-auto">Đang tải...</p></div>');
}

function fetchData(termId, week) {
    // Dispose all tooltips and popovers
    $("#tblStatistics [data-bs-toggle='tooltip']").tooltip('dispose');

    if (termId) {
        // Display loading message while fetching data
        loading();

        // Get Partial View timetable data
        $.get(url, { termId, week }, function (data) {
            timetableStatisticsDiv.html(data);
        });
    }
}
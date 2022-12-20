var formSelect = $('.form-select'),
    termSelect = $('#term'),
    weekSelect = $('#week'),
    rootUrl = $('#loader').data('request-url'),
    timetableStatisticsDiv = $('#timetableStatisticsDiv'),
    url = rootUrl + 'Statistics/GetTimetable';

$(function () {
    // Set selected option when form load
    formSelect.each(function () {
        var $this = $(this);
        $this.val($this.find('option:eq(1)').val());
    });

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
    fetchData();
});

formSelect.change(function () {
    fetchData();
});

function fetchData() {
    var termId = termSelect.val(),
        week = 0;
    if (termId) {
        getTimetable(termId, week);
    } else {
        timetableStatisticsDiv.html('<h4 class="text-center mt-2">Chưa có dữ liệu học kỳ</h4><div class="card-body"><img class="mx-auto p-3 d-block w-50" alt="No data" src="' + rootUrl + 'assets/images/img_no_data.svg"></div>');
    }
}

function getTimetable(termId, week) {
    // Dispose all tooltips and popovers
    $("#tblAssign [data-bs-toggle='tooltip']").tooltip('dispose');

    if (termId) {
        // Display loading message while fetching data
        timetableStatisticsDiv.html('<div class="d-flex justify-content-center mt-2"><div class="spinner-border text-primary me-1" role="status"><span class="visually-hidden">Loading...</span></div><p class="my-auto">Đang tải...</p></div>');

        // Get Partial View timetable data
        $.get(url, { termId, week }, function (data) {
            timetableStatisticsDiv.html(data);
        });
    }
}
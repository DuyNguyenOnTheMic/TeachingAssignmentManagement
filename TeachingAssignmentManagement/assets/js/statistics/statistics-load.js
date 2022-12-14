var termSelect = $('#term'),
    rootUrl = $('#loader').data('request-url'),
    statisticsDiv = $('#statisticsDiv'),
    curriculumsDiv = $('#curriculumsDiv'),
    url = rootUrl + 'Statistics/GetChart';

$(function () {
    var formSelect = $('.form-select');

    // Set latest term value
    var termId = $('#term option:eq(1)').val();
    termSelect.val(termId);

    // Populate select2 for choosing term and week
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

    if (termId) {
        // Get Partial View personal timetable data
        fetchData();
    } else {
        statisticsDiv.html('<h4 class="text-center mt-2">Chưa có dữ liệu học kỳ</h4><div class="card-body"><img class="mx-auto p-3 d-block w-50" alt="No data" src="' + rootUrl + 'assets/images/img_no_data.svg"></div>');
    }
});

// Fetch data on term change
termSelect.change(function () {
    // Empty curriculumsDiv
    curriculumsDiv.empty();
    // Display loading message while fetching data
    statisticsDiv.html('<div class="d-flex justify-content-center mt-2"><div class="spinner-border text-primary me-1" role="status"><span class="visually-hidden">Loading...</span></div><p class="my-auto">Đang tải...</p></div>');
    fetchData();
});


function fetchData() {
    $.get(url, function (data) {
        // Populate statistics data
        statisticsDiv.html(data);
    });
}
var unitSelect = $('#unit'),
    termSelect = $('#term'),
    yearSelect = $('#year'),
    rootUrl = $('#loader').data('request-url'),
    statisticsDiv = $('#statisticsDiv');

$(function () {
    var formSelect = $('.form-select');

    // Set latest term value
    var termId = $('#term option:eq(1)').val();
    termSelect.val(termId);

    if (termId) {
        // Get Partial View personal timetable data
        fetchData();
    } else {
        statisticsDiv.html('<h4 class="text-center mt-2">Chưa có dữ liệu học kỳ</h4><div class="card-body"><img class="mx-auto p-3 d-block w-50" alt="No data" src="' + rootUrl + 'assets/images/img_no_data.svg"></div>');
    }
});

// Unit select event for viewing statistics
unitSelect.change(function () {
    var $this = $(this);
    if ($this.val() == 'term') {
        termSelect.parent().removeClass('d-none');
        yearSelect.parent().addClass('d-none');
    } else {
        yearSelect.parent().removeClass('d-none');
        termSelect.parent().addClass('d-none');
    }
});

// Fetch data on term change
termSelect.change(function () {
    // Display loading message while fetching data
    statisticsDiv.html('<div class="d-flex justify-content-center mt-2"><div class="spinner-border text-primary me-1" role="status"><span class="visually-hidden">Loading...</span></div><p class="my-auto">Đang tải...</p></div>');
    fetchData();
});


function fetchData() {
    var url = rootUrl + 'Statistics/GetChart';
    $.get(url, function (data) {
        // Populate statistics data
        statisticsDiv.html(data);
    });
}
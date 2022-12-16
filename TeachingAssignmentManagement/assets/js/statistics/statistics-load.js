var unitSelect = $('#unit'),
    termSelect = $('#term'),
    yearSelect = $('#year'),
    formTermYear = $('.form-termyear'),
    rootUrl = $('#loader').data('request-url'),
    statisticsDiv = $('#statisticsDiv'),
    latestTermId = $('#term option:eq(1)').val(),
    latestYearId = $('#year option:eq(1)').val();

$(function () {
    var formSelect = $('.form-select');

    // Set latest term value
    termSelect.val(latestTermId);

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

    if (latestTermId) {
        // Get Partial View personal timetable data
        fetchData(termSelect.attr('id'), latestTermId);
    } else {
        statisticsDiv.html('<h4 class="text-center mt-2">Chưa có dữ liệu học kỳ</h4><div class="card-body"><img class="mx-auto p-3 d-block w-50" alt="No data" src="' + rootUrl + 'assets/images/img_no_data.svg"></div>');
    }
});

// Unit select event for viewing statistics
unitSelect.change(function () {
    var $this = $(this);
    if ($this.val() == 'term') {
        // Set latest term
        termSelect.val(latestTermId).trigger('change');

        // Show term select2 field
        $('#termDiv').show();
        $('#yearDiv').hide();
    } else {
        // Set latest year
        yearSelect.val(latestYearId).trigger('change');

        // Show year select2 field
        $('#yearDiv').show();
        $('#termDiv').hide();
    }
});

// Fetch data on term change
formTermYear.change(function () {
    var $this = $(this),
        type = $this.attr('id'),
        value = $this.val(),
        lecturerType = $('#lecturerType').val();
    // Display loading message while fetching data
    statisticsDiv.html('<div class="d-flex justify-content-center mt-2"><div class="spinner-border text-primary me-1" role="status"><span class="visually-hidden">Loading...</span></div><p class="my-auto">Đang tải...</p></div>');
    fetchData(type, value, lecturerType);
});


function fetchData(type, value, lecturerType) {
    var url = rootUrl + 'Statistics/GetChart';
    $.get(url, { type, value, lecturerType }, function (data) {
        // Populate statistics data
        statisticsDiv.html(data);
    });
}
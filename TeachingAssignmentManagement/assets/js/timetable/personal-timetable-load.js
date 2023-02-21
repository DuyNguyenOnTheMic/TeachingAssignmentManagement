var termSelect = $('#term'),
    weekSelect = $('#week'),
    rootUrl = $('#loader').data('request-url'),
    personalTimetableDiv = $('#personalTimetableDiv'),
    url = rootUrl + 'Timetable/GetPersonalData';

$(function () {
    var formSelect = $('.form-select');

    // Set latest term option
    var termId = $('#term option:eq(1)').val(),
        week = 0;
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
        fetchData(termId, week);
    } else {
        personalTimetableDiv.html('<h4 class="text-center mt-2">Chưa có dữ liệu học kỳ</h4><div class="card-body"><img class="mx-auto p-3 d-block w-50" alt="No data" src="' + rootUrl + 'assets/images/img_no_data.svg"></div>');
        weekSelect.parent().find('.select2-selection__placeholder').text('không khả dụng');
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
    // Display loading message while fetching data
    showLoading(personalTimetableDiv);

    // Dispose all tooltips
    $("[data-bs-toggle='tooltip']").tooltip('dispose');
}

function fetchData(termId, week) {
    // Get Partial View timetable data
    $.get(url, { termId, week }, function (data) {
        if (!data.error) {
            // Populate personal timetable
            personalTimetableDiv.html(data);
        } else {
            // Return not found error message
            personalTimetableDiv.html('<h4 class="text-center mt-2">' + data.message + '</h4><div class="card-body"><img class="mx-auto p-3 d-block w-50" alt="No data" src="' + rootUrl + 'assets/images/img_no_data.svg"></div>');
            weekSelect.parent().find('.select2-selection__placeholder').text('không khả dụng');
        }
    });
}
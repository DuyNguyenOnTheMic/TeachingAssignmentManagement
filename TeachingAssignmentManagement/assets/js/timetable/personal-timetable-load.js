var weekSelect = $('#week'),
    rootUrl = $('#loader').data('request-url'),
    personalTimetableDiv = $('#personalTimetableDiv'),
    url = rootUrl + 'Timetable/GetPersonalData';

$(function () {
    var formSelect = $('.form-select');

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

    var termId = $('#term option:last-child').val();
    var week = 0;
    $('#term').val(termId).trigger('change');
    // Get Partial View personal timetable data
    $.get(url, { termId, week }, function (data) {
        if (!data.error) {
            // Populate personal timetable
            personalTimetableDiv.html(data);
        } else {
            // Return not found error message
            personalTimetableDiv.html(data.message);
            weekSelect.parent().find('.select2-selection__placeholder').text('không khả dụng');
        }
    });
});

weekSelect.change(function () {
    // Display loading message while fetching data
    personalTimetableDiv.html('<div class="d-flex justify-content-center mt-2"><div class="spinner-border text-primary me-1" role="status"><span class="visually-hidden">Loading...</span></div><p class="my-auto">Đang tải...</p></div>');

    var termId = $('#term').val(),
        week = $(this).val();
    // Get Partial View timetable data
    $.get(url, { termId, week }, function (data) {
        personalTimetableDiv.html(data);
    });
});
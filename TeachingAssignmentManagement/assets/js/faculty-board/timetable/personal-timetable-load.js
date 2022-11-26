var rootUrl = $('#loader').data('request-url'),
    personalTimetableDiv = $('#personalTimetableDiv'),
    url = rootUrl + 'FacultyBoard/Timetable/GetPersonalData';

$(function () {
    var formSelect = $('.form-select');

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

    var termId = $('#term option:last-child').val();
    $('#term').val(termId).trigger('change');
    // Get Partial View personal timetable data
    $.get(url, { termId }, function (data) {
        personalTimetableDiv.html(data);
    });
});
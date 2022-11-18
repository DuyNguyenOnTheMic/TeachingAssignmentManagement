var rootUrl = $('#loader').data('request-url');

$(function () {
    var formSelect = $('.form-select'),
        assignLecturerDiv = $('#assignLecturerDiv'),
        url = rootUrl + 'FacultyBoard/Timetable/GetData';

    // select2
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

    formSelect.change(function () {
        var termId = $('#term').val(),
            majorId = $('#major').val();

        if (termId && majorId) {
            // Display loading message while fetching data
            assignLecturerDiv.html('<div class="d-flex justify-content-center"><div class="spinner-border text-primary me-1" role="status"><span class="visually-hidden">Loading...</span></div><p class="mt-25">Đang tải...</p></div>');

            // Get Partial View timetable data
            $.get(url, { termId, majorId }, function (data) {
                assignLecturerDiv.html(data);
            });
        }
    });
});
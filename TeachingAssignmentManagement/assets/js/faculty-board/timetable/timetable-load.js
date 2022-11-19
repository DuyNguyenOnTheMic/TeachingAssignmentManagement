var rootUrl = $('#loader').data('request-url');

// Reference the hub
var hubNotif = $.connection.curriculumClassHub;
// Start the connection
$.connection.hub.start();
// Notify while anyChanges
hubNotif.client.updatedData = function (id, lecturerId, isUpdate) {
    var element = $('#' + id);
    if (element.length) {
        if (isUpdate) {
            var $this = element.find('.form-select');
            $this.select2('destroy');
            $this.val(lecturerId); // Notify any JS components that the value changed
            alert($this.data('pretext'));
            $this.children('option[value = "' + lecturerId + '"]').text('hehe');
            $this.trigger('change');
        } else {
            element.remove();
        }
    }
}

$(function () {
    var formSelect = $('.form-select'),
        assignLecturerDiv = $('#assignLecturerDiv'),
        url = rootUrl + 'FacultyBoard/Timetable/GetData';

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
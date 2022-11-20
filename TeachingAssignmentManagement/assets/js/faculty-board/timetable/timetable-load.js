var rootUrl = $('#loader').data('request-url');

// Reference the hub
var hubNotif = $.connection.curriculumClassHub;
// Start the connection
$.connection.hub.start();
// Notify while anyChanges
hubNotif.client.updatedData = function (id, lecturerId, lecturerName, isUpdate) {
    var element = $('#' + id);
    if (element.length) {
        if (isUpdate) {
            var $this = element.find('.form-select');

            // Destroy select2 to update option text
            $this.select2('destroy').unwrap();

            // Set new selected values and change old value text
            $this.val(lecturerId);
            $this.children('option[value = "' + $this.data('preval') + '"]').text($this.data('pretext'));
            null != lecturerName && $this.children('option[value = "' + lecturerId + '"]').text(splitString(lecturerName));

            // Set data for preval and pretext
            $this.data('preval', lecturerId);
            $this.data('pretext', lecturerName);

            // Populate select2 dropdown again
            populateSelect($this);
        } else {
            // Delete curriculum class
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
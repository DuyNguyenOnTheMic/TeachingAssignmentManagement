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
            // Update curriculum class
            updateClass(element, lecturerId, lecturerName);
        } else {
            // Delete curriculum class
            element.popover('dispose');
            element.remove();
        }
    }
}

$(function () {
    var formSelect = $('.form-select'),
        assignLecturerDiv = $('#assignLecturerDiv'),
        url = rootUrl + 'FacultyBoard/Timetable/GetData';

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

    formSelect.change(function () {
        var termId = $('#term').val(),
            majorId = $('#major').val();

        // Hide all popovers
        $("[data-bs-toggle='popover']").popover('dispose');

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

function populateSelect($this) {
    // Populate select2
    $this.wrap('<div class="position-relative"></div>');
    $this.select2({
        language: 'vi',
        dropdownAutoWidth: true,
        dropdownParent: $this.parent()
    })
}

function updateClass(element, lecturerId, lecturerName) {
    element.data('lecturerid', lecturerId);
    if (lecturerId) {
        var classType = element.data('classtype');
        element.removeClass('btn-success btn-warning btn-secondary');
        if (classType == 'Lý thuyết') {
            // color of theory class
            element.addClass('btn-success');
        } else {
            // color of practical class
            element.addClass('btn-warning');
        }
        lecturerName = splitString(lecturerName);
    } else {
        lecturerName = "Chưa phân";
        element.removeClass('btn-success btn-warning').addClass('btn-secondary');
    }
    element.text(lecturerName);
    $('[data-bs-toggle="popover"]').popover('update');
}
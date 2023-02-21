var formSelect = $('.form-select'),
    rootUrl = $('#loader').data('request-url'),
    subjectDiv = $('#subjectDiv'),
    url = rootUrl + 'Remuneration/GetSubjectPartial';

// Reference the hub
var hubNotif = $.connection.timetableHub;
// Start the connection
$.connection.hub.start();
// Notify while anyChanges
hubNotif.client.refreshedData = function (term, major) {
    if (term == termId && major == majorId) {
        // Refresh timetable after someone import or re-import data
        getSubjectData(term, major);
    } else if (term == termId && majorId == -1) {
        // Refresh timetable when user is viewing all majors
        major = -1;
        getSubjectData(term, major);
    } else if (term == termId && major == null) {
        // Refresh timetable when someone change term status
        getSubjectData(term, majorId);
    }
}

$(function () {
    // Set selected option when form load
    formSelect.each(function () {
        var $this = $(this);
        $this.val($this.find('option:first').next().val());
    });

    // Append option to select all major
    $("#major option:first").after('<option value="-1">Tất cả</option>');

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

    // Fetch data on form load
    fetchData();
});

formSelect.change(function () {
    fetchData();
});

function fetchData() {
    var termId = $('#term').val(),
        majorId = $('#major').val();
    if (termId && majorId) {
        !0 === $(".ui-dialog-content").dialog("isOpen") && $(".ui-dialog-content").dialog("close");
        getSubjectData(termId, majorId);
    } else {
        subjectDiv.html('<h4 class="text-center mt-2">Chưa có dữ liệu học kỳ</h4><div class="card-body"><img class="mx-auto p-3 d-block w-50" alt="No data" src="' + rootUrl + 'assets/images/img_no_data.svg"></div>');
    }
}

function getSubjectData(termId, majorId) {
    if (termId && majorId) {
        // Display loading message while fetching data
        showLoading(subjectDiv);

        // Get Partial View Subject data
        $.get(url, { termId, majorId }, function (data) {
            subjectDiv.html(data);
        });
    }
}
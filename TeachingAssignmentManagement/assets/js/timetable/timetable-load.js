var formSelect = $('.form-select'),
    rootUrl = $('#loader').data('request-url'),
    assignLecturerDiv = $('#assignLecturerDiv'),
    url = rootUrl + 'Timetable/GetData';

// Reference the hub
var hubNotif = $.connection.timetableHub;
// Start the connection
$.connection.hub.start();
// Notify while anyChanges
hubNotif.client.updatedData = function (id, lecturerId, lecturerName, currentLecturerName, isUpdate) {
    var element = $('#' + id);
    if (element.length) {
        if (isUpdate) {
            // Update class
            updateClass(element, lecturerId, lecturerName, currentLecturerName);
        } else {
            // Delete class
            element.parent().tooltip('dispose');
            element.popover('dispose');
            element.remove();
        }
        updateTotalCount();
    }
}

hubNotif.client.refreshedData = function (term, major) {
    if (term == termId && major == majorId) {
        // Refresh timetable after someone import or re-import data
        getTimetable(term, major);
    } else if (term == termId && majorId == -1) {
        // Refresh timetable when user is viewing all majors
        major = -1;
        getTimetable(term, major);
    } else if (term == termId && major == null) {
        // Refresh timetable when someone change term status
        getTimetable(term, majorId);
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

function populateSelect($this) {
    // Populate select2
    $this.wrap('<div class="position-relative"></div>');
    $this.select2({
        language: 'vi',
        dropdownAutoWidth: true,
        dropdownParent: $this.parent(),
        placeholder: $this[0][0].innerHTML,
        allowClear: true
    })
}

function updateClass(element, lecturerId, lecturerName, currentLecturerName) {
    var classType = element.data('classtype');
    var tooltipElement = element.parent();
    var tooltipText = tooltipElement.attr('data-bs-original-title').split('<br /><b>Phân công bởi: </b>')[0];
    element.attr('data-lecturerid', lecturerId);
    element.removeClass('btn-success btn-warning btn-secondary unassigned-theory unassigned-practical');
    tooltipElement.tooltip('dispose');
    if (lecturerId) {
        if (classType == 'Lý thuyết') {
            // color of theory class
            element.addClass('btn-success');
        } else {
            // color of practical class
            element.addClass('btn-warning');
        }
        // Update tooltip text
        tooltipElement.attr('title', tooltipText + '<br /><b>Phân công bởi: </b>' + currentLecturerName);
        lecturerName = splitString(lecturerName);
    } else {
        lecturerName = 'Chưa phân';
        if (classType == 'Lý thuyết') {
            // color of theory class
            element.addClass('btn-secondary unassigned-theory');
        } else {
            // color of practical class
            element.addClass('btn-secondary unassigned-practical');
        }
        tooltipElement.attr('title', tooltipText);
    }
    element.text(lecturerName);
    tooltipElement.tooltip({
        trigger: 'hover'
    });
    $('[data-bs-toggle="popover"]').popover('update');
}

function updateTotalCount() {
    var assignedCount = $('#assignedCount'),
        totalCount = $('#totalCount'),
        allClass = $('#tblAssign tbody .assign-card'),
        theoreticalClass = $('#tblAssign tbody .btn-success'),
        practicalClass = $('#tblAssign tbody .btn-warning');
    assignedCount.text(theoreticalClass.length + practicalClass.length);
    totalCount.text(allClass.length);
}

function updateClassCount() {
    var theoreticalCount = $('#theoreticalCount'),
        practicalCount = $('#practicalCount'),
        theoreticalClass = $('#tblAssign tbody .assign-card[data-classtype="Lý thuyết"]'),
        practicalClass = $('#tblAssign tbody .assign-card[data-classtype="Thực hành"]');
    theoreticalCount.text(theoreticalClass.length);
    practicalCount.text(practicalClass.length)
}

function fetchData() {
    var termId = $('#term').val(),
        majorId = $('#major').val();
    if (termId && majorId) {
        getTimetable(termId, majorId);
    } else {
        showNoData(assignLecturerDiv, 'học kỳ');
    }
}

function getTimetable(termId, majorId) {
    // Dispose all tooltips and popovers
    $("#tblAssign [data-bs-toggle='tooltip']").tooltip('dispose');
    $("#tblAssign [data-bs-toggle='popover']").popover('dispose');

    if (termId && majorId) {
        // Display loading message while fetching data
        showLoading(assignLecturerDiv);

        // Get Partial View timetable data
        $.get(url, { termId, majorId }, function (data) {
            assignLecturerDiv.html(data);
        });
    }
}
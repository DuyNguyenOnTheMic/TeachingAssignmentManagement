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
        updateCount();
    }
}

hubNotif.client.refreshedData = function () {
    // Refresh data when hub is called
    fetchData();
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

function updateCount() {
    var assignedCount = $('#assignedCount'),
        totalCount = $('#totalCount'),
        allClass = $('#tblAssign tbody .assign-card'),
        theoryClass = $('#tblAssign tbody .btn-success'),
        practicalClass = $('#tblAssign tbody .btn-warning');
    assignedCount.text(theoryClass.length + practicalClass.length);
    totalCount.text(allClass.length);
}

function fetchData() {
    var termId = $('#term').val(),
        majorId = $('#major').val();
    if (termId && majorId) {
        getTimetable(termId, majorId);
    } else {
        assignLecturerDiv.html('<h4 class="text-center mt-2">Chưa có dữ liệu học kỳ</h4><div class="card-body"><img class="mx-auto p-3 d-block w-50" alt="No data" src="' + rootUrl + 'assets/images/img_no_data.svg"></div>');
    }
}

function getTimetable(termId, majorId) {
    // Dispose all tooltips and popovers
    $("#tblAssign [data-bs-toggle='tooltip']").tooltip('dispose');
    $("#tblAssign [data-bs-toggle='popover']").popover('dispose');

    if (termId && majorId) {
        // Display loading message while fetching data
        assignLecturerDiv.html('<div class="d-flex justify-content-center mt-2"><div class="spinner-border text-primary me-1" role="status"><span class="visually-hidden">Loading...</span></div><p class="my-auto">Đang tải...</p></div>');

        // Get Partial View timetable data
        $.get(url, { termId, majorId }, function (data) {
            assignLecturerDiv.html(data);
        });
    }
}
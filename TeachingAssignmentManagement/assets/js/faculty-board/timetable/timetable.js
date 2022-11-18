/*// function uses for real time update
setInterval(function () {
    $("#tblAssign").load("Timetable/GetData #tblAssign", { termId, majorId });
}, 2000); //refresh every 2 seconds*/
//alert($('#tblAssign tbody tr').length)

var termId = $('#term').val(),
    majorId = $('#major').val(),
    rootUrl = $('#loader').data('request-url');

$('table .form-select option:selected').each(function () {
    var selectedLecturer = $(this);
    if (selectedLecturer.val() != '') {
        // Add previous selected value to jquery data
        selectedLecturer.closest('.form-select').attr('data-preval', selectedLecturer.val());
        selectedLecturer.closest('.form-select').attr('data-pretext', selectedLecturer.text());

        var lecturerName = splitString(selectedLecturer.text());
        $(this).text(lecturerName);
    }
});

$('table .form-select').each(function () {
    // Populate select2 dropdown
    populateSelect($(this));
})

$('table .form-select').on('select2:select select2:unselecting', function () {
    var $this = $(this);

    // Destroy select2 to update option text
    $this.select2('destroy');

    // Split new selected lecturer name
    var newSelected = $this.find(':selected');
    var currentVal = newSelected.val();
    var currentText = newSelected.text();
    var newSelectedText = splitString(newSelected.text());
    newSelected.text(newSelectedText);

    // Change previous selected option value and text
    var preVal = $this.data('preval');
    var preText = $this.data('pretext');
    $this.children('option[value = "' + preVal + '"]').text(preText);

    // Set data for preval and pretext
    $this.data('preval', currentVal);
    $this.data('pretext', currentText);

    // Populate select2 dropdown again
    populateSelect($this);
});

$('.btn-assign').on('click', function () {
    $this = $(this);

    // Get values
    var curriculumClassId = $this.data('class');
    var day = $this.data('day');
    var roomId = $this.data('room');
    var lecturerId = $this.parent().find('.select2 :selected').val();

    if (lecturerId) {

        // Send ajax request to assign lecturer
        $.ajax({
            type: 'POST',
            url: rootUrl + 'FacultyBoard/Timetable/Assign',
            data: { termId, majorId, curriculumClassId, day, roomId, lecturerId },
            success: function (data) {
                if (data.success) {
                    toastr.options.positionClass = 'toast-bottom-right';
                    toastr.success('Thành công!');
                }
            }
        });
    }

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

function splitString(lecturerName) {
    // Split lecture name
    var removeLastWord = lecturerName.substring(0, lecturerName.lastIndexOf(' '));
    var splitName = removeLastWord.split(' ').map(function (item) { return item[0] }).join('.');
    var result = splitName + " " + lecturerName.split(' ').pop();
    return result;
}
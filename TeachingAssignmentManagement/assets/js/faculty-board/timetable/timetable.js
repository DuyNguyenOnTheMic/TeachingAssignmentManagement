/*var termId = $('#term').val(),
    majorId = $('#major').val();
// function uses for real time update
setInterval(function () {
    $("#tblAssign").load("Timetable/GetData #tblAssign", { termId, majorId });
}, 2000); //refresh every 2 seconds*/
//alert($('#tblAssign tbody tr').length)

$('table .form-select option:selected').each(function () {
    var selectedLecturer = $(this);

    // Add previous selected value to jquery data
    selectedLecturer.closest('.form-select').attr('data-preval', selectedLecturer.val());
    selectedLecturer.closest('.form-select').attr('data-pretext', selectedLecturer.text());

    if (selectedLecturer.val() != '') {
        var lecturerName = selectedLecturer.text();
        // Split lecture name
        var removeLastWord = lecturerName.substring(0, lecturerName.lastIndexOf(' '));
        var result = removeLastWord.split(' ').map(function (item) { return item[0] }).join('.');
        $(this).text(result + " " + lecturerName.split(' ').pop());
    }
});

$('table .form-select').each(function () {
    // Populate select2 dropdown
    populateSelect($(this));
})

$('table .form-select').on('change.select2', function () {
    var $this = $(this);

    // Destroy select2 to update option text
    $this.select2('destroy');
    var preVal = $this.data('preval');
    var preText = $this.data('pretext');
    $this.children('option[value = "' + preVal + '"]').text(preText);

    // Populate select2 dropdown again
    populateSelect($this);
});

function populateSelect($this) {
    // Populate select2
    $this.wrap('<div class="position-relative"></div>');
    $this.select2({
        language: 'vi',
        dropdownAutoWidth: true,
        dropdownParent: $this.parent(),
        placeholder: $this[0][0].innerHTML
    })
}
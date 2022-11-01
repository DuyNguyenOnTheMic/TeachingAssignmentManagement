var select = $('.select2');
select.each(function () {
    var $this = $(this);
    $this.wrap('<div class="position-relative"></div>');
    $this.select2({
        language: 'vi',
        dropdownAutoWidth: true,
        dropdownParent: $this.parent(),
        placeholder: $this[0][0].innerHTML
    });
})

var singleFile = $('#dpz-single-file');

// Basic example
singleFile.dropzone({
    url: '/FacultyBoard/Timetable/Import',
    paramName: 'file', // The name that will be used to transfer the file
    maxFiles: 1
});
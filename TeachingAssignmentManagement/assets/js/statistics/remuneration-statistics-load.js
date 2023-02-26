var termSelect = $('#term'),
    majorSelect = $('#major'),
    isLessonCheck = $('#isLesson'),
    formData = $('.form-data'),
    rootUrl = $('#loader').data('request-url'),
    statisticsDiv = $('#statisticsDiv'),
    latestTermId = $('#term option:eq(1)').val(),
    latestMajorId = $('#major option:eq(1)').val();

$(function () {
    var formSelect = $('.form-select');

    // Set latest term and major value
    termSelect.val(latestTermId);
    majorSelect.val(latestMajorId);

    // Populate select2
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

    // Append option to select all major
    $("#major option:first").after('<option value="-1">Tất cả</option>');

    if (latestTermId && latestMajorId) {
        // Get Partial View statistics data
        fetchData(false, latestTermId, latestMajorId);
    } else {
        showNoData(statisticsDiv, 'học kỳ');
    }
});

// Fetch data on form select change
formData.change(function () {
    var isLesson,
        value = termSelect.val(),
        major = majorSelect.val();

    // Check if user select unit lesson
    isLessonCheck.is(":checked") ? isLesson = true : isLesson = false;

    // Display loading message while fetching data
    showLoading(statisticsDiv);
    fetchData(isLesson, value, major);
});

function fetchData(isLesson, value, major) {
    var url = rootUrl + 'Statistics/GetRemunerationChart';
    $.get(url, { isLesson, value, major }, function (data) {
        // Populate statistics data
        statisticsDiv.html(data);
    });
}
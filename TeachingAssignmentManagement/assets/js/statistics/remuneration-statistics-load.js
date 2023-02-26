var termSelect = $('#term'),
    isLessonCheck = $('#isLesson'),
    formData = $('.form-data'),
    rootUrl = $('#loader').data('request-url'),
    statisticsDiv = $('#statisticsDiv'),
    latestTermId = $('#term option:eq(1)').val();

$(function () {
    var formSelect = $('.form-select');

    // Set latest term value
    termSelect.val(latestTermId);

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

    if (latestTermId) {
        // Get Partial View statistics data
        fetchData(false, latestTermId);
    } else {
        showNoData(statisticsDiv, 'học kỳ');
    }
});

// Fetch data on form select change
formData.change(function () {
    var isLesson,
        value = termSelect.val();

    // Check if user select unit lesson
    isLessonCheck.is(":checked") ? isLesson = true : isLesson = false;

    // Display loading message while fetching data
    showLoading(statisticsDiv);
    fetchData(isLesson, value);
});

function fetchData(isLesson, value) {
    var url = rootUrl + 'Statistics/GetRemunerationChart';
    $.get(url, { isLesson, value }, function (data) {
        // Populate statistics data
        statisticsDiv.html(data);
    });
}
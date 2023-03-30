var termSelect = $('#term'),
    rootUrl = $('#loader').data('request-url'),
    submit = $('#submit-all'),
    statisticsDiv = $('#statisticsDiv'),
    url = rootUrl + 'Statistics/GetVisitingLecturerData';

$(function () {
    // Populate select2
    termSelect.wrap('<div class="position-relative"></div>').select2({
        width: '100%',
        language: 'vi',
        dropdownAutoWidth: true,
        dropdownParent: termSelect.parent(),
        placeholder: termSelect[0][0].innerHTML
    });
});

submit.on('click', function () {
    var termId = termSelect.val();
    $.get(url, { termId }, function (data) {
        // Populate statistics data
        statisticsDiv.html(data);
    });
});
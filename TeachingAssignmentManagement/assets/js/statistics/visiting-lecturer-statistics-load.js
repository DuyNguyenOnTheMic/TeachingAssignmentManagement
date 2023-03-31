var termSelect = $('#term'),
    rootUrl = $('#loader').data('request-url'),
    submit = $('#submit-all'),
    statisticsDiv = $('#statisticsDiv'),
    url = rootUrl + 'Statistics/GetVisitingLecturerData';

// Populate select2
termSelect.wrap('<div class="position-relative"></div>').select2({
    width: '100%',
    language: 'vi',
    dropdownAutoWidth: true,
    dropdownParent: termSelect.parent(),
    placeholder: termSelect[0][0].innerHTML
});

submit.on('click', function () {
    var termIds = termSelect.val();
    $.ajax({
        type: 'GET',
        url: url,
        data: { termIds },
        traditional: true,
        success: function (data) {
            // Populate statistics data
            statisticsDiv.html(data);
        }
    });
});
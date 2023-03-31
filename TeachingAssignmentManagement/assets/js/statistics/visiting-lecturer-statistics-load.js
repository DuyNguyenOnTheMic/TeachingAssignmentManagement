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
    var termIds = termSelect.val();
    if (termIds.length) {
        // Disable button to prevent DDOS
        $(this).prop('disabled', true);
        setTimeout(() => $(this).prop('disabled', false), 1000);

        // Display loading message while fetching data
        showLoading(statisticsDiv);

        // Fetch statistics data
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
    } else {
        // Show not selected message
        toastr.warning('Bạn chưa chọn học kỳ để thống kê!');
    }
});
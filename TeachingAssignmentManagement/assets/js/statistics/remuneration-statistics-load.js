var termSelect = $('#term'),
    formData = $('.form-data'),
    rootUrl = $('#loader').data('request-url'),
    statisticsDiv = $('#statisticsDiv'),
    latestTermId = $('#term option:eq(1)').val();

$(function () {
    var formSelect = $('.form-select');

    // Set latest term and major value
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
        fetchData(latestTermId);
    } else {
        showNoData(statisticsDiv, 'học kỳ');
    }
});

// Fetch data on form select change
formData.change(function () {
    // Display loading message while fetching data
    showLoading(statisticsDiv);
    fetchData(value);
});

function fetchData(value) {
    var url = rootUrl + 'Statistics/GetRemunerationChart';
    $.get(url, { value }, function (data) {
        // Populate statistics data
        statisticsDiv.html(data);
    });
}
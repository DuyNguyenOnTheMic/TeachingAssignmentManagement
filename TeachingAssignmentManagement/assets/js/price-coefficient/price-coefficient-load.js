var formSelect = $('.form-select'),
    rootUrl = $('#loader').data('request-url'),
    priceCoefficientDiv = $('#priceCoefficientDiv'),
    url = rootUrl + 'Remuneration/GetPriceCoefficientData';

$(function () {
    // Set selected option when form load
    formSelect.each(function () {
        var $this = $(this);
        $this.val($this.find('option:first').next().val());
    });

    // Populate select2 for choosing year
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

function fetchData() {
    var schoolYear = $('#year').val(),
        yearSplit = schoolYear.split(" - "),
        startYear = yearSplit[0],
        endYear = yearSplit[1];
    if (startYear && endYear) {
        !0 === $(".ui-dialog-content").dialog("isOpen") && $(".ui-dialog-content").dialog("close");
        getPriceCoefficientData(startYear, endYear);
    } else {
        showNoData(priceCoefficientDiv, 'năm học');
    }
}

function getPriceCoefficientData(startYear, endYear) {
    // Display loading message while fetching data
    showLoading(priceCoefficientDiv);

    // Get Partial View Price Coefficient data
    $.get(url, { startYear, endYear }, function (data) {
        priceCoefficientDiv.html(data);
    });
}
var formSelect = $('.form-select'),
    rootUrl = $('#loader').data('request-url'),
    rankDiv = $('#priceCoefficientDiv'),
    url = rootUrl + 'Remuneration/GetPriceCoefficientData';

$(function () {
    // Set selected option when form load
    formSelect.each(function () {
        var $this = $(this);
        $this.val($this.find('option:first').next().val());
    });

    // Populate select2 for choosing term and major
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
        getRankData(startYear, endYear);
    } else {
        rankDiv.html('<h4 class="text-center mt-2">Chưa có dữ liệu năm học</h4><div class="card-body"><img class="mx-auto p-3 d-block w-50" alt="No data" src="' + rootUrl + 'assets/images/img_no_data.svg"></div>');
    }
}

function getRankData(startYear, endYear) {
    // Display loading message while fetching data
    rankDiv.html('<div class="d-flex justify-content-center mt-2"><div class="spinner-border text-primary me-1" role="status"><span class="visually-hidden">Loading...</span></div><p class="my-auto">Đang tải...</p></div>');

    // Get Partial View Rank data
    $.get(url, { startYear, endYear }, function (data) {
        rankDiv.html(data);
    });
}
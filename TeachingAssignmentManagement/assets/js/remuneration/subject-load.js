var formSelect = $('.form-select'),
    rootUrl = $('#loader').data('request-url'),
    subjectDiv = $('#subjectDiv'),
    url = rootUrl + 'Remuneration/GetSubjectData';

$(function () {
    // Set selected option when form load
    formSelect.each(function () {
        var $this = $(this);
        $this.val($this.find('option:first').next().val());
    });

    // Append option to select all major
    $("#major option:first").after('<option value="-1">Tất cả</option>');

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
    var termId = $('#term').val(),
        majorId = $('#major').val();
    if (termId && majorId) {
        getSubjectData(termId, majorId);
    } else {
        subjectDiv.html('<h4 class="text-center mt-2">Chưa có dữ liệu học kỳ</h4><div class="card-body"><img class="mx-auto p-3 d-block w-50" alt="No data" src="' + rootUrl + 'assets/images/img_no_data.svg"></div>');
    }
}

function getSubjectData(termId, majorId) {
    if (termId && majorId) {
        // Display loading message while fetching data
        subjectDiv.html('<div class="d-flex justify-content-center mt-2"><div class="spinner-border text-primary me-1" role="status"><span class="visually-hidden">Loading...</span></div><p class="my-auto">Đang tải...</p></div>');

        // Get Partial View Subject data
        $.get(url, { termId, majorId }, function (data) {
            subjectDiv.html(data);
        });
    }
}
var formSelect = $('.form-select'),
    rootUrl = $('#loader').data('request-url'),
    lecturerRankDiv = $('#lecturerRankDiv'),
    url = rootUrl + 'Remuneration/GetLecturerRankPartial';

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
    var termId = $('#term').val();
    if (termId) {
        !0 === $(".ui-dialog-content").dialog("isOpen") && $(".ui-dialog-content").dialog("close");
        getLecturerRankData(termId);
    } else {
        lecturerRankDiv.html('<h4 class="text-center mt-2">Chưa có dữ liệu học kỳ</h4><div class="card-body"><img class="mx-auto p-3 d-block w-50" alt="No data" src="' + rootUrl + 'assets/images/img_no_data.svg"></div>');
    }
}

function getLecturerRankData(termId) {
    if (termId) {
        // Display loading message while fetching data
        showLoading(lecturerRankDiv);

        // Get Partial View Lecturer Rank data
        $.get(url, { termId }, function (data) {
            lecturerRankDiv.html(data);
        });
    }
}
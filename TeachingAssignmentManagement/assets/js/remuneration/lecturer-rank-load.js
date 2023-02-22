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

    // Populate select2 for choosing term
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
        showNoData(lecturerRankDiv, 'học kỳ');
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
﻿var termSelect = $('#term'),
    rootUrl = $('#loader').data('request-url'),
    statisticsDiv = $('#statisticsDiv'),
    url = rootUrl + 'Statistics/GetChart';

$(function () {

    var formSelect = $('.form-select');

    // Populate select2 for choosing term and week
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

    var termId = $('#term option:eq(1)').val();
    if (termId) {
        termSelect.val(termId).trigger('change');
        // Get Partial View personal timetable data
        fetchData(termId);
    } else {
        statisticsDiv.html('<h4 class="text-center mt-2">Chưa có dữ liệu học kỳ</h4><div class="card-body"><img class="mx-auto p-3 d-block w-50" alt="No data" src="' + rootUrl + 'assets/images/img_no_data.svg"></div>');
    }
});

function fetchData(termId) {
    $.get(url, { termId }, function (data) {
        // Populate personal timetable
        statisticsDiv.html(data);
    });
}
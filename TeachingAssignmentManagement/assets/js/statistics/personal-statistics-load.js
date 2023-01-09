﻿var unitSelect = $('#unit'),
    termSelect = $('#term'),
    yearSelect = $('#year'),
    isLessonCheck = $('#isLesson'),
    formData = $('.form-data'),
    rootUrl = $('#loader').data('request-url'),
    statisticsDiv = $('#statisticsDiv'),
    latestTermId = $('#term option:eq(1)').val(),
    latestYearId = $('#year option:eq(1)').val();

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
        fetchData(false, termSelect.attr('id'), latestTermId);
    } else {
        statisticsDiv.html('<h4 class="text-center mt-2">Chưa có dữ liệu học kỳ</h4><div class="card-body"><img class="mx-auto p-3 d-block w-50" alt="No data" src="' + rootUrl + 'assets/images/img_no_data.svg"></div>');
    }
});

// Unit select event for viewing statistics
unitSelect.change(function () {
    var $this = $(this);
    if ($this.val() == 'term') {
        // Show term select2 field
        $('#termDiv').show();
        $('#yearDiv').hide();

        // Set latest term
        termSelect.val(latestTermId).trigger('change');
    } else {
        // Show year select2 field
        $('#yearDiv').show();
        $('#termDiv').hide();

        // Set latest year
        yearSelect.val(latestYearId).trigger('change');
    }
});

// Fetch data on form select change
formData.change(function () {
    var isLesson,
        type,
        value,
        lecturerType;
    // Check if user select unit lesson
    isLessonCheck.is(":checked") ? isLesson = true : isLesson = false;
    // Check if term or year select is hidden
    if (termSelect.is(':visible')) {
        type = termSelect.attr('id');
        value = termSelect.val();
    } else {
        type = yearSelect.attr('id');
        value = yearSelect.val();
    }
    // Display loading message while fetching data
    loading();
    fetchData(isLesson, type, value);
});

function loading() {
    statisticsDiv.html('<div class="d-flex justify-content-center mt-2"><div class="spinner-border text-primary me-1" role="status"><span class="visually-hidden">Loading...</span></div><p class="my-auto">Đang tải...</p></div>');
}

function fetchData(isLesson, type, value) {
    var url = rootUrl + 'Statistics/GetPersonalChart';
    $.get(url, { isLesson, type, value }, function (data) {
        // Populate statistics data
        statisticsDiv.html(data);
    });
}
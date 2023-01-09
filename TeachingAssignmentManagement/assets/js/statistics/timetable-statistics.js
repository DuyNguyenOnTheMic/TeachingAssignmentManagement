﻿var termId = $('#term').val(),
    weekData = $('#weekData'),
    startWeek = weekData.data('start-week'),
    endWeek = weekData.data('end-week'),
    currentWeek = weekData.data('current-week'),
    rootUrl = $('#loader').data('request-url'),
    lecturerFilter = $('#lecturerFilter'),
    lecturerType = $('#lecturerType');

// Display message when table have no data
var classCount = $('#tblStatistics .class-card').length;
if (classCount == 0) {
    $('#timetableStatisticsDiv').empty().append('<h4 class="text-center mt-2">Chưa có dữ liệu <i class="feather feather-help-circle"></i></h4><div class="card-body"><img class="mx-auto p-3 d-block w-50" alt="Welcome" src="' + rootUrl + 'assets/images/img_no_data.svg"></div>');
} else {
    // Initialize Tooltip
    $('#tblStatistics [data-bs-toggle="tooltip"]').tooltip({
        trigger: 'hover'
    });

    // Initialize Popover
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl, {
            html: true,
            sanitize: false
        });
    });

    // Hide all table rows on load
    $('#tblStatistics tbody tr').hide();
}

if (!weekSelect.val()) {
    if (startWeek <= endWeek) {
        // Populate week select dropdown
        for (var i = startWeek; i <= endWeek; i++) {
            weekSelect.append('<option value="' + i + '">Tuần ' + i + '</option>');
        }
        weekSelect.val(currentWeek);
    } else {
        // Catch error if start week is more than end week
        timetableStatisticsDiv.empty().append('<h4 class="text-center mt-2">Oops, có vẻ như ban chủ nhiệm khoa của bạn đã đặt tuần bắt đầu lớn hơn tuần kết thúc, vui lòng liên hệ để sửa lỗi.</h4><div class="card-body"><img class="mx-auto p-3 d-block w-50" alt="Welcome" src="' + rootUrl + 'assets/images/img_sorry.svg"></div>');
        weekSelect.parent().find('.select2-selection__placeholder').text('không khả dụng');
    }
}

$.fn.select2.amd.define('select2/selectAllAdapter', [
    'select2/utils',
    'select2/dropdown',
    'select2/dropdown/attachBody'
], function (Utils, Dropdown, AttachBody) {

    function SelectAll() { }
    SelectAll.prototype.render = function (decorated) {
        var self = this,
            $rendered = decorated.call(this),
            $selectAll = $(
                '<button class="btn btn-sm text-start filter-button" type="button"><i class="feather feather-check-square"></i> Chọn tất cả</button>'
            ),
            $unselectAll = $(
                '<button class="btn btn-sm text-start filter-button" type="button"><i class="feather feather-square"></i> Bỏ chọn tất cả</button>'
            ),
            $btnContainer = $('<div class="d-grid"></div>').append($selectAll).append($unselectAll);
        if (!this.$element.prop("multiple")) {
            // this isn't a multi-select -> don't add the buttons!
            return $rendered;
        }
        $rendered.find('.select2-dropdown').prepend($btnContainer);
        $selectAll.on('click', function () {
            // Select All Options
            var visibleLecturers = lecturerFilter.find('option:visible');
            var visibleLength = visibleLecturers.length;
            visibleLecturers.prop('selected', 'selected').trigger('change');
            filterCount(lecturerFilter);
            self.trigger('close');
            for (var i = 0; i < visibleLength; i++) {
                $('#' + visibleLecturers[i].value).show();
            }
        });
        $unselectAll.on('click', function () {
            // Unselect All Options
            lecturerFilter.val('-1').trigger('change');
            lecturerFilter.parent().find('.select2-search__field').attr('placeholder', 'Lọc giảng viên');
            self.trigger('close');
            $('#tblStatistics').find('tbody tr').hide();
        });
        return $rendered;
    };

    return Utils.Decorate(
        Utils.Decorate(
            Dropdown,
            AttachBody
        ),
        SelectAll
    );

});

// Populate select2 for lecturer filter
lecturerFilter.wrap('<div class="position-relative my-50 me-50"></div>');
lecturerFilter.select2({
    language: 'vi',
    dropdownAutoWidth: true,
    dropdownParent: lecturerFilter.parent(),
    dropdownAdapter: $.fn.select2.amd.require('select2/selectAllAdapter')
})
lecturerFilter.parent().find('.select2-search__field').attr('placeholder', 'Lọc giảng viên');
lecturerFilter.on('select2:select', function (e) {
    // Show table row on select
    var lecturerId = $('#' + e.params.data.id);
    lecturerId.show();
    filterCount(lecturerFilter);
}).on('select2:unselect', function (e) {
    // Hide table row on unselect
    var lecturerId = $('#' + e.params.data.id);
    lecturerId.hide();
    filterCount(lecturerFilter);
});

// Populate select2 for lecturer type
lecturerType.wrap('<div class="position-relative my-50"></div>');
lecturerType.select2({
    language: 'vi',
    dropdownAutoWidth: true,
    dropdownParent: lecturerFilter.parent(),
    placeholder: lecturerType[0][0].innerHTML
}).on('select2:select', function (e) {
    // Hide options on select
})

// User guide for timetable statistics
$(function () {
    'use strict';

    var startBtn = $('#tour');
    function setupTour(tour) {
        var backBtnClass = 'btn btn-sm btn-outline-primary',
            nextBtnClass = 'btn btn-sm btn-primary btn-next';
        tour.addStep({
            title: 'Chọn tuần',
            text: 'Bạn có thể chọn tuần để xem TKB ờ đây. Mặc định hệ thống sẽ chọn tuần hiện tại.',
            attachTo: { element: '#week + .select2-container', on: 'bottom' },
            buttons: [
                {
                    action: tour.cancel,
                    classes: backBtnClass,
                    text: 'Bỏ qua'
                },
                {
                    text: 'Tiếp',
                    classes: nextBtnClass,
                    action: tour.next
                }
            ]
        });
        tour.addStep({
            title: 'Lọc giảng viên',
            text: 'Chọn giảng viên bạn muốn xem TKB ở đây.',
            attachTo: { element: '#lecturerFilter + .select2-container', on: 'bottom' },
            buttons: [
                {
                    text: 'Quay lại',
                    classes: backBtnClass,
                    action: tour.back
                },
                {
                    text: 'OK',
                    classes: nextBtnClass,
                    action: tour.cancel
                }
            ]
        });
        return tour;
    }

    if (startBtn.length) {
        startBtn.on('click', function () {
            var tourVar = new Shepherd.Tour({
                defaultStepOptions: {
                    classes: 'shadow-md bg-purple-dark',
                    scrollTo: false,
                    cancelIcon: {
                        enabled: true
                    }
                },
                useModalOverlay: true
            });

            setupTour(tourVar).start();
        });
    }
});

function filterCount(element) {
    element.parent().find('.select2-search__field').attr('placeholder', 'Đã chọn ' + element.val().length + ' GV');
}
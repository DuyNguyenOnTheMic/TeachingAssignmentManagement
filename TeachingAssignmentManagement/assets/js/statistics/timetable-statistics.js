var termId = $('#term').val(),
    weekData = $('#weekData'),
    startWeek = weekData.data('start-week'),
    endWeek = weekData.data('end-week'),
    currentWeek = weekData.data('current-week'),
    rootUrl = $('#loader').data('request-url'),
    tableStatistics = $('#tblStatistics'),
    lecturerFilter = $('#lecturerFilter'),
    lecturerType = $('#lecturerType'),
    lessonFilter = $('#lessonFilter'),
    dayFilter = $('#dayFilter');

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

    // Update focus for multiple select2
    $(document).on('select2:open', () => {
        document.querySelector('.select2-container--open .select2-search__field').focus();
    });
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
            lecturerFilter.find('option').prop('selected', 'selected').trigger('change');
            lecturerType.val('-1').trigger('change');
            resetLecturerType();
            filterCount(lecturerFilter, 'GV');
            self.trigger('close');
            $('#tblStatistics tbody tr').show();
        });
        $unselectAll.on('click', function () {
            // Unselect All Options
            lecturerFilter.val('-1').trigger('change');
            lecturerType.val('-1').trigger('change');
            resetLecturerFilter();
            resetLecturerType();
            self.trigger('close');
            $('#tblStatistics tbody tr').hide();
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
lecturerFilter.wrap('<div class="position-relative my-50 me-1"></div>');
lecturerFilter.select2({
    language: 'vi',
    dropdownAutoWidth: true,
    dropdownParent: lecturerFilter.parent(),
    dropdownAdapter: $.fn.select2.amd.require('select2/selectAllAdapter')
})
resetLecturerFilter();
lecturerFilter.on('select2:select', function (e) {
    // Show table row on select
    var lecturerId = $('#' + e.params.data.id);
    lecturerId.show();
    filterCount(lecturerFilter, 'GV');
}).on('select2:unselect', function (e) {
    // Hide table row on unselect
    var lecturerId = $('#' + e.params.data.id);
    lecturerId.hide();
    filterCount(lecturerFilter, 'GV');
});

// Populate select2 for lecturer type
lecturerType.wrap('<div class="position-relative my-50 me-1"></div>');
lecturerType.select2({
    language: 'vi',
    dropdownAutoWidth: true,
    dropdownParent: lecturerType.parent(),
    placeholder: lecturerType[0][0].innerHTML,
    minimumResultsForSearch: Infinity
})
resetLecturerType();
lecturerType.on('select2:select', function (e) {
    // Select lecturers based on lecturer type
    $(this).val('-1').val(e.params.data.id).trigger('change');
    lecturerFilter.val('-1').find('option[data-type="' + e.params.data.id + '"]').prop('selected', 'selected').trigger('change');
    filterCount(lecturerFilter, 'GV');
    lecturerType.parent().find('.select2-search__field').attr('placeholder', e.params.data.text);
    $('#tblStatistics tbody tr').hide();
    $('#tblStatistics tbody tr[data-type="' + e.params.data.id + '"]').show();
}).on('select2:unselect', function () {
    // Reset placeholder for lecturer type
    resetLecturerType();
});

// Populate select2 for lesson filter
lessonFilter.wrap('<div class="position-relative my-50 me-1"></div>');
lessonFilter.select2({
    language: 'vi',
    dropdownAutoWidth: true,
    dropdownParent: lessonFilter.parent(),
    closeOnSelect: false
})
lessonFilter.parent().find('.select2-search__field').attr('placeholder', 'Lọc ca giảng');
lessonFilter.on('select2:select', function (e) {
    // Show lesson column on select
    var lesson = e.params.data.id;
    var colspan = lessonFilter.val().length;
    var visibleDays = dayFilter.val();
    // Only filter current visible days
    for (var i = 0; i < visibleDays.length; i++) {
        var visibleDay = visibleDays[i];
        tableStatistics.find('td[data-day="' + visibleDay + '"][data-startlesson="' + lesson + '"], th[data-day="' + visibleDay + '"][data-startlesson="' + lesson + '"]').show();
    }
    tableStatistics.find('thead .day-header').attr('colspan', colspan);
    filterCount(lessonFilter, 'ca giảng');

    // Remove divider class
    tableStatistics.find('td[data-startlesson!="1"]').removeClass('table-vertical-divider');

    // Add divider class for viewing between days in week
    updateDivider();
}).on('select2:unselect', function (e) {
    // Hide lesson column on unselect
    var lesson = e.params.data.id;
    var colspan = lessonFilter.val().length;
    tableStatistics.find('td[data-startlesson="' + lesson + '"], th[data-startlesson="' + lesson + '"]').hide();
    tableStatistics.find('thead .day-header').attr('colspan', colspan);
    filterCount(lessonFilter, 'ca giảng');

    // Add divider class for viewing between days in week
    updateDivider();
});

// Populate select2 for day filter
dayFilter.wrap('<div class="position-relative my-50"></div>');
dayFilter.select2({
    language: 'vi',
    dropdownAutoWidth: true,
    dropdownParent: dayFilter.parent(),
    closeOnSelect: false
})
dayFilter.parent().find('.select2-search__field').attr('placeholder', 'Lọc thứ');
dayFilter.on('select2:select', function (e) {
    // Show day column on select
    var day = e.params.data.id;
    var visibleLessons = lessonFilter.val();
    // Only filter current visible lessons
    for (var i = 0; i < visibleLessons.length; i++) {
        var visibleLesson = visibleLessons[i];
        tableStatistics.find('thead .day-header[data-day="' + day + '"]').show();
        tableStatistics.find('td[data-day="' + day + '"][data-startlesson="' + visibleLesson + '"], th[data-day="' + day + '"][data-startlesson="' + visibleLesson + '"]').show();
    }
    filterCount(dayFilter, 'thứ');
}).on('select2:unselect', function (e) {
    // Hide day column on unselect
    var day = e.params.data.id;
    tableStatistics.find('td[data-day="' + day + '"], th[data-day="' + day + '"]').hide();
    filterCount(dayFilter, 'thứ');
});

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
                    text: 'Bỏ qua',
                    classes: backBtnClass,
                    action: tour.cancel
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
                    text: 'Bỏ qua',
                    classes: backBtnClass,
                    action: tour.cancel
                },
                {
                    text: 'Quay lại',
                    classes: backBtnClass,
                    action: tour.back
                },
                {
                    text: 'Tiếp',
                    classes: nextBtnClass,
                    action: tour.next
                }
            ]
        });
        tour.addStep({
            title: 'Lọc loại giảng viên',
            text: 'Chọn và xem thời khoá biểu theo loại giảng viên ở đây.',
            attachTo: { element: '#lecturerType + .select2-container', on: 'bottom' },
            buttons: [
                {
                    text: 'Bỏ qua',
                    classes: backBtnClass,
                    action: tour.cancel
                },
                {
                    text: 'Quay lại',
                    classes: backBtnClass,
                    action: tour.back
                },
                {
                    text: 'Tiếp',
                    classes: nextBtnClass,
                    action: tour.next
                }
            ]
        });
        tour.addStep({
            title: 'Lọc ca giảng',
            text: 'Xem và lọc thời khoá biểu theo ca giảng ở đây.',
            attachTo: { element: '#lessonFilter + .select2-container', on: 'bottom' },
            buttons: [
                {
                    text: 'Bỏ qua',
                    classes: backBtnClass,
                    action: tour.cancel
                },
                {
                    text: 'Quay lại',
                    classes: backBtnClass,
                    action: tour.back
                },
                {
                    text: 'Tiếp',
                    classes: nextBtnClass,
                    action: tour.next
                }
            ]
        });
        tour.addStep({
            title: 'Lọc thứ',
            text: 'Xem và lọc theo thứ ở đây.',
            attachTo: { element: '#dayFilter + .select2-container', on: 'bottom' },
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

function filterCount(element, text) {
    element.parent().find('.select2-search__field').attr('placeholder', 'Đã chọn ' + element.val().length + ' ' + text);
}

function resetLecturerFilter() {
    lecturerFilter.parent().find('.select2-search__field').attr('placeholder', 'Lọc giảng viên');
}

function resetLecturerType() {
    lecturerType.parent().find('.select2-search__field').attr('placeholder', 'Lọc loại giảng viên');
}

function updateDivider() {
    if (tableStatistics.find('thead th[data-startlesson="1"]').is(':hidden')) {
        var firstVisible = tableStatistics.find('thead th.lesson-header:visible').first().data('startlesson');
        tableStatistics.find('td[data-startlesson="' + firstVisible + '"]').addClass('table-vertical-divider');
    }
}
var weekSelect = $('#week'),
    weekData = $('#weekData'),
    startWeek = weekData.data('start-week'),
    endWeek = weekData.data('end-week'),
    currentWeek = weekData.data('current-week');

// Initialize Tooltip
$('[data-bs-toggle="tooltip"]').tooltip({
    trigger: 'hover'
});

if (!weekSelect.val()) {
    if (startWeek <= endWeek) {
        // Populate week select dropdown
        for (var i = startWeek; i <= endWeek; i++) {
            weekSelect.append('<option value="' + i + '">Tuần ' + i + '</option>');
        }
        weekSelect.val(currentWeek);
    } else {
        // Catch error if start week is more than end week
        $('#personalTimetableDiv').empty().append('<h4 class="text-center mt-2">Oops, có vẻ như ban chủ nhiệm khoa của bạn đã đặt tuần bắt đầu lớn hơn tuần kết thúc, vui lòng liên hệ để sửa lỗi.</h4><div class="card-body"><img class="mx-auto p-3 d-block w-50" alt="Welcome" src="' + rootUrl + 'assets/images/img_sorry.svg"></div>');
        weekSelect.parent().find('.select2-selection__placeholder').text('không khả dụng');
    }
}
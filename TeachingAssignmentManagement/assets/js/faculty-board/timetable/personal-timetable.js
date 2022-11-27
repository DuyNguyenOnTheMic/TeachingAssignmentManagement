var weekSelect = $('#week'),
    weekData = $('#weekData'),
    startWeek = weekData.data('start-week'),
    endWeek = weekData.data('end-week'),
    currentWeek = weekData.data('current-week');

if (!weekSelect.val()) {
    for (var i = startWeek; i <= endWeek; i++) {
        weekSelect.append('<option value="' + i + '">Tuần ' + i + '</option>');
    }
    weekSelect.val(currentWeek);
}
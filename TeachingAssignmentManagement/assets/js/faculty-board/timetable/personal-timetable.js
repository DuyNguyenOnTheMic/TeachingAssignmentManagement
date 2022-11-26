var weekSelect = $('#week');
    startWeek = $('#startWeek').data('week'),
    endWeek = $('#endWeek').data('week');

for (var i = startWeek; i <= endWeek; i++) {
    weekSelect.append('<option value="' + i + '">Tuần ' + i + '</option>');
}
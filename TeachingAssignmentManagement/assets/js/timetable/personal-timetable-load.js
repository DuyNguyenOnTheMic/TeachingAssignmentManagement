var formSelect = $('.form-select'),
  termSelect = $('#term'),
  weekSelect = $('#week'),
  lecturerSelect = $('#lecturer'),
  rootUrl = $('#loader').data('request-url'),
  personalTimetableDiv = $('#personalTimetableDiv'),
  url = rootUrl + 'Timetable/GetPersonalData'

$(function () {
  // Set latest term option
  var termId = $('#term option:eq(1)').val(),
    week = 0,
    lecturerId = lecturerSelect.val()
  termSelect.val(termId)

  // Populate select2 for choosing term and week
  formSelect.each(function () {
    var $this = $(this)
    $this.wrap('<div class="position-relative"></div>')
    $this.select2({
      language: 'vi',
      dropdownAutoWidth: true,
      dropdownParent: $this.parent(),
      placeholder: $this[0][0].innerHTML
    })
  })

  if (termId) {
    if (lecturerId) {
      // Get Partial View personal timetable data
      fetchData(termId, week, lecturerId)
    } else {
      personalTimetableDiv.html(
        '<h4 class="text-center mt-2">Bạn chưa điền đẩy đủ mã giảng viên và tên giảng viên</h4><div class="card-body"><img class="mx-auto p-3 d-block w-50" alt="No data" src="' +
          rootUrl +
          'assets/images/img_no_data.svg"></div>'
      )
    }
  } else {
    showNoData(personalTimetableDiv, 'học kỳ')
    weekSelect.parent().find('.select2-selection__placeholder').text('không khả dụng')
  }
})

formSelect.on('change', function () {
  var termId = termSelect.val(),
    week = weekSelect.val(),
    lecturerId = $('#lecturer').val()
  if (termId && lecturerId) {
    if (!$(this).is(weekSelect)) {
      // Empty week select to re-populate weeks
      weekSelect.empty()
      week = 0
    }
    loading()
    fetchData(termId, week, lecturerId)
  }
})

function loading() {
  // Display loading message while fetching data
  showLoading(personalTimetableDiv)

  // Dispose all tooltips
  $("[data-bs-toggle='tooltip']").tooltip('dispose')
}

function fetchData(termId, week, lecturerId) {
  // Get Partial View timetable data
  $.get(url, { termId, week, lecturerId }, function (data) {
    if (!data.error) {
      // Populate personal timetable
      personalTimetableDiv.html(data)
    } else {
      // Return not found error message
      showNoData(personalTimetableDiv, 'phân công trong học kỳ')
      weekSelect.parent().find('.select2-selection__placeholder').text('không khả dụng')
    }
  })
}

var unitSelect = $('#unit'),
  termSelect = $('#term'),
  yearSelect = $('#year'),
  majorSelect = $('#major'),
  isLessonCheck = $('#isLesson'),
  formData = $('.form-data'),
  rootUrl = $('#loader').data('request-url'),
  statisticsDiv = $('#statisticsDiv'),
  latestTermId = $('#term option:eq(1)').val(),
  latestYearId = $('#year option:eq(1)').val(),
  latestMajorId = $('#major option:eq(1)').val()

$(function () {
  var formSelect = $('.form-select')

  // Set latest term and major value
  termSelect.val(latestTermId)
  majorSelect.val(latestMajorId)

  // Populate select2
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

  // Append option to select all major
  $('#major option:first').after('<option value="-1">Tất cả</option>')

  if (latestTermId && latestMajorId) {
    // Get Partial View statistics data
    fetchData(false, termSelect.attr('id'), latestTermId, latestMajorId)
  } else {
    showNoData(statisticsDiv, 'học kỳ')
  }
})

// Unit select event for viewing statistics
unitSelect.change(function () {
  var $this = $(this)
  if ($this.val() == 'term') {
    // Show term select2 field
    $('#termDiv').show()
    $('#yearDiv').hide()

    // Set latest term
    termSelect.val(latestTermId).trigger('change')
  } else {
    // Show year select2 field
    $('#yearDiv').show()
    $('#termDiv').hide()

    // Set latest year
    yearSelect.val(latestYearId).trigger('change')
  }
})

// Fetch data on form select change
formData.change(function () {
  var isLesson, type, value, major
  // Check if user select unit lesson
  isLessonCheck.is(':checked') ? (isLesson = true) : (isLesson = false)
  // Check if term or year select is hidden
  if (termSelect.is(':visible')) {
    type = termSelect.attr('id')
    value = termSelect.val()
  } else {
    type = yearSelect.attr('id')
    value = yearSelect.val()
  }
  major = majorSelect.val()
  // Display loading message while fetching data
  showLoading(statisticsDiv)
  fetchData(isLesson, type, value, major)
})

function fetchData(isLesson, type, value, major) {
  var url = rootUrl + 'Statistics/GetRemunerationChart'
  $.get(url, { isLesson, type, value, major }, function (data) {
    // Populate statistics data
    statisticsDiv.html(data)
  })
}

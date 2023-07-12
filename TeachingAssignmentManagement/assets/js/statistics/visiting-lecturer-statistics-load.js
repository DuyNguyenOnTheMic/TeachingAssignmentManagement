var termSelect = $('#term'),
  rootUrl = $('#loader').data('request-url'),
  submit = $('#submit-all'),
  statisticsDiv = $('#statisticsDiv'),
  url = rootUrl + 'Statistics/GetVisitingLecturerData'

$(function () {
  $.fn.select2.amd.define(
    'select2/selectAllAdapter',
    ['select2/utils', 'select2/dropdown', 'select2/dropdown/attachBody'],
    function (Utils, Dropdown, AttachBody) {
      function SelectAll() {}
      SelectAll.prototype.render = function (decorated) {
        var self = this,
          $rendered = decorated.call(this),
          $selectAll = $(
            '<button class="btn btn-sm text-start filter-button" type="button"><i class="feather feather-check-square"></i> Chọn tất cả</button>'
          ),
          $unselectAll = $(
            '<button class="btn btn-sm text-start filter-button" type="button"><i class="feather feather-square"></i> Bỏ chọn tất cả</button>'
          ),
          $btnContainer = $('<div class="d-grid"></div>').append($selectAll).append($unselectAll)
        if (!this.$element.prop('multiple')) {
          // this isn't a multi-select -> don't add the buttons!
          return $rendered
        }
        $rendered.find('.select2-dropdown').prepend($btnContainer)
        $selectAll.on('click', function () {
          // Select All Options
          termSelect.find('option').prop('selected', true).trigger('change')
          filterCount(termSelect)
          self.trigger('close')
        })
        $unselectAll.on('click', function () {
          // Unselect All Options
          termSelect.val('-1').trigger('change')
          termSelect.parent().find('.select2-search__field').attr('placeholder', '---- Chọn học kỳ ----')
          self.trigger('close')
        })
        return $rendered
      }

      return Utils.Decorate(Utils.Decorate(Dropdown, AttachBody), SelectAll)
    }
  )

  // Populate select2
  termSelect.wrap('<div class="position-relative"></div>').select2({
    width: '100%',
    language: 'vi',
    dropdownAutoWidth: true,
    dropdownParent: termSelect.parent(),
    placeholder: termSelect[0][0].innerHTML,
    dropdownAdapter: $.fn.select2.amd.require('select2/selectAllAdapter')
  })
  termSelect.parent().find('.select2-search__field').attr('placeholder', '---- Chọn học kỳ ----')
  termSelect.on('change', function () {
    // Update filter count on change
    filterCount(termSelect)
  })
})

submit.on('click', function () {
  var termIds = termSelect.val()
  if (termIds.length) {
    // Disable button to prevent DDOS
    $(this).prop('disabled', true)
    setTimeout(() => $(this).prop('disabled', false), 1000)

    // Display loading message while fetching data
    showLoading(statisticsDiv)

    // Fetch statistics data
    $.ajax({
      type: 'GET',
      url: url,
      data: { termIds },
      traditional: true,
      success: function (data) {
        // Populate statistics data
        statisticsDiv.html(data)
      }
    })
  } else {
    // Show not selected message
    toastr.warning('Bạn chưa chọn học kỳ để thống kê!')
  }
})

function filterCount(element) {
  element
    .parent()
    .find('.select2-search__field')
    .attr('placeholder', 'Đã chọn ' + element.val().length + ' học kỳ')
}

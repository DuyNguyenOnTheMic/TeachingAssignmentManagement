﻿//Get the context of the Chart canvas element we want to select
var ctx = $('#statistics-chart'),
  chartWrapper = $('.chartjs')

var labelColor = '#6e6b7b',
  titleColor = '#666666',
  gridLineColor = 'rgba(200, 200, 200, 0.2)' // RGBA color helps in dark layout

// Setup data
var dataLoader = $('#data-loader'),
  isLesson = dataLoader.data('islesson'),
  type = dataLoader.data('type'),
  majorId = dataLoader.data('major'),
  majorAbb = dataLoader.data('majorabb'),
  majorName = dataLoader.data('majorname'),
  value = dataLoader.val(),
  url = rootUrl + 'Statistics/',
  titleText,
  fileName,
  detailsFileName,
  data

// Detect Dark Layout
if ($('html').hasClass('dark-layout')) {
  titleColor = '#d0d2d6'
  labelColor = '#b4b7bd'
}

// Check if user is selecting term or year
if (type == yearSelect.attr('id')) {
  var yearSplit = value.split(' - '),
    startYear = yearSplit[0],
    endYear = yearSplit[1]
  data = { isLesson, startYear, endYear, majorId }
  titleText = 'Thống kê số giờ quy đổi năm học ' + value + ' ngành ' + majorName
  fileName = 'ThongKeSoGioQuyDoi_NamHoc_' + startYear + '-' + endYear + '_Nganh_' + majorAbb
  detailsFileName = 'ThongKeChiTietSoGioQuyDoi_NamHoc_' + startYear + '-' + endYear + '_Nganh_' + majorAbb
  url += 'GetYearRemunerationData'
} else {
  data = { isLesson, termId: value, majorId }
  titleText = 'Thống kê số giờ quy đổi HK' + value + ' ngành ' + majorName
  fileName = 'ThongKeSoGioQuyDoi_HK' + value + '_Nganh_' + majorAbb
  detailsFileName = 'ThongKeChiTietSoGioQuyDoi_HK' + value + '_Nganh_' + majorAbb
  url += 'GetTermRemunerationData'
}

Chart.defaults.font.family = 'Montserrat,Helvetica,Arial,serif'

// Chart Options
var chartOptions = {
  indexAxis: 'y',
  responsive: true,
  maintainAspectRatio: false,
  scaleShowVerticalLines: false,
  responsiveAnimationDuration: 500,
  scales: {
    x: {
      stacked: true,
      grid: {
        color: gridLineColor,
        drawTicks: false
      },
      grace: 1,
      ticks: {
        color: labelColor
      }
    },
    y: {
      stacked: true,
      grid: {
        color: gridLineColor
      },
      ticks: {
        autoSkip: false,
        color: labelColor
      }
    }
  },
  plugins: {
    title: {
      display: true,
      text: titleText,
      font: {
        size: 25
      },
      color: titleColor
    },
    subtitle: {
      display: true,
      align: 'start',
      font: {
        size: 15,
        weight: 'bold italic'
      },
      color: titleColor
    },
    datalabels: {
      color: labelColor
    },
    legend: {
      labels: {
        color: labelColor
      }
    }
  }
}

$.ajax({
  type: 'GET',
  url: url,
  data: data,
  async: false,
  success: function (response) {
    if (response.error) {
      // Show no coefficients set for year
      showNoData(statisticsDiv, 'hệ số cho năm học này <i class="feather feather-help-circle"></i>')
    } else if (!response.length) {
      // Show no data message
      showNoData(statisticsDiv, '<i class="feather feather-help-circle"></i>')
    } else {
      // Get chart labels and data
      var labels = response.map(function (e) {
        return e.FullName
      })

      var chartData
      var originalHours = response.map(function (e) {
        return e.OriginalHours
      })
      var remunerationHours = response.map(function (e) {
        return e.RemunerationHours
      })

      var chartHeight
      if (isLesson == 'False') {
        // Set chart height to be larger
        chartHeight = 1200

        // Fetch chart data
        chartData = {
          labels: labels,
          datasets: [
            {
              label: 'Số giờ giảng',
              data: originalHours,
              backgroundColor: 'rgba(115, 103, 240, 0.8)',
              borderColor: 'transparent',
              borderWidth: 1,
              borderRadius: 3,
              datalabels: {
                anchor: 'end',
                align: 'start',
                offset: -30
              },
              stack: 'Stack 0'
            },
            {
              label: 'Số giờ quy đổi',
              data: remunerationHours,
              backgroundColor: 'rgba(255,159,67, 0.8)',
              borderColor: 'transparent',
              borderWidth: 1,
              borderRadius: 3,
              datalabels: {
                anchor: 'end',
                align: 'start',
                offset: -30
              },
              stack: 'Stack 1'
            }
          ]
        }
        chartOptions.plugins.subtitle.text =
          'Số giảng viên: ' +
          response.length +
          ' / Tổng số giờ giảng: ' +
          hoursSum(response, 'OriginalHours') +
          ' / Tổng số giờ quy đổi: ' +
          hoursSum(response, 'RemunerationHours')
      } else {
        // Set chart height as in data-height
        chartHeight = chartWrapper.data('height')

        // Get lesson data mapping
        var sumLessons1 = response.map(function (e) {
          return e.SumLesson1
        })
        var sumLessons4 = response.map(function (e) {
          return e.SumLesson4
        })
        var sumLessons7 = response.map(function (e) {
          return e.SumLesson7
        })
        var sumLessons10 = response.map(function (e) {
          return e.SumLesson10
        })
        var sumLessons13 = response.map(function (e) {
          return e.SumLesson13
        })

        // Fetch lessons chart data
        chartData = {
          labels: labels,
          datasets: [
            {
              label: 'Ca 1',
              data: sumLessons1,
              backgroundColor: 'rgba(54, 162, 235, 0.5)',
              borderColor: 'rgba(54, 162, 235, 1)',
              borderWidth: 1,
              borderRadius: 3
            },
            {
              label: 'Ca 2',
              data: sumLessons4,
              backgroundColor: 'rgba(255, 99, 132, 0.5)',
              borderColor: 'rgba(255, 99, 132, 1)',
              borderWidth: 1,
              borderRadius: 3
            },
            {
              label: 'Ca 3',
              data: sumLessons7,
              backgroundColor: 'rgba(255, 205, 86, 0.5)',
              borderColor: 'rgba(255, 205, 86, 1)',
              borderWidth: 1,
              borderRadius: 3
            },
            {
              label: 'Ca 4',
              data: sumLessons10,
              backgroundColor: 'rgba(75, 192, 192, 0.5)',
              borderColor: 'rgba(75, 192, 192, 1)',
              borderWidth: 1,
              borderRadius: 3
            },
            {
              label: 'Ca 5',
              data: sumLessons13,
              backgroundColor: 'rgba(153, 102, 255, 0.5)',
              borderColor: 'rgba(153, 102, 255, 1)',
              borderWidth: 1,
              borderRadius: 3
            }
          ]
        }
        chartOptions.plugins.subtitle.text =
          'Số giảng viên: ' + response.length + ' / Tổng số giờ quy đổi: ' + hoursSum(response, 'RemunerationHours')
        chartOptions.plugins.datalabels = {
          color: labelColor,
          font: {
            size: '9'
          },
          display: function (context) {
            // Only return positive values
            return context.dataset.data[context.dataIndex] !== 0
          }
        }

        // Get on legend click event
        chartOptions.plugins.legend.onClick = function (event, legendItem, legend) {
          const index = legendItem.datasetIndex
          const ci = legend.chart
          if (ci.isDatasetVisible(index)) {
            ci.hide(index)
            legendItem.hidden = true
          } else {
            ci.show(index)
            legendItem.hidden = false
          }

          var sum = 0
          var array = []
          // Loop through each legend item to sum total hours
          for (var i = 0; i < 5; i++) {
            if (ci.isDatasetVisible(i)) {
              const dataSet = chart.data.datasets[i].data
              sum += dataSet.reduce((a, b) => a + b, 0)
              array.push(dataSet)
            }
          }
          // Update lecturer count
          var lecturerCount = 0
          if (array.length) {
            lecturerCount = array.reduce((r, a) => r.map((b, i) => a[i] + b)).filter(x => x > 0).length
          }

          // Update chart title
          chartOptions.plugins.subtitle.text = 'Số giảng viên: ' + lecturerCount + ' / Tổng số giờ quy đổi: ' + sum
          chart.update()
        }
      }

      // Wrap charts with div of height according to their data-height
      if (chartWrapper.length) {
        chartWrapper.each(function () {
          $(this).wrap($('<div style="height:' + chartHeight + 'px"></div>'))
        })
      }

      // Create the chart
      var chart = new Chart(ctx, {
        type: 'bar',
        data: chartData,
        options: chartOptions,
        plugins: [ChartDataLabels]
      })

      // Detect Dark Layout and change color
      $('.nav-link-style').on('click', function () {
        var titleLight = '#d0d2d6',
          titleDark = '#666666',
          textLight = '#b4b7bd',
          TextDark = '#6e6b7b',
          titleColor,
          textColor
        if ($('html').hasClass('dark-layout')) {
          titleColor = titleLight
          textColor = textLight
        } else {
          titleColor = titleDark
          textColor = TextDark
        }
        chart.options.plugins.subtitle.color = titleColor
        chart.options.plugins.title.color = titleColor
        chart.options.plugins.datalabels.color = titleColor
        chart.options.plugins.legend.labels.color = textColor
        chart.options.scales.x.ticks.color = textColor
        chart.options.scales.y.ticks.color = textColor
        chart.update()
      })

      // populate table for viewing statistics
      populateStatisticsDatatable(response)
      populateStatisticsDetailsDatatable(response)
    }
  }
})

function hoursSum(items, prop) {
  return items.reduce(function (a, b) {
    return a + b[prop]
  }, 0)
}

function populateStatisticsDatatable(data) {
  var dataTable
  var exportColumns = isLessonCheck.is(':checked')
    ? [0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17]
    : [0, 2, 3, 4, 5, 6, 7]

  // Populate statistics table
  dataTable = $('#tblStatistics').DataTable({
    columns: [
      { data: '', defaultContent: '' },
      {
        data: 'LecturerId',
        render: function (data) {
          return (
            "<button type='button' class='btn btn-icon btn-icon rounded-circle btn-success waves-effect waves-float waves-light p-25 viewInfo' title='Xem môn học' data-id='" +
            data +
            "'><i class='feather feather-plus'></i></button>"
          )
        }
      },
      { data: 'StaffId' },
      { data: 'FullName' },
      { data: 'SubjectCount' },
      { data: 'ClassCount' },
      { data: 'OriginalHours' },
      { data: 'RemunerationHours' },
      { data: 'OriginalSumLesson1', defaultContent: '' },
      { data: 'SumLesson1', defaultContent: '' },
      { data: 'OriginalSumLesson4', defaultContent: '' },
      { data: 'SumLesson4', defaultContent: '' },
      { data: 'OriginalSumLesson7', defaultContent: '' },
      { data: 'SumLesson7', defaultContent: '' },
      { data: 'OriginalSumLesson10', defaultContent: '' },
      { data: 'SumLesson10', defaultContent: '' },
      { data: 'OriginalSumLesson13', defaultContent: '' },
      { data: 'SumLesson13', defaultContent: '' }
    ],
    data: data,
    columnDefs: [
      {
        searchable: false,
        orderable: false,
        width: '1%',
        targets: [0, 1]
      },
      { className: 'text-center', target: [0, 4, 5, 6, 7, 8, 9, 10, 11, 12] }
    ],
    order: [[7, 'desc']],
    dom: '<"d-flex justify-content-between align-items-center header-actions mx-2 row"<"col-sm-12 col-lg-4 d-flex justify-content-center justify-content-lg-start" l><"col-sm-12 col-lg-8 ps-xl-75 px-0"<"dt-action-buttons d-flex align-items-center justify-content-center justify-content-lg-end flex-lg-nowrap flex-wrap"<"me-1"f>B>>>t<"d-flex justify-content-between mx-2 row mb-1"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>',
    displayLength: 10,
    lengthMenu: [
      [10, 25, 50, -1],
      [10, 25, 50, 'tất cả']
    ],
    buttons: [
      {
        extend: 'collection',
        className: 'btn btn-outline-secondary dropdown-toggle me-2',
        text: feather.icons['share'].toSvg({ class: 'font-small-4 me-50' }) + 'Export',
        buttons: [
          {
            extend: 'print',
            className: 'dropdown-item',
            title: fileName,
            exportOptions: {
              columns: exportColumns
            }
          },
          {
            extend: 'excel',
            className: 'dropdown-item',
            title: fileName,
            exportOptions: {
              columns: exportColumns
            }
          },
          {
            extend: 'pdf',
            className: 'dropdown-item',
            title: fileName,
            exportOptions: {
              columns: exportColumns
            }
          },
          {
            extend: 'copy',
            className: 'dropdown-item',
            title: fileName,
            exportOptions: {
              columns: exportColumns
            }
          }
        ],
        init: function (api, node, config) {
          $(node).removeClass('btn-secondary')
          $(node).parent().removeClass('btn-group')
          setTimeout(function () {
            $(node).closest('.dt-buttons').removeClass('btn-group').addClass('d-inline-flex mt-50')
          }, 50)
        }
      }
    ],
    initComplete: function () {
      isLessonCheck.is(':checked') ? setVisibleColumn(true) : setVisibleColumn(false)
    },
    language: {
      url: rootUrl + 'app-assets/language/datatables/vi.json'
    }
  })

  // Create Index column datatable
  dataTable.on('draw.dt', function () {
    dataTable
      .column(0, { search: 'applied', order: 'applied' })
      .nodes()
      .each(function (cell, i) {
        cell.innerHTML = i + 1
        dataTable.cell(cell).invalidate('dom')
      })
  })

  // Add event listener for opening and closing details
  $('#tblStatistics tbody').on('click', 'td .viewInfo ', function () {
    var $this = $(this),
      tr = $this.closest('tr'),
      row = dataTable.row(tr),
      lecturerId = $this.data('id'),
      subjectUrl = rootUrl + 'Statistics/',
      subjectData

    if (row.child.isShown()) {
      // Update icon on click
      $this.removeClass('btn-danger').addClass('btn-success')
      $this.find('i').removeClass('feather-minus').addClass('feather-plus')
      // This row is already open - close it
      row.child.hide()
      tr.removeClass('shown')
    } else {
      // Update icon on click
      $this.removeClass('btn-success').addClass('btn-danger')
      $this.find('i').removeClass('feather-plus').addClass('feather-minus')

      // Get data for ajax request
      if (type == yearSelect.attr('id')) {
        var yearSplit = value.split(' - '),
          startYear = yearSplit[0],
          endYear = yearSplit[1]
        subjectUrl += 'GetYearRemunerationSubjects'
        subjectData = { startYear, endYear, majorId, lecturerId }
      } else {
        subjectUrl += 'GetTermRemunerationSubjects'
        subjectData = { termId: value, majorId, lecturerId }
      }

      // Send request to fetch subjects
      $.ajax({
        type: 'GET',
        url: subjectUrl,
        data: subjectData,
        success: function (data) {
          // Open this row
          row.child(format(data)).show()
          tr.addClass('shown')
        }
      })
    }
  })
}

function populateStatisticsDetailsDatatable(data) {
  var dataTable

  // Populate statistics table
  dataTable = $('#tblStatisticsDetails').DataTable({
    columns: [
      { data: '', defaultContent: '' },
      { data: 'StaffId' },
      { data: 'FullName' },
      { data: 'ClassCount' },
      { data: 'ClassesTaught' },
      { data: 'RemunerationHours' }
    ],
    data: data,
    columnDefs: [
      {
        searchable: false,
        orderable: false,
        width: '1%',
        targets: 0
      },
      { width: '15%', targets: 2 },
      { className: 'text-center', targets: 3 },
      {
        // Join classes taught string
        targets: 4,
        render: function (data) {
          return data.join('; ')
        }
      },
      {
        visible: false,
        searchable: false,
        targets: 5
      }
    ],
    order: [[5, 'desc']],
    dom: '<"d-flex justify-content-between align-items-center header-actions mx-2 row"<"col-sm-12 col-lg-4 d-flex justify-content-center justify-content-lg-start" l><"col-sm-12 col-lg-8 ps-xl-75 px-0"<"dt-action-buttons d-flex align-items-center justify-content-center justify-content-lg-end flex-lg-nowrap flex-wrap"<"me-1"f>B>>>t<"d-flex justify-content-between mx-2 row mb-1"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>',
    displayLength: 10,
    lengthMenu: [
      [10, 25, 50, -1],
      [10, 25, 50, 'tất cả']
    ],
    buttons: [
      {
        extend: 'collection',
        className: 'btn btn-outline-secondary dropdown-toggle me-2',
        text: feather.icons['share'].toSvg({ class: 'font-small-4 me-50' }) + 'Export',
        buttons: [
          {
            extend: 'print',
            className: 'dropdown-item',
            title: detailsFileName,
            exportOptions: {
              columns: ':visible'
            }
          },
          {
            extend: 'excel',
            className: 'dropdown-item',
            title: detailsFileName,
            exportOptions: {
              columns: ':visible'
            }
          },
          {
            extend: 'pdf',
            className: 'dropdown-item',
            title: detailsFileName,
            exportOptions: {
              columns: ':visible'
            }
          },
          {
            extend: 'copy',
            className: 'dropdown-item',
            title: detailsFileName,
            exportOptions: {
              columns: ':visible'
            }
          }
        ],
        init: function (api, node, config) {
          $(node).removeClass('btn-secondary')
          $(node).parent().removeClass('btn-group')
          setTimeout(function () {
            $(node).closest('.dt-buttons').removeClass('btn-group').addClass('d-inline-flex mt-50')
          }, 50)
        }
      }
    ],
    language: {
      url: rootUrl + 'app-assets/language/datatables/vi.json'
    }
  })

  // Create Index column datatable
  dataTable.on('draw.dt', function () {
    dataTable
      .column(0, { search: 'applied', order: 'applied' })
      .nodes()
      .each(function (cell, i) {
        cell.innerHTML = i + 1
        dataTable.cell(cell).invalidate('dom')
      })
  })
}

// Adjust column width on navigating Boostrap tab
$('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
  $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust()
})

function setVisibleColumn(state) {
  var table = $('#tblStatistics').DataTable()
  for (var i = 8; i <= 17; i++) {
    table.column(i).visible(state, state)
  }
  table.columns.adjust().draw(state) // adjust column sizing and redraw
}

// HTML format for child rows of viewing statistics
function format(d) {
  // `d` is the original data object for the row
  var arrayLength = d.length,
    tableRow = ''

  // Map data object
  var subjectId = d.map(function (e) {
    return e.Id
  })
  var subjectName = d.map(function (e) {
    return e.Name
  })
  var subjectCredits = d.map(function (e) {
    return e.Credits
  })
  var subjectMajor = d.map(function (e) {
    return e.Major
  })
  var subjectHours = d.map(function (e) {
    return e.Hours
  })
  var subjectRemuneratioHours = d.map(function (e) {
    return e.RemunerationHours
  })
  var totalClass = d.map(function (e) {
    var theoryClass = e.TheoryCount + 'LT',
      practiceClass = e.PracticeCount + 'TH',
      totalClass
    if (e.TheoryCount && e.PracticeCount) {
      totalClass = theoryClass + ' + ' + practiceClass
    } else if (e.TheoryCount) {
      totalClass = theoryClass
    } else if (e.PracticeCount) {
      totalClass = practiceClass
    }
    return totalClass
  })

  // Append HTML rows
  for (var i = 0; i < arrayLength; i++) {
    tableRow +=
      '<tr><td>' +
      subjectId[i] +
      '</td><td>' +
      subjectName[i] +
      '</td><td>' +
      subjectMajor[i] +
      '</td><td class="text-center">' +
      subjectCredits[i] +
      '</td><td class="text-center">' +
      totalClass[i] +
      '</td><td class="text-center">' +
      subjectHours[i] +
      '</td><td class="text-center">' +
      subjectRemuneratioHours[i] +
      '</td></tr>'
  }

  // Render rows
  return (
    '<div class="slider">' +
    '<table class="table table-sm">' +
    '<thead class="table-light">' +
    '<tr>' +
    '<th>Mã HP</th>' +
    '<th>Tên HP</th>' +
    '<th>Ngành</th>' +
    '<th class="text-center">Số TC</th>' +
    '<th class="text-center">Số lớp</th>' +
    '<th class="text-center">Số giờ giảng</th>' +
    '<th class="text-center">Số giờ quy đổi</th>' +
    '</tr>' +
    '</thead>' +
    '<tbody>' +
    tableRow +
    '</tbody>' +
    '</table>' +
    '</div>'
  )
}

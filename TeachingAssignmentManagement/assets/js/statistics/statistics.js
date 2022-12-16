//Get the context of the Chart canvas element we want to select
var ctx = $("#statistics-chart"),
    chartWrapper = $('.chartjs');

var labelColor = '#6e6b7b',
    titleColor = '#666666',
    gridLineColor = 'rgba(200, 200, 200, 0.2)'; // RGBA color helps in dark layout

// Setup data
var dataLoader = $('#data-loader'),
    type = dataLoader.data('type'),
    value = dataLoader.val(),
    url = rootUrl + 'Statistics/',
    titleText,
    fileName,
    data;

// Check if user is selecting term or year
if (type == yearSelect.attr('id')) {
    var yearSplit = value.split(" - "),
        startYear = yearSplit[0],
        endYear = yearSplit[1];
    data = { startYear, endYear };
    titleText = 'Thống kê số giờ năm học ' + value;
    fileName = 'ThongKeSoGio_NamHoc_' + startYear + '-' + endYear;
    url += 'GetYearData';
} else {
    data = { 'termId': value };
    titleText = 'Thống kê số giờ học kỳ ' + value;
    fileName = 'ThongKeSoGio_HK' + value;
    url += 'GetTermData';
}

// Detect Dark Layout
if ($('html').hasClass('dark-layout')) {
    titleColor = '#d0d2d6',
        labelColor = '#b4b7bd';
}

// Wrap charts with div of height according to their data-height
if (chartWrapper.length) {
    chartWrapper.each(function () {
        $(this).wrap($('<div style="height:' + this.getAttribute('data-height') + 'px"></div>'));
    });
}

Chart.defaults.font.family = 'Montserrat,Helvetica,Arial,serif';

// Chart Options
var chartOptions = {
    indexAxis: 'y',
    responsive: true,
    maintainAspectRatio: false,
    scaleShowVerticalLines: false,
    responsiveAnimationDuration: 500,
    scales: {
        xAxis: {
            stacked: true,
            grid: {
                color: gridLineColor,
                drawTicks: false,
            },
            ticks: {
                color: labelColor
            }
        },
        yAxis: {
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
        datalabels: {
            color: labelColor
        },
        legend: {
            labels: {
                color: labelColor
            }
        }
    }
};

$.ajax({
    type: 'GET',
    url: url,
    data: data,
    async: false,
    success: function (response) {
        if (!response.length) {
            // Show no data message
            statisticsDiv.empty().append('<h4 class="text-center mt-2">Học kỳ này chưa có dữ liệu <i class="feather feather-help-circle"></i></h4><div class="card-body"><img class="mx-auto p-3 d-block w-50" alt="Welcome" src="' + rootUrl + 'assets/images/img_no_data.svg"></div>');
        } else {
            // Get chart labels and data
            var labels = response.map(function (e) {
                return e.full_name;
            });
            var totalLessons = response.map(function (e) {
                return e.sum;
            });

            // Chart Data
            var chartData = {
                labels: labels,
                datasets: [
                    {
                        label: 'Số giờ giảng',
                        data: totalLessons,
                        backgroundColor: 'rgba(115, 103, 240, 0.8)',
                        borderColor: 'transparent',
                        borderWidth: 1,
                        datalabels: {
                            anchor: 'end',
                            align: 'start',
                            offset: -30
                        }
                    }
                ]
            };

            // Create the chart
            var chart = new Chart(ctx, {
                type: 'bar',
                data: chartData,
                options: chartOptions,
                plugins: [ChartDataLabels]
            });

            // Detect Dark Layout and change color
            $('.nav-link-style').on('click', function () {
                var titleLight = '#d0d2d6',
                    titleDark = '#666666',
                    textLight = '#b4b7bd',
                    TextDark = '#6e6b7b',
                    titleColor,
                    textColor;
                if ($('html').hasClass('dark-layout')) {
                    titleColor = titleLight;
                    textColor = textLight;
                } else {
                    titleColor = titleDark;
                    textColor = TextDark;
                }
                chart.options.plugins.title.color = titleColor;
                chart.options.plugins.datalabels.color = textColor;
                chart.options.plugins.legend.labels.color = textColor;
                chart.options.scales.xAxis.ticks.color = textColor;
                chart.options.scales.yAxis.ticks.color = textColor;
                chart.update();
            });

            // populate table for viewing statistics
            populateDatatable(response);
        }
    }
});

function populateDatatable(data) {
    var dataTable;

    // Populate statistics table
    dataTable = $('#tblStatistics').DataTable(
        {
            columns: [
                { 'data': '', defaultContent: '' },
                {
                    'data': null, 'render': function () {
                        return "<button type='button' class='btn btn-icon btn-icon rounded-circle btn-success waves-effect waves-float waves-light p-25 viewInfo' title='Xem môn học'><i class='feather feather-plus'></i></button>";
                    }
                },
                { 'data': 'staff_id' },
                { 'data': 'full_name' },
                { 'data': 'curriculum_count' },
                { 'data': 'sum' }
            ],
            data: data,
            columnDefs: [
                {
                    searchable: false,
                    orderable: false,
                    width: '5%',
                    targets: [0, 1]
                },
                { className: 'text-center', target: [0, 4, 5] }
            ],
            order: [[5, 'desc']],
            dom: '<"d-flex justify-content-between align-items-center header-actions mx-2 row mt-75"<"col-sm-12 col-lg-4 d-flex justify-content-center justify-content-lg-start" l><"col-sm-12 col-lg-8 ps-xl-75 px-0"<"dt-action-buttons d-flex align-items-center justify-content-center justify-content-lg-end flex-lg-nowrap flex-wrap"<"me-1"f>B>>>t<"d-flex justify-content-between mx-2 row mb-1"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>',
            displayLength: 10,
            lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "tất cả"]],
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
                                columns: [0, 2, 3, 4]
                            }
                        },
                        {
                            extend: 'excel',
                            className: 'dropdown-item',
                            title: fileName,
                            exportOptions: {
                                columns: [0, 2, 3, 4]
                            }
                        },
                        {
                            extend: 'pdf',
                            className: 'dropdown-item',
                            title: fileName,
                            exportOptions: {
                                columns: [0, 2, 3, 4]
                            }
                        },
                        {
                            extend: 'copy',
                            className: 'dropdown-item',
                            title: fileName,
                            exportOptions: {
                                columns: [0, 2, 3, 4]
                            }
                        }
                    ],
                    init: function (api, node, config) {
                        $(node).removeClass('btn-secondary');
                        $(node).parent().removeClass('btn-group');
                        setTimeout(function () {
                            $(node).closest('.dt-buttons').removeClass('btn-group').addClass('d-inline-flex mt-50');
                        }, 50);
                    }
                }
            ],

            language: {
                'url': rootUrl + 'app-assets/language/datatables/vi.json'
            }
        });

    // Create Index column datatable
    dataTable.on('draw.dt', function () {
        dataTable.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
            dataTable.cell(cell).invalidate('dom');
        });
    });

    // Add event listener for opening and closing details
    $('#tblStatistics tbody').on('click', 'td .viewInfo ', function () {
        var $this = $(this);
        var tr = $this.closest('tr');
        var row = dataTable.row(tr);

        if (row.child.isShown()) {
            // Update icon on click
            $this.removeClass('btn-danger').addClass('btn-success');
            $this.find('i').removeClass('feather-minus').addClass('feather-plus');
            // This row is already open - close it
            row.child.hide();
            tr.removeClass('shown');
        } else {
            // Update icon on click
            $this.removeClass('btn-success').addClass('btn-danger');
            $this.find('i').removeClass('feather-plus').addClass('feather-minus');
            // Open this row
            row.child(format(row.data())).show();
            tr.addClass('shown');
        }
    });
}

// Adjust column width on navigating Boostrap tab
$('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
    $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust();
});

// HTML format for child rows of viewing statistics
function format(d) {
    // `d` is the original data object for the row
    var curriculumId = d.curriculum_id,
        curriculumName = d.curriculum_name,
        curriculumCredits = d.curriculum_credits,
        curriculumMajor = d.curriculum_major,
        arrayLength = curriculumId.length,
        tableRow = '';
    for (var i = 0; i < arrayLength; i++) {
        tableRow += '<tr><td>' + curriculumId[i] + '</td><td>' + curriculumName[i] + '</td><td>' + curriculumMajor[i] + '</td><td class="text-center">' + curriculumCredits[i] + '</td></tr>';
    }
    return (
        '<table class="table table-sm">' +
        '<thead class="table-light">' +
        '<tr>' +
        '<th>Mã HP</th>' +
        '<th>Tên HP</th>' +
        '<th>Ngành</th>' +
        '<th class="text-center">Số TC</th>' +
        '</tr>' +
        '</thead>' +
        '<tbody>' +
        tableRow +
        '</tbody>' +
        '</table>'
    );
}
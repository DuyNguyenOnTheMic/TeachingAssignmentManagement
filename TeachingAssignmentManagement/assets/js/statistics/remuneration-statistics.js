﻿//Get the context of the Chart canvas element we want to select
var ctx = $("#statistics-chart"),
    chartWrapper = $('.chartjs');

var labelColor = '#6e6b7b',
    titleColor = '#666666',
    gridLineColor = 'rgba(200, 200, 200, 0.2)'; // RGBA color helps in dark layout

// Setup data
var dataLoader = $('#data-loader'),
    isLesson = dataLoader.data('islesson'),
    value = dataLoader.val(),
    url = rootUrl + 'Statistics/GetRemunerationData',
    titleText = 'Thống kê số giờ thù lao học kỳ ' + value,
    fileName = 'ThongKeSoGio_HK' + value,
    data = { isLesson, 'termId': value };

// Detect Dark Layout
if ($('html').hasClass('dark-layout')) {
    titleColor = '#d0d2d6';
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
        x: {
            stacked: true,
            grid: {
                color: gridLineColor,
                drawTicks: false,
            },
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
};

$.ajax({
    type: 'GET',
    url: url,
    data: data,
    async: false,
    success: function (response) {
        if (!response.length) {
            // Show no data message
            showNoData(statisticsDiv, '<i class="feather feather-help-circle"></i>');
        } else {
            // Get chart labels and data
            var labels = response.map(function (e) {
                return e.FullName;
            });

            var chartData;
            var totalLessons = response.map(function (e) {
                return e.Remuneration;
            });

            if (isLesson == 'False') {
                // Fetch chart data
                chartData = {
                    labels: labels,
                    datasets: [
                        {
                            label: 'Số giờ giảng',
                            data: totalLessons,
                            backgroundColor: 'rgba(115, 103, 240, 0.8)',
                            borderColor: 'transparent',
                            borderWidth: 1,
                            borderRadius: 3,
                            datalabels: {
                                anchor: 'end',
                                align: 'start',
                                offset: -30
                            }
                        }
                    ]
                };
            } else {
                // Get lesson data mapping
                var sumLessons1 = response.map(function (e) {
                    return e.SumLesson1;
                });
                var sumLessons4 = response.map(function (e) {
                    return e.SumLesson4;
                });
                var sumLessons7 = response.map(function (e) {
                    return e.SumLesson7;
                });
                var sumLessons10 = response.map(function (e) {
                    return e.SumLesson10;
                });
                var sumLessons13 = response.map(function (e) {
                    return e.SumLesson13;
                });

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
                };

                chartOptions.plugins.datalabels = {
                    color: labelColor,
                    font: {
                        size: '9'
                    },
                    display: function (context) {
                        // Only return positive values
                        return context.dataset.data[context.dataIndex] !== 0;
                    }
                };
            }

            chartOptions.plugins.subtitle.text = 'Số giảng viên: ' + response.length + ' / Tổng số giờ: ' + hoursSum(response, 'Remuneration');

            // Create the chart
            var chart = new Chart(ctx, {
                type: 'bar',
                data: chartData,
                options: chartOptions,
                plugins: [ChartDataLabels]
            });

            // Get on legend click event
            chart.options.plugins.legend.onClick = function (event, legendItem, legend) {
                const index = legendItem.datasetIndex;
                const ci = legend.chart;
                if (ci.isDatasetVisible(index)) {
                    ci.hide(index);
                    legendItem.hidden = true;
                } else {
                    ci.show(index);
                    legendItem.hidden = false;
                }

                var sum = 0;
                var array = [];
                // Loop through each legend item to sum total hours
                for (var i = 0; i < 5; i++) {
                    if (ci.isDatasetVisible(i)) {
                        const dataSet = chart.data.datasets[i].data;
                        sum += dataSet.reduce((a, b) => a + b, 0);
                        array.push(dataSet);
                    }
                }
                // Update lecturer count
                var lecturerCount = 0;
                if (array.length) {
                    lecturerCount = array.reduce((r, a) => r.map((b, i) => a[i] + b)).filter(x => x > 0).length;
                }

                // Update chart title
                chartOptions.plugins.subtitle.text = 'Số giảng viên: ' + lecturerCount + ' / Tổng số giờ: ' + sum;
                chart.update();
            }

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
                chart.options.plugins.subtitle.color = titleColor;
                chart.options.plugins.title.color = titleColor;
                chart.options.plugins.datalabels.color = titleColor;
                chart.options.plugins.legend.labels.color = textColor;
                chart.options.scales.x.ticks.color = textColor;
                chart.options.scales.y.ticks.color = textColor;
                chart.update();
            });

            // populate table for viewing statistics
            // populateDatatable(response);
        }
    }
});

function hoursSum(items, prop) {
    return items.reduce(function (a, b) {
        return a + b[prop];
    }, 0);
};

function populateDatatable(data) {
    var dataTable;
    var exportColumns = isLessonCheck.is(":checked") ? [0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12] : [0, 2, 3, 4, 5, 6, 7];

    // Populate statistics table
    dataTable = $('#tblStatistics').DataTable(
        {
            columns: [
                { 'data': '', defaultContent: '' },
                {
                    'data': 'Key',
                    'render': function (data) {
                        return "<button type='button' class='btn btn-icon btn-icon rounded-circle btn-success waves-effect waves-float waves-light p-25 viewInfo' title='Xem môn học' data-id='" + data + "'><i class='feather feather-plus'></i></button>";
                    }
                },
                { 'data': 'staff_id' },
                { 'data': 'full_name' },
                { 'data': 'lecturer_type' },
                { 'data': 'subject_count' },
                { 'data': 'class_count' },
                { 'data': 'sum' },
                { 'data': 'sumLesson1', defaultContent: '' },
                { 'data': 'sumLesson4', defaultContent: '' },
                { 'data': 'sumLesson7', defaultContent: '' },
                { 'data': 'sumLesson10', defaultContent: '' },
                { 'data': 'sumLesson13', defaultContent: '' }
            ],
            data: data,
            columnDefs: [
                {
                    searchable: false,
                    orderable: false,
                    width: '1%',
                    targets: [0, 1]
                },
                {
                    // User type
                    targets: 4,
                    render: function (data) {
                        var $status = data;
                        if ($status) {
                            var typeBadgeObj = {
                                'CH': { title: 'Cơ hữu', class: 'badge-light-success' },
                                'TG': { title: 'Thỉnh giảng', class: 'badge-light-warning' }
                            };
                            return '<span class="badge rounded-pill ' + typeBadgeObj[$status].class + ' text-capitalized">' + typeBadgeObj[$status].title + '</span>';
                        } else {
                            return null;
                        }
                    }
                },
                { className: 'text-center', target: [0, 1, 4, 5, 6, 7, 8, 9, 10, 11, 12] }
            ],
            order: [[7, 'desc']],
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
}

// Adjust column width on navigating Boostrap tab
$('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
    $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust();
});

function setVisibleColumn(state) {
    var table = $('#tblStatistics').DataTable();
    for (var i = 8; i <= 12; i++) {
        table.column(i).visible(state, state);
    }
    table.columns.adjust().draw(state); // adjust column sizing and redraw
}
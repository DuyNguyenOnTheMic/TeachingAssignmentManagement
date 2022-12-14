//Get the context of the Chart canvas element we want to select
var ctx = $("#statistics-chart"),
    chartWrapper = $('.chartjs');

var labelColor = '#6e6b7b',
    titleColor = '#666666',
    gridLineColor = 'rgba(200, 200, 200, 0.2)'; // RGBA color helps in dark layout

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
    legend: {
        position: 'top'
    },
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

// Create the chart
var chart = new Chart(ctx, {
    type: 'bar',
    data: {},
    options: chartOptions,
    plugins: [ChartDataLabels]
});

var termId = termSelect.val();
$.ajax({
    type: 'GET',
    url: rootUrl + 'Statistics/GetTermData',
    data: { termId },
    success: function (response) {

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

        // Update chart data
        chart.options.plugins.title.text = 'Thống kê số giờ học kỳ 223';
        chart.data = chartData;
        chart.update();

        // populate table for viewing statistics
        populateDatatable(response);
    }
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

function populateDatatable(data) {
    var dataTable;
    var fileName = 'ThongKe_SoGio_HK' + termId;

    // Populate Error lecturers datatable
    dataTable = $('#tblStatistics').DataTable(
        {
            columns: [
                { 'data': '', defaultContent: '' },
                { 'data': 'staff_id' },
                { 'data': 'full_name' },
                { 'data': 'sum' },
                {
                    'data': 'Key', 'render': function (data) {
                        return "<a class='viewInfo text-success p-0' data-original-title='Xem môn học' title='Xem môn học' onclick=popupForm('" + rootUrl + "Major/Edit/" + data + "')><i class='feather feather-info font-medium-3 me-1'></i></a>";
                    }
                }
            ],
            data: data,
            columnDefs: [
                {
                    searchable: false,
                    orderable: false,
                    width: '5%',
                    targets: [0, 4]
                },
                { className: 'text-center', target: [0, 3, 4]}
            ],
            order: [[3, 'desc']],
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
                            title: fileName
                        },
                        {
                            extend: 'excel',
                            className: 'dropdown-item',
                            title: fileName
                        },
                        {
                            extend: 'pdf',
                            className: 'dropdown-item',
                            title: fileName
                        },
                        {
                            extend: 'copy',
                            className: 'dropdown-item',
                            title: fileName
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

$(function () {
    // Adjust column width on navigating Boostrap tab
    $('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
        $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust();
    });
})
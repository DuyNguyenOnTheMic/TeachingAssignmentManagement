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

    // Populate statistics table
    dataTable = $('#tblStatistics').DataTable(
        {
            columns: [
                { 'data': '', defaultContent: '' },
                {
                    'data': null, 'render': function () {
                        return "<button type='button' class='btn btn-icon btn-icon rounded-circle btn-success p-25 viewInfo' title='Xem môn học'><i class='feather feather-plus'></i></button>";
                    }
                },
                { 'data': 'staff_id' },
                { 'data': 'full_name' },
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
                { className: 'text-center', target: [0, 3, 4] }
            ],
            order: [[4, 'desc']],
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

    // Add event listener for opening and closing details
    $(document).on('click', '.viewInfo ', function () {
        var tr = $(this).closest('tr');
        var row = dataTable.row(tr);

        if (row.child.isShown()) {
            // This row is already open - close it
            row.child.hide();
            tr.removeClass('shown');
        } else {
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

function getCurriculums(lecturerId) {
    var url = rootUrl + 'Statistics/GetCurriculums';
    $.get(url, { lecturerId }, function (data) {
        // Populate curriculums data
        curriculumsDiv.html(data);
    });
}

/* Formatting function for row details - modify as you need */
function format(d) {
    // `d` is the original data object for the row
    return (
        '<table cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;">' +
        '<tr>' +
        '<td>Full name:</td>' +
        '<td>' +
        d.curriculum_id +
        '</td>' +
        '</tr>' +
        '<tr>' +
        '<td>Extension number:</td>' +
        '<td>' +
        d.curriculum_name +
        '</td>' +
        '</tr>' +
        '<tr>' +
        '<td>Extra info:</td>' +
        '<td>And any further details here (images etc)...</td>' +
        '</tr>' +
        '</table>'
    );
}
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

var termId = 223;
$.ajax({
    type: 'GET',
    url: '/Statistics/GetTermData',
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
$(window).on('load', function () {
    //Get the context of the Chart canvas element we want to select
    var ctx = $("#statistics-chart"),
        chartWrapper = $('.chartjs');

    var labelColor = '#6e6b7b',
        gridLineColor = 'rgba(200, 200, 200, 0.2)'; // RGBA color helps in dark layout

    // Detect Dark Layout
    if ($('html').hasClass('dark-layout')) {
        labelColor = '#b4b7bd';
    }

    // Wrap charts with div of height according to their data-height
    if (chartWrapper.length) {
        chartWrapper.each(function () {
            $(this).wrap($('<div style="height:' + this.getAttribute('data-height') + 'px"></div>'));
        });
    }

    // Chart Options
    var chartOptions = {
        indexAxis: 'y',
        responsive: true,
        maintainAspectRatio: false,
        scaleShowVerticalLines: false,
        responsiveAnimationDuration: 500,
        borderRadius: 4,
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
                    color: labelColor
                }
            }
        },
        plugins: {
            title: {
                display: true,
                font: {
                    size: 25
                }
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

    // Plugin block
    const legendMargin = {
        id: 'legendMargin',
        beforeInit(chart, legend, options) {
            const fitValue = chart.legend.fit;

            chart.legend.fit = function fit() {
                fitValue.bind(chart.legend)();
                return this.height += 30;
            }
        }
    };

    // Create the chart
    var chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['Red', 'Blue', 'Yellow', 'Green', 'Purple', 'Orange'],
            datasets: [{
                label: '# of Votes',
                data: [12, 19, 3, 5, 2, 3],
                borderWidth: 1,
                backgroundColor: 'rgba(40, 208, 148, 0.2)',
                borderColor: 'rgba(40, 208, 148)',
                datalabels: {
                    anchor: 'end',
                    align: 'start'
                }
            }]
        },
        options: chartOptions,
        plugins: [ChartDataLabels, legendMargin]
    });

    // Detect Dark Layout and change color
    $('.nav-link-style').on('click', function () {
        var lightColor = '#b4b7bd',
            darkColor = '#6e6b7b',
            color;
        if ($('html').hasClass('dark-layout')) {
            color = lightColor;
        } else {
            color = darkColor;
        }
        chart.options.scales.xAxis.ticks.color = color;
        chart.options.scales.yAxis.ticks.color = color;
        chart.options.plugins.datalabels.color = color;
        chart.options.plugins.legend.labels.color = color;
        chart.update();
    });
});
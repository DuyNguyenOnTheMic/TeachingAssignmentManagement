$(function () {
    //Get the context of the Chart canvas element we want to select
    var ctx = $("#statistics-chart");

    // Chart Options
    var chartOptions = {
        responsive: true,
        maintainAspectRatio: false,
        scaleShowVerticalLines: false,
        responsiveAnimationDuration: 500,
        borderRadius: 4,
        legend: {
            position: 'top',
        },
        scales: {
            xAxis: {
                stacked: true,
                grid: {
                    color: "#f3f3f3",
                    drawTicks: false,
                },
                ticks: {
                    padding: 20
                }
            },
            yAxis: {
                stacked: true,
                grid: {
                    color: "#f3f3f3",
                    drawTicks: false,
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
                borderWidth: 1
            }]
        },
        options: chartOptions,
        plugins: [ChartDataLabels, legendMargin]
    });
});
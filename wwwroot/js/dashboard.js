
// JavaScript functions for the dashboard chart
window.initializeExpensesChart = (labels, data, colors) => {
    const ctx = document.getElementById('expensesChart').getContext('2d');
    window.expensesChart = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: colors,
                borderWidth: 0,
                cutout: '60%'
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            return `${context.label}: ${context.parsed}%`;
                        }
                    }
                }
            }
        }
    });
};

window.updateExpensesChart = (labels, data, colors) => {
    if (window.expensesChart) {
        window.expensesChart.data.labels = labels;
        window.expensesChart.data.datasets[0].data = data;
        window.expensesChart.data.datasets[0].backgroundColor = colors;
        window.expensesChart.update();
    }
};
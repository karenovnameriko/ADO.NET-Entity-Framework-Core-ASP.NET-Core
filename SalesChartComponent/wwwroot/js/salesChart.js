
export function drawChart(canvas, months, sales) {
    const ctx = canvas.getContext("2d");

    const width = canvas.clientWidth;
    const height = 320;

    canvas.width = width;
    canvas.height = height;

    const padding = 45;
    const chartWidth = width - padding * 2;
    const chartHeight = height - padding * 2;

    const maxSales = Math.max(...sales) * 1.2;
    const stepX = chartWidth / Math.max(sales.length - 1, 1);

    ctx.clearRect(0, 0, width, height);

    // Оси графика
    ctx.beginPath();
    ctx.moveTo(padding, padding);
    ctx.lineTo(padding, height - padding);
    ctx.lineTo(width - padding, height - padding);
    ctx.strokeStyle = "#777";
    ctx.stroke();

    // Линия продаж
    ctx.beginPath();
    ctx.strokeStyle = "#2563eb";
    ctx.lineWidth = 3;

    sales.forEach((value, index) => {
        const x = padding + index * stepX;
        const y = height - padding -
            (value / maxSales) * chartHeight;

        if (index === 0) {
            ctx.moveTo(x, y);
        } else {
            ctx.lineTo(x, y);
        }
    });

    ctx.stroke();
    
    sales.forEach((value, index) => {
        const x = padding + index * stepX;
        const y = height - padding -
            (value / maxSales) * chartHeight;

        ctx.beginPath();
        ctx.arc(x, y, 5, 0, Math.PI * 2);
        ctx.fillStyle = "#2563eb";
        ctx.fill();

        ctx.fillStyle = "#333";
        ctx.font = "12px Arial";
        ctx.textAlign = "center";

        ctx.fillText(months[index], x, height - padding + 20);

        ctx.fillText(
            value.toLocaleString("ru-RU"),
            x,
            y - 12
        );
    });
}

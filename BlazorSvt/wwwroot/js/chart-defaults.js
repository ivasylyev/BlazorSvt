// Шрифт и цвет текста графиков по умолчанию (подписи делений, названия осей, подсказки) — Roboto и тёмно-зелёный СИБУР.
// Подключается после chart.umd.js (Host/Components/App.razor).
if (window.Chart) {
    Chart.defaults.font.family = '"Roboto", Arial, sans-serif';
    Chart.defaults.color = '#00313C';
}

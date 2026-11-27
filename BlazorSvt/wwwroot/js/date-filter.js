// Вставка даты из ячейки грида в фильтр: нативный paste в date/datetime-local
// не принимает локализованный текст, поэтому парсим сами и кладём нормализованное значение.
window.svtDateFilterPaste = function (event) {
    const el = event.target;
    const raw = (event.clipboardData || window.clipboardData)?.getData("text");
    const parsed = window.svtParseDateTimeText(raw);
    if (!parsed) return; // не дата — нативное поведение

    const pad = (n) => String(n).padStart(2, "0");
    const date = `${parsed.y}-${pad(parsed.mo)}-${pad(parsed.d)}`;
    // type=date принимает только дату; datetime-local — до минут (секунды в фильтре не нужны).
    el.value = el.type === "date" ? date : `${date}T${pad(parsed.h)}:${pad(parsed.mi)}`;
    event.preventDefault();
    el.dispatchEvent(new Event("input", { bubbles: true }));
};

// Распознаваемые форматы: 2020-07-13[T ]15:49[:45] (ISO), 13.07.2020 15:49[:45] (ru),
// 7/13/2020 3:49[:45] PM (en-US), и те же без времени.
window.svtParseDateTimeText = function (text) {
    if (!text) return null;
    text = text.trim();

    let m = text.match(/^(\d{4})-(\d{1,2})-(\d{1,2})(?:[T ](\d{1,2}):(\d{1,2})(?::(\d{1,2}))?)?/);
    if (m) return window.svtDateParts(+m[1], +m[2], +m[3], +(m[4] || 0), +(m[5] || 0));

    m = text.match(/^(\d{1,2})\.(\d{1,2})\.(\d{4})(?:[ ,]+(\d{1,2}):(\d{1,2})(?::(\d{1,2}))?)?/);
    if (m) return window.svtDateParts(+m[3], +m[2], +m[1], +(m[4] || 0), +(m[5] || 0));

    m = text.match(/^(\d{1,2})\/(\d{1,2})\/(\d{4})(?:[ ,]+(\d{1,2}):(\d{1,2})(?::(\d{1,2}))?\s*(AM|PM))?/i);
    if (m) {
        let h = +(m[4] || 0);
        if (m[7]) h = /p/i.test(m[7]) ? (h % 12) + 12 : h % 12;
        return window.svtDateParts(+m[3], +m[1], +m[2], h, +(m[5] || 0));
    }
    return null;
};

window.svtDateParts = function (y, mo, d, h, mi) {
    if (mo < 1 || mo > 12 || d < 1 || d > 31 || h > 23 || mi > 59) return null;
    return { y, mo, d, h, mi };
};

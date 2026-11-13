# -*- coding: utf-8 -*-
"""Wrap sections/subsections in screenshot-style accordion; add visible TOC."""
from pathlib import Path
import re
import html as htmlmod

p = Path(r"d:\SVT\docs\docs\svt-v-atlas\SVT-idealnyy-process.html")
text = p.read_text(encoding="utf-8")

HINTS = {
    "s1": "Предложение и кому идёт документ",
    "s2": "Девять шагов, кольцо, шесть работ",
    "shagi": "Как должно быть на Ш1…Ш9",
    "chelovek": "Четыре случая, когда нужен человек",
    "kolco": "Схема кольца ведения модели",
    "privychnye": "Привычная работа по шагам",
    "steps": "Как сейчас, в чём боль, как должно быть",
    "sh4-razbor": "Как суждение становится правилом",
    "kto-reshaet": "Кто решает и чего не делает",
    "roli-sh": "Пять ролей пользователей",
    "algo": "Автоподбор, дефлятор, синхронизация",
    "integ": "С кем СВТ обменивается",
    "subprocess": "Чем шесть работ отличаются",
    "kd": "Правила проверки КД-xx",
    "scenarii": "Сквозные сценарии",
    "s3": "Как собираем картину по шагам",
    "s4": "Три гипотезы Г1–Г3",
    "s5": "Следующие шаги из протокола",
    "s6": "Пробелы, которые закрывает бизнес",
    "s7": "Гипотезы подробно и идеи по шагам",
    "s7-1": "Г1, Г2, Г3 — если / то",
    "s7-2": "Двадцать идей по шагам",
    "s7-3": "Порядок волн",
    "s7-4": "Цифры, которые уже есть",
    "s8": "Кто за контур ведения",
    "s9": "Риски и меры",
    "s9-1": "Граница СВТ, Атлас, смежные системы",
    "s9-2": "Противоречия в материалах Атласа",
    "s10": "Стабилизация SQL, отдельно от процесса",
}

ACC_CSS = r"""
  .doc-toc { background:#fff; border:1px solid var(--line); border-radius:12px; padding:16px 20px 12px; margin:0 0 18px; }
  .doc-toc h2 { margin:0 0 10px; font-size:16px; color:var(--navy); }
  .doc-toc ol { margin:0; padding-left:22px; }
  .doc-toc > ol > li { margin:0 0 6px; color:#3d4c59; }
  .doc-toc a { color:var(--navy); text-decoration:none; }
  .doc-toc a:hover { text-decoration:underline; }
  .doc-toc .toc-sub { margin:4px 0 8px; padding-left:18px; list-style:disc; }
  .doc-toc .toc-sub li { margin:0 0 3px; font-size:13.5px; }
  .acc-stack { background:#fff; border:1px solid var(--line); border-radius:12px; overflow:hidden; margin-bottom:16px; }
  details.acc { background:#fff; border:0; border-bottom:1px solid var(--line); margin:0; border-radius:0; overflow:visible; }
  details.acc:last-child { border-bottom:0; }
  details.acc > summary { cursor:pointer; list-style:none; display:flex; align-items:center; gap:16px; padding:16px 18px; color:var(--navy); }
  details.acc > summary::-webkit-details-marker { display:none; }
  details.acc > summary::before { display:none; }
  details.acc > summary:hover { background:#fafcfe; }
  .acc-text { min-width:0; flex:1; }
  .acc-title { display:block; font-size:16px; font-weight:700; color:#1b2733; line-height:1.35; }
  .acc-hint { display:block; margin-top:3px; font-size:13.5px; font-weight:400; color:#8b9aab; line-height:1.35; }
  .acc-action { margin-left:auto; display:flex; align-items:center; gap:8px; flex-shrink:0; color:#7a8b9c; font-size:14px; font-weight:600; white-space:nowrap; }
  .acc-action::after { content:"Показать"; }
  details.acc[open] > summary .acc-action::after { content:"Скрыть"; }
  .acc-chev { width:28px; height:28px; border:1px solid #d5dee6; border-radius:50%; position:relative; flex-shrink:0; background:#fff; }
  .acc-chev::after { content:""; position:absolute; left:9px; top:10px; width:8px; height:8px; border-right:2px solid #8aa0b3; border-bottom:2px solid #8aa0b3; transform:rotate(45deg); }
  details.acc[open] > summary .acc-chev::after { transform:rotate(-135deg); top:12px; }
  details.acc > .acc-body { padding:4px 18px 18px; border-top:0; }
  details.acc.acc-nested { border-bottom:1px solid var(--line); }
  details.acc.acc-nested > summary { padding:14px 4px 14px 8px; }
  details.acc.acc-nested .acc-title { font-size:15px; }
  details.acc.acc-nested > .acc-body { padding:0 8px 16px 8px; }
  .acc-body .acc-stack { border:1px solid var(--line); margin:10px 0 0; }
  @media print {
    details.acc { border:0; }
    details.acc > summary { display:none; }
    details.acc > .acc-body { display:block !important; padding:0; }
  }
"""

# --- CSS: insert before existing details.fold block, keep fold for atlas-codes ---
old_fold = """  details.fold { background:var(--paper); border:1px solid var(--line); border-radius:12px; margin-bottom:16px; overflow:hidden; }"""
if ACC_CSS.strip() not in text:
    text = text.replace(old_fold, ACC_CSS + "\n" + old_fold, 1)

# --- Remove section-2 mini TOC ---
text = re.sub(
    r'\n  <nav class="sec-toc"[^>]*>.*?</nav>\n',
    "\n",
    text,
    count=1,
    flags=re.S,
)

TOC = """
  <nav class="doc-toc" id="oglavlenie" aria-label="Оглавление">
    <h2>Оглавление</h2>
    <ol>
      <li><a href="#s1">1. Зачем и как устроено</a></li>
      <li><a href="#s2">2. Что такое идеальный процесс СВТ</a>
        <ol class="toc-sub">
          <li><a href="#shagi">2.1 Девять шагов — как должно быть</a></li>
          <li><a href="#chelovek">2.2 Где нужен человек</a></li>
          <li><a href="#kolco">2.3 Кольцо</a></li>
          <li><a href="#privychnye">2.4 Привычная работа</a></li>
          <li><a href="#steps">2.5 Как сейчас, в чём боль, как должно быть</a></li>
          <li><a href="#sh4-razbor">2.6 Разбор шага Ш4</a></li>
          <li><a href="#kto-reshaet">2.7 Кто решает</a></li>
          <li><a href="#roli-sh">2.8 Роли пользователей</a></li>
          <li><a href="#algo">2.9 Алгоритмы</a></li>
          <li><a href="#integ">2.10 Интеграции</a></li>
          <li><a href="#subprocess">2.11 Шесть работ на одних шагах</a></li>
          <li><a href="#kd">2.12 Правила проверки</a></li>
          <li><a href="#scenarii">2.13 Сквозные сценарии</a></li>
        </ol>
      </li>
      <li><a href="#s3">3. Как ведём работу</a></li>
      <li><a href="#s4">4. Предложение: куда двигаемся</a></li>
      <li><a href="#s5">5. Что дальше</a></li>
      <li><a href="#s6">6. Вопросы к шагам</a></li>
      <li><a href="#s7">7. Гипотезы подробно</a>
        <ol class="toc-sub">
          <li><a href="#s7-1">7.1 Три гипотезы</a></li>
          <li><a href="#s7-2">7.2 Что улучшаем по шагам</a></li>
          <li><a href="#s7-3">7.3 В каком порядке улучшаем</a></li>
          <li><a href="#s7-4">7.4 Что изменится — в цифрах</a></li>
        </ol>
      </li>
      <li><a href="#s8">8. Роли и ответственность</a></li>
      <li><a href="#s9">9. Риски и меры</a>
        <ol class="toc-sub">
          <li><a href="#s9-1">9.1 Граница: СВТ, Атлас, смежные системы</a></li>
          <li><a href="#s9-2">9.2 Противоречия в материалах Атласа</a></li>
        </ol>
      </li>
      <li><a href="#s10">10. Параллельный трек: техника SQL</a></li>
    </ol>
  </nav>
"""

if 'id="oglavlenie"' not in text:
    marker = '  <details class="fold" id="atlas-codes">'
    if marker not in text:
        raise SystemExit("atlas-codes marker not found")
    text = text.replace(marker, TOC + "\n" + marker, 1)


def strip_tags(s):
    s = re.sub(r"<[^>]+>", "", s)
    s = htmlmod.unescape(s)
    return re.sub(r"\s+", " ", s).strip()


def acc_block(did, title, hint, inner, nested=False):
    cls = "acc acc-nested" if nested else "acc"
    hint_html = f'\n        <span class="acc-hint">{htmlmod.escape(hint)}</span>' if hint else ""
    return (
        f'<details class="{cls}" id="{did}">\n'
        f"    <summary>\n"
        f'      <span class="acc-text">\n'
        f'        <span class="acc-title">{htmlmod.escape(title)}</span>{hint_html}\n'
        f"      </span>\n"
        f'      <span class="acc-action"><span class="acc-chev" aria-hidden="true"></span></span>\n'
        f"    </summary>\n"
        f'    <div class="acc-body">\n{inner.rstrip()}\n    </div>\n'
        f"  </details>"
    )


def wrap_headings(html, tag_start, nested, next_pat, id_fallback):
    pattern = re.compile(tag_start + r"[^>]*>.*?</h[23]>", re.S)
    matches = list(pattern.finditer(html))
    for i, m in enumerate(reversed(matches)):
        orig_i = len(matches) - 1 - i
        heading = m.group(0)
        start, after = m.start(), m.end()
        rest = html[after:]
        nxt = re.search(next_pat, rest)
        end = after + nxt.start() if nxt else after + len(rest)
        inner = html[after:end]
        hid = re.search(r'\sid="([^"]+)"', heading)
        sn = re.search(r'class="sn">([^<]+)', heading)
        title = strip_tags(heading)
        title = re.sub(r"^(1|2|3|4|5|6|7|8|9|10)\s+", r"\1. ", title)
        did = hid.group(1) if hid else None
        if not did and sn:
            did = id_fallback(sn.group(1).strip())
        if not did:
            did = f"auto-{orig_i}"
        hint = HINTS.get(did, "")
        wrapped = acc_block(did, title, hint, inner, nested=nested)
        html = html[:start] + wrapped + html[end:]
    return html


def id_from_sn(sn):
    # "2.1" stay as existing ids; "7.1" -> s7-1
    if sn.startswith("7."):
        return "s7-" + sn.split(".", 1)[1]
    if sn.startswith("9."):
        return "s9-" + sn.split(".", 1)[1]
    return "sub-" + sn.replace(".", "-")


# Wrap subsections first, then sections. Do not touch <script>.
split_at = text.find("<p class=\"footer\">")
if split_at < 0:
    raise SystemExit("footer not found")
head, tail = text[:split_at], text[split_at:]

head = wrap_headings(
    head,
    r"<h3 class=\"sub\"",
    nested=True,
    next_pat=r"<h3 class=\"sub\"|<h2 class=\"sec\"|<details class=\"acc\"",
    id_fallback=id_from_sn,
)
head = wrap_headings(
    head,
    r"<h2 class=\"sec\"",
    nested=False,
    next_pat=r"<h2 class=\"sec\"|<details class=\"acc\"(?! acc-nested)",
    id_fallback=lambda sn: "s" + sn,
)

# After h3 wrap, h2's next sibling might be details.acc acc-nested — the second wrap
# next_pat for h2 should stop at next h2 or next top-level details.acc that is NOT nested.
# But we already converted h2 to details.acc as well in second pass using original h2 tags.
# First pass converted h3 -> details.acc.acc-nested, h2 still present.
# Second pass wraps h2 until next h2. Good — nested details are inside.

# Wrap the sequence of top-level details.acc (not nested) in acc-stack.
# Find first <details class="acc" id="s1">
m1 = re.search(r'<details class="acc" id="s1">', head)
if not m1:
    raise SystemExit("s1 accordion not created")
# last top-level before footer is s10
# insert acc-stack open before s1, close before footer (end of head)
head = head[: m1.start()] + '<div class="acc-stack">\n  ' + head[m1.start():] + "\n  </div>\n"

text = head + tail

# JS: open ancestor details for any hash
OLD_JS = """(function () {
  var fold = document.getElementById('atlas-codes');
  if (!fold) return;
  function openTo(id) {
    if (!id) return;
    var row = document.getElementById(id);
    if (!row || !fold.contains(row)) return;
    fold.open = true;
    requestAnimationFrame(function () { row.scrollIntoView({ block: 'center' }); });
  }
  document.addEventListener('click', function (e) {
    var a = e.target.closest ? e.target.closest('a.code') : null;
    if (!a) return;
    var href = a.getAttribute('href') || '';
    if (href.charAt(0) === '#') openTo(href.slice(1));
  });
  window.addEventListener('hashchange', function () { openTo((location.hash || '').slice(1)); });
  openTo((location.hash || '').slice(1));
})();"""

NEW_JS = """(function () {
  function openAncestors(el) {
    if (!el) return;
    var n = el;
    if (el.tagName === 'DETAILS') el.open = true;
    while (n) {
      if (n.tagName === 'DETAILS') n.open = true;
      n = n.parentElement;
    }
  }
  function openTo(id) {
    if (!id) return;
    var el = document.getElementById(id);
    if (!el) return;
    openAncestors(el);
    requestAnimationFrame(function () { el.scrollIntoView({ block: 'start' }); });
  }
  document.addEventListener('click', function (e) {
    var a = e.target.closest ? e.target.closest('a[href^="#"]') : null;
    if (!a) return;
    var href = a.getAttribute('href') || '';
    if (href.length > 1) openTo(href.slice(1));
  });
  window.addEventListener('hashchange', function () { openTo((location.hash || '').slice(1)); });
  openTo((location.hash || '').slice(1));
})();"""

if OLD_JS not in text:
    raise SystemExit("old fold JS not found")
text = text.replace(OLD_JS, NEW_JS, 1)

p.write_text(text, encoding="utf-8")
print("ok", p.stat().st_size)
# sanity
c = p.read_text(encoding="utf-8")
print("s1", c.count('id="s1"'))
print("acc", c.count("details class=\"acc\""))
print("nested", c.count("acc-nested"))
print("oglavlenie", 'id="oglavlenie"' in c)
print("svt-ring script", "var el = document.getElementById('svt-ring')" in c)
print("h2 remaining in main", len(re.findall(r"<h2 class=\"sec\"", c)))
print("h3 remaining", len(re.findall(r"<h3 class=\"sub\"", c)))

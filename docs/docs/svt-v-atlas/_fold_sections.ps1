# Wrap sections/subsections in screenshot-style accordion; add visible TOC.
$ErrorActionPreference = 'Stop'
$path = 'd:\SVT\docs\docs\svt-v-atlas\SVT-idealnyy-process.html'
$utf8 = New-Object System.Text.UTF8Encoding $false
$text = [System.IO.File]::ReadAllText($path, $utf8)

$hints = @{
  's1' = 'Предложение и кому идёт документ'
  's2' = 'Девять шагов, кольцо, шесть работ'
  'shagi' = 'Как должно быть на Ш1…Ш9'
  'chelovek' = 'Четыре случая, когда нужен человек'
  'kolco' = 'Схема кольца ведения модели'
  'privychnye' = 'Привычная работа по шагам'
  'steps' = 'Как сейчас, в чём боль, как должно быть'
  'sh4-razbor' = 'Как суждение становится правилом'
  'kto-reshaet' = 'Кто решает и чего не делает'
  'roli-sh' = 'Пять ролей пользователей'
  'algo' = 'Автоподбор, дефлятор, синхронизация'
  'integ' = 'С кем СВТ обменивается'
  'subprocess' = 'Чем шесть работ отличаются'
  'kd' = 'Правила проверки КД-xx'
  'scenarii' = 'Сквозные сценарии'
  's3' = 'Как собираем картину по шагам'
  's4' = 'Три гипотезы Г1–Г3'
  's5' = 'Следующие шаги из протокола'
  's6' = 'Пробелы, которые закрывает бизнес'
  's7' = 'Гипотезы подробно и идеи по шагам'
  's7-1' = 'Г1, Г2, Г3 — если / то'
  's7-2' = 'Двадцать идей по шагам'
  's7-3' = 'Порядок волн'
  's7-4' = 'Цифры, которые уже есть'
  's8' = 'Кто за контур ведения'
  's9' = 'Риски и меры'
  's9-1' = 'Граница СВТ, Атлас, смежные системы'
  's9-2' = 'Противоречия в материалах Атласа'
  's10' = 'Стабилизация SQL, отдельно от процесса'
}

$accCss = @'

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
  details.acc > summary::-webkit-details-marker,
  details.acc > summary::marker { display:none; content:""; }
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
  details.acc, #oglavlenie { scroll-margin-top:80px; }
  @media print {
    details.acc { border:0; }
    details.acc > summary { display:none; }
    details.acc > .acc-body { display:block !important; padding:0; }
  }
'@

$oldFold = '  details.fold { background:var(--paper); border:1px solid var(--line); border-radius:12px; margin-bottom:16px; overflow:hidden; }'
if ($text -notmatch [regex]::Escape('details.acc > summary')) {
  if ($text.Contains($oldFold)) {
    $text = $text.Replace($oldFold, ($accCss.TrimEnd() + "`n" + $oldFold))
  } else {
    throw 'fold CSS marker not found'
  }
}

$text = [regex]::Replace($text, '(?s)\n  <nav class="sec-toc"[^>]*>.*?</nav>\n', "`n", 1)

$toc = @'

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
'@

if ($text -notmatch 'id="oglavlenie"') {
  $marker = '  <details class="fold" id="atlas-codes">'
  if (-not $text.Contains($marker)) { throw 'atlas-codes marker not found' }
  $text = $text.Replace($marker, ($toc.TrimEnd() + "`n`n" + $marker))
}

function Strip-Tags([string]$s) {
  $s = [regex]::Replace($s, '<[^>]+>', '')
  $s = [System.Net.WebUtility]::HtmlDecode($s)
  return [regex]::Replace($s, '\s+', ' ').Trim()
}

function New-AccBlock([string]$did, [string]$title, [string]$hint, [string]$inner, [bool]$nested) {
  $cls = if ($nested) { 'acc acc-nested' } else { 'acc' }
  $hintHtml = ''
  if ($hint) {
    $encHint = [System.Net.WebUtility]::HtmlEncode($hint)
    $hintHtml = "`n        <span class=`"acc-hint`">$encHint</span>"
  }
  $titleEnc = [System.Net.WebUtility]::HtmlEncode($title)
  $body = $inner.TrimEnd()
  $nl = [Environment]::NewLine
  return ('<details class="' + $cls + '" id="' + $did + '">' + $nl +
    '    <summary>' + $nl +
    '      <span class="acc-text">' + $nl +
    '        <span class="acc-title">' + $titleEnc + '</span>' + $hintHtml + $nl +
    '      </span>' + $nl +
    '      <span class="acc-action"><span class="acc-chev" aria-hidden="true"></span></span>' + $nl +
    '    </summary>' + $nl +
    '    <div class="acc-body">' + $nl +
    $body + $nl +
    '    </div>' + $nl +
    '  </details>')
}

function Wrap-Headings([string]$html, [string]$tagStart, [bool]$nested, [string]$nextPat) {
  $pattern = [regex]::new($tagStart + '[^>]*>.*?</h[23]>', [System.Text.RegularExpressions.RegexOptions]::Singleline)
  $ms = $pattern.Matches($html)
  for ($i = $ms.Count - 1; $i -ge 0; $i--) {
    $m = $ms[$i]
    $heading = $m.Value
    $start = $m.Index
    $after = $m.Index + $m.Length
    $rest = $html.Substring($after)
    $nxt = [regex]::Match($rest, $nextPat)
    $end = if ($nxt.Success) { $after + $nxt.Index } else { $html.Length }
    $inner = $html.Substring($after, $end - $after)
    $hid = [regex]::Match($heading, '\sid="([^"]+)"')
    $sn = [regex]::Match($heading, 'class="sn">([^<]+)')
    $title = Strip-Tags $heading
    $title = [regex]::Replace($title, '^(10|[1-9])(?=\S)', '$1. ')
    $title = [regex]::Replace($title, '^(\d+\.\d+)(?=\S)', '$1 ')
    $did = $null
    if ($hid.Success) { $did = $hid.Groups[1].Value }
    elseif ($sn.Success) {
      $n = $sn.Groups[1].Value.Trim()
      if ($n.StartsWith('7.')) { $did = 's7-' + $n.Split('.')[1] }
      elseif ($n.StartsWith('9.')) { $did = 's9-' + $n.Split('.')[1] }
      else { $did = 'sub-' + $n.Replace('.', '-') }
    }
    if (-not $did) { $did = "auto-$i" }
    $hint = ''
    if ($hints.ContainsKey($did)) { $hint = $hints[$did] }
    $wrapped = New-AccBlock $did $title $hint $inner $nested
    $html = $html.Substring(0, $start) + $wrapped + $html.Substring($end)
  }
  return $html
}

$footerAt = $text.IndexOf('<p class="footer">')
if ($footerAt -lt 0) { throw 'footer not found' }
$head = $text.Substring(0, $footerAt)
$tail = $text.Substring($footerAt)

# Subsections first. next_pat must match already-wrapped nested siblings
# (`class="acc acc-nested"`) so earlier h3 does not swallow later ones.
$head = Wrap-Headings $head '<h3 class="sub"' $true '<h3 class="sub"|<h2 class="sec"|<details class="acc'

# Wrap consecutive nested plates in a stack
$nestedRun = [regex]::new('(?:[ \t]*<details class="acc acc-nested"[\s\S]*?</details>\s*){2,}')
$nruns = $nestedRun.Matches($head)
for ($i = $nruns.Count - 1; $i -ge 0; $i--) {
  $m = $nruns[$i]
  $block = $m.Value.TrimEnd()
  $repl = '<div class="acc-stack">' + "`n  " + $block + "`n  </div>`n"
  $head = $head.Substring(0, $m.Index) + $repl + $head.Substring($m.Index + $m.Length)
}

# Sections. Stop at next h2 or already-wrapped top-level details.acc id="s…"
$head = Wrap-Headings $head '<h2 class="sec"' $false '<h2 class="sec"|<details class="acc" id="'

$m1 = [regex]::Match($head, '<details class="acc" id="s1">')
if (-not $m1.Success) { throw 's1 accordion not created' }
$head = $head.Substring(0, $m1.Index) + "<div class=`"acc-stack`">`n  " + $head.Substring($m1.Index) + "`n  </div>`n"

$text = $head + $tail

$oldJs = @'
(function () {
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
})();
'@

$newJs = @'
(function () {
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
})();
'@

if (-not $text.Contains($oldJs)) { throw 'old fold JS not found' }
$text = $text.Replace($oldJs, $newJs)

[System.IO.File]::WriteAllText($path, $text, $utf8)
$c = [System.IO.File]::ReadAllText($path, $utf8)
Write-Output ('ok size=' + $c.Length)
Write-Output ('s1=' + ([regex]::Matches($c, 'id="s1"')).Count)
Write-Output ('acc=' + ([regex]::Matches($c, 'details class="acc"')).Count)
Write-Output ('nested=' + ([regex]::Matches($c, 'acc-nested')).Count)
Write-Output ('oglavlenie=' + ($c.Contains('id="oglavlenie"')))
Write-Output ('svt-ring=' + $c.Contains("var el = document.getElementById('svt-ring')"))
Write-Output ('h2-sec=' + ([regex]::Matches($c, '<h2 class="sec"')).Count)
Write-Output ('h3-sub=' + ([regex]::Matches($c, '<h3 class="sub"')).Count)
$ids = [regex]::Matches($c, 'details class="acc(?: acc-nested)?" id="([^"]+)"') | ForEach-Object { $_.Groups[1].Value }
Write-Output ('ids=' + ($ids -join ','))

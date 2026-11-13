# HTML (разделы 0 / 1а / 1б) → Markdown без CSS/JS.
# Usage:
#   .\extract.ps1
#   .\extract.ps1 -HtmlPath "...\file.html" -MdPath "...\разделы_0_1а_1б.md"
param(
  [string]$HtmlPath = '',
  [string]$MdPath = ''
)
$ErrorActionPreference = 'Stop'
$repoDocs = Join-Path (Split-Path (Split-Path (Split-Path (Split-Path $PSScriptRoot)))) 'docs\docs\План-график Атлас'
# PSScriptRoot = .../.cursor/skills/plan-grafik-md-html/scripts
# up1=plan-grafik-md-html, up2=skills, up3=.cursor, up4=repo root
$repoRoot = Split-Path (Split-Path (Split-Path (Split-Path $PSScriptRoot)))
$defaultDir = Join-Path $repoRoot 'docs\docs\План-график Атлас'
if (-not (Test-Path -LiteralPath $defaultDir)) {
  $defaultDir = Split-Path -Parent $PSScriptRoot
  if (-not (Test-Path -LiteralPath (Join-Path $defaultDir 'Логистика*v2_2.html'))) {
    $defaultDir = 'd:\SVT\docs\docs\План-график Атлас'
  }
}
if (-not $HtmlPath) {
  $HtmlPath = (Get-ChildItem -LiteralPath $defaultDir -Filter 'Логистика*v2_2.html' | Select-Object -First 1).FullName
}
if (-not $MdPath) {
  $MdPath = Join-Path (Split-Path -Parent $HtmlPath) 'разделы_0_1а_1б.md'
}

$enc = New-Object System.Text.UTF8Encoding $false
$t = [IO.File]::ReadAllText($HtmlPath, $enc)

function HtmlDecode([string]$s) {
  if ([string]::IsNullOrEmpty($s)) { return '' }
  return [System.Net.WebUtility]::HtmlDecode($s)
}
function StripTags([string]$s) {
  if ([string]::IsNullOrEmpty($s)) { return '' }
  $s = [regex]::Replace($s, '(?is)<br\s*/?>', "`n")
  $s = [regex]::Replace($s, '(?is)</(p|div|li|h\d|tr)>', "`n")
  $s = [regex]::Replace($s, '(?is)<li[^>]*>', '- ')
  $s = [regex]::Replace($s, '(?is)<t[dh][^>]*>', '| ')
  $s = [regex]::Replace($s, '(?is)</t[dh]>', ' ')
  $s = [regex]::Replace($s, '(?is)<b>|</b>|<strong>|</strong>', '**')
  $s = [regex]::Replace($s, '(?is)<i>|</i>|<em>|</em>', '_')
  $s = [regex]::Replace($s, '(?is)<span[^>]*class="p0-muted"[^>]*>(.*?)</span>', ' ($1)')
  $s = [regex]::Replace($s, '(?is)<[^>]+>', '')
  $s = HtmlDecode $s
  $s = [regex]::Replace($s, "[ \t]+\n", "`n")
  $s = [regex]::Replace($s, "\n{3,}", "`n`n")
  return $s.Trim()
}
function SecBetween([string]$startId, [string]$endId) {
  $a = $t.IndexOf("id=`"$startId`"")
  $b = $t.IndexOf("id=`"$endId`"")
  if ($a -lt 0 -or $b -le $a) { throw "bounds $startId -> $endId" }
  $secStart = $t.LastIndexOf('<section', $a)
  return $t.Substring($secStart, $b - $secStart)
}

$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine('---')
[void]$sb.AppendLine('source: ' + [IO.Path]::GetFileName($HtmlPath))
[void]$sb.AppendLine('sections: [0, 1а, 1б]')
[void]$sb.AppendLine('extracted: ' + (Get-Date -Format 'yyyy-MM-dd'))
[void]$sb.AppendLine('note: >')
[void]$sb.AppendLine('  Редактируемый снимок разделов 0 / 1а / 1б без CSS/JS.')
[void]$sb.AppendLine('  Обратная запись в HTML — skill plan-grafik-md-html.')
[void]$sb.AppendLine('---')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('# План-график Атлас: разделы 0, 1а, 1б')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('Источник: интерактивный HTML план-графика. Здесь только текст и таблицы данных.')
[void]$sb.AppendLine('')

# ========== SECTION 0 ==========
$s0 = SecBetween 'pres0-sec' 'goal-sec'
$s0Body = [regex]::Replace($s0, '(?is)<style>.*?</style>', '')
$s0Body = [regex]::Replace($s0Body, '(?is)<script\b[^>]*>.*?</script>', '')

$h2 = [regex]::Match($s0Body, '(?is)<h2>(.*?)</h2>').Groups[1].Value
$lead = [regex]::Match($s0Body, '(?is)<div class="sec-head">.*?<p>(.*?)</p>').Groups[1].Value
[void]$sb.AppendLine('## 0. ' + (StripTags $h2).Replace('0. ', ''))
[void]$sb.AppendLine('')
[void]$sb.AppendLine((StripTags $lead))
[void]$sb.AppendLine('')

$ring = [regex]::Match($s0Body, '(?is)<div class="p0-ring">(.*?)</div>\s*<h3 class="p0-h3">Стримы</h3>')
if ($ring.Success) {
  $ringHtml = $ring.Groups[1].Value
  [void]$sb.AppendLine('### Кольцо логистики')
  [void]$sb.AppendLine('')
  $rlead = [regex]::Match($ringHtml, '(?is)<p class="p0-lead">(.*?)</p>').Groups[1].Value
  [void]$sb.AppendLine((StripTags $rlead))
  [void]$sb.AppendLine('')
  [void]$sb.AppendLine('#### Шаги Ш1–Ш9')
  [void]$sb.AppendLine('')
  $steps9 = [regex]::Match($ringHtml, '(?is)<div class="p0-steps">(.*?)</div>\s*<p class="p0-note"')
  if ($steps9.Success) {
    foreach ($sm in [regex]::Matches($steps9.Groups[1].Value, '(?is)<div class="p0-step(\s+on)?\s*"><i>(.*?)</i><b>(.*?)</b><span>(.*?)</span></div>')) {
      $on = if ($sm.Groups[1].Value -match 'on') { ' (прототип 2026)' } else { '' }
      [void]$sb.AppendLine(('- **' + (StripTags $sm.Groups[2].Value) + ' ' + (StripTags $sm.Groups[3].Value) + '**' + $on + ' — ' + (StripTags $sm.Groups[4].Value)))
    }
  }
  [void]$sb.AppendLine('')
  $note = [regex]::Match($ringHtml, '(?is)<p class="p0-note">(.*?)</p>').Groups[1].Value
  if ($note) { [void]$sb.AppendLine('> ' + (StripTags $note)); [void]$sb.AppendLine('') }
  [void]$sb.AppendLine('#### Кольцо разработки (12 шагов)')
  [void]$sb.AppendLine('')
  $ring12 = [regex]::Match($ringHtml, '(?is)<div class="p0-steps p0-ring12">((?:<div class="p0-step\s*">.*?</div>)+)\s*</div>')
  if ($ring12.Success) {
    foreach ($sm in [regex]::Matches($ring12.Groups[1].Value, '(?is)<div class="p0-step\s*"><i>(.*?)</i><b>(.*?)</b><span>(.*?)</span></div>')) {
      [void]$sb.AppendLine(('- **' + (StripTags $sm.Groups[1].Value) + '. ' + (StripTags $sm.Groups[2].Value) + '** — ' + (StripTags $sm.Groups[3].Value)))
    }
  }
  [void]$sb.AppendLine('')
}

[void]$sb.AppendLine('### Стримы')
[void]$sb.AppendLine('')

foreach ($dm in [regex]::Matches($s0Body, '(?is)<details class="p0-stream([^"]*)"\s+id="(p0-[^"]+)"[^>]*>(.*?)</details>')) {
  $cls = $dm.Groups[1].Value
  $id = $dm.Groups[2].Value
  $body = $dm.Groups[3].Value
  $hl = if ($cls -match 'p0-blued') { 'blue' } elseif ($cls -match 'p0-edited') { 'edit' } else { '' }
  $sn = [regex]::Match($body, '(?is)<span class="p0-sn">(.*?)</span>').Groups[1].Value
  $ss = [regex]::Match($body, '(?is)<span class="p0-ss">(.*?)</span>').Groups[1].Value
  [void]$sb.AppendLine('#### ' + (StripTags $sn) + ' {' + $id + '}')
  [void]$sb.AppendLine('')
  if ($hl) { [void]$sb.AppendLine('- highlight: ' + $hl) }
  [void]$sb.AppendLine('- meta: ' + (StripTags $ss))
  [void]$sb.AppendLine('')

  foreach ($bx in [regex]::Matches($body, '(?is)<div class="p0-box[^"]*"><h4>([а-яa-z0-9\)\.\s]+)</h4>(.*?)</div>')) {
    $title = StripTags $bx.Groups[1].Value
    $content = StripTags $bx.Groups[2].Value
    if ($title -match '^[абв]') {
      [void]$sb.AppendLine('##### ' + $title)
      [void]$sb.AppendLine('')
      [void]$sb.AppendLine($content)
      [void]$sb.AppendLine('')
    }
  }

  $gNote = [regex]::Match($body, '(?is)<p class="p0-note[^"]*">(.*?)</p>')
  if ($gNote.Success) {
    [void]$sb.AppendLine('##### г) Вехи: результат и метрики')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('> ' + (StripTags $gNote.Groups[1].Value))
    [void]$sb.AppendLine('')
  }

  $tl = [regex]::Match($body, '(?is)<div class="p0-tl[^"]*"[^>]*>(.*?)</div>\s*<div class="p0-cards">')
  if ($tl.Success) {
    [void]$sb.AppendLine('###### Полоска вех')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('| Дата | Веха | Класс | Подпись |')
    [void]$sb.AppendLine('|---|---|---|---|')
    foreach ($ti in [regex]::Matches($tl.Groups[1].Value, '(?is)<div class="p0-tl-i\s*([^"]*)"><b>(.*?)</b><strong>(.*?)</strong><span>(.*?)</span></div>')) {
      $klass = $ti.Groups[1].Value.Trim()
      if (-not $klass) { $klass = 'later' }
      [void]$sb.AppendLine('| ' + (StripTags $ti.Groups[2].Value) + ' | ' + (StripTags $ti.Groups[3].Value) + ' | ' + $klass + ' | ' + (StripTags $ti.Groups[4].Value) + ' |')
    }
    [void]$sb.AppendLine('')
  }

  $cards = [regex]::Matches($body, '(?is)<div class="p0-card([^"]*)">\s*<div class="p0-ch"><b>(.*?)</b>(.*?)</div>\s*<div class="p0-ct">(.*?)</div>(.*?)</div>(?=\s*<div class="p0-card|\s*<h4|\s*$)')
  if ($cards.Count -gt 0) {
    [void]$sb.AppendLine('###### Карточки вех')
    [void]$sb.AppendLine('')
    foreach ($c in $cards) {
      $date = StripTags $c.Groups[2].Value
      $ch = $c.Groups[3].Value
      $ct = StripTags $c.Groups[4].Value
      $rest = $c.Groups[5].Value
      $tag = [regex]::Match($ch, '(?is)<span class="p0-tag">(.*?)</span>').Groups[1].Value
      $st = [regex]::Match($ch, '(?is)<span class="p0-st[^"]*">(.*?)</span>').Groups[1].Value
      $lane = [regex]::Match($ch, '(?is)<span class="p0-lane">(.*?)</span>').Groups[1].Value
      [void]$sb.AppendLine('**' + $date + ' — ' + $ct + '**')
      $metaBits = @()
      if ($tag) { $metaBits += ('тип: ' + (StripTags $tag)) }
      if ($st) { $metaBits += ('статус: ' + (StripTags $st)) }
      if ($lane) { $metaBits += ('дорожка: ' + (StripTags $lane)) }
      if ($metaBits.Count) { [void]$sb.AppendLine('- ' + ($metaBits -join ' · ')) }
      foreach ($cm in [regex]::Matches($rest, '(?is)<div class="p0-cm"><span class="p0-l">(.*?)</span>(.*?)</div>')) {
        [void]$sb.AppendLine('- **' + (StripTags $cm.Groups[1].Value) + ':** ' + (StripTags $cm.Groups[2].Value).Replace("`n", ' / '))
      }
      $cr = [regex]::Match($rest, '(?is)<div class="p0-cr"><span class="p0-l">(.*?)</span>(.*?)</div>')
      if ($cr.Success) {
        [void]$sb.AppendLine('- **' + (StripTags $cr.Groups[1].Value) + ':** ' + (StripTags $cr.Groups[2].Value).Replace("`n", ' '))
      }
      [void]$sb.AppendLine('')
    }
  }

  foreach ($h4 in [regex]::Matches($body, '(?is)<h4 class="p0-h4[^"]*">(.*?)</h4>\s*(<div class="p0-(?:box|grid2)[^"]*"[^>]*>.*?</div>)')) {
    $ht = StripTags $h4.Groups[1].Value
    if ($ht -match '^[деж]') {
      [void]$sb.AppendLine('##### ' + $ht)
      [void]$sb.AppendLine('')
      [void]$sb.AppendLine((StripTags $h4.Groups[2].Value))
      [void]$sb.AppendLine('')
    }
  }
}

# ========== SECTION 1а ==========
$s1a = SecBetween 'streams-sec' 'pipe6-sec'
$s1aClean = [regex]::Replace($s1a, '(?is)<script\b[^>]*>.*?</script>', '')
$h2a = [regex]::Match($s1aClean, '(?is)<h2>(.*?)</h2>').Groups[1].Value
$leada = [regex]::Match($s1aClean, '(?is)<div class="sec-head">.*?<p>(.*?)</p>').Groups[1].Value
[void]$sb.AppendLine('## 1а. ' + (StripTags $h2a).Replace('1а. ', '').Replace('1a. ', ''))
[void]$sb.AppendLine('')
[void]$sb.AppendLine((StripTags $leada))
[void]$sb.AppendLine('')
[void]$sb.AppendLine('Интерактивная таблица и iframe в HTML строятся из `V12_SPEC` + `TASKS`. Ниже — снимок вех по стримам из `V12_SPEC`.')
[void]$sb.AppendLine('')

$vm = [regex]::Match($t, 'const V12_SPEC = (\{.*?\});\r?\nconst V12_EXTRA', [System.Text.RegularExpressions.RegexOptions]::Singleline)
if (-not $vm.Success) { throw 'V12_SPEC not found' }
$spec = $vm.Groups[1].Value | ConvertFrom-Json
$typeMap = @{ del = 'запуск'; roll = 'тираж'; dev = 'развитие'; std = 'веха' }

foreach ($stream in $spec.streams) {
  [void]$sb.AppendLine('### Стрим `' + $stream.code + '`: ' + $stream.n)
  [void]$sb.AppendLine('')
  [void]$sb.AppendLine('- Ответственный бизнеса: ' + $stream.lb)
  [void]$sb.AppendLine('- ИТ-лидер: ' + $stream.li)
  if ($stream.g) { [void]$sb.AppendLine('- Группа: ' + $stream.g) }
  if ($stream.mtgoal) { [void]$sb.AppendLine('- Цель метрик: ' + $stream.mtgoal) }
  [void]$sb.AppendLine('')
  if ($stream.roster) {
    [void]$sb.AppendLine('| Роль-класс | ФИО | Роль |')
    [void]$sb.AppendLine('|---|---|---|')
    foreach ($r in $stream.roster) {
      [void]$sb.AppendLine('| ' + $r[0] + ' | ' + $r[1] + ' | ' + $r[2] + ' |')
    }
    [void]$sb.AppendLine('')
  }
  [void]$sb.AppendLine('| Код | Тип | Дата | Веха | Ответственный | Шаги кольца |')
  [void]$sb.AppendLine('|---|---|---|---|---|---|')
  foreach ($rel in $stream.rels) {
    $ty = if ($typeMap.ContainsKey([string]$rel.type)) { $typeMap[[string]$rel.type] } else { $rel.type }
    $steps = @()
    if ($rel.steps) { $rel.steps.PSObject.Properties | ForEach-Object { $steps += $_.Name } }
    $stepStr = ($steps | Sort-Object { [int]$_ }) -join ','
    [void]$sb.AppendLine('| `' + $rel.code + '` | ' + $ty + ' | ' + $rel.d + ' | ' + ($rel.t -replace '\|','/') + ' | ' + ($rel.own -replace '\|','/') + ' | ' + $stepStr + ' |')
  }
  [void]$sb.AppendLine('')
  [void]$sb.AppendLine('<details><summary>Метрики и критерии вех стрима ' + $stream.code + '</summary>')
  [void]$sb.AppendLine('')
  foreach ($rel in $stream.rels) {
    [void]$sb.AppendLine('#### `' + $rel.code + '` — ' + $rel.t)
    [void]$sb.AppendLine('')
    if ($rel.metric) { [void]$sb.AppendLine('- Критерий/метрика: ' + $rel.metric) }
    if ($rel.avail) { [void]$sb.AppendLine('- Доступность: ' + $rel.avail) }
    if ($rel.mt -and $rel.mt.what) { [void]$sb.AppendLine('- what: ' + $rel.mt.what) }
    [void]$sb.AppendLine('')
  }
  [void]$sb.AppendLine('</details>')
  [void]$sb.AppendLine('')
}

# ========== SECTION 1б ==========
$s1b = SecBetween 'pipe6-sec' 'next-sec'
$s1bClean = [regex]::Replace($s1b, '(?is)<script\b[^>]*>.*?</script>', '')
$h2b = [regex]::Match($s1bClean, '(?is)<h2>(.*?)</h2>').Groups[1].Value
$leadb = [regex]::Match($s1bClean, '(?is)<div class="sec-head">.*?<p>(.*?)</p>').Groups[1].Value
[void]$sb.AppendLine('## 1б. ' + (StripTags $h2b).Replace('1б. ', '').Replace('1b. ', ''))
[void]$sb.AppendLine('')
[void]$sb.AppendLine((StripTags $leadb))
[void]$sb.AppendLine('')
[void]$sb.AppendLine('Пайплайн в HTML рисуется скриптом из тех же `V12_SPEC.streams[].rels` и задач `TASKS` (окно «2 недели / месяц» от даты в UI).')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('Стримы пайплайна (как в UI 1б):')
[void]$sb.AppendLine('')
$p6List = @('АВТО','АГЕНТ','ПРАВ','РЕШ','ИНФРА','КОММ','СВТ')
foreach ($code in $p6List) {
  $st = $spec.streams | Where-Object { $_.code -eq $code } | Select-Object -First 1
  if ($st) {
    [void]$sb.AppendLine(('- **' + $st.n + '** (`' + $code + '`) — вех: ' + $st.rels.Count + '; лидер: ' + $st.lb + ' / ' + $st.li))
  } else {
    [void]$sb.AppendLine('- `' + $code + '` — нет в V12_SPEC')
  }
}
[void]$sb.AppendLine('')
[void]$sb.AppendLine('> Чтобы обновить интерактив 1б, правьте вехи в MD раздела 1а (таблица `V12_SPEC`) через apply.ps1; HTML-скрипты пересобирают пайплайн при открытии.')
[void]$sb.AppendLine('')

[IO.File]::WriteAllText($MdPath, $sb.ToString(), $enc)
Write-Host "OK extract -> $MdPath"
Write-Host ("chars: " + $sb.Length)

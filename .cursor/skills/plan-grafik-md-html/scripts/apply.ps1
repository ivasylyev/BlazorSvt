# Markdown (разделы 0 / 1а / 1б) → обновление HTML с сохранением CSS/JS.
param(
  [string]$HtmlPath = '',
  [string]$MdPath = '',
  [ValidateSet('intros','v12','p0','all')]
  [string]$What = 'all',
  [string]$StreamId = '',
  [switch]$DryRun,
  [switch]$NoBackup
)
$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path (Split-Path (Split-Path (Split-Path $PSScriptRoot)))
$defaultDir = Join-Path $repoRoot 'docs\docs\План-график Атлас'
if (-not (Test-Path -LiteralPath $defaultDir)) { $defaultDir = 'd:\SVT\docs\docs\План-график Атлас' }
if (-not $HtmlPath) {
  $HtmlPath = (Get-ChildItem -LiteralPath $defaultDir -Filter 'Логистика*v2_2.html' | Select-Object -First 1).FullName
}
if (-not $MdPath) { $MdPath = Join-Path (Split-Path -Parent $HtmlPath) 'разделы_0_1а_1б.md' }

$enc = New-Object System.Text.UTF8Encoding $false
$script:Doc = [IO.File]::ReadAllText($HtmlPath, $enc)
$script:Md = [IO.File]::ReadAllText($MdPath, $enc)
$script:Stats = [ordered]@{ intros = 0; v12 = 0; p0 = 0; skipped = 0 }

function EscText([string]$s) {
  if ($null -eq $s) { return '' }
  return $s.Replace('&', '&amp;').Replace('<', '&lt;').Replace('>', '&gt;')
}
function NormText([string]$s) {
  if ([string]::IsNullOrEmpty($s)) { return '' }
  $s = [regex]::Replace($s, '(?is)<br\s*/?>', ' ')
  $s = [regex]::Replace($s, '(?is)<li[^>]*>', ' ')
  $s = [regex]::Replace($s, '(?is)<[^>]+>', ' ')
  $s = [System.Net.WebUtility]::HtmlDecode($s)
  $s = [regex]::Replace($s, '\*\*', '')
  $s = [regex]::Replace($s, '\s+', ' ').Trim()
  return $s
}
function MdInlineToHtml([string]$s) {
  if ([string]::IsNullOrEmpty($s)) { return '' }
  $s = EscText $s
  return [regex]::Replace($s, '\*\*(.+?)\*\*', '<b>$1</b>')
}
function MdBlockToHtml([string]$block) {
  $block = $block.Trim()
  if (-not $block) { return '<p></p>' }
  $lines = @($block -split "`r?`n" | Where-Object { $_.Trim().Length -gt 0 -or $_ -match '^\s*-\s+' })
  $listLines = @($lines | Where-Object { $_ -match '^\s*-\s+' })
  if ($listLines.Count -ge 1 -and $listLines.Count -ge [Math]::Ceiling(($lines | Where-Object { $_.Trim() }).Count * 0.5)) {
    $lis = foreach ($ln in $lines) {
      if ($ln -match '^\s*-\s+(.*)$') { '<li>' + (MdInlineToHtml $Matches[1].Trim()) + '</li>' }
    }
    return '<ul>' + ($lis -join '') + '</ul>'
  }
  $paras = @([regex]::Split($block.Trim(), '(\r?\n){2,}') | Where-Object { $_.Trim() -and $_ -notmatch '^\r?\n+$' })
  return (($paras | ForEach-Object { '<p>' + (MdInlineToHtml ($_.Trim() -replace '\s*\r?\n\s*', ' ')) + '</p>' }) -join '')
}
function JsonEscape([string]$s) {
  if ($null -eq $s) { return '' }
  return $s.Replace('\', '\\').Replace('"', '\"').Replace("`r", '').Replace("`n", '\n').Replace("`t", '\t')
}
function ReplaceJsonStringProp([string]$obj, [string]$prop, [string]$newVal) {
  $pat = '"' + [regex]::Escape($prop) + '"\s*:\s*"(?:\\.|[^"\\])*"'
  if (-not [regex]::IsMatch($obj, $pat)) { return @{ Text = $obj; Ok = $false } }
  $rep = '"' + $prop + '":"' + (JsonEscape $newVal) + '"'
  $out = [regex]::Replace($obj, $pat, [System.Text.RegularExpressions.MatchEvaluator]{ param($m) $rep }, 1)
  return @{ Text = $out; Ok = $true }
}
function GetSection([string]$headingPrefix) {
  $m = [regex]::Match($script:Md, '(?m)^' + [regex]::Escape($headingPrefix) + '.*$')
  if (-not $m.Success) { return $null }
  $start = $m.Index
  $next = [regex]::Match($script:Md.Substring($start + $m.Length), '(?m)^## ')
  if ($next.Success) { return $script:Md.Substring($start, $m.Length + $next.Index) }
  return $script:Md.Substring($start)
}
function FindRelObject([string]$json, [string]$code) {
  $codePat = '"code"\s*:\s*"' + [regex]::Escape($code) + '"'
  $cm = [regex]::Match($json, $codePat)
  if (-not $cm.Success) { return $null }
  $start = $cm.Index
  while ($start -gt 0 -and $json[$start] -ne '{') { $start-- }
  $depth = 0; $end = -1
  for ($i = $start; $i -lt $json.Length; $i++) {
    if ($json[$i] -eq '{') { $depth++ }
    elseif ($json[$i] -eq '}') {
      $depth--
      if ($depth -eq 0) { $end = $i; break }
    }
  }
  if ($end -lt 0) { return $null }
  return @{ Start = $start; End = $end; Text = $json.Substring($start, $end - $start + 1) }
}

function ApplyIntros {
  foreach ($item in @(
      @{ md = '## 0.'; id = 'pres0-sec' },
      @{ md = '## 1а.'; id = 'streams-sec' },
      @{ md = '## 1б.'; id = 'pipe6-sec' }
    )) {
    $sec = GetSection $item.md
    if (-not $sec) { $script:Stats.skipped++; continue }
    $lines = $sec -split "`r?`n"
    $para = ''
    for ($i = 1; $i -lt $lines.Count; $i++) {
      if ($lines[$i] -match '^#{2,}') { break }
      $t = $lines[$i].Trim()
      if ($t -and $t -notmatch '^Источник:' -and $t -notmatch '^Интерактивная' -and $t -notmatch '^Пайплайн в HTML') {
        $para = $t; break
      }
    }
    if (-not $para) { $script:Stats.skipped++; continue }
    $pat = '(?is)(<section class="sec" id="' + [regex]::Escape($item.id) + '">\s*<div class="sec-head">\s*<h2>.*?</h2>\s*<p>)(.*?)(</p>)'
    $m = [regex]::Match($script:Doc, $pat)
    if (-not $m.Success) { Write-Warning "intro not found: $($item.id)"; $script:Stats.skipped++; continue }
    $new = EscText $para
    if ($m.Groups[2].Value -eq $new) { continue }
    $script:Doc = $script:Doc.Substring(0, $m.Groups[2].Index) + $new + $script:Doc.Substring($m.Groups[2].Index + $m.Groups[2].Length)
    $script:Stats.intros++
    Write-Host "intro updated: $($item.id)"
  }
}

function ApplyV12 {
  $sec = GetSection '## 1а.'
  if (-not $sec) { Write-Warning 'MD section 1а not found'; return }
  $typeRev = @{ 'запуск' = 'del'; 'тираж' = 'roll'; 'развитие' = 'dev'; 'веха' = 'std' }
  $vm = [regex]::Match($script:Doc, 'const V12_SPEC = (\{.*?\});\r?\nconst V12_EXTRA', [System.Text.RegularExpressions.RegexOptions]::Singleline)
  if (-not $vm.Success) { throw 'V12_SPEC not found in HTML' }
  $updated = $vm.Groups[1].Value

  foreach ($row in [regex]::Matches($sec, '(?m)^\|\s*`([^`]+)`\s*\|\s*([^|]+)\|\s*([^|]+)\|\s*([^|]+)\|\s*([^|]+)\|\s*([^|]*)\|')) {
    $code = $row.Groups[1].Value.Trim()
    $tyRu = $row.Groups[2].Value.Trim()
    $date = $row.Groups[3].Value.Trim()
    $title = $row.Groups[4].Value.Trim()
    $own = $row.Groups[5].Value.Trim()
    $ty = if ($typeRev.ContainsKey($tyRu)) { $typeRev[$tyRu] } else { $tyRu }

    $rel = FindRelObject $updated $code
    if (-not $rel) { Write-Warning "rel not in V12_SPEC: $code"; $script:Stats.skipped++; continue }
    $obj = $rel.Text
    $changed = $false
    foreach ($pair in @(
        @{ p = 't'; v = $title },
        @{ p = 'd'; v = $date },
        @{ p = 'own'; v = $own },
        @{ p = 'type'; v = $ty }
      )) {
      $r = ReplaceJsonStringProp $obj $pair.p $pair.v
      if ($r.Ok -and $r.Text -ne $obj) { $obj = $r.Text; $changed = $true }
    }
    if ($changed) {
      $updated = $updated.Substring(0, $rel.Start) + $obj + $updated.Substring($rel.End + 1)
      $script:Stats.v12++
      Write-Host "v12 rel updated: $code"
    }

    $met = [regex]::Match($sec, '(?ms)####\s*`' + [regex]::Escape($code) + '`[^\n]*\r?\n+(?:- [^\r\n]+\r?\n+)*?-\s*Критерий/метрика:\s*(.+?)(?=\r?\n\s*- |\r?\n\s*\r?\n|\r?\n####|\r?\n</details>)')
    if ($met.Success) {
      $metric = $met.Groups[1].Value.Trim()
      $rel2 = FindRelObject $updated $code
      if ($rel2) {
        $r = ReplaceJsonStringProp $rel2.Text 'metric' $metric
        if ($r.Ok -and $r.Text -ne $rel2.Text) {
          $updated = $updated.Substring(0, $rel2.Start) + $r.Text + $updated.Substring($rel2.End + 1)
          Write-Host "v12 metric updated: $code"
        }
      }
    }
  }

  if ($updated -ne $vm.Groups[1].Value) {
    $script:Doc = $script:Doc.Substring(0, $vm.Groups[1].Index) + $updated + $script:Doc.Substring($vm.Groups[1].Index + $vm.Groups[1].Length)
  }
}

function ApplyP0 {
  $sec0 = GetSection '## 0.'
  if (-not $sec0) { Write-Warning 'MD section 0 not found'; return }
  $streamMatches = [regex]::Matches($sec0, '(?ms)^####\s+(.+?)\s+\{(p0-[^}]+)\}\s*\r?\n(.*?)(?=^####\s|\z)')
  # process from end to start so indices stay valid
  for ($si = $streamMatches.Count - 1; $si -ge 0; $si--) {
    $sm = $streamMatches[$si]
    $id = $sm.Groups[2].Value.Trim()
    if ($StreamId -and $id -ne $StreamId) { continue }
    $bodyMd = $sm.Groups[3].Value
    $di = $script:Doc.IndexOf('id="' + $id + '"')
    if ($di -lt 0) { Write-Warning "p0 id not in HTML: $id"; $script:Stats.skipped++; continue }
    $ds = $script:Doc.LastIndexOf('<details', $di)
    $de = $script:Doc.IndexOf('</details>', $di)
    if ($ds -lt 0 -or $de -lt 0) { Write-Warning "details bounds fail: $id"; $script:Stats.skipped++; continue }
    $deEnd = $de + '</details>'.Length
    $block = $script:Doc.Substring($ds, $deEnd - $ds)
    $orig = $block

    foreach ($letter in @('а','б','в')) {
      $hm = [regex]::Match($bodyMd, '(?ms)^#####\s+' + $letter + '\)[^\r\n]*\r?\n+(.*?)(?=^#####\s|^######\s|\z)')
      if (-not $hm.Success) { continue }
      $contentHtml = MdBlockToHtml $hm.Groups[1].Value
      $boxPat = '(?is)(<div class="p0-box[^"]*"><h4>' + $letter + '\)[^<]*</h4>)(.*?)(</div>)'
      $bm = [regex]::Match($block, $boxPat)
      if (-not $bm.Success) { Write-Warning "$id box $letter not found"; continue }
      if ((NormText $bm.Groups[2].Value) -eq (NormText $hm.Groups[1].Value)) { continue }
      $block = $block.Substring(0, $bm.Groups[2].Index) + $contentHtml + $block.Substring($bm.Groups[2].Index + $bm.Groups[2].Length)
      Write-Host "p0 $id box $letter updated"
      $script:Stats.p0++
    }

    foreach ($letter in @('д','е')) {
      $hm = [regex]::Match($bodyMd, '(?ms)^#####\s+' + $letter + '\)[^\r\n]*\r?\n+(.*?)(?=^#####\s|^######\s|\z)')
      if (-not $hm.Success) { continue }
      $inner = MdBlockToHtml $hm.Groups[1].Value
      $boxPat = '(?is)(<h4 class="p0-h4[^"]*">' + $letter + '\)[^<]*</h4>\s*<div class="p0-box[^"]*">)(.*?)(</div>)'
      $bm = [regex]::Match($block, $boxPat)
      if (-not $bm.Success) { continue }
      if ((NormText $bm.Groups[2].Value) -eq (NormText $hm.Groups[1].Value)) { continue }
      $block = $block.Substring(0, $bm.Groups[2].Index) + $inner + $block.Substring($bm.Groups[2].Index + $bm.Groups[2].Length)
      Write-Host "p0 $id section $letter updated"
      $script:Stats.p0++
    }

    if ($block -ne $orig) {
      $script:Doc = $script:Doc.Substring(0, $ds) + $block + $script:Doc.Substring($deEnd)
    }
  }
}

Write-Host "HTML: $HtmlPath"
Write-Host "MD:   $MdPath"
Write-Host "What: $What  DryRun=$DryRun"

if ($What -eq 'intros' -or $What -eq 'all') { ApplyIntros }
if ($What -eq 'v12' -or $What -eq 'all') { ApplyV12 }
if ($What -eq 'p0' -or $What -eq 'all') { ApplyP0 }

Write-Host ("stats: intros={0} v12={1} p0={2} skipped={3}" -f $script:Stats.intros, $script:Stats.v12, $script:Stats.p0, $script:Stats.skipped)

if ($DryRun) {
  Write-Host 'DryRun: HTML not written'
  exit 0
}

if (-not $NoBackup) {
  $bak = Join-Path (Split-Path -Parent $HtmlPath) ('_backup_before_md_apply_' + (Get-Date -Format 'yyyyMMdd_HHmmss') + '.html')
  Copy-Item -LiteralPath $HtmlPath -Destination $bak -Force
  Write-Host "backup: $bak"
}

[IO.File]::WriteAllText($HtmlPath, $script:Doc, $enc)
Write-Host "OK apply -> $HtmlPath"

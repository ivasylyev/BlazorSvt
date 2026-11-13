$path = "d:\SVT\docs\docs\План-график Атлас\Логистика_—_эталонный_процесс_и_Атлас_план_график_v24_+_чат_v2_2.html"
$outPath = "d:\SVT\.cursor\skills\plan-grafik-md-html\scripts\_verify_metrics_out.txt"
$h = Get-Content -Raw -Encoding UTF8 -LiteralPath $path
$lines = New-Object System.Collections.Generic.List[string]

function GetObjAtCode([string]$json, [string]$code) {
  $pat = '"code"\s*:\s*"' + [regex]::Escape($code) + '"'
  $m = [regex]::Match($json, $pat)
  if (-not $m.Success) { return $null }
  $depth = 0; $start = -1
  for ($i = $m.Index; $i -ge 0; $i--) {
    $ch = $json[$i]
    if ($ch -eq [char]'}') { $depth++ }
    elseif ($ch -eq [char]'{') {
      if ($depth -eq 0) { $start = $i; break }
      $depth--
    }
  }
  if ($start -lt 0) { return $null }
  $depth = 0; $end = -1
  for ($i = $start; $i -lt $json.Length; $i++) {
    $ch = $json[$i]
    if ($ch -eq [char]'{') { $depth++ }
    elseif ($ch -eq [char]'}') {
      $depth--
      if ($depth -eq 0) { $end = $i; break }
    }
  }
  if ($end -lt 0) { return $null }
  return $json.Substring($start, $end - $start + 1)
}

foreach ($c in @('24.25','24.6','24.9','24.73','24.13')) {
  $obj = GetObjAtCode $h $c
  if (-not $obj) { [void]$lines.Add("$c : CODE NOT FOUND"); continue }
  $mm = [regex]::Match($obj, '"metric"\s*:\s*"((?:\\.|[^"\\])*)"')
  if ($mm.Success) {
    $t = $mm.Groups[1].Value -replace '\\n', ' | '
    if ($t.Length -gt 180) { $t = $t.Substring(0, 180) + '...' }
    [void]$lines.Add("$c : $t")
  } else {
    [void]$lines.Add("$c : NO METRIC")
  }
}

$obj = GetObjAtCode $h 'СВТ'
if ($obj) {
  $gm = [regex]::Match($obj, '"mtgoal"\s*:\s*"((?:\\.|[^"\\])*)"')
  if ($gm.Success) {
    $t = $gm.Groups[1].Value -replace '\\n', ' | '
    if ($t.Length -gt 220) { $t = $t.Substring(0, 220) + '...' }
    [void]$lines.Add("SVT mtgoal : $t")
  } else {
    [void]$lines.Add('SVT mtgoal : MISSING')
  }
} else {
  [void]$lines.Add('SVT stream : CODE NOT FOUND')
}

[System.IO.File]::WriteAllLines($outPath, $lines, [Text.UTF8Encoding]::new($false))
Write-Host "wrote $outPath"

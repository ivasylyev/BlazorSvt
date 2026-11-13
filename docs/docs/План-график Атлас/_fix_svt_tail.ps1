$p = 'd:\SVT\docs\docs\План-график Атлас\Логистика_—_эталонный_процесс_и_Атлас_план_график_v24_+_чат_v2_2.html'
$c = Get-Content -LiteralPath $p -Raw -Encoding UTF8

# Broken pattern after last СВТ rel
$bad = '"steps":{"12":{"b":["24.5"]}}},]},{"code":"ПРАВ"'
$good = '"steps":{"12":{"b":["24.5"]}}}]},{"code":"ПРАВ"'

if (-not $c.Contains($bad)) {
  Write-Host 'BAD pattern not found; searching nearby...'
  $i = $c.IndexOf('"code":"СВТ"')
  $j = $c.IndexOf('"code":"ПРАВ"', $i)
  Write-Host "context: $($c.Substring($j - 80, 120))"
  exit 1
}

$c2 = $c.Replace($bad, $good)
Set-Content -LiteralPath $p -Value $c2 -Encoding UTF8 -NoNewline
Write-Host 'fixed trailing braces'

# Verify
$c3 = Get-Content -LiteralPath $p -Raw -Encoding UTF8
$i = $c3.IndexOf('"code":"СВТ"')
$j = $c3.IndexOf('"code":"ПРАВ"', $i)
Write-Host "verify: $($c3.Substring($j - 90, 130))"

# Also ensure tasks 24.10-24.17 in LOG.added
if ($c3 -notmatch '"24\.10"') { Write-Host 'WARN: 24.10 not in LOG?' }
# patch LOG.added list
$oldLog = '"24.1","24.2","24.3","24.4","24.5"'
$newLog = '"24.1","24.2","24.3","24.4","24.5","24.10","24.11","24.12","24.13","24.14","24.15","24.16","24.17"'
if ($c3.Contains($oldLog) -and -not $c3.Contains('"24.10","24.11"')) {
  $c3 = $c3.Replace($oldLog, $newLog)
  Set-Content -LiteralPath $p -Value $c3 -Encoding UTF8 -NoNewline
  Write-Host 'LOG.added updated'
} else {
  Write-Host 'LOG.added check:' ($c3.Contains('"24.10","24.11"'))
}

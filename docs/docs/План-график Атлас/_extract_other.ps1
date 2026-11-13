$p = 'd:\SVT\docs\docs\План-график Атлас\Логистика_—_эталонный_процесс_и_Атлас_план_график_v24_+_чат_v2_2.html'
$c = Get-Content -LiteralPath $p -Raw -Encoding UTF8
# extract КОММ stream as a smaller comparable example
$needle = '"code":"КОММ"'
$i = $c.IndexOf($needle)
Write-Host "komm index=$i"
$start = $c.LastIndexOf('{', $i)
$depth = 0
$end = $start
for ($j = $start; $j -lt $c.Length; $j++) {
  $ch = $c[$j]
  if ($ch -eq '{') { $depth++ }
  elseif ($ch -eq '}') {
    $depth--
    if ($depth -eq 0) { $end = $j + 1; break }
  }
}
$chunk = $c.Substring($start, [Math]::Min(4500, $end - $start))
Set-Content -LiteralPath 'd:\SVT\docs\docs\План-график Атлас\_komm_v12_chunk.txt' -Value $chunk -Encoding UTF8
Write-Host "wrote $($chunk.Length)"

# Also find any old SVT phrasing in V12_SPEC area
$patterns = @(
  'границы с Atlas',
  'состав и границы',
  'живёт отдельно',
  'ставки для Ш',
  'на этапе проектирования',
  '12%',
  '28%',
  'П3'
)
foreach ($pat in $patterns) {
  $idx = 0
  $n = 0
  while (($idx = $c.IndexOf($pat, $idx)) -ge 0) {
    $n++
    if ($n -le 3) {
      $ctx = $c.Substring([Math]::Max(0,$idx-60), [Math]::Min(160, $c.Length-$idx+60))
      Write-Host "--- $pat #$n at $idx ---"
      Write-Host $ctx
    }
    $idx++
  }
  Write-Host "total $pat : $n"
}

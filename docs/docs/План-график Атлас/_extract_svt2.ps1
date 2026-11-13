$p = 'd:\SVT\docs\docs\План-график Атлас\Логистика_—_эталонный_процесс_и_Атлас_план_график_v24_+_чат_v2_2.html'
$c = Get-Content -LiteralPath $p -Raw -Encoding UTF8
$i = $c.IndexOf('"code":"СВТ"')
$start = $c.LastIndexOf(',{', $i) + 1
$after = $c.Substring($i + 10)
$m = [regex]::Match($after, '\},\{"code":"')
$end = $i + 10 + $m.Index + 1
$chunk = $c.Substring($start, $end - $start)
Set-Content -LiteralPath 'd:\SVT\docs\docs\План-график Атлас\_svt_v12_chunk.txt' -Value $chunk -Encoding UTF8
Write-Host "len=$($chunk.Length)"
# pretty-ish: show first 500 and search key phrases
Write-Host "--- head ---"
Write-Host $chunk.Substring(0, [Math]::Min(500, $chunk.Length))
Write-Host "--- markers ---"
@('excel','sla','24.10','24.15','ext','SVT реализован','СВТ реализован','Атлас продаж') | ForEach-Object {
  Write-Host "$_ : $($chunk.Contains($_))"
}

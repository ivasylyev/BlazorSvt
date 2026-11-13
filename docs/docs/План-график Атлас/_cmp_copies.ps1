$a = 'd:\SVT\docs\docs\План-график Атлас\Логистика_—_эталонный_процесс_и_Атлас_план_график_v24_+_чат_v2_2.html'
$b = 'C:\Users\Acer\Downloads\Telegram Desktop\Логистика_—_эталонный_процесс_и_Атлас_план_график_v24_+_чат_v2_2.html'
$ia = Get-Item -LiteralPath $a
$ib = Get-Item -LiteralPath $b
Write-Host "repo len=$($ia.Length) mtime=$($ia.LastWriteTime)"
Write-Host "tg   len=$($ib.Length) mtime=$($ib.LastWriteTime)"
$ha = (Get-FileHash -LiteralPath $a -Algorithm SHA256).Hash
$hb = (Get-FileHash -LiteralPath $b -Algorithm SHA256).Hash
Write-Host "same hash:" ($ha -eq $hb)
$cb = Get-Content -LiteralPath $b -Raw -Encoding UTF8
Write-Host "tg p0-edited:" ($cb.Contains('p0-edited'))
Write-Host "tg 24.10:" ($cb.Contains('N("24.10"'))
Write-Host "tg перечень:" ($cb.Contains('Сформирован перечень работ для создания СВТ 2.0'))
Write-Host "tg Claude stub:" ($cb.Contains('живёт отдельно'))
# V12 rows count in tg
$i = $cb.IndexOf('"code":"СВТ"')
Write-Host "tg V12 index:" $i
if ($i -ge 0) {
  $start = $cb.LastIndexOf(',{', $i) + 1
  $after = $cb.Substring($i + 10)
  $mm = [regex]::Match($after, '\},\{"code":"')
  $end = $i + 10 + $mm.Index + 1
  $obj = $cb.Substring($start, $end - $start)
  try {
    $j = $obj | ConvertFrom-Json
    Write-Host "tg JSON OK n=$($j.n) rows=$($j.rows.Count) mtdef=$($j.mtdef.Count)"
  } catch {
    Write-Host "tg JSON FAIL $($_.Exception.Message)"
  }
}

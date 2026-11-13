$p = 'd:\SVT\docs\docs\План-график Атлас\Логистика_—_эталонный_процесс_и_Атлас_план_график_v24_+_чат_v2_2.html'
$c = Get-Content -LiteralPath $p -Raw -Encoding UTF8

Write-Host "file bytes:" (Get-Item -LiteralPath $p).Length
Write-Host "p0-СВТ present:" ($c.Contains('id="p0-СВТ"'))
Write-Host "p0-edited present:" ($c.Contains('p0-СВТ" open'))
Write-Host "Claude stub:" ($c.Contains('живёт отдельно'))
Write-Host "old 12% metrics in p0-СВТ region:" 

# tasks 24.*
$m = [regex]::Matches($c, 'N\("24\.[^"]+"')
Write-Host "task count 24.*:" $m.Count
$m | ForEach-Object { Write-Host $_.Value }

# V12 СВТ object
$i = $c.IndexOf('"code":"СВТ"')
Write-Host "V12 СВТ index:" $i
if ($i -lt 0) { exit 1 }
$start = $c.LastIndexOf(',{', $i) + 1
$after = $c.Substring($i + 10)
$mm = [regex]::Match($after, '\},\{"code":"')
$end = $i + 10 + $mm.Index + 1
$obj = $c.Substring($start, $end - $start)
Set-Content -LiteralPath 'd:\SVT\docs\docs\План-график Атлас\_svt_v12_chunk.txt' -Value $obj -Encoding UTF8
Write-Host "chunk len:" $obj.Length
try {
  $j = $obj | ConvertFrom-Json
  Write-Host "JSON OK"
  Write-Host "n:" $j.n
  Write-Host "mtdef:" ($j.mtdef | ForEach-Object { $_.k }) -join ','
  Write-Host "rows:" $j.rows.Count
  foreach ($r in $j.rows) {
    Write-Host ("  " + $r.type + " " + $r.d + " " + $r.t.Substring(0, [Math]::Min(80, $r.t.Length)))
  }
} catch {
  Write-Host "JSON FAIL:" $_.Exception.Message
}

# P6 pipeline mention
Write-Host "P6 has СВТ:" ($c.Contains('P6_STREAMS = ["АВТО","АГЕНТ","ПРАВ","РЕШ","ИНФРА","КОММ","СВТ"]'))

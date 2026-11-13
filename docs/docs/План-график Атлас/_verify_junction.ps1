$p = 'd:\SVT\docs\docs\План-график Атлас\Логистика_—_эталонный_процесс_и_Атлас_план_график_v24_+_чат_v2_2.html'
$c = Get-Content -LiteralPath $p -Raw -Encoding UTF8
$i = $c.IndexOf('"code":"СВТ"')
$j = $c.IndexOf('"code":"ПРАВ"', $i)
Write-Host $c.Substring($j - 100, 140)

# Extract СВТ object and try JSON.parse via node if available, else powershell ConvertFrom-Json
$start = $c.LastIndexOf(',{', $i) + 1
$after = $c.Substring($i + 10)
$m = [regex]::Match($after, '\},\{"code":"')
$end = $i + 10 + $m.Index + 1
$obj = $c.Substring($start, $end - $start)
try {
  $null = $obj | ConvertFrom-Json
  Write-Host 'JSON parse: OK'
} catch {
  Write-Host "JSON parse FAIL: $($_.Exception.Message)"
  Write-Host "tail: $($obj.Substring($obj.Length-80))"
}

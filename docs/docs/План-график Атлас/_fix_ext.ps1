$p = 'd:\SVT\docs\docs\План-график Атлас\Логистика_—_эталонный_процесс_и_Атлас_план_график_v24_+_чат_v2_2.html'
$c = Get-Content -LiteralPath $p -Raw -Encoding UTF8

# Replace non-schema ext objects with empty arrays (intersections live in metric + task 24.12)
$a = '"ext":[{"t":"Атлас продаж, Атлас закупок, Атлас УЦП — потребители данных СВТ по API (состав запросов уточняется)","own":"Подоплелова Е."}]'
$b = '"ext":[{"t":"CRM — источник заявок as-is; продажи — база эффекта по сроку расчёта","own":"Подоплелова Е."}]'
$c2 = $c.Replace($a, '"ext":[]').Replace($b, '"ext":[]')
if ($c2 -eq $c) { Write-Host 'WARN: ext patterns not found' } else {
  Set-Content -LiteralPath $p -Value $c2 -Encoding UTF8 -NoNewline
  Write-Host 'ext cleared to []'
}

# Re-validate JSON
$c3 = Get-Content -LiteralPath $p -Raw -Encoding UTF8
$i = $c3.IndexOf('"code":"СВТ"')
$start = $c3.LastIndexOf(',{', $i) + 1
$after = $c3.Substring($i + 10)
$m = [regex]::Match($after, '\},\{"code":"')
$end = $i + 10 + $m.Index + 1
$obj = $c3.Substring($start, $end - $start)
try { $null = $obj | ConvertFrom-Json; Write-Host 'JSON OK' } catch { Write-Host "JSON FAIL $($_.Exception.Message)" }

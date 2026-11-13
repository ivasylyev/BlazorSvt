$p = 'd:\SVT\docs\docs\План-график Атлас\Логистика_—_эталонный_процесс_и_Атлас_план_график_v24_+_чат_v2_2.html'
$c = Get-Content -LiteralPath $p -Raw -Encoding UTF8
$needle = '"code":"СВТ"'
$i = $c.IndexOf($needle)
if ($i -lt 0) { throw 'СВТ stream not found' }
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

$new = '{"code":"СВТ","n":"Система ведения тарифов (СВТ 2.0)","lb":"Подоплелова Е.","li":"Васильев И.","g":"Смежные продукты","roster":[["biz","Подоплелова Е.","Владелец продукта СВТ"],["it","Васильев И.","Архитектор"]],"mtdef":[{"k":"proto","lab":"Прототип заявки на расчёт","start":"нет","suf":"","pre":"","agg":"last","cls":"u","tip":"Первый прототип — заявка на расчёт стоимости транспортировки."},{"k":"prod","lab":"Первый прототип в проде","start":"нет","suf":"","pre":"","agg":"last","cls":"r","tip":"Вывод в прод первого прототипа (заявка)."},{"k":"atlas","lab":"Атлас по СВТ","start":"0%","suf":"%","pre":"","agg":"last","cls":"s","tip":"Полнота Атласа по СВТ: роли и прототипы по обследованию процесса."}],"mtgoal":"Прототип заявки: {proto}; в проде: {prod}; Атлас по СВТ: {atlas}%","rels":[{"metric":"Первый прототип: заявка на расчёт стоимости транспортировки готов","avail":null,"mt":{"proto":"готов","prod":"нет","atlas":"0","what":"Первый прототип заявки"},"ext":[],"code":"24.1","type":"dev","t":"Первый прототип: заявка на расчёт стоимости транспортировки","own":"Подоплелова Е.","d":"12.26","ref":"24.1","anchor":null,"biz":null,"pre":[],"steps":{"3":{"b":["24.1"]},"5":{"b":["24.1"]}}},{"metric":"Первый прототип (заявка на расчёт стоимости транспортировки) выведен в прод","avail":"выкатка в прод","mt":{"proto":"готов","prod":"да","atlas":"пробел","what":"Прод первого прототипа"},"ext":[],"code":"24.2","type":"launch","t":"Вывод в прод первого прототипа — заявка на расчёт стоимости транспортировки","own":"Подоплелова Е.","d":"02.27","ref":"24.2","anchor":null,"biz":null,"pre":["24.1"],"steps":{"10":{"l":["24.2"]}}},{"metric":"Атлас по СВТ полностью: поэтапный вывод ролей и прототипов завершён","avail":null,"mt":{"proto":"готов","prod":"да","atlas":"100","what":"Атлас по СВТ полностью"},"ext":[],"code":"24.3","type":"roll","t":"Атлас по СВТ полностью","own":"Подоплелова Е.","d":"03.28","ref":"24.3","anchor":null,"biz":null,"pre":["24.2"],"steps":{"10":{"l":["24.3"]},"12":{"b":["24.3"]}}}]}'

$c2 = $c.Substring(0, $start) + $new + $c.Substring($end)
Set-Content -LiteralPath $p -Value $c2 -Encoding UTF8 -NoNewline
Write-Host 'patched V12_SPEC СВТ'
# validate
$c3 = Get-Content -LiteralPath $p -Raw -Encoding UTF8
$i3 = $c3.IndexOf($needle)
$start3 = $c3.LastIndexOf('{', $i3)
$depth = 0
$end3 = $start3
for ($j = $start3; $j -lt $c3.Length; $j++) {
  if ($c3[$j] -eq '{') { $depth++ }
  elseif ($c3[$j] -eq '}') { $depth--; if ($depth -eq 0) { $end3 = $j + 1; break } }
}
$obj = $c3.Substring($start3, $end3 - $start3)
try { $j = $obj | ConvertFrom-Json; Write-Host "JSON OK rels=$($j.rels.Count)" } catch { Write-Host "JSON FAIL $($_.Exception.Message)" }

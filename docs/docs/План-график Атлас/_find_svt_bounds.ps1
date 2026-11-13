$p = 'd:\SVT\docs\docs\План-график Атлас\Логистика_—_эталонный_процесс_и_Атлас_план_график_v24_+_чат_v2_2.html'
$c = Get-Content -LiteralPath $p -Raw -Encoding UTF8
$i = $c.IndexOf('"code":"СВТ"')
Write-Host "i=$i"
# find start: previous {"code" or streams array
$start = $c.LastIndexOf(',{', $i)
# if СВТ is not first, start is after comma; object starts at start+1
$objStart = $start + 1
Write-Host "objStart=$objStart contextBefore=$($c.Substring($start, 30))"

# Find next stream code after СВТ - search for "},{""code":" pattern after i
$after = $c.Substring($i + 10)
$m = [regex]::Match($after, '\},\{"code":"')
if (-not $m.Success) {
  # maybe last stream: ends with }]}
  $m2 = [regex]::Match($after, '\}\]\s*;')
  Write-Host "last stream? $($m2.Success) at $($m2.Index)"
  if ($m2.Success) {
    $objEnd = $i + 10 + $m2.Index + 1 # include closing }
    Write-Host "objEnd=$objEnd"
    Write-Host "ENDCTX $($c.Substring($objEnd-40, 80))"
  }
} else {
  $objEnd = $i + 10 + $m.Index + 1 # closing } of СВТ object
  Write-Host "next stream at relative $($m.Index); objEnd=$objEnd"
  Write-Host "ENDCTX $($c.Substring($objEnd-60, 120))"
  Write-Host "NEXT $($c.Substring($objEnd, 80))"
}

# Also check if my broken patch already ran
Write-Host "broken braces in mtgoal count: $([regex]::Matches($c.Substring([Math]::Max(0,$i-5), 500), '\{apps\}').Count)"
Write-Host "has excel in V12 near SVT: $($c.IndexOf('Доля/время работы в Excel', $i))"

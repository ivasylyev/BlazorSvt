$md = [IO.File]::ReadAllText('d:\SVT\docs\docs\План-график Атлас\разделы_0_1а_1б.md')
$sec = [regex]::Match($md, '(?ms)^## 1а\..*?(?=^## 1б\.)').Value
Write-Host "sec len $($sec.Length)"
# single-quoted: backtick is literal
$pat = '(?m)^\|\s*`([^`]+)`\s*\|\s*([^|]+)\|\s*([^|]+)\|\s*([^|]+)\|\s*([^|]+)\|\s*([^|]*)\|'
$rows = [regex]::Matches($sec, $pat)
Write-Host "full rows $($rows.Count)"
$pat2 = '(?m)^\|\s*`([^`]+)`\s*\|'
$rows2 = [regex]::Matches($sec, $pat2)
Write-Host "code rows $($rows2.Count)"
if ($rows2.Count -gt 0) { Write-Host $rows2[0].Value }
# show a sample table line
$sample = [regex]::Match($sec, '(?m)^\| `24\.').Value
Write-Host "sample: [$sample]"
$line = ($sec -split "`n" | Where-Object { $_ -match '^\| `24\.' } | Select-Object -First 1)
Write-Host "line: [$line]"

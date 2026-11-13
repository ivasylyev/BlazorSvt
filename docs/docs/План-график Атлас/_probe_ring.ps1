$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$htmlPath = (Get-ChildItem -LiteralPath $root -Filter 'Логистика*v2_2.html' | Select-Object -First 1).FullName
$t = [IO.File]::ReadAllText($htmlPath, [Text.UTF8Encoding]::new($false))
$idx = $t.IndexOf('p0-ring12')
Write-Host "p0-ring12 idx:" $idx
Write-Host $t.Substring([Math]::Max(0,$idx-80), 200)
$ring = [regex]::Match($t, '(?is)<div class="p0-ring">(.*?)</div>\s*<h3 class="p0-h3">Стримы</h3>')
Write-Host "ring success:" $ring.Success "len:" $ring.Groups[1].Value.Length
Write-Host "contains ring12:" ($ring.Groups[1].Value.Contains('p0-ring12'))
$r12 = [regex]::Match($ring.Groups[1].Value, '(?is)<div class="p0-steps p0-ring12">(.*?)</div>')
Write-Host "r12 success:" $r12.Success "len:" $r12.Groups[1].Value.Length
if ($r12.Success) { Write-Host $r12.Groups[1].Value.Substring(0, [Math]::Min(200, $r12.Groups[1].Value.Length)) }

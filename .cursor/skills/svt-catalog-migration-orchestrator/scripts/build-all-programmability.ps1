#Requires -Version 5.1
<#
.SYNOPSIS
    Собирает Migrations/{Version}/NNN.All_Programmability.sql из SoT Programmability.

.DESCRIPTION
    Порядок файлов совпадает с Create-Programmability.ps1: Platform, затем модули по имени.
    Пишет UTF-8 с BOM и переводами строк CRLF. Не использует CR CR LF.
#>
param(
    [Parameter(Mandatory = $true)]
    [string]$RepoRoot,

    [Parameter(Mandatory = $true)]
    [string]$VersionPrefix
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Get-ProgrammabilitySortKey([string]$FileName) {
    switch -Wildcard ($FileName) {
        'fn_*'             { return '010' }
        'sp_*'             { return '020' }
        'vw_*'             { return '010' }
        '*_Get.sql'        { return '020' }
        '*_ExportFull.sql' { return '030' }
        default            { return '100' }
    }
}

function Read-SqlText([string]$Path) {
    $text = [System.IO.File]::ReadAllText($Path).TrimStart([char]0xFEFF)
    $text = $text -replace "`r`n", "`n" -replace "`r", "`n"
    return ($text -replace "`n", "`r`n")
}

$sqlRoot = Join-Path $RepoRoot 'BlazorSvt\SqlScripts'
$migrationDir = Join-Path $sqlRoot "Migrations\$VersionPrefix"
if (-not (Test-Path -LiteralPath $migrationDir)) {
    throw "Migration folder not found: $migrationDir"
}

$items = New-Object System.Collections.Generic.List[object]

Get-ChildItem -LiteralPath (Join-Path $sqlRoot 'Platform\Programmability') -Filter '*.sql' -File |
    Sort-Object { Get-ProgrammabilitySortKey $_.Name }, Name |
    ForEach-Object {
        $items.Add([PSCustomObject]@{
            Group  = 'Platform'
            Source = "Platform\Programmability\$($_.Name)"
            Name   = $_.Name
            Path   = $_.FullName
        })
    }

Get-ChildItem -LiteralPath (Join-Path $sqlRoot 'Modules') -Directory | Sort-Object Name | ForEach-Object {
    $dir = Join-Path $_.FullName 'Programmability'
    if (-not (Test-Path -LiteralPath $dir)) { return }
    $moduleName = $_.Name
    Get-ChildItem -LiteralPath $dir -Filter '*.sql' -File |
        Sort-Object { Get-ProgrammabilitySortKey $_.Name }, Name |
        ForEach-Object {
            $items.Add([PSCustomObject]@{
                Group  = $moduleName
                Source = "Modules\$moduleName\Programmability\$($_.Name)"
                Name   = $_.Name
                Path   = $_.FullName
            })
        }
}

if ($items.Count -eq 0) {
    throw "No Programmability scripts under $sqlRoot"
}

$existing = Get-ChildItem -LiteralPath $migrationDir -Filter '*All_Programmability.sql' -File |
    Sort-Object Name -Descending |
    Select-Object -First 1
$max = 0
Get-ChildItem -LiteralPath $migrationDir -Filter '*.sql' -File |
    Where-Object { $_.Name -notlike '*All_Programmability.sql' } |
    ForEach-Object {
        if ($_.Name -match '^(\d+)') {
            $n = [int]$Matches[1]
            if ($n -gt $max) { $max = $n }
        }
    }
$next = '{0:D3}' -f ($max + 1)
$outPath = Join-Path $migrationDir "$next.All_Programmability.sql"

# Снимок должен остаться последним файлом папки: после добавления дельты его переносим в конец.
if ($existing -and $existing.FullName -ne $outPath) {
    $moved = $false
    Push-Location $RepoRoot
    try {
        git mv -- $existing.FullName $outPath 2>&1 | Out-Null
        $moved = ($LASTEXITCODE -eq 0)
    }
    catch { $moved = $false }
    finally { Pop-Location }
    if (-not $moved) { Move-Item -LiteralPath $existing.FullName -Destination $outPath }
    Write-Host "Renumbered $($existing.Name) -> $(Split-Path $outPath -Leaf)"
}

$n = $items.Count
$generated = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
$sb = New-Object System.Text.StringBuilder
[void]$sb.Append("/*`r`n")
[void]$sb.Append("  Auto-generated: All v2 Programmability`r`n")
[void]$sb.Append("  Generated: $generated`r`n")
[void]$sb.Append("  Scripts: $n`r`n")
[void]$sb.Append("  Order: Create-Programmability.ps1 (Platform -> Modules by name)`r`n")
[void]$sb.Append("*/`r`n")
[void]$sb.Append("GO`r`n")
[void]$sb.Append("`r`n")

for ($i = 0; $i -lt $n; $i++) {
    $item = $items[$i]
    $body = Read-SqlText $item.Path
    if (-not $body.EndsWith("`r`n")) { $body += "`r`n" }
    $num = $i + 1
    [void]$sb.Append("--------------------------------------------------------------------------------`r`n")
    [void]$sb.Append("-- [$num/$n] $($item.Group) :: $($item.Name)`r`n")
    [void]$sb.Append("-- Source: $($item.Source)`r`n")
    [void]$sb.Append("--------------------------------------------------------------------------------`r`n")
    [void]$sb.Append("GO`r`n")
    [void]$sb.Append($body)
    if ($i -lt ($n - 1) -and -not $body.EndsWith("`r`n`r`n")) {
        [void]$sb.Append("`r`n")
    }
}

$utf8Bom = New-Object System.Text.UTF8Encoding $true
[System.IO.File]::WriteAllText($outPath, $sb.ToString(), $utf8Bom)

$bytes = [System.IO.File]::ReadAllBytes($outPath)
$crcrlf = 0
for ($i = 0; $i -lt ($bytes.Length - 2); $i++) {
    if ($bytes[$i] -eq 13 -and $bytes[$i + 1] -eq 13 -and $bytes[$i + 2] -eq 10) { $crcrlf++ }
}
if ($crcrlf -gt 0) { throw "CR CR LF found in $outPath ($crcrlf)" }
if ($sb.ToString() -match '(?m)^SE ') { throw "Broken USE (line starts with SE) in $outPath" }

Write-Host "Wrote $outPath"
Write-Host "Scripts: $n"

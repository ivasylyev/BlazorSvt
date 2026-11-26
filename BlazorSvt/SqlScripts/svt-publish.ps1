#Requires -Version 5.1
<#
.SYNOPSIS
    Собирает пакет версии BlazorSVT в C:\publish\{версия}.

.DESCRIPTION
    Версия — VersionPrefix из BlazorSvt.csproj.
    Режимы: app (только приложение), sql (только склейка миграции),
    all (оба, по умолчанию), local (all и sqlcmd на localhost / mdm).
    Если VersionPrefix не совпадает с максимальной папкой Migrations,
    all и local не запускаются. app и sql — только с -AllowMismatch.
    Живые папка и zip заменяются после полной сборки. Ошибка sqlcmd их не трогает.

.EXAMPLE
    .\svt-publish.ps1

.EXAMPLE
    .\svt-publish.ps1 -Mode local

.EXAMPLE
    .\svt-publish.ps1 -Mode app -AllowMismatch
#>
[CmdletBinding()]
param(
    [ValidateSet('app', 'sql', 'all', 'local')]
    [string]$Mode = 'all',

    [switch]$AllowMismatch
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$publishRoot = 'C:\publish'
$appOutput = Join-Path $publishRoot 'Blazor'
$scriptRoot = $PSScriptRoot
$repoRoot = (Resolve-Path (Join-Path $scriptRoot '..\..')).Path
$csproj = (Resolve-Path (Join-Path $scriptRoot '..\BlazorSvt.csproj')).Path
$migrationsRoot = Join-Path $scriptRoot 'Migrations'
$appSettingsPath = Join-Path $scriptRoot '..\appsettings.json'

function Get-VersionPrefix {
    param([string]$CsprojPath)

    if (-not (Test-Path -LiteralPath $CsprojPath)) {
        throw "Не найден файл проекта: $CsprojPath"
    }

    $text = [System.IO.File]::ReadAllText($CsprojPath)
    $match = [regex]::Match($text, '<VersionPrefix>\s*([^<]+?)\s*</VersionPrefix>')
    if (-not $match.Success) {
        throw "В $CsprojPath нет VersionPrefix."
    }

    $prefix = $match.Groups[1].Value
    if ($prefix -notmatch '^\d+\.\d+\.\d+$') {
        throw "VersionPrefix '$prefix' не в виде x.y.z."
    }

    return $prefix
}

function Get-MaxMigration {
    param([string]$Root)

    if (-not (Test-Path -LiteralPath $Root)) {
        throw "Нет папки миграций: $Root"
    }

    $best = $null
    foreach ($dir in (Get-ChildItem -LiteralPath $Root -Directory)) {
        if ($dir.Name -notmatch '^\d+\.\d+\.\d+$') {
            continue
        }

        $version = [version]$dir.Name
        if ($null -eq $best -or $version -gt $best.Version) {
            $best = [PSCustomObject]@{
                Name    = $dir.Name
                Version = $version
                Path    = $dir.FullName
            }
        }
    }

    if ($null -eq $best) {
        throw "В $Root нет папки версии x.y.z."
    }

    return $best
}

function Get-MismatchMessage {
    param([string]$AppVersion, [string]$ScriptVersion)

    @"
Версия приложения $AppVersion не совпадает с последней папкой скриптов миграции ($ScriptVersion). Одним пакетом их публиковать нельзя.

Что сделать?

1. Отменить запуск
2. Опубликовать только приложение $AppVersion
3. Опубликовать только скрипты $ScriptVersion
"@
}

function Get-SqlCmdPath {
    $cmd = Get-Command sqlcmd -ErrorAction SilentlyContinue
    if ($cmd) {
        return $cmd.Source
    }

    $candidates = @(
        "$env:ProgramFiles\Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn\SQLCMD.EXE",
        "${env:ProgramFiles(x86)}\Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn\SQLCMD.EXE",
        "$env:ProgramFiles\Microsoft SQL Server\160\Tools\Binn\SQLCMD.EXE",
        "${env:ProgramFiles(x86)}\Microsoft SQL Server\160\Tools\Binn\SQLCMD.EXE",
        "$env:ProgramFiles\Microsoft SQL Server\150\Tools\Binn\SQLCMD.EXE",
        "${env:ProgramFiles(x86)}\Microsoft SQL Server\150\Tools\Binn\SQLCMD.EXE"
    )

    foreach ($path in $candidates) {
        if (Test-Path -LiteralPath $path) {
            return $path
        }
    }

    throw 'sqlcmd не найден. Установите SQL Server Command Line Utilities или добавьте sqlcmd в PATH.'
}

function Get-LocalSqlCredential {
    param([string]$SettingsPath)

    if (-not (Test-Path -LiteralPath $SettingsPath)) {
        throw "Не найден файл настроек: $SettingsPath"
    }

    $raw = Get-Content -LiteralPath $SettingsPath -Raw
    $raw = $raw -replace '(?m)^\s*//.*$', ''
    $raw = $raw -replace '/\*[\s\S]*?\*/', ''
    $settings = $raw | ConvertFrom-Json
    $connectionString = $settings.Database.MdmDb
    if ([string]::IsNullOrWhiteSpace($connectionString)) {
        throw "В $SettingsPath нет Database:MdmDb."
    }

    Add-Type -AssemblyName System.Data
    $builder = New-Object System.Data.SqlClient.SqlConnectionStringBuilder $connectionString
    $integrated = [bool]$builder.IntegratedSecurity
    if (-not $integrated -and [string]::IsNullOrWhiteSpace($builder.UserID)) {
        throw 'В Database:MdmDb нет User ID и нет Trusted_Connection. appsettings.json не менять.'
    }

    return [PSCustomObject]@{
        Server     = 'localhost'
        Database   = 'mdm'
        Integrated = $integrated
        User       = $builder.UserID
        Password   = $builder.Password
    }
}

function Get-PublishSqlName {
    param([string]$FileName)
    return $FileName.Replace("'", "''")
}

function Get-PublishStartBatch {
    param([string]$FileName)

    $name = Get-PublishSqlName -FileName $FileName
    $lines = @(
        "PRINT 'Starting $name';"
        'DECLARE @svt_publish_mark datetime2(7) = SYSDATETIME();'
        'EXEC sys.sp_set_session_context N''svt_publish_started'', @svt_publish_mark;'
    )
    return ($lines -join "`r`n")
}

function Get-PublishFinishBatch {
    param([string]$FileName)

    $name = Get-PublishSqlName -FileName $FileName
    $lines = @(
        'DECLARE @svt_publish_finished datetime2(7) = SYSDATETIME();'
        'DECLARE @svt_publish_started datetime2(7) = CAST(SESSION_CONTEXT(N''svt_publish_started'') AS datetime2(7));'
        'DECLARE @svt_publish_ms bigint = DATEDIFF_BIG(MILLISECOND, @svt_publish_started, @svt_publish_finished);'
        'DECLARE @svt_publish_min bigint = @svt_publish_ms / 60000;'
        'DECLARE @svt_publish_sec int = (@svt_publish_ms % 60000) / 1000;'
        'DECLARE @svt_publish_fff int = @svt_publish_ms % 1000;'
        "PRINT 'Finished $name in ' + CONVERT(varchar(20), @svt_publish_min) + ':' + RIGHT('0' + CONVERT(varchar(2), @svt_publish_sec), 2) + '.' + RIGHT('000' + CONVERT(varchar(3), @svt_publish_fff), 3);"
    )
    return ($lines -join "`r`n")
}

function Get-PublishJunctionBatch {
    param([string]$PreviousName, [string]$NextName)

    $finish = Get-PublishFinishBatch -FileName $PreviousName
    $start = Get-PublishStartBatch -FileName $NextName
    return $finish + "`r`n" + $start
}

function Read-PublishSqlFile {
    param([string]$Path)

    $text = [System.IO.File]::ReadAllText($Path)
    $text = $text.TrimStart([char]0xFEFF)
    $text = $text -replace "`r`n", "`n" -replace "`r", "`n"
    $text = $text.TrimEnd("`n")
    return ($text -replace "`n", "`r`n")
}

function Write-ConcatenatedSql {
    param([string]$SourceDir, [string]$DestinationPath)

    $files = @(Get-ChildItem -LiteralPath $SourceDir -Filter '*.sql' -File | Sort-Object Name)
    if ($files.Count -eq 0) {
        throw "В $SourceDir нет файлов *.sql."
    }

    $parts = New-Object System.Collections.Generic.List[string]
    for ($i = 0; $i -lt $files.Count; $i++) {
        $file = $files[$i]
        if ($i -eq 0) {
            $parts.Add((Get-PublishStartBatch -FileName $file.Name))
        }

        $parts.Add((Read-PublishSqlFile -Path $file.FullName))

        if ($i -lt ($files.Count - 1)) {
            $parts.Add((Get-PublishJunctionBatch -PreviousName $file.Name -NextName $files[$i + 1].Name))
        }
        else {
            $parts.Add((Get-PublishFinishBatch -FileName $file.Name))
        }
    }

    $body = ($parts -join "`r`nGO`r`n") + "`r`n"
    $encoding = New-Object System.Text.UTF8Encoding $true
    [System.IO.File]::WriteAllText($DestinationPath, $body, $encoding)
    return $files.Count
}

function Copy-AppOutput {
    param([string]$SourceDir, [string]$DestinationDir)

    if (-not (Test-Path -LiteralPath $SourceDir)) {
        throw "Нет каталога публикации приложения: $SourceDir"
    }

    $entries = @(Get-ChildItem -LiteralPath $SourceDir -Force)
    if ($entries.Count -eq 0) {
        throw "Каталог публикации приложения пуст: $SourceDir"
    }

    New-Item -ItemType Directory -Path $DestinationDir -Force | Out-Null
    Copy-Item -Path (Join-Path $SourceDir '*') -Destination $DestinationDir -Recurse -Force
}

function Install-VersionPackage {
    param(
        [string]$StagedDir,
        [string]$LiveDir,
        [string]$StagedZip,
        [string]$LiveZip
    )

    $ErrorActionPreference = 'Stop'
    $parent = Split-Path -Parent $LiveDir
    $bakDir = Join-Path $parent ((Split-Path -Leaf $LiveDir) + '.bak')
    $bakZip = "$LiveZip.bak"

    if ((Test-Path -LiteralPath $bakDir) -or (Test-Path -LiteralPath $bakZip)) {
        throw "Незавершённый обмен пакета ($bakDir). Живые папка и zip не изменены."
    }

    $dirAside = $false
    $dirInstalled = $false
    $zipAside = $false
    $zipInstalled = $false

    try {
        if (Test-Path -LiteralPath $LiveDir) {
            Rename-Item -LiteralPath $LiveDir -NewName (Split-Path -Leaf $bakDir)
            $dirAside = $true
        }

        Move-Item -LiteralPath $StagedDir -Destination $LiveDir
        $dirInstalled = $true

        if (Test-Path -LiteralPath $LiveZip) {
            Rename-Item -LiteralPath $LiveZip -NewName (Split-Path -Leaf $bakZip)
            $zipAside = $true
        }

        Move-Item -LiteralPath $StagedZip -Destination $LiveZip
        $zipInstalled = $true

        if ($dirAside) {
            Remove-Item -LiteralPath $bakDir -Recurse -Force
        }

        if ($zipAside) {
            Remove-Item -LiteralPath $bakZip -Force
        }
    }
    catch {
        if ($zipInstalled -and (Test-Path -LiteralPath $LiveZip)) {
            Remove-Item -LiteralPath $LiveZip -Force -ErrorAction SilentlyContinue
        }

        if ($zipAside -and (Test-Path -LiteralPath $bakZip) -and -not (Test-Path -LiteralPath $LiveZip)) {
            Rename-Item -LiteralPath $bakZip -NewName (Split-Path -Leaf $LiveZip)
        }

        if ($dirInstalled -and (Test-Path -LiteralPath $LiveDir)) {
            Remove-Item -LiteralPath $LiveDir -Recurse -Force -ErrorAction SilentlyContinue
        }

        if ($dirAside -and (Test-Path -LiteralPath $bakDir) -and -not (Test-Path -LiteralPath $LiveDir)) {
            Rename-Item -LiteralPath $bakDir -NewName (Split-Path -Leaf $LiveDir)
        }

        throw
    }
}

$prefix = Get-VersionPrefix -CsprojPath $csproj
$max = Get-MaxMigration -Root $migrationsRoot
$needsApp = $Mode -in @('app', 'all', 'local')
$needsSql = $Mode -in @('sql', 'all', 'local')
$needsLocal = $Mode -eq 'local'

if (([version]$prefix) -ne $max.Version) {
    if ($Mode -in @('all', 'local') -or -not $AllowMismatch) {
        Write-Output (Get-MismatchMessage -AppVersion $prefix -ScriptVersion $max.Name)
        exit 2
    }
}

$packageVersion = if ($Mode -eq 'sql') { $max.Name } else { $prefix }
$sqlDir = $max.Path
if ($needsSql -and -not (Test-Path -LiteralPath $sqlDir)) {
    throw "Нет папки скриптов: $sqlDir"
}

if ($needsApp) {
    Write-Host "dotnet publish -> $appOutput"
    & dotnet publish $csproj -c Release -o $appOutput --nologo
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet publish завершился с кодом $LASTEXITCODE. Папка пакета и zip не изменены."
    }
}

New-Item -ItemType Directory -Path $publishRoot -Force | Out-Null
$stagingRoot = Join-Path $publishRoot ('_svt-publish-staging\' + [guid]::NewGuid().ToString('n'))
$stagedDir = Join-Path $stagingRoot $packageVersion
$stagedZip = Join-Path $stagingRoot ($packageVersion + '.zip')
$liveDir = Join-Path $publishRoot $packageVersion
$liveZip = Join-Path $publishRoot ($packageVersion + '.zip')
$sqlCount = 0
$installed = $false

try {
    New-Item -ItemType Directory -Path $stagedDir -Force | Out-Null

    if ($needsApp) {
        Copy-AppOutput -SourceDir $appOutput -DestinationDir (Join-Path $stagedDir 'v2')
    }

    if ($needsSql) {
        $sqlCount = Write-ConcatenatedSql -SourceDir $sqlDir -DestinationPath (Join-Path $stagedDir ($packageVersion + '.sql'))
    }

    if ($needsLocal) {
        $sqlCmd = Get-SqlCmdPath
        $connection = Get-LocalSqlCredential -SettingsPath $appSettingsPath
        $sqlFile = Join-Path $stagedDir ($packageVersion + '.sql')
        if ($connection.Integrated) {
            Write-Host "sqlcmd $($connection.Server) / $($connection.Database) / Windows"
            & $sqlCmd -S $connection.Server -d $connection.Database -E -b -f 65001 -r 1 -i $sqlFile
        }
        else {
            Write-Host "sqlcmd $($connection.Server) / $($connection.Database) / $($connection.User)"
            & $sqlCmd -S $connection.Server -d $connection.Database -U $connection.User -P $connection.Password -b -f 65001 -r 1 -i $sqlFile
        }
        if ($LASTEXITCODE -ne 0) {
            throw "sqlcmd завершился с кодом $LASTEXITCODE. Папка пакета и zip не изменены."
        }
    }

    if (Test-Path -LiteralPath $stagedZip) {
        Remove-Item -LiteralPath $stagedZip -Force
    }

    Compress-Archive -Path $stagedDir -DestinationPath $stagedZip -CompressionLevel Optimal
    Install-VersionPackage -StagedDir $stagedDir -LiveDir $liveDir -StagedZip $stagedZip -LiveZip $liveZip
    $installed = $true
}
finally {
    if (Test-Path -LiteralPath $stagingRoot) {
        Remove-Item -LiteralPath $stagingRoot -Recurse -Force -ErrorAction SilentlyContinue
    }
}

Write-Host "Mode:     $Mode"
Write-Host "Version:  $packageVersion"
Write-Host "Package:  $liveDir"
Write-Host "Zip:      $liveZip"
if ($needsSql) {
    Write-Host "SqlFiles: $sqlCount"
}
if ($needsLocal) {
    Write-Host 'LocalApply: yes'
}

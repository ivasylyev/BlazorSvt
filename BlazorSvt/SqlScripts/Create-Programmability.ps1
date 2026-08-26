#Requires -Version 5.1
<#
.SYNOPSIS
    Deploys Programmability SQL scripts (Platform + all modules) to dev database.

.DESCRIPTION
    Reads connection string from BlazorSvt/appsettings.json.
    Stops on the first failed script (sqlcmd -b).

.EXAMPLE
    .\Create-Programmability.ps1
#>
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

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

    throw "sqlcmd not found. Install SQL Server Command Line Utilities or add sqlcmd to PATH."
}

function Get-ConnectionSettings {
    param([string]$AppSettingsPath)

    if (-not (Test-Path -LiteralPath $AppSettingsPath)) {
        throw "App settings file not found: $AppSettingsPath"
    }

    $settings = Get-Content -LiteralPath $AppSettingsPath -Raw | ConvertFrom-Json
    $connectionString = $settings.Database.MdmDb

    if ([string]::IsNullOrWhiteSpace($connectionString)) {
        throw "Database:MdmDb is missing in $AppSettingsPath"
    }

    Add-Type -AssemblyName System.Data
    $builder = New-Object System.Data.SqlClient.SqlConnectionStringBuilder $connectionString

    if ([string]::IsNullOrWhiteSpace($builder.DataSource)) {
        throw "Connection string is missing Server/Data Source"
    }

    if ([string]::IsNullOrWhiteSpace($builder.InitialCatalog)) {
        throw "Connection string is missing Database/Initial Catalog"
    }

    if ([string]::IsNullOrWhiteSpace($builder.UserID)) {
        throw "Connection string is missing User ID (SQL authentication required)"
    }

    return [PSCustomObject]@{
        Server   = $builder.DataSource
        Database = $builder.InitialCatalog
        User     = $builder.UserID
        Password = $builder.Password
    }
}

function Get-ProgrammabilitySortKey {
    param([string]$FileName)

    switch -Wildcard ($FileName) {
        'fn_*'             { return '010' }
        'sp_*'             { return '020' }
        'vw_*'             { return '010' }
        '*_Get.sql'        { return '020' }
        '*_ExportFull.sql' { return '030' }
        default            { return '100' }
    }
}

function Get-ProgrammabilityScripts {
    param([string]$DirectoryPath)

    if (-not (Test-Path -LiteralPath $DirectoryPath)) {
        return @()
    }

    return Get-ChildItem -LiteralPath $DirectoryPath -Filter '*.sql' -File |
        Sort-Object { Get-ProgrammabilitySortKey $_.Name }, Name |
        Select-Object -ExpandProperty FullName
}

function Invoke-SqlScript {
    param(
        [string]$SqlCmdPath,
        [PSCustomObject]$Connection,
        [string]$ScriptPath
    )

    $relativePath = Resolve-Path -LiteralPath $ScriptPath
    Write-Host "==> $relativePath" -ForegroundColor Cyan

    & $SqlCmdPath `
        -S $Connection.Server `
        -d $Connection.Database `
        -U $Connection.User `
        -P $Connection.Password `
        -i $ScriptPath `
        -b `
        -r 1

    if ($LASTEXITCODE -ne 0) {
        throw "Script failed with exit code $LASTEXITCODE : $ScriptPath"
    }

    Write-Host "OK" -ForegroundColor Green
}

$scriptRoot = $PSScriptRoot
$appSettingsPath = Join-Path $scriptRoot '..\appsettings.json' | Resolve-Path
$sqlCmdPath = Get-SqlCmdPath
$connection = Get-ConnectionSettings -AppSettingsPath $appSettingsPath

Write-Host "Server:   $($connection.Server)"
Write-Host "Database: $($connection.Database)"
Write-Host "User:     $($connection.User)"
Write-Host "sqlcmd:   $sqlCmdPath"
Write-Host ''

$scriptGroups = @(
    @{
        Name    = 'Platform'
        Scripts = @(Get-ProgrammabilityScripts (Join-Path $scriptRoot 'Platform\Programmability'))
    }
)

$moduleRoots = Get-ChildItem -LiteralPath (Join-Path $scriptRoot 'Modules') -Directory |
    Sort-Object Name

foreach ($module in $moduleRoots) {
    $programmabilityDir = Join-Path $module.FullName 'Programmability'
    $scripts = @(Get-ProgrammabilityScripts $programmabilityDir)

    if ($scripts.Count -gt 0) {
        $scriptGroups += @{
            Name    = "Modules\$($module.Name)"
            Scripts = $scripts
        }
    }
}

$totalScripts = ($scriptGroups | ForEach-Object { $_.Scripts.Count } | Measure-Object -Sum).Sum
if ($totalScripts -eq 0) {
    throw "No Programmability SQL scripts found under $scriptRoot"
}

Write-Host "Scripts to execute: $totalScripts"
Write-Host ''

$executed = 0
foreach ($group in $scriptGroups) {
    if ($group.Scripts.Count -eq 0) {
        continue
    }

    Write-Host "--- $($group.Name) ---" -ForegroundColor Yellow

    foreach ($script in $group.Scripts) {
        Invoke-SqlScript -SqlCmdPath $sqlCmdPath -Connection $connection -ScriptPath $script
        $executed++
    }

    Write-Host ''
}

Write-Host "Done. Successfully executed scripts: $executed" -ForegroundColor Green

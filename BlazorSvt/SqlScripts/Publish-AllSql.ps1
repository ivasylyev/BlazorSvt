#Requires -Version 5.1
<#
.SYNOPSIS
    Publishes SQL deploy artifacts from Migrations/ and/or SoT sources.

.DESCRIPTION
    Modes:
      All             - copy every Migrations/{version}/ into TargetPath\{version}\
      Release         - copy one Migrations/{Version}/ (requires -Version)
      Programmability - flatten latest CREATE OR ALTER scripts from SoT into TargetPath\programmability\
      FromSource      - legacy: flatten Structure+Programmability from SoT (greenfield snapshot)

    Does not modify source scripts in the repository.

.EXAMPLE
    .\Publish-AllSql.ps1 -Mode All

.EXAMPLE
    .\Publish-AllSql.ps1 -Mode Release -Version 2.0.1

.EXAMPLE
    .\Publish-AllSql.ps1 -Mode Programmability

.EXAMPLE
    .\Publish-AllSql.ps1 -Mode FromSource -TargetPath D:\deploy\v2-from-source
#>
[CmdletBinding()]
param(
    [ValidateSet('All', 'Release', 'Programmability', 'FromSource')]
    [string]$Mode = 'All',

    [string]$Version,

    [string]$TargetPath = 'C:\publish\v2'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$scriptRoot = $PSScriptRoot
$migrationsRoot = Join-Path $scriptRoot 'Migrations'

# Order matches BlazorSvt/SqlScripts/README.md (FromSource + Programmability)
$fullManifest = @(
  'Platform\Structure\00.Create_Schema.sql'
  'Platform\Structure\01.Create_Fulltext_Catalog.sql'
  'Platform\Structure\02.Create_SyncState.sql'
  'Sync\01.Legacy_AddRowVersion.sql'
  'Platform\Programmability\fn_GetDateSqlOperator.sql'
  'Platform\Programmability\sp_GetBlazorGridData.sql'
  'Platform\Programmability\sp_ExportBlazorGridDetail.sql'
  'Platform\Programmability\sp_SyncState_Get.sql'
  'Platform\Programmability\sp_SyncState_Upsert.sql'
  'Platform\Programmability\sp_SyncState_MarkReconciled.sql'
  'Platform\Programmability\sp_Sync_GetHighWatermark.sql'
  'Platform\Programmability\sp_Sync_UpsertAffected.sql'
  'Platform\Programmability\sp_Sync_Reconcile.sql'
  'Modules\TransportRate\Structure\01.TransportRate_CreateTable.sql'
  'Modules\TransportRate\Programmability\vw_TransportRate_SnapshotSource.sql'
  'Modules\TransportRate\Structure\02.TransportRate_Insert.sql'
  'Modules\TransportRate\Structure\03.TransportRate_CreateIndexes.sql'
  'Modules\TransportRate\Programmability\vw_TransportRate_Detail.sql'
  'Modules\TransportRate\Programmability\sp_TransportRate_PopulateAffectedKeys.sql'
  'Modules\ParityRates\Structure\01.ParityRates_CreateTable.sql'
  'Modules\ParityRates\Programmability\vw_ParityRates_SnapshotSource.sql'
  'Modules\ParityRates\Structure\02.ParityRates_Insert.sql'
  'Modules\ParityRates\Structure\03.ParityRates_CreateIndexes.sql'
  'Modules\ParityRates\Programmability\vw_ParityRates_Detail.sql'
  'Modules\ParityRates\Programmability\sp_ParityRates_PopulateAffectedKeys.sql'
  'Modules\AverageRateLevel3\Structure\01.AverageRateLevel3_CreateTable.sql'
  'Modules\AverageRateLevel3\Programmability\vw_AverageRateLevel3_SnapshotSource.sql'
  'Modules\AverageRateLevel3\Structure\02.AverageRateLevel3_Insert.sql'
  'Modules\AverageRateLevel3\Structure\03.AverageRateLevel3_CreateIndexes.sql'
  'Modules\AverageRateLevel3\Programmability\vw_AverageRateLevel3_Detail.sql'
  'Modules\AverageRateLevel3\Programmability\sp_AverageRateLevel3_PopulateAffectedKeys.sql'
  'Modules\TransportLeg\Structure\01.TransportLeg_CreateTable.sql'
  'Modules\TransportLeg\Programmability\vw_TransportLeg_SnapshotSource.sql'
  'Modules\TransportLeg\Structure\02.TransportLeg_Insert.sql'
  'Modules\TransportLeg\Structure\03.TransportLeg_CreateIndexes.sql'
  'Modules\TransportLeg\Programmability\vw_TransportLeg_Detail.sql'
  'Modules\TransportLeg\Programmability\sp_TransportLeg_PopulateAffectedKeys.sql'
  'Modules\LocationsNodes\Structure\01.LocationsNodes_CreateTable.sql'
  'Modules\LocationsNodes\Programmability\vw_LocationsNodes_SnapshotSource.sql'
  'Modules\LocationsNodes\Structure\02.LocationsNodes_Insert.sql'
  'Modules\LocationsNodes\Structure\03.LocationsNodes_CreateIndexes.sql'
  'Modules\LocationsNodes\Programmability\vw_LocationsNodes_Detail.sql'
  'Modules\LocationsNodes\Programmability\sp_LocationsNodes_PopulateAffectedKeys.sql'
)

$programmabilityManifest = $fullManifest | Where-Object {
    $_ -match '\\Programmability\\'
}

function Ensure-Directory {
    param([string]$Path)
    if (-not (Test-Path -LiteralPath $Path)) {
        New-Item -ItemType Directory -Path $Path -Force | Out-Null
        Write-Host "Created: $Path" -ForegroundColor Yellow
    }
}

function Clear-SqlFiles {
    param([string]$Path)
    if (Test-Path -LiteralPath $Path) {
        Get-ChildItem -LiteralPath $Path -Filter '*.sql' -File -ErrorAction SilentlyContinue |
            Remove-Item -Force
    }
}

function Get-MigrationVersions {
    if (-not (Test-Path -LiteralPath $migrationsRoot)) {
        throw "Migrations folder not found: $migrationsRoot"
    }

    Get-ChildItem -LiteralPath $migrationsRoot -Directory |
        Where-Object { $_.Name -match '^\d+\.\d+\.\d+$' } |
        Sort-Object {
            $parts = $_.Name.Split('.')
            [int]$parts[0] * 1000000 + [int]$parts[1] * 1000 + [int]$parts[2]
        }
}

function Copy-MigrationFolder {
    param(
        [Parameter(Mandatory)][string]$VersionName,
        [Parameter(Mandatory)][string]$DestinationDir
    )

    $sourceDir = Join-Path $migrationsRoot $VersionName
    if (-not (Test-Path -LiteralPath $sourceDir)) {
        throw "Migration version not found: $sourceDir"
    }

    Ensure-Directory -Path $DestinationDir
    Clear-SqlFiles -Path $DestinationDir

    $files = Get-ChildItem -LiteralPath $sourceDir -Filter '*.sql' -File |
        Sort-Object Name

    if ($files.Count -eq 0) {
        throw "No *.sql files in $sourceDir"
    }

    $copied = 0
    foreach ($file in $files) {
        Copy-Item -LiteralPath $file.FullName -Destination (Join-Path $DestinationDir $file.Name) -Force
        Write-Host "==> $VersionName/$($file.Name)" -ForegroundColor Cyan
        $copied++
    }
    return $copied
}

function Publish-FromManifest {
    param(
        [Parameter(Mandatory)][string[]]$Manifest,
        [Parameter(Mandatory)][string]$DestinationDir
    )

    Ensure-Directory -Path $DestinationDir
    Clear-SqlFiles -Path $DestinationDir

    $copied = 0
    for ($i = 0; $i -lt $Manifest.Count; $i++) {
        $relativePath = $Manifest[$i]
        $sourcePath = Join-Path $scriptRoot $relativePath

        if (-not (Test-Path -LiteralPath $sourcePath)) {
            throw "Source script not found: $sourcePath"
        }

        $prefix = '{0:D3}' -f ($i + 1)
        $originalName = Split-Path -Leaf $sourcePath
        $targetName = "$prefix.$originalName"
        Copy-Item -LiteralPath $sourcePath -Destination (Join-Path $DestinationDir $targetName) -Force
        Write-Host "==> $targetName" -ForegroundColor Cyan
        $copied++
    }
    return $copied
}

$total = 0

switch ($Mode) {
    'All' {
        Ensure-Directory -Path $TargetPath
        $versions = Get-MigrationVersions
        if ($versions.Count -eq 0) {
            throw "No version folders (x.y.z) under $migrationsRoot"
        }
        foreach ($dir in $versions) {
            $dest = Join-Path $TargetPath $dir.Name
            $total += Copy-MigrationFolder -VersionName $dir.Name -DestinationDir $dest
        }
    }
    'Release' {
        if ([string]::IsNullOrWhiteSpace($Version)) {
            throw "Mode Release requires -Version (e.g. -Version 2.0.1)"
        }
        $dest = Join-Path $TargetPath $Version
        $total = Copy-MigrationFolder -VersionName $Version -DestinationDir $dest
    }
    'Programmability' {
        $dest = Join-Path $TargetPath 'programmability'
        $total = Publish-FromManifest -Manifest $programmabilityManifest -DestinationDir $dest
    }
    'FromSource' {
        $total = Publish-FromManifest -Manifest $fullManifest -DestinationDir $TargetPath
    }
}

Write-Host ''
Write-Host "Done. Mode=$Mode. Published scripts: $total -> $TargetPath" -ForegroundColor Green
if ($Mode -eq 'All' -or $Mode -eq 'Release') {
    Write-Host "Hint: after schema migrations, publish programmability with -Mode Programmability" -ForegroundColor DarkGray
}

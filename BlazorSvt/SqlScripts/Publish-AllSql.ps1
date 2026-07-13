#Requires -Version 5.1
<#
.SYNOPSIS
    Publishes all deploy SQL scripts to a flat folder with ordered 3-digit prefixes.

.DESCRIPTION
    Copies Structure + Programmability scripts in README order to C:\publish\v2.
    Clears existing *.sql in the target folder before copy.
    Does not modify source scripts in the repository.

.EXAMPLE
    .\Publish-AllSql.ps1

.EXAMPLE
    .\Publish-AllSql.ps1 -TargetPath D:\deploy\v2
#>
[CmdletBinding()]
param(
    [string]$TargetPath = 'C:\publish\v2'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# Order matches BlazorSvt/SqlScripts/README.md
$scriptManifest = @(
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

$scriptRoot = $PSScriptRoot

if (-not (Test-Path -LiteralPath $TargetPath)) {
    New-Item -ItemType Directory -Path $TargetPath -Force | Out-Null
    Write-Host "Created: $TargetPath" -ForegroundColor Yellow
}

Get-ChildItem -LiteralPath $TargetPath -Filter '*.sql' -File -ErrorAction SilentlyContinue |
    Remove-Item -Force

$copied = 0
for ($i = 0; $i -lt $scriptManifest.Count; $i++) {
    $relativePath = $scriptManifest[$i]
    $sourcePath = Join-Path $scriptRoot $relativePath

    if (-not (Test-Path -LiteralPath $sourcePath)) {
        throw "Source script not found: $sourcePath"
    }

    $prefix = '{0:D3}' -f ($i + 1)
    $originalName = Split-Path -Leaf $sourcePath
    $targetName = "$prefix.$originalName"
    $targetFile = Join-Path $TargetPath $targetName

    Copy-Item -LiteralPath $sourcePath -Destination $targetFile -Force
    Write-Host "==> $targetName" -ForegroundColor Cyan
    $copied++
}

Write-Host ''
Write-Host "Done. Published scripts: $copied -> $TargetPath" -ForegroundColor Green

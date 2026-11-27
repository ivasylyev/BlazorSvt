# Builds docs/output-review-package.zip for architecture review.
# Canon stays in .cursor/skills/svt-architecture/ and docs/docs/svt-v-atlas/.
# After a successful zip, generated markdown, diagrams and ideal-process HTML
# are removed from this folder. README, this script and review-current-state.html stay.
# -NoZip leaves the generated files in -Destination.
#
#   .\docs\output-review-package\Build-ReviewPackage.ps1
#   .\docs\output-review-package\Build-ReviewPackage.ps1 -NoZip -Destination D:\tmp\pkg

param(
    [string]$Destination,
    [switch]$NoZip
)

$ErrorActionPreference = "Stop"
$packageDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = (Resolve-Path (Join-Path $packageDir "..\..")).Path
if (-not $Destination) {
    $Destination = $packageDir
}
$destinationPath = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($Destination)
New-Item -ItemType Directory -Force -Path $destinationPath | Out-Null

$utf8 = New-Object System.Text.UTF8Encoding $false
$readmePath = Join-Path $packageDir "README.md"
$readme = [System.IO.File]::ReadAllText($readmePath, $utf8)
$readmeLines = $readme -split "\r?\n", -1
$headerIndex = -1
for ($i = 0; $i -lt $readmeLines.Count; $i++) {
    if ($readmeLines[$i] -match '^\|.*Confluence') {
        $headerIndex = $i
        break
    }
}
if ($headerIndex -lt 0) {
    throw "README.md has no Confluence table."
}
$pages = New-Object System.Collections.Generic.List[object]
for ($i = $headerIndex + 1; $i -lt $readmeLines.Count; $i++) {
    $line = $readmeLines[$i]
    if ($line -notmatch '^\|') { break }
    if ($line -match '^\|\s*-') { continue }
    if ($line -notmatch '`([^`]+)`') {
        throw "Confluence table row has no canon file name: $line"
    }
    $name = $Matches[1]
    $title = $null
    $url = $null
    if ($line -match '\[([^\]]+)\]\((https://[^)]+)\)') {
        $title = $Matches[1]
        $url = $Matches[2]
    }
    $pages.Add([pscustomobject]@{
        Name  = $name
        Title = $title
        Url   = $url
    })
}
if ($pages.Count -eq 0) {
    throw "README.md Confluence table has no canon files."
}
$duplicateNames = @($pages | Group-Object Name | Where-Object { $_.Count -gt 1 } | ForEach-Object { $_.Name })
if ($duplicateNames.Count -gt 0) {
    throw "Duplicate canon file in Confluence table: $($duplicateNames -join ', ')"
}
$published = @($pages | Where-Object { $_.Url })

function Convert-ToConfluence([string]$text, $pageList) {
    foreach ($p in $pageList) {
        $name = $p.Name
        $canonical = "[{0}]({1})" -f $p.Title, $p.Url
        $wrapped = '`' + $name + '`'
        $text = $text.Replace("[$name]($name)", $canonical)
        $text = $text.Replace("[$name](./$name)", $canonical)
        $text = $text.Replace("[$wrapped]($name)", $canonical)
        $text = $text.Replace("]($name)", "]($($p.Url))")
        $text = $text.Replace("](./$name)", "]($($p.Url))")
        $text = $text.Replace($wrapped, $canonical)
        $text = $text.Replace($name, $canonical)
    }
    # Skill files sit three levels under the repo root; the package folder sits two.
    # ../../../docs/tasks/... from the skill becomes ../../docs/tasks/... in the package.
    return $text.Replace("](../../../", "](../../")
}

$skillDir = Join-Path $repoRoot ".cursor\skills\svt-architecture"
foreach ($p in $pages) {
    if ($p.Name -eq "SVT-idealnyy-process-2.7.html") {
        $source = Join-Path $repoRoot "docs\docs\svt-v-atlas\SVT-idealnyy-process-2.7.html"
    }
    elseif ($p.Name -eq "review-current-state.html") {
        $source = Join-Path $packageDir "review-current-state.html"
    }
    else {
        $source = Join-Path $skillDir $p.Name
    }
    if (-not (Test-Path -LiteralPath $source)) {
        throw "Missing canon file: $source"
    }
    $outFile = Join-Path $destinationPath $p.Name
    if ([System.IO.Path]::GetFullPath($source) -eq [System.IO.Path]::GetFullPath($outFile)) {
        continue
    }
    if ($p.Url) {
        $raw = [System.IO.File]::ReadAllText($source, $utf8)
        $converted = Convert-ToConfluence $raw $published
        [System.IO.File]::WriteAllText($outFile, $converted, $utf8)
    }
    else {
        Copy-Item -LiteralPath $source -Destination $outFile -Force
    }
}

$pngs = @(
    "01-as-is-legacy.png",
    "02-to-be-current-program.png",
    "03-future-containers-out-of-scope.png"
)
foreach ($png in $pngs) {
    Copy-Item -LiteralPath (Join-Path $skillDir $png) -Destination (Join-Path $destinationPath $png) -Force
}

if ($NoZip) {
    Write-Output "Package written to $destinationPath (zip skipped)."
    return
}

$zipPath = Join-Path $repoRoot "docs\output-review-package.zip"
if (Test-Path -LiteralPath $zipPath) {
    Remove-Item -LiteralPath $zipPath -Force
}
$stagingParent = Join-Path ([System.IO.Path]::GetTempPath()) ("svt-review-" + [guid]::NewGuid().ToString("N"))
$staging = Join-Path $stagingParent "output-review-package"
New-Item -ItemType Directory -Force -Path $staging | Out-Null
try {
    Copy-Item -Path (Join-Path $destinationPath "*") -Destination $staging -Recurse -Force
    $scriptInStaging = Join-Path $staging "Build-ReviewPackage.ps1"
    if (Test-Path -LiteralPath $scriptInStaging) {
        Remove-Item -LiteralPath $scriptInStaging -Force
    }
    Compress-Archive -Path $staging -DestinationPath $zipPath -Force
}
finally {
    Remove-Item -LiteralPath $stagingParent -Recurse -Force
}

$generatedNames = @($pages | Where-Object { $_.Name -ne "review-current-state.html" } | ForEach-Object { $_.Name }) + @("reference.md") + $pngs + @(
    "SVT-idealnyy-process2.html"
)
foreach ($name in $generatedNames) {
    $generatedPath = Join-Path $packageDir $name
    if (Test-Path -LiteralPath $generatedPath) {
        Remove-Item -LiteralPath $generatedPath -Force
    }
}

Write-Output "Zip: $zipPath"
if ($destinationPath.TrimEnd('\') -eq $packageDir.TrimEnd('\')) {
    Write-Output "Generated copies removed from $packageDir"
}
else {
    Write-Output "Package written to $destinationPath"
    Write-Output "Generated copies removed from $packageDir"
}

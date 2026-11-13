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
$pages = New-Object System.Collections.Generic.List[object]
foreach ($m in [regex]::Matches($readme, '\|\s*`([^`]+)`\s*\|\s*\[([^\]]+)\]\((https://[^)]+)\)\s*\|')) {
    $pages.Add([pscustomobject]@{
        Name  = $m.Groups[1].Value
        Title = $m.Groups[2].Value
        Url   = $m.Groups[3].Value
    })
}
if ($pages.Count -ne 4) {
    throw "README.md must list exactly 4 canon files in the Confluence table (found $($pages.Count))."
}

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
    $source = Join-Path $skillDir $p.Name
    if (-not (Test-Path -LiteralPath $source)) {
        throw "Missing canon file: $source"
    }
    $raw = [System.IO.File]::ReadAllText($source, $utf8)
    $converted = Convert-ToConfluence $raw $pages
    $outFile = Join-Path $destinationPath $p.Name
    [System.IO.File]::WriteAllText($outFile, $converted, $utf8)
}

$pngs = @(
    "01-as-is-legacy.png",
    "02-to-be-current-program.png",
    "03-future-containers-out-of-scope.png"
)
foreach ($png in $pngs) {
    Copy-Item -LiteralPath (Join-Path $skillDir $png) -Destination (Join-Path $destinationPath $png) -Force
}

$idealSource = Join-Path $repoRoot "docs\docs\svt-v-atlas\SVT-idealnyy-process-2.7.html"
$idealDest = Join-Path $destinationPath "SVT-idealnyy-process-2.7.html"
if (-not (Test-Path -LiteralPath $idealSource)) {
    throw "Missing ideal-process canon: $idealSource"
}
Copy-Item -LiteralPath $idealSource -Destination $idealDest -Force

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

$generatedNames = @($pages | ForEach-Object { $_.Name }) + $pngs + @(
    "SVT-idealnyy-process-2.7.html",
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

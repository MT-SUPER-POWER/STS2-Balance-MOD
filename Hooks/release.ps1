# Release helper script: modularized for manual control.
#
# Usage:
#   .\Hooks\release.ps1 -Version 0.0.5 -Build           # Only build the zip
#   .\Hooks\release.ps1 -Version 0.0.5 -Upload          # Only upload existing zip
#   .\Hooks\release.ps1 -Version 0.0.5 -UpdateJson      # Only update version in json
#   .\Hooks\release.ps1 -Version 0.0.5 -Build -Upload   # Build and then upload

param(
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [switch]$UpdateJson,
    [switch]$Build,
    [switch]$Upload,
    [switch]$All
)

$ErrorActionPreference = "Stop"
$root = Split-Path $PSScriptRoot -Parent
$modName = "Sts2BalanceMod"
$tag = "v$Version"
$zip = Join-Path $root "dist\$modName-v$Version.zip"

Set-Location $root

if ($All) {
    $UpdateJson = $true
    $Build = $true
    $Upload = $true
}

if (-not ($UpdateJson -or $Build -or $Upload)) {
    Write-Host "Usage: .\Hooks\release.ps1 -Version <version> [-UpdateJson] [-Build] [-Upload] [-All]"
    Write-Host "Example: .\Hooks\release.ps1 -Version 0.0.5 -Build -Upload"
    exit 0
}

# --- Step 1: Update JSON version ---
if ($UpdateJson) {
    $jsonPath = "Sts2BalanceMod.json"
    Write-Host ">> Updating $jsonPath to v$Version..."
    $json = Get-Content $jsonPath -Raw
    $json = $json -replace '"version"\s*:\s*"[^"]*"', ('"version": "v' + $Version + '"')
    [System.IO.File]::WriteAllText((Join-Path $root $jsonPath), $json, (New-Object System.Text.UTF8Encoding $false))
    Write-Host ">> Done."
}

# --- Step 2: Build Package ---
if ($Build) {
    Write-Host ">> Packaging release..."
    $zip = & "$PSScriptRoot\package-release.ps1" -Version $Version | Select-Object -Last 1
    if (-not (Test-Path $zip)) {
        throw "Package step did not produce a zip file at: $zip"
    }
    Write-Host ">> Package ready: $zip"
}

# --- Step 3: Upload to GitHub ---
if ($Upload) {
    if (-not (Test-Path $zip)) {
        throw "Zip file not found: $zip. Run with -Build first or ensure it exists."
    }

    if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
        throw "gh CLI not found. Please install GitHub CLI and run 'gh auth login'."
    }

    Write-Host ">> Checking for release $tag on GitHub..."
    gh release view $tag 1>$null 2>$null
    if ($LASTEXITCODE -ne 0) {
        Write-Host ">> Release $tag not found. Creating a draft release..."
        # If the release doesn't exist, we create it. 
        # We can use the changelog extractor if it exists.
        $notes = "Release $tag"
        if (Test-Path "$PSScriptRoot\extract-changelog.ps1") {
            $notes = & "$PSScriptRoot\extract-changelog.ps1" -Version $Version
        }
        $notes | gh release create $tag --title "Release $tag" --notes-file -
    }

    Write-Host ">> Uploading $zip to release $tag..."
    # --clobber overwrites existing assets with the same name
    gh release upload $tag $zip --clobber
    if ($LASTEXITCODE -eq 0) {
        Write-Host ">> Upload successful!"
    } else {
        throw "gh release upload failed."
    }
}

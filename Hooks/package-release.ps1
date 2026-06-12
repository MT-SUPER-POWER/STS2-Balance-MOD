# Build mod locally and package to dist/*.zip
# Release output: dist/Sts2BalanceMod/

param(
    [Parameter(Mandatory = $true)]
    [string]$Version
)

$ErrorActionPreference = "Stop"
$root = Split-Path $PSScriptRoot -Parent
$modName = "Sts2BalanceMod"
$distDir = Join-Path $root "dist"
$modDir = Join-Path $distDir $modName

Push-Location $root
try {
    Write-Host ">> dotnet publish -c Release"
    dotnet publish -c Release
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}
finally {
    Pop-Location
}

if (-not (Test-Path $modDir)) {
    throw "Build output not found: $modDir. Run 'dotnet publish -c Release' first."
}

$required = @("$modName.dll", "$modName.json", "$modName.pck")
foreach ($file in $required) {
    $path = Join-Path $modDir $file
    if (-not (Test-Path $path)) {
        throw "Missing release file: $file ($modDir)"
    }
}

$zip = Join-Path $distDir "$modName-v$Version.zip"
if (Test-Path $zip) { Remove-Item $zip -Force }

Write-Host ">> zip $modDir -> $zip"
Compress-Archive -Path "$modDir\*" -DestinationPath $zip -Force
Write-Host ">> done: $zip"
Write-Output $zip

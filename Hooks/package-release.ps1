# 在本机构建 Mod 并打包为 zip。
# Release 产物由 dotnet publish 输出到 build/Sts2BalanceMod/

param(
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [string]$OutputDir = "dist"
)

$ErrorActionPreference = "Stop"
$root = Split-Path $PSScriptRoot -Parent
$modName = "Sts2BalanceMod"
$modDir = Join-Path $root "build\$modName"

Push-Location $root
try {
    Write-Host ">> dotnet publish -c Release"
    dotnet publish -c Release
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
} finally {
    Pop-Location
}

if (-not (Test-Path $modDir)) {
    throw "未找到构建输出: $modDir`n请先成功执行 dotnet publish -c Release。"
}

$required = @("$modName.dll", "$modName.json", "$modName.pck")
foreach ($file in $required) {
    if (-not (Test-Path (Join-Path $modDir $file))) {
        throw "缺少发布文件: $file（目录: $modDir）"
    }
}

$distDir = Join-Path $root $OutputDir
New-Item -ItemType Directory -Force -Path $distDir | Out-Null
$zip = Join-Path $distDir "$modName-v$Version.zip"
if (Test-Path $zip) { Remove-Item $zip -Force }

Write-Host ">> 打包 $modDir -> $zip"
Compress-Archive -Path "$modDir\*" -DestinationPath $zip -Force
Write-Host ">> 完成: $zip"
Write-Output $zip

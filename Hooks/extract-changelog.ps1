# 从 CHANGELOG.md 提取指定版本的更新说明。
# 用法：
#   .\Hooks\extract-changelog.ps1 -Version 0.0.4.1
#   .\Hooks\extract-changelog.ps1 -Version 0.0.4.1 -OutputPath dist\release_notes.md

param(
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [string]$OutputPath,
    [string]$ChangelogPath = "CHANGELOG.md"
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $ChangelogPath)) {
    throw "未找到 $ChangelogPath"
}

$content = Get-Content -Path $ChangelogPath -Raw -Encoding UTF8
$escaped = [regex]::Escape($Version)
$patterns = @(
    "(?ms)^# v$escaped\s*\r?\n(.*?)(?=^# v|\z)",
    "(?ms)^# V $escaped\s*\r?\n(.*?)(?=^# [vV]|\z)",
    "(?ms)^## \[$escaped\].*?\r?\n(.*?)(?=^## \[|^# [vV]|\z)"
)

$notes = $null
foreach ($pattern in $patterns) {
    $match = [regex]::Match($content, $pattern)
    if ($match.Success) {
        $notes = $match.Groups[1].Value.Trim()
        break
    }
}

if (-not $notes) {
    $notes = "未在 CHANGELOG.md 中找到 ``# v$Version`` 段落，请查看 [CHANGELOG.md](CHANGELOG.md)。"
}

if ($OutputPath) {
    $dir = Split-Path $OutputPath -Parent
    if ($dir) { New-Item -ItemType Directory -Force -Path $dir | Out-Null }
    $notes | Out-File -FilePath $OutputPath -Encoding utf8NoBOM
}

Write-Output $notes

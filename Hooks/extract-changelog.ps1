# Extract release notes for a version from CHANGELOG.md

param(
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [string]$OutputPath,
    [string]$ChangelogPath = "CHANGELOG.md"
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $ChangelogPath)) {
    throw "Missing $ChangelogPath"
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
    $notes = "No # v$Version section in CHANGELOG.md. See CHANGELOG.md."
}

if ($OutputPath) {
    $dir = Split-Path $OutputPath -Parent
    if ($dir) { New-Item -ItemType Directory -Force -Path $dir | Out-Null }
    [System.IO.File]::WriteAllText($OutputPath, $notes, (New-Object System.Text.UTF8Encoding $false))
}

Write-Output $notes

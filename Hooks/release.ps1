# 本机构建 Mod 安装包，推送 Tag 后由 Actions 写入 Release 说明，本脚本上传 zip 附件。
# 需要：gh auth login
#
# 用法：
#   .\Hooks\release.ps1 -Version 0.0.4.1
#   .\Hooks\release.ps1 -Version 0.0.4.1 -SkipPush    # 仅本地构建打包

param(
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [switch]$SkipPush,
    [switch]$SkipBuild,
    [int]$WaitSeconds = 300
)

$ErrorActionPreference = "Stop"
$root = Split-Path $PSScriptRoot -Parent
$modName = "Sts2BalanceMod"
$tag = "v$Version"

Set-Location $root

function Wait-ForGitHubRelease {
    param(
        [string]$ReleaseTag,
        [int]$TimeoutSec
    )

    $deadline = (Get-Date).AddSeconds($TimeoutSec)
    while ((Get-Date) -lt $deadline) {
        gh release view $ReleaseTag 2>$null | Out-Null
        if ($LASTEXITCODE -eq 0) {
            Write-Host ">> Release $ReleaseTag 已就绪"
            return
        }
        Write-Host ">> 等待 Actions 创建 Release $ReleaseTag ..."
        Start-Sleep -Seconds 5
    }

    throw "等待 Release 超时（${TimeoutSec}s）。请检查 GitHub Actions 是否成功，或手动创建 Release 后执行：`n  gh release upload $ReleaseTag dist\$modName-v$Version.zip --clobber"
}

if (-not $SkipPush) {
    if (git tag --list $tag) { throw "本地 Tag $tag 已存在。请换版本号或删除：git tag -d $tag" }
    $remoteTags = git ls-remote --tags origin "refs/tags/$tag"
    if ($remoteTags) { throw "远程 Tag $tag 已存在。请换版本号或删除远程 Tag。" }
}

# --- 写入版本号 ---
$jsonPath = "Sts2BalanceMod.json"
$json = Get-Content $jsonPath -Raw
$json = $json -replace '"version"\s*:\s*"[^"]*"', "`"version`": `"v$Version`""
$json | Set-Content $jsonPath -Encoding utf8NoBOM -NoNewline
Write-Host ">> 已更新 $jsonPath -> v$Version"

# --- 构建 + 打包 ---
if (-not $SkipBuild) {
    $zip = & "$PSScriptRoot\package-release.ps1" -Version $Version
} else {
    $zip = Join-Path $root "dist\$modName-v$Version.zip"
    if (-not (Test-Path $zip)) { throw "未找到 $zip，去掉 -SkipBuild 或先手动打包。" }
}

Write-Host ">> 打包完成: $zip"

# --- 预览 CHANGELOG（Release 正文由 Actions 从 CHANGELOG.md 写入） ---
Write-Host "----- CHANGELOG 预览 -----"
& "$PSScriptRoot\extract-changelog.ps1" -Version $Version
Write-Host "--------------------------"

if ($SkipPush) {
    Write-Host ">> 已跳过 push / 上传（-SkipPush）"
    exit 0
}

if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    throw "未找到 gh 命令。请安装 GitHub CLI 并执行 gh auth login"
}

# --- 提交、打 Tag、推送（触发 Actions 写 Release 说明） ---
git add $jsonPath
if (git diff --cached --quiet) {
    Write-Warning "Sts2BalanceMod.json 无变更，跳过 commit。"
} else {
    git commit -m "chore(release): v$Version"
}

git tag $tag
git push origin HEAD --tags
Write-Host ">> 已推送 $tag，等待 Actions 创建 Release ..."

Wait-ForGitHubRelease -ReleaseTag $tag -TimeoutSec $WaitSeconds

# --- 上传安装包附件 ---
gh release upload $tag $zip --clobber
Write-Host ">> 已上传附件到 $tag"

$ErrorActionPreference = "Stop"

function Assert-Contains {
  param(
    [string]$Content,
    [string]$Pattern,
    [string]$Message
  )

  if ($Content -notmatch $Pattern) {
    throw $Message
  }
}

function Assert-NotContains {
  param(
    [string]$Content,
    [string]$Pattern,
    [string]$Message
  )

  if ($Content -match $Pattern) {
    throw $Message
  }
}

function Assert-FileExists {
  param(
    [string]$Path,
    [string]$Message
  )

  if (-not (Test-Path -LiteralPath $Path)) {
    throw $Message
  }
}

$repoRoot = Split-Path -Parent $PSScriptRoot
$timeEater = Get-Content -Raw (Join-Path $repoRoot "Sts2BalanceModCode/Monsters/TimeEater.cs")
$timeWarpPower = Get-Content -Raw (Join-Path $repoRoot "Sts2BalanceModCode/Powers/TimeWarpPower.cs")
$guardian = Get-Content -Raw (Join-Path $repoRoot "Sts2BalanceModCode/Monsters/Guardian.cs")
$hexaghost = Get-Content -Raw (Join-Path $repoRoot "Sts2BalanceModCode/Monsters/Hexaghost.cs")
$slimeBoss = Get-Content -Raw (Join-Path $repoRoot "Sts2BalanceModCode/Monsters/SlimeBoss.cs")
$mindBloomSlimeBoss = Get-Content -Raw (Join-Path $repoRoot "Sts2BalanceModCode/Encounters/MindBloomSlimeBoss.cs")

Assert-Contains $timeEater "BaseTimeWarpCounter\s*\+\s*TimeWarpCounterPerExtraPlayer\s*\*\s*\(PlayerCount\s*-\s*1\)" "Time Eater must scale TimeWarp counter by +3 for each extra player."
Assert-Contains $timeEater "BaseInitialHp\s*\*\s*PlayerCount" "Time Eater HP must scale by player count from the ascension base HP."
Assert-NotContains $timeEater "Apply<TimeWarpPower>\([^;]*12M" "Time Eater must not apply TimeWarp with a hardcoded 12 counter."
Assert-Contains $timeWarpPower "BaseCardsPerWarp\s*\+\s*CardsPerExtraPlayer\s*\*\s*\(PlayerCount\s*-\s*1\)" "TimeWarpPower must reset to the multiplayer-scaled counter."
Assert-NotContains $timeWarpPower "private\s+const\s+decimal\s+CardsPerWarp\s*=\s*12M" "TimeWarpPower must not reset to a fixed 12-card counter."

Assert-Contains $guardian 'ModAssetPaths\.Resource\("monsters", "guardian", "guardian\.tscn"\)' "Guardian visual path must use packed Sts2BalanceMod assets."
Assert-Contains $hexaghost 'ModAssetPaths\.Resource\("monsters", "hexaghost", "hexaghost\.tscn"\)' "Hexaghost visual path must use packed Sts2BalanceMod assets."
Assert-Contains $slimeBoss 'ModAssetPaths\.Resource\("monsters", "slime_boss", "slime_boss\.tscn"\)' "Slime Boss visual path must use packed Sts2BalanceMod assets."
Assert-Contains $mindBloomSlimeBoss 'ModAssetPaths\.Resource\("scenes", "actsfromthepast-mind_bloom_slime_boss\.tscn"\)' "Mind Bloom Slime Boss encounter must use packed Sts2BalanceMod scene assets."

foreach ($relativePath in @(
  "Sts2BalanceMod/monsters/guardian/guardian.tscn",
  "Sts2BalanceMod/monsters/guardian/guardian_skel_data.tres",
  "Sts2BalanceMod/monsters/guardian/guardian.spatlas",
  "Sts2BalanceMod/monsters/guardian/guardian.spskel",
  "Sts2BalanceMod/monsters/guardian/guardian.png",
  "Sts2BalanceMod/monsters/hexaghost/hexaghost.tscn",
  "Sts2BalanceMod/monsters/hexaghost/core.png",
  "Sts2BalanceMod/monsters/slime_boss/slime_boss.tscn",
  "Sts2BalanceMod/monsters/slime_boss/slime_boss_skel_data.tres",
  "Sts2BalanceMod/monsters/slime_boss/slime_boss.spatlas",
  "Sts2BalanceMod/monsters/slime_boss/slime_boss.spskel",
  "Sts2BalanceMod/monsters/slime_boss/slime_boss.png",
  "Sts2BalanceMod/scenes/actsfromthepast-mind_bloom_slime_boss.tscn"
)) {
  Assert-FileExists (Join-Path $repoRoot $relativePath) "Packed resource is missing: $relativePath"
}

foreach ($relativePath in @(
  "Sts2BalanceMod/monsters/guardian/guardian.tscn",
  "Sts2BalanceMod/monsters/guardian/guardian_skel_data.tres",
  "Sts2BalanceMod/monsters/hexaghost/hexaghost.tscn",
  "Sts2BalanceMod/monsters/slime_boss/slime_boss.tscn",
  "Sts2BalanceMod/monsters/slime_boss/slime_boss_skel_data.tres"
)) {
  $content = Get-Content -Raw (Join-Path $repoRoot $relativePath)
  Assert-NotContains $content "res://Assets/ActsFromPast" "Packed resource must not reference unpacked Assets path: $relativePath"
}

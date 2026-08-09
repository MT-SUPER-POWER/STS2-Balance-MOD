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

$repoRoot = Split-Path -Parent $PSScriptRoot
$mindBloom = Get-Content -Raw (Join-Path $repoRoot "Sts2BalanceModCode/Events/MindBloom.cs")
$bear = Get-Content -Raw (Join-Path $repoRoot "Sts2BalanceModCode/Monsters/Bear.cs")

Assert-Contains $mindBloom "Option\(Fight" "Mind Bloom must expose the Act 1 boss fight option."
Assert-NotContains $mindBloom "FIGHT_LOCKED" "Mind Bloom fight option must not stay locked."
Assert-Contains $mindBloom "Overgrowth" "Mind Bloom must support Overgrowth act 1 bosses."
Assert-Contains $mindBloom "Underdocks" "Mind Bloom must support Underdocks act 1 bosses."
Assert-Contains $mindBloom "AllBossEncounters" "Mind Bloom must support act 1 boss encounters."
Assert-NotContains $mindBloom "MindBloomGuardian" "Mind Bloom must not include custom Guardian fight."
Assert-NotContains $mindBloom "MindBloomSlimeBoss" "Mind Bloom must not include custom Slime Boss fight."
Assert-NotContains $mindBloom "MindBloomHexaghost" "Mind Bloom must not include custom Hexaghost fight."

Assert-Contains $bear "DexterityPower" "Bear Hug must apply Dexterity debuff."

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
Assert-Contains $mindBloom "MindBloomGuardian" "Mind Bloom must include Guardian as a possible fight."
Assert-Contains $mindBloom "MindBloomHexaghost" "Mind Bloom must include Hexaghost as a possible fight."
Assert-Contains $mindBloom "MindBloomSlimeBoss" "Mind Bloom must include Slime Boss as a possible fight."

Assert-Contains $bear "VulnerablePower" "Bear Hug must apply Vulnerable."
Assert-NotContains $bear "DexterityPower" "Bear Hug must not reduce Dexterity."

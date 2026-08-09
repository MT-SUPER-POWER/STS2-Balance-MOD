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
$registration = Get-Content -Raw (Join-Path $repoRoot "Sts2BalanceModCode/Patches/Encounters/EventEncounterRegistrationPatch.cs")

foreach ($encounter in @(
  "RedMaskBandits",
  "MindBloomGuardian",
  "MindBloomHexaghost",
  "MindBloomSlimeBoss"
)) {
  Assert-Contains $registration "ModelDb\.Encounter<$encounter>\(\)" "Expected DIY event encounter is not registered: $encounter"
}

Assert-NotContains $registration "ModelDb\.Encounter<MindBloomBossEncounter>\(\)" "The vanilla Act 1 Boss wrapper must not be registered as an event encounter."

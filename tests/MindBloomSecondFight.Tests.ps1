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
$secondFight = Get-Content -Raw (Join-Path $repoRoot "Sts2BalanceModCode/Events/MindBloomSecondFight.cs")
$enhancements = Get-Content -Raw (Join-Path $repoRoot "Sts2BalanceModCode/Monsters/MindBloomBossMonsterModel.cs")
$guardianEncounter = Get-Content -Raw (Join-Path $repoRoot "Sts2BalanceModCode/Encounters/MindBloomGuardian.cs")
$hexaghostEncounter = Get-Content -Raw (Join-Path $repoRoot "Sts2BalanceModCode/Encounters/MindBloomHexaghost.cs")
$slimeBossEncounter = Get-Content -Raw (Join-Path $repoRoot "Sts2BalanceModCode/Encounters/MindBloomSlimeBoss.cs")
$splitMonsters = @(
  "AcidSlimeLarge.cs",
  "AcidSlimeMedium.cs",
  "SpikeSlimeLarge.cs",
  "SpikeSlimeMedium.cs"
) | ForEach-Object {
  Get-Content -Raw (Join-Path $repoRoot "Sts2BalanceModCode/Monsters/$_")
}

Assert-Contains $secondFight "IsReady\s*=>\s*true" "Mind Bloom's second fight must be visible."
Assert-Contains $secondFight "ExtraGold\s*=\s*100" "The second fight must grant exactly 100 extra Gold."
foreach ($encounter in @("MindBloomGuardian", "MindBloomHexaghost", "MindBloomSlimeBoss")) {
  Assert-Contains $secondFight "ModelDb\.Encounter<$encounter>\(\)" "Missing second-fight encounter: $encounter"
}
Assert-Contains $secondFight "new GoldReward\(ExtraGold, owner\)" "The second fight must grant 100 extra Gold."
Assert-Contains $secondFight "RelicRarity\.Rare" "The second fight must grant a Rare relic."
Assert-Contains $secondFight "RelicRarity\.Uncommon" "The second fight must grant an Uncommon relic."

foreach ($durability in @("Giant", "Plating", "Regeneration")) {
  Assert-Contains $secondFight "MindBloomDurabilityEnhancement\.$durability" "Missing durability enhancement: $durability"
}
foreach ($threat in @("Strength", "Ritual")) {
  Assert-Contains $secondFight "MindBloomThreatEnhancement\.$threat" "Missing threat enhancement: $threat"
}

Assert-Contains $enhancements "Creature\.MaxHp\s*\*\s*0\.25M" "Giant must add 25% max and current HP after scaling."
Assert-Contains $enhancements "Apply<PlatingPower>" "Plating enhancement is missing."
Assert-Contains $enhancements "Apply<RegenPower>" "Regeneration enhancement is missing."
Assert-Contains $enhancements "Guardian\s*=>\s*2" "Guardian must gain 2 Strength."
Assert-Contains $enhancements "Hexaghost\s*=>\s*1" "Hexaghost must gain 1 Strength."
Assert-Contains $enhancements "SlimeBoss\s*=>\s*3" "Slime Boss must gain 3 Strength."
Assert-Contains $enhancements "Apply<RitualPower>" "Ritual enhancement is missing."

foreach ($encounterContent in @($guardianEncounter, $hexaghostEncounter, $slimeBossEncounter)) {
  Assert-Contains $encounterContent "MindBloomEnhancementPlan\s*=\s*_enhancementPlan" "The opening boss must receive the rolled enhancement plan."
}
Assert-NotContains ($splitMonsters -join "`n") "MindBloomEnhancementPlan" "Split slimes must not inherit Mind Bloom enhancements."

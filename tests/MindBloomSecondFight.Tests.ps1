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
$mindBloom = Get-Content -Raw (Join-Path $repoRoot "src/Events/MindBloom.cs")
$secondFight = Get-Content -Raw (Join-Path $repoRoot "src/Events/MindBloomSecondFight.cs")
$combatPatch = Get-Content -Raw (Join-Path $repoRoot "src/Patches/Events/MindBloomCombatPatch.cs")
$enhancements = Get-Content -Raw (Join-Path $repoRoot "src/Monsters/MindBloomBossMonsterModel.cs")
$guardianEncounter = Get-Content -Raw (Join-Path $repoRoot "src/Encounters/MindBloomGuardian.cs")
$hexaghostEncounter = Get-Content -Raw (Join-Path $repoRoot "src/Encounters/MindBloomHexaghost.cs")
$slimeBossEncounter = Get-Content -Raw (Join-Path $repoRoot "src/Encounters/MindBloomSlimeBoss.cs")
$splitMonsters = @(
  "AcidSlimeLarge.cs",
  "AcidSlimeMedium.cs",
  "SpikeSlimeLarge.cs",
  "SpikeSlimeMedium.cs"
) | ForEach-Object {
  Get-Content -Raw (Join-Path $repoRoot "src/Monsters/$_")
}

Assert-Contains $secondFight "IsReady\s*=>\s*true" "Mind Bloom's second fight must be visible."
Assert-Contains $secondFight "ExtraGold\s*=\s*100" "The second fight must grant exactly 100 extra Gold."
foreach ($encounter in @("MindBloomGuardian", "MindBloomHexaghost", "MindBloomSlimeBoss")) {
  Assert-Contains $secondFight "ModelDb\.Encounter<$encounter>\(\)" "Missing second-fight encounter: $encounter"
}
Assert-Contains $secondFight "new GoldReward\(ExtraGold, owner\)" "The second fight must grant 100 extra Gold."
Assert-Contains $secondFight "RelicRarity\.Rare" "The second fight must grant a Rare relic."
Assert-Contains $secondFight "RelicRarity\.Uncommon" "The second fight must grant an Uncommon relic."

Assert-Contains $mindBloom "NeedsReplayInitialization\s*=\s*true" "The second fight must arm replay initialization before entering combat."
$restoreStatePattern = 'protected\s+override\s+void\s+SetInitialEventState\(bool\s+isPreFinished\)'
Assert-Contains $mindBloom $restoreStatePattern "Mind Bloom must restore its post-first-fight page when an event save is loaded."
Assert-Contains $mindBloom "CurrentMapPointHistoryEntry" "Mind Bloom restoration must use the persisted current map-point history."
Assert-Contains $mindBloom "ModelDb\.Encounter<MindBloomBossEncounter>\(\)\.Id" "Mind Bloom restoration must identify the first-fight wrapper encounter."
Assert-Contains $mindBloom "TurnsTaken\s*>\s*0" "Mind Bloom must not treat an encounter history entry without a completed turn count as a finished first fight."
Assert-Contains $mindBloom 'SetEventState\(PageDescription\("POST_FIRST"\),\s*GeneratePostFirstOptions\(\)\)' "A completed first fight must restore the post-first-fight decision page."
Assert-Contains $mindBloom "base\.SetInitialEventState\(isPreFinished\)" "A fresh Mind Bloom event must retain the original initial page."
$armReplayIndex = $mindBloom.IndexOf("NeedsReplayInitialization = true;", [StringComparison]::Ordinal)
$enterSecondCombatIndex = $mindBloom.IndexOf(
  "EnterCombatWithoutExitingEvent(plan.Encounter", [StringComparison]::Ordinal)
if ($armReplayIndex -lt 0 -or $enterSecondCombatIndex -lt 0 -or $armReplayIndex -ge $enterSecondCombatIndex) {
  throw "Replay initialization must be armed before entering Mind Bloom's second combat."
}
Assert-Contains $combatPatch 'HarmonyPatch\(typeof\(CombatManager\), "StartCombatInternal"\)' "The second fight must initialize replay before CombatManager starts it."
Assert-Contains $combatPatch "RecordInitialState" "The second fight must record a fresh replay initial state."
Assert-Contains $combatPatch "IsRecordingReplay" "Replay initialization must not overwrite an active replay."
Assert-Contains $combatPatch "NeedsReplayInitialization\s*=\s*false" "The replay initialization flag must be consumed exactly once."
Assert-Contains $combatPatch "Encounter:\s*MindBloomBossEncounter" "Automatic Gold filtering must only apply to the first fight wrapper."
Assert-NotContains $combatPatch "IsMindBloomEncounter" "Second-fight encounters must retain normal monster-room Gold rewards."

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

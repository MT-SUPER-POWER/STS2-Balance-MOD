using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Sts2BalanceMod.Sts2BalanceModCode.Settings;

internal static class BalanceContent
{
  private static readonly Dictionary<string, string> Owners = new()
  {
    ["DeathReap"] = "C03", ["PowerThought"] = "C04", ["Evolve"] = "C05", ["BuddySlam"] = "C06",
    ["Concentrate"] = "C15", ["Eviscerate"] = "C16", ["StepByStep"] = "C17", ["Electrodynamics"] = "C22",
    ["Sparring"] = "C28", ["Ram"] = "C29", ["Sundial"] = "R13", ["OrangePill"] = "R14",
    ["DeadBranch"] = "R15", ["Omamori"] = "R16", ["PeacePipe"] = "R17", ["SmilingMask"] = "R18",
    ["CoffieCup"] = "R19", ["FusionHammer"] = "R20", ["CurseKey"] = "R21", ["DwarfAnvil"] = "R22",
    ["WristBlade"] = "R23", ["HoveringKite"] = "R24", ["SoulContract"] = "R25", ["StrangeSpoon"] = "R26",
    ["NeowsTalisman"] = "R10", ["DelicateDoll"] = "E14", ["WitchForm"] = "E14",
    ["SorceryStrike"] = "E14", ["SorceryDefend"] = "E14",
    ["OldBeggar"] = "E03", ["Cleric"] = "E04", ["CursedTome"] = "E05", ["MaskedBandits"] = "E06",
    ["Augmenter"] = "E07", ["TheDivineFountain"] = "E08", ["MindBloom"] = "E09", ["WheelOfChange"] = "E10",
    ["TombOfLordRedMask"] = "E11", ["TheLibrary"] = "E12", ["Colosseum"] = "E13",
    ["Necronomicurse"] = "E05", ["Necronomicon"] = "E05", ["NilrysCodex"] = "E05", ["Enchiridion"] = "E05",
    ["Jax"] = "E07", ["MutagenicStrength"] = "E07", ["MarkOfTheBloom"] = "E09"
  };
  public static bool IsEnabled(Type type) => type.Assembly != typeof(BalanceContent).Assembly ||
    !Owners.TryGetValue(type.Name, out string? id) || BalanceModSettings.IsEnabled(id);

  internal static IEnumerable<MethodBase> AllowedMethods(Type model) =>
    typeof(BalanceContent).Assembly.GetTypes().Where(t => !t.IsAbstract && model.IsAssignableFrom(t))
      .Select(t => AccessTools.Method(t, "IsAllowed")).Where(m => m != null).Distinct();
}

// WARNING: GetUnlockedCards/Relics are the acquisition boundary. Keep AllCards/AllRelics
// intact: existing saved models still need their Pool and preloaded resources after disabling.
[HarmonyPatch(typeof(CardPoolModel), nameof(CardPoolModel.GetUnlockedCards))]
internal static class BalanceCardAcquisitionPatch
{
  private static void Postfix(ref IEnumerable<CardModel> __result) =>
    __result = __result.Where(c => BalanceContent.IsEnabled(c.GetType()));
}

[HarmonyPatch(typeof(RelicPoolModel), nameof(RelicPoolModel.GetUnlockedRelics))]
internal static class BalanceRelicAcquisitionPatch
{
  private static void Postfix(ref IEnumerable<RelicModel> __result) =>
    __result = __result.Where(c => BalanceContent.IsEnabled(c.GetType()));
}

// Target each override as well as inherited base methods; disabling only the base
// EventModel.IsAllowed would miss custom events that implement their own conditions.
[HarmonyPatch]
internal static class BalanceEventAllowedPatch
{
  private static IEnumerable<MethodBase> TargetMethods() => BalanceContent.AllowedMethods(typeof(EventModel));
  private static void Postfix(EventModel __instance, ref bool __result) =>
    __result &= BalanceContent.IsEnabled(__instance.GetType());
}

[HarmonyPatch]
internal static class BalanceRelicAllowedPatch
{
  private static IEnumerable<MethodBase> TargetMethods() => BalanceContent.AllowedMethods(typeof(RelicModel));
  private static void Postfix(RelicModel __instance, ref bool __result) =>
    __result &= BalanceContent.IsEnabled(__instance.GetType());
}

/// <summary>
/// Target: LocManager.LoadTable, before the returned mod table is merged into vanilla.
/// Restore vanilla descriptions when a patch is off; retain custom content text for old saves.
/// WARNING: relies on the decompiled LoadTablesFromPath -> LoadTable -> MergeWith order.
/// </summary>
[HarmonyPatch(typeof(LocManager), "LoadTable")]
internal static class BalanceLocalizationPatch
{
  private static readonly Dictionary<string, string> Owners = new()
  {
    ["EXPECT_A_FIGHT"] = "C02", ["WRAITH_FORM"] = "C10", ["GLOW"] = "C30", ["FUEL"] = "C20",
    ["DRAIN_POWER"] = "C25", ["PULL_AGGRO"] = "C26", ["GRAND_FINALE"] = "C11",
    ["WELL_LAID_PLANS"] = "C09", ["WELL_LAID_PLANS_POWER"] = "C09", ["COOLANT"] = "C21",
    ["COOLANT_POWER"] = "C21", ["HISTORY_COURSE"] = "R05", ["DIAMOND_DIADEM"] = "R06",
    ["TOASTY_MITTENS"] = "R07", ["SIGNET_RING"] = "R08", ["BEAUTIFUL_BRACELET"] = "R09",
    ["SAND_CASTLE"] = "R01", ["NEOWS_TALISMAN"] = "R10", ["ROYAL_STAMP"] = "R12",
    ["BUGSLAYER"] = "E02", ["TINKER_TIME"] = "E15", ["THE_FUTURE_OF_POTIONS"] = "E16",
    ["TREASURE_ROOM"] = "G02", ["ZEN_WEAVER"] = "E01", ["BIASED_COGNITION_POWER"] = "C18", ["INFESTED_PRISM"] = "M02"
  };
  private static void Postfix(string path, Dictionary<string, string> __result)
  {
    if (!path.StartsWith("res://Sts2BalanceMod/localization/", StringComparison.OrdinalIgnoreCase)) return;
    foreach (string key in __result.Keys.ToArray())
      if (Owners.TryGetValue(key.Split('.')[0], out string? id) && !BalanceModSettings.IsEnabled(id))
        __result.Remove(key);
  }
}

/// <summary>
/// Target: CardFactory.FilterForPlayerCount / FilterForCombat, before random selection.
/// Also cover explicit option lists supplied by generation hooks. Direct creation of
/// existing event/witch-form reward cards is intentionally left intact for saved content.
/// WARNING: relies on the vanilla factory filtering before it consumes RNG.
/// </summary>
[HarmonyPatch]
internal static class BalanceGeneratedCardPatch
{
  private static IEnumerable<MethodBase> TargetMethods() =>
  [
    AccessTools.Method(typeof(MegaCrit.Sts2.Core.Factories.CardFactory), "FilterForPlayerCount"),
    AccessTools.Method(typeof(MegaCrit.Sts2.Core.Factories.CardFactory), "FilterForCombat")
  ];
  private static void Postfix(ref IEnumerable<CardModel> __result) =>
    __result = __result.Where(c => BalanceContent.IsEnabled(c.GetType()));
}

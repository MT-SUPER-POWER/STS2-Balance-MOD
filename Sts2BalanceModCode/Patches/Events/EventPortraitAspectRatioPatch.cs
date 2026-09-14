using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Events;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Events;

/// <summary>
/// Target: <see cref="NEventLayout.SetPortrait"/>.
/// Reason: preserve the aspect ratio of RitsuLib-provided event portraits.
/// WARNING: depends on the game scene's <c>%Portrait</c> node name; re-check after game UI updates.
/// </summary>
[HarmonyPatch(typeof(NEventLayout), nameof(NEventLayout.SetPortrait))]
internal static class EventPortraitAspectRatioPatch
{
  [HarmonyPostfix]
  private static void Postfix(NEventLayout __instance)
  {
    if (__instance.GetNodeOrNull<TextureRect>("%Portrait") is not { } portrait)
    {
      return;
    }

    portrait.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
    portrait.StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered;
  }
}

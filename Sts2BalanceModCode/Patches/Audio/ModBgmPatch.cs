using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using Sts2BalanceMod.Sts2BalanceModCode.Encounters;
using Sts2BalanceMod.Sts2BalanceModCode.Utility;
using System.Reflection;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Audio;

/// <summary>
/// MOD 自定义 BGM 管理补丁。
/// 改用 CombatRoom 生命周期 + Hook 触发 FadeIn/FadeOut。
/// 当前支持：TimeEaterBoss 战斗专属 BGM。
/// </summary>
[HarmonyPatch]
public static class ModBgmPatch
{
  private const string BeyondBossBgm = "res://Sts2BalanceMod/music/beyond_boss.ogg";

  private static readonly PropertyInfo StateProperty =
    typeof(RunManager).GetProperty("State", BindingFlags.NonPublic | BindingFlags.Instance)!;

  private static RunState? GetRunState() => StateProperty?.GetValue(RunManager.Instance) as RunState;

  /// <summary>
  /// 战斗开始 Hook：如果当前遭遇是 TimeEaterBoss，淡入自定义 BGM。
  /// </summary>
  [HarmonyPatch(typeof(Hook), nameof(Hook.BeforeCombatStart))]
  internal static class CombatStartPatch
  {
    public static void Prefix()
    {
      var room = GetRunState()?.CurrentRoom as CombatRoom;
      if (room?.Encounter is TimeEaterBoss)
      {
        Sts2ModAudio.FadeIn(BeyondBossBgm, 1.5f);
      }
    }
  }

  /// <summary>
  /// 战斗房间退出时停止 MOD 自定义 BGM，避免残留。
  /// </summary>
  [HarmonyPatch(typeof(CombatRoom), nameof(CombatRoom.Exit))]
  internal static class CleanupBgmOnCombatExitPatch
  {
    [HarmonyPostfix]
    private static void Postfix() => Sts2ModAudio.StopMusic();
  }
}

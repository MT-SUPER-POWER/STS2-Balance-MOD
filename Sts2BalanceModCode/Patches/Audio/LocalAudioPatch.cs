using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Audio;
using Sts2BalanceMod.Sts2BalanceModCode.Utility;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Audio;

/// <summary>
/// 本地 res:// 音乐桥接补丁。
/// 输入：NRunMusicController 收到的自定义音乐路径。
/// 输出：让 MOD 内 OGG 音乐绕过 FMOD event 检查，改用 Godot 音频播放器播放。
/// </summary>
[HarmonyPatch(typeof(NRunMusicController))]
public static class LocalAudioPatch
{
  [HarmonyPatch(nameof(NRunMusicController.PlayCustomMusic))]
  [HarmonyPrefix]
  private static bool PlayCustomMusicPrefix(NRunMusicController __instance, string customMusic)
  {
    if (!customMusic.StartsWith("res://Sts2BalanceMod/bgm/"))
      return true;

    var proxy = (Node?)AccessTools.Field(typeof(NRunMusicController), "_proxy").GetValue(__instance);
    proxy?.Call("stop_music");
    Sts2ModAudio.PlayMusic(customMusic);
    return false;
  }

  [HarmonyPatch(nameof(NRunMusicController.StopCustomMusic))]
  [HarmonyPostfix]
  private static void StopCustomMusicPostfix()
  {
    Sts2ModAudio.StopMusic();
  }

  [HarmonyPatch(nameof(NRunMusicController.StopMusic))]
  [HarmonyPostfix]
  private static void StopMusicPostfix()
  {
    Sts2ModAudio.StopMusic();
  }
}

using System.Text.Json;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;

namespace Sts2BalanceMod.src.Patches.Localization;

/// <summary>
/// 将 MOD 的 enchantments.json 翻译注入到 LocManager 的 enchantments 表中。
/// 与 MonsterLocalizationInjectionPatch 同理，游戏不会自动加载这个独立本地化表。
/// </summary>
[HarmonyPatch(typeof(LocManager), nameof(LocManager.GetTable))]
public static class EnchantmentLocalizationInjectionPatch
{
  private static bool _enchantmentsMerged;

  [HarmonyPostfix]
  private static void Postfix(string name, ref LocTable __result)
  {
    if (name != "enchantments" || _enchantmentsMerged)
      return;

    _enchantmentsMerged = true;

    try
    {
      var lang = LocManager.Instance.Language;
      var path = ModAssetPaths.Resource("localization", lang, "enchantments.json");

      if (!Godot.FileAccess.FileExists(path))
        return;

      using var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
      var json = file.GetAsText();
      var translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
      if (translations != null)
        __result.MergeWith(translations);
    }
    catch
    {
      // 静默失败，不阻塞游戏
    }
  }
}

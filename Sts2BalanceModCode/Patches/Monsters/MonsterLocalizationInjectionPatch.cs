using System.Text.Json;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Monsters;

/// <summary>
/// 将 MOD 的 monsters.json 翻译注入到 LocManager 的 monsters 表中。
/// 游戏不会自动加载原版 MonsterModel 使用的本地化表，需要手动注入。
/// </summary>
[HarmonyPatch(typeof(LocManager), nameof(LocManager.GetTable))]
public static class MonsterLocalizationInjectionPatch
{
    private static bool _monstersMerged;

    [HarmonyPostfix]
    private static void Postfix(string name, ref LocTable __result)
    {
        if (name != "monsters" || _monstersMerged)
            return;

        _monstersMerged = true;

        try
        {
            var lang = LocManager.Instance.Language;
            var path = ModAssetPaths.Resource("localization", lang, "monsters.json");

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

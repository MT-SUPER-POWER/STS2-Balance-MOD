using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Modding;

namespace Sts2BalanceMod.Sts2BalanceModCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
  public const string ModId = "Sts2BalanceMod"; //Used for resource filepath
  public const string ResPath = $"res://{ModId}";

  public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

  public static void Initialize()
  {
    // If you want to use scripts defined in your mod for Godot scenes, uncomment the following line.
    // Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(Assembly.GetExecutingAssembly());

    // ======================== CARD POOL REGISTRATION ========================
    /*     ModHelper.AddModelToPool<IroncladCardPool, DeathReap>();
        ModHelper.AddModelToPool<IroncladCardPool, PowerThought>();
        ModHelper.AddModelToPool<SilentCardPool, Concentrate>();
        ModHelper.AddModelToPool<DefectCardPool, Electrodynamics>(); */

    Harmony harmony = new(ModId);
    harmony.PatchAll();
  }
}

// ======================== DIAGNOSTIC ========================

/// <summary>
/// 联机诊断：在 ModelDb 完成所有注册后，打印模型注册表摘要
/// 联机双方对比日志中 Total 数量即可判断序列化表是否一致
/// </summary>
[HarmonyPatch(typeof(ModelDb), "InitIds")]
internal static class ModelDbInitIdsPatch
{
  [HarmonyPostfix]
  private static void LogRegisteredCounts()
  {
    var modAssembly = typeof(MainFile).Assembly;

    // ---- 本 mod 注册的模型 ----
    var modCards = ModelDb.AllCards
        .Where(c => c.GetType().Assembly == modAssembly)
        .ToList();
    var modPowers = ModelDb.AllPowers
        .Where(p => p.GetType().Assembly == modAssembly)
        .ToList();
    var modRelics = ModelDb.AllRelics
        .Where(r => r.GetType().Assembly == modAssembly)
        .ToList();

    // ---- 全局总数（先 ToList 再 Count，避免 lazy eval 问题） ----
    var allCardsList = ModelDb.AllCards.ToList();
    var allPowersList = ModelDb.AllPowers.ToList();
    var allRelicsList = ModelDb.AllRelics.ToList();

    MainFile.Logger.Info("========== MODEL DB REGISTRATION ==========");

    // NOTE: 联机双方此项必须一致，否则序列化表不同步
    MainFile.Logger.Info($"Total Cards : {allCardsList.Count}");
    MainFile.Logger.Info($"Total Powers: {allPowersList.Count}");
    MainFile.Logger.Info($"Total Relics: {allRelicsList.Count}");

    MainFile.Logger.Info("--- Sts2BalanceMod models ---");
    foreach (var card in modCards.OrderBy(c => c.Id.Entry))
      MainFile.Logger.Info($"  Card : {card.GetType().Name} (Pool={card.Pool?.GetType().Name})");
    foreach (var power in modPowers.OrderBy(p => p.Id.Entry))
      MainFile.Logger.Info($"  Power: {power.GetType().Name}");
    foreach (var relic in modRelics.OrderBy(r => r.Id.Entry))
      MainFile.Logger.Info($"  Relic: {relic.GetType().Name}");

    MainFile.Logger.Info("============================================");
  }
}

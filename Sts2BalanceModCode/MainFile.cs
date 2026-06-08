using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using Sts2BalanceMod.Sts2BalanceModCode.Cards;

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
    // 将自定义卡牌显式注册到 Ironclad 卡池（必须在游戏初始化前调用）
    ModHelper.AddModelToPool<IroncladCardPool, DeathReap>();
    ModHelper.AddModelToPool<IroncladCardPool, PowerThought>();

    Harmony harmony = new(ModId);
    harmony.PatchAll();
  }
}

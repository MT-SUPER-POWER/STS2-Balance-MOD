using BaseLib.Config;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Modding;
using Sts2BalanceMod.Sts2BalanceModCode.Config;

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
    ModConfigRegistry.Register(ModId, new BalanceModConfig());

    Harmony harmony = new(ModId);
    harmony.PatchAll();
  }
}

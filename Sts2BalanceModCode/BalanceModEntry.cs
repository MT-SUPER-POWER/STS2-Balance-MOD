using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using Sts2BalanceMod.Sts2BalanceModCode.Settings;
using STS2RitsuLib;
using STS2RitsuLib.Interop;

namespace Sts2BalanceMod.Sts2BalanceModCode;

[ModInitializer(nameof(Initialize))]
public partial class BalanceModEntry : Node
{
    public const string ModId = "Sts2BalanceMod";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; private set; } = null!;

    public static void Initialize()
    {
        Logger = RitsuLibFramework.CreateLogger(ModId);
        var assembly = Assembly.GetExecutingAssembly();

        // RitsuLib needs the assembly association before content can be auto-registered.
        ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);
        RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);

        BalanceModSettings.Register();

        new Harmony(ModId).PatchAll(assembly);
        Logger.Info("模组加载完成");
    }
}

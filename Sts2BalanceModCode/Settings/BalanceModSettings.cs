using STS2RitsuLib;
using STS2RitsuLib.Settings;
using STS2RitsuLib.Utils.Persistence;

namespace Sts2BalanceMod.Sts2BalanceModCode.Settings;

/// <summary>
/// User-editable switches for optional balance changes. The model is stored through RitsuLib so the UI and
/// persistence share one small, explicit source of truth.
/// </summary>
public sealed class BalanceModSettings
{
  private const string DataKey = "settings";
  private const string FileName = "balance-mod-settings.json";

  public bool EnableEventLeaveOptions { get; set; } = true;
  public bool EnableInfestedPrismRework { get; set; } = true;

  public static bool EventLeaveOptionsEnabled => Current.EnableEventLeaveOptions;
  public static bool InfestedPrismReworkEnabled => Current.EnableInfestedPrismRework;

  private static BalanceModSettings Current =>
    RitsuLibFramework.GetDataStore(BalanceModEntry.ModId).Get<BalanceModSettings>(DataKey);

  public static void Register()
  {
    var store = RitsuLibFramework.GetDataStore(BalanceModEntry.ModId);
    store.Register<BalanceModSettings>(
      DataKey,
      FileName,
      SaveScope.Global,
      defaultFactory: static () => new BalanceModSettings(),
      autoCreateIfMissing: true);

    RitsuLibFramework.RegisterModSettings(BalanceModEntry.ModId, page => page
      .WithTitle(ModSettingsText.Literal("平衡调整 Mod"))
      .WithModDisplayName(ModSettingsText.Literal("平衡调整 Mod"))
      .AddSection("optional-changes", section => section
        .WithTitle(ModSettingsText.Literal("可选调整"))
        .AddToggle(
          "event-leave-options",
          ModSettingsText.Literal("启用事件离开选项"),
          new ModSettingsValueBinding<BalanceModSettings, bool>(
            BalanceModEntry.ModId,
            DataKey,
            SaveScope.Global,
            settings => settings.EnableEventLeaveOptions,
            (settings, value) => settings.EnableEventLeaveOptions = value))
        .AddToggle(
          "infested-prism-rework",
          ModSettingsText.Literal("启用感染棱镜重做"),
          new ModSettingsValueBinding<BalanceModSettings, bool>(
            BalanceModEntry.ModId,
            DataKey,
            SaveScope.Global,
            settings => settings.EnableInfestedPrismRework,
            (settings, value) => settings.EnableInfestedPrismRework = value))),
      pageId: "settings");
  }
}

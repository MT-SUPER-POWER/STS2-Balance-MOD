using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib;
using STS2RitsuLib.Data;
using STS2RitsuLib.Settings;
using STS2RitsuLib.Utils.Persistence;

namespace Sts2BalanceMod.Sts2BalanceModCode.Settings;

public sealed class BalanceModSettings
{
  private const string DataKey = "settings";
  private const string FileName = "balance-mod-settings.json";
  // Retained only for migration from the original three settings.
  public bool EnableEventLeaveOptions { get; set; } = true;
  public bool EnableInfestedPrismRework { get; set; } = true;
  public bool EnableDelicateDollCrossover { get; set; } = true;
  public Dictionary<string, bool> Choices { get; set; } = new();

  private static IReadOnlyDictionary<string, bool> _active = new Dictionary<string, bool>();
  private static int _filter;
  internal static IReadOnlyDictionary<string, bool> Active => _active;
  public static bool IsEnabled(string id) => BalanceCatalog.Effective(_active, id);
  public static bool InfestedPrismReworkEnabled => IsEnabled("M02");
  public static bool DelicateDollEnabled => IsEnabled("E14");
  private static ModDataStore Store => RitsuLibFramework.GetDataStore(BalanceModEntry.ModId);
  private static BalanceModSettings Current => Store.Get<BalanceModSettings>(DataKey);
  private static bool Selected(string id) => Current.Choices.GetValueOrDefault(id, true);
  private static bool Pending(string id) => BalanceCatalog.Pending(Current.Choices, _active, id);
  internal static bool NeedsRestart => BalanceCatalog.All.Any(c => Pending(c.Id));

  internal void Migrate()
  {
    Choices ??= new();
    BalanceCatalog.Migrate(Choices, EnableEventLeaveOptions, EnableInfestedPrismRework, EnableDelicateDollCrossover);
  }

  public static void Register()
  {
    Store.Register<BalanceModSettings>(DataKey, FileName, SaveScope.Global,
      defaultFactory: static () => new(), autoCreateIfMissing: true);
    Current.Migrate();
    _active = new Dictionary<string, bool>(Current.Choices);
    Store.Save(DataKey);
    RitsuLibFramework.RegisterModSettings(BalanceModEntry.ModId, page =>
    {
      page.WithTitle(Text("平衡调整 Mod")).WithModDisplayName(Text("平衡调整 Mod"));
      page.AddSection("overview", section =>
      {
        section.WithTitle(Text("改动配置"));
        section.AddParagraph("status", ModSettingsText.Dynamic(() => NeedsRestart
          ? "配置已修改，重启游戏后生效。当前对局继续使用本次启动配置。"
          : "配置已生效。"));
        section.AddButton("restart", Text("应用配置"), Text("保存并重启游戏"), Restart);
        section.WithEntryVisibleWhen("restart", () => NeedsRestart);
        section.WithEntryEnabledWhen("restart", CanRestart);
        section.AddParagraph("restart-help", Text("立即重启仅限主菜单；请先退出对局或房间，再从主菜单打开设置。"));
        AddBulk(section, "all", BalanceCatalog.All);
        section.AddButton("defaults", Text("默认配置"), Text("恢复默认（全部开启）"),
          (IModSettingsUiActionHost host) => SetMany(BalanceCatalog.All, true, host));
        section.AddButton("filter", Text("列表筛选"), ModSettingsText.Dynamic(() => new[] { "全部", "当前生效", "待重启" }[_filter]),
          (IModSettingsUiActionHost host) => { _filter = (_filter + 1) % 3; host.RequestRefresh(); });
      });
      int index = 0;
      foreach (var group in BalanceCatalog.All.GroupBy(c => c.Group))
      {
        string groupId = $"group-{index++}";
        page.AddSection(groupId, section =>
        {
          var entries = group.ToArray();
          section.WithTitle(ModSettingsText.Dynamic(() => $"{group.Key} · {Summary(entries)}"));
          AddBulk(section, groupId, entries);
          foreach (var subgroup in entries.GroupBy(c => c.Subgroup))
          {
            if (subgroup.Key.Length > 0)
            {
              section.AddParagraph($"sub-{subgroup.First().Id}", ModSettingsText.Dynamic(() => $"{subgroup.Key} · {Summary(subgroup)}"));
              AddBulk(section, $"sub-{subgroup.First().Id}", subgroup);
            }
            foreach (BalanceChange change in subgroup)
            {
              section.AddToggle(change.Id, ModSettingsText.Dynamic(() => $"{change.Label} · {State(change.Id)}"),
                new ModSettingsValueBinding<BalanceModSettings, bool>(BalanceModEntry.ModId, DataKey, SaveScope.Global,
                  s => s.Choices.GetValueOrDefault(change.Id, true), (s, value) => s.Choices[change.Id] = value),
                Text(change.Description));
              section.WithEntryVisibleWhen(change.Id, () => _filter == 0 || (_filter == 1 ? IsEnabled(change.Id) : Pending(change.Id)));
            }
          }
        });
      }
    }, pageId: "settings");
  }

  private static ModSettingsText Text(string value) => ModSettingsText.Literal(value);
  private static string State(string id)
  {
    string state = Pending(id) ? (BalanceCatalog.Effective(Current.Choices, id) ? "待启用" : "待关闭") : (IsEnabled(id) ? "已启用" : "已关闭");
    if (id == "R04" && Selected(id) && !BalanceCatalog.Effective(Current.Choices, id))
      state = (Pending(id) ? "待重启" : "暂不生效") + "（需要至少开启一个红面具事件）";
    return state;
  }
  private static string Summary(IEnumerable<BalanceChange> changes)
  {
    var items = changes.ToArray();
    int selected = items.Count(c => Selected(c.Id));
    string state = selected == 0 ? "全部关闭" : selected == items.Length ? "全部开启" : "部分开启";
    return $"{state} {selected}/{items.Length} · 当前生效 {items.Count(c => IsEnabled(c.Id))} · 待重启 {items.Count(c => Pending(c.Id))}";
  }
  private static void AddBulk(ModSettingsSectionBuilder section, string id, IEnumerable<BalanceChange> changes)
  {
    var items = changes.ToArray();
    section.AddButton(id + "-on", Text("快速操作"), Text("全开"),
      (IModSettingsUiActionHost host) => SetMany(items, true, host));
    section.AddButton(id + "-off", Text("快速操作"), Text("全关"),
      (IModSettingsUiActionHost host) => SetMany(items, false, host));
  }
  private static void SetMany(IEnumerable<BalanceChange> changes, bool value, IModSettingsUiActionHost host)
  {
    foreach (BalanceChange c in changes) Current.Choices[c.Id] = value;
    Store.Save(DataKey);
    host.RequestRefreshAfterDataModelBatchChange();
  }

  private static bool CanRestart()
  {
    if (OS.HasFeature("editor") || RunManager.Instance.IsInProgress ||
        ModSettingsHostSurfaceResolver.ResolveCurrent() != ModSettingsHostSurface.MainMenu)
      return false;
    // WARNING: verified NSubmenuStack keeps hidden parent screens in _submenus.
    // Allow only the settings chain; lobby/setup/join screens beneath it prevent restart.
    var stack = NGame.Instance?.MainMenu?.SubmenuStack;
    if (stack == null) return false;
    return AccessTools.Field(typeof(NSubmenuStack), "_submenus")?.GetValue(stack) is IEnumerable<NSubmenu> screens && screens.All(s => s.GetType().Name.Contains("Settings", StringComparison.Ordinal));
  }
  private static void Restart()
  {
    if (!NeedsRestart || !CanRestart()) return;
    Store.Save(DataKey); // Must succeed before arming restart.
    OS.SetRestartOnExit(true, OS.GetCmdlineArgs());
    try { NGame.Instance!.Quit(); }
    catch { OS.SetRestartOnExit(false); throw; }
  }
}

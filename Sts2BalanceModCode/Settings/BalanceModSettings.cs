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
      page.WithTitle(Text("平衡调整 Mod")).WithModDisplayName(Text("平衡调整 Mod"))
        .WithDescription(Text("展开分区调整内容；修改后重启游戏生效。"));
      page.AddSection("overview", section =>
      {
        section.WithTitle(Text("浏览与应用"));
        section.AddChoice("filter", Text("显示内容"),
          new ModSettingsCallbackValueBinding<int>(BalanceModEntry.ModId, "view-filter", SaveScope.Global,
            () => _filter, value => _filter = value, () => { }),
          new[]
          {
            new ModSettingsChoiceOption<int>(0, Text("全部改动")),
            new ModSettingsChoiceOption<int>(1, Text("当前生效")),
            new ModSettingsChoiceOption<int>(2, Text("待重启"))
          }, presentation: ModSettingsChoicePresentation.Dropdown);
        section.AddParagraph("status", ModSettingsText.Dynamic(() => NeedsRestart
          ? $"有 {BalanceCatalog.All.Count(c => Pending(c.Id))} 项改动待重启，当前游戏仍使用原配置。"
          : _filter == 2 ? "没有待重启的改动。" : "配置已生效。展开下方分区，或从左侧直接定位。"));
        section.AddButton("restart", Text("应用改动"), Text("保存并重启"), Restart);
        section.WithEntryVisibleWhen("restart", () => NeedsRestart);
        section.WithEntryEnabledWhen("restart", CanRestart);
        section.AddParagraph("restart-help", Text("请先退出对局或房间，再从主菜单重启。"));
        section.WithEntryVisibleWhen("restart-help", () => NeedsRestart && !CanRestart());
        section.AddParagraph("empty", Text("没有符合筛选条件的内容。可切换到「全部改动」。"));
        section.WithEntryVisibleWhen("empty", () => _filter == 1 && !BalanceCatalog.All.Any(c => Visible(c.Id)));
      });
      int index = 0;
      foreach (var group in BalanceCatalog.All.GroupBy(c => c.Group))
      {
        string groupId = $"group-{index++}";
        page.AddSection(groupId, section =>
        {
          var entries = group.ToArray();
          section.WithTitle(Text(group.Key)).Collapsible(true);
          section.WithVisibleWhen(() => entries.Any(c => Visible(c.Id)));
          foreach (var subgroup in entries.GroupBy(c => c.Subgroup))
          {
            if (subgroup.Key.Length > 0)
            {
              string subgroupId = $"sub-{subgroup.First().Id}";
              section.AddParagraph(subgroupId, Text(subgroup.Key));
              section.WithEntryVisibleWhen(subgroupId, () => subgroup.Any(c => Visible(c.Id)));
            }
            foreach (BalanceChange change in subgroup)
            {
              section.AddToggle(change.Id, Text(change.Label),
                new ModSettingsValueBinding<BalanceModSettings, bool>(BalanceModEntry.ModId, DataKey, SaveScope.Global,
                  s => s.Choices.GetValueOrDefault(change.Id, true), (s, value) => s.Choices[change.Id] = value),
                ModSettingsText.Dynamic(() => $"{State(change.Id)} · {change.Description}"));
              section.WithEntryVisibleWhen(change.Id, () => Visible(change.Id));
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
  private static bool Visible(string id) => _filter == 0 || (_filter == 1 ? IsEnabled(id) : Pending(id));

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

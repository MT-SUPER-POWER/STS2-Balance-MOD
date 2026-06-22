# MindBloom 自定义 Boss 遭遇 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 解决 MindBloom 事件 Boss 战被系统当作最终 Boss 导致直接通关的问题

**Architecture:** 新建一个 `RoomType.Monster` 的自定义遭遇类 `MindBloomBossEncounter`，通过委托方式从一层 Boss 池抽选实际 Boss 并复制其怪物；`MindBloom.Fight()` 改用此遭遇，`GetFightBosses()` 移除 `HasSeenEncounter` 过滤。

**Tech Stack:** C# / Harmony / STS2 Mod

---
### Task 1: 新建 MindBloomBossEncounter 遭遇类

**Files:**
- Create: `Sts2BalanceModCode/Encounters/MindBloomBossEncounter.cs`

**Interfaces:**
- Consumes: `EncounterModel.MonstersWithSlots`, `EncounterModel.AllPossibleMonsters`
- Produces: `MindBloomBossEncounter` (RoomType.Monster, 动态委托怪物生成)

- [ ] **Step 1: 创建 MindBloomBossEncounter.cs**

```csharp
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;

namespace Sts2BalanceMod.Sts2BalanceModCode.Encounters;

/// <summary>
/// 心灵绽放专属遭遇 —  RoomType 为 Monster，避免被系统当作 Boss 战触发换幕。
/// 怪物/槽位从实际抽选的一层 Boss 遭遇委托生成。
/// </summary>
public sealed class MindBloomBossEncounter : Sts2EncounterModel
{
    private EncounterModel? _bossEncounter;

    /// <summary>
    /// 关键：RoomType 不为 Boss，让奖励界面不走换幕逻辑。
    /// </summary>
    public override RoomType RoomType => RoomType.Monster;

    /// <summary>
    /// 设置实际要打的 Boss 遭遇引用。
    /// </summary>
    public void SetBoss(EncounterModel boss)
    {
        _bossEncounter = boss;
    }

    public override IEnumerable<MonsterModel> AllPossibleMonsters =>
        _bossEncounter?.AllPossibleMonsters ?? [];

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
    {
        return _bossEncounter?.MonstersWithSlots
            .Select(m => (m.Item1.ToMutable(), m.Item2))
            .ToList() ?? [];
    }
}
```

### Task 2: 修改 MindBloom.Fight() 和 GetFightBosses()

**Files:**
- Modify: `Sts2BalanceModCode/Events/MindBloom.cs`

- [ ] **Step 1: 修改 `GetFightBosses()` — 移除 HasSeenEncounter 过滤**

```csharp
private static IReadOnlyList<EncounterModel> GetFightBosses(Player owner)
{
    var firstAct = owner.RunState.Acts.FirstOrDefault();
    if (firstAct is not (Overgrowth or Underdocks))
        return [];

    // 从本层全量 Boss 池真随机选，不过滤"已遭遇"
    return firstAct.AllBossEncounters.ToList();
}
```

- [ ] **Step 2: 修改 `Fight()` — 改用 MindBloomBossEncounter**

```csharp
private Task Fight()
{
    var owner = Owner;
    if (owner == null || Rng == null)
        return Task.CompletedTask;

    var bosses = GetFightBosses(owner);
    if (bosses.Count == 0)
        return Task.CompletedTask;

    // 从一层 Boss 池真随机选一个
    var bossEncounter = Rng.NextItem(bosses).ToMutable();
    bossEncounter.GenerateMonstersWithSlots(owner.RunState);

    // 创建自定义遭遇（RoomType = Monster）
    var mindBloomEncounter = ModelDb.Encounter<MindBloomBossEncounter>().ToMutable();
    mindBloomEncounter.SetBoss(bossEncounter);

    var rareRelic = RelicFactory.PullNextRelicFromFront(owner, RelicRarity.Rare)?.ToMutable();
    if (rareRelic == null)
        return Task.CompletedTask;

    var rewards = new List<Reward>
    {
        new GoldReward(FightGold, owner),
        new RelicReward(rareRelic, owner),
    };
    CombatActive = true;
    EnterCombatWithoutExitingEvent(mindBloomEncounter, rewards, false);
    return Task.CompletedTask;
}
```

- [ ] **Step 3: 移除旧的 using** — 检查 `MindBloom.cs` 中不再需要的 `using MegaCrit.Sts2.Core.Factories;`（如果 `GetFightBosses` 不再使用 `owner.UnlockState`），确认编译通过。

### Task 3: 编译与验证

**Files:**
- N/A（运行命令）

- [ ] **Step 1: Build**

```bash
dotnet build
```

- [ ] **Step 2: 提交**

```bash
git add Sts2BalanceModCode/Encounters/MindBloomBossEncounter.cs Sts2BalanceModCode/Events/MindBloom.cs
git commit -m "fix(event): MindBloom 使用自定义 Monster 遭遇避免误触发换幕

- 新建 MindBloomBossEncounter（RoomType.Monster），委托给实际 Boss 生成怪物
- GetFightBosses() 移除 HasSeenEncounter 过滤，改为全 Boss 池真随机
- 奖励继续由现有 MindBloomCombatPatch 管控"
```

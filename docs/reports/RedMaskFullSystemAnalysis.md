# 红面具事件完整流程 — 分层技术分析报告

> 报告日期：2026-06-18  
> 基于提交：`2b6c031 feat: 红面具强盗完整流程`

## 总览

本报告按"层级"解析红面具（Red Mask）完整事件链的实现。从底层数据模型往上，依次覆盖六个层级，每个层级包含技术分析、与参考代码的对比，以及设计决策说明。

```
┌──────────────────────────────────────────────┐
│ 第 6 层  本地化文本层 (Localization Layer)    │
│   events.json + encounters.json              │
├──────────────────────────────────────────────┤
│ 第 5 层  遗物池补丁层 (Relic Pool Patches)    │
│   RedMaskEventOnlyPatch.cs                   │
├──────────────────────────────────────────────┤
│ 第 4 层  事件注入层 (Event Injection Layer)   │
│   Sts1EventInjectionPatch.cs                 │
│   HiveEncounterInjectionPatch.cs             │
│   CustomEventPortraitPatch.cs                │
├──────────────────────────────────────────────┤
│ 第 3 层  遭遇战定义层 (Encounter Layer)       │
│   RedMaskBandits.cs (Encounter)              │
├──────────────────────────────────────────────┤
│ 第 2 层  事件逻辑层 (Event Logic Layer)       │
│   MaskedBandits.cs + TombOfLordRedMask.cs    │
├──────────────────────────────────────────────┤
│ 第 1 层  怪物/遗物实体层 (Entity Layer)       │
│   Pointy.cs + Romeo.cs + Bear.cs (Monsters)  │
│   RedMask.cs (二代原生 Relic)                │
└──────────────────────────────────────────────┘
```

---

## 第 1 层：怪物/遗物实体层

本层定义所有被高层逻辑引用的"原子实体"——三个强盗怪物和一枚红面具遗物。

### 1.1 怪物实体

| 怪物    | 类名   | 对应一代角色     | 行为特征                                      |
| ------- | ------ | ---------------- | --------------------------------------------- |
| 尖头    | Pointy | STS1 Pointy      | 主输出，高攻击力                              |
| 罗密欧  | Romeo  | STS1 Romeo       | 中等攻击，有特殊台词                          |
| 熊      | Bear   | STS1 Bear        | 肉盾，Romeo 台词中的"壮汉"                    |

三个怪物直接复用一代的 TSCN 场景资源，通过 `Sts2MonsterModel` 基类注册到 `ModelDb`。基类自动处理：
- Spine 骨骼动画路径映射
- 图鉴（Compendium）显示
- 属性（HP/攻击力）数据注入

**代码位置**：`Sts2BalanceModCode/Monsters/Pointy.cs`, `Romeo.cs`, `Bear.cs`

### 1.2 RedMask 遗物（二代原生）

```csharp
// D:\Game\Godot\StS2-Code\src\Core\Models\Relics\RedMask.cs
public sealed class RedMask : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Common;   // 原始稀有度：Common（常见）
    // 效果：战斗首回合对所有敌人施加 1 层虚弱
}
```

**关键点**：二代原生 `RedMask` 是 Common 稀有度，会出现在共享遗物池中正常掉落。MOD 通过第 5 层的补丁将其改为 Event 稀有度，使其仅通过事件获得。

---

## 第 2 层：事件逻辑层

两个事件类构成红面具故事线的核心流程控制。

### 2.1 MaskedBandits（面具强盗事件）— 第 2 幕城市

**文件**：`Sts2BalanceModCode/Events/MaskedBandits.cs`

#### 触发条件 (`IsAllowed`)

```
第 2 幕（CurrentActIndex == 1） AND 总层数 >= 23 AND 未持有 RedMask
```

| 条件          | 含义                          | 设计意图                     |
| ------------- | ----------------------------- | ---------------------------- |
| ActIndex = 1  | 第 2 幕"城市"                 | 对应一代 City 阶段           |
| TotalFloor≥23 | 至少走过 23 层                | 玩家有足够的金币可以交       |
| 无 RedMask    | 不能在有面具时再遇到          | 避免重复获得导致逻辑混乱     |

#### 流程分支

```
                    ┌─► Pay() ─── 失去所有金币
                    │       │
INITIAL ─ 玩家选择 ─┤       ├─► Paid2() ─── Romeo 台词 [语音气泡]
                    │       │       │
                    │       │       └─► Paid3() ─── 笑声，事件结束
                    │       │
                    └─► Fight() ─── 进入战斗 RedMaskBandits
                                      (不需要离开展开窗口)
```

#### 关键设计决策

**1. 语音气泡系统（新增功能）**

原作仅在战斗结束后通过 Patches 在大地图关闭时显示彩蛋台词。MOD 版本新增了 **战斗中对话气泡**：

```csharp
private void PlayPaidLine<TMonster>(string pageKey) where TMonster : MonsterModel
{
    // 仅在本地玩家的回合中显示
    if (!LocalContext.IsMe(Owner)) return;

    // 先关闭旧气泡
    if (_speechBubble != null) { _ = _speechBubble.AnimOut(); _speechBubble = null; }

    // 在对应怪物身上创建永久气泡
    var speaker = FindCreature<TMonster>();
    _speechBubble = TalkCmd.Play(PageDescription(pageKey), speaker, VfxColor.Red, VfxDuration.Forever);
}
```

**实现原理**：
- 使用 `TalkCmd.Play()` 直接在战斗中创建怪物头顶的气泡 VFX
- 泛型方法 `FindCreature<TMonster>()` 通过遍历 `NCombatRoom.Instance.CreatureNodes` 在战斗场景中定位到正确的怪物实体
- `VfxDuration.Forever` 让气泡持续显示直到玩家点击下一步
- `LocalContext.IsMe(Owner)` 确保多人游戏中只有对应玩家的 UI 才显示

**台词分配**：
| 阶段    | 说话者   | 内容                                          |
| ------- | -------- | --------------------------------------------- |
| PAID_1  | Pointy   | "嘿嘿嘿…谢谢你的金币啦！"                     |
| PAID_2  | Romeo    | "喂，熊，这家伙把金币全给我们了！太蠢了吧？"   |
| PAID_3  | Romeo    | "小的们，我们一起笑一笑！"                    |

**2. EmptyDescription（空描述文本）**

原作使用 `PageDescription("PAID_1")` 等方式，内容写在本地化文件中。MOD 统一使用 `EmptyDescription`（一个空白的 LocString），因为所有对话都通过语音气泡显示，事件窗口不需要展示重复的描述文本。

```csharp
private static readonly LocString EmptyDescription =
    new("events", "STS2BALANCEMOD-MASKED_BANDITS.pages.EMPTY.description");
```

**3. Fight 分支 — 内嵌战斗**

```csharp
private Task Fight()
{
    var redMaskRelic = ModelDb.Relic<RedMask>().ToMutable();
    var rewards = new List<Reward>
    {
        new GoldReward(25, 35, owner),         // 战斗胜利奖励 25-35 金币
        new RelicReward(redMaskRelic, owner),   // 战斗胜利奖励 RedMask 遗物
    };
    EnterCombatWithoutExitingEvent<RedMaskBandits>(rewards, false);
    //                                   ^^^^^^^^^^^^^^^^
    //                                   不退出事件 UI，在事件窗口内完成战斗
}
```

**核心 API**：`EnterCombatWithoutExitingEvent<T>` 
- 在事件 UI 内启动战斗
- 战斗结束后自动回到事件流程
- 奖励通过 `rewards` 列表传递，使用 `ToMutable()` 创建遗物的可变副本

### 2.2 TombOfLordRedMask（红面具大人之墓）— 第 3 幕深渊

**文件**：`Sts2BalanceModCode/Events/TombOfLordRedMask.cs`

#### 触发条件

```
第 3 幕（CurrentActIndex == 2） AND 未持有 RedMask
```

**注意**：TotalFloor 无需额外检查，因为第 3 幕本身已经足够深层。

#### 流程分支

```
                    ┌─► WearMask（已持有面具时）── 获得 222 金币
                    │
INITIAL ─ 玩家选择 ─┼─► PayRespects（未持有面具时）── 失去所有金币 → 获得 RedMask
                    │
                    └─► Leave ─── 无事发生
```

#### 选项设计

```csharp
protected override IReadOnlyList<EventOption> GenerateInitialOptions()
{
    var options = new List<EventOption>();

    if (owner.Relics.Any(r => r is RedMask))
    {
        // 已持有红面具 → 可以戴着获得金币
        options.Add(Option(WearMask));
    }
    else
    {
        // 未持有 → 显示 Wearing 选项为 Locked
        options.Add(new EventOption(this, null,
            $"{Id.Entry}.pages.INITIAL.options.WEAR_MASK_LOCKED",
            Array.Empty<IHoverTip>()));

        // NOTE: 二代原生 RedMask 的 HoverTip 在 Mod 事件内会触发能量图标池解析异常
        options.Add(Option(PayRespects));
    }

    options.Add(Option(Leave));
    return options;
}
```

**设计要点**：
1. **动态选项**：根据是否持有 RedMask 展示不同选项
2. **Locked 选项**：使用 `new EventOption(this, null, "LOCKED", ...)` 创建灰色不可选条目，展示给玩家"你缺什么"
3. **HoverTip 兼容问题**：二代原生 `RedMask` 的 HoverTip 通过 `FromPower<WeakPower>()` 生成，在 MOD 事件上下文内会触发能量图标池解析异常（因为事件场景缺少战斗上下文）。**临时方案**：去掉 `HoverTipFactory.FromRelic()`，只展示文本。

#### DynamicVar 系统

```csharp
protected override IEnumerable<DynamicVar> CanonicalVars => [
    new GoldVar(GoldAmount),         // {Gold} → 222
    new IntVar("PlayerGold", 0),     // {PlayerGold} → 动态值
];

public override void CalculateVars()
{
    DynamicVars["PlayerGold"].BaseValue = Owner?.Gold ?? 0;
}
```

`CalculateVars()` 在选项展示前调用，确保 `{PlayerGold}` 在文本中显示为玩家当前实际金币数。

#### 与参考代码的差异

| 特性             | 参考（AFTP）                         | MOD 实现                         |
| ---------------- | ------------------------------------ | -------------------------------- |
| HoverTip         | 完整显示 RedMask 遗物悬停提示        | 因兼容问题移除了 HoverTip        |
| RebalancedMode   | 额外"逃跑"分支，可给技能牌附魔 Fearful | 未移植                          |
| 持有面具检测     | `Owner.Relics.Any(r => r is RedMask)` | 相同                             |
| Acts             | `new[] { ModelDb.Act<TheBeyondAct>() }` | `[]`（不限制，由 IsAllowed 控制） |
| 获得面具奖励     | `await RelicCmd.Obtain(redMask, Owner)` | 相同                              |

---

## 第 3 层：遭遇战定义层

**文件**：`Sts2BalanceModCode/Encounters/RedMaskBandits.cs`

红面具强盗战斗专门定义为一个独立遭遇，不纳入任何正常遭遇池。

```csharp
public sealed class RedMaskBandits : Sts2EncounterModel
{
    public override RoomType RoomType => RoomType.Monster;
    public override bool HasScene => true;
    public override string CustomScenePath =>
        "res://Assets/ActsFromPast/scenes/encounters/actsfromthepast-red_mask_bandits_event.tscn";

    // 三个战斗位
    public override IReadOnlyList<string> Slots => ["pointy", "romeo", "bear"];

    // 所有可能出现的怪物（图鉴注册用）
    public override IEnumerable<MonsterModel> AllPossibleMonsters => [
        ModelDb.Monster<Pointy>(),
        ModelDb.Monster<Romeo>(),
        ModelDb.Monster<Bear>(),
    ];

    // 实际生成的怪物列表
    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() => [
        (ModelDb.Monster<Pointy>().ToMutable(), "pointy"),
        (ModelDb.Monster<Romeo>().ToMutable(), "romeo"),
        (ModelDb.Monster<Bear>().ToMutable(), "bear"),
    ];
}
```

### 继承链

```
EncounterModel (game) → Sts2EncounterModel (MOD 抽象基类)
```

`Sts2EncounterModel` 提供：
- `BossNodeSpineResource => null`（非 Boss 遭遇）
- `CustomScenePath` 虚属性（可由子类覆写加载自定义 TSCN）

### 关键设计

**1. 非注册遭遇**

不实现 `IsValidForAct()`（或返回 `false`）。此遭遇仅通过 `MaskedBandits.Fight()` → `EnterCombatWithoutExitingEvent<T>()` 触发。这意味着它 **不会** 出现在任何 Act 的随机遭遇池中。

**2. CustomScenePath**

指向 Assets 子模块（`ActsFromPast`）中的 `.tscn` 场景文件：

```
res://Assets/ActsFromPast/scenes/encounters/actsfromthepast-red_mask_bandits_event.tscn
```

这个 TSCN 包含三个怪物占位节点（对应 `slots: pointy, romeo, bear`），每个节点挂载了对应的 `NCreatureVisuals.cs` 脚本处理 Spine 动画渲染。

### 与参考代码对比

| 维度         | 参考（RedMaskBanditsEvent）                  | MOD（RedMaskBandits）             |
| ------------ | -------------------------------------------- | -------------------------------- |
| 基类         | `CustomEncounterModel`（AFTP 的基类）       | `Sts2EncounterModel`（MOD 基类） |
| 场景路径     | 隐式（游戏自动按 ID 查找）                   | 显式 `CustomScenePath`           |
| IsValidForAct| `return false`                                | 不覆写（基类默认）               |
| Slots        | `["pointy", "romeo", "bear"]`                 | 相同                             |

---

## 第 4 层：事件注入层

三个补丁类负责将自定义事件"塞"进游戏的世界结构中。

### 4.1 Sts1EventInjectionPatch — 事件池注入

**文件**：`Sts2BalanceModCode/Patches/Events/Sts1EventInjectionPatch.cs`

```
HarmonyPatch → ActModel.GenerateRooms() → Postfix
```

**注入点**：`ActModel` 每次生成房间池时触发。

```csharp
[HarmonyPatch(typeof(ActModel), nameof(ActModel.GenerateRooms))]
[HarmonyPostfix]
private static void Postfix(ActModel __instance, Rng rng)
{
    if (RoomsField?.GetValue(__instance) is not RoomSet rooms) return;

    AddIfMissing(rooms, ModelDb.Event<TombOfLordRedMask>());
    AddIfMissing(rooms, ModelDb.Event<MaskedBandits>());
    // ... 其他事件 ...
}
```

**实现原理**：
1. 通过反射 `typeof(ActModel).GetField("_rooms", BindingFlags.NonPublic | BindingFlags.Instance)` 访问私有字段 `_rooms`
2. 获取 `RoomSet` 对象（各类型房间的集合）
3. 检查 `rooms.events` 列表中是否已存在（防重复注入）
4. 将事件模型追加到 event list 末尾

**注意**：`IsAllowed()` 在事件入池时不被调用——它是运行时随机到该事件时才被检查的。所以事件的 `Acts` 属性（或空数组 `[]`）+ `IsAllowed` 才是实际上控制"此事件应在哪个 Act 出现"的机制。

### 4.2 HiveEncounterInjectionPatch — 遭遇池注入（图鉴用途）

**文件**：`Sts2BalanceModCode/Patches/Encounters/HiveEncounterInjectionPatch.cs`

```
HarmonyPatch → Hive.GenerateAllEncounters() → Postfix
```

```csharp
__result = __result.Append(ModelDb.Encounter<RedMaskBandits>());
```

注入到 Hive（第 2 幕"城市"）的遭遇列表。目的 **不是** 让它能在遭遇池中被随机到（`IsValidForAct` 未实现），而是让 `AllPossibleMonsters` 中的三个怪物出现在 **怪物图鉴（Compendium）** 中。

### 4.3 CustomEventPortraitPatch — 自定义事件肖像

**文件**：`Sts2BalanceModCode/Patches/Events/CustomEventPortraitPatch.cs`

```
HarmonyPatch → EventModel.GetAssetPaths() → Postfix  (资源路径替换)
HarmonyPatch → EventModel.CreateInitialPortrait() → Prefix (预加载纹理)
```

**问题**：MOD 资源位于 `Sts2BalanceMod/images/` 下，而默认事件系统按 `res://images/events/{id}.png` 查找肖像。

**解决**：
1. `GetAssetPaths` Postfix：检测到自定义事件时，将默认路径替换为 MOD 路径
2. `CreateInitialPortrait` Prefix：直接通过 `PreloadManager.Cache.GetTexture2D()` 加载纹理，跳过原生加载流程

```csharp
private static bool TryGetPortraitPath(EventModel eventModel, out string portraitPath)
{
    if (eventModel is not (Augmenter or Cleric or CursedTome or MindBloom
        or TheDivineFountain or TombOfLordRedMask or WheelOfChange))
    {
        portraitPath = string.Empty;
        return false;
    }

    // 路径: events/tomb_of_lord_red_mask.png → 通过 ImagePath() 解析为完整 MOD 资源路径
    portraitPath = $"events/{eventModel.Id.Entry.RemovePrefix().ToLowerInvariant()}.png".ImagePath();
    return true;
}
```

**注意**：`MaskedBandits` 不在肖像补丁列表中，因为它是 `LayoutType.Combat` 事件——战斗类型事件使用遭遇的场景背景而非普通事件肖像。

---

## 第 5 层：遗物池补丁层

**文件**：`Sts2BalanceModCode/Patches/Relics/RedMaskEventOnlyPatch.cs`

三个 Harmony 补丁将 `RedMask` 从"常见掉落"改为"事件专属"。

### 5.1 修改稀有度

```csharp
[HarmonyPatch(typeof(RedMask), "get_Rarity")]
[HarmonyPrefix]
public static bool Prefix(ref RelicRarity __result)
{
    __result = RelicRarity.Event;   // Community → Event
    return false;                    // 跳过原生 getter
}
```

**效果**：任何读取 `RedMask.Rarity` 的代码（如图鉴、池子筛选）现在看到 `Rarity = Event`。

### 5.2 从共享遗物池移除

```csharp
[HarmonyPatch(typeof(SharedRelicPool), "GenerateAllRelics")]
[HarmonyPostfix]
public static RelicModel[] Postfix(RelicModel[] __result)
{
    return __result.Where(r => r is not RedMask).ToArray();
}
```

**为什么需要这个**：`SharedRelicPool` 使用硬编码列表生成，不受 `Rarity` 动态值影响。直接改 Rarity 不够——必须从生成结果中物理移除。

### 5.3 注入事件遗物池

```csharp
[HarmonyPatch(typeof(EventRelicPool), "GenerateAllRelics")]
[HarmonyPostfix]
public static IEnumerable<RelicModel> Postfix(IEnumerable<RelicModel> __result)
{
    return __result.Append(ModelDb.Relic<RedMask>());
}
```

**效果**：`RedMask` 现在出现在"活动遗物"图鉴分类中，与其他事件专属遗物（如 Nexus Crystal）并排显示。

### 数据流总结

```
补丁前: RedMask ∈ SharedRelicPool (Common)   → 可在商店/战斗掉落中获得
补丁后: RedMask ∈ EventRelicPool  (Event)    → 仅可通过事件 MaskedBandits 获得
                                             → 在图鉴中分类为 Event
```

---

## 第 6 层：本地化文本层

### 6.1 事件文本 (events.json)

**文件**：`Sts2BalanceMod/localization/{lang}/events.json`

以 key `STS2BALANCEMOD-MASKED_BANDITS` 和 `STS2BALANCEMOD-TOMB_OF_LORD_RED_MASK` 组织。

**文本层级结构**（以 MaskedBandits 为例）：

```
STS2BALANCEMOD-MASKED_BANDITS
├── .title                              → "面具强盗"
├── .pages.INITIAL.description          → 初始场景描述
├── .pages.INITIAL.options.PAY.title    → "交钱"
├── .pages.INITIAL.options.PAY.description → "失去所有金币。"
├── .pages.INITIAL.options.FIGHT.title  → "开战"
├── .pages.INITIAL.options.FIGHT.description → "与强盗战斗，获得红面具！"
├── .pages.EMPTY.description            → "" (空文本，对话通过气泡显示)
├── .pages.PAID_1.description           → Pointy 的台词（气泡）
├── .pages.PAID_1.options.CONTINUE.title → "继续"
├── .pages.PAID_2.description           → Romeo 台词
├── .pages.PAID_2.options.CONTINUE.title → "继续"
├── .pages.PAID_3.description           → 三人笑声音效描述
└── .pages.PAID.description             → 备用摘要文本
```

**多语言覆盖**：eng（英语）、zhs（中文）、ita（意大利语）— 三个文件全部完整填充。

### 6.2 遭遇文本 (encounters.json)

**文件**：`Sts2BalanceMod/localization/{lang}/encounters.json`

```
STS2BALANCEMOD-RED_MASK_BANDITS.title  → "面具强盗" (图鉴名称)
STS2BALANCEMOD-RED_MASK_BANDITS.loss   → "{name} 被打劫了。" (死亡信息)
```

### 6.3 完整的 Key 组织规则

```
格式：STS2BALANCEMOD-{ID}
      ├── .title                          → 显示名称
      ├── .pages.{PAGE}.description       → 页面描述文本
      ├── .pages.{PAGE}.options.{OPT}.title       → 选项按钮文本
      ├── .pages.{PAGE}.options.{OPT}.description  → 选项悬停提示（HoverTip 文本）
      └── .pages.{PAGE}.selectionScreenPrompt      → 选牌界面提示（部分事件）
```

其中 `{ID}` 由 C# 类名自动推导（`MaskedBandits` → `MASKED_BANDITS`，`TombOfLordRedMask` → `TOMB_OF_LORD_RED_MASK`），并通过 `Id.Entry` 属性在代码中引用。

---

## 完整流程时序图

```
玩家进入第 2 幕 City (ActIndex=1)
│
├─► ActModel.GenerateRooms()
│   └─► Sts1EventInjectionPatch 注入 MaskedBandits + TombOfLordRedMask 到事件池
│
├─► 随机到 MaskedBandits 事件
│   │
│   ├─► IsAllowed(): ActIndex=1 ✓, Floor≥23 ✓, 无 RedMask ✓
│   │
│   ├─► GenerateInitialOptions()
│   │   │
│   │   ├─ [玩家选择 PAY]
│   │   │   ├─► Pay(): LoseGold(全部金币)
│   │   │   ├─► Paid2(): Romeo 气泡 "太蠢了是吧？"
│   │   │   └─► Paid3(): Romeo 气泡 "哈哈哈哈！"
│   │   │
│   │   └─ [玩家选择 FIGHT]
│   │       ├─► Fight(): 创建 GoldReward(25-35) + RelicReward(RedMask)
│   │       └─► EnterCombatWithoutExitingEvent<RedMaskBandits>()
│   │           │
│   │           ├─► 加载 TSCN 场景
│   │           ├─► 生成 Pointy(slot:pointy), Romeo(slot:romeo), Bear(slot:bear)
│   │           ├─► 战斗...
│   │           ├─► 胜利 → 获得 RedMask 遗物
│   │           └─► 事件窗口恢复 → 战斗奖励 UI 弹出
│   │
│   └─► 事件结束
│
├─► 玩家进入第 3 幕 Beyond (ActIndex=2)
│
└─► 随机到 TombOfLordRedMask 事件
    │
    ├─► IsAllowed(): ActIndex=2 ✓, 有 RedMask ✓ (或 无 RedMask ✓)
    │
    ├─► GenerateInitialOptions() [动态]
    │   │
    │   ├─ [有面具] WearMask → 获得 222 金币
    │   ├─ [无面具] PayRespects → 失去所有金币 → 获得 RedMask
    │   └─ 任意     Leave → 无事发生
    │
    └─► 事件结束
```

---

## 设计模式与架构决策

### 1. 组合式 Harmony 补丁

```
Effect:  RedMask 变为 Event 专属
├── Patch 1: Rarity getter → 返回 Event
├── Patch 2: SharedRelicPool → 物理移除
└── Patch 3: EventRelicPool  → 注入
```

三个补丁各司一职，相互独立但组合起来完成完整效果。这比直接修改 `RedMask.cs` 更安全——不从源码级别覆盖游戏文件，避免了 DLL 热替换的版本兼容问题。

### 2. 泛型委托式气泡定位

```csharp
private static Creature? FindCreature<TMonster>() where TMonster : MonsterModel
{
    return NCombatRoom.Instance?.CreatureNodes
        .FirstOrDefault(n => n.Entity.Monster is TMonster)?.Entity;
}
```

利用 C# 泛型类型匹配，一行代码完成"在当前战斗中的怪物集合里找到类型为 T 的实体并返回 Creature 引用"。这种模式适合在事件中与战斗场景交互的场景。

### 3. EmptyDescription 模式

事件窗口需要一个描述参数，但当所有叙事都通过语音气泡展示时，事件窗口本身不需要文本。解决方式是传入一个事先定义好的空 LocString：

```csharp
private static readonly LocString EmptyDescription =
    new("events", "STS2BALANCEMOD-MASKED_BANDITS.pages.EMPTY.description");
// .pages.EMPTY.description → ""（空字符串）
```

### 4. 防御性编程

所有异步方法开头都做了 null 守卫：

```csharp
private async Task Pay()
{
    var owner = Owner;
    if (owner == null) return;
    // ...
}
```

Owner 在事件完全卸载后可能变为 null（垃圾回收竞态），提前判断避免 `NullReferenceException` 污染日志。

### 5. 事件 + 遭遇分离

- `MaskedBandits`（事件） = 控制流程、分支、奖励逻辑
- `RedMaskBandits`（遭遇） = 纯战斗参数：怪物列表、场景路径、槽位

这种分离符合游戏的架构惯例——事件是叙事包装，遭遇是战斗参数包。事件调用 `EnterCombatWithoutExitingEvent<Encounter>()` 启动战斗，遭遇通过 `EncounterModel` 提供战斗参数。

---

## 文件清单

| 文件路径                                                | 功能                           |
| ------------------------------------------------------- | ------------------------------ |
| `Sts2BalanceModCode/Events/MaskedBandits.cs`            | 面具强盗事件逻辑 + 语音气泡     |
| `Sts2BalanceModCode/Events/TombOfLordRedMask.cs`        | 红面具之墓事件逻辑             |
| `Sts2BalanceModCode/Encounters/RedMaskBandits.cs`       | 三人帮遭遇定义（怪物+场景）     |
| `Sts2BalanceModCode/Patches/Events/Sts1EventInjectionPatch.cs` | 事件池注入                |
| `Sts2BalanceModCode/Patches/Encounters/HiveEncounterInjectionPatch.cs` | 遭遇图鉴注入        |
| `Sts2BalanceModCode/Patches/Events/CustomEventPortraitPatch.cs` | 事件肖像路径补丁        |
| `Sts2BalanceModCode/Patches/Relics/RedMaskEventOnlyPatch.cs` | RedMask 稀有度与池子迁移 |
| `Sts2BalanceModCode/Abstract/Sts2EncounterModel.cs`     | 遭遇基类                       |
| `Sts2BalanceMod/localization/{lang}/events.json`        | 事件文本（eng/zhs/ita）        |
| `Sts2BalanceMod/localization/{lang}/encounters.json`    | 遭遇文本（eng/zhs/ita）        |
| `Assets/ActsFromPast/.../red_mask_bandits_event.tscn`   | 战斗场景布局 (GODOT)           |

---

## 关键 API 速查

| API                                          | 用途                                |
| -------------------------------------------- | ----------------------------------- |
| `CustomEventModel.IsAllowed(IRunState)`      | 控制事件在什么条件下可被触发        |
| `CustomEventModel.GenerateInitialOptions()`  | 返回事件初始可选分支列表            |
| `SetEventFinished(description)`              | 结束事件，显示最终文本              |
| `SetEventState(description, options)`        | 过渡到新的选项页面                  |
| `EnterCombatWithoutExitingEvent<T>(rewards)` | 在内嵌模式下启动战斗                |
| `PlayerCmd.LoseGold(amount, owner, type)`    | 扣减玩家金币                        |
| `PlayerCmd.GainGold(amount, owner)`          | 增加玩家金币                        |
| `RelicCmd.Obtain(relic, owner)`              | 给予玩家遗物                        |
| `TalkCmd.Play(text, creature, color, dur)`   | 在怪物头顶创建对话气泡 VFX          |
| `CardPileCmd.Add(card, pile)`                | 添加卡牌到牌堆                      |
| `CardCmd.PreviewCardPileAdd(results, delay)` | 弹窗展示卡牌添加动画                |
| `CardSelectCmd.FromDeckForEnchantment(...)`  | 打开选牌界面，允许玩家选择供附魔的牌 |

---

## 移植总结：从 AFTP 到 Sts2BalanceMod

| 维度         | AFTP (ActsFromThePast)                       | Sts2BalanceMod                                    |
| ------------ | -------------------------------------------- | ------------------------------------------------- |
| 事件框架     | `CustomEventModel`（AFTP 自己的）            | `CustomEventModel`（BaseLib 提供，同一 API）       |
| 遭遇框架     | `CustomEncounterModel`（AFTP 自己的）        | `Sts2EncounterModel`（MOD 抽象基类）              |
| 场景路径     | 游戏自动推断（基于 ID）                      | 显式 `CustomScenePath` 指向 TSCN                   |
| 补丁系统     | `MaskedBanditsPatches`（3 个内嵌类）         | 分散到 3 个独立文件（池注入 / 遭遇注入 / 遗物补丁）|
| 语音气泡     | 大地图关闭时显示彩蛋（仅事件结束后一次）     | 交钱分支中实时战斗内气泡（多个分阶段台词）        |
| RedMask 池   | 依赖 AFTP 自己的配置（RebalancedMode）       | Harmony 三连击：Rarity → SharedPool 移除 → EventPool 注入 |
| 本地化       | 多语言（eng/zho/rus/ita 等）                 | 三语言（eng/zhs/ita）                              |
| Rebalance 分支| 品牌面具获得 HandOfGreed 等                   | 未移植（简化处理）                                  |
| Tomb Fearful  | 逃跑分支可给技能牌附魔                       | 未移植                                              |

# v0.0.7 实现规格说明书 (Spec)

> 基于 `docs/balance-changes.md` 与代码库现状编写的 v0.0.7 具体实现方案。

---

## 1. 前置检查

### git pull 结果
- `Assets/ActsFromPast/ActsFromThePast` 子模块已是最新，无需更新。
- 源代码参考位于 `docs/references/ActsFromThePast/` 已包含所有需要参考的代码。

---

## 2. MON-03 — 时间吞噬者 Time Warp 视觉效果

> 用户纠正："卡表"指 Time Warp 计时到 12 时的相关动画资源。

### 检查结果（已确认）
ActsFromThePast **存在完整的视觉效果实现**：

| 文件                          | 路径                                                                               | 行号      | 用途                                                                      |
| ----------------------------- | ---------------------------------------------------------------------------------- | --------- | ------------------------------------------------------------------------- |
| `TimeWarpTurnEndEffect.cs`    | `docs/references/ActsFromThePast/ActsFromThePast/Effects/TimeWarpTurnEndEffect.cs` | 第5-93行  | **时钟图标弹出动画** — 触发 Time Warp 时从屏幕底部弹入旋转时钟，2秒后淡出 |
| `powers.png` / `powers.atlas` | `Assets/ActsFromThePast/ActsFromThePast/literally_just_here_for_time_warp/`        | —         | 能力图标 atlas，含 `128/time` 区域（时钟图标）                            |
| `TimeWarpPower.cs` (AFTP版)   | `docs/references/ActsFromThePast/ActsFromThePast/Powers/TimeWarpPower.cs`          | 第59-78行 | 参考实现：使用 `DynamicVars` 追踪出牌计数，触发视觉效果                   |

### AFTP 版 TimeWarpPower 差异对比
| 功能       | 我们的实现 (`Powers/TimeWarpPower.cs`)   | AFTP 参考实现                             |
| ---------- | ---------------------------------------- | ----------------------------------------- |
| 出牌计数   | `PowerStackType.Counter` 显示 `Amount`   | `DynamicVars["CardCount"]` 独立显示       |
| 触发后重置 | `Amount - 1`, 到 0 重置为 `CardsPerWarp` | `CardCount` 归零，`Countdown` 保留常量    |
| 视觉动画   | **无**（仅 SFX）                         | `TimeWarpTurnEndEffect.Create()` 时钟弹出 |
| 边框闪光   | 无                                       | `BorderFlashEffect.PlayGold()`            |

### TimeWarpTurnEndEffect 动画详解
- 第24-54行 `Initialize()`：加载 atlas 中 `128/time` 区域的时钟图标，位置从屏幕底部开始
- 第56-87行 `Update(float delta)`：动画三个阶段：
  1. 弹入阶段（`Duration > 1.0f`）：back swing easing 从底部弹入屏幕中央
  2. 淡出阶段（`Duration < 1.0f`）：smooth fade out
  3. 整段持续旋转（`_sprite.Rotation = Duration * Mathf.Pi * 2f`）
- 第83行 `_sprite.Rotation`：时钟匀速旋转

### 实现方案

**步骤 1：拷贝 atlas 资源到 MOD**
- `powers.atlas` → `Sts2BalanceMod/images/powers/time_warp.atlas`
- `powers.png` → `Sts2BalanceMod/images/powers/time_warp.png`

**步骤 2：移植 `TimeWarpTurnEndEffect`**
- 新建 `Sts2BalanceModCode/Effects/TimeWarpTurnEndEffect.cs`
  - 继承 `Node2D`（不依赖 `NSts1Effect`，该基类可能不在游戏源码中）
  - 使用 `LibGdxAtlas.GetRegion` 加载 `128/time`（参考第27行）
  - 如果 `LibGdxAtlas` 不可用，改用 `ResourceLoader.Load + AtlasTexture` 手动实现
  - 核心动画逻辑直接从 AFTP 版移植（弹入 back swing + 旋转 + 淡出）

**步骤 3：修改 `TimeWarpPower.cs`**（第52-59行）
- 在触发 Time Warp 时添加视觉效果：
  ```csharp
  Flash();
  Sts2ModAudio.PlayOneShot(TimeWarpSfx);
  // 新增：时钟弹出动画
  var effect = TimeWarpTurnEndEffect.Create();
  if (NCombatRoom.Instance?.CombatVfxContainer is Node vfxContainer)
      vfxContainer.AddChildSafely(effect);
  BorderFlashEffect.PlayGold();
  ```
- 保留现有的 `Amount` 追踪逻辑

---

## 3. STS1-EVENT-07 — 大转盘自定义 UI

### 检查结果
ActsFromThePast **有完整的自定义转盘 UI 实现**，资源齐全：

| 文件                   | 路径                                                                             | 用途                                                                           |
| ---------------------- | -------------------------------------------------------------------------------- | ------------------------------------------------------------------------------ |
| `NWheelSpinScreen.cs`  | `docs/references/ActsFromThePast/ActsFromThePast/Minigames/NWheelSpinScreen.cs`  | **主 UI** — Godot Control, 覆盖层, 动画 (bounce-in/spin/decelerate/bounce-out) |
| `WheelSpinMinigame.cs` | `docs/references/ActsFromThePast/ActsFromThePast/Minigames/WheelSpinMinigame.cs` | **游戏逻辑** — 结果生成, 角度计算, 完成回调                                    |
| `wheel.png`            | Assets 中 `images/event_extras/wheel.png`                                        | 转盘纹理                                                                       |
| `wheelArrow.png`       | Assets 中 `images/event_extras/wheelArrow.png`                                   | 箭头纹理                                                                       |
| `spinButton.png`       | Assets 中 `images/event_extras/spinButton.png`                                   | 旋转按钮纹理                                                                   |
| `wheel.ogg`            | Assets 中 `sfx/events/wheel.ogg`                                                 | 转盘旋转音效                                                                   |
| Event 背景             | 使用 ActsFromThePast 的 `backgrounds/` atlas（每幕不同背景）                     | 场景背景                                                                       |

### 当前 MOD 状态
- `WheelOfChange.cs` 在 `Events/` 中已有事件逻辑实现，但使用纯文本结果展示
- `Sts1EventInjectionPatch.cs` 已注册到全局事件池
- 本地化 JSON 已包含 `WHEEL_OF_CHANGE` 的文本条目

### 实现方案

**步骤 1：拷贝资源到 MOD**
- `wheel.png` → `Sts2BalanceMod/images/event_extras/wheel.png`
- `wheelArrow.png` → `Sts2BalanceMod/images/event_extras/wheelArrow.png`
- `spinButton.png` → `Sts2BalanceMod/images/event_extras/spinButton.png`
- `wheel.ogg` → `Sts2BalanceMod/sfx/events/wheel.ogg`

**步骤 2：移植 UI 代码**
- 新建 `Sts2BalanceModCode/Events/UI/NWheelSpinScreen.cs`
  - 简化版：去掉 ActsFromThePast 专属的特性（粒子效果、控制器支持、atlas 背景）
  - 核心保留：bounce-in 动画、旋转动画（linear spin + elastic deceleration）、bounce-out 动画
  - 资源路径改为 MOD 自己的 `res://Sts2BalanceMod/images/event_extras/`
  - 音效路径改为 `res://Sts2BalanceMod/sfx/events/wheel.ogg`
  - 使用 MOD 现有的音频播放系统 (`Sts2ModAudio.PlayOneShot`) 替代 `AFTPModAudio.Play`

- 新建 `Sts2BalanceModCode/Events/UI/WheelSpinMinigame.cs`
  - 保持原有逻辑：接收 `(Player, result, actIndex)`，计算 `ResultAngle`，暴露 `PlayMinigame()` 方法
  - 调用 `NWheelSpinScreen.ShowScreen(this)` 展示 UI

**步骤 3：修改 `WheelOfChange.cs`**
- 在 `Play()` 方法中：创建 `WheelSpinMinigame` → `await minigame.PlayMinigame()` → `ShowResult(result)`
- 参考 ActsFromThePast 的 `WheelOfChange.cs` 修改逻辑
- 保留现有的 `ShowResult()` 文本提示和 `ApplyResult()` 逻辑

**步骤 4：更新资产清单**
- 在 `CustomEventPortraitPatch.cs` 中 `WheelOfChange` 已有关联 portrait 路径
- 确认 portrait 图片已存在于 `Assets/ActsFromThePast/images/events/actsfromthepast-wheel_of_change.png`
- 需要在 MOD 中拷贝该 portrait 或将其路径映射到子模块资源

### 关键实现参考
```csharp
// WheelOfChange.Play() 修改后:
private async Task Play()
{
    for (var i = 0; i < Owner.RunState.CurrentActIndex; i++)
        Rng.NextInt(1);

    var result = Rng.NextInt(6);
    var minigame = new WheelSpinMinigame(Owner, result, Owner.RunState.CurrentActIndex);
    await minigame.PlayMinigame();
    ShowResult(result);
}
```

---

## 4. RELIC-10 — 矮人铁砧（商店遗物）

### 需求
商店遗物：获得后，在火堆处可以选择一张**攻击**或**技能**牌，不限次数锻造（附魔效果）。
- 每个火堆只能锻造 **一次**
- 同一张卡牌可以跨火堆**反复锻造**
- 仅对攻击/防御等数值效果强化点数（伤害、格挡等）
- 强化公式：`ceil(n(n+7)/2) + x`
  - n = 锻造次数（附魔的 Amount）
  - x = 卡牌当前数值
  - 结果向上取整

### 调研结果：STS2 附魔系统

#### 核心类
| 类                 | 文件                                       | 用途                                           |
| ------------------ | ------------------------------------------ | ---------------------------------------------- |
| `EnchantmentModel` | `src/Core/Models/EnchantmentModel.cs`      | 附魔抽象基类（第21行）                         |
| `Sharp`            | `src/Core/Models/Enchantments/Sharp.cs`    | 攻击附魔：`EnchantDamageAdditive` 返回 Amount  |
| `Vigorous`         | `src/Core/Models/Enchantments/Vigorous.cs` | 攻击附魔（一次性的）                           |
| `Goopy`            | `src/Core/Models/Enchantments/Goopy.cs`    | 防御附魔：`EnchantBlockAdditive` 返回 Amount-1 |
| `ElectricShrymp`   | `src/Core/Models/Relics/ElectricShrymp.cs` | 遗物参考：获取时选择卡牌附魔                   |

#### 关键 API
- `EnchantmentModel.CanEnchant(CardModel card)` — 是否可附魔（第245行），默认检查：卡牌类型、现有附魔、不可打出等
- `EnchantmentModel.CanEnchantCardType(CardType cardType)` — 限制卡牌类型（Attack / Skill 等）
- `EnchantmentModel.EnchantDamageAdditive(decimal originalDamage, ValueProp props)` — 攻击伤害附加（第354行）
- `EnchantmentModel.EnchantBlockAdditive(decimal originalBlock)` — 格挡附加（第344行）
- `CardCmd.Enchant(EnchantmentModel enchantment, CardModel card, decimal amount)` — 应用附魔
- `CardSelectCmd.FromDeckForEnchantment(Player owner, EnchantmentModel canonicalEnchantment, int count, CardSelectorPrefs prefs)` — 选择可附魔的卡牌
- `CardSelectorPrefs.EnchantSelectionPrompt` — 附魔选择模式
- 图标路径约定：`images/enchantments/{id小写}.png`（`EnchantmentModel.IntendedIconPath` 第85行）

#### 重要机制
- 一张卡只能有一个附魔（除非 `IsStackable = true`）
- 附魔的 `Amount` 字段追踪层级
- `ModifyCard()` → 调用 `OnEnchant()` + `RecalculateValues()` (第306-315行)

### 实现方案

**步骤 1：创建抽象基类 `Sts2EnchantmentModel`**
- 新建 `Sts2BalanceModCode/Abstract/Sts2EnchantmentModel.cs`
  - 封装通用附魔逻辑
  - 提供 `GetBoostAmount(int forgeCount)` 方法计算 `ceil(n(n+7)/2)`

**步骤 2：创建自定义附魔 `ForgeEnchantment`**
- 新建 `Sts2BalanceModCode/Enchantments/ForgeEnchantment.cs`
  - 继承 `EnchantmentModel`
  - `ShowAmount => true` — 显示层级
  - `CanEnchantCardType` — 允许 Attack 和 Skill
  - `CanEnchant` — 卡牌尚未有附魔（利用基类逻辑）
  - 唯一标识：`STS2BALANCEMOD-FORGE_ENCHANTMENT`
  - 覆盖：
    - `EnchantDamageAdditive`：返回 `GetBoostAmount(Amount)`（仅对 `IsPoweredAttack()` 生效）
    - `EnchantBlockAdditive`：返回 `GetBoostAmount(Amount)`
  - `GetBoostAmount(int n)`：`(int)Math.Ceiling((decimal)n * (n + 7) / 2m)`

**步骤 3：创建遗物 `DwarfAnvil`**
- 新建 `Sts2BalanceModCode/Relics/DwarfAnvil.cs`
  - 继承 `Sts2RelicModel`
  - `RelicPool.Shop` + `RelicTag.Shop`
  - 不需要 `HasUponPickupEffect`（效果在火堆触发）
  - 添加 `ExtraHoverTips` 显示 `ForgeEnchantment` 的提示

**步骤 4：创建火堆选项 `DwarfAnvilRestSiteOption`**
- 新建 `Sts2BalanceModCode/RestSite/DwarfAnvilRestSiteOption.cs`
  - 继承 `Sts2RestSiteOption`
  - 标题：`"锻造"`（CustomTitle）
  - 图标：`option_dwarf_anvil.png`
  - `OnSelect` 逻辑（参考 `ElectricShrymp.cs` 第20-28行）：
    1. 检查玩家是否持有 `DwarfAnvil`
    2. 使用 `CardSelectCmd.FromDeckForEnchantment(owner, forgeEnchantment, 1, prefs)` 选牌
    3. 遍历结果：`CardCmd.Enchant(forgeEnchantment.ToMutable(), card, existingAmount + 1)`
    4. 注意：如果是首次附魔，Amount = 1；如果是已有该附魔（`IsStackable`），Amount++
    5. 显示升级预览：`CardCmd.Preview(card)`
  - 条件：玩家有 `DwarfAnvil` + 牌组有可附魔的卡牌

**步骤 5：注册火堆选项 Patch**
- 修改 `CustomRestSiteOptionButtonPatch.cs` 或新建 Patch
  - 在火堆列表中注入 `DwarfAnvilRestSiteOption`
  - 仅在玩家持有 `DwarfAnvil` 时显示

**步骤 6：本地化**
- `enchantments.json` 添加 `STS2BALANCEMOD-FORGE_ENCHANTMENT.title/description/extraCardText`
- `relics.json` 添加矮人铁砧名称和描述
- `rest_site_ui.json` 添加 `OPTION_DWARF_ANVIL.name`
- 中文描述参考："被附魔的卡牌可以获得不断叠加的伤害/格挡加成"

**步骤 7：资源**
- 附魔图标：用户放入 `image_gen/source/enchantments/forge_enchantment.png`，运行 `uv run enchantments forge_enchantment.png`
- 遗物图标：`uv run relics DwarfAnvil.png`
- 火堆选项图标：`uv run rest-site-options dwarf_anvil.png`

### 关键实现参考
```csharp
// Enchantment 实现
public sealed class ForgeEnchantment : EnchantmentModel
{
    public override bool ShowAmount => true;
    public override bool IsStackable => false; // 每张卡只有一个此附魔

    public override bool CanEnchantCardType(CardType cardType)
    {
        return cardType is CardType.Attack or CardType.Skill;
    }

    // 数值公式: ceil(n(n+7)/2)
    private static int GetBoostAmount(int n) =>
        (int)Math.Ceiling((decimal)n * (n + 7) / 2m);

    public override decimal EnchantDamageAdditive(decimal originalDamage, ValueProp props)
    {
        if (!props.IsPoweredAttack()) return 0m;
        return GetBoostAmount(Amount);
    }

    public override decimal EnchantBlockAdditive(decimal originalBlock)
    {
        return GetBoostAmount(Amount);
    }
}

// RestSite 选项逻辑
protected override async Task<bool> OnSelect()
{
    var anvil = ModelDb.Relic<DwarfAnvil>();
    if (!Owner.Relics.Any(r => r is DwarfAnvil))
        return false;

    var enchantment = ModelDb.Enchantment<ForgeEnchantment>();
    var prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
    var selectedCards = await CardSelectCmd.FromDeckForEnchantment(Owner, enchantment, 1, prefs);

    foreach (var card in selectedCards)
    {
        var existingAmount = card.Enchantment?.Amount ?? 0;
        var mutableEnchant = enchantment.ToMutable();
        CardCmd.Enchant(mutableEnchant, card, existingAmount + 1);
        CardCmd.Preview(card);
    }
    return selectedCards.Count > 0;
}
```

### 注意
- 需要在 `ModelDb` 中注册新类型（`ForgeEnchantment`、`DwarfAnvil`）
- `CustomEventPortraitPatch.cs` 类似的注册模式可用于确保类型被游戏发现
- 附魔图标路径将解析为 `res://images/enchantments/forge_enchantment.png`
- 确保 `EnchantmentModel.IntendedIconPath` 能找到图标文件

---

## 5. BUG-12 — 面具强盗事件遭遇战消失

### 问题分析
BUG 描述：「面具强盗事件注册的遭遇战，事件版本的遇不到了」

### 当前实现
- `MaskedBandits` 事件（`Events/MaskedBandits.cs`）：
  - `Acts => []` — 空数组，通过 `Sts1EventInjectionPatch` 注入所有 Act
  - `CanonicalEncounter => ModelDb.Encounter<RedMaskBandits>()`
  - `IsAllowed`：仅第 2 幕（actIndex=1）且 floor >= 23，且未持有红面具
- `RedMaskBandits` 遭遇（`Encounters/RedMaskBandits.cs`）：
  - `RoomType => RoomType.Monster`
  - 通过 `HiveEncounterInjectionPatch` 注入到 Hive 的遭遇池
- `MaskedBandits.Fight()` 使用 `EnterCombatWithoutExitingEvent<RedMaskBandits>(...)` 触发战斗

### 对照 ActsFromThePast 实现
| 属性               | 我们的实现            | ActsFromThePast                      |
| ------------------ | --------------------- | ------------------------------------ |
| 事件类             | `MaskedBandits`       | `MaskedBandits`                      |
| 遭遇类             | `RedMaskBandits`      | `RedMaskBanditsEvent`                |
| 事件 Acts          | `[]`（注入，不限Act） | `[TheCityAct]`（固定第2幕自定义Act） |
| 遭遇 IsValidForAct | 未重写（从基类）      | `false`（始终返回 false）            |
| 遭遇 IsWeak        | `false`               | `false`                              |

**关键差异**：`RedMaskBanditsEvent.IsValidForAct(act)` 返回 `false`，确保它不通过普通遭遇池抽取。我们缺少这个。

### 用户确认
- 需要限制 Act 2。ActsFromThePast 使用 `TheCityAct` 是因为它实现了自定义场景。我们的游戏内置 Act 2 是 `Hive`。
- 使用 `ModelDb.Act<Hive>()` 替代 `Acts => []`

### 修复方案

**步骤 1：修复 `MaskedBandits.cs` — 指定 Act 归属**
```csharp
// 修改前: Acts => []
// 修改后:
public override ActModel[] Acts => [ModelDb.Act<Hive>()];
```

**步骤 2：修复 `RedMaskBandits.cs` — 禁止普通抽取**
```csharp
// 新增:
public override bool IsValidForAct(ActModel act) => false;
```

**步骤 3：保留 `HiveEncounterInjectionPatch`**
- 继续将 `RedMaskBandits` 注入 Hive 遭遇池（用于图鉴显示）
- `IsValidForAct => false` 确保它不被普通地图抽取，但图鉴仍可找到

**步骤 4：测试验证**
- 开启多局游戏到第 2 幕 floor >= 23
- 确认事件出现并可正常进入战斗
- 确认战斗结束后获得红面具和相关奖励

---

## 6. FEATURE-03 — 时间吞噬者 BGM 体系改造

> 用户要求：参考 ActsFromThePast 源码的音乐实现逻辑，使用其资源。

### 当前实现
- `TimeEaterBoss.cs`：`CustomBgm => "res://Sts2BalanceMod/music/beyond_boss.ogg"`
- `LocalAudioPatch.cs`：拦截 `NRunMusicController.PlayCustomMusic()`，对 mod 音乐路径使用 Godot `AudioStreamPlayer` 播放
- 退出战斗时 `CleanupBgmOnCombatExitPatch` 停止 BGM

### ActsFromThePast 参考实现
AFTPModAudio (`docs/references/ActsFromThePast/ActsFromThePast/Utility/AFTPModAudio.cs`) 提供：
- `FadeIn(string[] musicOptions, float duration)` — 渐入音乐（带 crossfade：旧音乐淡出 + 新音乐淡入）
- `FadeOut(float duration)` — 渐出音乐
- `PlayBossStinger(float seekFrom)` — Boss 胜利短曲
- `PlayAmbience / FadeInAmbience / FadeOutAmbience` — 环境音管理
- 音量通过 `SaveManager.Instance.SettingsSave.VolumeBgm` 响应游戏设置

### 分析：当前方案 vs AFTP 方案
| 方面        | 当前方案 (LocalAudioPatch)               | AFTP 方案                               |
| ----------- | ---------------------------------------- | --------------------------------------- |
| 播放方式    | 拦截 `PlayCustomMusic()` 转 Godot 播放器 | 直接通过 Godot `AudioStreamPlayer` 播放 |
| 淡入淡出    | 无                                       | FadeIn / FadeOut (支持 crossfade)       |
| 音量响应    | 可能不响应                               | 通过 `SaveManager.VolumeBgm` 响应       |
| Boss 胜利曲 | 无                                       | `PlayBossStinger`                       |
| 环境音      | 无                                       | 独立环境音轨道                          |

### 实现方案

**步骤 1：增强 `Sts2ModAudio` — 仿照 `AFTPModAudio` 实现**

### 用户确认
> "时间老头使用的 AFTP 模式的话，感觉 Feature 后续的就是正常的接入游戏流程了，他就不再是一个问题了"

意思是：对 Time Eater 使用 AFTP 的 BGM 管理模式（FadeIn/FadeOut/AudioStreamPlayer 直接播放），实现后这个 FEATURE 就算完成，后续随正常游戏流程即可，不再需要额外处理。
- 添加 FadeIn / FadeOut / Crossfade 支持（参考 AFTPModAudio 第131-195行）
- 添加 Boss Stinger 播放（参考第342-367行）
- 添加音量响应（参考第112-113行、第259-261行）
- 添加环境音轨道（可选，参考第240-305行）

**步骤 2：修改 `TimeEaterBoss.cs` — 使用新音频系统**
- 移除 `CustomBgm` 属性
- 覆盖生命周期方法（如 `OnRoomEnter` 或通过 Hook）调用 `Sts2ModAudio.FadeIn(...)` 播放自定义 BGM
- 使用 ActsFromThePast 的 BGM 资源（或已拷贝到 `Sts2BalanceMod/music/` 的版本）

**步骤 3：准备 BGM 资源**
- 我们已有 `Sts2BalanceMod/music/beyond_boss.ogg` — 可直接使用
- ActsFromThePast 的 `bgm/` 目录有更全的曲目（boss stingers 等），可选择性拷贝

**步骤 4：清理旧注入机制**
- 移除 `LocalAudioPatch.cs` 或在确认新系统稳定后移除
- 移除 `CleanupBgmOnCombatExitPatch`

**步骤 5：将来扩展**
- 如果需要全 Act 音乐替换（类似 AFTP 的 LegacyActMusic），可以为每幕设置探索/精英/Boss 曲目
- 当前版本仅针对 Time Eater Boss 战

---

## 7. 依赖关系与执行顺序

```
Phase 1: 前置准备 (已完成)
├── git pull
└── 确认 ActsFromThePast 资源现状

Phase 2: 并行实现
├── MON-03 — Time Warp 视觉效果移植
│   ├── 拷贝 atlas 资源
│   ├── 移植 TimeWarpTurnEndEffect.cs
│   └── 修改 TimeWarpPower.cs (添加视觉效果)
├── STS1-EVENT-07 — 转盘 UI 移植
│   ├── 拷贝资源到 MOD
│   ├── 移植 NWheelSpinScreen.cs
│   ├── 移植 WheelSpinMinigame.cs
│   └── 修改 WheelOfChange.cs
├── BUG-12 — 面具强盗修复
│   ├── 修改 MaskedBandits.cs (Acts = [Hive])
│   ├── 修改 RedMaskBandits.cs (IsValidForAct = false)
│   └── 验证
└── FEATURE-03 — BGM 体系改造
    ├── 增强 Sts2ModAudio (FadeIn/FadeOut/Stinger)
    ├── 修改 TimeEaterBoss.cs (移除 CustomBgm)
    └── 清理 LocalAudioPatch.cs

Phase 3: 独立实现
└── RELIC-10 — 矮人铁砧
    ├── 创建 DwarfAnvil 遗物模型
    ├── 创建 DwarfAnvilRestSiteOption
    ├── 创建 Patch 注册火堆选项
    ├── 本地化
    └── 图片资源生成

Phase 4: 验证
├── dotnet build
├── 集成测试 (tests/)
├── 游戏内验证
└── CHANGELOG.md 更新
```

---

## 8. 风险与注意事项

### MON-03 风险
- `NSts1Effect` 基类在游戏中可能不存在 — 需要自实现 `Node2D` 版
- `LibGdxAtlas.GetRegion` 方法需要确认可用性；备选方案：手动加载 PNG + AtlasTexture

### RELIC-10 风险
- "反复升级"在 STS2 中可能不支持（原版卡牌只能升级一次）
- 需要调研 `CardCmd.Upgrade` 是否可以多次调用同一张牌
- 如不可行需要自定义升级机制或改为"附魔"系统

### BUG-12 风险
- 修复后需检查图鉴是否仍显示 `RedMaskBandits`
- `HiveEncounterInjectionPatch` 可能因 `IsValidForAct => false` 而无法显示图鉴
- 如不显示，需找图鉴显示的替代机制

### 转盘 UI 风险
- `NWheelSpinScreen` 使用了 `NOverlayStack`、`NProceedButton` 等游戏内部组件
- 简化版需去掉粒子、背景 atlas 切换等功能
- 控制器支持可能需要额外调试

### FEATURE-03 风险
- 增强 `Sts2ModAudio` 需要测试音量联动、淡入淡出
- 战斗结束时需确保 BGM 正确停止（避免残留）
- LocalAudioPatch 的移除需确认不再有其他内容依赖它

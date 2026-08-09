# 宝箱房跳过按钮重构 — 技术报告

## 概述

本文档记录诅咒钥匙（CurseKey）的宝箱房跳过功能如何从「额外创建跳过按钮」重构为「复用 ProceedButton」，以及诅咒钥匙图标在按钮上的放置方式。

相关代码：`src/Patches/Relics/TreasureRoomSkipPatch.cs`、`CurseKeyPatch.cs`

---

## 一、问题背景

原版的诅咒钥匙跳过宝箱流程存在两个核心问题：

1. **多按钮混乱**：宝箱房中央有一个通过 `choice_selection_skip_button.tscn` 创建的独立跳过按钮，右下角又有原版的 ProceedButton，玩家面临两个操作入口。
2. **黑屏崩溃**：`choice_selection_skip_button.tscn` 内部依赖 `NChoiceSelectionSkipButton` 脚本，该脚本试图加载卡牌/遗物选择跳过场景，在宝箱房上下文中缺少该场景导致黑屏。

**重构目标**：不创建任何新按钮，直接复用宝箱房右下角的 ProceedButton，根据宝箱开/关状态切换其文字和行为。

---

## 二、设计思路

### 2.1 状态驱动复用

ProceedButton 是一个按钮、两个语义：

| 时机 | 按钮文字 | 点击行为 |
|------|---------|---------|
| 进房未开箱（持有诅咒钥匙） | "跳过宝箱" | 跳过宝箱 → 开启地图 → 无诅咒 |
| 开箱取遗物后 | "Proceed"（原版文字） | 正常离开 → 诅咒触发 |

状态通过两个静态字段追踪：
- `_chestOpened`：宝箱是否已打开
- `SkipChestForCurseKey`：玩家是否选择了跳过

### 2.2 为什么不直接修改场景

Godot 场景（`.tscn`）是资源文件，Mod 不宜直接修改。使用 Harmony Patch 在运行时修改节点行为和属性，兼容性更好、不破坏原版场景。

---

## 三、Patch 架构

所有 Patch 集中在 `TreasureRoomSkipPatch` 类，通过 Harmony 拦截 `NTreasureRoom` 的 4 个关键方法：

```
NTreasureRoom._Ready
  ↓ Postfix: 改文字 + 放图标 + Enable 按钮
  
NTreasureRoom.OnActiveScreenChanged
  ↓ Postfix: 防止原生逻辑在未开箱时 Disable 按钮
  
NTreasureRoom.OpenChest
  ↓ Prefix: 记录 _chestOpened=true + 移除图标
  
NTreasureRoom.OnProceedButtonPressed
  ↓ Prefix (return false): 未开箱时主动调用 ProceedFromTerminalRewardsScreen() 离开房间
```

### 3.1 `_Ready` Postfix — 初始化

在宝箱房节点就绪时：
1. 重置 `_chestOpened = false`、`SkipChestForCurseKey = false`
2. 检查是否单人模式 + 持有诅咒钥匙
3. 若符合条件，调用 `proceedButton.UpdateText(new LocString("gameplay_ui", "STS2BALANCEMOD-SKIP_CHEST"))` 将文字改为"跳过宝箱"
4. 调用 `proceedButton.Enable()` 启用按钮（原生 `_Ready` 末尾会 Disable 掉）
5. 在按钮 Image 节点中添加诅咒钥匙图标

### 3.2 `OnActiveScreenChanged` Postfix — 保持启用

原生 `NTreasureRoom.OnActiveScreenChanged` 在 `_hasChestBeenOpened == false` 时会 Disable ProceedButton。Mod 的 Postfix 在未开箱状态下重新 Enable 按钮，确保"跳过宝箱"可点击。

### 3.3 `OpenChest` Prefix — 记录状态 + 移除图标

在 `OpenChest()` 执行前：
1. 设 `_chestOpened = true`
2. 重置 `SkipChestForCurseKey = false`（如果玩家改变主意开了宝箱，诅咒正常触发）
3. 移除之前添加的诅咒钥匙图标（后续文字由原生 OpenChest 管理）

### 3.4 `OnProceedButtonPressed` Prefix — 跳过流程

拦截点击事件。如果 `_chestOpened == true`，`return true` 走原生流程（正常离开，诅咒触发）。

如果 `_chestOpened == false`（未开箱跳过着），`return false` 跳过原生 handler，在 Prefix 中执行：
```
1. SkipChestForCurseKey = true
2. NMapScreen.Instance.SetTravelEnabled(true)
3. TaskHelper.RunSafely(ProceedFromTerminalRewardsScreen())
```

`ProceedFromTerminalRewardsScreen()` 会打开地图界面，允许玩家选择下一层。原生的 else 分支虽然也调用这个方法，但不会先调用 `SetTravelEnabled(true)`，导致地图打开但无法选择地点。

---

## 四、诅咒时机变更

### 4.1 原行为

`CurseKeyPatch` 拦截 `RewardsCmd.GenerateForRoomEnd`（开箱瞬间生成诅咒），无论玩家是否取走遗物都触发。

### 4.2 现行为

Patch 点改为 `NProceedButton.UpdateText` Postfix。当文字被设为 `NProceedButton.ProceedLoc` 时（= OpenChest 流程已执行完 relic picked），生成诅咒。配合 `_chestOpened` 过滤掉 `_Ready` 中的那一次调用。

```csharp
[HarmonyPatch(typeof(NProceedButton), "UpdateText")]
static void Postfix(LocString loc)
{
    if (loc.LocEntryKey != NProceedButton.ProceedLoc.LocEntryKey) return; // 只关心 ProceedLoc
    if (!TreasureRoomSkipPatch.IsAfterChestOpen()) return; // 过滤 _Ready
    if (TreasureRoomSkipPatch.SkipChestForCurseKey) return; // 过滤 skip

    // 生成诅咒
    TaskHelper.RunSafely(AddRandomCurse(player));
}
```

选择遗物后才生成诅咒，让诅咒在体验上成为"取遗物的代价"而非"开箱的代价"。

---

## 五、Icon 放置

### 5.1 实现方式

在 ProceedButton 的 Image 节点下动态创建一个 `TextureRect`：

```csharp
var icon = new TextureRect();
icon.Name = "CurseKeyIcon";
icon.Texture = ResourceLoader.Load<Texture2D>("res://Sts2BalanceMod/images/relics/curse_key.png");
icon.Size = new Vector2I(32, 32);
icon.Position = new Vector2(12, 34); // 按钮左侧、文字左方
icon.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
image.AddChild(icon);
```

### 5.2 为什么放在 Image 下

ProceedButton 的节点结构：

```
ProceedButton (NProceedButton / Control)  269×108
  ├── Image (TextureRect)   ← 图标放这里
  │   ├── Shadow
  │   ├── Outline
  │   └── Label (MegaLabel)  ← 文字"跳过宝箱"
  └── ControllerIcon
```

Label 从 x=57 开始（中心对齐），图标放在 x=12、宽 32px，到 x=44 结束，与文字有充足间隔不重叠。

### 5.3 清理时机

`OpenChest` Prefix 中调用 `RemoveSkipIcon()`，开箱后图标消失，文字由原生 `OpenChest` 管理（→ SkipLoc → ProceedLoc）。

### 5.4 为什么不直接修改 .tscn 文件

- Harmony 方案不需要分发修改后的场景文件
- Mod 升级时不会因场景文件合并冲突
- 代码控制在 Patch 中，逻辑集中、可维护

---

## 六、黑屏问题修复

旧方案设置 `chest.Visible = false` 隐藏宝箱。地图的"返回"按钮将玩家带回房间时，宝箱不可见 + 房间状态不完整 → 只剩全黑背景 `ColorRect` → 黑屏。

**修复**：不再隐藏宝箱。Skip 后玩家返回可正常开箱，`OpenChest` Prefix 中的 `SkipChestForCurseKey = false` 确保诅咒正确触发。

---

## 七、多语言

跳过宝箱文字使用 `LocString` 系统对接本地化文件：

| 文件 | Key | EN | ZHS | ITA |
|------|-----|----|-----|-----|
| `localization/{lang}/gameplay_ui.json` | `STS2BALANCEMOD-SKIP_CHEST` | Skip Chest | 跳过宝箱 | Salta Forziere |

定位在 `gameplay_ui.json`（与原版 `PROCEED_BUTTON` 同源），而非和遗物描述混放的 `relics.json`。

---

## 八、完整数据流

```
玩家持有 CurseKey 进入宝箱房
├─ _Ready Postfix: 文字→"跳过宝箱"，图标→显示，Enable
│
├─ 玩家点击"跳过宝箱"
│  └─ OnProceedButtonPressed Prefix
│     ├─ SkipChestForCurseKey = true
│     ├─ SetTravelEnabled(true)
│     ├─ ProceedFromTerminalRewardsScreen() → 打开地图
│     └─ return false (跳过原生)
│
├─ 玩家点击宝箱
│  └─ OpenChest Prefix
│     ├─ _chestOpened = true
│     ├─ SkipChestForCurseKey = false
│     └─ 移除图标
│
├─ 原生 OpenChest 流程
│  ├─ UpdateText(SkipLoc)    → "跳过"(可选择跳过遗物)
│  ├─ 玩家选遗物
│  ├─ UpdateText(ProceedLoc) → "Proceed"
│  │  └─ CurseKeyPatch Postfix: 检测到 ProceedLoc → 生成诅咒
│  └─ Enable → 可点击离开
│
└─ 玩家点击 Proceed
   └─ OnProceedButtonPressed Prefix
      ├─ _chestOpened = true → return true (走原生)
      └─ 原生: IsSkip=false → else → ProceedFromTerminalRewardsScreen()
```

---

## 九、涉及的文件

| 文件 | 用途 |
|------|------|
| `src/Patches/Relics/TreasureRoomSkipPatch.cs` | ProceedButton 复用、状态追踪、图标管理 |
| `src/Patches/Relics/CurseKeyPatch.cs` | 诅咒生成时机（选遗物后） |
| `Sts2BalanceMod/localization/{eng,zhs,ita}/gameplay_ui.json` | 按钮文字本地化 |

# STS2 平衡调整需求清单

> 本文档追踪**未完成 / 待处理**的调整项，并保留最近完成任务的验收状态。
> 所有已实现调整的玩家说明请查看 [README.md](../README.md#调整内容) 的「调整内容」章节。

---

## 待办项


### 卡牌


### 遗物


### 事件


### BOSS

- [ ] **AFP-BOSS-TEST-01** — 三场第一幕 Boss 战独立验收
  - 编译: `dotnet build` 通过，且 PCK 中包含本任务使用的怪物场景、Spine、贴图与音效。
  - 运行: 分别进入守护者、六火亡魂、史莱姆 Boss 专用遭遇，完整打到胜利；史莱姆 Boss 必须覆盖两级分裂流程。
  - 日志: 检查 `%AppData%/SlayTheSpire2/logs/godot.log`，不得出现 Mod 加载、模型注册、场景路径、动画、音效或战斗状态机异常。
  - 边界: 三场遭遇不进入正常第一幕地图池；心灵绽放第二战已启用，运行验收需同时覆盖随机强化、奖励结算和史莱姆分裂体不继承强化。


### 怪物


---

## 最近完成

### 卡牌

- [x] **CARD-COOLANT-01** — 故障机器人能力牌「冷却剂」（Coolant）重做为能力抽牌引擎（散热片）并降为蓝卡
  - 稀有度降为蓝卡（Uncommon），1 费能力牌，重做效果为“每当你打出一张能力牌，抽 1(2) 张牌”（类似 1 代散热片 Heatsinks）。
  - 通过 Harmony Patch 拦截 `Coolant.Rarity`、`Coolant.CanonicalVars`，屏蔽原版充能球格挡逻辑，并在打出能力牌时触发抽牌。
  - 同步更新四国语言本地化说明文本。

### 遗物

- [x] **RELIC-NEOWS-TALISMAN-01** — 涅奥遗物「涅奥的护符」（Neow's Talisman）重做为快速开局遗物（涅奥的悲哀）
  - 拾起后，你遇到的接下来 3 场战斗中，所有敌人的初始生命值变为 1 点。
  - 遗物包含计数器（显示剩余生效战斗场数 3 -> 0），消耗完毕后遗物置灰失效；同步更新四国语言（zhs/eng/ita/rus）本地化文本。

- [x] **RELIC-SANDCASTLE-01** — 先古遗物「沙堡」（Sand Castle）调整

  - 拾起时，先由玩家从牌组中选择 3 张可升级牌进行升级，随后在牌组剩余可升级牌中随机升级 3 张。
  - 通过 Harmony Patch 拦截 `SandCastle.AfterObtained` 实现，并同步更新四国语言本地化说明文本。

- [x] **RELIC-STRANGE-SPOON-01** — 商店遗物「奇怪的汤勺」（Strange Spoon）机制修复与【凋萎】必定消耗
  - 规范中文名称为「奇怪的汤勺」；打出应消耗的牌时，有 50% 几率进入弃牌堆而不是消耗；打出【凋萎】（Wither）时 100% 必定消耗。
  - 描述文本与 HoverTip 明确注明「（凋萎必定会被消耗。）」。
  - 通过 Harmony Patch 拦截 `CardModel.GetResultLocationForCardPlay`，清理遗物内部不正确的 `AfterCardPlayed` 逻辑。

### 卡牌

- [x] **CARD-ROLLBACK-01** — 移除助燃剂（触媒 Accelerant）的稀有度 Patch，恢复原版
  - 删除 `AccelerantRarityRollbackPatch.cs`，保持原版 Uncommon 稀有度。

- [x] **CARD-ROLLBACK-02** — 移除战士卡池移除残酷（Cruelty）的 Patch，恢复原版
  - 删除 `IroncladCardPoolPatch.cs`，恢复残酷在战士卡池中正常出现。

### 基础设施

- [x] **CONFIG-01** — Mod 设置页面 MVP
  - 注册 RitsuLib 设置页面并启用自动持久化。
  - 首个开关控制“除虫者”“科学怪人”和“药水的未来？”的离开选项，默认开启。
  - 补齐英语、简体中文、意大利语和俄语设置界面文本。

- [x] **CONFIG-02** — 感染棱柱重做开关
  - 默认开启，保持 BOSS-01 当前的固定四回合循环与【感染】机制。
  - 关闭时完整恢复原版【活力火花】开场能力与原版行动状态机。
  - 下次进入感染棱柱战斗时生效；多人游戏时所有玩家必须使用相同设置。


### BOSS

- [x] **AFP-BOSS-03** — 史莱姆 Boss（Slime Boss）及心灵绽放专用遭遇
  - 移植史莱姆 Boss、尖刺/酸液大型与中型史莱姆、Split Power、两级分裂链、动画与音效。
  - 新增包含七个固定分裂槽位的 `RoomType.Monster` 专用遭遇，保持 AFP 原始数值与进阶分档。


### 遗物

- [x] **RELIC-SWAP-01** — 骨妹（Silent）DIY 遗物稀有度互换
  - 目前: 悬浮风筝（HoveringKite）为 Common，袖箭（WristBlade）为 Uncommon
  - 目标: 悬浮风筝改为 Uncommon，袖箭改为 Common
  - 备注: 两者均注册在 `SilentRelicPool`，仅需修改 `Rarity` 属性


---

## 现有的问题以及无法解决的问题


### BUG 列表

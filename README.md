<div align="center">
  <img alt="logo" height="100" width="100" src="docs/img/icon.ico" />
  <h2> Sts2BalanceMod </h2>
  <p> Sts2BalanceMod — 《杀戮尖塔 2》平衡调整 Mod </p>
  <p>
    <img src="https://img.shields.io/badge/Language-%E7%AE%80%E4%BD%93%E4%B8%AD%E6%96%87-blue?style=flat-square" alt="简体中文" />
    <a href="README_EN.md"><img src="https://img.shields.io/badge/Language-English-lightgrey?style=flat-square" alt="English" /></a>
  </p>
  <p>
    <img src="Assets/profile/ironclad.png" width="28" height="28" title="铁甲战士 (Ironclad)" />
    <img src="Assets/profile/silent.png" width="28" height="28" title="静默猎手 (Silent)" />
    <img src="Assets/profile/regent.png" width="28" height="28" title="储君 (Regent)" />
    <img src="Assets/profile/necrobinder.png" width="28" height="28" title="死灵缚者 (Necrobinder)" />
    <img src="Assets/profile/defect.png" width="28" height="28" title="故障机器人 (Defect)" />
  </p>
  <a href="https://github.com/MT-SUPER-POWER/STS2-Balance-MOD/stargazers">
    <img src="https://img.shields.io/github/stars/MT-SUPER-POWER/STS2-Balance-MOD?style=flat" alt="Stars" />
  </a>
  <a href="https://github.com/MT-SUPER-POWER/STS2-Balance-MOD/releases">
    <img src="https://img.shields.io/github/v/release/MT-SUPER-POWER/STS2-Balance-MOD" alt="Version" />
  </a>
  <a href="https://github.com/MT-SUPER-POWER/STS2-Balance-MOD/issues">
    <img src="https://img.shields.io/github/issues/MT-SUPER-POWER/STS2-Balance-MOD" alt="Issues" />
  </a>
  <a href="LICENSE">
    <img src="https://img.shields.io/github/license/MT-SUPER-POWER/STS2-Balance-MOD" alt="License" />
  </a>
</div>


## 安装

### 前置要求

1. **[Slay the Spire 2](https://store.steampowered.com/app/2868840/Slay_the_Spire_2/)** 版本 ≥ 0.110.0
2. **[RitsuLib](https://github.com/BAKAOLC/STS2-RitsuLib)** — Mod 加载前置库，需先安装与当前游戏版本兼容的最新稳定版

### 安装步骤

1. 下载本 Mod 的最新发布包（从 [Releases](https://github.com/MT-SUPER-POWER/STS2-Balance-MOD/releases) 页面获取 `.zip`）
2. 将解压后的 **整个文件夹** 放入 STS2 的 Mod 目录：
   - **Windows**: `%AppData%/SlayTheSpire2/mods/`
   - **macOS**: `~/Library/Application Support/SlayTheSpire2/mods/`
   - **Linux**: `~/.local/share/SlayTheSpire2/mods/`
3. 确保 `STS2-RitsuLib` 也已安装在同一目录
4. 启动游戏，在 Mod 管理页面确认 `Sts2BalanceMod` 已勾选

---

## 调整内容

> [!note]
> 以下是本 Mod 已经实装的所有平衡与内容调整。
>
> 版本变更记录见 **[CHANGELOG.md](CHANGELOG.md)**，能力与效果手册见 **[docs/powers.md](docs/powers.md)**，事件与遭遇手册见 **[docs/events.md](docs/events.md)**，待办项见 **[docs/balance-changes.md](docs/balance-changes.md)**。

### Mod 设置

本 Mod 已接入 RitsuLib 的 Mod 设置页面；配置会自动保存并在后续游戏中恢复。

> [!warning]
> 多人游戏不会自动同步 Mod 配置。所有玩家必须使用相同设置，否则事件选项或游戏内容可能不一致。

| 设置项 | 默认值 | 效果 |
| :--- | :---: | :--- |
| **为事件添加“离开”选项** | 开启 | 控制是否为“除虫者”“科学怪人”和“药水的未来？”添加可直接离开的选项；修改后会在下次进入这些事件时生效。 |
| **启用感染棱柱重做** | 开启 | 开启时使用固定四回合循环与【感染】机制；关闭时恢复原版【活力火花】与原版行动状态机。修改后会在下次进入感染棱柱战斗时生效。 |

### 商店

#### 高进阶删牌价格（SHOP-01）

- **原版**：A6+ 删牌一律 50 金币，后续每删一张 +25。
- **MOD 改后**：小于A6 基础 50 金币（每张 +25），**A6+ 基础 75 金币**（每张 +25）。

### 卡牌调整

| 卡牌 | 角色 | 类型 | 原版 | MOD 改后 |
|------|:---:|:----:|------|------|
| **挽歌 [Dirge]** | <img src="Assets/profile/necrobinder.png" width="22" height="22" title="死灵缚者 (Necrobinder)"> | 能力 | 升级后召唤次数 +1，消耗，灵魂+ | 升级后**额外追加保留词条** |
| **刀舞 [Blade Dance]** | <img src="Assets/profile/silent.png" width="22" height="22" title="静默猎手 (Silent)"> | 技能 | 白卡（普通），打出后消耗 | 删除消耗词条，稀有度升为**蓝卡（罕见）**，可多次复用 |
| **杂技 [Acrobatics]** | <img src="Assets/profile/silent.png" width="22" height="22" title="静默猎手 (Silent)"> | 技能 | 蓝卡（罕见），入手门槛较高 | 稀有度降为**白卡（普通）**，提升入手概率 |
| **认知偏差 [Biased Cognition]** | <img src="Assets/profile/defect.png" width="22" height="22" title="故障机器人 (Defect)"> | 能力 | 每回合 -1 集中，永久持续 | 集中在对应回合后自动消失，不再扣除超出提升点数的集中 |
| **多重释放 [Multicast]** | <img src="Assets/profile/defect.png" width="22" height="22" title="故障机器人 (Defect)"> | 技能 | 升级后 X+1 次释放 | 升级改为仅追加**保留**词条（取消原版 X+1）|
| **幽魂形态 [Wraith Form]** | <img src="Assets/profile/silent.png" width="22" height="22" title="静默猎手 (Silent)"> | 技能 | 获得无实体后每回合累积减敏捷负面 | 打出时只施加无实体，**彻底删除减敏捷负面效果** |
| **袖里乾坤 [Up My Sleeve]** | <img src="Assets/profile/silent.png" width="22" height="22" title="静默猎手 (Silent)"> | 技能 | 每次打出后减少费用，定位与刀舞冲突 | 从猎人卡池中**完全移除** |
| **燃料 [Fuel]** | <img src="Assets/profile/defect.png" width="22" height="22" title="故障机器人 (Defect)"> | 技能 | 将所有状态牌转换获得 2 费 | 改为获得 1 点能量并**抽 1（升级：2）张牌** |
| **能量汲取 [Drain Power]** | <img src="Assets/profile/necrobinder.png" width="22" height="22" title="死灵缚者 (Necrobinder)"> | 攻击 | 造成 10/12 伤害，升级后随机升级弃牌堆 3 张 | 伤害降为 **6/8**；基础随机升级弃牌堆 **2 张**，升级后改为升级弃牌堆**全部可升级牌** |
| **吸引仇恨 [Pull Aggro]** | <img src="Assets/profile/necrobinder.png" width="22" height="22" title="死灵缚者 (Necrobinder)"> | 技能 | 升级后原版数值 | 升级后调整为：**召唤 6，格挡 9 点** |
| **凋零 [Wither]** | <img src="Assets/map/aeonglass_boss.png" width="22" height="22" title="永世沙漏 (Aeonglass)"> | 状态 | 不可打出，进入消耗堆 | 改为 **1 费可打出**，打出后消耗 |
| **辉光 [Glow]** | <img src="Assets/profile/regent.png" width="22" height="22" title="储君 (Regent)"> | 技能 | 获得星尘，下回合额外抽牌 | 改为**当回合立即抽 2 张**，升级额外多获得 1 星尘 |
| **放血 [Bloodletting]** | <img src="Assets/profile/ironclad.png" width="22" height="22" title="铁甲战士 (Ironclad)"> | 技能 | 蓝卡（罕见）（v1.0.9 修改） | 稀有度改回**白卡（普通）** |
| **创世之柱 [Pillar of Creation]** | <img src="Assets/profile/regent.png" width="22" height="22" title="储君 (Regent)"> | 能力 | 每回合第一次生成卡牌时获得 5/8 点格挡 | 回调为每生成一张牌，格挡数值调整为 **3 / 4 点** |
| **残酷 [Cruelty]** | <img src="Assets/profile/ironclad.png" width="22" height="22" title="铁甲战士 (Ironclad)"> | 能力 | 下放为战士蓝卡 | 从战士卡池中**完全移除**（由“进化”卡牌替代） |
| **探寻 [Dowsing]** | <img src="Assets/profile/neow.png" width="22" height="22" title="涅奥 (Neow)"> | 任务 | 进入 5 个 ? 房间后转化为「丰饶」 | 调整为进入 **4 个 ? 房间**后转化为「丰饶」 |
| **魔球 [The Ball]** | <img src="Assets/profile/colorless.png" width="22" height="22" title="无色 (Colorless)"> | 攻击 | 每次打出后伤害成长 **10 / 15** | 成长幅度改为 **15 / 20** |
| **巨镰 [The Scythe]** | <img src="Assets/profile/necrobinder.png" width="22" height="22" title="死灵缚者 (Necrobinder)"> | 攻击 | 2 费，消耗；造成 13 点伤害，每次打出后伤害成长 **4 / 5** | 初始伤害调整为 **16** 点，配合官方 **5 / 7** 的成长幅度 |
| **火箭飞拳 [Rocket Punch]** | <img src="Assets/profile/defect.png" width="22" height="22" title="故障机器人 (Defect)"> | 攻击 | 2 费；生成状态牌时减 1 费 | 每当生成状态牌时，获得**单次 0 费打出额度**（不按张数/张数累加），打出任意 1 张后立刻恢复 2 费 |
| **触媒 [Accelerant]** | <img src="Assets/profile/silent.png" width="22" height="22" title="静默猎手 (Silent)"> | 能力 | 蓝卡（罕见） | 稀有度回调为**金卡（稀有）** |
| **计划妥当 [Well-Laid Plans]** | <img src="Assets/profile/silent.png" width="22" height="22" title="静默猎手 (Silent)"> | 能力 | 金卡（稀有），1 / 0 费；回合结束时不弃牌 | 罕见，2 / 1 费；回合结束时保留最多 **1 / 2** 张牌（再次打出保留张数可叠加，选卡界面自动过滤手牌中已有【保留】属性的牌），未选择的手牌正常弃置，并保留多人可用支持 |
| **华丽收场 [Grand Finale]** | <img src="Assets/profile/silent.png" width="22" height="22" title="静默猎手 (Silent)"> | 攻击 | 0 费，抽牌堆有 0 张牌时打出 | **X 费**卡牌，打出条件调整为**抽牌堆卡牌数 ≤ X**；**升级后打出少扣除 2 点费用**（扣除 $\max(0, X - 2)$ 能量） |
| **精密 [Pinpoint]** | <img src="Assets/profile/silent.png" width="22" height="22" title="静默猎手 (Silent)"> | 攻击 | 耗能按技能打出数减少，造成伤害 | 从猎人卡池中**完全移除**（由「内脏切除」替代） |


### 一代卡牌回归

| 卡牌 | 角色 | 类型 | 稀有度 | 费用 | 效果（基础 / 升级） |
|------|:---:|:----:|:------:|:----:|------|
| **死亡收割 [Death Reap]** | <img src="Assets/profile/ironclad.png" width="22" height="22" title="铁甲战士 (Ironclad)"> | 攻击 | 稀有 | 2 | 消耗。对所有敌人造成 **4 / 6** 点伤害，并回复等量于实际造成的非格挡伤害的生命值。 |
| **硬撑 [Power Through]** | <img src="Assets/profile/ironclad.png" width="22" height="22" title="铁甲战士 (Ironclad)"> | 技能 | 罕见 | 1 | 获得 **15 / 20** 点格挡，将 2 张伤口加入手牌。 |
| **进化 [Evolve]** | <img src="Assets/profile/ironclad.png" width="22" height="22" title="铁甲战士 (Ironclad)"> | 能力 | 罕见 | 1 | 每当你抽到一张状态牌，抽 **1 / 2** 张牌。 |
| **全神贯注 [Concentrate]** | <img src="Assets/profile/silent.png" width="22" height="22" title="静默猎手 (Silent)"> | 技能 | 罕见 | 0 | 丢弃 **3 / 2** 张牌，获得 2 点能量。 |
| **内脏切除 [Eviscerate]** | <img src="Assets/profile/silent.png" width="22" height="22" title="静默猎手 (Silent)"> | 攻击 | 罕见 | 3 | 本回合每丢弃 1 张牌耗能 -1 点。造成 **7 / 9** 点伤害 3 次。 |
| **电动力学 [Electrodynamics]** | <img src="Assets/profile/defect.png" width="22" height="22" title="故障机器人 (Defect)"> | 能力 | 稀有 | 2 | 召唤 **2 / 3** 个闪电球，且闪电球改为攻击所有敌人。 |

### 新增卡牌

| 卡牌 | 角色 | 类型 | 稀有度 | 费用 | 效果（基础 / 升级） |
|------|:---:|:----:|:------:|:----:|------|
| **比试 [Sparring]** | <img src="Assets/profile/necrobinder.png" width="22" height="22" title="死灵缚者 (Necrobinder)"> | 攻击 | 罕见 | 2 | 消耗。玩家对单个敌人造成 **8** 点伤害，奥斯提造成 **7 / 9** 点伤害；实际造成非格挡伤害较少的一方回复 **4 / 6** 点生命。 |
| **猛撞 [Ram]** | <img src="Assets/profile/necrobinder.png" width="22" height="22" title="死灵缚者 (Necrobinder)"> | 攻击 | 普通 | 2 | 奥斯提失去 **6 / 5** 点生命，对所有敌人造成 **20 / 26** 点伤害；奥斯提生命不足时无法触发效果。 |
| **步步为营 [Step by Step]** | <img src="Assets/profile/silent.png" width="22" height="22" title="静默猎手 (Silent)"> | 技能 | 稀有 | X | 消耗。接下来 X（升级：X+1）回合，每回合多抽 1 张牌并多获得 1 点能量。升级后额外获得保留词条。 |
| **巫术打击 [Sorcery Strike]** | <img src="Assets/profile/tanx.png" width="22" height="22" title="探克斯 (Tanx)"> | 攻击 | 先古 | 1（升级：0） | 消耗。造成 **9** 点伤害，抽 **1** 张牌，施加 **1** 层巫术易伤。 |
| **巫术防御 [Sorcery Defend]** | <img src="Assets/profile/tanx.png" width="22" height="22" title="探克斯 (Tanx)"> | 技能 | 先古 | 1（升级：0） | 消耗。获得 **8** 点格挡，抽 **1** 张牌，施加 **1** 层巫术虚弱。 |

### 能力调整

> [!tip]
> 完整的能力、减益与 Boss 机制说明见 **[docs/powers.md](docs/powers.md)**。

| 能力 / 效果 | 分类 | 来源 | 效果说明 |
| :--- | :---: | :--- | :--- |
| <img src="Assets/powers/electrodynamics_power.png" width="22" height="22" valign="middle"> **电动力学 [Electrodynamics]** | 玩家 Buff | <img src="Assets/profile/defect.png" width="18" height="18" title="故障机器人"> 故障机器人卡牌「电动力学」 | [闪电]球改为攻击所有敌人。 |
| <img src="Assets/powers/evolve_power.png" width="22" height="22" valign="middle"> **进化 [Evolve]** | 玩家 Buff | <img src="Assets/profile/ironclad.png" width="18" height="18" title="铁甲战士"> 铁甲战士卡牌「进化」 | 每当你抽到状态牌，抽 **1 / 2** 张牌。 |
| <img src="Assets/powers/step_by_step_power.png" width="22" height="22" valign="middle"> **步步为营 [Step by Step]** | 玩家 Buff | <img src="Assets/profile/silent.png" width="18" height="18" title="静默猎手"> 静默猎手卡牌「步步为营」 | 每回合多抽 1 张牌并多获得 1 点能量，持续 **X / X+1** 回合。 |
| <img src="Assets/powers/sorcery_vulnerable.png" width="22" height="22" valign="middle"> **巫术易伤 [Sorcery Vulnerable]** | Debuff | <img src="Assets/profile/tanx.png" width="18" height="18" title="探克斯"> 先古卡「巫术打击」 | 受到的攻击伤害增加 **75%**；本回合受到过攻击则回合结束减 1 层。 |
| <img src="Assets/powers/sorcery_weak.png" width="22" height="22" valign="middle"> **巫术虚弱 [Sorcery Weak]** | Debuff | <img src="Assets/profile/tanx.png" width="18" height="18" title="探克斯"> 先古卡「巫术防御」 | 造成的攻击伤害减少 **50%**；本回合进行过攻击则回合结束减 1 层。 |
| <img src="Assets/powers/infected_power.png" width="22" height="22" valign="middle"> **感染 [Infected]** | Debuff | <img src="Assets/map/elite.png" width="18" height="18"> 感染棱柱 Boss | 回合结束时失去 **{Amount}** 点生命（穿透格挡攻击施加）。 |
| <img src="Assets/powers/mode_shift_power.png" width="22" height="22" valign="middle"> **形态转换 [Mode Shift]** | Boss 机制 | <img src="Assets/map/guardian_boss.png" width="18" height="18"> 守护者 Boss | 受到 **{Amount}** 点未被格挡伤害后转入防御模式。 |
| <img src="Assets/powers/sharp_hide_power.png" width="22" height="22" valign="middle"> **尖刺外壳 [Sharp Hide]** | Boss 机制 | <img src="Assets/map/guardian_boss.png" width="18" height="18"> 守护者 Boss | 攻击者每次攻击受到 **{Amount}** 点伤害。 |
| <img src="Assets/powers/split_power.png" width="22" height="22" valign="middle"> **分裂 [Split]** | Boss 机制 | <img src="Assets/map/slime_boss.png" width="18" height="18"> 史莱姆老大 Boss | 生命值 ≤ **50%** 时分裂成 2 只较小的史莱姆。 |
| <img src="Assets/powers/time_warp_power.png" width="22" height="22" valign="middle"> **时间扭曲 [Time Warp]** | Boss 机制 | 1 代 Boss 时间吞噬者（代码预留） | 玩家再打出 **{Amount}** 张牌后结束其回合并获得 2 力量。 |



### 怪物与 Boss

| 名称 | 编号 | 原版机制 | MOD 改后机制 |
| :--- | :--- | :--- | :--- |
| <img src="Assets/map/aeonglass_boss.png" width="22" height="22" valign="middle"> **永世沙漏 [Aeonglass]** | MON-01 | 永世沙漏 Boss 生成的凋零卡无法打出，直接进入消耗堆。 | 凋零卡可以**1c打出并消耗**，保留成长机制。 |
| <img src="Assets/map/elite.png" width="22" height="22" valign="middle"> **感染棱柱 [InfestedPrism]** | BOSS-01 | 开场污染玩家技能牌（活力火花），玩家打出污染技能为棱柱增加力量；4回合循环均为攻击。 | 移除【活力火花】机制，改为穿透格挡攻击时叠加【感染】[InfectedPower]，并重构 4 回合固定行动循环（支持 Mod 设置切换）。 |
| <img src="Assets/map/monster.png" width="22" height="22" valign="middle"> **红面具强盗 Bear [Bear]** | MONSTER-01 | 首回合【熊抱 BEAR_HUG】给予目标 1 层【易伤】 (`VulnerablePower`)。 | 【熊抱 BEAR_HUG】Debuff 修改为减少 2 点【敏捷】 (`DexterityPower` -2)。 |
| <img src="Assets/map/guardian_boss.png" width="22" height="22" valign="middle"> **守护者 [Guardian]** | AFP-BOSS-01 | - | 按 Acts From the Past v1.0.5 移植完整行动循环、形态转换、Mode Shift、Sharp Hide、动画与音效；提供不进入普通地图池的心灵绽放专用遭遇。 |
| <img src="Assets/map/hexaghost_boss.png" width="22" height="22" valign="middle"> **六火亡魂 [Hexaghost]** | AFP-BOSS-02 | - | 按 Acts From the Past v1.0.5 移植六火球状态、Divider/Sear/Inferno、灼伤升级、火焰特效与音效；提供不进入普通地图池的心灵绽放专用遭遇。 |
| <img src="Assets/map/slime_boss.png" width="22" height="22" valign="middle"> **史莱姆老大 [Slime Boss]** | AFP-BOSS-03 | - | 按 Acts From the Past v1.0.5 移植 Boss、酸液/尖刺史莱姆大型与中型单位及完整两级分裂链；提供七槽位的心灵绽放专用遭遇。 |

### 事件与遭遇

> [!tip]
> 完整的事件选择分支、触发条件与详细逻辑请查阅 **[docs/events.md](docs/events.md)**。

#### 原版事件调整
- **禅意织者 [Zen Weaver]**：删牌价格分别下调至 **75 / 150 金币**。
- **旧日垃圾堆 [Trash Heap]**：遗物奖励池加入 <img src="Assets/relics/omamori.png" width="18" height="18" valign="middle"> **御守**。
- **除虫者 / 科学怪人 / 药水的未来？**：初始选项中新增可配置的 **「离开」** 分支。

#### 一代事件回归
| 事件名称 | 先行条件 | 简要说明 |
| :--- | :--- | :--- |
| **老乞丐 [Old Beggar]** | 所有玩家金币 ≥ 75 | 给 75 金币后变身为牧师提供删牌服务。 |
| **诅咒书本 [Cursed Tome]** | Act 2（且无书） | 连续翻页测试，可获 <img src="Assets/relics/necronomicon.png" width="18" height="18" valign="middle"> **死灵之书** / <img src="Assets/relics/nilrys_codex.png" width="18" height="18" valign="middle"> **尼利的宝典** / <img src="Assets/relics/enchiridion.png" width="18" height="18" valign="middle"> **英雄宝典**。 |
| **面具强盗 [Masked Bandits]** | Act 2，层数 ≥ 23 | 交出所有金币或战斗获胜获得 <img src="Assets/relics/red_mask.png" width="18" height="18" valign="middle"> **红面具**。 |
| **J.A.X. [Augmenter]** | Act 2（可删牌 ≥ 2） | 获得 J.A.X. / 变化 2 张牌 / 获得 <img src="Assets/relics/mutagenic_strength.png" width="18" height="18" valign="middle"> **突变之力**。 |
| **神圣泉水 [The Divine Fountain]** | 牌组有诅咒牌 | 彻底清除牌组中所有诅咒牌（无伤害副作用）。 |
| **牧师 [Cleric]** | 所有玩家金币 ≥ 35 | 提供付钱回复 25% HP / 75 金删牌选项。 |
| **心灵绽放 [Mind Bloom]** | Act 3 | 可选一幕 Boss 战（支持连战挑战强化二战）、全牌升级（得 <img src="Assets/relics/mark_of_the_bloom.png" width="18" height="18" valign="middle"> **绽放印记**）或 999 金币。 |
| **大转盘 [Wheel of Change]** | - | 旋转随机转盘获得金币/遗物/全满治疗/删牌/诅咒/伤害。 |
| **红面具大人之墓 [Tomb of Lord Red Mask]** | Act 3 | 献上全部金币获得 <img src="Assets/relics/red_mask.png" width="18" height="18" valign="middle"> **红面具**（或持面具收获 222 金币）。 |
| **大图书馆 [The Library]** | Act 3 | 提供 **【阅读】**（跨职业选卡 1 张）与 **【睡觉】**（回复 33% HP）。 |


### 遗物

#### 新增遗物

| 遗物 | 类型 | 描述 |
| :--- | :--- | :--- |
| <img src="Assets/relics/sundial.png" width="22" height="22" valign="middle"> **日晷 [Sundial]** | <img src="Assets/profile/merchant.png" width="22" height="22" title="商人 (Merchant)"> | 每将抽牌堆洗牌 3 次（跨战斗保留计数），获得 3 点能量。 |
| <img src="Assets/relics/orange_pill.png" width="22" height="22" valign="middle"> **橙色药丸 [Orange Pill]** | <img src="Assets/profile/merchant.png" width="22" height="22" title="商人 (Merchant)"> | 同一回合打出攻击 / 技能 / 能力各一张后，移除所有负面效果（女王的魂缚锁链除外）。 |
| <img src="Assets/relics/dead_branch.png" width="22" height="22" valign="middle"> **枯木树枝 [Dead Branch]** | 稀有 | 每消耗一张牌，随机将一张牌加入手牌（虚无牌触发时给当回合保留）。 |
| <img src="Assets/relics/omamori.png" width="22" height="22" valign="middle"> **御守 [Omamori]** | <img src="Assets/map/event.png" width="22" height="22" title="事件 (Event)"> | 抵消接下来获得的 2 张诅咒牌（带计数器）。 |
| <img src="Assets/relics/peace_pipe.png" width="22" height="22" valign="middle"> **宁静烟斗 [Peace Pipe]** | 稀有 | 在火堆新增"烟斗"选项，可删除一张牌。 |
| <img src="Assets/relics/smiling_mask.png" width="22" height="22" valign="middle"> **微笑面具 [Smiling Mask]** | <img src="Assets/profile/merchant.png" width="22" height="22" title="商人 (Merchant)"> | 删牌价格固定为 50 金币。 |
| <img src="Assets/relics/coffie_cup.png" width="22" height="22" valign="middle"> **咖啡杯 [Coffee Cup]** | <img src="Assets/profile/darv.png" width="22" height="22" title="达尔夫 (Darv)"> | 无法在火堆休息，但每回合 +1 费用。 |
| <img src="Assets/relics/fusion_hammer.png" width="22" height="22" valign="middle"> **融合之锤 [Fusion Hammer]** | <img src="Assets/profile/darv.png" width="22" height="22" title="达尔夫 (Darv)"> | 无法锻造，但每回合 +1 费用。 |
| <img src="Assets/relics/curse_key.png" width="22" height="22" valign="middle"> **诅咒钥匙 [Curse Key]** | <img src="Assets/profile/darv.png" width="22" height="22" title="达尔夫 (Darv)"> | 每回合 +1 费用，每次打开宝箱获得一张随机诅咒。**仅限单人模式出现，单人模式下可通过右下角的"跳过宝箱"按钮（ProceedButton）直接离开，同时规避诅咒。** |
| <img src="Assets/relics/dwarf_anvil.png" width="22" height="22" valign="middle"> **矮人铁砧 [Dwarf Anvil]** | <img src="Assets/profile/merchant.png" width="22" height="22" title="商人 (Merchant)"> | 拾起时选择 3 张牌附加"锻造"附魔，被附魔的牌费用永久 -1（最低 0 费）。 |
| <img src="Assets/relics/wrist_blade.png" width="22" height="22" valign="middle"> **袖箭 [Wrist Blade]** | 罕见 | <img src="Assets/profile/silent.png" width="18" height="18" valign="middle" title="静默猎手"> 猎人专属。费用为 0 的攻击牌额外造成 4 点伤害。 |
| <img src="Assets/relics/hovering_kite.png" width="22" height="22" valign="middle"> **悬浮风筝 [Hovering Kite]** | 普通 | <img src="Assets/profile/silent.png" width="18" height="18" valign="middle" title="静默猎手"> 猎人专属。你在每回合第一次弃牌时，获得 1 点能量。 |
| <img src="Assets/relics/soul_contract.png" width="22" height="22" valign="middle"> **灵魂契约 [Soul Contract]** | <img src="Assets/profile/merchant.png" width="22" height="22" title="商人 (Merchant)"> | 选择牌组中的 1 张有消耗的牌，永久去除其消耗属性。 |
| <img src="Assets/relics/nilrys_codex.png" width="22" height="22" valign="middle"> **尼利的宝典 [Nilry's Codex]** | <img src="Assets/map/event.png" width="22" height="22" title="事件 (Event)"> | 每回合结束时，从 3 张随机**升级版**卡牌中选择 1 张洗入抽牌堆。（RELIC-04：MOD 改为展示升级版） |
<!-- | <img src="Assets/relics/shabbydoll.png" width="22" height="22" valign="middle"> **破旧的玩偶 [Shabby Doll]** | <img src="Assets/profile/tanx.png" width="22" height="22" title="探克斯 (Tanx)"> | 拾起时，扣除 50% 最大生命值上限，并将牌组中所有的基础【打击】与【防御】替换为升级后的【巫术打击+】与【巫术防御+】。（先古之民 Tanx 专属替换撕咬机制选项） | -->


#### 原版调整

| 遗物 | 类型 | 原版 | MOD 改后 |
| :--- | :--- | :--- | :--- |
| <img src="Assets/relics/sturdy_clamp.png" width="22" height="22" valign="middle"> **坚固钳子 [Sturdy Clamp]** | 稀有 | 保留 10 护甲 | 保留 **15 护甲** |
| <img src="Assets/relics/preserved_fog.png" width="22" height="22" valign="middle"> **活雾 [Preserved Fog]** | <img src="Assets/profile/vakuu.png" width="22" height="22" title="瓦库 (Vakuu)"> | 删除 3 张牌 | 删除 **4 张牌** |
| <img src="Assets/relics/red_mask.png" width="22" height="22" valign="middle"> **红面具 [Red Mask]** | <img src="Assets/map/event.png" width="22" height="22" title="事件 (Event)"> | 在一般共享遗物池中 | 从一般共享遗物池**移除**，只通过红面具相关事件获得 |
| <img src="Assets/relics/history_course.png" width="22" height="22" valign="middle"> **历史课 [History Course]** | <img src="Assets/map/event.png" width="22" height="22" title="事件 (Event)"> | 只重复上回合最后打出的攻击牌 | 回调为重复上回合最后打出的**攻击牌或技能牌** |
| <img src="Assets/relics/diamond_diadem.png" width="22" height="22" valign="middle"> **诺努佩佩的钻石王冠 [Nonupeipe's Diamond Diadem]** | <img src="Assets/profile/nonupeipe.png" width="22" height="22" title="诺努佩佩 (Nonupeipe)"> | 战斗开始时获得 20 格挡，并在下回合开始时保留 | 回调为：一回合打出不超过 2 张牌时，受到的敌人伤害减半 |
| <img src="Assets/relics/toasty_mittens.png" width="22" height="22" valign="middle"> **烘焙手套 [Toasty Mittens]** | <img src="Assets/profile/tezcatara.png" width="22" height="22" title="提兹卡塔拉 (Tezcatara)"> | 回合开始自动从抽牌堆消耗 1 张牌并 +1 力量 | 回合开始支持从手牌中选择 1 张卡牌消耗（**提供 Skip 选项**），成功消耗后才获得 1 点力量 |
| <img src="Assets/relics/signet_ring.png" width="22" height="22" valign="middle"> **图章戒指 [Signet Ring]** | <img src="Assets/profile/nonupeipe.png" width="22" height="22" title="诺努佩佩 (Nonupeipe)"> | 获得 888 金币 | 回调获得 **999** 金币 |

---

## 项目知识库

👉 **[文档知识库总索引 (docs/README.md)](docs/README.md)**

-  **[能力与效果手册](docs/powers.md)**：包含 Mod 内所有 Buff、Debuff、Boss 机制能力及原版修改说明。
-  **[事件与遭遇手册](docs/events.md)**：包含 1 代回归事件、原版事件调整、选择分支与触发条件。
-  **[STS2 Mod 制作指南](docs/sts2-modding-guide.md)**：从零开始制作 STS2 Mod 的教程。
-  **[图片生成自动化脚本](image_gen/)**：遗物工具从主图自动提取主题色，一次生成 94×94 主图、256×256 大图和带 3px 主题色外环的 94×94 outline。
-  **[技术分析报告](docs/README.md#技术分析报告)**：红面具事件分层架构分析、宝箱房跳过功能技术报告等。
-  **[未完成需求清单](docs/balance-changes.md)** 与 **[版本变更日志](CHANGELOG.md)**。

## 关于尖塔

1. [杀戮尖塔2 Wiki](https://sts2.huijiwiki.com/wiki/%E9%A6%96%E9%A1%B5)
2. [杀戮尖塔1 Wiki](https://sts.huijiwiki.com/wiki/%E9%A6%96%E9%A1%B5)
3. [Steam 官方公告](https://steamcommunity.com/games/2868840/announcements/)
4. [玩家数据统计](https://spire-codex.com/)
5. [模组开发教程](https://tutorials.sts2modding.com/)

## 许可证 (License)

本项目基于 **[GNU General Public License v3.0 (GPL-3.0)](LICENSE)** 协议开源。

- 允许免费使用、修改（二次开发）及商业分发。
- **开源传染性约束**：任何使用本项目代码进行二次开发（二开）或商业分发衍生作品，**必须以 GPL-3.0 协议开源其全部源代码**，不得闭源。

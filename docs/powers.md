# Sts2BalanceMod — 能力与效果手册 (Powers Guide)

本文档归纳整理了 **Sts2BalanceMod** 中所有新增、重构以及由卡牌/遗物/ Boss / 遭遇战引入的**能力（Powers / Debuffs / Mechanisms）**。

---

## 目录

- [玩家能力 (Player Buffs)](#玩家能力-player-buffs)
- [探克斯与战斗减益 (Debuffs)](#探克斯与战斗减益-debuffs)
- [Boss 与遭遇机制能力 (Boss & Encounter Powers)](#boss-与遭遇机制能力-boss--encounter-powers)
- [原版能力与减益调整 (Modified Vanilla Powers & Debuffs)](#原版能力与减益调整-modified-vanilla-powers--debuffs)

---

## 玩家能力 (Player Buffs)

由玩家打出卡牌或装备遗物获得的正面能力。

| 图标 | 能力名称 | C# 类名 / ID | 来源 | 效果说明 |
| :---: | :--- | :--- | :--- | :--- |
| <img src="../Assets/powers/electrodynamics_power.png" width="22" height="22" valign="middle"> | **电动力学 [Electrodynamics]** | `ElectrodynamicsPower`<br>`STS2BALANCEMOD-ELECTRODYNAMICS_POWER` | <img src="../Assets/profile/defect.png" width="18" height="18"> 故障机器人稀有能力卡「电动力学」 | [闪电]球改为攻击所有敌人。 |
| <img src="../Assets/powers/evolve_power.png" width="22" height="22" valign="middle"> | **进化 [Evolve]** | `EvolvePower`<br>`STS2BALANCEMOD-EVOLVE_POWER` | <img src="../Assets/profile/ironclad.png" width="18" height="18"> 铁甲战士罕见能力卡「进化」 | 每当你抽到一张状态牌，抽 **1**（升级：**2**）张牌。 |
| <img src="../Assets/powers/step_by_step_power.png" width="22" height="22" valign="middle"> | **步步为营 [Step by Step]** | `StepByStepPower`<br>`STS2BALANCEMOD-STEP_BY_STEP_POWER` | <img src="../Assets/profile/silent.png" width="18" height="18"> 静默猎手稀有技能卡「步步为营」 | 每回合开始时，多抽 1 张牌并获得 1 点能量。持续 **X**（升级：**X+1**）回合。 |
| <img src="../Assets/powers/mutagenic_strength.png" width="22" height="22" valign="middle"> | **突变之力 [Mutagenic Strength]** | `MutagenicStrengthPower`<br>`STS2BALANCEMOD-MUTAGENIC_STRENGTH_POWER` | 遗物 <img src="../Assets/relics/mutagenic_strength.png" width="18" height="18"> 突变之力（J.A.X. 事件） | 战斗开始时获得 3 点临时力量，首个回合结束时失去 3 点力量。 |

---

## 探克斯与战斗减益 (Debuffs)

施加于敌人或玩家身上的负面状态。

| 图标 | 能力名称 | C# 类名 / ID | 来源 | 效果说明 |
| :---: | :--- | :--- | :--- | :--- |
| <img src="../Assets/powers/sorcery_vulnerable.png" width="22" height="22" valign="middle"> | **巫术易伤 [Sorcery Vulnerable]** | `SorceryVulnerable`<br>`STS2BALANCEMOD-SORCERY_VULNERABLE` | 先古卡「巫术打击」（探克斯 Tanx） | 受到的攻击伤害增加 **75%**。若本回合受到过攻击，回合结束时减少 1 层。 |
| <img src="../Assets/powers/sorcery_weak.png" width="22" height="22" valign="middle"> | **巫术虚弱 [Sorcery Weak]** | `SorceryWeak`<br>`STS2BALANCEMOD-SORCERY_WEAK` | 先古卡「巫术防御」（探克斯 Tanx） | 造成的攻击伤害减少 **50%**。若本回合进行过攻击，回合结束时减少 1 层。 |
| <img src="../Assets/powers/infected_power.png" width="22" height="22" valign="middle"> | **感染 [Infected]** | `InfectedPower`<br>`STS2BALANCEMOD-INFECTED_POWER` | Boss <img src="../Assets/map/elite.png" width="18" height="18"> 感染棱柱（穿透格挡攻击） | 在你的回合结束时，失去 **{Amount}** 点生命。 |
| <img src="../Assets/powers/draw_reduction_power.png" width="22" height="22" valign="middle"> | **抽牌减少 [Draw Reduction]** | `DrawReductionPower`<br>`STS2BALANCEMOD-DRAW_REDUCTION_POWER` | 1 代回归减益 / 遭遇战 | 持续 **{Amount}** 回合，每回合开始时少抽 1 张牌。 |

---

## Boss 与遭遇机制能力 (Boss & Encounter Powers)

由 Boss 或特定战斗遭遇持有的特殊机制与行为能力。

| 图标 | 能力名称 | C# 类名 / ID | 来源 | 效果说明 |
| :---: | :--- | :--- | :--- | :--- |
| <img src="../Assets/powers/mode_shift_power.png" width="22" height="22" valign="middle"> | **形态转换 [Mode Shift]** | `ModeShiftPower`<br>`STS2BALANCEMOD-MODE_SHIFT_POWER` | Boss <img src="../Assets/map/guardian_boss.png" width="18" height="18"> 守护者 [Guardian] | 守护者在受到 **{Amount}** 点未被格挡的伤害后转入防御模式。 |
| <img src="../Assets/powers/sharp_hide_power.png" width="22" height="22" valign="middle"> | **尖刺外壳 [Sharp Hide]** | `SharpHidePower`<br>`STS2BALANCEMOD-SHARP_HIDE_POWER` | Boss <img src="../Assets/map/guardian_boss.png" width="18" height="18"> 守护者（防御形态） | 攻击者每次对其发起攻击时受到 **{Amount}** 点伤害。 |
| <img src="../Assets/powers/split_power.png" width="22" height="22" valign="middle"> | **分裂 [Split]** | `SplitPower`<br>`STS2BALANCEMOD-SPLIT_POWER` | Boss <img src="../Assets/map/slime_boss.png" width="18" height="18"> 史莱姆老大 [Slime Boss] | 当生命 **50%** 或以下时，分裂成 2 只较小的史莱姆，每只拥有当前的生命值。 |
| <img src="../Assets/powers/time_warp_power.png" width="22" height="22" valign="middle"> | **时间扭曲 [Time Warp]** | `TimeWarpPower`<br>`STS2BALANCEMOD-TIME_WARP_POWER` | 1 代 Boss「时间吞噬者 [Time Eater]」（代码预留 / 暂未实装） | 每当玩家再打出 **{Amount}** 张牌，强制结束该玩家的回合，并获得 2 点力量。 |

---

## 原版能力与减益调整 (Modified Vanilla Powers & Debuffs)

通过 Harmony Patch 对游戏原版现有能力或怪物 Debuff 效果进行的重构与调整。

| 调整项目 | 涉及卡牌 / 怪物 / 机制 | 原版机制 | MOD 改后机制 |
| :--- | :--- | :--- | :--- |
| **幽魂形态负面 [Wraith Form Power]** | <img src="../Assets/profile/silent.png" width="18" height="18"> 静默猎手技能卡「幽魂形态」 | 打出后获得无实体，同时施加 `WraithFormPower` 负面效果（每回合结束时 -1 敏捷）。 | 打出时直接拦截 `WraithFormPower` 的施加，**彻底删除每回合减敏捷负面效果**，卡牌悬浮提示同步移除了敏捷说明。 |
| **红面具熊抱 [Bear Hug]** | <img src="../Assets/map/monster.png" width="18" height="18"> 红面具强盗熊 Bear | 首回合【熊抱 BEAR_HUG】给予目标 1 层【易伤】 (`VulnerablePower`)。 | 【熊抱 BEAR_HUG】Debuff 效果修改为**减少 2 点敏捷** (`DexterityPower` -2)。 |
| **认知偏差 [Biased Cognition]** | <img src="../Assets/profile/defect.png" width="18" height="18"> 故障机器人能力卡「认知偏差」 | 每回合开始时 -1 集中，永久持续倒扣（导致负集中无限累积）。 | 当集中降至 0 时自动移除该能力，不再倒扣超出提升数值的集中点数，同时彻底修复集中归零后结束回合按钮不响应的 BUG。 |
| **创世之柱 [Pillar of Creation]** | <img src="../Assets/profile/regent.png" width="18" height="18"> 储君能力卡「创世之柱」 | 每回合第一次生成卡牌时获得 5 / 8 点格挡。 | 回调为每生成一张牌触发，格挡数值调整为 **3 / 4 点**。 |


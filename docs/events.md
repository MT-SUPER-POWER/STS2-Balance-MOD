# Sts2BalanceMod — 事件与遭遇手册 (Events Guide)

本文档归纳整理了 **Sts2BalanceMod** 中所有调整的原版事件以及新增回归的 1 代经典事件。

---

## 目录

- [原版事件调整 (Modified Vanilla Events)](#原版事件调整-modified-vanilla-events)
- [一代回归事件 (STS1 Classic Events Return)](#一代回归事件-sts1-classic-events-return)

---

## 原版事件调整 (Modified Vanilla Events)

对游戏原版已有事件选项、价格与逻辑进行的平衡调整。

| 事件配图 | 事件名称 | C# 类名 / ID | 调整内容 |
| :---: | :--- | :--- | :--- |
| <img src="../Assets/events/zen_weaver.png" width="120"> | **禅意织者 [Zen Weaver]** | `ZenWeaverCostPatch` | **删牌价格大幅下调**：<br>• 删 1 张牌价格从 125 金币**下调至 75 金币**（事件触发门槛同步降至 75 金）。<br>• 删 2 张牌价格从 250 金币**下调至 150 金币**。 |
| <img src="../Assets/events/trash_heap.png" width="120"> | **旧日垃圾堆 [Trash Heap]** | `TrashHeapAddCustomRelicAndCard` | 遗物奖励池中加入 <img src="../Assets/relics/omamori.png" width="18" height="18"> **御守 [Omamori]**。 |
| <img src="../Assets/events/bugslayer.png" width="120"> | **除虫者 [Bugslayer]** | `EventLeaveOptionPatches` | 初始选项中**新增「离开」分支**，允许玩家直接走开而无须获取卡牌（可在 Mod 设置中开关）。 |
| <img src="../Assets/events/tinker_time.png" width="120"> | **科学怪人 [Tinker Time]** | `EventLeaveOptionPatches` | 初始选项中**新增「离开」分支**，允许玩家直接走开而不用强制接受突变卡（可在 Mod 设置中开关）。 |
| <img src="../Assets/events/the_future_of_potions.png" width="120"> | **药水的未来？ [The Future of Potions]** | `EventLeaveOptionPatches` | 初始选项中**新增「离开」分支**，允许玩家保留药水直接离开（可在 Mod 设置中开关）。 |

---

## 一代回归事件 (STS1 Classic Events Return)

从《杀戮尖塔 1》完整移植并重新制作的经典事件，包含全新交互、特殊逻辑与联动机制。

| 事件配图 | 事件名称 | 先行条件 | 详细选项与效果 |
| :---: | :--- | :--- | :--- |
| <img src="../Assets/events/old_beggar.png" width="120"> | **老乞丐 [Old Beggar]**<br>`OldBeggar` | 所有玩家金币 ≥ 75 | • **【给金币】**：失去 75 金币，乞丐脱下外套变身为[牧师]，从牌组中移除 1 张牌。<br>• **【离开】**：不给钱直接离开。 |
| <img src="../Assets/events/cursed_tome.png" width="120"> | **诅咒书本 [Cursed Tome]**<br>`CursedTome` | Act 2，且牌组无对应书籍遗物 | 连续翻页扣血（1+2+3 HP 或 2+3+3 HP）：<br>• **【拿走】**：随机获得 <img src="../Assets/relics/necronomicon.png" width="18" height="18" valign="middle"> **死灵之书**、<img src="../Assets/relics/nilrys_codex.png" width="18" height="18" valign="middle"> **尼利的宝典** 或 <img src="../Assets/relics/enchiridion.png" width="18" height="18" valign="middle"> **英雄宝典**。<br>• **【停止 / 离开】**：合上书本离开。 |
| <img src="../Assets/events/mirror_mask3.png" width="120"> | **面具强盗 [Masked Bandits]**<br>`MaskedBandits` | Act 2，层数 ≥ 23，且无人持有红面具 | • **【交钱】**：失去所有金币。<br>• **【开战】**：与红面具三人帮（Pointy, Romeo, Bear）战斗，获胜获得 <img src="../Assets/relics/red_mask.png" width="18" height="18"> **红面具**。<br>*（强盗战斗归入事件战斗图鉴；Bear 首回合【熊抱】改为施加 -2 敏捷）* |
| <img src="../Assets/events/augmenter.png" width="120"> | **增益研究者 [Augmenter]**<br>`Augmenter` | Act 2，且所有玩家牌组中可移除牌 ≥ 2 张 | • **【试一下 J.A.X.】**：将 J.A.X. 卡牌加入牌组。<br>• **【当实验对象】**：变化 2 张牌。<br>• **【喝突变剂】**：获得 <img src="../Assets/relics/mutagenic_strength.png" width="18" height="18"> **突变之力** 遗物。 |
| <img src="../Assets/events/the_divine_fountain.png" width="120"> | **神圣泉水 [The Divine Fountain]**<br>`TheDivineFountain` | 所有玩家牌组中存在可移除的诅咒牌 | • **【喝水】**：移除牌组中的**所有诅咒牌**（已删除了原版的伤害副作用）。<br>• **【离开】**：直接走开。 |
| <img src="../Assets/events/cleric.png" width="120"> | **牧师 [Cleric]**<br>`Cleric` | 所有玩家金币 ≥ 35 | • **【治疗】**：失去 35 金币，回复 25% 最大生命值。<br>• **【净化】**：失去 75 金币，移除 1 张牌。<br>• **【离开】**：不接受服务走开。 |
| <img src="../Assets/events/mind_bloom.png" width="120"> | **心灵绽放 [Mind Bloom]**<br>`MindBloom` | Act 3 | • **【我即战争】**：与第一幕 Boss 战斗，胜获得 50 金 + 稀有遗物；战胜后可选择 **【继续深入】** 接受随机二战遭遇（守护者/六火亡魂/史莱姆老大 + 随机强化），胜额外得 100 金 + 稀有遗物 + 罕见遗物。<br>• **【我即清醒】**：升级牌组中所有牌，并获得 <img src="../Assets/relics/mark_of_the_bloom.png" width="18" height="18"> **绽放印记**。<br>• **【我即富足】**：（层数 < 41，多人 < 38）获得 999 金币，牌组加入 2 张「凡庸」。 |
| <img src="../Assets/events/wheel_of_change.png" width="120"> | **大转盘 [Wheel of Change]**<br>`WheelOfChange` | 无 | 旋转随机转盘：<br>• 金币 (+100~300)<br>• 随机遗物<br>• 恢复全部生命<br>• 获得诅咒「腐朽」<br>• 移除 1 张牌<br>• 受到伤害 |
| <img src="../Assets/events/tomb_of_lord_red_mask.png" width="120"> | **红面具大人之墓 [Tomb of Lord Red Mask]**<br>`TombOfLordRedMask` | Act 3，且无人持有红面具 | • **【献上敬意】**：献上所有金币，获得 <img src="../Assets/relics/red_mask.png" width="18" height="18"> **红面具**。<br>• **【戴上红面具】**：（已持有红面具）收获 222 金币。<br>• **【离开】**：不打扰墓穴。 |
| <img src="../Assets/events/the_library.png" width="120"> | **大图书馆 [The Library]**<br>`TheLibrary` | Act 3 | • **【阅读】**：从 20 张跨职业卡牌中选择 1 张加入牌组。<br>• **【睡觉】**：回复 33% 最大生命值。 |

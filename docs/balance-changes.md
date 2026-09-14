# STS2 平衡调整需求清单

> 本文档追踪**未完成 / 待处理**的调整项，并保留最近完成任务的验收状态。
> 所有已实现调整的玩家说明请查看 [README.md](../README.md#调整内容) 的「调整内容」章节。

---

## 待办项

### 卡牌

- [x] **CARD-WITHER-01** — 凋萎（Wither）打出费用随 FakeUpgrade 成长
  - 目前: Mod 将【凋萎】由不可打出改为 1 费消耗；永世沙漏（Aeonglass）施放技能触发 `FakeUpgrade()` 时仅增加回合结束受到伤害（每次 +3），打出费用恒为 1 费。
  - 目标: 对 `Wither.FakeUpgrade` 打 Harmony Patch，使其每经历 2 次升级，打出消耗的能量费用 +1。
  - 数值: 
    - 0 次升级（初始）：1 费
    - 1 次升级：1 费
    - 2 次升级：2 费（+1 费）
    - 3 次升级：2 费
    - 4 次升级：3 费（+2 费）
    - 费用成长公式：`费用 = 1 + (FakeUpgradeLevel / 2)`
  - 备注: 沙漏生成新【凋萎】时会通过 `MatchWitherToUpgradeCount` 自动补全 `FakeUpgrade()` 调用，费用随之同步增长。


### 遗物

- [x] **RELIC-MUTAGENIC-01** — 突变之力（Mutagenic Strength）重做为递减流失 Debuff
  - 目前: 战斗开始时获得 3 点临时力量，在第 1 回合结束时立即失去全部 3 点力量。
  - 目标: 战斗开始时获得 3 点力量，并附加 3 层 Debuff【突变衰退/力量流失】；玩家每回合结束时流失 1 点力量并扣减 1 层 Debuff，扣完 3 点后 Debuff 自动移除。
  - 数值: 
    - 入场获得: +3 力量，3 层力量流失 Debuff
    - 第 1 回合结束: 失去 1 点力量（净剩 2 力量），Debuff 剩余 2 层
    - 第 2 回合结束: 失去 1 点力量（净剩 1 力量），Debuff 剩余 1 层
    - 第 3 回合结束: 失去 1 点力量（净剩 0 力量），Debuff 扣完移除
  - 备注: 流失效果为独立 Debuff，可正常与人工制品（Artifact）交互（抵挡流失效果后保留永久 +3 力量）。

- [x] **RELIC-ROYAL-STAMP-01** — 王室印章（Royal Stamp / 皇室印章）放宽附魔类型约束
  - 目前: 遗物【王室印章】所对应的附魔【王室认证】（Royally Approved）仅允许附魔攻击牌（Attack）和技能牌（Skill），无法附魔能力牌。
  - 目标: 降低附魔约束，允许能力牌（Power）也可以附魔【王室认证】（获得固有与保留）；拾取【王室印章】时可选能力牌，并同步更新多语言遗物描述文本。
  - 数值: 可附魔类型由「攻击牌、技能牌」放宽为「攻击牌、技能牌、能力牌」。
  - 备注: 通过 Harmony Patch 拦截 `RoyallyApproved.CanEnchantCardType`，当 `cardType == CardType.Power` 时返回 `true`。

### BOSS

### 怪物

---

### 事件

### 卡牌

### 遗物

### 卡牌

### 基础设施

### BOSS

### 遗物

---

## 现有的问题以及无法解决的问题

### BUG 列表

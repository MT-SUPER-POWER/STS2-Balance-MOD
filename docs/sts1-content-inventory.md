# STS1 内容回归盘点清单

> 本文档基于 `docs/references/ActsFromThePast` 整理一代内容资产，用于判断哪些内容适合合并进 `STS2-Balance-MOD`。
> `balance-changes.md` 只记录执行状态；本文件记录完整候选池和取舍理由。

---

## 1. 合并原则

| 优先级 | 类型 | 说明 |
| ------ | ---- | ---- |
| P0 | 当前需求直接命中 | 已在 `balance-changes.md` 中出现，或能直接解决现有待办 |
| P1 | 轻量回归 | 事件卡、事件遗物、单个 Boss 等可独立移植的内容 |
| P2 | 系统级回归 | 需要完整幕、遭遇池、地图、VFX、音频或小游戏配套的内容 |
| P3 | 暂不合并 | 与二代当前设计冲突，或维护成本明显高于收益 |

**当前建议：**

- 第一批只合并已经选定、且 ActsFromThePast 有参考实现的内容。
- 第二层、第三层后续可以像第一层暗港/密林一样，新增“一代回归区域”来区分一代和二代怪物及最终 Boss。
- 不直接复制 ActsFromThePast 的三幕系统；需要时只抽取可独立运行的模型、事件、补丁和本地化。
- 双拳机器人是二代原生事件，不是 ActsFromThePast 的 `BronzeAutomaton`，当前机器没有 STS2 反编译源码，暂缓到源码到位后处理。
- 每个合并项进入实现前，都需要在 `balance-changes.md` 中拥有独立编号。

---

## 2. 总览

| 分类 | ActsFromThePast 已有内容 | 建议 |
| ---- | ------------------------ | ---- |
| 一代三幕 | Exordium、The City、The Beyond | P2，暂不整幕移植 |
| Boss | 守护者、六火亡魂、史莱姆老大、铜制机械人偶、第一勇士、收藏家、觉醒者、甜圈八体、时间吞噬者 | P1；时间吞噬者为 P0 |
| 精英 | 地精大块头、乐加维林、三哨卫、奴隶贩子、地精首领、扎人的书、大脑袋、天罚、拜蛇术士 | P2，等 Boss 跑通后再评估 |
| 普通怪 | 三幕常规怪与事件怪 | P2，暂不合并 |
| 事件 | 一代全部事件，除 A Note For Yourself | P1/P2，优先挑事件奖励或简单事件 |
| 事件卡/诅咒 | 噬咬、J.A.X.、疯狂、死灵诅咒、疼痛、寄生、仪式匕首、盒子 | P1 |
| 事件遗物 | 鲜血神像、金神像、死灵之书等 17 个 | P1 |
| 支撑 Power | 时间扭曲、易变、飞行、金属化、再生等 | 作为怪物/遗物依赖按需合并 |
| 补丁/配置 | Rebalanced Mode、房间事件、音频/VFX、调试补丁 | 按需参考，不整体合并 |

---

## 3. 第一批确认合并范围

| 编号 | 内容 | 来源 | 说明 |
| ---- | ---- | ---- | ---- |
| STS1-BOSS-01 | 时间吞噬者 | `Acts/TheBeyond/Enemies/TimeEater.cs`、`Acts/TheBeyond/Encounters/Boss/TimeEaterBoss.cs`、`Powers/TimeWarpPower.cs` | 三层一代回归 Boss 候选 |
| STS1-BOSS-02 | 收藏家 | `Acts/TheCity/Enemies/Collector.cs`、`Acts/TheCity/Encounters/Boss/CollectorBoss.cs` | 二层一代回归 Boss 候选 |
| STS1-EVENT-01 | 诅咒书本 | `Acts/TheCity/Events/CursedTome.cs` | 奖励死灵之书、尼利的宝典、英雄宝典之一 |
| STS1-EVENT-02 | 红面具事件 | `Acts/TheCity/Events/MaskedBandits.cs`、`Acts/TheCity/Encounters/Normal/RedMaskBanditsEvent.cs`、`Patches/RoomEvents/MaskedBanditsPatches.cs` | 需要同步红面具帮战斗逻辑；`RedMask`、`HandOfGreed` 依赖需确认二代可用模型 |
| STS1-EVENT-03 | 增益研究者 | `Acts/TheCity/Events/Augmenter.cs` | J.A.X. 事件，需要 `Cards/Jax.cs` 与突变之力相关逻辑 |
| STS1-EVENT-04 | 神圣泉水 | `SharedEvents/TheDivineFountain.cs` | 删除诅咒事件 |
| STS1-EVENT-05 | 牧师 | `Acts/Exordium/Events/Cleric.cs` | 治疗 / 删牌事件 |
| STS1-EVENT-06 | 心灵绽放 | `Acts/TheBeyond/Events/MindBloom.cs`、`Patches/Events/MindBloomPatches.cs` | 包含 999 金币打一层 Boss、绽放印记不能再回复等分支 |
| STS1-EVENT-07 | 大转盘 | `SharedEvents/WheelOfChange.cs` | 随机奖励/惩罚事件 |
| STS1-CARD-01 | 本批事件依赖卡牌 | `Cards/Jax.cs`、`Cards/Necronomicurse.cs` | J.A.X. 与死灵之书链路依赖 |
| STS1-RELIC-01 | 本批事件依赖遗物 | `Relics/Necronomicon.cs`、`Relics/NilrysCodex.cs`、`Relics/Enchiridion.cs`、`Relics/MarkOfTheBloom.cs` 等 | 先围绕本批事件补齐，不做全遗物池；红面具若二代没有可用模型再单独补 |

---

## 4. P0：当前需求直接命中

| 编号 | 内容 | 来源 | 当前判断 |
| ---- | ---- | ---- | -------- |
| STS1-BOSS-01 | 时间吞噬者 | `Acts/TheBeyond/Enemies/TimeEater.cs`、`Encounters/Boss/TimeEaterBoss.cs`、`Powers/TimeWarpPower.cs` | 直接对应 `MON-02`，优先移植 |
| STS1-BOSS-02 | 收藏家 | `Acts/TheCity/Enemies/Collector.cs`、`Encounters/Boss/CollectorBoss.cs` | 本批新增 Boss |
| STS1-DOC-01 | 一代内容资产清单 | 本文档 | 已建立候选池，后续从这里挑选 |
| EVENT-01 | 双拳机器人事件血量降低 | 二代原生事件 | 不是 `BronzeAutomaton`；需要 STS2 反编译源码后再定位模型与补丁点 |

---

## 5. P1：轻量回归候选

### 5.1 事件卡与诅咒

| 内容 | 文件 | 合并价值 | 注意事项 |
| ---- | ---- | -------- | -------- |
| 噬咬 | `Cards/Bite.cs` | 事件奖励基础卡 | 需要确认二代是否已有同名/类似卡 |
| J.A.X. | `Cards/Jax.cs` | 增益研究者事件奖励 | 消耗生命换力量，适合单卡移植 |
| 疯狂 | `Cards/Madness.cs` | 事件奖励 | 需要确认随机减费在二代中的持续战斗实现 |
| 死灵诅咒 | `Cards/Necronomicurse.cs` | 死灵之书依赖 | 与诅咒池、御守、诅咒钥匙有联动价值 |
| 疼痛 | `Cards/Pain.cs` | 诅咒池扩展 | 与御守/诅咒钥匙联动 |
| 寄生 | `Cards/Parasite.cs` | 诅咒池扩展 | 最大生命惩罚需要补丁验证 |
| 仪式匕首 | `Cards/RitualDagger.cs` | 事件奖励 | 永久成长逻辑需要确认存档/升级兼容 |
| 盒子 | `Cards/TheBox.cs` | Rebalanced Mode 事件奖励 | 与商店移除逻辑相关，暂列候选 |

### 5.2 事件遗物

| 内容 | 文件 | 合并价值 | 注意事项 |
| ---- | ---- | -------- | -------- |
| 金神像 | `Relics/GoldenIdol.cs` | 一代代表性事件遗物 | 金币掉落倍率需确认二代奖励管线 |
| 鲜血神像 | `Relics/BloodyIdol.cs` | 与金币事件联动 | 需要监听金币获得 |
| 蛇的头 | `Relics/SsserpentHead.cs` | 问号房金币奖励 | 需要房间进入钩子 |
| 牧师的脸 | `Relics/FaceOfCleric.cs` | 战后最大生命成长 | 逻辑相对独立 |
| 奇怪蘑菇 | `Relics/OddMushroom.cs` | 易伤减伤 | 需要伤害修正钩子 |
| 突变之力 | `Relics/MutagenicStrength.cs` | 战斗开始临时力量 | 逻辑相对独立 |
| 死灵之书 | `Relics/Necronomicon.cs` | 高辨识度事件遗物 | 依赖死灵诅咒、攻击牌重复打出 |
| 尼利的宝典 | `Relics/NilrysCodex.cs` | 回合末选牌洗入 | UI/选择流程较重 |
| 英雄宝典 | `Relics/Enchiridion.cs` | 战斗开始随机能力牌 | 需要临时免费打出逻辑 |
| 弯曲铁钳 | `Relics/WarpedTongs.cs` | 回合开始临时升级 | 需要确认临时升级生命周期 |
| 恩洛斯的礼物 | `Relics/NlothsGift.cs` | 稀有牌概率调整 | 需要奖励池概率钩子 |
| 恩洛斯的饥饿的脸 | `Relics/NlothsHungryFace.cs` | 下个宝箱为空 | 与诅咒钥匙、宝箱跳过问题有关 |
| 绽放印记 | `Relics/MarkOfTheBloom.cs` | 禁止回复 | 需要全局回复拦截 |
| 邪教徒头套 | `Relics/CultistHeadpiece.cs` | 偏彩蛋 | P3 候选 |
| 地精容貌 | `Relics/GremlinVisage.cs` | 开局虚弱 | 负面遗物，需谨慎 |
| 精灵便便 | `Relics/SpiritPoop.cs` | 事件结果 | 主要是收藏/彩蛋 |
| 鲜血储蓄袋 | `Relics/BloodBank.cs` | Rebalanced Mode 新遗物 | 非一代原版，暂缓 |

### 5.3 简单事件候选

| 内容 | 文件 | 初步判断 |
| ---- | ---- | -------- |
| 牧师 | `Exordium/Events/Cleric.cs` | 商店外治疗/删牌，和本项目删牌价格调整有关 |
| 大金鱼 | `Exordium/Events/BigFish.cs` | 奖励结构清晰，可作为事件移植试点 |
| 金神像 | `Exordium/Events/GoldenIdol.cs` | 与金神像遗物配套 |
| 增益研究者 | `TheCity/Events/Augmenter.cs` | 与 J.A.X./突变之力配套 |
| 诅咒书本 | `TheCity/Events/CursedTome.cs` | 与三本书遗物配套，但链路较长 |
| 复制祭坛 | `SharedEvents/Duplicator.cs` | 简单强力事件，需平衡数值 |
| 净化祭坛 | `SharedEvents/Purifier.cs` | 删牌事件，和本项目商店/烟斗逻辑相关 |

---

## 6. P2：系统级内容，暂不一次合并

### 6.1 幕与遭遇池

| 幕 | Boss | 精英 | 普通遭遇 |
| -- | ---- | ---- | -------- |
| Exordium | 守护者、六火亡魂、史莱姆老大 | 地精大块头、乐加维林、三哨卫 | 邪教徒、大颚虫、虱虫、史莱姆、地精群、抢劫者等 |
| The City | 铜制机械人偶、第一勇士、收藏家 | 奴隶贩子、地精首领、扎人的书 | 异鸟、被拣选者、百夫长和神秘术士、蛇花、异蛇等 |
| The Beyond | 觉醒者、甜圈八体、时间吞噬者 | 大脑袋、天罚、拜蛇术士 | 圆球行者、巨口、倏忽魔、扭曲团块、黑球群、形状组合等 |

**暂缓原因：**

- 需要地图、Boss 图标、背景、怪物站位、VFX、音频和本地化一起维护。
- 会把本项目从平衡 MOD 推向完整内容 MOD。
- 更适合作为后续单独里程碑，而不是混在卡牌/遗物平衡调整里。

### 6.2 复杂事件与小游戏

| 类型 | 内容 |
| ---- | ---- |
| 小游戏 | 配对、转盘、传送门地图构建 |
| 复杂事件链 | 竞技场、心灵绽放、面具强盗、神秘球体、冒险者尸体 |
| 事件重平衡模式 | `RebalancedMode` 删除离开按钮、强化事件收益 |

**暂缓原因：**

- UI 与房间事件流程改动大。
- ActsFromThePast README 明确说明仍是 WIP，且多人模式不稳定。
- 当前项目更适合先抽取“可独立测试”的卡牌、遗物、Boss。

---

## 7. 后续执行建议

1. 先实现本批 `STS1-BOSS-01` 到 `STS1-EVENT-07`，只处理必要依赖。
2. 本批事件依赖的 `J.A.X.`、`死灵诅咒`、三本书遗物、绽放印记随事件同步补齐。
3. 双拳机器人事件、二层/三层一代回归区域挂载、二代原生 Boss 池调整，等 STS2 反编译源码到位后再做。
4. 幕、普通怪、完整遭遇池另开里程碑，不混入本批。

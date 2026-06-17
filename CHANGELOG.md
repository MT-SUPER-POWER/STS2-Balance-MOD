# Changelog

本文件记录 [Sts2BalanceMod](https://github.com/MT-SUPER-POWER/STS2-Balance-MOD) 的版本变更。

每个版本以 `# vx.x.x` 为标题。推送 Tag 后 GitHub Actions 自动把该段写入 Release 说明；zip 附件由本机 `Hooks/release.ps1` 上传。

完整需求清单与待办项见 [docs/balance-changes.md](docs/balance-changes.md)。

# v0.0.7

**新增**
- 怪物：红面具三人帮 — Pointy（尖头）、Romeo（罗密欧）、Bear（熊）模型与战斗 AI。
- 遭遇：RedMaskBandits 遭遇战，三个强盗同时登场。
- 事件：MaskedBandits 事件，第 2 幕第 23 层后触发，可选择交金或战斗获取红面具。
- 抽象基类：新增 `Sts2MonsterModel`（怪物基类）和 `Sts2EncounterModel`（遭遇基类），统一 MOD 怪物/遭遇代码模式。

**变更**
- Boss：暂注释 Collector/TimeEater Boss 代码（`#if false`），资源与本地化保留，后续版本恢复开发。
- 重构：Collector、TorchHead、TimeEater 改为继承 `Sts2MonsterModel`；CollectorBoss、TimeEaterBoss 改为继承 `Sts2EncounterModel`（代码已注释）。
- 分支：当前 Boss 开发进度保存至 `feature/act3-bosses` 分支。

**修复**
- 红面具三人帮（Bear/Pointy/Romeo）资源引用路径从旧 `res://Assets/ActsFromThePast/` 迁移至 `res://Sts2BalanceMod/`，确保三怪在怪物系统中正确加载。
- MaskedBandits 事件注册到 `ActModel.GenerateRooms` 事件池，第 2 幕问号格可见。
- RedMaskBandits 遭遇注入 `Hive.GenerateAllEncounters`，三怪在 Compendium 图鉴可见。

**本地化**
- 新增 eng/zhs/ita 三语言红面具强盗团本地化（名字、技能、事件文案）。

# v0.0.6
**新增**
- 卡牌：新增 J.A.X. 与死灵诅咒，作为一代事件回归依赖。
- 遗物：新增死灵之书、尼利的宝典、英雄宝典、绽放印记与突变之力。
- 事件：新增诅咒书本、增益研究者、神圣泉水、牧师、大转盘、红面具大人之墓，并简单注入现有事件池。
- Boss：移植时间吞噬者，接入时间扭曲能力、Boss 候选池与 ActsFromThePast 怪物资源。
- Boss：移植收藏家，接入收藏家本体、Torch Head 召唤物、Boss 候选池与 ActsFromThePast 怪物资源。
- 资源：补充 J.A.X.、死灵诅咒及本批一代事件遗物的卡图/遗物贴图。
- 资源：完整拆出 ActsFromThePast `0.12.0` release PCK 资源到 `Assets/ActsFromThePast/`，并批量还原实际 PNG/OGG 素材，供后续移植复用。
- 资源：从 ActsFromThePast `0.12.0` release PCK 解出并转存本批一代事件 portrait 贴图，接入 `Sts2BalanceMod/images/events/`。
- 资源：为本批一代事件补充 portrait 路径补丁，使二代事件布局读取 `Sts2BalanceMod/images/events/` 下的 Mod 资源。

**调整**
- 骨妹挽歌：改为升级后不消耗，但不再提高召唤次数（CARD-01）
- 红面具从一般共享遗物池移除，改为通过红面具相关事件获得（RELIC-08）。
- 红面具大人之墓：移除支付敬意选项上的原生红面具 HoverTip，避免原生遗物描述解析能量图标池时中断事件初始化。
- 心灵绽放先接入非战斗分支；打一层 Boss 分支等待 Boss/遭遇资源链移植。
- 时间吞噬者 Head Slam 暂不套二代 `NoDrawPower`，避免把 STS1 的少抽牌误还原成完全禁抽；后续补自定义减抽能力后再精确接入。

**文档**
- 新增 STS1 内容回归盘点清单，基于 ActsFromThePast 整理一代候选池与合并优先级
- 更新平衡调整清单，加入一代内容回归合并路线图
- 明确第一批一代回归范围：时间吞噬者、收藏家、诅咒书本、红面具、J.A.X.、神圣泉水、牧师、心灵绽放与大转盘事件
- 补充红面具规划：从一般遗物获取池移除，并通过红面具帮战斗与红面具大人之墓事件获得


# v0.0.5

**新增**
- 遗物 · 宁静烟斗（PeacePipe）：在火堆新增烟斗选项，可移除一张牌（RELIC-05）。
- 遗物 · 诅咒钥匙（CurseKey）：每回合 +1 费用，每次获取奖励时获得一张随机诅咒牌（RELIC-07）
- 遗物 · 咖啡杯（CoffeeCup）：无法在火堆休息，每回合 +1 费用（RELIC-07）
- 遗物 · 融合之锤（FusionHammer）：无法锻造，每回合 +1 费用（RELIC-07）
- 遗物 · 微笑面具（SmilingMask）：删牌价格固定 50 金币（RELIC-06）

**变更**
- 骨妹挽歌：升级后改为不消耗，且不再提高召唤次数（CARD-01）

# v0.0.4

**新增**
- 遗物 · 御守（Omamori）：抵消接下来获得的 2 张诅咒牌，带计数器显示（EVENT-04）
- 遗物 · 枯木树枝（DeadBranch）：消耗牌时随机加入一张手牌（RELIC-03）
- 事件：旧日垃圾堆奖励池加入御守
- 补丁：坚固钳子保留护甲 10 → 20（RELIC-04）
- 本地化：新增意大利语（`ita`）翻译

**修复**
- 死亡收割：修复仅能攻击单体的问题，现对所有敌人生效（BUG-02）
- 枯木树枝：虚无牌消耗后，树枝生成的牌保留在手牌，不再误入弃牌堆（BUG-03）

**文档**
- 修正 README 中从其他项目拷贝的错误链接与项目名
- 新增 CHANGELOG 与 GitHub Actions 自动发布流程
- 添加 WatcherMod 参考 Mod 为 git submodule

# v0.0.3

**新增**
- 遗物 · 日晷（Sundial）：每将抽牌堆洗牌 3 次，获得 2 点能量（RELIC-01）
- 遗物 · 橙色药丸（OrangePill）：同一回合内打出攻击 / 技能 / 能力牌各一张后，移除所有负面效果（RELIC-02）
- 补丁：Boss 沙漏凋零卡改为可打出并消耗，保留成长机制（MON-01）
- 补丁：暂时从猎人卡池移除袖里乾坤，避免与刀舞定位冲突（CARD-08）

**变更**
- 刀舞：删除消耗词条，稀有度提升为蓝卡（CARD-02）
- 声明最低游戏版本 `0.107.0`，锁定 BaseLib `3.2.0+`
- 引入 `Sts2RelicModel` / `Sts2PowerModel` 抽象基类，重构遗物与能力注册方式

**修复**
- 橙色药丸：修复回合外未正确清理状态的问题
- 电动力学：修复特斯拉电圈无法触发群伤的问题
- 修复遗物图标文件命名不规范导致缺少图框的问题
- 联机黑屏：修复 `ModelId entry ID out of range` 报错（BUG-01）

**文档**
- 添加 README.md（安装指南、调整摘要、项目结构）

# v0.0.2

**新增**
- 卡牌回归 · 死亡收割（战士，LEGACY-01）
- 卡牌回归 · 硬撑（战士，LEGACY-02）
- 卡牌回归 · 全神贯注（猎人，LEGACY-03）
- 卡牌回归 · 电动力学（机器人，替换吞噬暗影，LEGACY-04）
- 补丁：V6+ 高阶删牌价格调整为 75 基础 + 25/次递增（SHOP-01）

**变更**
- 骨妹挽歌：改为不消耗（CARD-01）
- 杂技：蓝卡降为白卡（CARD-03）
- 认知偏差：五回合内每回合扣 1 点聚焦，之后停止扣除（CARD-04）
- 多重释放：升级后增加保留词条（CARD-05）
- 幽魂形态：清除减敏捷负面效果（CARD-07）
- 放松：格挡 15/17 → 18/20（CARD-06）
- 卡池注入方式由 Harmony Patch 改为 `ModHelper.AddModelToPool`

**修复**
- 声明不支持旧版游戏，避免版本不匹配时内部报错

**文档**
- 添加 `docs/sts2-modding-guide.md` 与 `docs/balance-changes.md`
- 添加 `AGENTS.md` 协作规范与知识库

# v0.0.1

**新增**
- 初始化 Godot 4 + C# + BaseLib + Harmony 项目框架
- Mod 入口 `MainFile.cs` 与资源路径工具 `StringExtensions`
- 图片批处理脚本 `image_gen/`（卡牌 / 遗物 / 能力切图）
- GitHub Actions 发布工作流

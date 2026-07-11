# Changelog

本文件记录 [Sts2BalanceMod](https://github.com/MT-SUPER-POWER/STS2-Balance-MOD) 的版本变更。

每个版本以 `# vx.x.x` 为标题。推送 Tag 后 GitHub Actions 自动把该段写入 Release 说明；zip 附件由本机 `Hooks/release.ps1` 上传。

已完成的所有改动见 [README.md](../README.md#调整内容)；未完成的待办项见 [docs/balance-changes.md](docs/balance-changes.md)。

# v0.0.8.1-beta

### Added

- 遗物：新增 Hunter（Silent）专属罕见遗物「袖箭」（WristBlade）及其配套的中、英、意三语本地化与图标资源
- 遗物：新增 Hunter（Silent）专属普通遗物「悬浮风筝」（HoveringKite）及其配套的中、英、意三语本地化与图标资源
- 卡牌：新增 Necrobinder（骨妹）普通卡牌「猛撞」（Ram）及其配套的中、英、意三语本地化（CARD-11）
- 卡牌：新增 Necrobinder（骨妹）卡牌「比试」（Sparring）及其配套的中、英、意三语本地化与卡牌立绘（CARD-10）
- 基础设施：添加了 CodeGraph 相关的配置（如 Cursor/Gemini MCP、opencode、CLAUDE.md 等），方便在 Agent 中对项目源码进行快速索引和跳转
- 基础设施：为 `image_gen` 中的所有图片处理脚本（[cards.py](file:///d:/Github/STS2-Balance-MOD/image_gen/cards.py), [relics.py](file:///d:/Github/STS2-Balance-MOD/image_gen/relics.py), [powers.py](file:///d:/Github/STS2-Balance-MOD/image_gen/powers.py), [events.py](file:///d:/Github/STS2-Balance-MOD/image_gen/events.py), [enchantments.py](file:///d:/Github/STS2-Balance-MOD/image_gen/enchantments.py), [rest_site_options.py](file:///d:/Github/STS2-Balance-MOD/image_gen/rest_site_options.py)）添加了命名规范化功能（驼峰/大写转下划线蛇形小写 `to_snake_case`），使输出的资源文件名自动匹配游戏内 `RemovePrefix().ToLowerInvariant()` 路径规则（例如 `DeathReap.png` 自动转换为 `death_reap.png`）


### Changed

- 卡牌：能量汲取（Drain Power / DRAIN_POWER）伤害从 10/12 调整为 6/8，升级后由“随机升级 3 张”改为“升级弃牌堆的所有牌”（CARD-01）
- 卡牌：吸引仇恨（Pull Aggro / PULL_AGGRO）升级后属性调整为：召唤生命 6，格挡 9（CARD-02）
- 卡牌：回调了「挽歌」（Dirge）的消耗（Exhaust），但是增加了升级后的保留属性

### Fixed

- 修复/清理：移除了无用的 Time Eater 资源文件，并修复了 Romeo 怪物类中 `Mock` 异步方法缺少 `await` 的编译警告（CS1998）

# v0.0.8-beta

### Added

- 卡牌：新增猎人金卡「步步为营」（StepByStep），X 费用消耗，后续 X 回合每回合 +1 抽 +1 能量（CARD-09）
- 能力：新增 `StepByStepPower`，基于 `ClarityPower` + `EnergyNextTurnPower` 实现多回合持续效果
- 能力图标：新增 `step_by_step_power.png` 大小图

### Changed
- BOSS：沙漏回归
- CARD: 沙漏的凋零卡，改为可以用一费打出消耗
- CARD: 鸡煲的压缩回调

### Fixed
- BUG：修复全神贯注的弃牌数量可选的问题


# v0.0.7

### Added

- 事件：更新自定义事件池的背景图
- 事件：新增老乞丐事件，要求每名玩家至少 75 金币才会进入事件池；给金币后切换为牧师图并进入删牌阶段。
- 遗物：活雾（瓦库）改为删4，不再删3。
- 事件：大转盘事件现在有自定义转盘 UI（移植自 ActsFromThePast 的 NWheelSpinScreen + WheelSpinMinigame）
- 怪物：时间吞噬者 Time Warp 触发时现在有时钟弹出视觉效果（移植自 ActsFromThePast 的 TimeWarpTurnEndEffect）
- 遗物：新增商店遗物「矮人铁砧」（DwarfAnvil），拾起时选择 3 张牌为它们附加「锻造」附魔；被附魔的牌费用永久 -1（最低 0 费），由 `ForgeEnchantment` 通过 `CardEnergyCost.UpgradeBy(-1)` 实现。

### Changed

- 音频：Sts2ModAudio 升级为 AFTP 风格（FadeIn/FadeOut/BossStinger/音量联动）
- 音频：移除旧 LocalAudioPatch 注入机制，改用 ModBgmPatch（Hook.BeforeCombatStart 触发 TimeEater BGM）
- 音频：TimeEaterBoss 移除 `CustomBgm`，由 ModBgmPatch 直接使用 Sts2ModAudio.FadeIn 播放
- 遗物：矮人铁砧效果从「Smith 锻造次数叠加公式」简化为「拾起时选 3 张牌附魔（费用 -1）」
- 事件：死灵书事件（CursedTome）和 J.A.X. 事件（Augmenter）**限制为仅 Act 2（Hive）出现**，避免第一幕过早遇到过于强大的遗物/卡牌。

### Fixed

- 事件：心灵绽放「我即战争」分支改为使用自定义 `MindBloomBossEncounter`（RoomType.Monster），避免 BOSS 战结束后误触发通关换幕。
- 事件：心灵绽放「我即战争」分支改为从本局第一层全量 Boss 池真随机选取，不再限制"已遭遇过的"。
- 事件：面具强盗事件改为限定在 Act 2（Hive）触发，修复事件不出现的 BUG-12
- 音频：修复带 `CustomBgm` 的 Boss 遭遇（TimeEater 等）战斗结束后 MOD 自定义 BGM 不停止的问题。
- 事件：修复心灵绽放 BOSS 战导致在第三幕会直接认为是和原版一样的 Boss 战，直接结束游戏的问题。
- 事件：面具帮熊Bear的Bear Hug改为施加1层脆弱，不再减少玩家敏捷。
- 事件：心灵绽放限制为仅在第三幕事件池出现，保留三层内宝箱前后分支阈值判断。
- Boss：多人模式下 TimeEater 血量按玩家数放大（456/480 × n），TimeWarp 计数改为 `12 + 3 × (n - 1)` 并在每次触发后按同一公式回填。
- 本地化：锻造附魔描述中 `{Energy:energyIcons()}` 改为 `{Amount:energyIcons()}`，匹配 `EnergyIconsFormatter` 期望的变量名（参考 Sown）。
- 附魔图标：`ForgeEnchantmentIconPatch` 通过 Harmony 重定向 `EnchantmentModel.get_IconPath`，把图标路径指向 `res://Sts2BalanceMod/images/enchantments/forge_enchantment.png`，避免回退到 `missing_enchantment.png`。

### Docs

- README：重写「调整内容」章节，按商店 / 卡牌 / 回归卡牌 / 怪物 Boss / 事件 / 遗物 / 附魔分类，并标注每条改动的原版效果对照，方便玩家快速了解 Mod 改动。
- `docs/balance-changes.md`：精简为只保留未完成的待办项（CARD-08 / CARD-09）与未解决的 BUG / FEATURE；所有已完成项移到 README。


# v0.0.6

**修复**
- 联机：升级最低 BaseLib 依赖到 `3.2.1`，避免旧版自定义消息注册表在反序列化联机数据包时抛出 `KeyNotFoundException`。
- 编译：修复 `ModeShiftPower.cs` 中 `decimal→int` 类型转换错误。
- 编译：修复 `Guardian.cs` 中 `ConditionalBranchState` 构造参数错误，创建 `SelectorBranchState` 自定义分支类替代。
- 编译：修复 `ModeShiftPower` 和 `SharpHidePower` 缺少本地化条目导致的 STS001 错误。
- 警告：修复 `MindBloom.cs` 中 `Rng` 空引用警告（CS8602）。
- Boss：将 TimeEater 本地音乐资源目录从 `bgm` 统一为 `music`，并让本地 OGG 音乐播完后自动循环。
- Boss：修正抽牌减少说明，明确层数代表持续回合数，效果固定为每回合少抽 1 张牌。
- Boss：为 TimeEater 的本地 OGG 音乐与 TimeWarp 音效增加 Godot 音频播放桥，避免把 `res://` 资源传给 FMOD 后报找不到音乐/音效路径。
- Boss：Head Slam 叠加 TimeWarp 凝视特效与原本命中打击特效；TimeWarp 强制结束回合取消额外屏幕特效，改为延长音效提示。
- Boss：修正 TimeEater 的 BGM 与 TimeWarp 音效资源路径，避免进入第三幕或触发时间扭曲时报缺失资源。
- Boss：TimeWarp 计数归零时直接回填至 12，不再被系统移除；触发强制结束玩家回合时延长播放时间扭曲音效提示。
- Boss：按设计表修正 TimeEater 招式，Head Slam 改为 2 层抽牌减少，A19+ 才塞入 2 张黏液；Ripple 不再施加脆弱；Haste 仅在 A19+ 额外获得格挡。

**补充修复**
- 红面具事件：Bear/Pointy/Romeo 改用 `res://Assets/ActsFromPast/ActsFromThePast/monsters/` 下的新资源路径，并为 MaskedBandits 事件接入可预加载 portrait，避免迁移目录后事件或战斗视觉资源为空。
- 构建：禁用只支持简单资源的 quick PCK，改由 Godot export 产出完整 PCK；Debug 不再把 `Assets`、`images`、`localization` 作为裸目录复制到 mods 目录。

**新增**
- 怪物：红面具三人帮 — Pointy（尖头）、Romeo（罗密欧）、Bear（熊）模型与战斗 AI。
- 遭遇：RedMaskBandits 遭遇战，三个强盗同时登场。
- 事件：MaskedBandits 事件，第 2 幕第 23 层后触发，可选择交金或战斗获取红面具。
- 卡牌：新增 J.A.X. 与死灵诅咒，作为一代事件回归依赖。
- 遗物：新增死灵之书、尼利的宝典、英雄宝典、绽放印记与突变之力。
- 事件：新增诅咒书本、增益研究者、神圣泉水、牧师、大转盘、红面具大人之墓，并简单注入现有事件池。
- Boss：移植时间吞噬者，接入时间扭曲能力、Boss 候选池与 ActsFromThePast 怪物资源。
- Boss：移植收藏家，接入收藏家本体、Torch Head 召唤物、Boss 候选池与 ActsFromThePast 怪物资源。
- 资源：补充 J.A.X.、死灵诅咒及本批一代事件遗物的卡图/遗物贴图。
- 抽象基类：新增 `Sts2MonsterModel`（怪物基类）和 `Sts2EncounterModel`（遭遇基类），统一 MOD 怪物/遭遇代码模式。

**变更**
- Boss：时间吞噬者改为替换三层 `AeonglassBoss`（永世沙漏）候选，不再追加到三层 Boss 池，保持三层 Boss 数量不变。
- Boss：Collector Boss 代码仍暂注释（`#if false`），资源与本地化保留，后续版本恢复开发。
- 怪物：移除六火亡魂（Hexaghost）及其心灵绽放遭遇，资源不再需要。
- 重构：Collector、TorchHead、TimeEater 改为继承 `Sts2MonsterModel`；CollectorBoss、TimeEaterBoss 改为继承 `Sts2EncounterModel`。

**修复**
- Boss：补充 `res://images/ui/run_history/time_eater_boss*.png` 兼容图标，修复进入第三幕时顶部 Boss 图标预加载失败导致的崩溃。
- Boss：恢复 TimeEater/TimeEaterBoss 编译，补齐半血 Haste 转阶段台词气泡、`TalkPos` Marker2D、Head Slam 减抽牌能力与三语本地化。
- ActsFromThePast 资源：修正 `Assets/ActsFromPast/ActsFromThePast/` 下场景、Spine 数据与导入元数据的 `res://ActsFromThePast/` 旧路径，改为当前项目实际路径，避免 monster `.tscn` 打开时视觉资源为空。
- 怪物图鉴 Error：红面具三人帮（Bear/Pointy/Romeo）spine 资源路径迁移至 `res://Assets/ActsFromThePast/ActsFromThePast/`；所有 61 个怪物 .tscn 恢复 `NCreatureVisuals.cs` 脚本引用（Godot 4 C# 脚本运行时由程序集解析，该引用是怪物视觉系统必需组件）。
- 图鉴名称显示 `monsters.XXX.name`：`Sts2MonsterModel` 覆盖 `Title` 属性显式拼接 `STS2BALANCEMOD-` 前缀；新增 `MonsterLocalizationInjectionPatch` 运行时注入 MOD 的 `monsters.json`。
- MaskedBandits 事件注册到 `ActModel.GenerateRooms` 事件池，第 2 幕问号格可见。
- RedMaskBandits 遭遇注入 `Hive.GenerateAllEncounters`，三怪在 Compendium 图鉴可见。

**本地化**
- 新增 eng/zhs/ita 三语言红面具强盗团本地化（名字、技能、事件文案）。
- 新增 eng/zhs/ita 三语言红面具强盗团本地化对话气泡文案

**调整**
- 骨妹挽歌：改为升级后不消耗，但不再提高召唤次数（CARD-01）
- 红面具从一般共享遗物池移除，改为通过红面具相关事件获得（RELIC-08）。
- 红面具大人之墓：移除支付敬意选项上的原生红面具 HoverTip，避免原生遗物描述解析能量图标池时中断事件初始化。
- 心灵绽放先接入非战斗分支；打一层 Boss 分支等待 Boss/遭遇资源链移植。

**文档**
- 新增 STS1 内容回归盘点清单，基于 ActsFromThePast 整理一代候选池与合并优先级
- 更新平衡调整清单，加入一代内容回归合并路线图


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

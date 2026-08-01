# Changelog

本文件记录 [Sts2BalanceMod](https://github.com/MT-SUPER-POWER/STS2-Balance-MOD) 的版本变更。

每个版本以 `## vX.X.X` 为标题。推送 Tag 后 GitHub Actions 会自动在云端构建、打包，并将打包好的 zip 附件发布到对应的 Release 页面中，无须再本地手动上传。

已完成的所有改动见 [README.md](README.md##调整内容)；未完成的待办项见 [docs/balance-changes.md](docs/balance-changes.md)。


## v0.1.6

### Changed

- 卡牌：修复「计划妥当」（Well-Laid Plans）升级时不降低耗能的问题，升级后正常从 2 费降至 1 费（CARD-11）。
- 卡牌：支持「计划妥当」（Well-Laid Plans）能力层数可叠加，重复打出时保留卡牌张数正常累加（CARD-12）。


## v0.1.5

### Fixed

- baselib 底层 bug 修复 ModelID Patch 不正确

### Added

- 俄文本地化

### Changed

- 卡牌：回调「火箭飞拳」（Rocket Punch）为降至 0 费效果，生成状态牌时直接降为 0 费（CARD-09）。
- 卡牌：调整「华丽收场」（Grand Finale）升级效果为抽牌堆卡牌数 ≤ X + 2 时允许打出，且升级不再增加伤害（CARD-10）。
- 卡牌：调整「巨镰」（The Scythe）初始伤害为 16 点（CARD-08）。
- 卡牌：调整「创世之柱」（Pillar of Creation）格挡数值为 3 点（升级后 4 点）（CARD-07）。
- 遗物：重构「烘焙手套」（Toasty Mittens），回合开始可从手牌选择 1 张卡牌消耗（提供 Skip 选项），成功消耗后获得 1 点力量（RELIC-02）。
- 遗物：回调「图章戒指」（Signet Ring）获得金币为 999（RELIC-03）。

### Removed

- 卡牌：撤销「放松」（Relax）格挡增强 Patch，保持官方原版属性（CARD-06）。

## v0.1.4


### FIXED

- 卡牌：修复「计划妥当」（Well-Laid Plans）回合结束选卡保留界面，自动过滤掉手牌中已经具有【保留】属性的卡牌（CARD-04）。

## v0.1.3

### Added

- 卡牌：新增静默猎手（Silent）罕见攻击卡牌「内脏切除」（Eviscerate），耗能 3（本回合每丢弃 1 张牌耗能 -1），造成 7（升级 9）点伤害 3 次（CARD-01）。
- 事件：为「药水的未来？」（The Future of Potions）事件初始选项中新增「离开」分支，允许玩家保留药水直接离开（EVENT-01）。

### Refactored

- CI/CD：重构 Release GitHub Actions 工作流，采用官方 `actions/github-script@v7` 原生提取 `CHANGELOG.md` 版本日志，支持 `## vX.X.X` / `## [X.X.X]` 格式并移除对三方 Action 及 PowerShell 脚本的依赖。

### Changed

- 卡牌：从静默猎手卡池中移除原版卡牌「精密」（Pinpoint），由「内脏切除」（Eviscerate）替代（CARD-01）。

- 怪物：红面具强盗 Bear（熊）首回合【熊抱 BEAR_HUG】的 Debuff 从施加 1 层【易伤】调整为减少 2 点【敏捷】（MONSTER-01）。



## v0.1.2

### Refactored

- BOSS：重构感染棱柱 [InfestedPrism]，移除开场【活力火花】技能牌污染，改为受击穿透格挡叠加可计数的【感染】[InfectedPower] 机制，并重构 4 回合固定行动循环（轻击 / 重击 / 连击 / 强化）（BOSS-01）。

### Fixed

- BOSS：修复感染棱柱的单段攻击在部分被格挡时重复叠加【感染】的问题；同一攻击段对每名玩家最多施加 2 层，连击仍按每段独立结算（BOSS-01）。

### Changed

- 卡牌：巨镰 [The Scythe] 初始伤害由 13 提高至 20，维持每次打出后 4 / 5 的伤害成长（CARD-05）。
- 遗物：灵魂契约 [Soul Contract] 改为商店遗物，并移除拾取时扣除 10% 最大生命的代价（RELIC-03）。



## v0.1.1

### Changed

- 卡牌：回调魔球 [The Ball] 的每次打出成长幅度至 15 / 20（CARD-01）。
- 卡牌：回调触媒 [Accelerant] 至稀有（CARD-02）。
- 卡牌：恢复计划妥当 [Well-Laid Plans] 为 1 费、保留最多 1 / 2 张牌的效果；未选择的手牌会正常弃置，计划妥当仍支持多人游戏（CARD-03）。
- 卡牌：调整华丽收场 [Grand Finale] 为 X 费卡牌，打出条件调整为抽牌堆的牌数小于或等于 X（即当前能量）（CARD-04）。
- 遗物：历史课 [History Course] 恢复重复攻击牌与技能牌；诺努佩佩的钻石王冠 [Nonupeipe's Diamond Diadem] 恢复少牌回合受到敌人伤害减半（RELIC-01、RELIC-02）。

### Added

- UI/UX：添加「猛撞」（Ram）卡牌立绘（卡图）
- UI/UX：完善「进化」（Evolve）卡牌立绘（卡图）

### Changed

- 卡牌：探寻（Dowsing）任务要求从进入 5 个 ? 房间调整为进入 4 个 ? 房间（CARD-01）

### Docs

- 文档：将 [README.md](README.md) 中所有表格的卡牌、遗物、怪物和事件名称统一更新为 `中文 [英文]` 格式，便于代码查阅与检索

### Devops

- AI：添加了一套 AI 开发工具（skills）



## v0.1.0

### Added

- 卡牌：新增战士罕见卡牌「进化」（Evolve）与对应能力「进化」（EvolvePower）（CARD-04）
  - 效果：每当你抽到一张状态牌，抽 1（未升级）/ 2（升级）张牌。
  - 资源：生成并配置了「进化」卡牌插图 `evolve.png` 及能力图标 `evolve_power.png`。
- 本地化：补充了「进化」卡牌及能力的中文、英文、意大利文三语本地化翻译与说明。
- UI：采用中和作为 mod 的封面图
- 遗物：为新增遗物「灵魂契约」（SoulContract）添加限制补丁，使其仅出现在先古之民「瓦库」（Vakuu）的专属遗物池中。此外，拦截 Vakuu 的 `Pool1` 选项，固定让该遗物出现在第一选项，且当且仅当玩家卡组中拥有包含消耗（Exhaust）属性的卡牌时才出现该选项。

### Changed

- 卡牌：探寻（Dowsing）任务要求从进入 5 个 ? 房间调整为进入 4 个 ? 房间（CARD-01）
- 遗物：诅咒钥匙（CurseKey）跳过宝箱功能重构 + 诅咒时机调整（RELIC-01）
  - 移除独立跳过按钮，复用 ProceedButton，开箱前显示”跳过宝箱”+ 图标，点击后直接离开；开箱后恢复原生行为。
  - 诅咒改为选完遗物后生成（`UpdateText(ProceedLoc)` 时），而非开箱瞬间。
  - 选遗物时文字残留 Skip 修复、地图返回黑屏修复。`SkipLoc` 改为 `gameplay_ui.json`。
- 卡牌：战士卡牌「放血」（Bloodletting）稀有度由蓝卡（Uncommon）改回白卡（Common）（CARD-01）。
- 卡牌：颜色无特定「创世之柱」（Pillar of Creation）平衡调整（CARD-02）
  - 数值：护甲从原版的 5/8 调整为 3/4（通过 `BaseBlock = 3`，`UpgradeBlock = 1` 实现）。
  - 效果：重构其 Power 逻辑，由”每回合首次生成卡牌时触发”修改为”每当你生成一张卡牌均获得格挡”。
  - 描述：在 localization overrides 中重写了其三语描述以符合全新行为。
- 卡池：将「残酷」（Cruelty）从战士卡池中过滤移除，实现用「进化」无缝替代「残酷」（CARD-03）。

### Chore

- 新增 `.editorconfig`：统一 C# 12 代码风格规则（缩进、换行、命名等），与 `dotnet format` 配合
- 新增 `.github/workflows/lint.yml`：PR 自动触发 `dotnet format --verify-no-changes` 格式检查
- 新增 `.github/workflows/ai-review.yml`：PR 自动调用 Gemini Flash API 进行 AI 代码审查
- 优化构建与打包流程：消除 36 个重复 UID 警告、Unrecognized UID 警告、`.NET: Failed to load project assembly` 错误警告、Godot 导出崩溃 MSB3073 干扰警告，实现编译/打包 0 错误、0 警告输出

### Fixed

- 遗物：修复坚固夹子（SturdyClamp）因为 API 更新接口导致的错误


# v0.0.8.2-beta

### Added

- 事件：为「除虫者」（Bugslayer）事件初始选项新增「离开」分支，玩家可以安全离开而不用被迫选择卡牌。
- 事件：为「科学怪人」（TinkerTime，对应 `MadScience.cs`）事件初始选项新增「离开」分支，玩家可以选择直接走开而无须被迫当实验对象。
- 基础设施：在 `.csproj` 中增加了对 `BSchneppe.Sts2.ReferenceAssemblies` 和 `Lib.Harmony` 的条件引用，解决了在没有游戏文件的 CI 构建机上的编译依赖。
- 基础设施：重构了 GitHub Actions 的 `release.yml` 流程。目前当推送 tag（如 `v0.0.8.2-beta`）时，云端自动下载并配置 Godot 4.5.1 Mono 命令行工具及导出模板，全自动完成打包与 zip 上传，实现全托管的 CI/CD 发布。

### Fixed

- 遗物：修复「枯木树枝」（DeadBranch）对回合结束时消耗的虚无牌生成新牌时，因重复调用生成函数导致新加入手牌没有正确获得保留（Retain）效果的 BUG。

# v0.0.8.1-beta

### Added

- 遗物：新增 Hunter（Silent）专属罕见遗物「袖箭」（WristBlade）及其配套的中、英、意三语本地化与图标资源
- 遗物：新增 Hunter（Silent）专属普通遗物「悬浮风筝」（HoveringKite）及其配套的中、英、意三语本地化与图标资源
- 卡牌：新增 Necrobinder（骨妹）普通卡牌「猛撞」（Ram）及其配套的中、英、意三语本地化（CARD-11）
- 卡牌：新增 Necrobinder（骨妹）卡牌「比试」（Sparring）及其配套的中、英、意三语本地化与卡牌立绘（CARD-10）
- 基础设施：添加了 CodeGraph 相关的配置（如 Cursor/Gemini MCP、opencode、CLAUDE.md 等），方便在 Agent 中对项目源码进行快速索引和跳转
- 基础设施：为 `image_gen` 中的所有图片处理脚本添加了命名规范化功能（驼峰/大写转下划线蛇形小写 `to_snake_case`），使输出的资源文件名自动匹配游戏内 `RemovePrefix().ToLowerInvariant()` 路径规则（例如 `DeathReap.png` 自动转换为 `death_reap.png`）


### Changed

- 卡牌：能量汲取（Drain Power / DRAIN_POWER）伤害从 10/12 调整为 6/8，升级后由“随机升级 3 张”改为“升级弃牌堆的所有牌”（CARD-01）
- 卡牌：吸引仇恨（Pull Aggro / PULL_AGGRO）升级后属性调整为：召唤生命 6，格挡 9（CARD-02）
- 卡牌：回调了「挽歌」（Dirge）的消耗（Exhaust），但是增加了升级后的保留属性

### Fixed

- 修复/清理：移除了无用的 Time Eater 资源文件，并修复了 Romeo 怪物类中 `Mock` 异步方法缺少 `await` 的编译警告（CS1998）
- 修复：解决悬浮风筝（HoveringKite.cs）中因未对 Owner 或 Owner.Creature 进行空值校验而导致的潜在空引用解引用编译警告（CS8602）
- 修复：解决心灵绽放（MindBloom）事件点击 Boss 战斗分支时，由于传入了已 calls `.ToMutable()` 的 mutable 遭遇模型，导致 `EnterCombatWithoutExitingEvent` 内部重复调用 `ToMutable()` 触发 `MutableModelException` 崩溃的问题。改为直接传递 canonical 遭遇模型。

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

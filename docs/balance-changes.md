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

### 基础设施

- [x] **CONFIG-01** — Mod 设置页面 MVP
  - 注册 RitsuLib 设置页面并启用自动持久化。
  - 首个开关控制“除虫者”“科学怪人”和“药水的未来？”的离开选项，默认开启。
  - 补齐英语、简体中文、意大利语和俄语设置界面文本。

- [x] **CONFIG-02** — 感染棱柱重做开关
  - 默认开启，保持 BOSS-01 当前的固定四回合循环与【感染】机制。
  - 关闭时完整恢复原版【活力火花】开场能力与原版行动状态机。
  - 下次进入感染棱柱战斗时生效；多人游戏时所有玩家必须使用相同设置。

- [x] **MIGRATION-01** — RitsuLib 基础设施迁移
  - 移除 BaseLib 的包引用、运行时依赖和部署逻辑；内容改由 RitsuLib 自动注册。
  - 卡牌、遗物、能力、事件、怪物和遭遇统一采用 RitsuLib 模板与资源配置；路径集中于 `ModAssetPaths`。
  - 对齐参考项目的可维护结构：代码根目录统一为 `Sts2BalanceModCode/`，共享模板集中在 `Abstract/`，内容按类别并列，运行时辅助代码集中在 `Runtime/`。
  - 将本地化 key 迁移到 RitsuLib 的规范化 Model ID，并完成本地游戏启动到主菜单的日志验证。

- [x] **ENCHANTMENT-01** — RitsuLib 附魔模块重构
  - 目前：`ForgeEnchantment` 直接继承游戏 `EnchantmentModel`，图标与本地化分别由专用 Harmony Patch 兜底；`DwarfAnvil` 直接处理附魔模型克隆、应用和卡牌预览。
  - 目标：保留铁砧“选择 3 张牌、每张费用永久 -1（最低 0）”的玩家可见行为，改用教程中的 `[RegisterEnchantment]` 与 `ModEnchantmentTemplate`。
  - 结构：新增 `BalanceEnchantmentTemplate` 作为深模块，集中附魔图标路径、本地化与公共默认值；新增 `EnchantmentExtensions`，将“获取可变附魔、应用到卡牌、刷新预览”的流程封装为单一扩展接口。具体附魔只声明筛选条件和效果，授予来源只调用扩展接口。
  - 清理：删除仅服务于 ForgeEnchantment 的图标与本地化注入 Patch；四种语言改为标准 `enchantments.json`，图标遵循统一资源命名约定。

### BOSS

- [x] **AFP-BOSS-02** — 六火亡魂（Hexaghost）及心灵绽放专用遭遇
  - 按 Acts From the Past v1.0.5 移植六颗火球状态、Divider/Sear/Inferno 行动、灼伤升级补丁、动画、音效与图集特效。
  - 新增不可进入普通地图池的 `RoomType.Monster` 专用遭遇，保持 AFP 原始数值与进阶分档。

- [x] **AFP-BOSS-03** — 史莱姆 Boss（Slime Boss）及心灵绽放专用遭遇
  - 移植史莱姆 Boss、尖刺/酸液大型与中型史莱姆、Split Power、两级分裂链、动画与音效。
  - 新增包含七个固定分裂槽位的 `RoomType.Monster` 专用遭遇，保持 AFP 原始数值与进阶分档。

- [x] **AFP-BOSS-PACK-01** — Boss 资源打包链
  - 删除 `Sts2BalanceMod/monsters/.gdignore`，让该目录的全部怪物资源进入 Godot 扫描；场景根节点由既有 `MonsterVisualsPatch` 在运行时包装为 `NCreatureVisuals`。
  - 将 LibGDX `vfx.atlas` 加入导出白名单；最终 PCK 已核对包含全部自定义怪物场景、Spine、六火亡魂贴图、VFX 与三组音效。

### 遗物


### 卡牌


### 事件

- [x] **MASKED-BANDITS-02** — 将红面具劫匪注册为事件战斗
  - 通过 Harmony Postfix 将 `RedMaskBandits` 幂等追加到 `ModelDb.EventEncounters`，使 Pointy、Romeo、Bear 归入怪物图鉴的「事件」分组。
  - 保持遭遇的 `RoomType.Monster`，不改变事件出现条件、交钱分支、进入战斗流程、25–35 金币与红面具奖励，也不加入任何幕的普通怪物池。
  - 编译验证通过；游戏更新后需复核 `NBestiary.AddEvents` 仍直接枚举 `ModelDb.EventEncounters`。


---

## 现有的问题以及无法解决的问题


### BUG 列表

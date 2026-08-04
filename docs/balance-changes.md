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
  - 注册 BaseLib 配置页面并启用自动持久化。
  - 首个开关控制“除虫者”“科学怪人”和“药水的未来？”的离开选项，默认开启。
  - 补齐英语、简体中文、意大利语和俄语设置界面文本。

- [x] **CONFIG-02** — 感染棱柱重做开关
  - 默认开启，保持 BOSS-01 当前的固定四回合循环与【感染】机制。
  - 关闭时完整恢复原版【活力火花】开场能力与原版行动状态机。
  - 下次进入感染棱柱战斗时生效；多人游戏时所有玩家必须使用相同设置。

### BOSS

- [x] **AFP-BOSS-02** — 六火亡魂（Hexaghost）及心灵绽放专用遭遇
  - 按 Acts From the Past v1.0.5 移植六颗火球状态、Divider/Sear/Inferno 行动、灼伤升级补丁、动画、音效与图集特效。
  - 新增不可进入普通地图池的 `RoomType.Monster` 专用遭遇，保持 AFP 原始数值与进阶分档。

- [x] **AFP-BOSS-03** — 史莱姆 Boss（Slime Boss）及心灵绽放专用遭遇
  - 移植史莱姆 Boss、尖刺/酸液大型与中型史莱姆、Split Power、两级分裂链、动画与音效。
  - 新增包含七个固定分裂槽位的 `RoomType.Monster` 专用遭遇，保持 AFP 原始数值与进阶分档。

- [x] **AFP-BOSS-PACK-01** — Boss 资源打包链
  - 删除 `Sts2BalanceMod/monsters/.gdignore`，让该目录的全部怪物资源进入 Godot 扫描；场景根节点由既有 `Sts2MonsterVisualsPatch` 在运行时包装为 `NCreatureVisuals`。
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

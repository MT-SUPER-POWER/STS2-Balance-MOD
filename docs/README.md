# 📚 Sts2BalanceMod 知识库总索引 (Knowledge Base Hub)

欢迎来到 **Sts2BalanceMod** 中央知识库！本文档为所有玩家、Mod 开发者及 AI 协作 Agent 提供本项目所有规则、设计文档、参考手册、技术分析报告与工具链的导航入口。

> [!important]
> **致协作 Agent 与开发者**：在处理任何卡牌/遗物/能力/事件/遭遇的修改、重构或设计时，请优先查阅本知识库中的相关文档。完成新增独立功能或文档编写后，需在本文档中同步更新索引。

---

## 目录

- [📖 玩家与内容手册](#-玩家与内容手册)
- [📋 需求与版本追踪](#-需求与版本追踪)
- [🛠️ 开发者与 Mod 制作指南](#️-开发者与-mod-制作指南)
- [🔍 系统设计与技术分析报告](#-系统设计与技术分析报告)
- [📁 资源与自动化工具脚本](#-资源与自动化工具脚本)
- [📐 架构约定与代码规范](#-架构约定与代码规范)

---

## 📖 玩家与内容手册

专为玩家与开发人员提供的详细游戏机制图文手册：

- ⚡ **[能力与效果手册 (powers.md)](powers.md)** — 包含 Mod 中所有玩家 Buff、战斗 Debuff、Boss 机制能力及原版能力调整（如幽魂形态负面移除、认知偏差修正等）的完整图文说明。
- 📜 **[事件与遭遇手册 (events.md)](events.md)** — 包含原版事件调整（删牌降价、离开分支）以及 1 代 10+ 经典回归事件（老乞丐、诅咒书本、面具强盗、心灵绽放、大转盘等）的选择分支与触发条件。

---

## 📋 需求与版本追踪

- 📝 **[更新日志 (CHANGELOG.md)](../CHANGELOG.md)** — 各版本已实现功能、修复与重构记录。
- 🎯 **[平衡调整需求清单 (balance-changes.md)](balance-changes.md)** — 所有需求项的编号、描述、状态、落地进度与资源补齐记录。
- 📦 **[STS1 内容回归盘点清单 (sts1-content-inventory.md)](sts1-content-inventory.md)** — 基于 ActsFromThePast 整理的一代内容候选池与合并优先级。

---

## 🛠️ 开发者与 Mod 制作指南

- 📖 **[STS2 Mod 制作指南 (sts2-modding-guide.md)](sts2-modding-guide.md)** — 从零开始制作 STS2 Mod 的完整开发教程。
- 💡 **[WatcherMod 参考代码 (references/WatcherMod/)](references/WatcherMod/)** — 社区优秀 Mod 参考（GitHub Submodule）。
- 💡 **[ActsFromThePast 参考代码 (references/ActsFromThePast/)](references/ActsFromThePast/)** — STS1 内容移植参考 Mod（GitHub Submodule）。

---

## 🔍 系统设计与技术分析报告

针对复杂机制与底层 Harmony 架构的深度技术分析：

- 🎭 **[红面具事件完整流程 — 分层技术分析报告 (reports/RedMaskFullSystemAnalysis.md)](reports/RedMaskFullSystemAnalysis.md)** — 从底层实体到本地化的六层架构剖析，含时序图与 API 速查。
- 🚪 **[宝箱房跳过功能技术报告 (treasure-room-skip-technical-report.md)](treasure-room-skip-technical-report.md)** — 宝箱房 ProceedButton 跳过按钮交互与诅咒钥匙避免机制的技术报告。

---

## 📁 资源与自动化工具脚本

- 📦 **[ActsFromThePast 拆包资源目录](../Assets/ActsFromThePast/)** — 已从 release PCK 完整拆包的一代 Boss、怪物、事件、贴图、音频与本地化资源。
- 🖼️ **[一代事件贴图](../Sts2BalanceMod/images/events/)** — 转存并按 Mod 事件 ID 命名的事件 Portrait 贴图。
- 🐍 **[图片生成自动化脚本 (image_gen/)](../image_gen/)** — Python CLI 切图工具（包含卡牌、遗物、能力、火堆选项、事件背景的裁切脚本）。
- 🔧 **[打包与发布脚本 (Hooks/)](../Hooks/)** — PowerShell 构建打包与 Release 说明提取脚本。

---

## 📐 架构约定与代码规范

1. **`Sts2BalanceModCode/Patches/`**：包含对游戏原版现有逻辑、卡牌、遗物、怪物和事件的 Harmony Patch 修改。
2. **`Sts2BalanceModCode/{Cards,Relics,Powers,Monsters,Events,Encounters}/`**：包含所有新增加的内容类与模型逻辑。
3. **资源放置**：游戏运行时使用的图片存放在 `Sts2BalanceMod/images/`，文档与 Markdown 预览使用的图标与贴图同步放置于 `Assets/`。

---
name: sts2-workshop-release
description: Steam Workshop publish and release workflow for STS2 Balance MOD. Triggers when the user requests to release a new version, publish/upload to Steam Workshop, or update the workshop item. Maintains workshop/WORKSHOP_DESCRIPTION.md (manual BBCode draft for Steam web page), verifies changelog/git diff, and always presents a full preview before user confirmation.
---

# STS2 Workshop Release Workflow

## Overview

A two-phase, approval-gated workflow for publishing and updating **STS2 Balance MOD** on Steam Workshop:

1. **Phase 1: Sync & Preview (核对文案、动态同步与用户确认)**：
   - 核对自上一版本以来的 Git 提交与 `CHANGELOG.md`，维护 `workshop/WORKSHOP_DESCRIPTION.md`（供作者手动粘贴至 Steam 工坊后台的主页 BBCode 文档草稿）。
   - 运行动态同步脚本生成 `workshop/tmp/preview.json` 与更新 `workshop/workshop.json`。
   - 向用户呈现完整的发布与文案预览，**停止并等待用户确认**。
2. **Phase 2: Build & Upload (打包上传与 Git 提交)**：
   - 用户明确确认后，检查游戏进程关闭状态，执行 `dotnet publish` Release 编译构建与文件打包。
   - 默认执行日常代码包秒传（`WorkshopPublisher -c`，不覆盖已在 Steam 网页排版好的主页文案）。
   - 提交并推送版本与工坊配置文件。

---

## 创意工坊文件与文案定位认知

- **`workshop/WORKSHOP_DESCRIPTION.md` 的定位**：
  - **不是**通过工坊上传直接打包到游戏里的内容，**也不是**每次发版都需要强行通过 API 覆盖工坊网页的字段。
  - 它是专为开发者本地维护的 **Steam BBCode 格式主页文案草稿**，供开发者在需要时手动复制并填充到 Steam 创意工坊网页后台进行精细排版。
  - **发版时的维护要求**：发版时必须对照 Git 提交与 `CHANGELOG.md` 检查是否有新卡牌、新遗物、新事件或机制重构。若有，必须在 `WORKSHOP_DESCRIPTION.md` 中补充/更新对应条目，并在对话中向用户明确展示变更以供确认；若仅为 Bug 修复/卡图更新且条目已在案，则明确说明“无需改动现有主页文案”。

- **创意工坊文件管理与 Git 约束**：
  - **受 Git 版本管理**：
    - `workshop/WORKSHOP_DESCRIPTION.md` — 创意工坊主页 Steam BBCode 说明文档
    - `workshop/workshop.json` — 创意工坊元数据配置文件（含 Title、Tag、Release 链接等）
    - `workshop/image.png` — 创意工坊封面图（< 1MB）
    - `workshop/mod_id.txt` — 创意工坊 Item ID（`3776706758`）
    - `workshop/.gdignore` — Godot 资源扫描忽略标识
  - **被 `.gitignore` 忽略（不提交）**：
    - `workshop/content/` — 打包编译产物（dll / pck / json 等发布文件，由 WorkshopPublisher 上传）
    - `workshop/tmp/` — 临时预览文件（`preview.json`）

---

## Phase 1: Sync & Preview (文案核对、同步与玩家确认)

### Step 1: Pre-publish Check & Description Review

1. **版本与日志检查**：
   - 检查 `Sts2BalanceMod.json` 版本号 (`"version": "vX.X.X"`)。
   - 确认 `CHANGELOG.md` 顶层包含当前版本的日志段落。

2. **Git 差异对比与 `workshop/WORKSHOP_DESCRIPTION.md` 维护**：
   - 使用 `git log <last-release-tag>..HEAD` 对比当前版本的改动内容。
   - **新增玩法机制**（新增卡牌、遗物、事件、怪物、机制重构或设置开关）：
     - 将对应的新条目按 BBCode 格式补充/总结至 `workshop/WORKSHOP_DESCRIPTION.md` 对应模块。
     - 记录下本次文档的具体改动差异（Diff）。
   - **纯修复/卡图更新**（仅 Bugfix、数值微调、卡面立绘替换）：
     - 核对现有条目表述是否依然准确。如果已涵盖，则无需盲目修改主页文案，记录结论为“主页文案与当前功能一致，无需改动”。

3. **运行进程预检**：
   - 检查后台是否有正在运行的 `SlayTheSpire2.exe`。若有，在后续预览中提示用户在确认前退出游戏，避免 Release 编译与文件拷贝因文件占用失败。

### Step 2: Run Dynamic Sync

在 `image_gen/` 目录下运行：

```bash
uv run sync-workshop
```

该命令会自动：

- 从 `workshop/WORKSHOP_DESCRIPTION.md` 读取完整的 Steam BBCode 主描述
- 将当前版本 GitHub Release 链接写入 `changeNote`
- 动态注入当前版本号至 `title`
- 配置分类标签 `["Balance"]` 与 RitsuLib 前置依赖 `3747602295`
- 写入分支限定 `minBranch: "public-beta"` 与 `maxBranch: "public-beta"`
- 导出预览结果至 `workshop/tmp/preview.json` 并更新 `workshop/workshop.json`

### Step 3: Present Preview & Wait for Confirmation (关键阻断节点)

在对话中向用户完整呈现以下预览信息：

```markdown
### 📋 创意工坊发布内容预览 (Pending User Confirmation)

- **`title` (Mod 标题)**: 动态嵌入当前版本号（如 `STS2 Balance MOD [v0.3.11] | 《杀戮尖塔2》平衡调整 Mod`）。
- **`branch` (分支兼容)**: 固定声明 `minBranch: "public-beta"` 与 `maxBranch: "public-beta"`，限定在 public-beta 测试分支运行。
- **`description` (主页 BBCode 文案)**:
  - [明确列出本次是否改动了 `workshop/WORKSHOP_DESCRIPTION.md`，若改动了则展示新增/修改的具体条目；若未改动则说明为何无需改动]
  - *(注：该文件供您手动复制到 Steam 工坊网页后台排版；本次上传默认不通过 API 覆盖工坊网页排版)*
- **`changeNote` (更新说明)**: `https://github.com/MT-SUPER-POWER/STS2-Balance-MOD/releases/tag/vX.X.X`
- **本次版本变更明细 (`vX.X.X`)**：
  - [摘录 CHANGELOG.md 最新改动条目]
- **`tags` (分类标签)**: 固定标签 `["Balance"]`。
- **`dependencies` (前置依赖)**: 自动绑定 RitsuLib 创意工坊 ID `3747602295`。

⚠️ **注意**：如果检测到 `SlayTheSpire2.exe` 正在运行，请在确认前先关闭游戏。

是否确认发布以上内容？确认后将开始 Release 构建与 Steam 创意工坊上传。
```

**⚠️ 绝对规则**：必须在此步骤**停止并等待用户明确确认（例如“确认”、“继续”等）**后，才能进入 Phase 2。

---

## Phase 2: Build & Upload (确认后打包与上传)

在收到用户明确确认后，依次执行：

### Step 4: Build Release Bundle

```powershell
dotnet publish -c Release
```

产物输出在 `dist/Sts2BalanceMod/` 目录。
若报错文件被占用，请提醒用户确认游戏是否已彻底关闭。

### Step 5: Package Content

```powershell
Copy-Item -Path "dist\Sts2BalanceMod\*" -Destination "workshop\content" -Force
```

确认 `workshop/image.png`（封面图 < 1MB）与 `workshop/mod_id.txt`（`3776706758`）存在。

### Step 6: Execute Steam Upload

```powershell
# 默认秒传模式（强烈推荐：仅上传代码资源包和更新 changeNote 版本链接，不破坏已排好的网页主页文案）
dotnet run --project tools/WorkshopPublisher -c Release -- -c

# 可选：完整双语发布（仅当用户明确要求通过 API 同步覆写工坊网页中英文文案时使用）
# dotnet run --project tools/WorkshopPublisher -c Release
```

### Step 7: Git Commit & Handoff

提交版本与工坊元数据变动记录：

```bash
git add .gitignore workshop/WORKSHOP_DESCRIPTION.md workshop/workshop.json
git commit -m "chore(release): 发布 STS2 Balance MOD vX.X.X 到 Steam 创意工坊"
```

向用户汇报上传成功状态与创意工坊链接。

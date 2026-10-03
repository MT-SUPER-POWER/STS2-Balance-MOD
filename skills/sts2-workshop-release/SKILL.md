---
name: sts2-workshop-release
description: Steam Workshop publish and release workflow for STS2 Balance MOD. Triggers when the user requests to release a new version, publish/upload to Steam Workshop, or update the workshop item. Maintains workshop/WORKSHOP_DESCRIPTION.md and always displays a preview in workshop/tmp/ preview for user confirmation before executing the actual upload.
---

# STS2 Workshop Release Workflow

## Overview

A two-phase, approval-gated workflow for publishing and updating **STS2 Balance MOD** on Steam Workshop:

1. **Phase 1: Sync & Preview (更新文档、生成预览与用户确认)** — 同步更新 `workshop/WORKSHOP_DESCRIPTION.md`，提取 `CHANGELOG.md` 最新版本说明，生成 `workshop/tmp/preview.json` 并在聊天界面呈现，**停止并等待用户确认**。
2. **Phase 2: Build & Upload (打包上传与 Git 提交)** — 用户明确确认后，执行 `dotnet publish` 编译构建、拷贝文件并调用 `ModUploader` 完成上传，最后提交并推送版本与工坊元数据。

---

## 创意工坊文件管理与 Git 约束

`workshop/` 目录不完全被 Git 忽略，仅忽略临时产物与打包目录：

- **受 Git 版本管理**：
  - `workshop/WORKSHOP_DESCRIPTION.md` — 创意工坊主页 Steam BBCode 完整说明文档（随版本迭代更新）
  - `workshop/workshop.json` — 创意工坊元数据配置文件
  - `workshop/image.png` — 创意工坊封面图（< 1MB）
  - `workshop/mod_id.txt` — 创意工坊 Item ID（`3776706758`）
  - `workshop/.gdignore` — Godot 资源扫描忽略标识
- **被 `.gitignore` 忽略（不提交）**：
  - `workshop/content/` — 打包编译产物（dll / pck / json 等发布文件，由 ModUploader 上传）
  - `workshop/tmp/` — 临时预览文件（`preview.json`）

---

## Phase 1: Sync & Preview (生成预览与玩家确认)

### Step 1: Pre-publish Check & Description Update

- 检查 `Sts2BalanceMod.json` 版本号 (`"version": "vX.X.X"`)。
- 确认 `CHANGELOG.md` 顶层包含当前版本的日志段落。
- **更新 `workshop/WORKSHOP_DESCRIPTION.md`**：
  - 核对当前版本的新增卡牌、新增/调整遗物、新增事件、Boss/机制重构以及 Mod 设置项。
  - 将最新内容按 BBCode 格式补充/总结至 `workshop/WORKSHOP_DESCRIPTION.md`，确保工坊简介与 Mod 实际内容严格对齐。

### Step 2: Run Dynamic Sync

在 `image_gen/` 目录下运行：

```bash
uv run sync-workshop
```

该命令会自动：

- 从 `workshop/WORKSHOP_DESCRIPTION.md` 读取完整的 Steam BBCode 主描述
- 从 `CHANGELOG.md` 动态提取最新版本变更记录写入 `changeNote`
- 动态注入当前版本号至 `title`
- 配置分类标签 `["Balance"]` 与 RitsuLib 前置依赖 `3747602295`
- 写入分支限定 `minBranch: "public-beta"` 与 `maxBranch: "public-beta"`
- 导出预览结果至 `workshop/tmp/preview.json` 并更新 `workshop/workshop.json`

### Step 3: Present Preview & Wait for Confirmation (关键阻断节点)

在对话中明确列出预览内容给用户，格式示例：

```markdown
### 📋 创意工坊发布内容预览 (Pending User Confirmation)

- **`title` (Mod 标题)**: 动态嵌入当前版本号（如 `STS2 Balance MOD [v0.3.10] | 《杀戮尖塔2》平衡调整 Mod`）。
- **`branch` (分支兼容)**: 固定声明 `minBranch: "public-beta"` 与 `maxBranch: "public-beta"`，限定在 public-beta 测试分支运行。
- **`description` (主描述)**: 已同步更新 `workshop/WORKSHOP_DESCRIPTION.md` 最新版本内容。
- **`changeNote` (更新说明)**: **动态提取** `CHANGELOG.md` 中最新版本 (如 `## v0.3.10`) 的改动明细。
- **`tags` (分类标签)**: 固定标签 `["Balance"]`。
- **`dependencies` (前置依赖)**: 自动绑定 RitsuLib 创意工坊 ID `3747602295`。

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

### Step 5: Package Content

```powershell
Copy-Item -Path "dist\Sts2BalanceMod\*" -Destination "workshop\content" -Force
```

确认 `workshop/image.png`（封面图 < 1MB）与 `workshop/mod_id.txt`（`3776706758`）存在。

### Step 6: Execute Steam Upload

```powershell
# 完整双语发布（自动更新中文与英文槽位）
dotnet run --project tools/WorkshopPublisher -c Release

# 或者仅更新代码产物（推荐，不碰主页文案）
# dotnet run --project tools/WorkshopPublisher -c Release -- -c
```

### Step 7: Git Commit & Handoff

提交版本与工坊元数据变动记录：

```bash
git add .gitignore workshop/WORKSHOP_DESCRIPTION.md workshop/workshop.json
git commit -m "chore(release): 发布 STS2 Balance MOD vX.X.X 到 Steam 创意工坊"
```

向用户汇报上传成功状态与创意工坊链接。

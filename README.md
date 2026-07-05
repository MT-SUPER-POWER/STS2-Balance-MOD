<div align="center">
<img alt="logo" height="100" width="100" src="docs/img/icon.ico" />
<h2> Sts2BalanceMod </h2>
<p> Sts2BalanceMod — 《杀戮尖塔 2》平衡调整 Mod </p>

<br/>

[![Stars](https://img.shields.io/github/stars/MT-SUPER-POWER/STS2-Balance-MOD?style=flat)](https://github.com/MT-SUPER-POWER/STS2-Balance-MOD/stargazers)
[![Version](https://img.shields.io/github/v/release/MT-SUPER-POWER/STS2-Balance-MOD)](https://github.com/MT-SUPER-POWER/STS2-Balance-MOD/releases)
[![Issues](https://img.shields.io/github/issues/MT-SUPER-POWER/STS2-Balance-MOD)](https://github.com/MT-SUPER-POWER/STS2-Balance-MOD/issues)

</div>

> [!NOTE]
> Beta 版本的分支的三层 BOSS 是沙漏，但是原版分支替换为了时间老头，对于沙漏其关键机制改为：凋零卡是可以打出并消耗的

---

## 安装

### 前置要求

1. **Slay the Spire 2** — Steam 版，版本 ≥ 0.107.0
2. **[BaseLib](https://github.com/STS2-Modding/BaseLib) v3.2.1+** — Mod 加载前置库，需先安装

### 安装步骤

1. 下载本 Mod 的最新发布包（从 [Releases](https://github.com/MT-SUPER-POWER/STS2-Balance-MOD/releases) 页面获取 `.zip`）
2. 将解压后的 **整个文件夹** 放入 STS2 的 Mod 目录：
   - **Windows**: `%AppData%/SlayTheSpire2/mods/`
   - **macOS**: `~/Library/Application Support/SlayTheSpire2/mods/`
   - **Linux**: `~/.local/share/SlayTheSpire2/mods/`
3. 确保 `BaseLib` 也已安装在同一目录
4. 启动游戏，在 Mod 管理页面确认 `Sts2BalanceMod` 已勾选

### 从源码构建

```bash
# 克隆仓库
git clone --recurse-submodules https://github.com/MT-SUPER-POWER/STS2-Balance-MOD.git
cd STS2-Balance-MOD

# 用 Godot 4.x 打开 project.godot，导出为 .pck + .dll
# 或者用 dotnet 命令行发布
dotnet publish -c Release
```

Release 构建产物位于 `dist/Sts2BalanceMod/`（dll + json + pck），打包后为 `dist/Sts2BalanceMod-vX.X.X.zip`；Debug 构建仍自动拷贝到游戏 `mods/` 目录（前提是你在 `Directory.Build.props` 中正确配置了路径）。

---

## 调整内容

> 以下是本 Mod 已经实装的所有平衡与内容调整。版本变更记录见 **[CHANGELOG.md](CHANGELOG.md)**，未完成的待办项见 **[docs/balance-changes.md](docs/balance-changes.md)**。

### 商店

#### 高进阶删牌价格（SHOP-01）
- **原版**：A20+ 删牌一律 50 金币，后续每删一张 +25。
- **MOD 改后**：A6~A19 基础 50 金币（每张 +25），**A20+ 基础 75 金币**（每张 +25）。高进阶下删牌成本压力陡增，需谨慎规划牌组。

### 卡牌调整

| 卡牌 | 角色 | 原版 | MOD 改后 |
|------|------|------|------|
| **骨妹升级挽歌** | 骨妹 | 升级后保留消耗，并额外提高召唤次数 +1 | **升级后不消耗**，但也不再提高召唤次数 |
| **刀舞** | 猎人 | 白卡，打出后消耗 | **删除消耗**词条，稀有度提升为蓝卡，可多次复用 |
| **杂技** | 猎人 | 蓝卡，入手门槛较高 | 降为**白卡**，提升入手概率 |
| **认知偏差**| 猎人 | 每回合 -1 聚焦，永久持续 | 集中在对应回合后自动消失，但是不再扣除提升点数之外的集中 |
| **多重释放** | 机宝  | x+1 次释放 | **保留**词条|
| **放松** | 佩尔 | 格挡 15（升级 17） | **格挡 18（升级 20）** |
| **幽魂形态** | 佩尔 | 获得无实体时仅清除敏捷 Buff，减敏捷负面会保留 | **减敏捷负面一并清除** |
| **袖里乾坤** | 猎人 | 每一次打出后，减少费用，定位和到刀舞冲突 | 从卡池中删除 |

### 一代卡牌回归（新增卡牌）

| 卡牌 | 角色 | 描述 |
|------|------|------|
| **死亡收割** | 战士 | 对所有敌人造成伤害，并回复等量于实际造成的非格挡伤害的生命。 |
| **硬撑** | 战士 | 获得格挡，将 2 张伤口加入手牌。 |
| **全神贯注** | 猎人 | 丢弃若干张牌，获得能量。 |
| **电动力学** | 机器人 | 召唤闪电球，且闪电球改为攻击所有敌人。 |

### 怪物与 Boss

| 名称 | 编号 | 原版机制 | MOD 改后机制 |
| :--- | :--- | :--- | :--- |
| **沙漏凋零卡** | MON-01 | 永世沙漏 Boss 生成的凋零卡无法打出，直接进入消耗堆。 | 凋零卡可以**打出并消耗**，保留成长机制。 |
| **时间吞噬者 Boss** | MON-02 / MON-03 | - | 替换三层永世沙漏 Boss 候选；时间扭曲触发时有**时钟弹出视觉效果**；多人模式血量按玩家数放大、TimeWarp 计数同步调整；BGM 使用 AFTP 风格的 FadeIn/FadeOut。 |
| **收藏家 Boss** | MON-04 | - | 移植收藏家本体与 Torch Head 召唤物，进入 Boss 候选池。 |

### 事件

| 事件名称 | 编号 / 版本 | 详情与 MOD 修改内容 |
| :--- | :--- | :--- |
| **一代事件回归** | STS1-EVENT-01~08 | **诅咒书本**：可获得死灵之书、尼利的宝典、英雄宝典。<br>**红面具**（限定 Act 2）：可选择交金或与红面具三人帮战斗获取红面具。<br>**J.A.X.**（增益研究者）：失去生命换取力量 Buff。<br>**神圣泉水**：治疗（删除原版的诅咒副作用）。<br>**牧师**：提供治疗 / 删牌选项。<br>**心灵绽放**（限定 Act 3）：多分支选择，含自定义 BOSS 战。<br>**大转盘**：自定义转盘 UI 与小游戏。<br>**红面具大人之墓**：可获得红面具或戴上红面具。 |
| **老乞丐事件** | v0.0.7 新增 | 至少 75 金币才会进入事件池；给金币后切换为牧师图，进入删牌阶段。 |
| **旧日垃圾堆** | EVENT-02 | MOD 改后奖励池加入**御守**遗物。 |。

### 遗物

| 遗物 | 类型 / 编号 | 描述 / MOD 修改内容 |
| :--- | :--- | :--- |
| **日晷** | 商店 | 每将抽牌堆洗牌 3 次（跨战斗保留），获得 2 点能量。 |
| **橙色药丸** | 商店 | 同一回合打出攻击 / 技能 / 能力各一张后，移除所有负面效果（女王的魂缚锁链除外）。 |
| **枯木树枝** | 稀有 | 每消耗一张牌，随机将一张牌加入手牌（虚无牌触发时给当回合保留）。 |
| **御守** | 事件 | 抵消接下来获得的 2 张诅咒牌（带计数器）。 |
| **宁静烟斗** | 稀有 | 在火堆新增"烟斗"选项，可删除一张牌。 |
| **微笑面具** | 普通 | 删牌价格固定为 50 金币。 |
| **咖啡杯** | 先古 | 无法在火堆休息，但每回合 +1 费用。 |
| **融合之锤** | 先古 | 无法锻造，但每回合 +1 费用。 |
| **诅咒钥匙** | 先古 | 每回合 +1 费用，每次打开宝箱获得一张随机诅咒。 |
| **矮人铁砧** | 商店 | 拾起时为 3 张牌附加"锻造"附魔，被附魔的牌费用永久 -1（最低 0 费）。 |
| **坚固钳子** | 先古 | **原版**：保留 10 护甲。<br>**MOD 改后**：保留 **20 护甲**。 |
| **活雾** | 先古 | **原版**：删除 3 张牌。<br>**MOD 改后**：删除 **4 张牌**。 |
| **红面具** | 事件 | **MOD 改后**：从一般共享遗物池**移除**，只通过红面具相关事件获得。 |

---

## 图片资源处理

新增`卡牌/遗物/能力/事件`后，需要准备对应的图片素材，使用脚本批量处理。

> **前置要求**：需要安装 [uv](https://docs.astral.sh/uv/)（Python 包管理器）。
> ```bash
> # macOS / Linux
> curl -LsSf https://astral.sh/uv/install.sh | sh
> # Windows (PowerShell)
> powershell -c "irm https://astral.sh/uv/install.ps1 | iex"
> ```
> 首次使用在 `image_gen/` 目录下执行 `uv sync` 即可创建虚拟环境并安装依赖。

### 卡牌立绘

```bash
# 首次使用：安装依赖（需要先安装 uv，详见下方说明）
cd image_gen && uv sync && cd ..

# 将原始素材放入 image_gen/source/cards/
# 运行处理脚本
uv run cards

# 只处理指定文件
uv run cards death_reap.png

# 缩放模式（默认 cover 居中裁切填满）
uv run cards --mode contain    # 完整显示（留透明边）
uv run cards --mode stretch    # 拉伸（可能变形）

# 裁切锚点（cover 模式）
uv run cards --anchor top      # 顶部对齐
uv run cards --anchor bottom   # 底部对齐
```

- 源图 → 大图 `1000×760` → `images/card_portraits/big/`
- 源图 → 小图 `500×380` → `images/card_portraits/`

### 遗物图标

```bash
# 将遗物图标放入 image_gen/source/relics/
# 轮廓图放入 image_gen/source/relics/outlines/
uv run relics

# 只处理轮廓图
uv run relics --outline-only

# 只处理指定文件
uv run relics Sundial.png
```

- 源图 → 大图 `256×256` → `images/relics/big/`
- 源图 → 小图 `94×94` → `images/relics/`
- 轮廓图 → `94×94` → `images/relics/{name}_outline.png`

> 图片路径是自动解析的：只需文件名为 `{id小写}.png`，代码中的 `Sts2RelicModel` / `Sts2CardModel` 基类会自动拼接完整路径。

### 火堆选项图标

火堆按钮图标由游戏本体按 `res://images/ui/rest_site/option_{OptionId小写}.png` 读取；Mod 自定义火堆选项通过 Harmony 接管按钮刷新，统一从 `Sts2BalanceMod/images/ui/rest_site/` 读取。

```bash
# 将源图放入 image_gen/source/rest_site_options/
uv run rest-site-options

# 只处理指定文件
uv run rest-site-options smoke.png
```

- 源图 → 选项图标 `256×169` → `Sts2BalanceMod/images/ui/rest_site/option_smoke.png`

> [!NOTE] 还有其他更多的类型的图片处理脚本都放在我们的 `image_gen/pyproject.toml` 中，使用 `uv run <type>` 即可执行。

---

## 开发

### 技术栈

- **游戏**: Slay the Spire 2 (Godot 4 / .NET)
- **Mod 框架**: BaseLib 3.2.1+ (自定义模型注册与联机自定义消息兼容)
- **补丁引擎**: Harmony 2.x (运行时方法劫持)
- **语言**: C# 12



---

## 发布到 GitHub Releases

项目使用模块化的发布脚本，发布流程由用户手动管理 Git 标签，脚本负责自动化打包和附件上传。

### 前置条件

1. 本机环境支持 `dotnet publish -c Release`
2. 已安装 [GitHub CLI](https://cli.github.com/) 并登录：`gh auth login`

### 发布步骤

1. **更新版本号**：在 `CHANGELOG.md` 写好对应版本段落（如 `# v0.0.5`）。
2. **同步配置**（可选）：运行脚本更新 `Sts2BalanceMod.json`。
   ```powershell
   .\Hooks\release.ps1 -Version 0.0.5 -UpdateJson
   ```
3. **手动推送**：提交代码并打上 Tag 推送到 GitHub。
   ```bash
   git add .
   git commit -m "chore: release v0.0.5"
   git tag v0.0.5
   git push origin main v0.0.5
   ```
4. **打包上传**：运行脚本执行构建并上传 zip 到对应的 Release。
   ```powershell
   .\Hooks\release.ps1 -Version 0.0.5 -Build -Upload
   ```

### 常用命令

| 命令                                         | 说明                                                |
| -------------------------------------------- | --------------------------------------------------- |
| `.\Hooks\release.ps1 -Version 0.0.5 -Build`  | **仅本地打包**：构建并生成 `dist/*.zip`             |
| `.\Hooks\release.ps1 -Version 0.0.5 -Upload` | **仅上传**：将已存在的 zip 上传到 GitHub (支持覆盖) |
| `.\Hooks\release.ps1 -Version 0.0.5 -All`    | **全自动化**：同步 JSON + 构建 + 上传               |

> **提示**：`-Upload` 步骤会自动检测 GitHub Release。如果不存在，它会创建一个包含更新日志说明的 Release 页面，并将包上传。如果已存在附件，会直接覆盖更新。

---

## 已知问题

历史 BUG 记录见 [docs/balance-changes.md](docs/balance-changes.md#现有的问题以及无法解决的问题)。

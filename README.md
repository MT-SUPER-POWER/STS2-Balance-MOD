<div align="center">
<img alt="logo" height="100" width="100" src="docs/img/icon.ico" />
<h2> Sts2BalanceMod </h2>
<p> Sts2BalanceMod — 《杀戮尖塔 2》平衡调整 Mod </p>

<br/>

[![Stars](https://img.shields.io/github/stars/MT-SUPER-POWER/STS2-Balance-MOD?style=flat)](https://github.com/MT-SUPER-POWER/STS2-Balance-MOD/stargazers)
[![Version](https://img.shields.io/github/v/release/MT-SUPER-POWER/STS2-Balance-MOD)](https://github.com/MT-SUPER-POWER/STS2-Balance-MOD/releases)
[![Issues](https://img.shields.io/github/issues/MT-SUPER-POWER/STS2-Balance-MOD)](https://github.com/MT-SUPER-POWER/STS2-Balance-MOD/issues)

</div>

---

## 安装

### 前置要求

1. **Slay the Spire 2** — Steam 版，版本 ≥ 0.107.0
2. **[BaseLib](https://github.com/STS2-Modding/BaseLib) v3.2.0+** — Mod 加载前置库，需先安装

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

Release 构建产物位于 `dist/Sts2BalanceMod/`（dll + json + pck），打包后为 `dist/Sts2BalanceMod-vX.X.X.zip`；Debug 构建仍自动拷贝到游戏 `mods/` 目录。

---

## 调整内容

版本更新记录见 **[CHANGELOG.md](CHANGELOG.md)**；完整需求清单和状态见 **[docs/balance-changes.md](docs/balance-changes.md)**。

| 分类 | 调整项                                | 状态         |
| ---- | ------------------------------------- | ------------ |
| 商店 | V6+ 高阶删牌价格 75+25/次             | ✅ 已实现     |
| 卡牌 | 骨妹升级挽歌 → 不消耗、不提高召唤次数 | ✅ 已实现     |
| 卡牌 | 刀舞 → 删除消耗词条、升为蓝卡         | ✅ 已实现     |
| 卡牌 | 杂技 → 蓝卡降白卡                     | ✅ 已实现     |
| 卡牌 | 认知偏差 → 五回合后停止扣聚焦         | ✅ 已实现     |
| 卡牌 | 多重释放 → 升级保留                   | ✅ 已实现     |
| 卡牌 | 幽魂形态 → 清除减敏捷负面             | ✅ 已实现     |
| 卡牌 | 腐蚀波 → 单体效果                     | 🔲 待处理     |
| 回归 | 死亡收割 / 硬撑 / 全神贯注 / 电动力学 | ✅ 已实现     |
| 怪物 | Boss 沙漏凋零卡可打出消耗             | ✅ 已实现     |
| 怪物 | Boss 时间吞噬者                       | ✅ 已实现     |
| 怪物 | Boss 收藏家                           | ✅ 已实现     |
| 事件 | 双拳机器人、牧师删牌、JAX 变牌        | 🔲 待处理     |
| 事件 | 旧日垃圾堆加入御守                    | ✅ 已实现     |
| 遗物 | 日晷 / 药丸 / 树枝 / 坚固钳子         | ✅ 已实现     |
| 遗物 | 宁静烟斗 / 微笑面具 / 达福遗物补充    | ✅ 部分已实现 |

---

## 项目结构

```
Sts2BalanceMod/
├── Sts2BalanceMod.json              # Mod 元信息（id、版本、依赖）
├── Sts2BalanceModCode/              # C# 源代码
│   ├── MainFile.cs                  # Mod 入口，Harmony 初始化
│   ├── Abstract/                    # 抽象基类
│   │   ├── Sts2CardModel.cs         #   卡牌基类（自动加载图片路径）
│   │   ├── Sts2PowerModel.cs        #   能力基类
│   │   └── Sts2RelicModel.cs        #   遗物基类
│   ├── Cards/                       # 新增卡牌
│   │   ├── DeathReap.cs             #   死亡收割（战士）
│   │   ├── PowerThought.cs          #   硬撑（战士）
│   │   ├── Concentrate.cs           #   全神贯注（猎人）
│   │   └── Electrodynamics.cs       #   电动力学（机器人）
│   ├── Powers/                      # 新增能力
│   │   ├── ElectrodynamicsPower.cs  #   电动力学能力标记
│   │   ├── TimeWarpPower.cs         #   时间吞噬者时间扭曲
│   │   └── Sts2BalanceModPower.cs   #   [已废弃] 旧基类
│   ├── Encounters/                  # 新增遭遇
│   │   ├── TimeEaterBoss.cs         #   时间吞噬者 Boss
│   │   └── CollectorBoss.cs         #   收藏家 Boss
│   ├── Monsters/                    # 新增怪物
│   │   ├── TimeEater.cs             #   时间吞噬者
│   │   ├── Collector.cs             #   收藏家
│   │   └── CollectorTorchHead.cs    #   收藏家召唤物
│   ├── RestSite/                    # 新增火堆选项
│   │   └── PeacePipeRestSiteOption.cs # 宁静烟斗删牌选项
│   ├── Relics/                      # 新增遗物
│   │   ├── Sundial.cs               #   日晷
│   │   ├── OrangePill.cs            #   药丸
│   │   ├── DeadBranch.cs            #   树枝
│   │   ├── Omamori.cs               #   御守
│   │   └── PeacePipe.cs             #   宁静烟斗
│   ├── Patches/                     # Harmony 补丁（按功能域分子目录）
│   │   ├── Cards/                   #   单卡属性/行为修改
│   │   ├── CardPools/               #   角色卡池增删
│   │   ├── Powers/                  #   原版能力行为修改
│   │   ├── Orbs/                    #   球体行为修改
│   │   ├── Merchant/                #   商店/删牌价格
│   │   ├── Relics/                  #   原版遗物行为修改
│   │   └── Events/                  #   事件奖励池修改
│   └── Extensions/
│       └── StringExtensions.cs      # 资源路径工具方法
├── Sts2BalanceMod/                  # Mod 资源文件
│   ├── localization/                # 本地化文本（eng/zhs）
│   │   ├── eng/                     #   英文
│   │   │   ├── cards.json
│   │   │   ├── powers.json
│   │   │   └── relics.json
│   │   └── zhs/                     #   简体中文
│   │       ├── cards.json
│   │       ├── powers.json
│   │       └── relics.json
│   └── images/                      # 图片资源
│       ├── card_portraits/          #   卡牌立绘（500x380 + big/1000x760）
│       ├── powers/                  #   能力图标（big/ 大图）
│       └── relics/                  #   遗物图标（94x94 + big/256x256 + 轮廓图）
├── image_gen/                       # 图片批处理脚本
│   ├── cards.py                     #   卡牌立绘切图工具
│   ├── relics.py                    #   遗物图标生成工具
│   ├── powers.py                    #   能力图标生成工具
│   ├── pyproject.toml               #   项目配置与依赖声明
│   ├── uv.lock                      #   依赖锁定文件（自动生成）
│   ├── .python-version              #   Python 版本锁定
│   └── source/                      #   原始素材目录
│       ├── cards/
│       ├── powers/
│       ├── relics/
│       └── rest_site_options/
├── docs/                            # 文档
│   ├── README.md                    #   docs 目录索引
│   ├── balance-changes.md           #   平衡调整需求清单
│   ├── sts2-modding-guide.md        #   Mod 制作教程
│   └── references/WatcherMod/       #   参考 Mod（git submodule）
└── .github/workflows/               #   CI/CD
    └── release.yml                  #   推送 Tag 时写入 Release 说明
```

---

## 图片资源处理

新增卡牌/遗物/能力后，需要准备对应的图片素材，使用脚本批量处理。

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

---

## 开发

### 技术栈

- **游戏**: Slay the Spire 2 (Godot 4 / .NET)
- **Mod 框架**: BaseLib 3.2.0+ (自定义模型注册)
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

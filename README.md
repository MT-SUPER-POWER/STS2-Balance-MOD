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

构建产物位于 `.godot/mono/temp/bin/Release/publish/`。

---

## 调整内容

版本更新记录见 **[CHANGELOG.md](CHANGELOG.md)**；完整需求清单和状态见 **[docs/balance-changes.md](docs/balance-changes.md)**。

| 分类 | 调整项 | 状态 |
|------|--------|------|
| 商店 | V6+ 高阶删牌价格 75+25/次 | ✅ 已实现 |
| 卡牌 | 骨妹挽歌 → 不消耗 | ✅ 已实现 |
| 卡牌 | 刀舞 → 删除消耗词条、升为蓝卡 | ✅ 已实现 |
| 卡牌 | 杂技 → 蓝卡降白卡 | ✅ 已实现 |
| 卡牌 | 认知偏差 → 五回合后停止扣聚焦 | ✅ 已实现 |
| 卡牌 | 多重释放 → 升级保留 | ✅ 已实现 |
| 卡牌 | 幽魂形态 → 清除减敏捷负面 | ✅ 已实现 |
| 卡牌 | 腐蚀波 → 单体效果 | 🔲 待处理 |
| 回归 | 死亡收割 / 硬撑 / 全神贯注 / 电动力学 | ✅ 已实现 |
| 怪物 | Boss 沙漏凋零卡可打出消耗 | ✅ 已实现 |
| 怪物 | Boss 时间吞噬者 | 🔲 待处理 |
| 事件 | 双拳机器人、牧师删牌、JAX 变牌 | 🔲 待处理 |
| 事件 | 旧日垃圾堆加入御守 | ✅ 已实现 |
| 遗物 | 日晷 / 药丸 / 树枝 / 坚固钳子 | ✅ 已实现 |
| 遗物 | 宁静烟斗 / 微笑面具 / 达福遗物补充 | 🔲 待处理 |

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
│   │   └── Sts2BalanceModPower.cs   #   [已废弃] 旧基类
│   ├── Relics/                      # 新增遗物
│   │   ├── Sundial.cs               #   日晷
│   │   ├── OrangePill.cs            #   药丸
│   │   ├── DeadBranch.cs            #   树枝
│   │   └── Omamori.cs               #   御守
│   ├── Patches/                     # Harmony 补丁（修改原版行为）
│   │   ├── MerchantCardRemovalCostPatch.cs    # 删牌价格调整
│   │   ├── DirgeExhaustPatch.cs               # 骨妹挽歌不消耗
│   │   ├── BladeDanceExhaustPatch.cs          # 刀舞去消耗
│   │   ├── AcrobaticsRarityPatch.cs           # 杂技降级
│   │   ├── BiasedCognitionPowerPatch.cs       # 认知偏差修复
│   │   ├── MultiCastRetainPatch.cs            # 多重释放保留
│   │   ├── WraithFormNoDexDebuffPatch.cs      # 幽魂形态去除减敏捷
│   │   ├── DefectCardPoolPatch.cs             # 移除吞噬暗影
│   │   ├── SilentCardPoolPatch.cs             # 移除袖里乾坤
│   │   ├── LightningOrbElectrodynamicsPatch.cs  # 电动力学 AOE
│   │   ├── AgeonglassWitherExhaustPatch.cs    # Boss 沙漏凋零卡可打出
│   │   ├── SturdyClampEnhanced.cs             # 坚固钳子护甲增强
│   │   ├── TrashHeapAddCustomRelicAndCard.cs  # 旧日垃圾堆加入御守
│   │   └── RelaxBlockPatch.cs                 # 放松格挡值调整
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
│   ├── requirements.txt             #   Python 依赖（Pillow）
│   └── source/                      #   原始素材目录
│       ├── cards/
│       ├── powers/
│       └── relics/
├── docs/                            # 文档
│   ├── README.md                    #   docs 目录索引
│   ├── balance-changes.md           #   平衡调整需求清单
│   ├── sts2-modding-guide.md        #   Mod 制作教程
│   └── references/WatcherMod/       #   参考 Mod（git submodule）
└── .github/workflows/               # CI/CD
    └── publish.yml                  #   自动构建 + 发布
```

---

## 图片资源处理

新增卡牌/遗物/能力后，需要准备对应的图片素材，使用脚本批量处理。

### 卡牌立绘

```bash
# 安装依赖
pip install -r image_gen/requirements.txt

# 将原始素材放入 image_gen/source/cards/
# 运行处理脚本
python image_gen/cards.py

# 只处理指定文件
python image_gen/cards.py death_reap.png

# 缩放模式（默认 cover 居中裁切填满）
python image_gen/cards.py --mode contain    # 完整显示（留透明边）
python image_gen/cards.py --mode stretch    # 拉伸（可能变形）

# 裁切锚点（cover 模式）
python image_gen/cards.py --anchor top      # 顶部对齐
python image_gen/cards.py --anchor bottom   # 底部对齐
```

- 源图 → 大图 `1000×760` → `images/card_portraits/big/`
- 源图 → 小图 `500×380` → `images/card_portraits/`

### 遗物图标

```bash
# 将遗物图标放入 image_gen/source/relics/
# 轮廓图放入 image_gen/source/relics/outlines/
python image_gen/relics.py

# 只处理轮廓图
python image_gen/relics.py --outline-only

# 只处理指定文件
python image_gen/relics.py Sundial.png
```

- 源图 → 大图 `256×256` → `images/relics/big/`
- 源图 → 小图 `94×94` → `images/relics/`
- 轮廓图 → `94×94` → `images/relics/{name}_outline.png`

> 图片路径是自动解析的：只需文件名为 `{id小写}.png`，代码中的 `Sts2RelicModel` / `Sts2CardModel` 基类会自动拼接完整路径。

---

## 开发

### 技术栈

- **游戏**: Slay the Spire 2 (Godot 4 / .NET)
- **Mod 框架**: BaseLib 3.2.0+ (自定义模型注册)
- **补丁引擎**: Harmony 2.x (运行时方法劫持)
- **语言**: C# 12



---

## GitHub Release 自动构建

推送 Tag 后，`.github/workflows/publish.yml` 会自动构建 Mod 并发布到 GitHub Releases。

### 发布步骤

1. 在 `CHANGELOG.md` 写好对应版本段落（`# vx.x.x`）
2. 打标签并推送：

```bash
git tag v0.0.4
git push origin v0.0.4
```

3. Actions 完成后，Release 页面会显示 `CHANGELOG.md` 中该版本的改动说明，并附带 `.zip` 安装包

### Actions 流程

1. 从 `CHANGELOG.md` 提取 `# vx.x.x` 段落作为 Release 说明
2. 通过 DepotDownloader 下载 STS2（用于编译引用 `sts2.dll`）
3. 安装 Godot 4.5.1 与 .NET 9 SDK
4. 执行 `dotnet publish -c Release`，打包 `Sts2BalanceMod/` 文件夹为 `.zip`
5. 创建 GitHub Release 并上传产物

### 所需 Secrets

在仓库 **Settings → Secrets and variables → Actions** 中配置：

| Secret | 说明 |
|--------|------|
| `STEAM_USERNAME` | 拥有 STS2 的 Steam 账号 |
| `STEAM_PASSWORD` | 对应密码 |
| `STEAM_GUARD_CODE` | 可选；开启 Steam 令牌时需要填写一次性验证码 |

> **NOTE**: CI 使用 `public-beta` 分支下载游戏（Depot `2868841`）。建议使用专用 Steam 小号，避免个人主号频繁输入令牌。

---

## 已知问题

当前无已知未修复问题。历史 BUG 记录见 [docs/balance-changes.md](docs/balance-changes.md#现存-bug)。

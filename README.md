# Sts2BalanceMod — 《杀戮尖塔 2》平衡调整 Mod

> 作者：Momo | 版本：v0.0.2 | 支持游戏版本：≥ 0.107.0

对 STS2 进行多方面平衡调整——商店定价、卡牌稀有度/效果、新增老版回归卡牌、以及机制修复。

---

## 安装

### 前置要求

1. **Slay the Spire 2** — Steam 版，版本 ≥ 0.107.0
2. **[BaseLib](https://github.com/STS2-Modding/BaseLib) v3.2.0+** — Mod 加载前置库，需先安装

### 安装步骤

1. 下载本 Mod 的最新发布包（从 [Releases](../../releases) 页面获取 `.zip`）
2. 将解压后的 **整个文件夹** 放入 STS2 的 Mod 目录：
   - **Windows**: `%AppData%/SlayTheSpire2/mods/`
   - **macOS**: `~/Library/Application Support/SlayTheSpire2/mods/`
   - **Linux**: `~/.local/share/SlayTheSpire2/mods/`
3. 确保 `BaseLib` 也已安装在同一目录
4. 启动游戏，在 Mod 管理页面确认 `Sts2BalanceMod` 已勾选

### 从源码构建

```bash
# 克隆仓库
git clone https://github.com/MT-SUPER-POWER/Sts2BalanceMod.git
cd Sts2BalanceMod

# 用 Godot 4.x 打开 project.godot，导出为 .pck + .dll
# 或者用 dotnet 命令行发布
dotnet publish -c Release
```

构建产物位于 `.godot/mono/temp/bin/Release/publish/`。

---

## 调整内容

所有调整项的完整清单和状态，请查看 **[docs/balance-changes.md](docs/balance-changes.md)**。

| 分类 | 调整项 | 状态 |
|------|--------|------|
| 商店 | V6+ 高阶删牌价格 75+25/次 | ✅ 已实现 |
| 卡牌 | 骨妹挽歌 → 不消耗 | ✅ 已实现 |
| 卡牌 | 刀舞 → 删除消耗词条 | ✅ 已实现 |
| 卡牌 | 杂技 → 蓝卡降白卡 | ✅ 已实现 |
| 卡牌 | 认知偏差 → 聚焦归零自动移除 | ✅ 已实现 |
| 卡牌 | 多重释放 → 升级保留 | ✅ 已实现 |
| 卡牌 | 幽魂形态 → 清除减敏捷负面 | ✅ 已实现 |
| 回归 | 死亡收割（战士） | ✅ 已实现 |
| 回归 | 硬撑（战士） | ✅ 已实现 |
| 回归 | 全神贯注（猎人） | ✅ 已实现 |
| 回归 | 电动力学（机器人，替换吞噬暗影） | ✅ 已实现 |
| 遗物 | 日晷 — 每 3 次洗牌获得 2 点能量 | ✅ 已实现 |
| 怪物 | Boss 沙漏 / 感染棱柱 | 🔲 待处理 |
| 事件 | 双拳机器人、牧师删牌、JAX 变牌等 | 🔲 待处理 |
| 遗物 | 药丸祛除负面 | 🔲 待处理 |

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
│   │   └── Sundial.cs               #   日晷
│   ├── Patches/                     # Harmony 补丁（修改原版行为）
│   │   ├── MerchantCardRemovalCostPatch.cs   # 删牌价格调整
│   │   ├── DirgeExhaustPatch.cs              # 骨妹挽歌不消耗
│   │   ├── BladeDanceExhaustPatch.cs         # 刀舞去消耗
│   │   ├── AcrobaticsRarityPatch.cs          # 杂技降级
│   │   ├── BiasedCognitionPowerPatch.cs      # 认知偏差修复
│   │   ├── MultiCastRetainPatch.cs           # 多重释放保留
│   │   ├── WraithFormNoDexDebuffPatch.cs     # 幽魂形态去除减敏捷
│   │   ├── DefectCardPoolPatch.cs            # 移除吞噬暗影
│   │   ├── LightningOrbElectrodynamicsPatch.cs # 电动力学 AOE
│   │   └── RelaxBlockPatch.cs                # 放松格挡值调整
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
│   └── sts2-modding-guide.md        #   Mod 制作教程
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

### 核心概念

| 概念 | 说明 |
|------|------|
| `[HarmonyPatch]` | 标记一个类是补丁类，指定要劫持的目标方法 |
| `[HarmonyPrefix]` | 在目标方法**之前**执行，返回 `false` 可跳过原方法 |
| `[HarmonyPostfix]` | 在目标方法**之后**执行，可读取/修改返回值 |
| `[Pool(...)]` | 通过 BaseLib 将卡牌/遗物注册到指定角色卡池 |
| `[SavedProperty]` | 标记需要自动存档/读档的属性 |
| `ConstructedCardModel` | BaseLib 提供的自定义卡牌基类，无需 JSON 定义即可创建新卡 |

### 添加新卡牌

1. 在 `Cards/` 下新建类，继承 `Sts2CardModel`
2. 用 `[Pool(typeof(XxxCardPool))]` 指定卡池
3. 构造函数中设置费用、类型、稀有度、目标类型、基础数值
4. 重写 `OnPlay()` 实现卡牌效果
5. 在 `image_gen/source/cards/` 放入立绘素材，运行 `cards.py`
6. 在 `localization/` 中添加卡牌名称和描述文本

---

## GitHub Release 自动构建

项目配置了 GitHub Actions（`.github/workflows/publish.yml`），当推送 Tag 时自动构建并发布 Release：

```bash
# 打标签并推送
git tag v0.1.0
git push origin v0.1.0
```

Actions 流程会自动：
1. 安装 .NET SDK
2. 执行 `dotnet publish -c Release`
3. 打包构建产物为 `.zip`
4. 上传到 GitHub Releases 页面

> **注意**: 当前 `publish.yml` 为空模板，需要补充完整的 CI 配置后再启用。

---

## 已知问题

- **BUG-01**: 联机报错黑屏 — `Parameter 'ModelId entry ID 1920 is out of range! We have 1627 entries'`
  可能与 BaseLib 模型注册数量超出限制有关，排查中。

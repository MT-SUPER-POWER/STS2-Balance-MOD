<div align="center">
  <img alt="logo" height="100" width="100" src="docs/img/icon.ico" />
  <h2> Sts2BalanceMod </h2>
  <p> Sts2BalanceMod — 《杀戮尖塔 2》平衡调整 Mod </p>
  <br/>
  <a href="https://github.com/MT-SUPER-POWER/STS2-Balance-MOD/stargazers">
    <img src="https://img.shields.io/github/stars/MT-SUPER-POWER/STS2-Balance-MOD?style=flat" alt="Stars" />
  </a>
  <a href="https://github.com/MT-SUPER-POWER/STS2-Balance-MOD/releases">
    <img src="https://img.shields.io/github/v/release/MT-SUPER-POWER/STS2-Balance-MOD" alt="Version" />
  </a>
  <a href="https://github.com/MT-SUPER-POWER/STS2-Balance-MOD/issues">
    <img src="https://img.shields.io/github/issues/MT-SUPER-POWER/STS2-Balance-MOD" alt="Issues" />
  </a>
</div>


## 安装

### 前置要求

1. **[Slay the Spire 2](https://store.steampowered.com/app/2868840/Slay_the_Spire_2/)** 版本 ≥ 0.108.0
2. **[BaseLib](https://github.com/Alchyr/BaseLib-StS2)** — Mod 加载前置库，需先安装，版本 ≥ 3.3.3+

> [!warning]
>
> v3.3.5 开始的 baselib 又非常严重的加载同步冲突，杀戮尖塔2 在进入游戏的时候会开始云同步数据，baselib 加载好在这里会造成严重的冲突，具体去 issue 可以看看
>
> 所以建议你在游戏的时候，关闭我们的 steam 云同步，点击 steam 游戏的齿轮，在通用里面关闭云存档的选项

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

**CI/CD 自动发布**：
项目配置了完整的 GitHub Actions 工作流。当推送格式为 `vX.X.X` 的 Git Tag 时，GitHub Actions 会在云端自动下载 .NET 和 Godot Mono 环境，调用还原引用编译，自动导出 PCK 资源并压缩成 ZIP 附件，最后创建并发布到 GitHub Release 中，无须本地手动执行脚本进行打包上传。

---

## 调整内容

> [!note]
> 以下是本 Mod 已经实装的所有平衡与内容调整。
>
> 版本变更记录见 **[CHANGELOG.md](CHANGELOG.md)**，未完成的待办项见 **[docs/balance-changes.md](docs/balance-changes.md)**。

### 商店

#### 高进阶删牌价格（SHOP-01）

- **原版**：A6+ 删牌一律 50 金币，后续每删一张 +25。
- **MOD 改后**：小于A6 基础 50 金币（每张 +25），**A6+ 基础 75 金币**（每张 +25）。

### 卡牌调整

| 卡牌 | 角色 | 类型 | 原版 | MOD 改后 |
|------|------|:----:|------|------|
| **挽歌 [Dirge]** | 骨妹 | 能力 | 升级后召唤次数 +1，消耗，灵魂+ | 升级后**额外追加保留词条** |
| **刀舞 [Blade Dance]** | 猎人 | 技能 | 白卡（普通），打出后消耗 | 删除消耗词条，稀有度升为**蓝卡（罕见）**，可多次复用 |
| **杂技 [Acrobatics]** | 猎人 | 技能 | 蓝卡（罕见），入手门槛较高 | 稀有度降为**白卡（普通）**，提升入手概率 |
| **认知偏差 [Biased Cognition]** | 猎人 | 能力 | 每回合 -1 聚焦，永久持续 | 聚焦在对应回合后自动消失，不再扣除超出提升点数的聚焦 |
| **多重释放 [Multicast]** | 鸡煲 | 技能 | 升级后 X+1 次释放 | 升级改为仅追加**保留**词条（取消原版 X+1）|
| **放松 [Relax]** | 佩尔 | 技能 | 格挡 15（升级 17）| 格挡调整为 **18（升级 20）** |
| **幽魂形态 [Wraith Form]** | 佩尔 | 技能 | 获得无实体后每回合累积减敏捷负面 | 打出时只施加无实体，**彻底删除减敏捷负面效果** |
| **袖里乾坤 [Up My Sleeve]** | 猎人 | 技能 | 每次打出后减少费用，定位与刀舞冲突 | 从猎人卡池中**完全移除** |
| **燃料 [Fuel]** | 鸡煲 | 技能 | 将所有状态牌转换，2 费 | 改为 **1 费**，获得 1 点能量并**抽 1（升级：2）张牌** |
| **能量汲取 [Drain Power]** | 骨妹 | 攻击 | 造成 10/12 伤害，升级后随机升级弃牌堆 3 张 | 伤害降为 **6/8**；基础随机升级弃牌堆 **2 张**，升级后改为升级弃牌堆**全部可升级牌** |
| **吸引仇恨 [Pull Aggro]** | 骨妹 | 技能 | 升级后原版数值 | 升级后调整为：**召唤骨头 6 个，格挡 9 点** |
| **凋零 [Wither]** | 永世沙漏 | 状态 | 不可打出，进入消耗堆 | 改为 **1 费可打出**，打出后消耗 |
| **辉光 [Glow]** | 佩尔 | 技能 | 获得星尘，下回合额外抽牌 | 改为**当回合立即抽 2 张**，升级额外多获得 1 星尘 |
| **放血 [Bloodletting]** | 战士 | 技能 | 蓝卡（罕见）（v1.0.9 修改） | 稀有度改回**白卡（普通）** |
| **创世之柱 [Pillar of Creation]** | 储君 | 能力 | 每回合第一次生成卡牌时获得 5/8 点格挡 | 改为每生成一张卡牌均获得 **3/4 点格挡**（取消每回合首次限制） |
| **残酷 [Cruelty]** | 战士 | 能力 | 升级后原版数值，下放为战士卡牌 | 从战士卡池中**完全移除**（由“进化”卡牌替代） |
| **探寻 [Dowsing]** | 无色 | 任务 | 进入 5 个 ? 房间后转化为「丰饶」 | 调整为进入 **4 个 ? 房间**后转化为「丰饶」 |
| **魔球 [The Ball]** | 无色 | 攻击 | 每次打出后伤害成长 **10 / 15** | 成长幅度回调为 **15 / 20** |
| **触媒 [Accelerant]** | 猎人 | 能力 | 蓝卡（罕见） | 稀有度回调为**金卡（稀有）** |
| **计划妥当 [Well-Laid Plans]** | 猎人 | 能力 | 金卡（稀有），1 / 0 费；回合结束时不弃牌 | 罕见，1 费；回合结束时保留最多 **1 / 2** 张牌，未选择的手牌正常弃置，并保留多人可用支持 |
| **华丽收场 [Grand Finale]** | 猎人 | 攻击 | 0 费，抽牌堆有 0 张牌时打出 | **X 费**卡牌，打出条件调整为**抽牌堆的牌数小于或等于 X（即当前能量）** |


### 一代卡牌回归

| 卡牌 | 角色 | 类型 | 稀有度 | 费用 | 效果（基础 / 升级） |
|------|------|:----:|:------:|:----:|------|
| **死亡收割 [Death Reap]** | 战士 | 攻击 | 稀有 | 2 | 消耗。对所有敌人造成 **4 / 6** 点伤害，并回复等量于实际造成的非格挡伤害的生命值。 |
| **硬撑 [Power Through]** | 战士 | 技能 | 罕见 | 1 | 获得 **15 / 20** 点格挡，将 2 张伤口加入手牌。 |
| **全神贯注 [Concentrate]** | 猎人 | 技能 | 罕见 | 0 | 丢弃 **3 / 2** 张牌，获得 2 点能量。 |
| **电动力学 [Electrodynamics]** | 机器人 | 能力 | 稀有 | 2 | 召唤 **2 / 3** 个闪电球，且闪电球改为攻击所有敌人。 |

### 新增卡牌

| 卡牌 | 角色 | 类型 | 稀有度 | 费用 | 效果（基础 / 升级） |
|------|------|:----:|:------:|:----:|------|
| **比试 [Sparring]** | 骨妹 | 攻击 | 罕见 | 2 | 消耗。玩家对单个敌人造成 **8** 点伤害，奥斯提造成 **7 / 9** 点伤害；实际造成非格挡伤害较少的一方回复 **4 / 6** 点生命。 |
| **猛撞 [Ram]** | 骨妹 | 攻击 | 普通 | 2 | 奥斯提失去 **6 / 5** 点生命，对所有敌人造成 **20 / 26** 点伤害；奥斯提生命不足时无法触发效果。 |
| **步步为营 [Step by Step]** | 猎人 | 技能 | 稀有 | X | 消耗。接下来 X（升级：X+1）回合，每回合多抽 1 张牌并多获得 1 点能量。升级后额外获得保留词条。 |
| **进化 [Evolve]** | 战士 | 能力 | 罕见 | 1 | 每当你抽到一张状态牌，抽 **1 / 2** 张牌。 |

### 怪物与 Boss

| 名称 | 编号 | 原版机制 | MOD 改后机制 |
| :--- | :--- | :--- | :--- |
| **永世沙漏 [Aeonglass]** | MON-01 | 永世沙漏 Boss 生成的凋零卡无法打出，直接进入消耗堆。 | 凋零卡可以**1c打出并消耗**，保留成长机制。 |

### 事件

| 事件名称 | 详情 |
| :--- | :--- |
| **旧日垃圾堆 [Trash Heap]** | 遗物奖励池加入**御守 [Omamori]**。 |
| **除虫者 [Bugslayer]** | 初始选项中**新增「离开」分支**，允许玩家直接走开而无须获取卡牌。 |
| **科学怪人 [Tinker Time]** | 初始选项中**新增「离开」分支**，允许玩家直接走开而不用强制接受突变卡。 |

### 一代事件回归

| 事件名称 | 先行条件 | 详情 |
| :--- | :--- | :--- |
| **老乞丐 [Old Beggar]** | 所有玩家金币 ≥ 75 | 给金币后切换为牧师删牌。 |
| **诅咒书本 [Cursed Tome]** | Act 2，且牌组无对应书籍遗物 | 可获得死灵之书、尼利的宝典、英雄宝典。 |
| **红面具 [Masked Bandits]** | Act 2，层数 ≥ 23，且无人持有红面具 | 可选择交金或与红面具三人帮战斗获取红面具。 |
| **J.A.X. [Augmenter]** | Act 2，且所有玩家牌组中可移除牌 ≥ 2 张 | 获得 J.A.X. 卡牌 / 变2张牌 / 获得突变之力遗物。 |
| **神圣泉水 [The Divine Fountain]** | 所有玩家牌组中存在可移除的诅咒牌 | 移除牌组中全部可移除诅咒（删除原版的伤害副作用）。 |
| **牧师 [Cleric]** | 所有玩家金币 ≥ 35 | 提供付钱选择治疗（25% 最大 HP）/ 删牌选项（75 金）。 |
| **心灵绽放 [Mind Bloom]** | Act 3 | 1. **战斗**——随机召唤第一幕的 Boss 进行决战，胜利获得 50 金 + 稀有遗物。<br>2. **升级**——升级牌组中所有可升级的牌，并获得「绽放印记」遗物。<br>3. **宝库**（层数 < 41，多人 < 38）获得 999 金 + 牌组加入 2 张「凡庸」。|
| **大转盘 [Wheel of Change]** | - | 自定义转盘小游戏，随机获得金/遗物/治疗/诅咒/删牌/受伤。 |
| **红面具大人之墓 [Tomb of Lord Red Mask]** | Act 3，且无人持有红面具 | 可献上全部金币获得红面具，或（持有红面具时）收获 222 金。 |


### 遗物

#### 新增遗物

| 遗物 | 类型 / 编号 | 描述 |
| :--- | :--- | :--- |
| **日晷 [Sundial]** | 商店 | 每将抽牌堆洗牌 3 次（跨战斗保留计数），获得 2 点能量。 |
| **橙色药丸 [Orange Pill]** | 商店 | 同一回合打出攻击 / 技能 / 能力各一张后，移除所有负面效果（女王的魂缚锁链除外）。 |
| **枯木树枝 [Dead Branch]** | 稀有 | 每消耗一张牌，随机将一张牌加入手牌（虚无牌触发时给当回合保留）。 |
| **御守 [Omamori]** | 事件 | 抵消接下来获得的 2 张诅咒牌（带计数器）。 |
| **宁静烟斗 [Peace Pipe]** | 稀有 | 在火堆新增"烟斗"选项，可删除一张牌。 |
| **微笑面具 [Smiling Mask]** | 普通 | 删牌价格固定为 50 金币。 |
| **咖啡杯 [Coffee Cup]** | 先古 | 无法在火堆休息，但每回合 +1 费用。 |
| **融合之锤 [Fusion Hammer]** | 先古 | 无法锻造，但每回合 +1 费用。 |
| **诅咒钥匙 [Curse Key]** | 先古 / RELIC-01 | 每回合 +1 费用，每次打开宝箱获得一张随机诅咒。**仅限单人模式出现，单人模式下可通过右下角的"跳过宝箱"按钮（ProceedButton）直接离开，同时规避诅咒。** |
| **矮人铁砧 [Dwarf Anvil]** | 商店 | 拾起时为 3 张牌附加"锻造"附魔，被附魔的牌费用永久 -1（最低 0 费）。 |
| **袖箭 [Wrist Blade]** | 罕见 | 猎人专属。费用为 0 的攻击牌额外造成 4 点伤害。 |
| **悬浮风筝 [Hovering Kite]** | 普通 | 猎人专属。你在每回合第一次弃牌时，获得 1 点能量。 |
| **灵魂契约 [Soul Contract]** | 先古 | 拾起时扣除最大生命上限 10%。选择牌组中的 1 张有消耗的牌，永久去除其消耗属性。 |


#### 原版调整

| 遗物 | 类型 | 原版 | MOD 改后 |
| :--- | :--- | :--- | :--- |
| **坚固钳子 [Sturdy Clamp]** | 稀有 | 保留 10 护甲 | 保留 **20 护甲** |
| **活雾 [Preserved Fog]** | 先古 | 删除 3 张牌 | 删除 **4 张牌** |
| **红面具 [Red Mask]** | 事件 | 在一般共享遗物池中 | 从一般共享遗物池**移除**，只通过红面具相关事件获得 |
| **历史课 [History Course]** | 事件 | 只重复上回合最后打出的攻击牌 | 回调为重复上回合最后打出的**攻击牌或技能牌** |
| **诺努佩佩的钻石王冠 [Nonupeipe's Diamond Diadem]** | 先古 | 战斗开始时获得 20 格挡，并在下回合开始时保留 | 回调为：一回合打出不超过 2 张牌时，受到的敌人伤害减半 |


---

## 图片资源处理

新增`卡牌/遗物/能力/事件/火堆选项`后，需要适配对应的图片素材，可以使用我们的脚本批量处理图像大小和名称，路径在 `image_gen`。

> [!warning]
>
> **前置要求**：需要安装 [uv](https://docs.astral.sh/uv/)（Python 包管理器）。

 ```bash
 # macOS / Linux
 curl -LsSf https://astral.sh/uv/install.sh | sh
 # Windows (PowerShell)
 powershell -c "irm https://astral.sh/uv/install.ps1 | iex"
 ```

首次使用在 `image_gen/` 目录下执行 `uv sync` 即可创建虚拟环境并安装依赖。

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
uv run cards --anchor top     # 顶部对齐
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

> [!NOTE]
>
> 还有其他更多的类型的图片处理脚本都放在我们的 `image_gen/pyproject.toml` 中，使用 `uv run <type>` 即可执行。

---

## 开发

### 技术栈

- **游戏**: Slay the Spire 2 (Godot 4 / .NET)
- **Mod 框架**: BaseLib 3.2.1+ (自定义模型注册与联机自定义消息兼容)
- **补丁引擎**: Harmony 2.x (运行时方法劫持)
- **语言**: C# 12


---

## 版本发布

项目配置了基于 GitHub Actions 的 CI/CD 自动发布。每次发布新版本只需以下步骤：

1. **更新版本号**：在 `Sts2BalanceMod.json` 中更新 `"version"` 字段。
2. **同步日志**：在 `CHANGELOG.md` 中添加对应版本的更新日志段落（以 `# vX.X.X` 为标题）。
3. **推送 Tag**：提交代码并打上对应版本号的 Git Tag，然后推送至远程仓库：

   ```bash
   git add .
   git commit -m "chore: release vX.X.X"
   git tag vX.X.X
   git push origin <当前分支> vX.X.X
   ```

当推送 Tag 后，GitHub Actions 流水线会自动触发，在云端拉取依赖、自动配置 Godot 4.5.1 Mono 编译器与导出模板进行构建，并将最终的 `.zip` 附件自动关联并上传至对应的 GitHub Release 页面中，无须本地手动执行任何打包或上传命令。

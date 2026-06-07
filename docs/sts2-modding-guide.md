# STS2 Mod 制作指南

> 本文档记录 Slay the Spire 2 的 Mod 制作全流程，基于 Godot 4.5(.NET) + C# + BaseLib + Harmony 的技术栈。

---

## 目录

1. [技术背景](#1-技术背景)
2. [环境准备](#2-环境准备)
3. [项目结构说明](#3-项目结构说明)
4. [编译与部署](#4-编译与部署)
5. [使用 Harmony 修改原版数据](#5-使用-harmony-修改原版数据)
6. [使用 BaseLib 新增卡牌](#6-使用-baselib-新增卡牌)
7. [新增事件](#7-新增事件)
8. [常见问题](#8-常见问题)
9. [参考资源](#9-参考资源)

---

## 1. 技术背景

STS2（杀戮尖塔2）基于 **Godot 4.5.1 C#** 引擎开发，使用 **.NET 9.0**。与一代的 Java/LibGDX 完全不同。

Mod 依赖的两个核心库：

| 库 | 作用 |
|----|------|
| **BaseLib** | 社区维护的 Mod 基础库，提供 `CustomCardModel`、`CustomRelicModel` 等基类 |
| **Harmony** | 运行时补丁工具，拦截/修改游戏原本的方法，不改源文件 |

**Mod 加载原理：**

```
C# 源码 → dotnet build → .dll + .pck 文件
                              ↓
                    放到游戏 mods/ 目录
                              ↓
                    启动器选 "Load with mods"
                              ↓
                    游戏加载 BaseLib → 加载你的 mod → Harmony 补丁生效
```

**两种修改方式的区别：**

| 方式 | 适用场景 | 特点 |
|------|---------|------|
| **Harmony Patch** | 修改原版数值、属性、逻辑 | 轻量，代码少，适合"改" |
| **BaseLib 自定义类** | 新增卡牌、遗物、事件 | 需要继承基类，适合"加" |

---

## 2. 环境准备

### 必需工具

| 工具 | 说明 | 下载 |
|-----|------|------|
| .NET SDK 9.0 | 编译 C# 代码 | https://dotnet.microsoft.com/download |
| Godot 4.5.1 .NET 版 | 编辑素材和导出 .pck | https://godotengine.org/download/archive/4.5.1/ |
| 任意 C# IDE | 写代码（VS Code / Rider / Visual Studio） | - |

### 验证安装

```bash
dotnet --version
# 输出应为 9.x.x

dotnet --list-sdks
# 应看到 9.0.x
```

---

## 3. 项目结构说明

### 建议目录结构

```
你的Mod名称/
├── YourMod/                      # 素材文件夹（图片、本地化文本）
│   ├── images/
│   │   ├── mod_image.png         # Mod 图标
│   │   ├── cards/                # 卡牌图片
│   │   ├── powers/               # 状态图标
│   │   └── relics/               # 遗物图标
│   └── localization/
│       └── eng/                  # 英文文本
│           ├── Cards.json
│           └── Relics.json
├── YourModCode/                  # C# 源码文件夹
│   ├── MainFile.cs               # Mod 入口
│   ├── Patches/                  # Harmony 补丁
│   │   └── ShopPatch.cs
│   ├── Cards/                    # 新增卡牌
│   │   └── DualWield.cs
│   ├── Relics/                   # 新增遗物
│   ├── Powers/                   # 新增状态
│   └── Events/                   # 新增事件
├── YourMod.csproj                # 项目文件
├── YourMod.json                  # Mod 清单文件
├── Directory.Build.props         # Godot 路径配置
├── Sts2PathDiscovery.props       # 游戏路径自动发现
├── project.godot                 # Godot 项目配置
├── export_presets.cfg            # 导出配置
├── .gitignore
└── README.md
```

### 关键文件说明

#### `YourMod.json` — Mod 清单

```json
{
  "id": "Sts2BalanceMod",
  "name": "Sts2 Balance Mod",
  "author": "YourName",
  "description": "平衡调整 Mod",
  "version": "v0.1.0",
  "has_pck": true,
  "has_dll": true,
  "dependencies": ["BaseLib"],
  "affects_gameplay": true
}
```

| 字段 | 说明 |
|------|------|
| `id` | mod 唯一标识，和文件夹名一致 |
| `dependencies` | 依赖的 mod，必须包含 BaseLib |
| `has_dll` | 是否包含代码 DLL |
| `has_pck` | 是否包含素材包（卡图、文本等） |
| `affects_gameplay` | 是否影响游戏玩法 |

#### `YourMod.csproj` — 项目文件

```xml
<Project Sdk="Godot.NET.Sdk/4.5.1" InitialTargets="CheckDependencyPaths">
  <Import Project=".\Sts2PathDiscovery.props" />

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
  </PropertyGroup>

  <ItemGroup Condition="Exists('$(Sts2DataDir)')">
    <Reference Include="0Harmony" HintPath="$(Sts2DataDir)\0Harmony.dll" Private="false" />
    <Reference Include="sts2" HintPath="$(Sts2DataDir)\sts2.dll" Private="false" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Alchyr.Sts2.BaseLib" Version="*" PrivateAssets="all" />
    <PackageReference Include="Krafs.Publicizer" Version="2.3.0" PrivateAssets="all" />
    <AdditionalFiles Include="YourMod/localization/**/*.json" />
  </ItemGroup>

  <!-- 排除素材文件夹（这些进 .pck 不是 .dll） -->
  <ItemGroup>
    <Compile Remove="YourMod/**" />
    <EmbeddedResource Remove="YourMod/**" />
  </ItemGroup>

  <ItemGroup>
    <None Include="YourMod.json" />
    <None Include="project.godot" />
    <None Include="YourMod/**" />
  </ItemGroup>
</Project>
```

#### `MainFile.cs` — Mod 入口

```csharp
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace YourMod.YourModCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "YourMod";

    public static readonly MegaCrit.Sts2.Core.Logging.Logger Logger =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        // 注册自定义 Godot 脚本（如果需要场景的话取消注释）
        // ScriptManagerBridge.LookupScriptsInAssembly(Assembly.GetExecutingAssembly());

        Harmony harmony = new(ModId);
        harmony.PatchAll();
    }
}
```

#### `Sts2PathDiscovery.props` — 游戏路径发现

该文件会自动通过 Steam 注册表找到游戏路径和 `sts2.dll` 的位置。如果游戏不在默认 Steam 库路径，可以在 `Directory.Build.props` 中手动指定：

```xml
<Project>
  <PropertyGroup>
    <!-- 如果自动发现没找到，取消注释并改成你的游戏路径 -->
    <!-- <Sts2Path>D:\Game\Godot\Slay the Spire 2</Sts2Path> -->
    <GodotPath>C:\Program Files\Godot_4.5.1\Godot_v4.5.1-stable_mono_win64.exe</GodotPath>
  </PropertyGroup>
</Project>
```

---

## 4. 编译与部署

### 编译

```bash
# 在项目根目录执行
dotnet build
```

成功后在 `bin/Debug/net9.0/` 下会生成：
- `YourMod.dll`（你的代码）
- `YourMod.pdb`（调试符号）

### 部署（自动拷贝）

编译时项目会自动将 `.dll` 和 `.json` 拷贝到游戏目录的 `mods/YourMod/` 下。

### 导出素材包

如果需要图片、本地化文本等素材：

```bash
# 需要先安装 Godot 并设置 GodotPath
dotnet publish
```

这会调用 Godot 导出 .pck 文件到 mods 目录。

### 手动部署

如果自动拷贝没生效，手动复制：

```
游戏目录/mods/Sts2BalanceMod/
├── Sts2BalanceMod.dll
├── Sts2BalanceMod.pdb
├── Sts2BalanceMod.json
└── Sts2BalanceMod.pck （如果有素材）
```

### 启动游戏

1. Steam 中启动 STS2
2. 选择 **"Load with mods"** 选项
3. 在游戏内 **Settings → Mod Settings** 中启用你的 mod

---

## 5. 使用 Harmony 修改原版数据

### 基本模式

```csharp
using HarmonyLib;

namespace YourMod.YourModCode.Patches;

/// <summary>
/// 示例：修改 V6+ 删牌价格为固定 25 金币
/// </summary>
[HarmonyPatch(typeof(ShopScreen), nameof(ShopScreen.GetRemoveCardCost))]
public static class ShopRemoveCardCostPatch
{
    public static bool Prefix(ref int __result)
    {
        // 如果启用了 Ascension 6+，返回固定 25
        if (RunManager.Current?.AscensionLevel >= 6)
        {
            __result = 25;
            return false;  // 跳过原方法
        }
        return true;  // 原逻辑继续
    }
}
```

### 常用补丁模式

| 模式 | 用法 |
|------|------|
| **Prefix** | 在原始方法前执行，可修改参数或跳过原方法（返回 false） |
| **Postfix** | 在原始方法后执行，可修改返回值 |
| **Transpiler** | 修改原始方法的 IL 代码（高级） |
| **Finalizer** | 异常处理 |

### 修改卡牌属性示例

```csharp
/// <summary>
/// 批量修改卡牌属性
/// </summary>
[HarmonyPatch]
public static class CardDataPatches
{
    // 在卡牌数据初始化后修改
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CardLibrary), nameof(CardLibrary.RegisterAllCards))]
    public static void ModifyCards()
    {
        // 获取卡牌 "SkeletonLament"（骨妹挽歌）并移除消耗
        var card = CardLibrary.GetCard("SkeletonLament");
        if (card != null)
        {
            card.Exhaust = false;
            Logger.LogInfo("骨妹挽歌已改为不消耗");
        }
    }
}
```

---

## 6. 使用 BaseLib 新增卡牌

### 创建自定义卡牌

```csharp
using MegaCrit.Sts2.Cards;
using MegaCrit.Sts2.Core;

namespace YourMod.YourModCode.Cards;

/// <summary>
/// 示例：双持（Dual Wield）
/// 1费 技能牌 | 选择一张手牌，复制一张到抽牌堆
/// 升级后：复制 2 张
/// </summary>
public class DualWieldCard : CustomCardModel
{
    public override string Id => "DualWield";
    public override int Cost => 1;
    public override CardRarity Rarity => CardRarity.Uncommon;
    public override CardType Type => CardType.Skill;
    public override CardColor Color => CardColor.Red;  // 战士

    public override string GetLocalizedString(CardLocVariant variant, string language)
    {
        if (language == "eng")
        {
            return variant == CardLocVariant.Default
                ? "Dual Wield"
                : "Dual Wield+";
        }
        return variant == CardLocVariant.Default ? "双持" : "双持+";
    }

    public override string GetLocalizedDescription(CardLocVariant variant, string language)
    {
        bool upgraded = variant == CardLocVariant.Upgraded;
        int copies = upgraded ? 2 : 1;
        
        if (language == "eng")
        {
            return $"Choose a card. Add {copies} cop{(copies > 1 ? "ies" : "y")} of it to your draw pile.";
        }
        return $"选择一张牌。将 {copies} 张复制加入抽牌堆。";
    }

    protected override bool OnPlay(CardActionContext ctx)
    {
        // 实现卡牌效果：选择手牌中一张，复制到抽牌堆
        // 具体实现参考 BaseLib API
        return base.OnPlay(ctx);
    }
}
```

### 注册新卡牌

在 `MainFile.cs` 或其他初始化位置注册：

```csharp
// 在 Initialize() 中
CardLibrary.RegisterCard<DualWieldCard>();
```

---

## 7. 新增事件

### 使用 Harmony 添加事件

```csharp
/// <summary>
/// 在特定章节的事件池中添加新事件
/// </summary>
[HarmonyPatch]
public static class EventPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(EventManager), nameof(EventManager.Initialize))]
    public static void AddCustomEvents()
    {
        // 在 Act 1 事件池中添加牧师删牌事件
        EventManager.RegisterEvent("ClericRemove", new ClericRemoveEvent());
    }
}
```

---

## 8. 常见问题

### Mod 不加载

- 确认 `Sts2BalanceMod.json` 与文件夹名一致
- 确认依赖了 BaseLib 且 BaseLib 已安装
- 查看游戏 `Player.log` 的报错信息

### 编译错误

- 确认 .NET 9.0 SDK 已安装
- 确认 NuGet 源能访问（BaseLib 包在 nuget.org 上）
- 确认 `Sts2DataDir` 路径能找到 `sts2.dll`

### 游戏更新后 Mod 失效

- 等 BaseLib 更新（通常 1 天内）
- 等你的 mod 更新适配新版本
- 不要用 beta 分支玩 mod

### "Badlogic" 错误

- BaseLib 缺失或版本不匹配
- 重新安装 BaseLib

---

## 9. 参考资源

- **BaseLib Wiki**: https://github.com/Alchyr/ModTemplate-StS2/wiki
- **ModTemplate**: https://github.com/Alchyr/ModTemplate-StS2
- **Modding MCP (AI 辅助)**: https://github.com/elliotttate/sts2-modding-mcp
- **社区教程**: https://github.com/Cany0udance/EarlyStS2ModdingGuides/wiki

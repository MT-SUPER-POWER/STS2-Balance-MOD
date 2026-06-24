# STS2-Balance-MOD

《杀戮尖塔 2》平衡调整 Mod（Godot 4.5.1 / C# 12 / .NET 9 / Harmony 2.x / BaseLib 3.3.0+）。

## Build & Verify

```powershell
dotnet build                              # 编译（Debug 自动拷贝 dll/json/pck 到游戏 mods/ 目录）
dotnet publish -c Release                 # 发布到 dist/Sts2BalanceMod/
./Hooks/release.ps1 -Version 0.0.X -Build # 仅打包（dotnet publish → zip）
```

**前置**：`Sts2PathDiscovery.props` 自动检测游戏路径；`Directory.Build.props` 需配置 `GodotPath`（PCK 导出需要 Godot 4.5.1 mono 命令行）。该文件在 `.gitignore` 中。

**PCK 导出**：由 `.csproj` 的 `GodotExportPckOnBuild` target 自动触发，仅资源有变动时重新导出。Release 构建如缺少 PCK 会报错。

**没有单元测试**，`tests/` 下是 PowerShell 集成测试脚本，直接执行。

## 关键路径

| 路径 | 用途 |
|------|------|
| `Sts2BalanceModCode/MainFile.cs` | Mod 入口：`[ModInitializer]` → `Harmony.PatchAll()` |
| `Sts2BalanceModCode/Abstract/` | 基类：`Sts2CardModel`、`Sts2RelicModel`、`Sts2PowerModel`、`Sts2MonsterModel`、`Sts2EncounterModel` |
| `Sts2BalanceModCode/Patches/` | Harmony Patch（子目录：Cards/ / Relics/ / Powers/ / Orbs/ / Merchant/ / Events/ / CardPools/ / Encounters/ / Monsters/） |
| `Sts2BalanceModCode/Cards/` | 新增卡牌 |
| `Sts2BalanceModCode/Relics/` | 新增遗物 |
| `Sts2BalanceModCode/Powers/` | 新增能力 |
| `Sts2BalanceModCode/Monsters/` | 新增怪物 |
| `Sts2BalanceModCode/Encounters/` | 新增遭遇 |
| `Sts2BalanceModCode/Events/` | 新增事件 |
| `Sts2BalanceModCode/RestSite/` | 火堆选项 |
| `Sts2BalanceMod/localization/{eng,zhs,ita}/` | 本地化 JSON（cards.json / powers.json / relics.json 等） |
| `Sts2BalanceMod/images/` | 图片资源（card_portraits/ / powers/ / relics/ / events/ / ui/） |
| `image_gen/` | Python 图片批处理脚本（需 `uv`） |

## 图片生成（需 [uv](https://docs.astral.sh/uv/)）

```bash
cd image_gen && uv sync && cd ..
uv run cards death_reap.png                   # 卡牌立绘（1000×760 + 500×380）
uv run relics Sundial.png                     # 遗物图标（256×256 + 94×94 + 轮廓图）
uv run powers                                 # 能力图标
uv run rest-site-options smoke.png            # 火堆选项图标（256×169）
```

图片路径自动解析：文件名 `{id小写}.png`，基类自动拼接完整路径。

## 发布流程

1. 更新 `CHANGELOG.md`（`# vX.X.X` 格式，推送 Tag 后 Actions 自动创建 Release）
2. `git tag vX.X.X && git push origin main vX.X.X`
3. `.\Hooks\release.ps1 -Version X.X.X -Build -Upload`（本机打包 + 上传 zip）

`Sts2BalanceMod.json` 中的 `version` 需同步更新（运行 `release.ps1 -Version X.X.X -UpdateJson`）。

## Harmony Patch 约束

- 优先 Postfix > Prefix > Transpiler
- Patch 目标方法先用反编译源码验证（`D:\Game\Godot\StS2-Code`）
- 每个 Patch 必须注明目标类型、方法、修改原因和依赖反编译细节的警告
- **不要直接修改反编译源码**

## Git 提交

```text
<type>(<scope>): <subject>   # 50 字内
```

常用 type/scope：`feat(card)` `fix(relic)` `refactor(patch)` `chore(infra)` `docs(docs)`。

每次提交前：`git diff` / `git status`，只提交任务相关文件，同步更新 `CHANGELOG.md`。

## 调试

- 日志：`%AppData%/SlayTheSpire2/logs/godot.log`
- 查阅日志重点：Mod 加载、Harmony Patch 应用、类型/方法找不到、资源路径错误、C# 异常堆栈

## 约束

- **子模块只读**（`docs/references/WatcherMod`、`docs/references/ActsFromThePast`），不要提交改动
- `.gitignore` 中的目录（`bin/`、`dist/`、`.godot/`、`*.uid`、`*.import`）不要提交
- 修改现有卡牌/遗物/能力/怪物 → 用 Harmony Patch，不要新建替代品（除非明确要求新增）
- 注释标签：`TODO` `FIXME` `WARNING` `NOTE` `BUG` 等用于标记非显而易见的决策

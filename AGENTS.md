# STS2-Balance-MOD

《杀戮尖塔 2》平衡调整 Mod（Godot 4.5.1 / C# 12 / .NET 9 / Harmony 2.x / BaseLib 3.3.0+）。

<!-- BUILD_START -->
## Build & Verify

```powershell
dotnet build                              # 编译（Debug 自动拷贝 dll/json/pck 到游戏 mods/ 目录）
dotnet publish -c Release                 # 发布到 dist/Sts2BalanceMod/
./Hooks/release.ps1 -Version 0.0.X -Build # 仅打包（dotnet publish → zip）
```

**前置**：`Sts2PathDiscovery.props` 自动检测游戏路径；`Directory.Build.props` 需配置 `GodotPath`（PCK 导出需要 Godot 4.5.1 mono 命令行）。该文件在 `.gitignore` 中。

**PCK 导出**：由 `.csproj` 的 `GodotExportPckOnBuild` target 自动触发，仅资源有变动时重新导出。Release 构建如缺少 PCK 会报错。

**没有单元测试**，因为是三方 mod 的缘故，没办法做 godot 测试
- 只能够输出检查 `godot.log` 日志文件的内容，具体日志位置在[调试](AGENTS.md#调试)中有说明。
- 或者使用 `donet build` 检查报错信息。
<!-- BUILD_END -->


<!-- DOCUMENTATION_RULE_START -->
## 文档同步约定（重要！！！）

每当你完成我发布给你的任务，记得更新对应的文档。

1. 在 `docs/` 文件夹里有一个 `balance-changes.md` 的文件，里面记录了所有我要求做的调整项。你务必确保这个文件里的我分配给你的任务完成后，更新这个文件里对应任务的状态。
2. 每一个完成的改动必须同步更新 **`CHANGELOG.md`** 和 **`README.md`**：
  - 在 `CHANGELOG.md` 文件中记录你的任务，尽可能明细分环节，你可以参考别的记录模式，至少保证清晰可读。任何图的绘制使用 `mermaid`，文字总结规律一致的内容使用 `table`。
  - 在 `README.md` 的「调整内容」章节中同步更新对应的表格（例如卡牌调整、新增遗物等），确保玩家可见的说明文档与代码完全一致。

<!-- DOCUMENTATION_RULE_END -->

<!-- PATHS_START -->
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
<!-- PATHS_END -->

<!-- IMAGE_GEN_START -->
## 图片生成（需 [uv](https://docs.astral.sh/uv/)）

```bash
cd image_gen && uv sync && cd ..
uv run cards death_reap.png                   # 卡牌立绘（1000×760 + 500×380）
uv run relics Sundial.png                     # 遗物图标（256×256 + 94×94 + 轮廓图）
uv run powers                                 # 能力图标
uv run rest-site-options smoke.png            # 火堆选项图标（256×169）
```

图片路径自动解析：文件名 `{id小写}.png`，基类自动拼接完整路径。

> [!warning]
> **!必须做的检查!**
>
如果你使用了 `image_gen` 里面生成了任意相关的图片之后，做一个检查：
1. 检查一下 `Sts2BalanceMod/images/` 下面是否有有对应的生成
2. 用于 xxx 资源的图片名是否按照对应的 `xxx.cs` 文件的需求，负责图片无法使用


<!-- IMAGE_GEN_END -->

<!-- RELEASE_START -->
## 发布流程

1. 更新 `CHANGELOG.md`（`# vX.X.X` 格式，推送 Tag 后 Actions 自动创建 Release）
2. `git tag vX.X.X && git push origin main vX.X.X`
3. `.\Hooks\release.ps1 -Version X.X.X -Build -Upload`（本机打包 + 上传 zip）

`Sts2BalanceMod.json` 中的 `version` 需同步更新（运行 `release.ps1 -Version X.X.X -UpdateJson`）。
<!-- RELEASE_END -->

<!-- HARMONY_PATCH_START -->
## Harmony Patch 约束

- 优先 Postfix > Prefix > Transpiler
- Patch 目标方法先用反编译源码验证（`D:\Game\Godot\StS2-Code`）
- 每个 Patch 必须注明目标类型、方法、修改原因和依赖反编译细节的警告
- **不要直接修改反编译源码**
<!-- HARMONY_PATCH_END -->

<!-- GIT_COMMIT_START -->
## Git 提交

提交信息采用结构化的 Commit Message 格式。首行是一句话概括本次提交总体做了什么，空一行后列出具体的细节，每个细节需要加前缀修饰符。

格式模板：
```text
<type>(<scope>): <一句话概括这次提交总体做了什么，主旨清晰且精炼>

- [<action>] <细节描述 1>
- [<action>] <细节描述 2>
```

常用 type/scope：`feat(card)` `fix(relic)` `refactor(patch)` `chore(infra)` `docs(docs)`。

> [!note]
> 如果你真的可以一句话概括，那么二级的 `-[<action>]` 其实没有那么重要

细节前缀修饰符 `<action>` 推荐：
- `[Add]`: 新增功能/资源/卡牌/遗物/代码等
- `[Fix]`: 修复缺陷/Bug/逻辑问题/编译错误等
- `[Refactor]`: 重构、代码优化（不改变外部行为）
- `[Chore]`: 构建脚本、配置、依赖更新等杂项
- `[Docs]`: 补充或修改文档、注释说明等

每次提交前：`git diff` / `git status`，只提交任务相关文件，检查 `CHANGELOG.md` 是否记录的更新，如没有同步更新。
<!-- GIT_COMMIT_END -->

<!-- DEBUG_START -->
## 调试

- 日志：`%AppData%/SlayTheSpire2/logs/godot.log`
- 查阅日志重点：Mod 加载、Harmony Patch 应用、类型/方法找不到、资源路径错误、C# 异常堆栈
<!-- DEBUG_END -->

<!-- CONSTRAINTS_START -->
## 约束

- **子模块只读**（`docs/references/WatcherMod`、`docs/references/ActsFromThePast`），不要提交改动
- `.gitignore` 中的目录（`bin/`、`dist/`、`.godot/`、`*.uid`、`*.import`）不要提交
- 修改现有卡牌/遗物/能力/怪物 → 用 Harmony Patch，不要新建替代品（除非明确要求新增）
- 注释标签：`TODO` `FIXME` `WARNING` `NOTE` `BUG` 等用于标记非显而易见的决策
<!-- CONSTRAINTS_END -->

<!-- CODEGRAPH_START -->
## CodeGraph

In repositories indexed by CodeGraph (a `.codegraph/` directory exists at the repo root), reach for it BEFORE grep/find or reading files when you need to understand or locate code:

- **MCP tool** (when available): `codegraph_explore` answers most code questions in one call — the relevant symbols' verbatim source plus the call paths between them, including dynamic-dispatch hops grep can't follow. Name a file or symbol in the query to read its current line-numbered source. If it's listed but deferred, load it by name via tool search.
- **Shell** (always works): `codegraph explore "<symbol names or question>"` prints the same output.

If there is no `.codegraph/` directory, skip CodeGraph entirely — indexing is the user's decision.
<!-- CODEGRAPH_END -->

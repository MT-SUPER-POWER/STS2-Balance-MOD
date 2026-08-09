# STS2-Balance-MOD

《杀戮尖塔 2》平衡调整 Mod（Godot 4.5.1 / C# 12 / .NET 9 / Harmony 2.x / RitsuLib 最新稳定版）。

<!-- BUILD_START -->
## Build & Verify

```powershell
dotnet build                              # 编译（Debug 自动拷贝 dll/json/pck 到游戏 mods/ 目录，同时自动从本地游戏 DLL 更新 libs/ 目录下的 API Stub）
dotnet publish -c Release                 # 本地发布编译到 dist/Sts2BalanceMod/（仅限本地发布验证，正式发布由 CI/CD 自动进行）
```

**前置**：`Sts2PathDiscovery.props` 自动检测游戏路径；`Directory.Build.props` 需配置 `GodotPath`（PCK 导出需要 Godot 4.5.1 mono 命令行）。该文件在 `.gitignore` 中。

**PCK 导出**：由 `.csproj` 的 `GodotExportPckOnBuild` target 自动触发，仅资源有变动时重新导出。Release 构建如缺少 PCK 会报错。

**API Stub 自动同步**：由 `.csproj` 的 `UpdateStubDlls` target 自动触发。本地开发编译时，若本地游戏 DLL 存在，会自动使用 `refasmer` 重新剥离 API Stub DLL 输出到 `libs/`，确保提交后 CI/CD 能永远获得匹配的 API。

**没有单元测试**，因为是三方 mod 的缘故，没办法做 godot测试
- 只能够输出检查 `godot.log` 日志文件的内容，具体日志位置在[调试](AGENTS.md#调试)中有说明。
- 或者使用 `dotnet build` 检查报错信息。
<!-- BUILD_END -->


<!-- DOCUMENTATION_RULE_START -->
## 文档同步约定与知识库（重要！！！）

本项目以 **[docs/README.md](docs/README.md)** 作为中央**知识库总索引（Knowledge Base Hub）**。在进行任何架构设计、事件/能力/遗物改动或查阅参考规范时，优先阅读 `docs/README.md`。

每当你完成我发布给你的任务，记得更新对应的文档：
1. **知识库总索引**：若新增或重构了独立文档（如 `docs/powers.md` / `docs/events.md` 等），务必在 **[docs/README.md](docs/README.md)** 中登记索引。
2. **需求清单**：在 `docs/balance-changes.md` 中更新对应任务的完成状态。
3. **变更与说明**：
  - 在 `CHANGELOG.md` 中记录改动明细（图表使用 `mermaid`，文字表格使用 `table`）。
  - 在 `README.md` 的「调整内容」章节中同步更新对应的表格与跳转链接。

<!-- DOCUMENTATION_RULE_END -->

<!-- PATHS_START -->
## 关键路径

| 路径 | 用途 |
|------|------|
| `docs/README.md` | **知识库总索引（Knowledge Base Hub）** |
| `docs/powers.md` | 能力与效果手册（Buff / Debuff / Boss 机制） |
| `docs/events.md` | 事件与遭遇手册（原版调整 / 1 代回归事件） |
| `Sts2BalanceModCode/BalanceModEntry.cs` | Mod 入口：注册 RitsuLib 程序集、设置与 Harmony Patch |
| `Sts2BalanceModCode/Abstract/` | 共享模板：`BalanceCardTemplate`、`BalanceRelicTemplate`、`BalancePowerTemplate`、`BalanceMonsterTemplate`、`BalanceEncounterTemplate` |
| `Sts2BalanceModCode/Patches/` | Harmony Patch（子目录：Cards/ / Relics/ / Powers/ / Orbs/ / Merchant/ / Events/ / CardPools/ / Encounters/ / Monsters/） |
| `Sts2BalanceModCode/{Cards,Relics,Powers,Monsters,Encounters,Events,RestSite,Enchantments}/` | 按内容类别组织的 RitsuLib 模型 |
| `Sts2BalanceModCode/Runtime/` | 运行时视觉、音频与战斗状态辅助代码 |
| `Sts2BalanceModCode/{Extensions,Settings}/` | 路径约定与玩家可编辑设置 |
| `Sts2BalanceMod/localization/{eng,zhs,ita,rus}/` | 本地化 JSON（cards.json / powers.json / relics.json 等） |
| `D:\Game\Sts2Code\localization/{eng,zhs,ita,rus}/` | 游戏原版本地化 JSON（查阅原版卡牌、遗物、能力、事件等的文本与 LocKey 规则） |
| `Sts2BalanceMod/images/` | 图片资源（card_portraits/ / powers/ / relics/ / events/ / ui/） |
| `image_gen/` | Python 图片批处理脚本（需 `uv`） |
<!-- PATHS_END -->

<!-- CHARACTER_NAMING_START -->
## 人物称呼

中文沟通中可使用下表的常用外号；外号、官方中文名和英文名均指向同一名可操作角色。涉及 C# 类型、卡池、资源 ID、本地化 key 或游戏 API 时，必须使用表中的英文标识符，不得将外号写入标识符。

| 常用外号 | 官方中文名 | 英文标识符 |
|------|------|------|
| 战士 | 铁甲战士 | `Ironclad` |
| 猎人 | 静默猎手 | `Silent` |
| 骨妹 | 死灵缚者 | `Necrobinder` |
| 机宝 | 故障机器人 | `Defect` |
| 储君 | 储君 | `Regent` |
| - | 先古之民 | `Ancients` |

- 面向玩家的文档和本地化优先使用官方中文名；首次出现时可附英文名以消除歧义。
- 新增可操作角色或约定新的外号时，必须在同一改动中更新此表。
<!-- CHARACTER_NAMING_END -->

<!-- TERMINOLOGY_START -->
## 游戏术语与约定

下表记录了项目中的通用游戏术语翻译约定。面向玩家的文档、代码注释及本地化中，必须统一使用表中的规范中文名。

| 英文名 / 概念 | 规范中文名 | 禁用/弃用旧称 | 备注 |
|------|------|------|------|
| `Focus` | 集中 | 聚焦 | 故障机器人 (`Defect`) 属性与能力词条 |
<!-- TERMINOLOGY_END -->

<!-- IMAGE_GEN_START -->
## 图片生成（需 [uv](https://docs.astral.sh/uv/)）

```bash
cd image_gen && uv sync && cd ..
uv run cards DeathReap.png                   # 普通卡牌立绘（1000×760 + 500×380）
uv run cards SorceryStrike.png --fullart     # 先古卡/满画幅立绘（606×852 + 303×426）
uv run relics Sundial.png                     # 遗物图标（256×256 + 94×94 + 轮廓图）
uv run powers                                 # 能力图标
uv run rest-site-options Smoke.png            # 火堆选项图标（256×169）
uv run events                                 # 事件背景图（3440×1616）
```

图片路径自动解析：文件名 `{PascalCase}.png`（大驼峰命名，参考 `D:\Github\sts2-arknights-mod`），基类自动拼接完整路径。遗物轮廓图统一存放于 `images/relics/outlines/{PascalCase}.png` 目录下。

> [!warning]
> **!必须做的检查!**
>
如果你使用了 `image_gen` 里面生成了任意相关的图片之后，做一个检查：
1. 检查一下 `Sts2BalanceMod/images/` 下面是否有对应的生成
2. 用于 xxx 资源的图片名是否按照对应的 `xxx.cs` 文件的需求，否则图片无法使用
3. 先古卡 (`CardRarity.Ancient`) 或满画幅卡牌切图时必须附加 `--fullart` / `--ancient` 参数，输出 `606×852` 大图与 `303×426` 小图以匹配先古卡满画幅底图要求。


<!-- IMAGE_GEN_END -->

<!-- RELEASE_START -->
## 发布流程

1. 更新 `Sts2BalanceMod.json` 中的 `version` 字段为目标版本号 `vX.X.X`。
2. 在 `CHANGELOG.md` 中以 `# vX.X.X` 的格式追加该版本的变更内容。
3. 提交改动并打上对应版本号的 Git Tag，然后推送至远程仓库：
   ```bash
   git add .
   git commit -m "chore: release vX.X.X"
   git tag vX.X.X
   git push origin <当前分支> vX.X.X
   ```
4. 推送后，GitHub Actions 会在云端自动拉取依赖、还原 API Stub DLL 编译、使用 Godot Mono Headless 导出 PCK，并最终将打包好的 ZIP 发布到 Release 页面中，无须本地手动打包上传。
<!-- RELEASE_END -->

<!-- HARMONY_PATCH_START -->
## Harmony Patch 约束

- 优先 Postfix > Prefix > Transpiler
- Patch 目标方法先用反编译源码验证（`D:\Game\Sts2Code`）
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

日志分为两个阶段：

### 编译阶段

- `dotnet build` — 仅检查**编译错误**（C# 语法、类型找不到等）
- 编译通过不代表运行时能正常工作

### 运行时阶段

- **必须运行游戏后才会生成日志**
- 路径：`%AppData%/SlayTheSpire2/logs/godot.log`
- 查阅重点：Mod 加载、Harmony Patch 应用、类型/方法找不到、资源路径错误、C# 异常堆栈
- 注意事项：
  - 如果 Mod 有改动，**必须关闭游戏**再重新启动，否则可能不会重新加载
  - 日志是**累积写入**的，每次游戏启动会追加而非覆盖<!-- DEBUG_END -->

<!-- CONSTRAINTS_START -->
## 约束

- **子模块只读**（`docs/references/WatcherMod`、`docs/references/ActsFromThePast`），不要提交改动
- `.gitignore` 中的目录（`bin/`、`dist/`、`.godot/`、`*.uid`、`*.import`）不要提交
- 修改现有卡牌/遗物/能力/怪物 → 用 Harmony Patch，不要新建替代品（除非明确要求新增）
- 注释标签：`TODO` `FIXME` `WARNING` `NOTE` `BUG` 等用于标记非显而易见的决策
<!-- CONSTRAINTS_END -->

<!-- CODEGRAPH_START -->
## CodeGraph

在支持或需分析的代码库中（如 Mod 项目或游戏反编译项目 `D:\Game\Sts2Code`），在阅读文件或 grep 之前优先使用 CodeGraph 了解及定位代码：

- **MCP 工具**（推荐）：`codegraph_explore` 可以在一次调用中回答代码结构、符号源码与调用链。查询游戏原版代码时务必显式指定 `projectPath: "D:\\Game\\Sts2Code"`。
- **Shell 命令行**：`codegraph explore "<symbol names or question>"`。
- **兜底策略（自动建立索引）**：如果目标代码库（如 `D:\Game\Sts2Code` 或新项目）未生成 `.codegraph/` 索引或索引丢失，**允许且应当**直接通过 shell 运行 `codegraph init` 或 `codegraph index` 自动建立索引，建立完成后即可正常使用 `codegraph_explore` 进行深入分析，严禁回退到使用脚本/反射解包 DLL。
<!-- CODEGRAPH_END -->

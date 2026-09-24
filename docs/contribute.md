# 如何给项目做贡献

先配置本机路径，再运行 `dotnet build`。建议使用 .NET 9 SDK 和 Godot **4.5.1 .NET / Mono 版**；普通 Godot 不包含 C# 支持，较新的版本导出的 PCK 也可能无法被游戏加载。

## 配置路径

编辑仓库根目录的 `Directory.Build.props`，只需按自己的安装位置修改这两项：

```xml
<Project>
  <PropertyGroup>
    <GodotPath>D:/Tools/Godot/Godot_v4.5.1-stable_mono_win64.exe</GodotPath>
    <Sts2Path>D:/SteamLibrary/steamapps/common/Slay the Spire 2</Sts2Path>
  </PropertyGroup>
</Project>
```

- `GodotPath`：Godot .NET 版的可执行文件，不是文件夹。
- `Sts2Path`：游戏根目录，不是 `mods/` 或 `data_sts2_*` 目录。
- `Sts2PathDiscovery.props` 根据系统推导游戏数据目录与 Mod 输出目录，一般不需要修改。当前工程没有导入 `local.props`，不要只创建它就以为配置已生效。
- 本机绝对路径不要混入贡献提交。临时覆盖也可用 `dotnet build -p:Sts2Path="游戏路径" -p:GodotPath="Godot可执行文件路径"`。

## DLL 从哪里来，为什么需要 libs

C# 编译需要游戏 API 的类型和方法声明：本地构建优先引用游戏 `data_sts2_*` 中的 `sts2.dll`，以及 Harmony 补丁库 `0Harmony.dll`。RitsuLib 由 NuGet 自动还原，不用自己从其他 Mod 拷贝。

`libs/` 保存供没有安装游戏的 CI 使用的 **API Stub DLL**，即剥离实现后的编译引用，不是可运行的游戏本体。构建脚本会调用 `refasmer` 从本机游戏 DLL 更新这些引用，使 CI 与当前游戏 API 对齐：

```powershell
dotnet tool install --global Refasmer
# 已安装时可使用 dotnet tool update --global Refasmer
dotnet build
```

游戏更新后，旧 DLL 可能导致类型或方法找不到。应从匹配版本的本地游戏重新生成 Stub，不要随意下载、混用别人的 DLL，也不要把完整游戏 DLL 当成 Stub 提交。

## 构建与调试

- `dotnet build`：编译、更新 Stub、导出 PCK，并将 Debug Mod 部署到游戏 `mods/`；RitsuLib 也由构建目标部署。
- `dotnet build -p:SkipGodotExport=true`：跳过资源导出，适合只检查 C#；不会验证 PCK 是否可用。
- 修改后关闭并重新启动游戏。编译成功不等于运行正常，Windows 日志在 `%AppData%/SlayTheSpire2/logs/godot.log`。
- 处理图片时才需要安装 `uv`，在 `image_gen/` 运行 `uv sync`。用到参考子模块时运行 `git submodule update --init --recursive`，子模块仅作只读参考。

## 目录速览

| 目录 | 内容 |
| --- | --- |
| `Sts2BalanceModCode/` | C# 源码：`Cards/`、`Relics/`、`Events/` 等放新增内容，`Patches/` 修改原版行为 |
| `Sts2BalanceModCode/Abstract/` | 内容共享模板 |
| `Sts2BalanceModCode/Settings/`、`Runtime/` | 设置与运行时辅助代码 |
| `Sts2BalanceMod/` | 打包进游戏的图片、本地化与其他资源 |
| `Assets/` | 文档预览及参考资源 |
| `libs/` | CI 编译使用的 API Stub DLL |
| `image_gen/` | 图片批处理脚本与母版 |
| `docs/` | 需求、玩法说明及开发参考；入口是 [知识库索引](README.md) |

改动后补充对应需求、README 和 CHANGELOG。不要提交构建缓存、个人路径或无关文件；更多约定见根目录 `AGENTS.md`。

# WorkshopPublisher (创意工坊发布工具)

专为本项目打造的 Steam 创意工坊发布与多语言同步工具，彻底替代繁琐且黑盒的官方 `ModUploader`。

---

## ✨ 核心特性

- **自动双语同步**：一条命令自动将中文写入 Steam 工坊的「**简体中文** (`schinese`)」槽位，无需再去网页后台手动复制粘贴。
- **提前拦截超长**：在提交前自动计算 UTF-8 字节，超过 Steam 8000 字节上限直接提示，避免盲目报参数错误。
- **支持解耦发布**：可以只传代码产物不碰网页介绍，或者只改网页文字不重复上传 76MB 大文件。

---

## 常用操作命令

在项目根目录下按需选择对应的命令运行（确保 Steam 客户端已在后台登录）：

```powershell
# 1. 完整发布操作（大版本发版：上传代码包 + 同步更新中英文主页介绍）
dotnet run --project tools/WorkshopPublisher -c Release

# 2. 日常秒传代码操作（最推荐：仅传新代码包和版本链接，不改动已排好的主页介绍）
dotnet run --project tools/WorkshopPublisher -c Release -- -c

# 3. 仅改网页介绍（秒级生效：只更新工坊网页文案和标签，不重复上传 76MB 大文件）
dotnet run --project tools/WorkshopPublisher -c Release -- -m

# 4. 发布前演练检查（Dry Run：仅预检参数与中英文字符上限，不向 Steam 真正提交）
dotnet run --project tools/WorkshopPublisher -c Release -- --dry-run
```

---

## ⚠️ 注意事项

1. **Steam 客户端运行**：本工具调用原生 Steamworks API，执行前请确保电脑上的 Steam 客户端处于登录运行状态。
2. **描述字符上限**：Steam 限制主页描述单语言不能超过 **8000 字节**（中文字符占 3 字节，建议总汉字控制在 2500 字以内）。

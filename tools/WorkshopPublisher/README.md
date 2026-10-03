# WorkshopPublisher (创意工坊极速发布工具)

专为本项目打造的 Steam 创意工坊发布工具，彻底替代繁琐的官方 `ModUploader`。

---

## ✨ 核心好处

1. **自动双语同步**：一条命令自动把中文写入工坊的「简体中文」槽位，**不用再去网页后台手动复制粘贴**。
2. **提前拦截超长**：自动计算 UTF-8 字节，超过 8000 字节直接报出数字，不再报莫名其妙的错误代码。
3. **支持分开更新**：可以只传代码不改网页，或者只改网页文字不重传 76MB 的大文件。

---

## 🚀 常用命令

在项目根目录下直接运行：

### 1. 完整发布（代码 + 双语介绍）

上传新版本代码，并同步更新 Steam 工坊的中文和英文主页：

```powershell
dotnet run --project tools/WorkshopPublisher -c Release
```

### 2. 日常秒传：只传代码（推荐）

如果你不想动工坊主页，只想把新版本代码发上去并带上 Release 链接：

```powershell
dotnet run --project tools/WorkshopPublisher -c Release -- -c
```

### 3. 只改工坊主页介绍（秒级完成）

改了 `WORKSHOP_DESCRIPTION.md` 文案，只想更新网页文字，不重新上传 76MB 大文件：

```powershell
dotnet run --project tools/WorkshopPublisher -c Release -- -m
```

### 4. 检查参数（不真正上传）

发布前看一眼字数和配置对不对：

```powershell
dotnet run --project tools/WorkshopPublisher -c Release -- --dry-run
```

---

## ⚙️ 常用参数说明

- `-c`（`--content-only`）：只传代码文件，不碰主页文字。
- `-m`（`--meta-only`）：只改主页文字/标签/封面，不传代码。
- `--lang schinese`：只更新简体中文。
- `--dry-run`：仅演练检查，不提交。

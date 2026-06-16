# docs 目录索引

- [更新日志](../CHANGELOG.md) — 各版本已实现功能与修复记录（`# vx.x.x` 格式，推送 Tag 时自动用于 Release 说明）
- [平衡调整需求清单](balance-changes.md) — 所有需求项的编号、描述和状态
- [STS1 内容回归盘点清单](sts1-content-inventory.md) — 基于 ActsFromThePast 整理的一代内容候选池与合并优先级
- [STS2 Mod 制作指南](sts2-modding-guide.md) — 从零开始制作 STS2 Mod 的完整教程
- [WatcherMod 参考代码](references/WatcherMod/) — 社区参考 Mod（[GitHub](https://github.com/lamali292/WatcherMod) submodule）
- [ActsFromThePast 参考代码](references/ActsFromThePast/) — STS1 三幕与事件回归参考 Mod（[GitHub](https://github.com/Cany0udance/ActsFromThePast) submodule）

## 工具脚本

- [发布脚本](../Hooks/release.ps1) — 本机构建打包并上传 Release 附件（说明由 Actions 从 CHANGELOG 写入）
- [提取更新说明](../Hooks/extract-changelog.ps1) — 从 CHANGELOG.md 读取指定版本段落
- [仅打包](../Hooks/package-release.ps1) — `dotnet publish` → `dist/Sts2BalanceMod/` → `dist/*.zip`
- [卡牌立绘切图](../image_gen/cards.py) — 将原始素材裁切为 1000×760 / 500×380，输出到 `Sts2BalanceMod/images/card_portraits/`
- [遗物图标切图](../image_gen/relics.py) — 将原始素材裁切为 256×256 / 94×94，轮廓图 94×94，输出到 `Sts2BalanceMod/images/relics/`
- [火堆选项图标切图](../image_gen/rest_site_options.py) — 将原始素材裁切为 256×169，输出到 `Sts2BalanceMod/images/ui/rest_site/`


## 文件夹修改规则

1. Patch 这个里面的修改都是对当前原有内容的调整
2. 除开 `Patch` 的文件夹就是对应着新增内容

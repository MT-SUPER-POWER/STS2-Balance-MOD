# docs 目录索引

- [更新日志](../CHANGELOG.md) — 各版本已实现功能与修复记录（`# vx.x.x` 格式，推送 Tag 时自动用于 Release 说明）
- [平衡调整需求清单](balance-changes.md) — 所有需求项的编号、描述和状态
- [STS2 Mod 制作指南](sts2-modding-guide.md) — 从零开始制作 STS2 Mod 的完整教程
- [WatcherMod 参考代码](references/WatcherMod/) — 社区参考 Mod（[GitHub](https://github.com/lamali292/WatcherMod) submodule）

## 工具脚本

- [发布打包](../Hooks/package-release.ps1) — `dotnet publish` 输出到 `build/Sts2BalanceMod/`，再打包为 `dist/Sts2BalanceMod-vX.X.X.zip`
- [卡牌立绘切图](../image_gen/cards.py) — 将原始素材裁切为 1000×760 / 500×380，输出到 `Sts2BalanceMod/images/card_portraits/`
- [遗物图标切图](../image_gen/relics.py) — 将原始素材裁切为 256×256 / 94×94，轮廓图 94×94，输出到 `Sts2BalanceMod/images/relics/`


## 文件夹修改规则

1. Patch 这个里面的修改都是对当前原有内容的调整
2. 除开 `Patch` 的文件夹就是对应着新增内容

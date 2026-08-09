# Harmony 补丁目录

补丁按**修改对象**分子目录，命名空间与目录一致（如 `Patches.Cards`）。
`BalanceModEntry.Initialize()` 中的 `Harmony.PatchAll()` 会扫描整个程序集，无需手动注册。

## 目录说明

| 目录 | 用途 | 示例 |
|------|------|------|
| `Cards/` | 单张卡牌的属性、关键词、打出逻辑 | 刀舞去消耗、幽魂形态 rework |
| `CardPools/` | 角色卡池的增删 | 移除吞噬暗影、袖里乾坤 |
| `Powers/` | 原版 Power 的行为修改 | 认知偏差聚焦归零后移除 |
| `Orbs/` | 球体被动/激发逻辑 | 电动力学闪电球 AOE |
| `Merchant/` | 商店条目与删牌价格 | 删牌价格、微笑面具 Flash |
| `Relics/` | 原版遗物效果修改 | 坚固钳子保留护甲 |
| `Events/` | 事件奖励池修改 | 旧日垃圾堆加入御守 |

## 新增补丁

1. 根据上表选择对应子目录
2. 新建 `XxxPatch.cs`，命名空间设为 `Sts2BalanceMod.Sts2BalanceModCode.Patches.{子目录名}`
3. 在类上标注 `[HarmonyPatch(...)]`，编译后自动生效

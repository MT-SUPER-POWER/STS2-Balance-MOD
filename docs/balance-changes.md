# STS2 平衡调整需求清单

> 本文档追踪**未完成 / 待处理**的调整项，并保留最近完成任务的验收状态。
> 所有已实现调整的玩家说明请查看 [README.md](../README.md#调整内容) 的「调整内容」章节。

---

## 待办项


### 卡牌

### 遗物


### BOSS


### 怪物


---

## 最近完成

### 卡牌

- [x] **CARD-WELL-LAID-PLANS-01** — 静默猎手能力牌「计划妥当」（Well-Laid Plans）费用调整为始终 1 费
  - 类别: 静默猎手（绿卡）/ 能力牌 / 罕见（Uncommon）
  - 费用: 1 费（升级后保持 1 费）
  - 效果: 回合结束时保留最多 1（升级后 2）张牌。
  - 实现: 通过 Harmony Patch 拦截 `CardModel.CanonicalEnergyCost` 为 1，并在 `OnUpgrade` 中移除减费逻辑。


### 遗物

- [x] **RELIC-PANDORAS-BOX-01** — 先古遗物「潘多拉的魔盒」（Pandora's Box）变牌后增加异步确认环节，允许 SL 重新选择
  - 类别: 先古遗物（Ancient Relic）
  - 目前: 玩家选择「潘多拉的魔盒」时，`PandorasBox.AfterObtained` 在弹出变牌卡牌展示界面后未等待玩家确认即瞬间结束，随即触发事件 `Done()` 并自动存盘落盘，玩家无法通过 SL 重新做出遗物选择。
  - 目标: 参考「玻璃的眼珠」（Glass Eye）的异步挂起模式，在弹出变牌展示界面（`NSimpleCardsViewScreen`）后异步等待玩家点击「确认」按钮；在玩家确认之前，不结束 `AfterObtained` 且不触发事件结束与存盘；若玩家在此期间通过 SL 重新载入游戏，将回到选择遗物前的初始状态。
  - 数值: 保持原版变牌规则与卡牌数量不变。
  - 实现: 通过 Harmony Patch 拦截 `PandorasBox.AfterObtained`，弹出变牌界面后使用 `TaskCompletionSource` 异步等待确认按钮，并兼容暂停菜单打开后重新展示。



### 卡牌



### 基础设施




### BOSS




### 遗物



---

## 现有的问题以及无法解决的问题


### BUG 列表

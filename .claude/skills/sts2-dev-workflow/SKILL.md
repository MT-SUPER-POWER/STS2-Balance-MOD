---
name: sts2-dev-workflow
description: Use when working on game content modifications for STS2 Balance MOD. Triggers on requests to modify cards, relics, powers, encounters, or any game content. Conducts requirement analysis first, then implements after confirmation.
---

# STS2 Dev Workflow

## Overview

A two-phase workflow for game content modifications:
1. **Phase 1: Requirement Analysis** — Interview user, write requirements to `docs/balance-changes.md`, get confirmation
2. **Phase 2: Implementation** — Execute confirmed tasks following project conventions

## When to Use

- User wants to modify game content (cards, relics, powers, encounters, etc.)
- User says "我想调整..." or "帮我改一下..."
- User provides a task ID like "CARD-01"
- Any request involving game balance changes

## Phase 1: Requirement Analysis

### Step 1: Interview

Ask clarifying questions **one at a time** to understand:

```
┌─────────────────────────────────────────────────────────────┐
│  Q1: What do you want to modify? (card/relic/power/...)     │
├─────────────────────────────────────────────────────────────┤
│  Q2: What's the current behavior?                           │
├─────────────────────────────────────────────────────────────┤
│  Q3: What's the desired behavior?                           │
├─────────────────────────────────────────────────────────────┤
│  Q4: Any specific values/numbers?                           │
├─────────────────────────────────────────────────────────────┤
│  Q5: Dependencies or constraints?                           │
└─────────────────────────────────────────────────────────────┘
```

### Step 2: Write Requirements

After gathering info, write to `docs/balance-changes.md`:

```markdown
## 待办项

### 卡牌

- [ ] **CARD-XX** — [简短标题]
  - 目前: [当前行为]
  - 目标: [期望行为]
  - 数值: [具体数值变化，如有]
  - 备注: [其他说明]
```

### Step 3: Confirm

Show the user what you wrote:

```markdown
## 需求确认

我已将以下需求写入 `docs/balance-changes.md`:

[展示写入的内容]

请确认是否准确反映你的需求？
- 确认 → 进入 Phase 2
- 需要修改 → 调整后重新确认
```

**Do NOT proceed to Phase 2 until user explicitly confirms.**

---

## Phase 2: Implementation

Only after user confirms the requirements document.

### Step 1: Analyze Code

Use CodeGraph to understand target code:

```bash
# Find relevant code
codegraph_explore "CardName" "PowerName" "RelicName"
```

Key locations:
| Type | Primary | Secondary |
|------|---------|-----------|
| Card | `Sts2BalanceModCode/Cards/` | `Sts2BalanceModCode/Patches/Cards/` |
| Relic | `Sts2BalanceModCode/Relics/` | `Sts2BalanceModCode/Patches/Relics/` |
| Power | `Sts2BalanceModCode/Powers/` | `Sts2BalanceModCode/Patches/Powers/` |
| Monster | `Sts2BalanceModCode/Monsters/` | `Sts2BalanceModCode/Patches/Monsters/` |
| Encounter | `Sts2BalanceModCode/Encounters/` | `Sts2BalanceModCode/Patches/Encounters/` |
| Event | `Sts2BalanceModCode/Events/` | `Sts2BalanceModCode/Patches/Events/` |
| Localization | `Sts2BalanceMod/localization/{eng,zhs,ita}/` | |
| Images | `Sts2BalanceMod/images/` | `image_gen/` |

### Step 2: Present Implementation Plan

```markdown
## Implementation Plan

### Task: [TASK-ID] — [Title]

### Affected Files
- `path/to/file1.cs` — [changes]
- `path/to/file2.json` — [changes]

### Approach
1. [Step 1]
2. [Step 2]

### Technical Details
- [Harmony patch type]
- [Target type and method]
```

### Step 3: Implement

Follow project conventions:

**Harmony Patches:**
```csharp
// Must include:
// - Target type and method
// - Modification reason
// - Warning about decompiled source

[HarmonyPatch(typeof(TargetType), "MethodName")]
public static class MyPatch
{
    // WHY this change
    private static void Postfix(ref ReturnType __result)
    {
        // Implementation
    }
}
```

**New Cards/Relics:**
- Inherit from `Sts2CardModel` / `Sts2RelicModel`
- Add localization in all three languages
- Generate images if needed

**Image Generation:**
```bash
cd image_gen && uv sync && cd ..
uv run cards filename.png
uv run relics filename.png
```

### Step 4: Document

**Mark task complete in `docs/balance-changes.md`:**
```markdown
- [x] **CARD-XX** — [Completed]
```

**Update `CHANGELOG.md`:**
```markdown
# [Unreleased]

### Added/Changed/Fixed
- [Type]: [Description] (TASK-ID)
```

**Update `README.md`:**
- Update "调整内容" table

### Step 5: Commit

```
<type>(<scope>): <summary>

- [<action>] <detail>
```

Types: `feat(card)`, `fix(relic)`, `refactor(patch)`, `chore(infra)`, `docs(docs)`

### Step 6: Sync CodeGraph

```bash
codegraph index
```

Or notify user to sync manually.

---

## Anti-Patterns

- **Don't** skip requirement confirmation
- **Don't** modify decompiled source directly
- **Don't** forget to update all three doc files
- **Don't** commit without checking git status

## Quick Reference

| What | Where |
|------|-------|
| Requirements | `docs/balance-changes.md` |
| Entry point | `Sts2BalanceModCode/MainFile.cs` |
| Abstract bases | `Sts2BalanceModCode/Abstract/` |
| Harmony patches | `Sts2BalanceModCode/Patches/` |
| Localization | `Sts2BalanceMod/localization/{eng,zhs,ita}/` |
| Images | `Sts2BalanceMod/images/` |
| Build | `dotnet build` |
| Logs | `%AppData%/SlayTheSpire2/logs/godot.log` |

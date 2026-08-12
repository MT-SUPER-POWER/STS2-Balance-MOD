---
name: sts2-dev-workflow
description: Use when working on game-content modifications for STS2 Balance MOD, including cards, relics, powers, monsters, encounters, events, card pools, merchants, and rest-site options. Conduct requirement analysis and obtain confirmation before implementation; then resolve the authoritative mod and game sources with CodeGraph before editing.
---

# STS2 Dev Workflow

Use this two-phase workflow: analyze and confirm the requirement first, then implement the confirmed task.

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

### Step 1: Resolve the Source of Truth

Do this **before** `rg`, directory scans, or reading C# files. Do not guess filenames from an English or localized game name. CodeGraph is the default tool for both locating **and reading** repository source.

1. Check that the repository root contains `.codegraph/`. CodeGraph automatically watches the workspace and updates the index on every file change, so no manual status check or sync command is required.
2. Select and combine CodeGraph operations based on the unanswered question; do **not** mechanically run every command or rely on one fixed prompt. Use the MCP equivalents when available, otherwise use these repository CLI commands:

| Need | CLI | Example |
|------|-----|---------|
| Orient to indexed paths | `codegraph files` | Identify the relevant `Cards/`, `Patches/`, or localization area without a filesystem scan |
| Find a known ID, type, or method | `codegraph query <search>` | `codegraph query "Electrodynamics"` |
| Read a known symbol or source file | `codegraph node <name>` | `codegraph node "GlowDrawCardPatch"` or `codegraph node "Sts2BalanceModCode/Patches/Cards/GlowDrawCardPatch.cs"` |
| Understand a behavior spanning multiple symbols | `codegraph explore <question>` | `codegraph explore "Trace the card-pool path for Electrodynamics, including registration, injected models, and every patch that can add or remove it."` |
| Find direct control-flow neighbors | `codegraph callers <symbol>` / `codegraph callees <symbol>` | `codegraph callers "Glow.OnPlay"` |
| Assess the modification blast radius | `codegraph impact <symbol>` | `codegraph impact "GlowDrawCardPatch.Prefix"` |
| Identify relevant tests after a code change | `codegraph affected <files...>` | `codegraph affected Sts2BalanceModCode/Patches/Cards/GlowDrawCardPatch.cs` |

   `codegraph node` and `codegraph explore` return current on-disk source with line numbers; treat that output as the source read. Do **not** re-read a file returned there with filesystem tools. Start with `query` or `files` only when the symbol or area is unknown, then use `node` or `explore` to read the relevant code. Use `callers`, `callees`, or `impact` when the implementation decision depends on relationships rather than text alone. Fall back to `rg` or direct reads only after CodeGraph cannot surface the needed repository source.
3. Determine which source governs the requested behavior. Use this map:

| Source | Location | Use it for | Editing rule |
|--------|----------|------------|--------------|
| Mod source | `Sts2BalanceModCode/` | This mod's new content, patches, abstractions, and extensions | Authoritative editable implementation |
| Vanilla game source | `D:\Game\Sts2Code\` | Exact target type, overload, control flow, private fields, and patch feasibility | Read-only decompiled reference; never modify |
| Reference mods | `docs/references/WatcherMod/`, `docs/references/ActsFromThePast/` | Read-only examples and compatibility research | Never edit or treat as the target implementation |
| Player resources | `Sts2BalanceMod/localization/{eng,zhs,ita,rus}/`, `Sts2BalanceMod/images/` | Text and artwork for new or player-visible content | Update only when the task requires them |

4. If CodeGraph finds no mod-side implementation, inspect the exact vanilla type and method in `D:\Game\Sts2Code\` before selecting a Harmony target. This external decompiled tree is not the repository's CodeGraph index; use a direct read there only after resolving the type with CodeGraph. Verify the fully qualified type, method overload, return type, relevant fields, and whether a Prefix/Postfix can safely express the change. Prefer Postfix, then Prefix, then Transpiler.

Record this source-resolution result in the implementation plan:

```markdown
### Source resolution
- Target behavior: `<Type>.<Method>(<signature>)`
- Vanilla evidence: `D:\Game\Sts2Code\...` — `<relevant control flow/field>`
- Existing mod evidence: `<repo path and symbol>` or `none found`
- Chosen seam: `<existing patch | new patch | custom model>` — `<why>`
```

### Step 2: Locate the Edit Surface

Use the CodeGraph result to select the narrowest matching location. These are starting points, not substitutes for source resolution:

| Change | New content | Existing-content patch |
|--------|-------------|------------------------|
| Card | `Sts2BalanceModCode/Cards/` | `Sts2BalanceModCode/Patches/Cards/` or `Patches/CardPools/` |
| Relic | `Sts2BalanceModCode/Relics/` | `Sts2BalanceModCode/Patches/Relics/` |
| Power or orb | `Sts2BalanceModCode/Powers/` | `Sts2BalanceModCode/Patches/Powers/` or `Patches/Orbs/` |
| Monster or encounter | `Sts2BalanceModCode/Monsters/`, `Encounters/` | `Sts2BalanceModCode/Patches/Monsters/`, `Patches/Encounters/` |
| Event, merchant, or rest site | `Events/`, `RestSite/` | `Sts2BalanceModCode/Patches/Events/`, `Patches/Merchant/` |

### Step 3: Present Implementation Plan

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
- [Decompiled-source finding that makes this patch safe]
```

### Step 4: Implement

Follow project conventions:

**Harmony Patches:**
```csharp
/// <summary>
/// TASK-ID — <change summary>.
/// Target: <fully qualified target type>.<method>.
/// Reason: <why this behavior changes>.
/// WARNING: Verified against D:\Game\Sts2Code\<file>.cs; game updates may change this decompiled implementation.
/// </summary>

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
- Add `eng`, `zhs`, `ita`, and `rus` localization for player-visible new content
- Generate matching images when the content needs artwork
- Verify the generated filename matches the model's ID-derived image path

**Image Generation:**
```bash
cd image_gen && uv sync && cd ..
uv run cards filename.png
uv run relics filename.png
```

### Step 5: Verify and Document

Run `dotnet build`. For behavior that requires runtime confirmation, restart the game and inspect the new portion of `%AppData%/SlayTheSpire2/logs/godot.log` for mod loading, Harmony target failures, missing resources, and exceptions.

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

### Step 6: Review and Commit

Before committing, inspect `git diff` and `git status`; stage only task-related files.

```
<type>(<scope>): <summary>

- [<action>] <detail>
```

Types: `feat(card)`, `fix(relic)`, `refactor(patch)`, `chore(infra)`, `docs(docs)`

### Step 7: CodeGraph Auto-Sync

CodeGraph automatically watches the repository and syncs file changes in the background as you edit, add, or delete files. No manual `codegraph sync` or `codegraph index` step is required.

---

## Anti-Patterns

- **Don't** skip requirement confirmation
- **Don't** start with `rg`, guessed paths, or direct source reads when `.codegraph/` exists
- **Don't** choose a Harmony target without checking its decompiled implementation in `D:\Game\Sts2Code\`
- **Don't** modify decompiled source directly
- **Don't** forget to update all three doc files
- **Don't** commit without checking git status

## Quick Reference

| What | Where |
|------|-------|
| Requirements | `docs/balance-changes.md` |
| Entry point | `Sts2BalanceModCode/BalanceModEntry.cs` |
| Abstract bases | `Sts2BalanceModCode/Abstract/` |
| Harmony patches | `Sts2BalanceModCode/Patches/` |
| Vanilla behavior reference | `D:\Game\Sts2Code\` |
| Localization | `Sts2BalanceMod/localization/{eng,zhs,ita,rus}/` |
| Images | `Sts2BalanceMod/images/` |
| Build | `dotnet build` |
| Logs | `%AppData%/SlayTheSpire2/logs/godot.log` |

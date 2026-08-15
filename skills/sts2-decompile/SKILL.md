---
name: sts2-decompile
description: Decompile STS2 game EXE when game version updates. Use GDRE Tools CLI to fully recover game code (GDScript + C# assemblies + resources), then update source code directory.
---

# STS2 Decompile Workflow

## Overview

When Slay the Spire 2 updates, we need to **fully decompile the game EXE** to keep our mod's reference source up to date. This includes extracting embedded PCK, decompiling C# assemblies, and recovering all resources.

GDRE Tools can directly process the EXE file — it extracts the embedded PCK and decompiles everything in one step.

## When to Use

- Game version updated (new patch/release)
- User says "游戏更新了，需要反编译" or "更新源码"
- Need to compare old vs new game code
- Need to extract specific resources from game

## Prerequisites

- GDRE Tools v2.6.0+ at `D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe`
- Game EXE located (typically at game install directory)
- Output directory: `D:\Game\Sts2Code` (git-tracked reference source)

## Workflow

### Recommended: One-Click Automated Decompile

Run the automated decompiler script from the repository root:

```powershell
python skills/sts2-decompile/scripts/decompile.py
```

This script automatically:
1. **Recovers all Godot assets and localization**: Runs GDRE Tools CLI on `SlayTheSpire2.exe` to decompile PCK resources, translations, shaders, and configs to `D:\Game\Sts2Code`.
2. **Cleans & Decompiles C# Assembly**: Uses `ilspycmd` on `sts2.dll` directly to generate 100% clean, conflict-free C# source files under `D:\Game\Sts2Code\src\`.
3. **Verifies Output**: Validates that all `.cs` and `.json` localization files are fully extracted.

#### Variations:
```powershell
# Only decompile C# assembly (faster when only checking code logic):
python skills/sts2-decompile/scripts/decompile.py --csharp-only

# Only recover PCK resources:
python skills/sts2-decompile/scripts/decompile.py --resources-only
```

### Step 6: Compare Changes

```powershell
# Check what changed
cd D:\Game\Sts2Code
git status
git diff --stat
```

### Step 7: Initialize / Verify CodeGraph Index

```bash
# If D:\Game\Sts2Code does not have a .codegraph/ index yet, run:
codegraph index
```

Once initialized, CodeGraph automatically watches and updates the index on file changes.

### Step 8: Review and Commit

```powershell
cd D:\Game\Sts2Code
git add .
git commit -m "chore: decompile STS2 v<VERSION>"
```

## Common Variations

```powershell
# Scripts only (faster, for code review)
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --recover="$exePath" `
    --output="D:\Game\Sts2Code" `
    --scripts-only

# Specific directory only
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --recover="$exePath" `
    --output="D:\Game\Sts2Code" `
    --include="res://scripts/**/*.gdc"

# Exclude certain files
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --recover="$exePath" `
    --output="D:\Game\Sts2Code" `
    --exclude="res://**/*.png"
```

## CLI Reference

See `skills/gdre-tools/guide/CLI.md` for full CLI options.

## Troubleshooting

| Issue | Solution |
|-------|----------|
| EXE not found | Check game install path, ask user |
| Version mismatch | Use `--force-bytecode-version=<version>` |
| Incomplete decompilation | Try `--ignore-checksum-errors` |
| Encrypted PCK | Use `--key=<64-char-hex>` |
| C# decompilation fails | Specify `--csharp-assembly=<path>` manually |
| Partial recovery | Check `--include`/`--exclude` patterns |

## Related Files

- GDRE Tools CLI: `D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe`
- Output directory: `D:\Game\Sts2Code`
- CLI guide: `skills/gdre-tools/guide/CLI.md`

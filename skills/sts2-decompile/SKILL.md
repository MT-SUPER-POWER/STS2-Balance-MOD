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

### Step 1: Locate Game EXE

```powershell
# STS2 EXE path
$exePath = "D:\Game\Steam\steamapps\common\Slay the Spire 2\SlayTheSpire2.exe"

# Verify it exists
if (-not (Test-Path $exePath)) {
    Write-Host "EXE not found at: $exePath"
    # Ask user for correct path
}
```

### Step 2: Check Current Version

```powershell
# List files to see version info
$exePath = "D:\Game\Steam\steamapps\common\Slay the Spire 2\SlayTheSpire2.exe"

& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" --headless --list-files="$exePath"
```

### Step 3: Backup Current Source (Optional)

```powershell
# Create timestamped backup
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
Copy-Item -Path "D:\Game\Sts2Code" -Destination "D:\Game\Sts2Code_backup_$timestamp" -Recurse
```

### Step 4: Fully Decompile EXE

```powershell
# Full EXE recovery (PCK + C# assemblies + all resources)
$exePath = "D:\Game\Steam\steamapps\common\Slay the Spire 2\SlayTheSpire2.exe"

& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --recover="$exePath" `
    --output="D:\Game\Sts2Code"
```

**Note:** When recovering from EXE, GDRE Tools automatically:
- Extracts the embedded PCK
- Decompiles GDScript bytecode (.gdc → .gd)
- Converts binary resources to text format (.tres/.tscn)
- Decompiles C# assemblies if present

### Step 5: If C# Assembly Not Auto-Detected

```powershell
# Manually specify C# assembly path
$assemblyPath = "D:\Game\Sts2Code\SEMB_0.dll"  # or wherever it's located

& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --recover="$exePath" `
    --output="D:\Game\Sts2Code" `
    --csharp-assembly="$assemblyPath"
```

### Step 6: Compare Changes

```powershell
# Check what changed
cd D:\Game\Sts2Code
git status
git diff --stat
```

### Step 7: Update CodeGraph Index

```bash
codegraph index
```

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

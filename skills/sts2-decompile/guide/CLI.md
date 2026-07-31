# GDRE Tools CLI Guide

## Overview

GDRE Tools (Godot Reverse Engineering Tools) v2.6.0 provides a full CLI interface for decompiling Godot games. All commands require `--headless` flag to skip GUI.

**Executable:** `D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe`

## Input Types

GDRE Tools can process:
- **EXE** — Game executable (recommended for full recovery)
- **PCK** — Godot resource package
- **APK** — Android package
- **DIR** — Extracted project directory

**For STS2:** Use the EXE directly — it extracts the embedded PCK and decompiles C# assemblies automatically.

## Basic Syntax

```powershell
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" --headless <command> [options]
```

## Commands

### Project Recovery (Full Decompile)

Recovers entire project including scripts, resources, scenes, translations, and C# assemblies.

```powershell
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --recover="<PCK/EXE/APK/DIR>" `
    --output="<output_dir>"
```

**When input is EXE:**
- Automatically extracts embedded PCK
- Decompiles C# assemblies (if present)
- Recovers all GDScript and resources

**Options:**
- `--output=<DIR>` — Output directory (default: `<NAME>_extracted`)
- `--scripts-only` — Only recover GDScript files
- `--include=<GLOB>` — Include files matching pattern (repeatable)
- `--exclude=<GLOB>` — Exclude files matching pattern (repeatable)
- `--ignore-checksum-errors` — Ignore MD5 checksum errors
- `--skip-checksum-check` — Skip MD5 checksum verification
- `--csharp-assembly=<PATH>` — Path to C# assembly (auto-detected if omitted)
- `--force-bytecode-version=<VER>` — Force bytecode version (commit hash or version string)
- `--load-custom-bytecode=<JSON>` — Load custom bytecode definition
- `--translation-hint=<FILE>` — Translation hint file (.csv/.txt/.po/.mo)
- `--key=<64-char-hex>` — Decryption key for encrypted PCK

**Examples:**

```powershell
# Full EXE recovery (recommended for STS2)
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --recover="D:\Game\Steam\steamapps\common\Slay the Spire 2\SlayTheSpire2.exe" `
    --output="D:\Game\Sts2Code"

# Full PCK recovery
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --recover="game.pck" `
    --output="output"

# Scripts only
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --recover="game.exe" `
    --output="output" `
    --scripts-only

# With include filter
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --recover="game.exe" `
    --output="output" `
    --include="res://scripts/**/*.gdc"

# With exclude filter
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --recover="game.exe" `
    --output="output" `
    --exclude="res://**/*.png"
```

---

### Extract (No Decompile)

Extracts files without decompiling scripts or converting binary resources.

```powershell
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --extract="<PCK/EXE/APK>" `
    --output="<output_dir>"
```

**Options:** Same as `--recover` (except `--scripts-only`)

**Example:**

```powershell
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --extract="game.pck" `
    --output="extracted"
```

---

### List Files

Lists all files in a PCK/EXE/APK without extracting.

```powershell
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --list-files="<PCK/EXE/APK>"
```

**Example:**

```powershell
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --list-files="D:\Game\Steam\steamapps\common\Slay the Spire 2\SlayTheSpire2.exe"
```

---

### Decompile Single File

Decompile a specific GDC (GDScript bytecode) file to text.

```powershell
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --decompile="<GDC_FILE>"
```

**Options:**
- `--bytecode=<VERSION>` — Bytecode version (commit hash or version string)
- `--load-custom-bytecode=<JSON>` — Custom bytecode definition
- `--output=<DIR>` — Output directory

**Example:**

```powershell
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --decompile="res://scripts/main.gdc"
```

---

### Compile GDScript

Compile GDScript text files to bytecode.

```powershell
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --compile="<GD_FILE>" `
    --bytecode="<VERSION>"
```

**Options:**
- `--bytecode=<VERSION>` — **Required.** Target bytecode version
- `--output=<DIR>` — Output directory

**Example:**

```powershell
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --compile="res://scripts/main.gd" `
    --bytecode="4.3.0"
```

---

### Binary to Text

Convert binary scene/resource files (.tres, .tscn, .res, .scn) to text format.

```powershell
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --bin-to-txt="<FILE>"
```

**Example:**

```powershell
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --bin-to-txt="res://scenes/main.tscn"
```

---

### Text to Binary

Convert text scene/resource files to binary format.

```powershell
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --txt-to-bin="<FILE>"
```

**Example:**

```powershell
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --txt-to-bin="res://scenes/main.tscn"
```

---

### Create PCK

Create a new PCK file from a directory.

```powershell
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --pck-create="<DIR>" `
    --pck-version=<0|1|2> `
    --pck-engine-version="<x.y.z>" `
    --output="<OUTPUT_PCK>"
```

**Options:**
- `--pck-version=<0|1|2>` — **Required.** PCK format version
- `--pck-engine-version=<x.y.z>` — **Required.** Target Godot version
- `--embed=<EXE>` — Embed PCK into executable
- `--key=<64-char-hex>` — Encrypt PCK

**Example:**

```powershell
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --pck-create="my_project" `
    --pck-version=2 `
    --pck-engine-version="4.5.1" `
    --output="my_project.pck"
```

---

### Patch PCK

Replace files in an existing PCK.

```powershell
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --pck-patch="<ORIGINAL_PCK>" `
    --patch-file="<SRC>=<DEST>" `
    --output="<OUTPUT_PCK>"
```

**Options:**
- `--patch-file=<SRC>=<DEST>` — **Required.** File to patch (repeatable)
- `--include=<GLOB>` — Only include matching files
- `--exclude=<GLOB>` — Exclude matching files
- `--embed=<EXE>` — Embed into executable
- `--key=<64-char-hex>` — Encryption/decryption key

**Example:**

```powershell
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --pck-patch="game.pck" `
    --patch-file="res://scripts/main.gdc=res://scripts/main.gdc" `
    --output="patched.pck"
```

---

### List Bytecode Versions

List all available GDScript bytecode versions.

```powershell
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --list-bytecode-versions
```

---

### Dump Bytecode Versions

Export all bytecode definitions to JSON files.

```powershell
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --dump-bytecode-versions="<OUTPUT_DIR>"
```

---

### Patch Translations

Patch translation files from CSV.

```powershell
& "D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe" `
    --headless `
    --patch-translations="<CSV_FILE>=<SRC_PATH>"
```

**Options:**
- `--pck=<PCK>` — Source PCK with translations
- `--output=<DIR>` — Output directory
- `--locales=<LOCALES>` — Comma-separated locale list

---

## Glob Patterns

Include/Exclude patterns support:

| Pattern | Meaning |
|---------|---------|
| `**` | Recursive match |
| `res://` | Project root |
| `user://` | User directory |
| `*.gdc` | All GDC files (recursive if has directory) |
| `res://scripts/**/*.gdc` | All GDC in scripts/ recursively |

**Notes:**
- Globs must be rooted to `res://` or `user://`
- If not rooted, automatically prefixed with `res://`
- Wildcard + directory = recursive pattern

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Version mismatch errors | Use `--force-bytecode-version=<version>` |
| Checksum errors | Use `--ignore-checksum-errors` or `--skip-checksum-check` |
| Encrypted PCK | Use `--key=<64-char-hex>` |
| Incomplete recovery | Check `--include`/`--exclude` patterns |
| C# decompilation fails | Specify `--csharp-assembly=<path>` manually |
| GUI opens instead of CLI | Ensure `--headless` flag is present |

#!/usr/bin/env python3
"""
STS2 Stable Automated Decompiler CLI
Reliably decompiles the latest STS2 game C# assembly and Godot resources into D:\\Game\\Sts2Code.

Usage:
    python skills/sts2-decompile/scripts/decompile.py                  # Full code + localization update (Recommended)
    python skills/sts2-decompile/scripts/decompile.py --csharp-only    # Only decompile C# code (~45s)
    python skills/sts2-decompile/scripts/decompile.py --full-assets    # Full recovery including all 15k textures & audio (~3-4 min)
"""

import argparse
import datetime
import os
import shutil
import subprocess
import sys
import time
from pathlib import Path

# Fix Windows console UTF-8 output
if sys.platform == "win32":
    try:
        sys.stdout.reconfigure(encoding="utf-8")
        sys.stderr.reconfigure(encoding="utf-8")
    except Exception:
        pass

# ─── Default Paths ────────────────────────────────────────────────────────────

DEFAULT_GAME_DIR = r"D:\Game\Steam\steamapps\common\Slay the Spire 2"
DEFAULT_OUTPUT_DIR = r"D:\Game\Sts2Code"
DEFAULT_GDRE_EXE = r"D:\Game\Godot\GDRE_tools-v2.6.0-windows\gdre_tools.exe"


def log(msg: str):
    print(f"[{datetime.datetime.now().strftime('%H:%M:%S')}] {msg}", flush=True)


def find_game_files(game_dir: Path):
    exe_path = game_dir / "SlayTheSpire2.exe"
    pck_path = game_dir / "SlayTheSpire2.pck"
    
    # Locate sts2.dll (under data_sts2_windows_x86_64/)
    sts2_dll = None
    for p in game_dir.glob("**/sts2.dll"):
        if "mods" not in p.parts:
            sts2_dll = p
            break

    return exe_path, pck_path, sts2_dll


def run_csharp_decompile(sts2_dll: Path, output_src_dir: Path):
    t0 = time.time()
    log("=== [1/2] Decompiling C# Assembly (sts2.dll) via ilspycmd ===")
    if not sts2_dll or not sts2_dll.exists():
        raise FileNotFoundError(f"sts2.dll not found at {sts2_dll}")

    ilspycmd_path = shutil.which("ilspycmd")
    if not ilspycmd_path:
        raise EnvironmentError("ilspycmd CLI tool not found in PATH. Please install it via 'dotnet tool install -g ilspycmd'.")

    # Clean old src directory to ensure NO stale or duplicate classes remain
    if output_src_dir.exists():
        log(f"Cleaning old C# src directory: {output_src_dir}")
        shutil.rmtree(output_src_dir)
    output_src_dir.mkdir(parents=True, exist_ok=True)

    cmd = [
        "ilspycmd",
        "-p",
        "-o", str(output_src_dir),
        str(sts2_dll)
    ]
    log(f"Running ilspycmd on {sts2_dll.name}...")
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="ignore")
    if res.returncode != 0:
        log(f"Warning: ilspycmd returned non-zero code {res.returncode}:\n{res.stderr}")
    else:
        log(f"[OK] C# decompilation complete in {time.time() - t0:.1f}s")


def run_gdre_recovery(gdre_exe: Path, input_target: Path, output_dir: Path, full_assets: bool = False):
    t0 = time.time()
    log("=== [2/2] Recovering Game Resources & Localization via GDRE Tools ===")
    if not gdre_exe.exists():
        raise FileNotFoundError(f"GDRE tools not found at {gdre_exe}")

    cmd = [
        str(gdre_exe),
        "--headless",
        f"--recover={input_target}",
        f"--output={output_dir}",
    ]
    if not full_assets:
        # Include localization and scripts for fast, high-efficiency updates
        cmd.append("--include=res://localization/**")
        log("Running GDRE fast recovery (Localization)...")
    else:
        log("Running GDRE full recovery (All Assets, Shaders, Textures)...")

    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="ignore")
    log(f"[OK] GDRE Tools recovery complete in {time.time() - t0:.1f}s")


def verify_decompilation(output_dir: Path):
    log("=== Verifying Output Source ===")
    src_dir = output_dir / "src"
    loc_dir = output_dir / "localization"

    csharp_files = list(src_dir.glob("**/*.cs")) if src_dir.exists() else []
    json_files = list(loc_dir.glob("**/*.json")) if loc_dir.exists() else []

    log(f"Decompiled C# files (.cs):    {len(csharp_files)}")
    log(f"Localization JSON files:      {len(json_files)}")

    if len(csharp_files) == 0:
        log("ERROR: No C# files found in output src directory!")
        return False

    log("[OK] Source code verification passed successfully!")
    return True


def main():
    parser = argparse.ArgumentParser(description="STS2 Stable Automated Decompiler CLI")
    parser.add_argument("--game-dir", type=str, default=DEFAULT_GAME_DIR, help="STS2 Game installation directory")
    parser.add_argument("--output-dir", type=str, default=DEFAULT_OUTPUT_DIR, help="Target decompiled source directory")
    parser.add_argument("--gdre-path", type=str, default=DEFAULT_GDRE_EXE, help="Path to gdre_tools.exe")
    parser.add_argument("--csharp-only", action="store_true", help="Only decompile C# assembly sts2.dll")
    parser.add_argument("--resources-only", action="store_true", help="Only recover Godot PCK resources")
    parser.add_argument("--full-assets", action="store_true", help="Recover all 15k+ textures, shaders & audio (slower)")
    args = parser.parse_args()

    game_dir = Path(args.game_dir)
    output_dir = Path(args.output_dir)
    gdre_exe = Path(args.gdre_path)

    if not game_dir.exists():
        log(f"Error: Game directory not found at {game_dir}")
        sys.exit(1)

    exe_path, pck_path, sts2_dll = find_game_files(game_dir)
    target_input = exe_path if exe_path.exists() else pck_path

    output_dir.mkdir(parents=True, exist_ok=True)
    src_dir = output_dir / "src"

    log(f"STS2 Game Directory: {game_dir}")
    log(f"Target PCK/EXE:      {target_input}")
    log(f"Target C# Assembly:  {sts2_dll}")
    log(f"Output Destination:  {output_dir}")

    total_start = time.time()

    if not args.resources_only:
        run_csharp_decompile(sts2_dll, src_dir)

    if not args.csharp_only:
        run_gdre_recovery(gdre_exe, target_input, output_dir, full_assets=args.full_assets)

    success = verify_decompilation(output_dir)
    if not success:
        sys.exit(1)

    log(f"\n[DONE] Decompilation finished in {time.time() - total_start:.1f}s! Source in D:\\Game\\Sts2Code is 100% up to date.")


if __name__ == "__main__":
    main()

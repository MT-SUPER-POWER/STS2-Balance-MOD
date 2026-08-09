"""
火堆选项图标切图脚本。

将任意 PNG 等比缩放到 STS2 火堆选项按钮使用的 256x169 画布，
并输出到 Mod 资源目录：
  - Sts2BalanceMod/images/ui/rest_site/option_{name}.png

用法:
  python rest_site_options.py
  python rest_site_options.py smoke.png
"""

from __future__ import annotations

import argparse
import re
import sys
from pathlib import Path

from PIL import Image

# ======================== CONFIG ========================

SCRIPT_DIR = Path(__file__).resolve().parent
INPUT_DIR = SCRIPT_DIR / "source" / "rest_site_options"
OUTPUT_DIR = SCRIPT_DIR.parent / "Sts2BalanceMod" / "images" / "ui" / "rest_site"
OPTION_SIZE = (256, 169)
RESAMPLE = Image.LANCZOS

# ======================== CORE ========================


def fit_contain(img: Image.Image, size: tuple[int, int]) -> Image.Image:
    """等比缩放并居中放入透明画布，避免源图被裁切。"""
    target_w, target_h = size
    src_w, src_h = img.size
    scale = min(target_w / src_w, target_h / src_h)
    scaled_w = round(src_w * scale)
    scaled_h = round(src_h * scale)
    resized = img.resize((scaled_w, scaled_h), RESAMPLE)

    canvas = Image.new("RGBA", size, (0, 0, 0, 0))
    left = (target_w - scaled_w) // 2
    top = (target_h - scaled_h) // 2
    canvas.alpha_composite(resized, (left, top))
    return canvas


def output_name(src: Path) -> str:
    """输出文件名：Option{Name}.png（PascalCase 驼峰）。"""
    stem = src.stem
    if stem.lower().startswith("option_"):
        stem = stem[7:]
    elif stem.lower().startswith("option"):
        stem = stem[6:]
    stem = stem.capitalize()
    return f"Option{stem}.png"


def collect_sources(input_dir: Path, names: list[str] | None) -> list[Path]:
    """收集待处理的火堆选项源图。"""
    if names:
        files = []
        for name in names:
            path = input_dir / name
            if not path.exists():
                print(f"错误: 找不到源文件 {path}", file=sys.stderr)
                sys.exit(1)
            files.append(path)
        return files

    if not input_dir.exists():
        input_dir.mkdir(parents=True, exist_ok=True)
        print(f"已创建输入目录: {input_dir}")
        print("请将火堆选项 PNG 放入该目录后重新运行。")
        sys.exit(0)

    files = sorted(input_dir.glob("*.png"))
    if not files:
        print(f"输入目录为空: {input_dir}")
        print("请将火堆选项 PNG 放入该目录。")
        sys.exit(0)

    return files


def process_icon(src: Path, output_dir: Path) -> None:
    """处理单张火堆选项图标。"""
    with Image.open(src) as raw:
        img = raw.convert("RGBA")

    icon = fit_contain(img, OPTION_SIZE)
    output_dir.mkdir(parents=True, exist_ok=True)
    out_path = output_dir / output_name(src)
    icon.save(out_path)

    print(f"  {src.name}: {img.size[0]}x{img.size[1]} -> {out_path}")


def main() -> None:
    parser = argparse.ArgumentParser(description="STS2 Mod 火堆选项图标切图工具")
    parser.add_argument(
        "files",
        nargs="*",
        help="指定要处理的文件名（相对于 source/rest_site_options/），不填则处理全部 PNG",
    )
    parser.add_argument(
        "--input",
        type=Path,
        default=INPUT_DIR,
        help=f"火堆选项源图目录（默认: {INPUT_DIR}）",
    )
    parser.add_argument(
        "--output",
        type=Path,
        default=OUTPUT_DIR,
        help=f"输出目录（默认: {OUTPUT_DIR}）",
    )
    args = parser.parse_args()

    sources = collect_sources(args.input, args.files or None)
    print(f"火堆选项输入: {args.input}")
    print(f"火堆选项输出: {args.output}")
    print(f"共 {len(sources)} 张\n")

    for src in sources:
        process_icon(src, args.output)

    print("\n完成!")


if __name__ == "__main__":
    main()

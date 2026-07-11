"""
附魔图标生成脚本

将原始素材缩放到 STS2 Mod 要求的小尺寸，并输出到 Sts2BalanceMod/images/enchantments/。

用法:
  python enchantments.py                        # 处理 source/enchantments/ 下所有 PNG
  python enchantments.py forge_enchantment.png   # 只处理指定附魔图标
"""

from __future__ import annotations

import argparse
import re
import sys
from pathlib import Path

from PIL import Image

# ======================== CONFIG ========================

SCRIPT_DIR = Path(__file__).resolve().parent
INPUT_DIR = SCRIPT_DIR / "source" / "enchantments"
MOD_ROOT = SCRIPT_DIR.parent / "Sts2BalanceMod" / "images" / "enchantments"

TARGET_SIZE = (128, 128)
RESAMPLE = Image.LANCZOS


def fit_cover(img: Image.Image, size: tuple[int, int]) -> Image.Image:
    """等比放大后居中裁切，填满目标尺寸（不拉伸变形）。"""
    target_w, target_h = size
    src_w, src_h = img.size
    scale = max(target_w / src_w, target_h / src_h)
    scaled_w = round(src_w * scale)
    scaled_h = round(src_h * scale)
    resized = img.resize((scaled_w, scaled_h), RESAMPLE)

    left = (scaled_w - target_w) // 2
    top = (scaled_h - target_h) // 2

    return resized.crop((left, top, left + target_w, top + target_h))


def to_snake_case(name: str) -> str:
    s1 = re.sub(r'([a-z0-9])([A-Z])', r'\1_\2', name)
    s2 = re.sub(r'([A-Z])([A-Z][a-z])', r'\1_\2', s1)
    s3 = re.sub(r'[\s\-]+', '_', s2)
    return re.sub(r'_+', '_', s3).lower()


def output_name(src: Path) -> str:
    """输出文件名：全小写，驼峰转下划线小写。"""
    return f"{to_snake_case(src.stem)}.png"


def process_image(src: Path, out: Path) -> None:
    """处理单张附魔图标。"""
    with Image.open(src) as raw:
        img = raw.convert("RGBA")

    result = fit_cover(img, TARGET_SIZE)

    out.parent.mkdir(parents=True, exist_ok=True)
    result.save(out)

    print(f"  {src.name}: {img.size[0]}x{img.size[1]} -> {TARGET_SIZE[0]}x{TARGET_SIZE[1]}")


def collect_sources(input_dir: Path, names: list[str] | None) -> list[Path]:
    """收集待处理的源文件列表。"""
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
        print("请将附魔图标 PNG 放入该目录后重新运行。")
        sys.exit(0)

    files = sorted(input_dir.glob("*.png"))
    if not files:
        print(f"输入目录为空: {input_dir}")
        print("请将附魔图标 PNG 放入该目录。")
        sys.exit(0)

    return files


def main() -> None:
    parser = argparse.ArgumentParser(description="STS2 Mod 附魔图标生成工具")
    parser.add_argument(
        "files", nargs="*",
        help="指定要处理的文件名（相对于 source/enchantments/），不填则处理全部",
    )
    parser.add_argument(
        "--input", type=Path, default=INPUT_DIR,
        help=f"源图目录（默认: {INPUT_DIR}）",
    )
    parser.add_argument(
        "--output", type=Path, default=MOD_ROOT,
        help=f"输出目录（默认: {MOD_ROOT}）",
    )
    args = parser.parse_args()

    sources = collect_sources(args.input, args.files or None)

    print(f"输入: {args.input}")
    print(f"输出: {args.output} ({TARGET_SIZE[0]}x{TARGET_SIZE[1]})")
    print(f"共 {len(sources)} 张\n")

    for src in sources:
        process_image(src, args.output / output_name(src))

    print("\n完成!")


if __name__ == "__main__":
    main()

"""
能力图标生成脚本

将原始素材裁切/缩放到 STS2 Mod 要求的尺寸，并输出到对应目录：
  - 大图（战斗内展示）: 256x256 -> Sts2BalanceMod/images/powers/big/
  - 小图（卡牌提示等）: 64x64   -> Sts2BalanceMod/images/powers/

用法:
  python powers.py                             # 处理 image_gen/source/powers/ 下所有 PNG
  python powers.py electrodynamics_power.png    # 只处理指定文件
"""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

from PIL import Image

# ======================== CONFIG ========================

SCRIPT_DIR = Path(__file__).resolve().parent
INPUT_DIR = SCRIPT_DIR / "source" / "powers"
MOD_ROOT = SCRIPT_DIR.parent / "Sts2BalanceMod" / "images" / "powers"

BIG_SIZE = (256, 256)
SMALL_SIZE = (64, 64)

RESAMPLE = Image.LANCZOS

# ======================== CORE ========================


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


def process_power(src: Path, out_big: Path, out_small: Path) -> None:
    """处理单张能力图标，输出大图 (256x256) 与小图 (64x64)。"""
    with Image.open(src) as raw:
        img = raw.convert("RGBA")

    big = fit_cover(img, BIG_SIZE)
    small = fit_cover(img, SMALL_SIZE)

    out_big.parent.mkdir(parents=True, exist_ok=True)
    out_small.parent.mkdir(parents=True, exist_ok=True)

    big.save(out_big)
    small.save(out_small)

    print(f"  {src.name}: {img.size[0]}x{img.size[1]} -> {BIG_SIZE[0]}x{BIG_SIZE[1]} + {SMALL_SIZE[0]}x{SMALL_SIZE[1]}")


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
        print("请将能力图标 PNG 放入该目录后重新运行。")
        sys.exit(0)

    files = sorted(input_dir.glob("*.png"))
    if not files:
        print(f"输入目录为空: {input_dir}")
        print("请将能力图标 PNG 放入该目录。")
        sys.exit(0)

    return files


def main() -> None:
    parser = argparse.ArgumentParser(description="STS2 Mod 能力图标生成工具")
    parser.add_argument(
        "files", nargs="*",
        help="指定要处理的文件名（相对于 source/），不填则处理全部 PNG",
    )
    parser.add_argument(
        "--input", type=Path, default=INPUT_DIR,
        help=f"源图目录（默认: {INPUT_DIR}）",
    )
    parser.add_argument(
        "--output", type=Path, default=MOD_ROOT,
        help=f"输出根目录（默认: {MOD_ROOT}）",
    )
    args = parser.parse_args()

    sources = collect_sources(args.input, args.files or None)
    out_big_dir = args.output / "big"
    out_small_dir = args.output

    print(f"输入: {args.input}")
    print(f"输出: {out_small_dir} + {out_big_dir}")
    print(f"共 {len(sources)} 张\n")

    for src in sources:
        process_power(
            src,
            out_big_dir / src.name,
            out_small_dir / src.name,
        )

    print("\n完成!")


if __name__ == "__main__":
    main()

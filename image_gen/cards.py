"""
卡牌立绘切图脚本

将原始素材裁切/缩放到 STS2 Mod 要求的尺寸，并输出到对应目录：
  - 大图（战斗内展示）: 1000x760 -> Sts2BalanceMod/images/card_portraits/big/
  - 小图（卡牌列表等）  : 500x380  -> Sts2BalanceMod/images/card_portraits/

用法:
  python cards.py                     # 处理 image_gen/source/cards/ 下所有 PNG
  python cards.py death_reap.png      # 只处理指定文件
  python cards.py --mode contain      # 完整显示（留透明边），默认 cover 居中裁切
  python cards.py --anchor top        # 裁切锚点: center / top / bottom
"""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

from PIL import Image

# ======================== CONFIG ========================

SCRIPT_DIR = Path(__file__).resolve().parent
INPUT_DIR = SCRIPT_DIR / "source" / "cards"
MOD_ROOT = SCRIPT_DIR.parent / "Sts2BalanceMod" / "images" / "card_portraits"

BIG_SIZE = (1000, 760)
SMALL_SIZE = (500, 380)

RESAMPLE = Image.LANCZOS

# ======================== CORE ========================


def fit_cover(img: Image.Image, size: tuple[int, int], anchor: str) -> Image.Image:
    """
    等比放大后居中裁切，填满目标尺寸（不拉伸变形）。

    参数:
        img: 输入 RGBA 图像
        size: 目标 (宽, 高)
        anchor: 垂直锚点 center / top / bottom
    返回:
        裁切后的图像
    """
    target_w, target_h = size
    src_w, src_h = img.size
    scale = max(target_w / src_w, target_h / src_h)
    scaled_w = round(src_w * scale)
    scaled_h = round(src_h * scale)
    resized = img.resize((scaled_w, scaled_h), RESAMPLE)

    left = (scaled_w - target_w) // 2
    if anchor == "top":
        top = 0
    elif anchor == "bottom":
        top = scaled_h - target_h
    else:
        top = (scaled_h - target_h) // 2

    return resized.crop((left, top, left + target_w, top + target_h))


def fit_contain(img: Image.Image, size: tuple[int, int]) -> Image.Image:
    """
    等比缩小后居中放置，不足区域填充透明。

    参数:
        img: 输入 RGBA 图像
        size: 目标 (宽, 高)
    返回:
        缩放并居中后的图像
    """
    target_w, target_h = size
    src_w, src_h = img.size
    scale = min(target_w / src_w, target_h / src_h)
    scaled_w = round(src_w * scale)
    scaled_h = round(src_h * scale)
    resized = img.resize((scaled_w, scaled_h), RESAMPLE)

    canvas = Image.new("RGBA", size, (0, 0, 0, 0))
    left = (target_w - scaled_w) // 2
    top = (target_h - scaled_h) // 2
    canvas.paste(resized, (left, top))
    return canvas


def fit_stretch(img: Image.Image, size: tuple[int, int]) -> Image.Image:
    """直接拉伸到目标尺寸（可能变形）。"""
    return img.resize(size, RESAMPLE)


def process_image(
    src: Path,
    mode: str,
    anchor: str,
    out_big: Path,
    out_small: Path,
) -> None:
    """
    处理单张卡牌立绘，输出大图与小图。

    参数:
        src: 源文件路径
        mode: cover / contain / stretch
        anchor: 裁切锚点（仅 cover 模式有效）
        out_big: 大图输出路径
        out_small: 小图输出路径
    """
    fitters = {
        "cover": lambda img, size: fit_cover(img, size, anchor),
        "contain": fit_contain,
        "stretch": fit_stretch,
    }
    fit = fitters[mode]

    with Image.open(src) as raw:
        img = raw.convert("RGBA")

    big = fit(img, BIG_SIZE)
    small = fit(img, SMALL_SIZE)

    out_big.parent.mkdir(parents=True, exist_ok=True)
    out_small.parent.mkdir(parents=True, exist_ok=True)

    big.save(out_big)
    small.save(out_small)

    print(f"  {src.name}: {img.size[0]}x{img.size[1]} -> big {BIG_SIZE[0]}x{BIG_SIZE[1]}, small {SMALL_SIZE[0]}x{SMALL_SIZE[1]}")


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
        print("请将原始卡牌立绘 PNG 放入该目录后重新运行。")
        sys.exit(0)

    files = sorted(input_dir.glob("*.png"))
    if not files:
        print(f"输入目录为空: {input_dir}")
        print("请将原始卡牌立绘 PNG 放入该目录。")
        sys.exit(0)

    return files


def main() -> None:
    parser = argparse.ArgumentParser(description="STS2 Mod 卡牌立绘切图工具")
    parser.add_argument(
        "files",
        nargs="*",
        help="指定要处理的文件名（相对于 source/），不填则处理全部",
    )
    parser.add_argument(
        "--input",
        type=Path,
        default=INPUT_DIR,
        help=f"源图目录（默认: {INPUT_DIR}）",
    )
    parser.add_argument(
        "--output",
        type=Path,
        default=MOD_ROOT,
        help=f"输出根目录（默认: {MOD_ROOT}）",
    )
    parser.add_argument(
        "--mode",
        choices=["cover", "contain", "stretch"],
        default="cover",
        help="缩放模式: cover=裁切填满(默认), contain=完整显示留边, stretch=拉伸",
    )
    parser.add_argument(
        "--anchor",
        choices=["center", "top", "bottom"],
        default="center",
        help="cover 模式的垂直裁切锚点（默认: center）",
    )
    args = parser.parse_args()

    sources = collect_sources(args.input, args.files or None)
    out_big_dir = args.output / "big"
    out_small_dir = args.output

    print(f"模式: {args.mode}  锚点: {args.anchor}")
    print(f"输入: {args.input}")
    print(f"输出: {out_small_dir} + {out_big_dir}")
    print(f"共 {len(sources)} 张\n")

    for src in sources:
        process_image(
            src,
            args.mode,
            args.anchor,
            out_big_dir / src.name,
            out_small_dir / src.name,
        )

    print("\n完成!")


if __name__ == "__main__":
    main()

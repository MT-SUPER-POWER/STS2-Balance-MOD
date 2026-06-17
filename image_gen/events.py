"""
事件背景图切图脚本

将原始素材裁切/缩放到 STS2 Mod 要求的事件图尺寸（3440x1616），
并输出到 Sts2BalanceMod/images/events/ 目录。

用法:
  python events.py                          # 处理 source/events/ 下所有 PNG
  python events.py masked_bandits.png       # 只处理指定文件
  python events.py --mode contain           # 完整显示（留透明边），默认 cover 居中裁切
"""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

from PIL import Image

# ======================== CONFIG ========================

SCRIPT_DIR = Path(__file__).resolve().parent
INPUT_DIR = SCRIPT_DIR / "source" / "events"
MOD_ROOT = SCRIPT_DIR.parent / "Sts2BalanceMod" / "images" / "events"

TARGET_SIZE = (3440, 1616)

RESAMPLE = Image.LANCZOS

# ======================== CORE ========================


def fit_cover(img: Image.Image, size: tuple[int, int], anchor: str) -> Image.Image:
    """
    等比放大后裁切，填满目标尺寸（不拉伸变形）。

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
    out: Path,
) -> None:
    """
    处理单张事件背景图，输出 3440x1616。

    参数:
        src: 源文件路径
        mode: cover / contain / stretch
        anchor: 裁切锚点（仅 cover 模式有效）
        out: 输出路径
    """
    fitters = {
        "cover": lambda img, size: fit_cover(img, size, anchor),
        "contain": fit_contain,
        "stretch": fit_stretch,
    }
    fit = fitters[mode]

    with Image.open(src) as raw:
        img = raw.convert("RGBA")

    result = fit(img, TARGET_SIZE)

    out.parent.mkdir(parents=True, exist_ok=True)
    result.save(out)

    print(f"  {src.name}: {img.size[0]}x{img.size[1]} -> {TARGET_SIZE[0]}x{TARGET_SIZE[1]}")


def output_name(src: Path) -> str:
    """输出文件名：去掉模组前缀后全小写。"""
    name = src.stem.lower()
    # 去掉常见的模组前缀
    for prefix in ("sts2balancemod-", "sts2balancemod_", "actsfromthepast-"):
        if name.startswith(prefix):
            name = name[len(prefix):]
            break
    return f"{name}.png"


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
        print("请将原始事件背景 PNG 放入该目录后重新运行。")
        sys.exit(0)

    files = sorted(input_dir.glob("*.png"))
    if not files:
        print(f"输入目录为空: {input_dir}")
        print("请将原始事件背景 PNG 放入该目录。")
        sys.exit(0)

    return files


def main() -> None:
    parser = argparse.ArgumentParser(description="STS2 Mod 事件背景图生成工具")
    parser.add_argument(
        "files",
        nargs="*",
        help="指定要处理的文件名（相对于 source/events/），不填则处理全部",
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
        help=f"输出目录（默认: {MOD_ROOT}）",
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

    print(f"模式: {args.mode}  锚点: {args.anchor}")
    print(f"输入: {args.input}")
    print(f"输出: {args.output} ({TARGET_SIZE[0]}x{TARGET_SIZE[1]})")
    print(f"共 {len(sources)} 张\n")

    for src in sources:
        out_name = output_name(src)
        process_image(src, args.mode, args.anchor, args.output / out_name)

    print("\n完成!")


if __name__ == "__main__":
    main()

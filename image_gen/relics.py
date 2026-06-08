"""
遗物图标生成脚本

将原始素材裁切/缩放到 STS2 Mod 要求的尺寸，并输出到对应目录：
  - 大图（战斗内展示）: 256x256 -> Sts2BalanceMod/images/relics/big/
  - 小图（卡牌提示等）: 94x94   -> Sts2BalanceMod/images/relics/
  - 轮廓图（小图轮廓）: 94x94   -> Sts2BalanceMod/images/relics/{name}_outline.png

用法:
  python relics.py                             # 处理 source/relics/ 与 source/relics/outlines/ 下所有 PNG
  python relics.py Sundial.png                   # 只处理指定遗物图标
  python relics.py --outline-only              # 只处理轮廓图
"""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

from PIL import Image

# ======================== CONFIG ========================

SCRIPT_DIR = Path(__file__).resolve().parent
INPUT_DIR = SCRIPT_DIR / "source" / "relics"
OUTLINE_INPUT_DIR = INPUT_DIR / "outlines"
MOD_ROOT = SCRIPT_DIR.parent / "Sts2BalanceMod" / "images" / "relics"

BIG_SIZE = (256, 256)
SMALL_SIZE = (94, 94)

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


def relic_output_name(src: Path) -> str:
    """遗物小图/大图输出文件名（小写）。"""
    return f"{src.stem.lower()}.png"


def outline_output_name(src: Path) -> str:
    """遗物轮廓图输出文件名（小写，带 _outline 后缀）。"""
    stem = src.stem.lower()
    for suffix in ("_outlines", "_outline"):
        if stem.endswith(suffix):
            stem = stem[: -len(suffix)]
            break
    return f"{stem}_outline.png"


def process_relic(src: Path, out_big: Path, out_small: Path) -> None:
    """处理单张遗物图标，输出大图 (256x256) 与小图 (94x94)。"""
    with Image.open(src) as raw:
        img = raw.convert("RGBA")

    big = fit_cover(img, BIG_SIZE)
    small = fit_cover(img, SMALL_SIZE)

    out_big.parent.mkdir(parents=True, exist_ok=True)
    out_small.parent.mkdir(parents=True, exist_ok=True)

    big.save(out_big)
    small.save(out_small)

    print(
        f"  {src.name}: {img.size[0]}x{img.size[1]} -> "
        f"{BIG_SIZE[0]}x{BIG_SIZE[1]} + {SMALL_SIZE[0]}x{SMALL_SIZE[1]}"
    )


def process_outline(src: Path, out_small: Path) -> None:
    """处理单张遗物轮廓图，输出 94x94 小图。"""
    with Image.open(src) as raw:
        img = raw.convert("RGBA")

    small = fit_cover(img, SMALL_SIZE)

    out_small.parent.mkdir(parents=True, exist_ok=True)
    small.save(out_small)

    print(f"  {src.name}: {img.size[0]}x{img.size[1]} -> {SMALL_SIZE[0]}x{SMALL_SIZE[1]} (outline)")


def collect_sources(
    input_dir: Path,
    names: list[str] | None,
    *,
    label: str,
    required: bool = True,
) -> list[Path]:
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
        if required:
            input_dir.mkdir(parents=True, exist_ok=True)
            print(f"已创建输入目录: {input_dir}")
            print(f"请将{label} PNG 放入该目录后重新运行。")
            sys.exit(0)
        return []

    files = sorted(input_dir.glob("*.png"))
    if not files and required:
        print(f"输入目录为空: {input_dir}")
        print(f"请将{label} PNG 放入该目录。")
        sys.exit(0)

    return files


def main() -> None:
    parser = argparse.ArgumentParser(description="STS2 Mod 遗物图标生成工具")
    parser.add_argument(
        "files", nargs="*",
        help="指定要处理的文件名（相对于 source/relics/），不填则处理全部 PNG",
    )
    parser.add_argument(
        "--input", type=Path, default=INPUT_DIR,
        help=f"遗物源图目录（默认: {INPUT_DIR}）",
    )
    parser.add_argument(
        "--outline-input", type=Path, default=OUTLINE_INPUT_DIR,
        help=f"轮廓图源图目录（默认: {OUTLINE_INPUT_DIR}）",
    )
    parser.add_argument(
        "--output", type=Path, default=MOD_ROOT,
        help=f"输出根目录（默认: {MOD_ROOT}）",
    )
    parser.add_argument(
        "--outline-only", action="store_true",
        help="只处理轮廓图，跳过遗物主图标",
    )
    args = parser.parse_args()

    out_big_dir = args.output / "big"
    out_small_dir = args.output

    if not args.outline_only:
        sources = collect_sources(args.input, args.files or None, label="遗物图标")
        print(f"遗物输入: {args.input}")
        print(f"遗物输出: {out_small_dir} + {out_big_dir}")
        print(f"共 {len(sources)} 张\n")

        for src in sources:
            process_relic(
                src,
                out_big_dir / relic_output_name(src),
                out_small_dir / relic_output_name(src),
            )

    outline_sources = collect_sources(
        args.outline_input,
        args.files or None if args.outline_only else None,
        label="遗物轮廓图",
        required=args.outline_only,
    )

    if outline_sources:
        print(f"\n轮廓输入: {args.outline_input}")
        print(f"轮廓输出: {out_small_dir}")
        print(f"共 {len(outline_sources)} 张\n")

        for src in outline_sources:
            process_outline(
                src,
                out_small_dir / outline_output_name(src),
            )

    if not args.outline_only and not outline_sources:
        print(f"\n跳过轮廓图: 目录为空或不存在 ({args.outline_input})")

    print("\n完成!")


if __name__ == "__main__":
    main()

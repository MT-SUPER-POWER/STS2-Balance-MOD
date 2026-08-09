"""
遗物图标生成脚本
将原始素材裁切/缩放到 STS2 Mod 要求的尺寸，并输出到对应目录：
  - 大图（战斗内展示）: 256x256 -> Sts2BalanceMod/images/relics/big/
  - 小图（卡牌提示等）: 94x94   -> Sts2BalanceMod/images/relics/
  - 轮廓图（小图轮廓）: 94x94   -> Sts2BalanceMod/images/relics/outlines/

轮廓图固定从遗物主图自动生成“白色主体 + 3px 主题色外环”。

用法:
  python relics.py                 # 处理 source/relics/ 下所有 PNG
  python relics.py Sundial.png     # 只处理指定遗物图标
"""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

import cv2
import numpy as np
from PIL import Image


# ======================== CONFIG ========================

SCRIPT_DIR = Path(__file__).resolve().parent
INPUT_DIR = SCRIPT_DIR / "source" / "relics"
MOD_ROOT = SCRIPT_DIR.parent / "Sts2BalanceMod" / "images" / "relics"

BIG_SIZE = (256, 256)
SMALL_SIZE = (94, 94)
RESAMPLE = Image.Resampling.LANCZOS

OUTLINE_WIDTH = 3
OUTLINE_EXTRA_PADDING = 2
SUPERSAMPLE = 4
ALPHA_CROP_THRESHOLD = 4
ALPHA_MASK_THRESHOLD = 32
MIN_COMPONENT_RATIO = 0.00025
THEME_HUE_BINS = 24
FALLBACK_THEME_COLOR = (127, 82, 168)


# ======================== CORE ========================


def trim_transparent(
    image: Image.Image,
    alpha_threshold: int = ALPHA_CROP_THRESHOLD,
) -> Image.Image:
    """裁掉 Alpha 低于阈值的透明边缘。"""
    alpha = np.asarray(image.getchannel("A"))
    visible_y, visible_x = np.nonzero(alpha >= alpha_threshold)
    if visible_x.size == 0 or visible_y.size == 0:
        raise ValueError("遗物母版没有可见像素")

    bbox = (
        int(visible_x.min()),
        int(visible_y.min()),
        int(visible_x.max()) + 1,
        int(visible_y.max()) + 1,
    )
    return image.crop(bbox)


def fit_contain(
    image: Image.Image,
    size: tuple[int, int],
    padding: int = 0,
) -> Image.Image:
    """裁掉透明边缘后等比完整装入透明画布，不裁切主体。"""
    content = trim_transparent(image)
    target_width, target_height = size
    available_width = target_width - padding * 2
    available_height = target_height - padding * 2
    if available_width <= 0 or available_height <= 0:
        raise ValueError(f"留白 {padding}px 超过目标尺寸 {size}")

    scale = min(available_width / content.width, available_height / content.height)
    resized = content.resize(
        (
            max(1, round(content.width * scale)),
            max(1, round(content.height * scale)),
        ),
        RESAMPLE,
    )
    canvas = Image.new("RGBA", size, (0, 0, 0, 0))
    left = (target_width - resized.width) // 2
    top = (target_height - resized.height) // 2
    canvas.alpha_composite(resized, (left, top))
    return canvas


def fit_cover(image: Image.Image, size: tuple[int, int]) -> Image.Image:
    """等比放大后居中裁切，填满目标尺寸（不拉伸变形）。"""
    target_width, target_height = size
    scale = max(target_width / image.width, target_height / image.height)
    resized = image.resize(
        (round(image.width * scale), round(image.height * scale)),
        RESAMPLE,
    )
    left = (resized.width - target_width) // 2
    top = (resized.height - target_height) // 2
    return resized.crop((left, top, left + target_width, top + target_height))


def collect_sources(
    input_dir: Path,
    names: list[str] | None,
) -> list[Path]:
    """收集待处理的源文件列表。"""
    if names:
        sources = [input_dir / name for name in names]
        missing = [source for source in sources if not source.exists()]
        if missing:
            print(f"错误: 找不到源文件 {missing[0]}", file=sys.stderr)
            raise SystemExit(1)
        return sources

    if not input_dir.exists():
        input_dir.mkdir(parents=True, exist_ok=True)
        print(f"已创建输入目录: {input_dir}")
        print("请将遗物图标 PNG 放入该目录后重新运行。")
        raise SystemExit(0)

    sources = sorted(input_dir.glob("*.png"))
    if not sources:
        print(f"输入目录为空: {input_dir}")
        print("请将遗物图标 PNG 放入该目录。")
        raise SystemExit(0)
    return sources


def _normalize_theme_color(color: np.ndarray) -> tuple[int, int, int]:
    """限制主题色的饱和度和明度，保证小尺寸轮廓清晰可见。"""
    rgb = np.clip(np.rint(color), 0, 255).astype(np.uint8).reshape(1, 1, 3)
    hsv = cv2.cvtColor(rgb, cv2.COLOR_RGB2HSV)
    hsv[0, 0, 1] = max(int(hsv[0, 0, 1]), 120)
    hsv[0, 0, 2] = min(210, max(int(hsv[0, 0, 2]), 105))
    normalized = cv2.cvtColor(hsv, cv2.COLOR_HSV2RGB)[0, 0]
    return tuple(int(channel) for channel in normalized)


def extract_theme_color(image: Image.Image) -> tuple[int, int, int]:
    """从不透明、高饱和像素的主色相中提取稳定且清晰的描边色。"""
    sample = trim_transparent(image)
    scale = min(1.0, 256 / max(sample.size))
    if scale < 1.0:
        sample = sample.resize(
            (
                max(1, round(sample.width * scale)),
                max(1, round(sample.height * scale)),
            ),
            RESAMPLE,
        )

    rgba = np.asarray(sample, dtype=np.uint8)
    rgb = rgba[:, :, :3]
    alpha = rgba[:, :, 3]
    hsv = cv2.cvtColor(rgb, cv2.COLOR_RGB2HSV)
    hue = hsv[:, :, 0]
    saturation = hsv[:, :, 1]
    value = hsv[:, :, 2]

    candidates = (
        (alpha >= 64)
        & (saturation >= 50)
        & (value >= 35)
        & ~((value >= 230) & (saturation < 85))
    )
    if not np.any(candidates):
        fallback = (alpha >= 64) & (value >= 35) & (value <= 235)
        if not np.any(fallback):
            return FALLBACK_THEME_COLOR
        return _normalize_theme_color(
            np.average(rgb[fallback], axis=0, weights=alpha[fallback])
        )

    candidate_hues = hue[candidates].astype(np.int32)
    candidate_saturation = saturation[candidates].astype(np.float64)
    candidate_value = value[candidates].astype(np.float64)
    candidate_alpha = alpha[candidates].astype(np.float64)
    weights = (
        (candidate_alpha / 255.0)
        * np.power(candidate_saturation / 255.0, 1.5)
        * (0.5 + candidate_value / 510.0)
    )
    hue_bins = candidate_hues * THEME_HUE_BINS // 180
    histogram = np.bincount(hue_bins, weights=weights, minlength=THEME_HUE_BINS)
    smoothed = histogram + np.roll(histogram, 1) * 0.5 + np.roll(histogram, -1) * 0.5
    peak_bin = int(np.argmax(smoothed))
    circular_distance = np.minimum(
        (hue_bins - peak_bin) % THEME_HUE_BINS,
        (peak_bin - hue_bins) % THEME_HUE_BINS,
    )
    dominant = circular_distance <= 1
    dominant_rgb = rgb[candidates][dominant]
    dominant_weights = weights[dominant]
    return _normalize_theme_color(
        np.average(dominant_rgb, axis=0, weights=dominant_weights)
    )


def clean_binary_mask(alpha: np.ndarray) -> np.ndarray:
    """二值化 Alpha，并移除不会影响主体轮廓的极小孤立组件。"""
    mask = np.where(alpha >= ALPHA_MASK_THRESHOLD, 255, 0).astype(np.uint8)
    component_count, labels, stats, _ = cv2.connectedComponentsWithStats(
        mask,
        connectivity=8,
    )
    if component_count <= 1:
        return mask

    visible_area = int(np.count_nonzero(mask))
    min_area = max(4, round(visible_area * MIN_COMPONENT_RATIO))
    cleaned = np.zeros_like(mask)
    kept_any = False
    for label in range(1, component_count):
        if int(stats[label, cv2.CC_STAT_AREA]) >= min_area:
            cleaned[labels == label] = 255
            kept_any = True
    if not kept_any:
        largest_label = 1 + int(np.argmax(stats[1:, cv2.CC_STAT_AREA]))
        cleaned[labels == largest_label] = 255
    return cleaned


def build_automatic_outline(
    image: Image.Image,
    theme_color: tuple[int, int, int],
) -> Image.Image:
    """以超采样方式生成白色主体与主题色外环。"""
    high_resolution_size = (
        SMALL_SIZE[0] * SUPERSAMPLE,
        SMALL_SIZE[1] * SUPERSAMPLE,
    )
    padding = (OUTLINE_WIDTH + OUTLINE_EXTRA_PADDING) * SUPERSAMPLE
    fitted = fit_contain(image, high_resolution_size, padding=padding)
    body_mask = clean_binary_mask(np.asarray(fitted.getchannel("A")))

    radius = OUTLINE_WIDTH * SUPERSAMPLE
    kernel = cv2.getStructuringElement(
        cv2.MORPH_ELLIPSE,
        (radius * 2 + 1, radius * 2 + 1),
    )
    expanded_mask = cv2.dilate(body_mask, kernel, iterations=1)

    rgba = np.zeros(
        (high_resolution_size[1], high_resolution_size[0], 4),
        dtype=np.uint8,
    )
    expanded_pixels = expanded_mask > 0
    body_pixels = body_mask > 0
    rgba[expanded_pixels, :3] = theme_color
    rgba[expanded_pixels, 3] = 255
    rgba[body_pixels, :3] = 255
    rgba[body_pixels, 3] = 255
    return Image.fromarray(rgba).resize(SMALL_SIZE, RESAMPLE)


def validate_generated_outline(image: Image.Image, source: Path) -> None:
    """拒绝空白、裁切或缺失白色主体/主题色外环的自动轮廓。"""
    rgba = np.asarray(image, dtype=np.uint8)
    alpha = rgba[:, :, 3]
    visible_y, visible_x = np.nonzero(alpha >= 16)
    if visible_x.size == 0 or visible_y.size == 0:
        raise ValueError(f"{source.name} 的自动轮廓为空")
    if (
        visible_x.min() == 0
        or visible_y.min() == 0
        or visible_x.max() == image.width - 1
        or visible_y.max() == image.height - 1
    ):
        raise ValueError(f"{source.name} 的自动轮廓接触画布边缘，可能被裁切")

    opaque_rgb = rgba[alpha >= 220, :3]
    has_white_body = np.any(np.all(opaque_rgb >= 245, axis=1))
    has_colored_outline = np.any(np.any(opaque_rgb < 235, axis=1))
    if not has_white_body or not has_colored_outline:
        raise ValueError(f"{source.name} 的自动轮廓缺少白色主体或主题色外环")


def color_hex(color: tuple[int, int, int]) -> str:
    return "#" + "".join(f"{channel:02X}" for channel in color)


def process_relic(source: Path, output_dir: Path) -> None:
    """从一张母版生成遗物主图、大图与自动主题色轮廓图。"""
    with Image.open(source) as raw:
        image = raw.convert("RGBA")

    name = f"{source.stem}.png"
    fit_cover(image, SMALL_SIZE).save(output_dir / name)
    fit_cover(image, BIG_SIZE).save(output_dir / "big" / name)

    theme_color = extract_theme_color(image)
    outline = build_automatic_outline(image, theme_color)
    validate_generated_outline(outline, source)
    outline.save(output_dir / "outlines" / name)
    print(
        f"  {source.name}: {image.width}x{image.height} -> "
        f"94x94 + 256x256 + 94x94 outline (theme {color_hex(theme_color)})"
    )


def main() -> None:
    parser = argparse.ArgumentParser(description="STS2 Mod 遗物图标生成工具")
    parser.add_argument(
        "files",
        nargs="*",
        help="指定要处理的文件名（相对于 source/relics/），不填则处理全部 PNG",
    )
    parser.add_argument(
        "--input",
        type=Path,
        default=INPUT_DIR,
        help=f"遗物源图目录（默认: {INPUT_DIR}）",
    )
    parser.add_argument(
        "--output",
        type=Path,
        default=MOD_ROOT,
        help=f"输出根目录（默认: {MOD_ROOT}）",
    )
    args = parser.parse_args()

    sources = collect_sources(args.input, args.files or None)

    args.output.mkdir(parents=True, exist_ok=True)
    (args.output / "big").mkdir(parents=True, exist_ok=True)
    (args.output / "outlines").mkdir(parents=True, exist_ok=True)

    print(f"遗物输入: {args.input}")
    print(f"遗物输出: {args.output}")
    print(f"自动轮廓: {OUTLINE_WIDTH}px 主题色外环")
    print(f"共 {len(sources)} 张\n")

    for source in sources:
        process_relic(source, args.output)

    print("\n完成!")


if __name__ == "__main__":
    main()

#!/usr/bin/env bash
set -euo pipefail

GODOT_VERSION="${1:-4.5.1}"

echo "==> Setting up Godot Mono ${GODOT_VERSION}..."

# 1. 下载并安装 Godot Mono Headless 编译器
wget -q "https://github.com/godotengine/godot/releases/download/${GODOT_VERSION}-stable/Godot_v${GODOT_VERSION}-stable_mono_linux_x86_64.zip"
unzip -q "Godot_v${GODOT_VERSION}-stable_mono_linux_x86_64.zip"
mv "Godot_v${GODOT_VERSION}-stable_mono_linux_x86_64/Godot_v${GODOT_VERSION}-stable_mono_linux.x86_64" godot
mv "Godot_v${GODOT_VERSION}-stable_mono_linux_x86_64/GodotSharp" .

# 2. 下载并安装导出模板（导出 PCK 所需）
wget -q "https://github.com/godotengine/godot/releases/download/${GODOT_VERSION}-stable/Godot_v${GODOT_VERSION}-stable_mono_export_templates.tpz"
unzip -q "Godot_v${GODOT_VERSION}-stable_mono_export_templates.tpz" -d /tmp/templates
mkdir -p "$HOME/.local/share/godot/export_templates/${GODOT_VERSION}.stable.mono"
mv /tmp/templates/templates/* "$HOME/.local/share/godot/export_templates/${GODOT_VERSION}.stable.mono/"

# 3. 将当前目录下的 godot 添加到 GITHUB_PATH
if [ -n "${GITHUB_PATH:-}" ]; then
  echo "$(pwd)" >> "$GITHUB_PATH"
fi

echo "==> Godot Mono ${GODOT_VERSION} setup completed."

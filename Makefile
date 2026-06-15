# STS2-Balance-MOD Makefile
# 统一的构建、发布、开发命令入口

.PHONY: help build clean rebuild release release-build release-upload release-json \
        image-gen image-cards image-powers image-relics image-rest-site \
        install-tools docs changelog

# ======================== 帮助文档 ========================
help:
	@echo "STS2-Balance-MOD 命令列表"
	@echo ""
	@echo "【构建相关】"
	@echo "  make build           - 编译项目"
	@echo "  make clean           - 清理编译输出"
	@echo "  make rebuild         - 清理并重新编译"
	@echo ""
	@echo "【发布相关】"
	@echo "  make release VERSION=0.0.X          - 完整发布流程 (更新版本+构建+上传)"
	@echo "  make release-json VERSION=0.0.X     - 仅更新 JSON 版本号"
	@echo "  make release-build VERSION=0.0.X    - 仅构建发布包"
	@echo "  make release-upload VERSION=0.0.X   - 仅上传发布包"
	@echo ""
	@echo "【图片生成】"
	@echo "  make install-tools                  - 安装 Python 图片生成工具"
	@echo "  make image-gen                      - 生成所有图片 (卡牌/遗物/能力)"
	@echo "  make image-cards                    - 生成卡牌图片"
	@echo "  make image-powers                   - 生成能力图片"
	@echo "  make image-relics                   - 生成遗物图片"
	@echo "  make image-rest-site                - 生成火堆选项图片"
	@echo ""
	@echo "【文档相关】"
	@echo "  make docs                           - 生成/更新文档"
	@echo "  make changelog                      - 提取变更日志"
	@echo ""

# ======================== 构建相关 ========================
build:
	dotnet build

clean:
	dotnet clean

rebuild: clean build

# ======================== 发布相关 ========================
# 完整发布：更新版本 + 构建 + 上传
release:
	@if [ -z "$(VERSION)" ]; then \
		echo "错误：请指定 VERSION"; \
		echo "用法: make release VERSION=0.0.X"; \
		exit 1; \
	fi
	powershell -NoProfile -ExecutionPolicy Bypass -File Hooks/release.ps1 -Version $(VERSION) -All

# 仅更新版本号
release-json:
	@if [ -z "$(VERSION)" ]; then \
		echo "错误：请指定 VERSION"; \
		exit 1; \
	fi
	powershell -NoProfile -ExecutionPolicy Bypass -File Hooks/release.ps1 -Version $(VERSION) -UpdateJson

# 仅构建发布包
release-build:
	@if [ -z "$(VERSION)" ]; then \
		echo "错误：请指定 VERSION"; \
		exit 1; \
	fi
	powershell -NoProfile -ExecutionPolicy Bypass -File Hooks/release.ps1 -Version $(VERSION) -Build

# 仅上传发布包
release-upload:
	@if [ -z "$(VERSION)" ]; then \
		echo "错误：请指定 VERSION"; \
		exit 1; \
	fi
	powershell -NoProfile -ExecutionPolicy Bypass -File Hooks/release.ps1 -Version $(VERSION) -Upload

# ======================== 图片生成相关 ========================
# 安装 Python 图片生成工具
install-tools:
	cd image_gen && pip install -e .

# 生成所有图片
image-gen: image-cards image-powers image-relics image-rest-site

# 生成卡牌图片
image-cards:
	python -m cards

# 生成能力图片
image-powers:
	python -m powers

# 生成遗物图片
image-relics:
	python -m relics

# 生成火堆选项图片
image-rest-site:
	python -m rest_site_options

# ======================== 文档相关 ========================
# 生成/更新文档（如需要）
docs:
	@echo "文档已在 docs/ 目录中，请手动维护"

# 提取变更日志
changelog:
	powershell -NoProfile -ExecutionPolicy Bypass -File Hooks/extract-changelog.ps1


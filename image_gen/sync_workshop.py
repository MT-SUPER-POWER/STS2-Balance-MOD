import json
from pathlib import Path
import md2steam

def main():
    root_dir = Path(__file__).resolve().parent.parent
    workspace_json = root_dir / "workshop" / "workshop.json"

    if not workspace_json.exists():
        print(f"[ERROR] Workshop config file not found: {workspace_json}")
        return

    # 1. 简要说明 (Bilingual Description)
    desc_markdown = """[ZH]
本 Mod 旨在优化《杀戮尖塔 2》的游戏平衡性与游玩体验，针对部分卡牌、遗物、商店价格及随机事件进行了机制重构与数值调优。支持 BaseLib 原生配置菜单。

🌐 支持语言：简体中文、English、Italiano、Русский
📌 前置需求：需在创意工坊订阅并启用 BaseLib (>= 3.4.0)

[EN]
A balance and quality-of-life mod for Slay the Spire 2, tweaking cards, relics, shop prices, and events for an improved gameplay experience. Supports full BaseLib config menu.

🌐 Languages: English, Simplified Chinese, Italian, Russian
📌 Dependency: BaseLib (>= 3.4.0)

GitHub & Feedback: https://github.com/MT-SUPER-POWER/STS2-Balance-MOD"""

    # 使用三方库 md2steam 转换为 Steam BBCode
    description_bbcode = md2steam.markdown_to_steam_bbcode(desc_markdown)

    # 2. 读取 workshop.json 并更新完整配置
    with open(workspace_json, "r", encoding="utf-8-sig") as f:
        data = json.load(f)

    data["title"] = "STS2 Balance MOD | 《杀戮尖塔2》平衡调整 Mod"
    data["description"] = description_bbcode
    data["changeNote"] = """v0.1.8 版本更新日志：

【新增与设置】
- 接入 BaseLib Mod 配置页面，支持多语言（中/英/意/俄）持久化设置。
- 新增“启用感染棱柱重做”配置开关（默认开启，关闭恢复原版机制）。
- 新增“事件离开选项”配置开关（控制除虫者、科学怪人、药水的未来等事件）。

【卡牌与事件调整】
- 华丽收场 (Grand Finale)：调整升级效果，判定统一为抽牌堆卡牌数 ≤ X，升级改为少扣除 2 点费用。
- 禅意织者 (Zen Weaver)：下调删牌价格（删1张降至 75 金，删2张降至 150 金）。"""
    data["tags"] = [
        "Balance",
        "Cards",
        "Relics",
        "Events",
        "QoL",
        "Chinese",
        "English",
        "Italian",
        "Russian"
    ]
    data["dependencies"] = [3737335127]

    with open(workspace_json, "w", encoding="utf-8") as f:
        json.dump(data, f, ensure_ascii=False, indent=2)

    print(f"[OK] Successfully converted via md2steam and updated {workspace_json}")

if __name__ == "__main__":
    main()

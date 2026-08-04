import json
from pathlib import Path
import md2steam

def extract_latest_changelog(changelog_path: Path) -> str:
    """动态提取 CHANGELOG.md 中最新版本 (## vX.X.X) 的更新日志内容"""
    if not changelog_path.exists():
        return "暂无更新说明"

    content = changelog_path.read_text(encoding="utf-8")
    lines = content.splitlines()
    latest_lines = []
    in_latest_section = False

    for line in lines:
        if line.startswith("## v"):
            if not in_latest_section:
                in_latest_section = True
                latest_lines.append(line)
            else:
                # 遇到下一个版本标题，停止提取
                break
        elif in_latest_section:
            latest_lines.append(line)

    changelog_md = "\n".join(latest_lines).strip()
    return changelog_md if changelog_md else "暂无更新说明"

def main():
    root_dir = Path(__file__).resolve().parent.parent
    workspace_json = root_dir / "workshop" / "workshop.json"
    changelog_md_path = root_dir / "CHANGELOG.md"

    if not workspace_json.exists():
        print(f"[ERROR] Workshop config file not found: {workspace_json}")
        return

    # 1. 固定精炼主描述 (Fixed Concise Description + GitHub Link)
    desc_markdown = """【中文介绍】
《杀戮尖塔 2》平衡调整 Mod

本 Mod 旨在优化《杀戮尖塔 2》的游戏平衡性与游玩体验，针对部分卡牌、遗物、商店价格及随机事件进行了机制重构与数值调优。

📌 **前置需求**: 需在创意工坊订阅并启用 **BaseLib (>= 3.4.0)**
📖 **详细调整清单与 README**:
https://github.com/MT-SUPER-POWER/STS2-Balance-MOD

----------------------------------------

【English Description】
Slay the Spire 2 Balance MOD

This mod aims to optimize the balance and gameplay experience of Slay the Spire 2 by reworking mechanics and fine-tuning values for select cards, relics, shop prices, and random events.

📌 **Dependency**: Requires **BaseLib (>= 3.4.0)** from Steam Workshop
📖 **Detailed Changes & README**:
https://github.com/MT-SUPER-POWER/STS2-Balance-MOD"""

    description_bbcode = md2steam.markdown_to_steam_bbcode(desc_markdown)

    # 2. 动态从 CHANGELOG.md 提取最新版本日志填入 changeNote
    latest_change_note = extract_latest_changelog(changelog_md_path)

    # 3. 读取 Sts2BalanceMod.json 获取当前版本号
    mod_json_path = root_dir / "Sts2BalanceMod.json"
    version = "v0.1.0"
    if mod_json_path.exists():
        with open(mod_json_path, "r", encoding="utf-8-sig") as f:
            mod_meta = json.load(f)
            version = mod_meta.get("version", version)

    # 4. 读取 workshop.json 并更新配置
    with open(workspace_json, "r", encoding="utf-8-sig") as f:
        data = json.load(f)

    data["title"] = f"STS2 Balance MOD [{version}] | 《杀戮尖塔2》平衡调整 Mod"
    data["description"] = description_bbcode
    data["changeNote"] = latest_change_note
    data["tags"] = [
        "Balance"
    ]
    data["dependencies"] = [3737335127]
    data["minBranch"] = "public-beta"
    data["maxBranch"] = "public-beta"

    tmp_dir = root_dir / "workshop" / "tmp"
    tmp_dir.mkdir(parents=True, exist_ok=True)
    preview_json_path = tmp_dir / "preview.json"

    with open(workspace_json, "w", encoding="utf-8") as f:
        json.dump(data, f, ensure_ascii=False, indent=2)

    with open(preview_json_path, "w", encoding="utf-8") as f:
        json.dump(data, f, ensure_ascii=False, indent=2)

    print(f"[OK] Dynamic sync complete! Saved preview to {preview_json_path} and updated {workspace_json}")

if __name__ == "__main__":
    main()

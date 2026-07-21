#!/usr/bin/env python3
"""
STS2 Official News Sync Script
Fetches official news from the Steam API for Slay the Spire 2 (AppID: 2868840),
converts BBCode to Markdown, and writes original.md files under docs/official-news/.

Summary generation (summary.md) is intentionally left to the Agent so that
LLM intelligence — not fragile heuristics — extracts gameplay changes.

Usage:
    python sync_news.py                  # incremental: only new articles
    python sync_news.py --force          # overwrite all existing original.md files
    python sync_news.py --count 200      # fetch up to 200 articles (default: 200)
"""

import argparse
import datetime
import json
import re
import urllib.request
from pathlib import Path

# ─── Configuration ────────────────────────────────────────────────────────────

STEAM_APP_ID = 2868840
CLAN_CDN = "https://clan.fastly.steamstatic.com/images"
# maxlength=0 means unlimited — the default (300) silently truncates articles.
STEAM_NEWS_URL = (
    f"https://api.steampowered.com/ISteamNews/GetNewsForApp/v0002/"
    f"?appid={STEAM_APP_ID}&count={{count}}&maxlength=0&format=json"
)

DEFAULT_OUTPUT_DIR = r"docs\official-news"


# ─── Helpers ──────────────────────────────────────────────────────────────────

def slugify(text: str) -> str:
    """Filesystem-safe slug: lowercase, spaces→hyphens, keep alphanumeric + CJK."""
    text = text.lower()
    text = re.sub(r"[^\w\u4e00-\u9fff\u3040-\u30ff\u31f0-\u31ff]+", "-", text)
    text = re.sub(r"-+", "-", text).strip("-")
    return text or "untitled"


def convert_bbcode(bbcode: str) -> str:
    """Convert Steam BBCode to clean Markdown."""
    if not bbcode:
        return ""
    t = bbcode

    # Convert [img]{STEAM_CLAN_IMAGE} to real CDN image markdown FIRST
    t = re.sub(
        r"\[img\]\{STEAM_CLAN_IMAGE\}/(\d+)/(\S+?)\[/img\]",
        lambda m: f"![]({CLAN_CDN}/{m.group(1)}/{m.group(2)})",
        t, flags=re.IGNORECASE,
    )
    t = re.sub(
        r"\{STEAM_CLAN_IMAGE\}/(\d+)/(\S+)",
        lambda m: f"![]({CLAN_CDN}/{m.group(1)}/{m.group(2)})",
        t, flags=re.IGNORECASE,
    )
    t = re.sub(r"\[img\].*?\[/img\]", "", t, flags=re.IGNORECASE | re.DOTALL)

    # Process [url] BEFORE headings/bold so catch-all won't eat Markdown link text
    t = re.sub(r"\[url=(.*?)\](.*?)\[/url\]", r"[\2](\1)", t, flags=re.IGNORECASE | re.DOTALL)
    t = re.sub(r"\[url\](.*?)\[/url\]",       r"[\1](\1)", t, flags=re.IGNORECASE | re.DOTALL)

    # Headings
    for level in (1, 2, 3):
        tag = f"h{level}"
        hashes = "#" * level
        t = re.sub(rf"\[{tag}\](.*?)\[/{tag}\]", rf"{hashes} \1", t, flags=re.IGNORECASE | re.DOTALL)

    # Inline formatting
    t = re.sub(r"\[b\](.*?)\[/b\]",           r"**\1**",  t, flags=re.IGNORECASE | re.DOTALL)
    t = re.sub(r"\[i\](.*?)\[/i\]",           r"*\1*",    t, flags=re.IGNORECASE | re.DOTALL)
    t = re.sub(r"\[u\](.*?)\[/u\]",           r"\1",      t, flags=re.IGNORECASE | re.DOTALL)
    t = re.sub(r"\[strike\](.*?)\[/strike\]", r"~~\1~~",  t, flags=re.IGNORECASE | re.DOTALL)

    # Inline formatting

    # Lists
    t = re.sub(r"\[list\]|\[/list\]", "", t, flags=re.IGNORECASE)
    t = re.sub(r"\[\*\]", "- ",             t, flags=re.IGNORECASE)

    # Code / quote
    t = re.sub(r"\[code\](.*?)\[/code\]",   r"`\1`",  t, flags=re.IGNORECASE | re.DOTALL)
    t = re.sub(r"\[quote\](.*?)\[/quote\]", r"> \1",  t, flags=re.IGNORECASE | re.DOTALL)

    # Strip remaining unrecognized BBCode tags (tight pattern: no spaces in tag name)
    # e.g. [gold], [/gold], [previewyoutube=id], but NOT [link text] from Markdown
    t = re.sub(r"\[/?[a-zA-Z_-]+(?:=[^\]]+)?\]", "", t)

    # Collapse excessive blank lines (max 2 consecutive)
    lines = [line.rstrip() for line in t.splitlines()]
    cleaned: list[str] = []
    blank_run = 0
    for line in lines:
        if not line:
            blank_run += 1
            if blank_run <= 2:
                cleaned.append(line)
        else:
            blank_run = 0
            cleaned.append(line)

    return "\n".join(cleaned).strip()


def extract_version(title: str, body: str) -> str:
    """Try to pull a version string from the title or article body."""
    combined = f"{title}\n{body}"
    patterns = [
        r"\b(v\d+\.\d+(?:\.\d+)?)\b",
        r"\bPatch\s+(\d+\.\d+(?:\.\d+)?)\b",
        r"\b(0\.\d+(?:\.\d+)?)\b",
    ]
    for pat in patterns:
        m = re.search(pat, combined, re.IGNORECASE)
        if m:
            v = m.group(1).strip()
            return v if re.match(r"v\d|[Pp]atch", v) else f"v{v}"
    return ""


def fetch_news(count: int) -> list[dict]:
    url = STEAM_NEWS_URL.format(count=count)
    req = urllib.request.Request(url, headers={"User-Agent": "Mozilla/5.0"})
    with urllib.request.urlopen(req, timeout=30) as resp:
        data = json.loads(resp.read().decode("utf-8"))
    return data.get("appnews", {}).get("newsitems", [])


def load_existing_dates(output_dir: Path) -> set[str]:
    """Return the set of article dates already present in index.md."""
    index_file = output_dir / "index.md"
    if not index_file.exists():
        return set()
    dates: set[str] = set()
    for line in index_file.read_text(encoding="utf-8").splitlines():
        # Table rows start with "| YYYY-MM-DD"
        m = re.match(r"\|\s*(\d{4}-\d{2}-\d{2})\s*\|", line)
        if m:
            dates.add(m.group(1))
    return dates


def write_original(article_dir: Path, date_str: str, title: str,
                   url: str, author: str, version: str, body_md: str) -> None:
    original = article_dir / "original.md"
    escaped_title = title.replace('"', '\\"')
    frontmatter = (
        "---\n"
        f'date: "{date_str}"\n'
        f'version: "{version}"\n'
        f'title: "{escaped_title}"\n'
        f'source: "{url}"\n'
        f'author: "{author}"\n'
        "---\n\n"
    )
    original.write_text(frontmatter + body_md + "\n", encoding="utf-8")


# ─── Main ─────────────────────────────────────────────────────────────────────

def main() -> None:
    parser = argparse.ArgumentParser(
        description="Sync STS2 Steam news → docs/official-news/ (original.md only)."
    )
    parser.add_argument("--count",      type=int, default=200,           help="Max articles to fetch")
    parser.add_argument("--output-dir", type=str, default=DEFAULT_OUTPUT_DIR, help="Output directory")
    parser.add_argument("--force",      action="store_true",             help="Overwrite existing original.md files")
    args = parser.parse_args()

    output_dir = Path(args.output_dir)
    output_dir.mkdir(parents=True, exist_ok=True)

    existing_dates = set() if args.force else load_existing_dates(output_dir)
    if existing_dates:
        print(f"Incremental mode: skipping {len(existing_dates)} already-archived date(s).")
    else:
        print("Full import mode: fetching all articles.")

    print(f"Fetching up to {args.count} articles from Steam API...", flush=True)
    items = fetch_news(args.count)
    print(f"Fetched {len(items)} items.", flush=True)

    new_articles: list[dict] = []

    for idx, item in enumerate(items, 1):
        title  = item.get("title", "Untitled").strip()
        url    = item.get("url", "")
        author = item.get("author", "").strip()
        ts     = item.get("date", 0)

        if "steam_community_announcements" not in url:
            print(f"[{idx}/{len(items)}] SKIP (third-party): {title[:50]}")
            continue

        dt       = datetime.datetime.fromtimestamp(ts, tz=datetime.timezone.utc)
        date_str = dt.strftime("%Y-%m-%d")

        if date_str in existing_dates and not args.force:
            print(f"[{idx}/{len(items)}] SKIP {date_str}: {title[:50]}")
            continue

        print(f"[{idx}/{len(items)}] Archiving {date_str}: {title[:50]}...", flush=True)

        slug        = slugify(title)
        folder_name = f"{date_str}-{slug}"
        article_dir = output_dir / folder_name
        article_dir.mkdir(parents=True, exist_ok=True)

        body_md = convert_bbcode(item.get("contents", ""))
        version = extract_version(title, body_md)

        original_path = article_dir / "original.md"
        if not original_path.exists() or args.force:
            write_original(article_dir, date_str, title, url, author, version, body_md)

        # Note: summary.md is intentionally NOT written here.
        # The Agent (sts2-news skill) reads original.md and writes summary.md
        # using LLM analysis, which is far more accurate than regex heuristics.

        new_articles.append({
            "date":    date_str,
            "version": version,
            "title":   title,
            "folder":  folder_name,
            "ts":      ts,
        })

    if not new_articles:
        print("Nothing new to archive.")
        return

    # Update index.md — prepend new rows (newest first)
    new_articles.sort(key=lambda x: (x["date"], x["ts"]), reverse=True)

    index_file   = output_dir / "index.md"
    header = (
        "# 杀戮尖塔 2 官方新闻存档\n\n"
        "| 日期 | 版本号 | 标题 | 档案链接 |\n"
        "| :--- | :--- | :--- | :--- |\n"
    )

    new_rows = []
    for a in new_articles:
        t   = a["title"].replace("|", "\\|")
        v   = a["version"] if a["version"] else "-"
        lnk = f"[查看原文](./{a['folder']}/original.md)"
        new_rows.append(f"| {a['date']} | {v} | {t} | {lnk} |")

    # Read existing rows (skip header lines)
    existing_rows: list[str] = []
    if index_file.exists():
        for line in index_file.read_text(encoding="utf-8").splitlines():
            if line.startswith("| ") and not line.startswith("| 日期") and not line.startswith("| :"):
                existing_rows.append(line)

    all_rows = new_rows + existing_rows
    index_file.write_text(header + "\n".join(all_rows) + "\n", encoding="utf-8")

    print(f"\nDone. Archived {len(new_articles)} new article(s).")
    print(f"Next step: run the sts2-news Agent skill to generate summary.md for each new folder.")


if __name__ == "__main__":
    main()

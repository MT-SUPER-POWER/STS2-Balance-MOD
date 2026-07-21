---
name: sts2-news
description: Maintains and queries an offline archive of STS2 official news articles from Steam. Use when the user wants to sync/update official news, view the news index table, or search for changes to a specific card, relic, power, or mechanic across all articles.
---

Maintain and query the local **archive** in `docs/official-news/`. Every official article lands in a dedicated folder with two files: the raw original and an Agent-extracted game-change summary.

## Archive layout

```
docs/official-news/
├── index.md                              ← speed-read entry point (newest first)
└── YYYY-MM-DD-{title-slug}/
    ├── original.md                       ← full article, BBCode → Markdown
    └── summary.md                        ← game-change extract; "本期无游戏数值改动" if none
```

### index.md columns

`日期 | 版本号 | 标题 | 摘要 | 档案链接` — sorted newest-first; 摘要 is one sentence max.

### original.md frontmatter

```yaml
---
date: YYYY-MM-DD
version: "v0.x"     # blank if article doesn't state one
title: "..."
source: https://...
author: ...
---
```

### summary.md

```markdown
## 游戏改动摘要

- **卡牌** 吞噬暗影 [Devour Shadow]：费用 1→2
- **遗物** 日晷 [Sundial]：触发条件修改
- **能力** 力量 [Strength]：初始值调整
```

Name format is `中文 [English]`. Look up translations from the game's localization files at `D:\Game\Sts2Code\localization`. Pattern: key `{ID}.title` in `eng/{file}.json` paired with the same key in `zhs/{file}.json`. Cover `cards.json`, `relics.json`, and `powers.json`. If a Chinese name is not found, keep English only.

Category prefixes: `**卡牌**`, `**遗物**`, `**能力**`, `**机制**`, `**其他**`.  
If the article contains no gameplay-affecting changes, write a single line: `本期无游戏数值改动`.

## Steam API

```
https://api.steampowered.com/ISteamNews/GetNewsForApp/v0002/?appid=2868840&count=50&format=json
```

No API key required. Key response fields per item: `gid`, `title`, `url`, `author`, `date` (Unix timestamp → convert to `YYYY-MM-DD`), `contents` (Steam BBCode).

### BBCode conversion rules

| BBCode | Markdown |
|--------|----------|
| `[h1]…[/h1]` | `# …` |
| `[h2]…[/h2]` | `## …` |
| `[h3]…[/h3]` | `### …` |
| `[b]…[/b]` | `**…**` |
| `[i]…[/i]` | `*…*` |
| `[url=X]Y[/url]` | `[Y](X)` |
| `[img]{STEAM_CLAN_IMAGE}/…[/img]` | strip entirely |
| `{STEAM_CLAN_IMAGE}/…` (bare) | strip entirely |

## Branches

### Sync

Triggered by: "同步", "更新官方新闻", "拉取推文", or when the archive is empty.

1. Read `docs/official-news/index.md`. Find the latest archived `date`. If the file is missing or has no data rows → this is a full import; treat the cutoff as "all articles".
2. Fetch the Steam API with `count=50`. For a full import, fetch again with `count=200` to reach all historical articles.
3. For each article whose date is newer than the cutoff (all articles on first run), process newest-first:
   - **a. Create folder** `docs/official-news/YYYY-MM-DD-{slug}/` where slug = lowercase title, spaces→hyphens, strip special chars (keep alphanumeric, hyphens, CJK characters).
   - **b. Write `original.md`**: populate frontmatter (extract version number from article body using patterns like `v0.x`, `Patch x.y`, `0.x.x`; leave blank if none found). Convert BBCode body to Markdown using the rules above.
   - **c. Translate names**: grep `D:\Game\Sts2Code\localization\eng\cards.json`, `relics.json`, and `powers.json` for all `.title` values. For each English name found verbatim in the article, look up its Chinese equivalent in `zhs\`. Build a replacement map and apply it to `summary.md` entries.
   - **d. Write `summary.md`**: read the article body and extract only gameplay-affecting changes — card stat changes, relic effect changes, power description changes, mechanic rule changes. Format as a bullet list with the category prefixes above. Write `本期无游戏数值改动` if none found.
4. Prepend new rows to `index.md` (newest-first). The 摘要 column = first meaningful sentence from `summary.md`, or "本期无游戏数值改动" if that's all it says.

**Completion criterion**: every article not yet in `index.md` has a folder containing both `original.md` and `summary.md`, and `index.md` has been updated with one row per new article.

---

### Show

Triggered by: "看看最新动态", "列出推文", "展示新闻", "官方更新了什么".

Read `docs/official-news/index.md` and render the table. State how many articles are archived and the date of the most recent one.

---

### Query

Triggered by: questions about a specific card, relic, power, or mechanic — e.g. "某卡被改过吗", "查查遗物X的历史改动", "搜索燃烧契约".

1. Collect all `docs/official-news/*/summary.md` files.
2. Search for the query term across all summaries, matching both Chinese and English variants.
3. For each match present results as a timeline entry:
   ```
   • YYYY-MM-DD「Article Title」
     改动内容：…
     → [查看原文](../YYYY-MM-DD-slug/original.md)
   ```
4. If no matches: reply `未找到"<query>"的相关记录`.

**Completion criterion**: all `summary.md` files scanned; result lists every match with date and source link, or explicitly states no record found.

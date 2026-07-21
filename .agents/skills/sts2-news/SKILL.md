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

`日期 | 版本号 | 标题 | 档案链接` — sorted newest-first.

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

`summary.md` 全部用**中文**撰写：名称用 `中文 [English]` 双语格式，改动描述也翻译成中文，方便直接阅读。`original.md` 保持英文原文不动。

```markdown
## 游戏改动摘要

- **卡牌** 吞噬暗影 [Devour Shadow]：费用 1→2
- **遗物** 日晷 [Sundial]：触发条件改为「每打出 5 张牌时触发」
- **能力** 魔像之心 [Juggernaut]：基础伤害 5→7
- **机制** 能量上限现在在多人模式中正确共享
```

名称格式：`中文 [English]`。从 `D:\Game\Sts2Code\localization` 查翻译，规律：`{ID}.title` 在 `eng/{file}.json` 对应 `zhs/{file}.json` 同一 key。覆盖 `cards.json`、`relics.json`、`powers.json`。找不到中文时保留英文。

分类前缀：`**卡牌**`、`**遗物**`、`**能力**`、`**机制**`、`**其他**`。  
无游戏数值改动时，写一行：`本期无游戏数值改动`。

## Steam API

```
https://api.steampowered.com/ISteamNews/GetNewsForApp/v0002/?appid=2868840&count=200&maxlength=0&format=json
```

> **`maxlength=0` is required.** The Steam API defaults to 300 characters — omitting it silently truncates every article body.

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
| `[img]{STEAM_CLAN_IMAGE}/…[/img]` | `![](https://clan.fastly.steamstatic.com/images/…)` |
| `{STEAM_CLAN_IMAGE}/…` (bare) | `![](https://clan.fastly.steamstatic.com/images/…)` |

*Note: The script automatically filters out third-party media articles (e.g., PCGamesN) and only archives official `steam_community_announcements`.*

## Branches

### Sync

Triggered by: "同步", "更新官方新闻", "拉取推文", or when the archive is empty.

Sync is a **two-step process** — the script handles mechanical fetching, the Agent handles intelligent extraction.

**Step 1 — Run the script** (fetches articles and writes `original.md`):

```powershell
# From repo root. First run = full import; subsequent runs = incremental.
python .agents/skills/sts2-news/scripts/sync_news.py

# Force re-download everything:
python .agents/skills/sts2-news/scripts/sync_news.py --force
```

The script writes `original.md` for each new official article and automatically updates `index.md`.

**Step 2 — Generate `summary.md` for each new folder** (Agent task):

For every folder in `docs/official-news/` that has `original.md` but no `summary.md`:

1. Read `original.md`.
2. Identify all English card/relic/power names mentioned. Look up Chinese translations from `D:\Game\Sts2Code\localization`: key pattern is `{ID}.title` in `eng/{file}.json` → same key in `zhs/{file}.json`. Cover `cards.json`, `relics.json`, `powers.json`.
3. Extract gameplay-affecting changes (stat numbers, costs, effect descriptions, mechanic rules). Ignore art updates, community showcases, merch, and dev blogs.
4. Write `summary.md` **entirely in Chinese**: translate the change descriptions, use `中文 [English]` format for names, and category prefixes. Write `本期无游戏数值改动` if the article has no balance changes.

**Completion criterion**: every official article folder has both `original.md` and `summary.md`.

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

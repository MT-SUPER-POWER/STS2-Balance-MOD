#!/usr/bin/env node
import { readdir, readFile, stat } from "node:fs/promises";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { parseArgs } from "node:util";

const appId = 2868840;
const defaultWorkspace = fileURLToPath(new URL("../workshop", import.meta.url));
const languages = ["schinese", "english"];
const visibilityValues = new Map([
  ["public", 0],
  ["friends_only", 1],
  ["private", 2],
  ["unlisted", 3],
]);

const help = `Usage: pnpm workshop:publish [options]

  -w, --workspace <dir>    Workshop workspace (default: repository workshop/)
  -c, --content-only       Upload content/ and the change note (default)
  -m, --meta-only          Update metadata without uploading content/
      --full               Upload content and update metadata
      --lang <language>    schinese or english (default: both in metadata modes)
      --dry-run            Validate and preview without contacting Steam (default)
      --publish            Submit the previewed update to Steam
  -h, --help               Show this help
`;

async function readText(filename, fallback) {
  try {
    return (await readFile(filename, "utf8")).replace(/^\uFEFF/, "");
  } catch (error) {
    if (error.code === "ENOENT" && fallback !== undefined) return fallback;
    throw error;
  }
}

function checkedText(value, name, maxBytes = Infinity) {
  if (typeof value !== "string" || value.includes("\0")) {
    throw new Error(`${name} must be text without NUL characters.`);
  }
  const bytes = Buffer.byteLength(value);
  if (bytes > maxBytes) {
    throw new Error(`${name} is ${bytes} UTF-8 bytes (limit: ${maxBytes}).`);
  }
  return value;
}

async function readMetadata(workspace, config, selectedLanguage) {
  const options = selectedLanguage ? [selectedLanguage] : languages;
  const updates = [];
  for (const language of options) {
    const english = language === "english";
    const title =
      (english ? config.title_en : undefined) ??
      config.title ??
      "STS2 Balance MOD";
    const descriptionPath = path.join(
      workspace,
      english ? "WORKSHOP_DESCRIPTION_EN.md" : "WORKSHOP_DESCRIPTION.md",
    );
    const draft = await readText(descriptionPath, null);
    const description =
      draft ??
      (english
        ? (config.description_en ??
          (await readText(
            path.join(workspace, "WORKSHOP_DESCRIPTION.md"),
            config.description ?? "",
          )))
        : (config.description ?? ""));
    updates.push({
      language,
      title: checkedText(title, `${language} title`, 128),
      description: checkedText(description, `${language} description`, 8000),
    });
  }

  const shared = updates.at(-1);
  if (config.visibility !== undefined) {
    if (!visibilityValues.has(config.visibility)) {
      throw new Error(
        "visibility must be public, friends_only, private or unlisted.",
      );
    }
    shared.visibility = visibilityValues.get(config.visibility);
  }
  if (config.tags !== undefined) {
    if (!Array.isArray(config.tags)) throw new Error("tags must be an array.");
    const tags = config.tags.map((tag) =>
      checkedText(tag, "Workshop tag").trim(),
    );
    if (tags.some((tag) => !tag || tag.includes(","))) {
      throw new Error("Workshop tags must be nonempty and contain no commas.");
    }
    if (tags.length > 0) shared.tags = tags;
  }

  const previewPath = path.join(workspace, "image.png");
  let preview;
  try {
    preview = await stat(previewPath);
  } catch (error) {
    if (error.code !== "ENOENT") throw error;
  }
  if (preview) {
    if (!preview.isFile() || preview.size < 16 || preview.size >= 1024 * 1024) {
      throw new Error(
        `Workshop preview must be a file from 16 bytes to less than 1 MiB: ${previewPath}`,
      );
    }
    shared.previewPath = previewPath;
  }
  return updates;
}

async function makePlan(workspace, mode, selectedLanguage) {
  const idPath = path.join(workspace, "mod_id.txt");
  const idText = (await readText(idPath)).trim();
  const itemId = /^[0-9]{1,20}$/.test(idText) ? BigInt(idText) : 0n;
  if (itemId === 0n || itemId > 0xffffffffffffffffn) {
    throw new Error(`Invalid Workshop item ID in ${idPath}.`);
  }

  const configPath = path.join(workspace, "workshop.json");
  const configText = await readText(configPath);
  let config;
  try {
    config = JSON.parse(configText);
  } catch (error) {
    throw new Error(`Invalid JSON in ${configPath}: ${error.message}`);
  }
  if (!config || typeof config !== "object" || Array.isArray(config)) {
    throw new Error(`Expected an object in ${configPath}.`);
  }

  const updates =
    mode === "content"
      ? [{}]
      : await readMetadata(workspace, config, selectedLanguage);
  const finalUpdate = updates.at(-1);
  if (mode !== "metadata") {
    const contentPath = path.join(workspace, "content");
    const entries = await readdir(contentPath, {
      withFileTypes: true,
      recursive: true,
    });
    if (!entries.some((entry) => entry.isFile())) {
      throw new Error(`Workshop content folder is empty: ${contentPath}`);
    }
    finalUpdate.contentPath = contentPath;
  }
  finalUpdate.changeNote = checkedText(config.changeNote ?? "", "changeNote");
  return { appId, itemId, updates };
}

function printPreview({ itemId, updates }, mode) {
  console.log(
    `Workshop item: https://steamcommunity.com/sharedfiles/filedetails/?id=${itemId}`,
  );
  console.log(`Mode: ${mode} | Steam updates: ${updates.length}`);
  for (const update of updates) {
    if (update.language) {
      console.log(`\n${update.language} title: ${update.title}`);
      console.log(
        `Description (${Buffer.byteLength(update.description)} UTF-8 bytes):\n${update.description}`,
      );
    }
    if (update.contentPath) console.log(`\nContent: ${update.contentPath}`);
    if (update.previewPath) console.log(`Preview: ${update.previewPath}`);
    if (update.tags) console.log(`Tags: ${update.tags.join(", ")}`);
    if (update.visibility !== undefined) {
      const name = [...visibilityValues].find(
        ([, value]) => value === update.visibility,
      )[0];
      console.log(`Visibility: ${name}`);
    }
    if (update.changeNote !== undefined)
      console.log(`Change note: ${update.changeNote || "(empty)"}`);
  }
}

async function main() {
  const { values } = parseArgs({
    options: {
      workspace: { type: "string", short: "w" },
      "content-only": { type: "boolean", short: "c" },
      "meta-only": { type: "boolean", short: "m" },
      full: { type: "boolean" },
      lang: { type: "string" },
      "dry-run": { type: "boolean" },
      publish: { type: "boolean" },
      help: { type: "boolean", short: "h" },
    },
  });
  if (values.help) {
    console.log(help);
    return;
  }
  if (
    [values["content-only"], values["meta-only"], values.full].filter(Boolean)
      .length > 1
  ) {
    throw new Error(
      "Choose only one of --content-only, --meta-only and --full.",
    );
  }
  if (values["dry-run"] && values.publish)
    throw new Error("Choose either --dry-run or --publish.");
  const mode = values["meta-only"]
    ? "metadata"
    : values.full
      ? "full"
      : "content";
  if (
    values.lang !== undefined &&
    (mode === "content" || !languages.includes(values.lang))
  ) {
    throw new Error(
      "--lang must be schinese or english and requires --meta-only or --full.",
    );
  }

  const workspace = path.resolve(values.workspace ?? defaultWorkspace);
  const plan = await makePlan(workspace, mode, values.lang);
  printPreview(plan, mode);
  if (!values.publish) {
    console.log("\nDry run complete. Use --publish to submit this update.");
    return;
  }
  const { publishUpdates } = await import("./workshop-steam.mjs");
  await publishUpdates(plan);
  console.log(
    `Workshop item updated: https://steamcommunity.com/sharedfiles/filedetails/?id=${plan.itemId}`,
  );
}

try {
  await main();
} catch (error) {
  console.error(`[Workshop] ${error.message}`);
  process.exitCode = 1;
}

#!/usr/bin/env bun
/**
 * Synchronize and bump versions across STS2-Balance-MOD project files.
 *
 * Usage:
 *   pnpm bump                # Defaults to bumping patch (e.g. 0.4.0 -> 0.4.1)
 *   pnpm bump patch          # Bump patch version
 *   pnpm bump minor          # Bump minor version (e.g. 0.4.0 -> 0.5.0)
 *   pnpm bump major          # Bump major version (e.g. 0.4.0 -> 1.0.0)
 *   pnpm bump 0.5.0          # Set exact version
 *   pnpm bump v0.5.0         # Set exact version (strips leading 'v')
 */

import { existsSync, readFileSync, writeFileSync } from "node:fs";
import path from "node:path";

const workspaceRoot = process.cwd();

const MOD_CONFIG_FILE = "Sts2BalanceMod.json";
const WORKSHOP_CONFIG_FILE = "workshop/workshop.json";
const WORKSHOP_CONTENT_CONFIG_FILE = "workshop/content/Sts2BalanceMod.json";
const PACKAGE_JSON_FILE = "package.json";
const CHANGELOG_FILE = "CHANGELOG.md";

function readCurrentVersion(): string {
  const modJsonPath = path.join(workspaceRoot, MOD_CONFIG_FILE);
  if (existsSync(modJsonPath)) {
    try {
      const content = JSON.parse(readFileSync(modJsonPath, "utf8"));
      if (content.version) {
        return content.version.replace(/^v/, "").trim();
      }
    } catch {
      // fallback
    }
  }

  const pkgPath = path.join(workspaceRoot, PACKAGE_JSON_FILE);
  if (existsSync(pkgPath)) {
    try {
      const content = JSON.parse(readFileSync(pkgPath, "utf8"));
      if (content.version) {
        return content.version.replace(/^v/, "").trim();
      }
    } catch {
      // fallback
    }
  }

  return "0.1.0";
}

function parseSemver(v: string): [number, number, number, string] {
  const cleaned = v.replace(/^v/, "").trim();
  const match = cleaned.match(/^(\d+)\.(\d+)\.(\d+)(?:-([0-9A-Za-z.-]+))?$/);
  if (!match) {
    throw new Error(`Invalid semver version: "${v}". Expected format: X.Y.Z or X.Y.Z-tag`);
  }
  return [Number(match[1]), Number(match[2]), Number(match[3]), match[4] || ""];
}

function calculateNextVersion(current: string, bumpTypeOrVersion: string): string {
  const [major, minor, patch] = parseSemver(current);
  const target = bumpTypeOrVersion.toLowerCase().trim();

  switch (target) {
    case "patch":
      return `${major}.${minor}.${patch + 1}`;
    case "minor":
      return `${major}.${minor + 1}.0`;
    case "major":
      return `${major + 1}.0.0`;
    default: {
      const cleaned = bumpTypeOrVersion.replace(/^v/, "").trim();
      parseSemver(cleaned); // validates format
      return cleaned;
    }
  }
}

export function bumpVersions(nextVersion: string): void {
  const semver = nextVersion.replace(/^v/, "").trim();
  parseSemver(semver);
  const taggedVersion = `v${semver}`;

  console.log(`\n🚀 Synchronizing versions to ${taggedVersion} (${semver})...\n`);

  // 1. Update Sts2BalanceMod.json
  const modJsonPath = path.join(workspaceRoot, MOD_CONFIG_FILE);
  if (existsSync(modJsonPath)) {
    const raw = readFileSync(modJsonPath, "utf8");
    const updated = raw.replace(/("version":\s*")[^"]+(")/, `$1${taggedVersion}$2`);
    writeFileSync(modJsonPath, updated, "utf8");
    console.log(`  ✓ Updated ${MOD_CONFIG_FILE} -> ${taggedVersion}`);
  }

  // 2. Update workshop/workshop.json (title and changeNote)
  const workshopPath = path.join(workspaceRoot, WORKSHOP_CONFIG_FILE);
  if (existsSync(workshopPath)) {
    const raw = readFileSync(workshopPath, "utf8");
    let updated = raw.replace(
      /("title":\s*"STS2 Balance MOD \[)[^\]]+(\]\s*\|\s*《杀戮尖塔2》平衡调整 Mod")/,
      `$1${taggedVersion}$2`,
    );
    updated = updated.replace(
      /("changeNote":\s*"https:\/\/github\.com\/MT-SUPER-POWER\/STS2-Balance-MOD\/releases\/tag\/)[^"]+(")/,
      `$1${taggedVersion}$2`,
    );
    writeFileSync(workshopPath, updated, "utf8");
    console.log(`  ✓ Updated ${WORKSHOP_CONFIG_FILE} -> ${taggedVersion}`);
  }

  // 3. Update workshop/content/Sts2BalanceMod.json if present
  const workshopContentModPath = path.join(workspaceRoot, WORKSHOP_CONTENT_CONFIG_FILE);
  if (existsSync(workshopContentModPath)) {
    const raw = readFileSync(workshopContentModPath, "utf8");
    const updated = raw.replace(/("version":\s*")[^"]+(")/, `$1${taggedVersion}$2`);
    writeFileSync(workshopContentModPath, updated, "utf8");
    console.log(`  ✓ Updated ${WORKSHOP_CONTENT_CONFIG_FILE} -> ${taggedVersion}`);
  }

  // 4. Update package.json
  const pkgPath = path.join(workspaceRoot, PACKAGE_JSON_FILE);
  if (existsSync(pkgPath)) {
    const raw = readFileSync(pkgPath, "utf8");
    let updated: string;
    if (/"version":\s*"[^"]+"/.test(raw)) {
      updated = raw.replace(/("version":\s*")[^"]+(")/, `$1${semver}$2`);
    } else {
      updated = raw.replace(/("name":\s*"[^"]+",)/, `$1\n  "version": "${semver}",`);
    }
    writeFileSync(pkgPath, updated, "utf8");
    console.log(`  ✓ Updated ${PACKAGE_JSON_FILE} -> ${semver}`);
  }

  // 5. Verify CHANGELOG.md entry
  const changelogPath = path.join(workspaceRoot, CHANGELOG_FILE);
  if (existsSync(changelogPath)) {
    const changelogContent = readFileSync(changelogPath, "utf8");
    const hasVersionHeading =
      changelogContent.includes(`## ${taggedVersion}`) ||
      changelogContent.includes(`# ${taggedVersion}`) ||
      changelogContent.includes(`## v${semver}`) ||
      changelogContent.includes(`## [${taggedVersion}]`);

    if (hasVersionHeading) {
      console.log(`  ✓ Found section for ${taggedVersion} in ${CHANGELOG_FILE}`);
    } else {
      console.warn(
        `  ⚠️  Warning: No "## ${taggedVersion}" section found in ${CHANGELOG_FILE}. Don't forget to document your changes!`,
      );
    }
  }

  console.log(`\n✨ Successfully bumped all target project files to ${taggedVersion}!\n`);
  console.log(`💡 Next recommended release steps:`);
  console.log(`   git add .`);
  console.log(`   git commit -m "chore: release ${taggedVersion}"`);
  console.log(`   git tag ${taggedVersion}`);
  console.log(`   git push origin <branch> ${taggedVersion}\n`);
}

// CLI Execution
if (import.meta.main) {
  const args = process.argv.slice(2).filter((arg) => !arg.startsWith("-"));

  if (process.argv.includes("--help") || process.argv.includes("-h")) {
    console.log(`
STS2 Balance MOD Version Bumper

Usage:
  pnpm bump                # Defaults to bumping patch (e.g. 0.4.0 -> 0.4.1)
  pnpm bump patch          # 0.4.0 -> 0.4.1
  pnpm bump minor          # 0.4.0 -> 0.5.0
  pnpm bump major          # 0.4.0 -> 1.0.0
  pnpm bump <version>      # Set explicit version (e.g. 0.4.1 or v0.4.1)

Target files:
  - Sts2BalanceMod.json
  - workshop/workshop.json
  - package.json
  - (optional) workshop/content/Sts2BalanceMod.json
`);
    process.exit(0);
  }

  const current = readCurrentVersion();
  const input = args[0] || "patch";
  const next = calculateNextVersion(current, input);

  console.log(`Current version: v${current}`);
  bumpVersions(next);
}

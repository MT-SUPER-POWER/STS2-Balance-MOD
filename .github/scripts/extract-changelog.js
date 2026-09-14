const fs = require('fs');
const path = require('path');

/**
 * Extracts the release notes for a given tag/version from CHANGELOG.md.
 * @param {{ context?: any, tagOverride?: string }} options
 * @returns {string}
 */
module.exports = function ({ context, tagOverride }) {
  const changelogPath = path.resolve(process.cwd(), 'CHANGELOG.md');
  if (!fs.existsSync(changelogPath)) {
    return '';
  }

  const content = fs.readFileSync(changelogPath, 'utf8');
  const tag = tagOverride || (context && context.ref ? context.ref.replace('refs/tags/', '') : '');
  const version = tag.replace(/^v/, '');

  const lines = content.split(/\r?\n/);
  let capturing = false;
  const changelogLines = [];
  const escapedVer = version.replace(/\./g, '\\.');
  const headerRegex = new RegExp(`^##\\s+\\[?v?${escapedVer}\\]?`);

  for (const line of lines) {
    if (headerRegex.test(line)) {
      capturing = true;
      continue;
    } else if (capturing && /^##\s+/.test(line)) {
      break;
    }
    if (capturing) {
      changelogLines.push(line);
    }
  }

  return changelogLines.join('\n').trim();
};

if (require.main === module) {
  const tag = process.argv[2] || '';
  const result = module.exports({ tagOverride: tag });
  process.stdout.write(result + '\n');
}

const fs = require('fs');
const path = require('path');

const tag = process.argv[2];
if (!tag) {
  console.error('Usage: node update-mod-version.js <version-tag>');
  process.exit(1);
}

const modJsonPath = path.resolve(process.cwd(), 'Sts2BalanceMod.json');
if (!fs.existsSync(modJsonPath)) {
  console.error(`Error: ${modJsonPath} does not exist.`);
  process.exit(1);
}

const modJson = JSON.parse(fs.readFileSync(modJsonPath, 'utf8'));
modJson.version = tag;
fs.writeFileSync(modJsonPath, JSON.stringify(modJson, null, 2) + '\n', 'utf8');
console.log(`Successfully updated Sts2BalanceMod.json version to: ${tag}`);

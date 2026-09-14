#!/usr/bin/env bash
set -euo pipefail

REF_NAME="${1:-}"
if [ -z "$REF_NAME" ]; then
  echo "Error: Missing release ref name (e.g. v0.3.8)" >&2
  exit 1
fi

echo "==> Verifying build outputs..."
ls -la dist/Sts2BalanceMod

for file in "dist/Sts2BalanceMod/Sts2BalanceMod.dll" "dist/Sts2BalanceMod/Sts2BalanceMod.json" "dist/Sts2BalanceMod/Sts2BalanceMod.pck"; do
  if [ ! -f "$file" ]; then
    echo "Error: Missing required build output: $file" >&2
    exit 1
  fi
done

echo "==> Compressing release archive..."
cd dist/Sts2BalanceMod
zip -r "../Sts2BalanceMod-${REF_NAME}.zip" .
cd ../..

ls -la dist/
echo "==> Successfully packaged dist/Sts2BalanceMod-${REF_NAME}.zip"

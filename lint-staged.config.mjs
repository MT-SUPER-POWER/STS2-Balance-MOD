export default
{
  "*.cs": ["dotnet format Sts2BalanceMod.sln --no-restore --include"],
  "*.md": ["prettier --write --ignore-unknown", "markdownlint-cli2 --fix"]
}

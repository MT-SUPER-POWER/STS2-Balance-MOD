export default
{
  "*.cs": ["dotnet format Sts2BalanceMod.sln --include"],
  "*.md": ["prettier --write --ignore-unknown", "markdownlint-cli2 --fix"]
}

using System;
using System.IO;

namespace Sts2BalanceMod.Tests;

public static class TestPathHelper
{
    private static readonly Lazy<string> RepositoryRootLazy = new(FindRepositoryRoot);

    public static string RepositoryRoot => RepositoryRootLazy.Value;

    public static string GetPath(params string[] relativePaths)
    {
        string combined = Path.Combine(relativePaths);
        return Path.GetFullPath(Path.Combine(RepositoryRoot, combined));
    }

    private static string FindRepositoryRoot()
    {
        string? current = AppContext.BaseDirectory;
        while (!string.IsNullOrEmpty(current))
        {
            if (File.Exists(Path.Combine(current, "Sts2BalanceMod.json")) ||
                File.Exists(Path.Combine(current, "Sts2BalanceMod.sln")))
            {
                return current;
            }

            current = Directory.GetParent(current)?.FullName;
        }

        throw new DirectoryNotFoundException("Could not locate repository root containing Sts2BalanceMod.json or Sts2BalanceMod.sln.");
    }
}

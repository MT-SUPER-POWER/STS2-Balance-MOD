using System.Reflection;
using HarmonyLib;

namespace Sts2BalanceMod.Sts2BalanceModCode.Settings;

[AttributeUsage(AttributeTargets.Class)]
internal sealed class BalancePatch(string id) : Attribute
{
  internal static string? FindSettingId(Type type)
  {
    for (Type? owner = type; owner != null; owner = owner.DeclaringType)
    {
      if (owner.GetCustomAttribute<BalancePatch>() is { } choice)
        return choice.Id;
    }
    return null;
  }

  internal static void Install(Harmony harmony, IEnumerable<(Type Type, string? Id)> patches)
  {
    foreach (var (type, id) in patches)
      if (id == null || BalanceModSettings.IsEnabled(id))
        harmony.CreateClassProcessor(type).Patch();
  }

  private string Id { get; } = id;
}

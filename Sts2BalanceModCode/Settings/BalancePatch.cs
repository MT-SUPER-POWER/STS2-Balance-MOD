using System.Reflection;
using HarmonyLib;

namespace Sts2BalanceMod.Sts2BalanceModCode.Settings;

[AttributeUsage(AttributeTargets.Class)]
internal sealed class BalancePatch(string id) : Attribute
{
  internal static void Install(Harmony harmony, Assembly assembly)
  {
    foreach (Type type in assembly.GetTypes())
    {
      Type? owner = type;
      BalancePatch? choice = null;
      while (owner != null && choice == null)
      {
        choice = owner.GetCustomAttribute<BalancePatch>();
        owner = owner.DeclaringType;
      }
      if (choice == null || BalanceModSettings.IsEnabled(choice.Id))
        harmony.CreateClassProcessor(type).Patch();
    }
  }

  private string Id { get; } = id;
}

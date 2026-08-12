using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using HarmonyLib;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Xunit;

namespace Sts2BalanceMod.Tests;

public class HarmonyPatchTests
{
    private static Assembly GetModAssembly() => typeof(BalanceCardTemplate).Assembly;

    [Fact]
    public void HarmonyPatchClasses_ShouldTargetValidTypeAndMethod()
    {
        var patchTypes = GetModAssembly().GetTypes()
            .Where(t => t.GetCustomAttributes<HarmonyPatch>(inherit: true).Any())
            .ToList();

        patchTypes.Should().NotBeEmpty("mod assembly should contain HarmonyPatch classes");

        foreach (var patchType in patchTypes)
        {
            var patchAttrs = patchType.GetCustomAttributes<HarmonyPatch>(inherit: true).ToList();
            var info = HarmonyMethod.Merge(patchAttrs.Select(a => a.info).ToList());

            if (info.declaringType != null)
            {
                info.declaringType.Should().NotBeNull($"patch '{patchType.FullName}' must target a non-null declaring type");

                if (!string.IsNullOrEmpty(info.methodName))
                {
                    string targetName = info.methodName;
                    if (info.methodType == MethodType.Getter)
                    {
                        targetName = $"get_{info.methodName}";
                    }
                    else if (info.methodType == MethodType.Setter)
                    {
                        targetName = $"set_{info.methodName}";
                    }

                    var methods = info.declaringType.GetMethods(
                        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
                    ).Where(m => m.Name == targetName || m.Name == info.methodName).ToList();

                    methods.Should().NotBeEmpty(
                        $"patch '{patchType.FullName}' targets method '{targetName}' on type '{info.declaringType.FullName}', which could not be found"
                    );
                }
            }
        }
    }
}

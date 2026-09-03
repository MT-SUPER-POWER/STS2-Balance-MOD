using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Relics;
using Sts2BalanceMod.Sts2BalanceModCode.Patches.Relics;
using Xunit;

namespace Sts2BalanceMod.Tests;

public class PandorasBoxPatchTests
{
    [Fact]
    public void PandorasBoxPatch_ShouldHaveHarmonyPatchAttributeTargetingAfterObtained()
    {
        var patchAttr = typeof(PandorasBoxPatch).GetCustomAttribute<HarmonyPatch>();
        patchAttr.Should().NotBeNull();
        patchAttr!.info.declaringType.Should().Be(typeof(PandorasBox));
        patchAttr.info.methodName.Should().Be(nameof(PandorasBox.AfterObtained));
    }

    [Fact]
    public void PandorasBoxPatch_Prefix_ShouldReturnFalseAndProvideTask()
    {
        var method = typeof(PandorasBoxPatch).GetMethod("Prefix", BindingFlags.Static | BindingFlags.Public);
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(bool));

        var parameters = method.GetParameters();
        parameters.Should().HaveCount(2);
        parameters[0].ParameterType.Should().Be(typeof(PandorasBox));
        parameters[1].ParameterType.Should().Be(typeof(Task).MakeByRefType());
    }
}

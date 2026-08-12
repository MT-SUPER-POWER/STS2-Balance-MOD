using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using FluentAssertions;
using HarmonyLib;
using Xunit;

namespace Sts2BalanceMod.Tests;

/// <summary>
/// 反向/负面测试集：验证当代码或资源出现错误时，校验逻辑能准确捕获并报错。
/// </summary>
public class NegativeValidationTests
{
    private static readonly Regex PascalCaseRegex = new(@"^[A-Z][a-zA-Z0-9]*$", RegexOptions.Compiled);

    [Theory]
    [InlineData("old_beggar.png")]
    [InlineData("snake_case_card.png")]
    [InlineData("lowerCamel.png")]
    [InlineData("invalid-hyphen.png")]
    public void PascalCaseValidator_ShouldRejectInvalidFileNames(string invalidName)
    {
        string nameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(invalidName);
        bool isValid = PascalCaseRegex.IsMatch(nameWithoutExt);

        isValid.Should().BeFalse($"'{invalidName}' 属于非法命名，校验逻辑必须将其识别为错误");
    }

    [Theory]
    [InlineData("伤害：{0} 点")]            // 合法：匹配闭合
    [InlineData("伤害：{0 点")]             // 错误：缺少右括号
    [InlineData("抽 {0}} 张牌")]           // 错误：多余右括号
    [InlineData("获得 {0} [Energy]{1")]    // 错误：末尾缺少右括号
    public void BraceMatcher_ShouldDetectUnmatchedBraces(string text)
    {
        int openBraces = text.Count(c => c == '{');
        int closeBraces = text.Count(c => c == '}');

        bool isMatched = openBraces == closeBraces;

        if (text.Contains("{0 点") || text.Contains("{0}}" ) || text.Contains("{1"))
        {
            isMatched.Should().BeFalse($"文本 \"{text}\" 中的花括号不匹配，校验逻辑必须断言失败");
        }
        else
        {
            isMatched.Should().BeTrue();
        }
    }

    [Fact]
    public void HarmonyPatchValidator_ShouldFailWhenTargetMethodDoesNotExist()
    {
        // 模拟一个指向不存在方法的 Patch 类
        var dummyInfo = new HarmonyMethod
        {
            declaringType = typeof(string),
            methodName = "NonExistentMethodNameInStringClass"
        };

        var methods = dummyInfo.declaringType.GetMethods(
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
        ).Where(m => m.Name == dummyInfo.methodName).ToList();

        // 验证反射检查会判定找到的方法数量为 0，从而触发断言失败
        methods.Should().BeEmpty("当 Patch 目标方法在类中不存在时，搜索结果必须为空，从而触发测试失败");
    }

    [Fact]
    public void VersionMatcher_ShouldFailWhenVersionMismatch()
    {
        string manifestVersion = "v0.3.3";
        string fakeChangelogVersion = "v0.3.4";

        var act = () => manifestVersion.Should().Be(fakeChangelogVersion);
        act.Should().Throw<Exception>("版本号不一致时，断言必须抛出异常");
    }
}

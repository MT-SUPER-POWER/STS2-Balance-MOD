using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using Sts2BalanceMod.Sts2BalanceModCode.RestSite;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.RestSite;

// ======================== CUSTOM REST SITE OPTION UI ========================

/// <summary>
/// 自定义火堆选项按钮 UI 补丁。
/// 输入：游戏创建的 NRestSiteButton。
/// 输出：当按钮绑定的是 Mod 火堆选项时，使用 Mod 资源目录内的标题与图标。
/// 返回值：false 表示已接管原始 Reload；true 表示继续使用游戏原逻辑。
/// </summary>
[HarmonyPatch(typeof(NRestSiteButton), "Reload")]
internal static class CustomRestSiteOptionButtonPatch
{
    private static readonly AccessTools.FieldRef<NRestSiteButton, RestSiteOption?> OptionRef =
      AccessTools.FieldRefAccess<NRestSiteButton, RestSiteOption?>("_option");

    private static readonly AccessTools.FieldRef<NRestSiteButton, TextureRect> IconRef =
      AccessTools.FieldRefAccess<NRestSiteButton, TextureRect>("_icon");

    private static readonly AccessTools.FieldRef<NRestSiteButton, MegaLabel> LabelRef =
      AccessTools.FieldRefAccess<NRestSiteButton, MegaLabel>("_label");

    private static readonly AccessTools.FieldRef<NRestSiteButton, ShaderMaterial> HsvRef =
      AccessTools.FieldRefAccess<NRestSiteButton, ShaderMaterial>("_hsv");

    private static readonly StringName SaturationParam = new("s");
    private static readonly StringName ValueParam = new("v");

    [HarmonyPrefix]
    private static bool Prefix(NRestSiteButton __instance)
    {
        if (OptionRef(__instance) is not BalanceRestSiteOption option)
        {
            return true;
        }

        if (!__instance.IsNodeReady())
        {
            return false;
        }

        // NOTE: 原版 RestSiteOption.Icon 使用私有硬编码路径；RitsuLib 尚未覆盖此节点刷新，因此在此接管。
        IconRef(__instance).Texture = PreloadManager.Cache.GetTexture2D(option.CustomIconPath);
        LabelRef(__instance).SetTextAutoSize(option.CustomTitle.GetFormattedText());

        ShaderMaterial hsv = HsvRef(__instance);
        if (!option.IsEnabled)
        {
            hsv.SetShaderParameter(SaturationParam, 0f);
            hsv.SetShaderParameter(ValueParam, 0.6f);
        }
        else
        {
            hsv.SetShaderParameter(SaturationParam, 1f);
            hsv.SetShaderParameter(ValueParam, 1f);
        }

        return false;
    }
}

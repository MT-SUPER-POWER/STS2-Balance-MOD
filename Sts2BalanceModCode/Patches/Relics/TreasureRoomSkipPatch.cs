using System.Reflection;
using HarmonyLib;
using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Runs;
using Sts2BalanceMod.Sts2BalanceModCode.Relics;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Relics;

[HarmonyPatch]
public static class TreasureRoomSkipPatch
{
    internal static bool SkipChestForCurseKey { get; private set; }
    private static bool _chestOpened;
    private static TextureRect? _skipIcon;

    internal static bool IsAfterChestOpen() => _chestOpened;

    private static bool IsCurseKeySinglePlayer()
    {
        if (typeof(RunManager)
            .GetProperty("State", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(RunManager.Instance) is not IRunState runState || runState.Players.Count != 1)
            return false;

        return runState.Players[0].GetRelic<CurseKey>() != null;
    }

    [HarmonyPatch(typeof(NTreasureRoom), "OpenChest")]
    [HarmonyPrefix]
    public static void NTreasureRoomOpenChestPrefix()
    {
        _chestOpened = true;
        SkipChestForCurseKey = false; // 玩家改变主意开了宝箱 → 诅咒正常触发

        RemoveSkipIcon();
    }

    private static void RemoveSkipIcon()
    {
        if (_skipIcon == null)
            return;
        if (GodotObject.IsInstanceValid(_skipIcon))
            _skipIcon.QueueFree();
        _skipIcon = null;
    }

    [HarmonyPatch(typeof(NTreasureRoom), "_Ready")]
    [HarmonyPostfix]
    public static void NTreasureRoomReadyPostfix(NTreasureRoom __instance)
    {
        SkipChestForCurseKey = false;
        _chestOpened = false;

        if (!IsCurseKeySinglePlayer())
            return;

        NProceedButton proceedButton = __instance.GetNodeOrNull<NProceedButton>("%ProceedButton");
        if (proceedButton == null)
            return;

        // NOTE: 如何获取文字翻译内容
        proceedButton.UpdateText(new LocString("gameplay_ui", "STS2_BALANCEMOD_SKIP_CHEST"));
        proceedButton.Enable();

        // NOTE: 在按钮左侧添加 <诅咒钥匙图标>
        RemoveSkipIcon();
        TextureRect image = proceedButton.GetNodeOrNull<TextureRect>("%Image");
        if (image != null && image.FindChild("CurseKeyIcon", recursive: false, owned: false) == null)
        {
            _skipIcon = new TextureRect
            {
                Name = "CurseKeyIcon",
                Texture = ResourceLoader.Load<Texture2D>(ModAssetPaths.RelicIcon("curse_key.png")),
                Size = new Vector2I(24, 24),
                Position = new Vector2(6, 34),
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
            };
            image.AddChild(_skipIcon);
        }
    }

    [HarmonyPatch(typeof(NTreasureRoom), "OnActiveScreenChanged")]
    [HarmonyPostfix]
    public static void NTreasureRoomOnActiveScreenChangedPostfix(NTreasureRoom __instance)
    {
        if (!IsCurseKeySinglePlayer())
            return;

        NProceedButton proceedButton = __instance.GetNodeOrNull<NProceedButton>("%ProceedButton");
        if (proceedButton == null)
            return;

        if (!_chestOpened)
        {
            // 开箱前：原生 OnActiveScreenChanged 在 _hasChestBeenOpened==false 时会 Disable 按钮
            // 这里重新 Enable 确保"跳过宝箱"可点击。仅当宝箱房是当前活动屏幕时。
            if (!ActiveScreenContext.Instance.IsCurrent(__instance))
                return;
            proceedButton.Enable();
            return;
        }

        // 开箱后：如果遗物已选（RelicCollection 已关闭）但文字仍是 Skip，强制恢复
        bool isRelicCollectionOpen = Traverse.Create(__instance)
        .Field("_isRelicCollectionOpen").GetValue<bool>();
        if (!isRelicCollectionOpen && proceedButton.IsSkip)
            proceedButton.UpdateText(NProceedButton.ProceedLoc);
    }

    [HarmonyPatch(typeof(NTreasureRoom), "OnProceedButtonPressed")]
    [HarmonyPrefix]
    public static bool NTreasureRoomOnProceedButtonPressedPrefix()
    {
        if (_chestOpened)
            return true; // 开箱后走原生流程

        // 未开箱 → 跳过宝箱（不隐藏宝箱，地图返回后仍可正常交互）
        SkipChestForCurseKey = true;

        NMapScreen.Instance?.SetTravelEnabled(enabled: true);
        TaskHelper.RunSafely(RunManager.Instance.ProceedFromTerminalRewardsScreen());

        return false; // 跳过原生 handler
    }
}

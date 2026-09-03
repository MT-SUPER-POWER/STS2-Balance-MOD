using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Runs;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Relics;

/// <summary>
/// RELIC-PANDORAS-BOX-01 — 先古遗物「潘多拉的魔盒」（Pandora's Box）变牌后增加异步确认环节，允许 SL 重新选择
/// Target: MegaCrit.Sts2.Core.Models.Relics.PandorasBox.AfterObtained.
/// Reason: 原版 AfterObtained 弹出变牌展示弹窗后未 await 等待玩家确认即瞬间结束，导致事件立刻标记 Done() 并触发 SaveRun 存盘写入磁盘；
/// 改为在弹出变牌展示界面（NSimpleCardsViewScreen）后异步等待玩家点击确认，确认前不结束 AfterObtained（不触发存盘），使得玩家可在此期间通过 SL 重新进入游戏做别的选择。
/// WARNING: Verified against D:\Game\Sts2Code\src\MegaCrit.Sts2.Core.Models.Relics\PandorasBox.cs.
/// </summary>
[HarmonyPatch(typeof(PandorasBox), nameof(PandorasBox.AfterObtained))]
public static class PandorasBoxPatch
{
    [HarmonyPrefix]
    public static bool Prefix(PandorasBox __instance, ref Task __result)
    {
        __result = CustomAfterObtained(__instance);
        return false;
    }

    private static async Task CustomAfterObtained(PandorasBox pandorasBox)
    {
        List<CardModel> source = PileType.Deck.GetPile(pandorasBox.Owner).Cards
            .Where(c => c != null && c.IsBasicStrikeOrDefend && c.IsRemovable).ToList();

        IEnumerable<CardTransformation> transformations = source.Select(c =>
            new CardTransformation(c, CardFactory.CreateRandomCardForTransform(c, isInCombat: false, pandorasBox.Owner.RunState.Rng.Niche)));

        List<CardPileAddResult> list = (await CardCmd.Transform(transformations, null, CardPreviewStyle.None)).ToList();

        if (list.Count > 0 && LocalContext.IsMe(pandorasBox.Owner))
        {
            LocString infoText = new LocString("relics", "PANDORAS_BOX.infoText");
            await ShowAndAwaitConfirmAsync(list, infoText);
        }
    }

    private static async Task ShowAndAwaitConfirmAsync(List<CardPileAddResult> list, LocString infoText)
    {
        NSimpleCardsViewScreen? screen = NSimpleCardsViewScreen.ShowScreen(list, infoText) as NSimpleCardsViewScreen;
        if (screen == null)
        {
            return;
        }

        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        void AttachConfirmHandler(NSimpleCardsViewScreen s)
        {
            NButton? confirmBtn = s.GetNodeOrNull<NButton>("ConfirmButton");
            if (confirmBtn != null)
            {
                confirmBtn.Connect(NClickableControl.SignalName.Released, Callable.From(() =>
                {
                    tcs.TrySetResult(true);
                }));
            }
        }

        AttachConfirmHandler(screen);

        // 处理暂停菜单（Esc）打开又恢复的情况：
        // 若玩家在确认前按 Esc 打开了暂停菜单并点击“继续游戏”（Resume），重新弹出卡牌展示界面供玩家点击确认
        void OnCapstoneClosed()
        {
            if (tcs.Task.IsCompleted)
            {
                return;
            }

            if (RunManager.Instance == null || !RunManager.Instance.IsInProgress)
            {
                tcs.TrySetCanceled();
                return;
            }

            // 如果当前没有 Capstone 打开（说明暂停菜单已关闭回到游戏）
            if (NCapstoneContainer.Instance?.CurrentCapstoneScreen == null)
            {
                NSimpleCardsViewScreen? newScreen = NSimpleCardsViewScreen.ShowScreen(list, infoText) as NSimpleCardsViewScreen;
                if (newScreen != null)
                {
                    AttachConfirmHandler(newScreen);
                }
            }
        }

        Callable capstoneClosedCallable = Callable.From(OnCapstoneClosed);
        if (NCapstoneContainer.Instance != null)
        {
            NCapstoneContainer.Instance.Connect(NCapstoneContainer.SignalName.CapstoneClosed, capstoneClosedCallable);
        }

        try
        {
            await tcs.Task;
        }
        catch (TaskCanceledException)
        {
            // 玩家退出游戏或局内取消，正常退出
        }
        finally
        {
            if (NCapstoneContainer.Instance != null && NCapstoneContainer.Instance.IsConnected(NCapstoneContainer.SignalName.CapstoneClosed, capstoneClosedCallable))
            {
                NCapstoneContainer.Instance.Disconnect(NCapstoneContainer.SignalName.CapstoneClosed, capstoneClosedCallable);
            }
        }
    }
}

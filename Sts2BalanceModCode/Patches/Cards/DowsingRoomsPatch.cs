using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Rooms;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// CARD-01 — 探寻 (Dowsing)
/// - 任务目标从进入 5 个 ? 房间调整为进入 4 个 ? 房间。
/// </summary>
public static class DowsingRoomsPatch
{
    private const int TargetMaxRooms = 4;

    [HarmonyPatch(typeof(Dowsing), "get_CanonicalVars")]
    public static class CanonicalVarsPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref IEnumerable<DynamicVar> __result)
        {
            __result = new DynamicVar[]
            {
                new DynamicVar("Rooms", TargetMaxRooms)
            };
            return false;
        }
    }

    [HarmonyPatch(typeof(Dowsing), nameof(Dowsing.RoomsEntered), MethodType.Setter)]
    public static class RoomsEnteredSetterPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Dowsing __instance, int value)
        {
            __instance.AssertMutable();
            Traverse.Create(__instance).Field("_roomsEntered").SetValue(value);
            __instance.DynamicVars["Rooms"].BaseValue = TargetMaxRooms - value;
            return false;
        }
    }

    [HarmonyPatch(typeof(Dowsing), nameof(Dowsing.AfterRoomEntered))]
    public static class AfterRoomEnteredPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Dowsing __instance, AbstractRoom room, ref Task __result)
        {
            __result = AfterRoomEntered(__instance, room);
            return false;
        }

        private static async Task AfterRoomEntered(Dowsing card, AbstractRoom room)
        {
            CardPile? pile = card.Pile;
            if (pile == null || pile.Type != PileType.Deck || card.Owner.RunState.CurrentRoomCount > 1)
            {
                return;
            }
            MapPoint? currentMapPoint = card.Owner.RunState.CurrentMapPoint;
            if (currentMapPoint != null && currentMapPoint.PointType == MapPointType.Unknown)
            {
                card.RoomsEntered++;
                if (card.RoomsEntered >= TargetMaxRooms)
                {
                    PlayerCmd.CompleteQuest(card);
                    await CardCmd.TransformTo<Abundance>(card);
                }
            }
        }
    }
}

using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Random;
using Sts2BalanceMod.Sts2BalanceModCode.Events;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Events;

/// <summary>
/// STS1-ACT-01 — 本批一代事件先简单注入到现有事件池，不规划新区域。
/// </summary>
[HarmonyPatch(typeof(ActModel), nameof(ActModel.GenerateRooms))]
public static class Sts1EventInjectionPatch
{
  private static readonly FieldInfo? RoomsField =
    typeof(ActModel).GetField("_rooms", BindingFlags.NonPublic | BindingFlags.Instance);

  [HarmonyPostfix]
  private static void Postfix(ActModel __instance, Rng rng)
  {
    if (RoomsField?.GetValue(__instance) is not RoomSet rooms)
      return;

    AddIfMissing(rooms, ModelDb.Event<TheDivineFountain>());
    AddIfMissing(rooms, ModelDb.Event<Cleric>());
    AddIfMissing(rooms, ModelDb.Event<CursedTome>());
    AddIfMissing(rooms, ModelDb.Event<Augmenter>());
    AddIfMissing(rooms, ModelDb.Event<MindBloom>());
    AddIfMissing(rooms, ModelDb.Event<WheelOfChange>());
    AddIfMissing(rooms, ModelDb.Event<TombOfLordRedMask>());
  }

  private static void AddIfMissing(RoomSet rooms, EventModel eventModel)
  {
    if (rooms.events.Any(e => e.Id == eventModel.Id))
      return;

    rooms.events.Add(eventModel);
  }
}

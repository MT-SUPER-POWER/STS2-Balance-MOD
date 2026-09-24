using Sts2BalanceMod.Sts2BalanceModCode.Settings;

int checks = 0;
void Check(bool value, string name)
{
  if (!value) throw new Exception(name);
  checks++;
}
void Reject(byte[] bytes, string name)
{
  try { BalanceConfigWire.Decode(bytes); }
  catch (InvalidDataException) { checks++; return; }
  throw new Exception(name);
}
var choices = new Dictionary<string, bool>();
BalanceCatalog.Migrate(choices, false, false, false);
Check(choices.Count == 78 && BalanceCatalog.All.Select(c => c.Id).Distinct().Count() == 78, "unique catalog");
Check(new[] { "E02", "E15", "E16", "M02", "E14" }.All(id => !choices[id]), "legacy false values survive");
choices["E15"] = true;
BalanceCatalog.Migrate(choices, false, false, false);
Check(choices["E15"], "migration must not overwrite edited child options");
var active = new Dictionary<string, bool>(choices);
choices["C03"] = false;
Check(BalanceCatalog.Effective(active, "C03") && BalanceCatalog.Pending(choices, active, "C03"), "saved changes must not mutate active snapshot");
choices["C03"] = true;
Check(!BalanceCatalog.Pending(choices, active, "C03"), "reverting clears pending");
choices["E06"] = false;
choices["E11"] = false;
Check(choices["R04"] && !BalanceCatalog.Effective(choices, "R04"), "red mask keeps saved selection while dependency pauses it");
Check(BalanceCatalog.Pending(choices, active, "R04"), "dependent state appears pending");
choices["E11"] = true;
Check(BalanceCatalog.Effective(choices, "R04"), "either mask event restores dependency");
foreach (var c in BalanceCatalog.All.Where(c => c.Group == "新增普通遗物")) choices[c.Id] = false;
Check(new[] { "R19", "R20", "R21" }.All(id => choices[id]), "ordinary relic bulk action excludes ancients");
Check(BalanceCatalog.All.Count(c => c.Group == "先古之民调整") == 13, "ancient grouping");
choices["C13"] = true; choices["C16"] = false;
Check(BalanceCatalog.Effective(choices, "C13") && !BalanceCatalog.Effective(choices, "C16"), "removed vanilla and new replacement remain independent");
var wire = BalanceCatalog.All.Where(c => c.AffectsGameplay).ToDictionary(c => c.Id, c => BalanceCatalog.Effective(active, c.Id));
Check(!wire.ContainsKey("C02") && wire.Count == 77, "cosmetic excluded");
byte[] encoded = BalanceConfigWire.Encode(wire);
Check(BalanceConfigWire.Differences(wire, BalanceConfigWire.Decode(encoded)).Length == 0, "wire round trip");
Check(encoded.SequenceEqual(BalanceConfigWire.Encode(wire.Reverse().ToDictionary())), "wire order deterministic");
var remote = new Dictionary<string, bool>(wire) { ["C03"] = false, ["R19"] = false };
Check(BalanceConfigWire.Differences(wire, remote).SequenceEqual(new[] { "C03", "R19" }), "precise mismatch IDs");
remote.Remove("E02"); remote["G99"] = true;
Check(BalanceConfigWire.Differences(wire, remote).Contains("E02") && BalanceConfigWire.Differences(wire, remote).Contains("G99"), "missing and unknown settings differ");
Reject([], "empty rejected");
Reject(encoded[..^1], "truncation rejected");
Reject([.. encoded, .. encoded[..4]], "duplicate rejected");
Reject([(byte)'C', (byte)'0', (byte)'1', 2], "invalid bool rejected");
Reject([(byte)'X', (byte)'0', (byte)'1', 1], "invalid ID rejected");
Reject(new byte[516], "oversized rejected");
Console.WriteLine($"PASS: {checks} configuration checks (migration, startup isolation, dependencies, grouping, handshake codec).");

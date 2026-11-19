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

const string catalogPrefixes = "CREMG";
var catalogIds = BalanceCatalog.All.Select(change => change.Id).ToArray();
var duplicateIds = catalogIds.GroupBy(id => id, StringComparer.Ordinal)
  .Where(group => group.Count() > 1).Select(group => group.Key).ToArray();
Check(duplicateIds.Length == 0, $"duplicate catalog IDs: {string.Join(", ", duplicateIds)}");
Check(catalogIds.All(id => id.Length == 3 && catalogPrefixes.Contains(id[0]) &&
  char.IsAsciiDigit(id[1]) && char.IsAsciiDigit(id[2])), "catalog IDs must have a category prefix and two digits");
Check(catalogIds.SequenceEqual(catalogIds.OrderBy(id => catalogPrefixes.IndexOf(id[0]))
  .ThenBy(id => id, StringComparer.Ordinal)), "catalog IDs must be grouped by C/R/E/M/G and ascend within each group");
Check(!catalogIds.Contains("C28") && !catalogIds.Contains("C31"), "retired IDs C28 and C31 must stay reserved");
Check(BalanceCatalog.All.GroupBy(change => (change.Group, change.Subgroup, change.Label))
  .All(group => group.Count() == 1), "no duplicate (Group, Subgroup, Label) entries");

var choices = new Dictionary<string, bool>();
BalanceCatalog.Migrate(choices, false, false, false);
Check(choices.Keys.ToHashSet(StringComparer.Ordinal).SetEquals(catalogIds), "migration supplies a value for every catalog ID");
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
Check(!wire.ContainsKey("C02") && wire.Count == catalogIds.Length - 1, "only the cosmetic setting is excluded");
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

var host = new Dictionary<string, bool> { ["C22"] = true, ["E03"] = false, ["G99"] = true };
var guest = new Dictionary<string, bool> { ["C22"] = false, ["E03"] = false, ["R29"] = true };
string[] diffIds = BalanceConfigWire.Differences(host, guest);
string[] chineseDetails = BalanceConfigDiagnostics.MismatchLines("zhs", diffIds, host, guest, localIsHost: false);
Check(chineseDetails.Length == 3 && chineseDetails[0].Contains("C22 电动力学 (新增卡牌 / 故障机器人)") &&
  chineseDetails[0].Contains("房主 = 开启 / 你 = 关闭"), "guest sees the individual toggle and both values");
Check(chineseDetails[1].Contains("G99") && chineseDetails[1].Contains("房主 = 开启 / 你 = 无此设置") &&
  chineseDetails[2].Contains("R29") && chineseDetails[2].Contains("房主 = 无此设置 / 你 = 开启"),
  "missing settings are distinct from disabled values in either direction");
string[] englishDetails = BalanceConfigDiagnostics.MismatchLines("eng", diffIds, host, guest, localIsHost: true);
Check(englishDetails[0].Contains("New Cards / Defect") &&
  englishDetails[0].Contains("You (Host) = Enabled / Joining player = Disabled"), "host sees the same values with host-side labels");
Check(BalanceConfigDiagnostics.MismatchSummary("zhs", diffIds.Length).Contains("3 项"), "popup summary includes the total");
Check(BalanceConfigDiagnostics.ReadFailure("zhs", BalanceConfigReadError.MissingHeader, false).Body.Contains("未收到房主"),
  "missing configuration is reported as a missing packet from the host");
Check(BalanceConfigDiagnostics.ReadFailure("eng", BalanceConfigReadError.TruncatedPayload, true).Body.Contains("joining player sent incomplete"),
  "truncated configuration identifies the joining player");

Console.WriteLine($"PASS: {checks} configuration checks (catalog IDs, migration, startup isolation, dependencies, grouping, handshake codec and diagnostics).");

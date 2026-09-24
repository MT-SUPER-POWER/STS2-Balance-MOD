namespace Sts2BalanceMod.Sts2BalanceModCode.Settings;

/// <summary>Bounded, deterministic handshake extension. IDs, not row positions, are sent.</summary>
internal static class BalanceConfigWire
{
  internal const uint Magic = 0x424D4346; // BMCF
  internal const int MaxEntries = 128;
  internal static byte[] Encode(IReadOnlyDictionary<string, bool> values)
  {
    if (values.Count > MaxEntries) throw new InvalidDataException("Too many configuration entries.");
    byte[] bytes = new byte[values.Count * 4];
    int offset = 0;
    foreach (var (id, enabled) in values.OrderBy(p => p.Key, StringComparer.Ordinal))
    {
      if (!ValidId(id)) throw new InvalidDataException("Invalid configuration ID.");
      foreach (char c in id) bytes[offset++] = (byte)c;
      bytes[offset++] = enabled ? (byte)1 : (byte)0;
    }
    return bytes;
  }
  internal static Dictionary<string, bool> Decode(byte[] bytes)
  {
    if (bytes.Length == 0 || bytes.Length % 4 != 0 || bytes.Length > MaxEntries * 4)
      throw new InvalidDataException("Invalid configuration payload length.");
    var result = new Dictionary<string, bool>(StringComparer.Ordinal);
    for (int i = 0; i < bytes.Length; i += 4)
    {
      string id = new(new[] { (char)bytes[i], (char)bytes[i + 1], (char)bytes[i + 2] });
      if (!ValidId(id) || bytes[i + 3] > 1 || !result.TryAdd(id, bytes[i + 3] != 0))
        throw new InvalidDataException("Invalid configuration entry.");
    }
    return result;
  }
  private static bool ValidId(string id) => id.Length == 3 && "CREMG".Contains(id[0]) &&
    char.IsAsciiDigit(id[1]) && char.IsAsciiDigit(id[2]);

  internal static string[] Differences(IReadOnlyDictionary<string, bool> local, IReadOnlyDictionary<string, bool> remote) =>
    local.Keys.Union(remote.Keys).OrderBy(id => id, StringComparer.Ordinal)
      .Where(id => !local.TryGetValue(id, out bool a) || !remote.TryGetValue(id, out bool b) || a != b).ToArray();
}

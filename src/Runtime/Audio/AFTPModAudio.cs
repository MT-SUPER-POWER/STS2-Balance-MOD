using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Random;

namespace Sts2BalanceMod.src.Runtime.Audio;

/// <summary>
/// AFP 资源的一次性音效播放器。
/// Guardian、Hexaghost 与 Slime Boss 共用此入口，统一处理资源路径、缓存和音高随机化。
/// </summary>
public static class AFTPModAudio
{
  private static readonly string AudioRoot = ModAssetPaths.Resource("sfx");
  private const float SfxVolumeOffset = 0f;

  private static readonly Dictionary<string, AudioStream> CachedStreams = new();

  public static void Play(
    string folder,
    string soundName,
    float volume = 0f,
    float pitchVariation = 0f,
    float basePitch = 1f)
  {
    var stream = GetOrLoadStream(folder, soundName);
    if (stream == null)
      return;

    var player = new AudioStreamPlayer
    {
      Stream = stream,
      VolumeDb = volume + SfxVolumeOffset,
      Bus = "SFX",
      PitchScale = pitchVariation > 0f
        ? basePitch + (float)Rng.Chaotic.NextDouble() * 2f * pitchVariation - pitchVariation
        : basePitch,
    };

    Node? parent = NRun.Instance;
    parent ??= NGame.Instance;
    parent ??= (Engine.GetMainLoop() as SceneTree)?.Root;
    if (parent == null)
    {
      player.QueueFree();
      return;
    }

    parent.AddChildSafely(player);
    player.Finished += player.QueueFree;
    player.Play();
  }

  public static void Play(Creature creature, string folder, string soundName, float volume = 0f)
  {
    Play(folder, soundName, volume);
  }

  private static AudioStream? GetOrLoadStream(string folder, string soundName)
  {
    var key = $"{folder}/{soundName}";
    if (CachedStreams.TryGetValue(key, out var cached))
      return cached;

    var stream = ResourceLoader.Load<AudioStream>($"{AudioRoot}/{key}.ogg");
    if (stream != null)
      CachedStreams[key] = stream;

    return stream;
  }
}

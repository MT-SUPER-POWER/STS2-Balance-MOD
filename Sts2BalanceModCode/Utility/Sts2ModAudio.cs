using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes;

namespace Sts2BalanceMod.Sts2BalanceModCode.Utility;

/// <summary>
/// MOD 本地音频播放工具。
/// 输入：打包在 MOD PCK 内的 res:// 音频资源路径。
/// 输出：通过 Godot AudioStreamPlayer 播放一次性音效或持续音乐。
/// </summary>
public static class Sts2ModAudio
{
  private static AudioStreamPlayer? _musicPlayer;

  public static void PlayOneShot(string path, float volume = 1f)
  {
    var stream = ResourceLoader.Load<AudioStream>(path);
    if (stream == null)
      return;

    var player = new AudioStreamPlayer
    {
      Stream = stream,
      VolumeLinear = volume,
      Bus = "SFX",
    };
    player.Finished += player.QueueFree;
    NGame.Instance?.AddChildSafely(player);
    player.Play();
  }

  public static void PlayMusic(string path, float volume = 1f)
  {
    var stream = ResourceLoader.Load<AudioStream>(path);
    if (stream == null)
      return;

    StopMusic();
    _musicPlayer = new AudioStreamPlayer
    {
      Stream = stream,
      VolumeLinear = volume,
    };
    NGame.Instance?.AddChildSafely(_musicPlayer);
    _musicPlayer.Play();
  }

  public static void StopMusic()
  {
    if (_musicPlayer == null)
      return;

    if (GodotObject.IsInstanceValid(_musicPlayer))
    {
      _musicPlayer.Stop();
      _musicPlayer.QueueFreeSafely();
    }

    _musicPlayer = null;
  }
}

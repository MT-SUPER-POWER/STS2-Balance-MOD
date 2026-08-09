using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Saves;

namespace Sts2BalanceMod.Sts2BalanceModCode.Runtime.Audio;

/// <summary>
/// MOD 增强音频播放工具。
/// 支持淡入/淡出/Boss 胜利曲，响应游戏音量设置。
/// 参考 ActsFromThePast.AFTPModAudio 的实现。
/// </summary>
public static class BalanceModAudio
{
    // ─── 常量 ───
    private const float MusicVolumeOffset = -3f;
    private const float FadeOutDb = -80f;

    // ─── 音乐状态 ───
    private static AudioStreamPlayer? _musicPlayer;
    private static string? _currentMusicPath;
    private static float _currentVolumeOffset;
    private static Tween? _fadeTween;

    // ─── Crossfade 状态 ───
    private static AudioStreamPlayer? _outgoingPlayer;
    private static Tween? _outgoingFadeTween;

    // ─── 公共 API ───

    /// <summary>
    /// 播放一次性音效。
    /// </summary>
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

    /// <summary>
    /// 直接播放背景音乐（停止当前）。
    /// </summary>
    public static void PlayMusic(string path, float volumeDbOffset = 0f)
    {
        var stream = LoadAndLoopMusic(path);
        if (stream == null)
            return;

        StopMusicImmediate();

        _musicPlayer = CreateMusicPlayer(stream, volumeDbOffset);
        _currentVolumeOffset = volumeDbOffset;
        ApplyVolume();

        var runNode = NRun.Instance;
        if (runNode != null)
        {
            runNode.AddChild(_musicPlayer);
            _musicPlayer.Play();
            _currentMusicPath = path;
        }
    }

    /// <summary>
    /// 淡入背景音乐（带 crossfade：旧音乐淡出 + 新音乐淡入）。
    /// </summary>
    public static void FadeIn(string path, float duration = 1.0f, float volumeDbOffset = 0f)
    {
        if (_currentMusicPath == path && _musicPlayer?.Playing == true)
            return;

        // 将当前播放器移到 outgoing 槽进行 crossfade
        if (_musicPlayer != null && GodotObject.IsInstanceValid(_musicPlayer))
        {
            _outgoingFadeTween?.Kill();
            _outgoingPlayer?.QueueFree();

            _outgoingPlayer = _musicPlayer;
            _outgoingFadeTween = _outgoingPlayer.CreateTween();
            _outgoingFadeTween.TweenProperty(_outgoingPlayer, "volume_db", FadeOutDb, duration)
              .SetTrans(Tween.TransitionType.Sine)
              .SetEase(Tween.EaseType.In);
            _outgoingFadeTween.TweenCallback(Callable.From(() =>
            {
                _outgoingPlayer?.QueueFree();
                _outgoingPlayer = null;
            }));
        }

        _fadeTween?.Kill();
        _musicPlayer = null;
        _currentMusicPath = null;

        var stream = LoadAndLoopMusic(path);
        if (stream == null)
            return;

        _musicPlayer = CreateMusicPlayer(stream, volumeDbOffset);
        _musicPlayer.VolumeDb = FadeOutDb;

        _currentVolumeOffset = volumeDbOffset;

        var runNode = NRun.Instance;
        if (runNode != null)
        {
            runNode.AddChild(_musicPlayer);
            _musicPlayer.Play();
            _currentMusicPath = path;

            var targetDb = CalculateVolumeDb(volumeDbOffset);
            _fadeTween = _musicPlayer.CreateTween();
            _fadeTween.TweenProperty(_musicPlayer, "volume_db", targetDb, duration)
              .SetTrans(Tween.TransitionType.Sine)
              .SetEase(Tween.EaseType.Out);
        }
    }

    /// <summary>
    /// 淡出并停止当前背景音乐。
    /// </summary>
    public static void FadeOut(float duration = 1.0f)
    {
        if (_musicPlayer == null || !GodotObject.IsInstanceValid(_musicPlayer))
            return;

        _fadeTween?.Kill();
        _fadeTween = _musicPlayer.CreateTween();
        _fadeTween.TweenProperty(_musicPlayer, "volume_db", FadeOutDb, duration)
          .SetTrans(Tween.TransitionType.Sine)
          .SetEase(Tween.EaseType.In);
        _fadeTween.TweenCallback(Callable.From(StopMusicImmediate));
    }

    /// <summary>
    /// 停止当前背景音乐（直接，无淡出）。
    /// </summary>
    public static void StopMusic()
    {
        StopMusicImmediate();
    }

    /// <summary>
    /// 播放 Boss 胜利短曲（一次性，不循环）。
    /// </summary>
    public static void PlayBossStinger(string path, float seekFrom = 0f)
    {
        var stream = ResourceLoader.Load<AudioStream>(path);
        if (stream == null)
            return;

        if (stream is AudioStreamOggVorbis ogg)
            ogg.Loop = false;

        StopMusicImmediate();

        _musicPlayer = new AudioStreamPlayer
        {
            Stream = stream,
            Bus = "Master",
        };
        ApplyVolume();

        var runNode = NRun.Instance;
        if (runNode != null)
        {
            runNode.AddChild(_musicPlayer);
            _musicPlayer.Play(seekFrom);
            _currentMusicPath = path;
        }
    }

    /// <summary>
    /// 当游戏音量设置变更时调用，同步更新播放器音量。
    /// </summary>
    public static void UpdateVolume()
    {
        ApplyVolume();
    }

    // ─── 内部方法 ───

    private static void StopMusicImmediate()
    {
        _fadeTween?.Kill();
        _fadeTween = null;
        _outgoingFadeTween?.Kill();
        _outgoingFadeTween = null;

        if (_musicPlayer != null && GodotObject.IsInstanceValid(_musicPlayer))
        {
            _musicPlayer.Stop();
            _musicPlayer.QueueFree();
        }
        _musicPlayer = null;
        _currentMusicPath = null;

        if (_outgoingPlayer != null && GodotObject.IsInstanceValid(_outgoingPlayer))
        {
            _outgoingPlayer.Stop();
            _outgoingPlayer.QueueFree();
        }
        _outgoingPlayer = null;
    }

    private static AudioStream? LoadAndLoopMusic(string path)
    {
        var stream = ResourceLoader.Load<AudioStream>(path);
        if (stream == null)
            return null;

        if (stream is AudioStreamOggVorbis ogg)
            ogg.Loop = true;

        return stream;
    }

    private static AudioStreamPlayer CreateMusicPlayer(AudioStream stream, float volumeDbOffset)
    {
        return new AudioStreamPlayer
        {
            Stream = stream,
            Bus = "Master",
        };
    }

    private static float CalculateVolumeDb(float volumeDbOffset)
    {
        var bgmVolume = SaveManager.Instance?.SettingsSave?.VolumeBgm ?? 1f;
        return Mathf.LinearToDb(Mathf.Pow(bgmVolume, 2f)) + volumeDbOffset + MusicVolumeOffset;
    }

    private static void ApplyVolume()
    {
        if (_musicPlayer != null && GodotObject.IsInstanceValid(_musicPlayer))
        {
            _musicPlayer.VolumeDb = CalculateVolumeDb(_currentVolumeOffset);
        }
    }
}

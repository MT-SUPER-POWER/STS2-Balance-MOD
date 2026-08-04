using Godot;

namespace Sts2BalanceMod.Sts2BalanceModCode.Effects;

/// <summary>
/// AFP STS1 风格特效的轻量生命周期宿主。
/// 使用内建 Node2D 作为场景节点，避免自定义 Godot 节点类型在 Mod 环境中触发脚本绑定问题。
/// </summary>
public abstract class NSts1Effect
{
  private SceneTree? _sceneTree;
  private bool _isDisposed;

  protected float Duration;
  protected float StartingDuration;
  protected Color EffectColor = Colors.White;
  protected bool IsDone;

  public Node2D Root { get; } = new();

  protected Vector2 Position
  {
    get => Root.Position;
    set => Root.Position = value;
  }

  protected void Setup()
  {
    Root.ProcessMode = Node.ProcessModeEnum.Always;
    Root.TreeEntered += OnTreeEntered;
    Root.TreeExited += OnTreeExited;
  }

  protected void AddChild(Node child)
  {
    Root.AddChild(child);
  }

  protected Node? GetParent()
  {
    return Root.GetParent();
  }

  private void OnTreeEntered()
  {
    if (_isDisposed)
      return;

    Initialize();
    if (IsDone)
    {
      Dispose();
      return;
    }

    _sceneTree = Root.GetTree();
    _sceneTree.ProcessFrame += OnProcessFrame;
  }

  private void OnTreeExited()
  {
    UnsubscribeFromProcessFrame();
  }

  private void OnProcessFrame()
  {
    if (_isDisposed || !GodotObject.IsInstanceValid(Root) || !Root.IsInsideTree())
    {
      UnsubscribeFromProcessFrame();
      return;
    }

    Update((float)Root.GetProcessDeltaTime());
    if (IsDone)
      Dispose();
  }

  private void Dispose()
  {
    if (_isDisposed)
      return;

    _isDisposed = true;
    UnsubscribeFromProcessFrame();
    if (GodotObject.IsInstanceValid(Root))
      Root.QueueFree();
  }

  private void UnsubscribeFromProcessFrame()
  {
    if (_sceneTree != null && GodotObject.IsInstanceValid(_sceneTree))
      _sceneTree.ProcessFrame -= OnProcessFrame;

    _sceneTree = null;
  }

  protected virtual void Initialize()
  {
  }

  protected abstract void Update(float delta);

  protected static float Lerp(float from, float to, float t)
  {
    return from + (to - from) * t;
  }

  protected static float EaseOut(float t)
  {
    return 1f - (1f - t) * (1f - t);
  }
}

namespace RaylibEngine.Components;

using RaylibEngine.Core;
using System.Numerics;

/// <summary>
/// Abstract drawable container.
/// </summary>
public abstract class SceneNode : Container, IDrawable
{
    /// <summary>
    /// World transformation matrix.
    /// </summary>
    protected Matrix4x4 worldMatrix = Matrix4x4.Identity;

    public bool Visible { get; set; }

    public bool IsDirty { get; set; }

    public abstract void Draw();
}

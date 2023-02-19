namespace RaylibEngine.Core;

using Raylib_CsLo;
using System.Numerics;

/// <summary>
/// Supports rendering entities during the scenes render phase.
/// </summary>
public interface IDrawable
{
    void Draw();
    bool Visible { get; set; }
    bool IsDirty { get; }
}

public interface IDrawable2D : IDrawable
{
    float Angle { get; }
    Vector2 Position { get; }
    Vector2 Pivot { get; }
    Vector2 Anchor { get; }
    Rectangle Dst { get; }
}

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
    /// <summary>
    /// Scale in X, Y direction..
    /// </summary>
    Vector2 Scale { get; }

    /// <summary>
    /// Angle in degrees.
    /// </summary>
    float Angle { get; }

    /// <summary>
    /// Position in local space (relative to parent).
    /// </summary>
    Vector2 Position { get; }

    Vector2 Pivot { get; }
    Vector2 Anchor { get; }
    
    Matrix4x4 WorldMatrix => Matrix4x4.CreateScale(Scale.X, Scale.Y, 1) *
                             Matrix4x4.CreateRotationZ((float)(Angle * Helpers.DEGREE_2_RADIAN)) *
                             Matrix4x4.CreateTranslation(Position.X, Position.Y, 0);
}

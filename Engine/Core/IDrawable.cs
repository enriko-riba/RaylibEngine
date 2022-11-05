namespace RaylibEngine.Core;

/// <summary>
/// Supports rendering entities during the scenes render phase.
/// </summary>
public interface IDrawable
{
    void Draw();
    bool Visible { get; set; }
}

namespace RaylibEngine.Core;

/// <summary>
/// Supports updating entities during the scenes update phase.
/// </summary>
public interface IUpdateable
{
    void Update(float elapsedSeconds);
}

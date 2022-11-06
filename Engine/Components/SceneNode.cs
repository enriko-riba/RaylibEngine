namespace RaylibEngine.Components;

using RaylibEngine.Core;

public abstract class SceneNode : Container, IDrawable
{
    public bool Visible { get ; set; }

    public bool IsDirty { get; set; }

    public abstract void Draw();
}

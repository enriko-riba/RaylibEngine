namespace RaylibEngine.SceneManagement;

using Raylib_CsLo;
using RaylibEngine.Components;
using RaylibEngine.Core;

public abstract class Scene : Container, IDrawable
{
    protected readonly Color textColor = Raylib.LIME;

    public Scene(string name)
    {
        Name = name;        
    }

    /// <summary>
    /// Invokes <see cref="Scene.OnUpdate(int)"/> on the active scene followed by <see cref="IUpdateable.Update(int)"/> on each child implementing <see cref="IUpdateable"/>.
    /// </summary>
    /// <param name="ellapsedSeconds"></param>
    public void Update(float ellapsedSeconds)
    {
        OnUpdate(ellapsedSeconds);
        foreach (var child in Children)
        {
            if (child is IUpdateable uc) uc.Update(ellapsedSeconds);
        }
    }

    /// <summary>
    /// Clears the background and renders the current scene and all <see cref="IDrawable"/> children.
    /// Note: <see cref="Raylib.BeginDrawing"/> is invoked on method start and <see cref="Raylib.EndDrawing"/> before method exit.
    /// </summary>
    public void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(BackgroundColor);
        OnBeginDraw();
        foreach (var child in Children)
        {
            if (child is IDrawable rc) rc.Draw();
        }
		OnEndDraw();
		Raylib.EndDrawing();
    }

    /// <summary>
    /// The scenes window title.
    /// </summary>
    public string? WindowTitle { get; protected set; } = "Base Scene";

    /// <summary>
    /// The background color used to clear the background on each <see cref="Draw"/> invocation.
    /// Default value is <see cref="Raylib.DARKBLUE"/>
    /// </summary>
    public Color BackgroundColor { get; set; } = Raylib.DARKBLUE;

    /// <summary>
    /// Invoked after the scene becomes the active scene.
    /// Note: the base class has no implementation, if overridden in a derived class invoking base.OnActivate does nothing and should be omitted.
    /// </summary>
    public virtual void OnActivate() { }

    /// <summary>
    /// Invoked from <see cref="SceneManager"/>.
    /// Note: the base class has no implementation, if overridden in a derived class invoking base.OnUpdate does nothing and should be omitted.
    /// </summary>
    /// <param name="ellapsedSeconds">passed time in milliseconds</param>
    public virtual void OnUpdate(float ellapsedSeconds) { }

	/// <summary>
	/// Invoked from <see cref="SceneManager"/>.
	/// Note: the base class has no implementation, if overridden in a derived class invoking the base <see cref="OnBeginDraw"/> does nothing and should be omitted.
	/// </summary>
	public virtual void OnBeginDraw() { }

	/// <summary>
	/// Invoked from <see cref="SceneManager"/>.
	/// Note: the base class has no implementation, if overridden in a derived class invoking the base <see cref="OnEndDraw"/> does nothing and should be omitted.
	/// </summary>
	public virtual void OnEndDraw() { }
}

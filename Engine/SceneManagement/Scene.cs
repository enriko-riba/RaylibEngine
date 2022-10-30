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
	public bool Visible { get => true; set { } }

    /// <summary>
    /// Invokes <see cref="Scene.OnUpdate(int)"/> on the active scene followed by <see cref="IUpdateable.Update(int)"/> on each child implementing <see cref="IUpdateable"/>.
    /// </summary>
    /// <param name="ellapsedSeconds"></param>
    public void Update(float ellapsedSeconds)
    {
		var isAltDown = Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_ALT) || Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_ALT);
		if (isAltDown && Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER))
		{
			int display = Raylib.GetCurrentMonitor();
			if (Raylib.IsWindowFullscreen())
			{
				Raylib.SetWindowSize(Raylib.GetMonitorWidth(display)-150, Raylib.GetMonitorHeight(display)-50);
			}
			else
			{
				Raylib.SetWindowSize(Raylib.GetMonitorWidth(display), Raylib.GetMonitorHeight(display));
			}
			Raylib.ToggleFullscreen();
		}

		if (Raylib.IsWindowResized())
		{
			var w = Raylib.GetScreenWidth();
			var h = Raylib.GetScreenHeight();
			OnResize(w, h);
		}

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
		RenderChildren(this);        
		OnEndDraw();
		Raylib.EndDrawing();
    }

	/// <summary>
	/// Renders the whole children graph.
	/// Note: invisible nodes including child nodes are not rendered.
	/// </summary>
	/// <param name="container"></param>
	private void RenderChildren(IContainer container)
	{
		foreach (var child in container.Children)
		{
			if (child is IDrawable dc) 
			{
				if (dc.Visible)
				{
					dc.Draw();
					RenderChildren(child);
				}
			}
		}
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

	/// <summary>
	/// Invoked when the scree is resized.
	/// </summary>
	/// <param name="width">the new window width</param>
	/// <param name="height">the new window height</param>
	public virtual void OnResize(int width, int height) { }
}

namespace RaylibEngine.SceneManagement;

using Raylib_CsLo;
using RaylibEngine.Components;
using RaylibEngine.Core;

public abstract class Scene : Container, IDrawable
{
    protected readonly Color textColor = Raylib.LIME;

    private int width;
    private int height;

    public Scene(string name)
    {
        Name = name;
        width = Raylib.GetScreenWidth();
        height = Raylib.GetScreenHeight();
    }

    /// <summary>
    /// Returns the screen width in pixels.
    /// </summary>
    public int ScreenWidth => width;

    /// <summary>
    /// Returns the screen height in pixels.
    /// </summary>
    public int ScreenHeight => height;

    /// <summary>
    /// Always returns true, exists just for <see cref="IDrawable"/> support.
    /// </summary>
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
                Raylib.SetWindowSize(Raylib.GetMonitorWidth(display) - 150, Raylib.GetMonitorHeight(display) - 50);
            }
            else
            {
                Raylib.SetWindowSize(Raylib.GetMonitorWidth(display), Raylib.GetMonitorHeight(display));
            }
            Raylib.ToggleFullscreen();
        }

        if (Raylib.IsWindowResized())
        {
            width = Raylib.GetScreenWidth();
            height = Raylib.GetScreenHeight();
            OnResize();
        }

        OnBeginUpdate(ellapsedSeconds);
        foreach (var child in Children)
        {
            if (child is IUpdateable uc) uc.Update(ellapsedSeconds);
        }
        OnEndUpdate(ellapsedSeconds);
        ExecuteActions(this);
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

    internal void Activate()
    {
        width = Raylib.GetScreenWidth();
        height = Raylib.GetScreenHeight();
        this.OnActivate();
    }

    /// <summary>
    /// Executes queued actions on child nodes in graph.
    /// </summary>
    /// <param name="container"></param>
    private void ExecuteActions(IContainer container)
    {
        if (container is IActionQueue actionContainer) ExecuteQueuedActions(actionContainer.QueuedActions);
        foreach (var child in container.Children)
        {
            ExecuteActions(child);
        }
    }

    /// <summary>
    /// Executes all queued actions and removes them from queue.
    /// </summary>
    /// <param name="queuedActions"></param>
    private static void ExecuteQueuedActions(Queue<Action> queuedActions)
    {
        while (queuedActions.TryDequeue(out var action))
        {
            action();
        }
        queuedActions.Clear();
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
    /// Note: the base class has no implementation, if overridden in a derived class invoking base.OnBeginUpdate does nothing and should be omitted.
    /// </summary>
    /// <param name="ellapsedSeconds">passed time in milliseconds</param>
    public virtual void OnBeginUpdate(float ellapsedSeconds) { }

    /// <summary>
    /// Invoked from <see cref="SceneManager"/>.
    /// Note: the base class has no implementation, if overridden in a derived class invoking base.OnEndUpdate does nothing and should be omitted.
    /// </summary>
    /// <param name="ellapsedSeconds">passed time in milliseconds</param>
    public virtual void OnEndUpdate(float ellapsedSeconds) { }

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
    /// Invoked when the screen is resized. The new screen size is in <see cref="ScreenWidth"/> and <see cref="ScreenHeight"/> properties.
    /// </summary>	
    public virtual void OnResize() { }
}

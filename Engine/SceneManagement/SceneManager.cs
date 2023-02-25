
using Raylib_CsLo;

namespace RaylibEngine.SceneManagement;
/// <summary>
/// Provides basic scene management.
/// </summary>
public static class SceneManager
{
    private static readonly Dictionary<string, Scene> scenes = new();
    private static Scene? currentScene;

    /// <summary>
    /// Returns the current (active) scene.
    /// </summary>
    public static Scene? CurrentScene => currentScene;

    /// <summary>
    /// Adds a scene to the scene graph. The scene can be activated by its name with <see cref="ActivateScene(string)"/>
    /// </summary>
    /// <param name="scene"></param>
    public static void AddScene(Scene scene)
    {
        scenes[scene.Name!] = scene;
    }

    /// <summary>
    /// Activates a registered <see cref="Scene"/> by its name.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <exception cref="ArgumentException"></exception>
    public static void ActivateScene(string sceneName)
    {
        if (scenes.TryGetValue(sceneName, out var scene))
        {
            ActivateScene(scene);
        }
        else
        {
            throw new ArgumentException("Scene not found!", nameof(sceneName));
        }
    }

    /// <summary>
    /// Adds the <see cref="Scene"/> to the scene graph and activates it.
    /// </summary>
    /// <param name="scene"></param>
    public static void ActivateScene(Scene scene)
    {
        if (scene != currentScene)
        {
            scenes[scene.Name!] = scene;
            currentScene = scene;
            Raylib.SetWindowTitle(currentScene.WindowTitle ?? currentScene.Name ?? string.Empty);
            currentScene.Activate();
        }
    }

    /// <summary>
    /// The main entry for rendering. Usually, this is the only method invoked from the main game loop.
    /// Note: invoking this method is mandatory for every game tick.
    /// </summary>
    public static void Draw()
    {
        currentScene?.Draw();
    }

    /// <summary>
    /// This method uses <see cref="Raylib.GetFrameTime()"/> to provide the elapsed frame time and invokes the <see cref="Scene.Update(int)"/> on the active scene which in turn updates all <see cref="Core.IUpdateable"/> child entities.
    /// It is not mandatory to invoke this method.
    /// </summary>
    public static void Update()
    {
        if (currentScene is not null)
        {
            var dt = Raylib.GetFrameTime();
            currentScene.Update(dt);
        }
    }

    /// <summary>
    /// Invokes the <see cref="Scene.Update(int)"/> on the active scene which in turn updates all <see cref="Core.IUpdateable"/> child entities.
    /// Note: It is not mandatory to invoke this method, use if you want to provide your own update time interval.
    /// </summary>
    /// <param name="elapsedSeconds">the elapsed time in seconds since last update</param>
    public static void Update(float elapsedSeconds)
    {
        currentScene?.Update(elapsedSeconds);
    }
}

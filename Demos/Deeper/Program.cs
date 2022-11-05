
using Deeper.Entities;
using RaylibEngine.SceneManagement;

namespace Deeper;
public static class Program
{
    const string Title = "Deeper";

    public static int Main()
    {
        SetConfigFlags((uint)(ConfigFlags.FLAG_WINDOW_RESIZABLE | ConfigFlags.FLAG_MSAA_4X_HINT));
        InitWindow(0, 0, Title);
        ToggleFullscreen();
        SetTargetFPS(144);

        DeeperScene mainScene = new(Title);
        SceneManager.ActivateScene(mainScene);

        //  main loop
        while (!WindowShouldClose())
        {
            SceneManager.Update();
            SceneManager.Draw();
        }
        CloseWindow();
        return 0;
    }
}
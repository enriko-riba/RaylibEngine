namespace Deeper;

using Deeper.Entities;
using RaylibEngine.SceneManagement;

public static class Program
{
    const string Title = "Deeper";

    public static int Main()
    {
        SetConfigFlags((uint)(ConfigFlags.FLAG_WINDOW_RESIZABLE | ConfigFlags.FLAG_MSAA_4X_HINT));
		InitWindow(0, 0, Title);
		ToggleFullscreen();
        SetTargetFPS(200);

        var mainScene = new DeeperScene(Title);
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
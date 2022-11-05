
using RaylibEngine.SceneManagement;
using RaySnake.Scenes;

namespace RaySnake;
public static class Program
{
    const string Title = "Raylib Snake Game";

    const int ScreenWidth = 1920;
    const int ScreenHeight = 1080;
    const int MinScreenWidth = 1440;
    const int MinScreenHeight = 900;

    public static int Main()
    {
        SetConfigFlags((uint)(ConfigFlags.FLAG_FULLSCREEN_MODE | ConfigFlags.FLAG_MSAA_4X_HINT));
        InitWindow(ScreenWidth, ScreenHeight, Title);
        SetWindowMinSize(MinScreenWidth, MinScreenHeight);
        SetWindowPosition(0, 0);
        SetTargetFPS(120);

        MenuScene startScene = new(Title);
        SceneManager.ActivateScene(startScene);

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

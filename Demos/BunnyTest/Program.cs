
using RaylibEngine.SceneManagement;

namespace BunnyTest;
public static class Program
{
    const string Title = "Raylib Bunny test";

    const int ScreenWidth = 800;
    const int ScreenHeight = 450;


    public static int Main()
    {
        SetConfigFlags((uint)(ConfigFlags.FLAG_WINDOW_RESIZABLE));
        InitWindow(ScreenWidth, ScreenHeight, Title);
        SetWindowPosition(1, 30);

        BunnyScene bunnyScene = new(Title);
        SceneManager.ActivateScene(bunnyScene);

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
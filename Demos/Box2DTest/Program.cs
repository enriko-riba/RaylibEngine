
using RaylibEngine.SceneManagement;

namespace Box2DTest;
public static class Program
{
    const string Title = "Raylib Sprite test";

    public static int Main()
    {
        SetConfigFlags((uint)(ConfigFlags.FLAG_WINDOW_RESIZABLE | ConfigFlags.FLAG_MSAA_4X_HINT));
        InitWindow(0, 0, Title);

        SetTargetFPS(144);
        PhysicsScene bunnyScene = new(Title);
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
namespace SpriteTest;

using RaylibEngine.SceneManagement;

public static class Program
{
    const string Title = "Raylib Sprite test";

    public static int Main()
    {
        SetConfigFlags((uint)(ConfigFlags.FLAG_WINDOW_RESIZABLE | ConfigFlags.FLAG_MSAA_4X_HINT));
		InitWindow(GetScreenWidth(), GetScreenHeight(), Title);
		
        SetTargetFPS(144);
        var bunnyScene = new SpriteScene(Title);
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
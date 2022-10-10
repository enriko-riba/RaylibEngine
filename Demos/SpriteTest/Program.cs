namespace SpriteTest;

using RaylibEngine.SceneManagement;

public static class Program
{
    const string Title = "Raylib Sprite test";
    const int ScreenWidth = 1024;
    const int ScreenHeight = 768;

    public static int Main()
    {
        SetConfigFlags((uint)(ConfigFlags.FLAG_WINDOW_RESIZABLE | ConfigFlags.FLAG_MSAA_4X_HINT | ConfigFlags.FLAG_WINDOW_MINIMIZED));
		InitWindow(ScreenWidth, ScreenHeight, Title);
		
		//SetWindowPosition(1, 20);
        SetTargetFPS(60);
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
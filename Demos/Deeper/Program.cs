
using Deeper.Entities;
using RaylibEngine.SceneManagement;

namespace Deeper;
public static class Program
{
    const string Title = "Deeper";
    const int ScreenWidth = 1920;
    const int ScreenHeight = 1080;
    const int MinScreenWidth = 1440;
    const int MinScreenHeight = 900;

    public static int Main()
    {
        SetConfigFlags((uint)(ConfigFlags.FLAG_WINDOW_RESIZABLE | ConfigFlags.FLAG_VSYNC_HINT));
        InitWindow(ScreenWidth, ScreenHeight, Title);
        var monitorCount = GetMonitorCount();
        if(monitorCount > 0 )
        {
            SetWindowMonitor(1);
        }
        SetWindowMinSize(MinScreenWidth, MinScreenHeight);
        
        var img = LoadImage("./Assets/Spike Head/Idle.png");
        SetWindowIcon(img);

        //ToggleFullscreen();
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
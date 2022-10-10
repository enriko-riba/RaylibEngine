namespace RaySnake.Scenes;

using RaylibEngine.SceneManagement;
using static Raylib_CsLo.RayGui;

internal class MenuScene : Scene
{
    private readonly string originalTitle;
    public static readonly string SceneName = nameof(MenuScene);

    public MenuScene(string title) : base(SceneName)
    {
        originalTitle = title;
        WindowTitle = $"{title} - main menu";
    }

    public override void OnBeginDraw()
    {
        const string Text = "WELCOME TO THE SNAKE GAME";
        const int FontSize = 50;
        const int ButtonHeight = 50;
        const int ButtonWidth = 150;

        var font = GetFontDefault();
        var ts = MeasureTextEx(font, Text, FontSize, 0);
        var w = GetScreenWidth();
        var h = GetScreenHeight();

        DrawText(Text, (w - ts.X) / 2, h/3, FontSize, textColor);

        if (GuiButton(new Rectangle((w - ButtonWidth) / 2, h - ButtonHeight - 10, ButtonWidth, ButtonHeight), "Play"))
        {
            SceneManager.ActivateScene(new GameScene(originalTitle));
        }
    }
}

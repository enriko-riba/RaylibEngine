﻿using RaylibEngine.SceneManagement;
namespace BunnyTest;

internal class BunnyScene : Scene
{
    const int MOUSE_BUTTON_LEFT = 0;
    const int MOUSE_BUTTON_RIGHT = 1;
    const int BUNNY_TEXTURE_COUNT = 1;	//	Note: change the count to 12 in order to use different textures - using multiple textures will break the batching

    private int bunnyCount = 1000;
    private readonly Texture[] texture = new Texture[BUNNY_TEXTURE_COUNT];


    public BunnyScene(string title) : base(title)
    {
        BackgroundColor = DARKBROWN;
        for (var i = 0; i < texture.Length; i++)
        {
            texture[i] = LoadTexture($"./Assets/bunny{i + 1:D2}.png");
        }
        CreateBunnies(bunnyCount);
    }

    public override void OnBeginDraw()
    {
        if (IsMouseButtonPressed(MOUSE_BUTTON_LEFT))
        {
            bunnyCount += 1000;
            CreateBunnies(1000);
        }
        if (IsMouseButtonPressed(MOUSE_BUTTON_RIGHT))
        {
            bunnyCount += 10000;
            CreateBunnies(10000);
        }
    }

    public override void OnEndDraw()
    {
        DrawRectangle(2, 2, 350, 40, BLACK);
        DrawFPS(5, 10);
        DrawText($"bunnies: {bunnyCount}", 150, 10, 20, LIME);
    }

    private void CreateBunnies(int count)
    {
        var w = GetScreenWidth();
        var h = GetScreenHeight();

        for (var i = 0; i < count; i++)
        {
            Bunny bunny = new(texture[i % BUNNY_TEXTURE_COUNT], w, h)
            {
                Position = new(Random.Shared.Next(0, w), Random.Shared.Next(10, 300)),
                Tint = new Color(GetRandomValue(50, 240), GetRandomValue(80, 240), GetRandomValue(100, 240), 255),
            };
            AddChild(bunny);
        }
    }
}

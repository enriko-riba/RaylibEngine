
using RaylibEngine;
using RaylibEngine.Components;
using RaylibEngine.SceneManagement;
using System.Numerics;

namespace SpriteTest;
internal class SpriteScene : Scene
{
    const int MOUSE_BUTTON_LEFT = 0;
    const int MOUSE_BUTTON_RIGHT = 1;

    private readonly Vector2[] Anchors = new Vector2[] {
        new(0, 0),
        new(0.5f, 1),
        new(1, 1),
        new(0.5f, 0.5f),
    };
    private int anchorId = 3;
    private readonly string[] AnchorNames = new string[] { "Top left", "Bottom mid", "Bottom right", "Center" };

    private readonly Texture atlas;
    private readonly Sprite sprite;
    private bool isRotating = true;
    private bool isHover;
    private readonly Bunny[] bunnies;
    private readonly bool[] bunnyCollision;

    public SpriteScene(string name) : base(name)
    {
        WindowTitle = name;
        BackgroundColor = DARKBROWN;
        atlas = LoadTexture("./Assets/spr.png");
        SetTextureFilter(atlas, TextureFilter.TEXTURE_FILTER_BILINEAR);
        var w = GetScreenWidth();
        var h = GetScreenHeight();

        const int SpriteSize = 64;  //	those constants are dimensions of sprite frames inside the texture atlas
        const int BunnySize = 32;

        //	create rotating sprite from atlas
        sprite = new Sprite(atlas)
        {
            Frame = new Rectangle(0, 0, SpriteSize, SpriteSize),
            Position = new(w / 2, h / 2),
            Pivot = new(0.5f, 0.5f),
            Anchor = Anchors[anchorId],
            Width = 128,
            Height = 128,
        };
        AddChild(sprite);

        // create random bunnies from atlas
        bunnies = new Bunny[15];
        bunnyCollision = new bool[15];
        for (var i = 0; i < bunnies.Length; i++)
        {
            var x = SpriteSize + Random.Shared.Next(0, 2) * BunnySize;
            var y = Random.Shared.Next(0, 2) * BunnySize;
            bunnies[i] = new Bunny(atlas)
            {
                Frame = new Rectangle(x, y, BunnySize, BunnySize),
                Position = new(Random.Shared.Next(0, w - BunnySize), Random.Shared.Next(0, h - BunnySize)),
                Width = BunnySize,
                Height = BunnySize
            };
            AddChild(bunnies[i]);
        }
    }

    public override void OnBeginDraw()
    {
        RenderAxes();
    }

    public override void OnEndDraw()
    {
        RenderMenu();
        RenderSprites();
    }

    public override void OnBeginUpdate(float elapsedSeconds)
    {
        //	update pivot, anchor and rotation states
        if (IsKeyPressed(KeyboardKey.KEY_SPACE))
        {
            isRotating = !isRotating;
        }
        if (isRotating) sprite.Angle = (sprite.Angle + 0.1f) % 360;

        if (IsMouseButtonPressed(MOUSE_BUTTON_LEFT))
        {
            sprite.Pivot = sprite.Pivot == Vector2.Zero ? new(0.5f, 0.5f) : Vector2.Zero;
        }
        if (IsMouseButtonPressed(MOUSE_BUTTON_RIGHT))
        {
            anchorId = ++anchorId % Anchors.Length;
            sprite.Anchor = Anchors[anchorId];
        }

        //	update sprite position
        sprite.Position += IsKeyDown(KeyboardKey.KEY_RIGHT) ? new(1f, 0) :
                            IsKeyDown(KeyboardKey.KEY_LEFT) ? new(-1f, 0) :
                            IsKeyDown(KeyboardKey.KEY_UP) ? new(0, -1f) :
                            IsKeyDown(KeyboardKey.KEY_DOWN) ? new(0, 1f) :
                            Vector2.Zero;

        //	update mouse hover status 
        isHover = sprite.Aabb.ContainsPoint(GetMousePosition());

        //	save sprite/bunny collision status
        for (var i = 0; i < bunnies.Length; i++)
        {
            bunnyCollision[i] = sprite.Aabb.IntersectsAabb(bunnies[i].Aabb);
        }
    }

    private static void RenderAxes()
    {
        var w = GetScreenWidth();
        var h = GetScreenHeight();
        DrawLine(w / 2, 10, w / 2, h - 10, YELLOW);
        DrawLine(10, h / 2, w - 10, h / 2, BLUE);
    }

    private void RenderSprites()
    {
        var isCollidingWithAnyBunny = false;
        for (var i = 0; i < bunnies.Length; i++)
        {
            if (bunnyCollision[i])
            {
                isCollidingWithAnyBunny = true;
                DrawRectangleLinesEx(bunnies[i].Aabb, 2f, RED);
            }
        }

        var color = isCollidingWithAnyBunny ? RED :
                    isHover ? GREEN : GRAY;
        DrawRectangleLinesEx(sprite.Aabb, 2f, color);
    }

    private void RenderMenu()
    {
        DrawFPS(5, 10);
        DrawText($"Pivot: {sprite.Pivot} left click to change", 5, 30, 20, LIME);
        DrawText($"Anchor: {AnchorNames[anchorId]} right click to change", 5, 50, 20, LIME);
        DrawText($"Angle: {sprite.Angle:N2} space to toggle", 5, 70, 20, LIME);
        DrawText("arrows to move", 5, 90, 20, LIME);
    }
}

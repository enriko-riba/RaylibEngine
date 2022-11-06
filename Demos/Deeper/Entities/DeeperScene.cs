
namespace Deeper.Entities;

using RaylibEngine.Components;
using RaylibEngine.SceneManagement;
using System.Numerics;

internal class DeeperScene : Scene
{
    private readonly Rectangle FrameSpotMask = new(354, 0, 32, 256);
    private readonly TilingSprite skyBackground;
    private readonly Vehicle vehicle;
    private readonly Map map;

    private Camera2D camera;
    private Vector2 halfOffset;

    public DeeperScene(string name) : base(name)
    {
        WindowTitle = name;
        BackgroundColor = BLACK;
        halfOffset = new(ScreenWidth / 2, ScreenHeight / 2);
        camera = new()
        {
            offset = halfOffset,
            zoom = 1f
        };

        var atlas = LoadTexture("./Assets/Atlas.png");
        SetTextureWrap(atlas, TextureWrap.TEXTURE_WRAP_CLAMP);
        SetTextureFilter(atlas, TextureFilter.TEXTURE_FILTER_BILINEAR);

        //	static sky background texture		
        skyBackground = new TilingSprite(atlas)
        {
            Frame = new Rectangle(448, 0, 64, 64),
            Width = ScreenWidth,
            Height = ScreenHeight / 2 + Map.TileSize * 2,
            Position = Vector2.Zero
        };
        //AddChild(skyBackground);

        //	ground map
        map = new Map(atlas);
        foreach (var mapTile in map.Tiles)
        {
            if (mapTile.Sprite is not null) AddChild(mapTile.Sprite);
        }

        //	players vehicle		
        vehicle = new Vehicle(atlas, new(Map.Width / 2, 0), map)
        {
            Pivot = new(0.5f, 1f),
            Anchor = new(0.5f, 0f),
        };
        AddChild(vehicle);

        //	light spot mask around vehicle	
        VehicleSpotMask spotMask = new(atlas, FrameSpotMask, Map.TileSize * 0.25f, Map.TileSize * 2f)
        {
            Width = (Map.Width + 3) * Map.TileSize,         //	few tiles larger then map to hide edge 
            Height = (Map.Height + 2) * Map.TileSize,       //	gradient caused by bilinear filtering

            Position = new(-Map.TileSize * 2, Map.TileSize),    //	move gradient area outside of map
        };
        AddChild(spotMask);
        spotMask.UpdateViewport(halfOffset + new Vector2(0, -Map.TileSize));    //	center of screen with offset due to vehicles bottom anchor
    }

    public override void OnBeginDraw()
    {
        BeginMode2D(camera);
    }

    public override void OnEndDraw()
    {
        EndMode2D();
        RenderMenu();
    }

    public override void OnEndUpdate(float ellapsedSeconds)
    {
        //	limit vehicle position
        //if (vehicle.Position.X < Map.TileSize)
        //	vehicle.Position = new(Map.TileSize, vehicle.Position.Y);
        //if (vehicle.Position.X > Map.TileSize * (Map.Width - 2))
        //	vehicle.Position = new(Map.TileSize * (Map.Width - 2), vehicle.Position.Y);
        if (vehicle.Position.Y < 0)
            vehicle.Position = new(vehicle.Position.X, 0);
        //if (vehicle.Position.Y > Map.TileSize * (Map.Height - 1))
        //	vehicle.Position = new(vehicle.Position.X, Map.TileSize * (Map.Height - 1));

        camera.target = new(vehicle.Position.X, vehicle.Position.Y);
        camera.target.Y -= Map.TileSize / 2;

        skyBackground.Position = new(vehicle.Position.X - halfOffset.X, -halfOffset.Y - Map.TileSize);
    }

    public override void OnResize()
    {
        halfOffset = new Vector2(ScreenWidth / 2f, ScreenHeight / 2f);
        camera.offset = halfOffset;
        var spotMask = GetChildByName(VehicleSpotMask.NodeName) as VehicleSpotMask;
        spotMask?.UpdateViewport(halfOffset + new Vector2(0, -Map.TileSize));
    }

    private void RenderMenu()
    {
        DrawRectangle(5, 5, 300, 105, BLACK);
        DrawFPS(15, 10);
        DrawText("resolution:", 15, 40, 20, YELLOW); DrawText($"{ScreenWidth} x {ScreenHeight}", 130, 40, 20, WHITE);
        DrawText("position:", 15, 60, 20, YELLOW); DrawText($"({vehicle.Position.X:N0}, {vehicle.Position.Y:N0})", 130, 60, 20, WHITE);
        DrawText("tile:", 15, 80, 20, YELLOW); DrawText($"({vehicle.Position.X / Map.TileSize:N0}, {vehicle.Position.Y / Map.TileSize:N0})", 130, 80, 20, WHITE);
    }
}

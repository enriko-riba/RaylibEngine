﻿
namespace Deeper.Entities;

using RaylibEngine.Components;
using RaylibEngine.SceneManagement;
using System.Numerics;

internal class DeeperScene : Scene
{
    private readonly Rectangle spotMaskSourceFrame = new(354, 0, 32, 256);
    private readonly TilingSprite skyBackground;
    private readonly Vehicle vehicle;
    private readonly GameModel gameModel;
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
        AddChild(skyBackground);

        //	ground map
        var map = new Map(atlas);
        gameModel = new GameModel(map);
        foreach (var mapTile in map.Tiles)
        {
            if (mapTile.Sprite is not null) AddChild(mapTile.Sprite);
        }

        //	players vehicle		
        vehicle = new Vehicle(atlas, gameModel)
        {
            Pivot = new(0.5f, 1f),
            Anchor = new(0.5f, 0f),
        };
        AddChild(vehicle);

        //	light spot mask around vehicle	
        var innerRadius = Map.TileSize * 1.75f;
        var outerRadius = Map.TileSize * 3.5f;
        VehicleSpotMask spotMask = new(atlas, spotMaskSourceFrame, innerRadius, outerRadius)
        {
            //	make size few tiles larger then map to hide edge gradient caused by bilinear filtering
            Width = (Map.Width + 2) * Map.TileSize,             
            Height = (Map.Height + 2) * Map.TileSize,           
            Position = new(-Map.TileSize*2, Map.TileSize),    //	move gradient area outside of map
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

    public override void OnEndUpdate(float elapsedSeconds)
    {        
        if (vehicle.Position.Y < 0)
            vehicle.Position = new(vehicle.Position.X, 0);        

        camera.target = new(vehicle.Position.X, vehicle.Position.Y - Map.TileSize / 2);
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
        DrawRectangle(5, 5, 300, 125, BLACK);
        DrawFPS(15, 10);
        DrawText("depth:", 15, 40, 20, YELLOW); DrawText($"{gameModel.Depth:N2}", 130, 40, 20, WHITE);
        DrawText("resolution:", 15, 60, 20, YELLOW); DrawText($"{ScreenWidth} x {ScreenHeight}", 130, 60, 20, WHITE);
        DrawText("position:", 15, 80, 20, YELLOW); DrawText($"({vehicle.Position.X:N0}, {vehicle.Position.Y:N0})", 130, 80, 20, WHITE);
        DrawText("tile:", 15, 100, 20, YELLOW); DrawText($"({vehicle.Position.X / Map.TileSize:N0}, {vehicle.Position.Y / Map.TileSize:N0})", 130, 100, 20, WHITE);
    }
}

namespace Deeper.Entities;

using RaylibEngine.Components;
using RaylibEngine.SceneManagement;
using System.Numerics;

internal class DeeperScene : Scene
{
	const int TileSize = 48;    //	dimensions of sprite frames inside the texture atlas
	const int MapWidth = 100;
	const int MapHeight = 100;

	//---------------------------------
	//	Atlas frame sizes
	//---------------------------------
	private readonly Rectangle FrameGroundLine = new(96, 64, TileSize, TileSize);
	private readonly Rectangle FrameGroundGrass = new(96, 0, TileSize, TileSize);
	private readonly Rectangle FrameGroundDirt = new(96, 192, TileSize, TileSize);
	private readonly Rectangle FrameGroundBrick = new(272, 64, TileSize, TileSize);
	private readonly Rectangle FrameGroundLevel = new(48, 192, TileSize, TileSize);
	private readonly Rectangle FrameSpotMask = new(354, 0, 4, 256);

	private readonly TilingSprite backGround;
	private readonly Vehicle vehicle;
	private readonly Tile[] map = new Tile[MapWidth * MapHeight];
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
		SetTextureFilter(atlas, TextureFilter.TEXTURE_FILTER_POINT);
		SetTextureWrap(atlas, TextureWrap.TEXTURE_WRAP_CLAMP);
		
		//	static background texture		
		backGround = new TilingSprite(atlas)
		{
			Frame = new Rectangle(448, 0, 64, 64),
			Width = ScreenWidth,
			Height = ScreenHeight / 2 + TileSize * 2,
			Position = Vector2.Zero
		};
		AddChild(backGround);

		//	ground map
		for (int x = 0; x < MapWidth; x++)
		{
			for (int y = 0; y < MapHeight; y++)
			{
				var mapTile = new Sprite(atlas, new(x * TileSize, y * TileSize + TileSize / 2), TileSize, TileSize)
				{
					Frame = GetMapFrame(x, y),
					Anchor = new(0.5f, 0.5f),
				};
				AddChild(mapTile);
				map[x + y * MapWidth] = new Tile(x, y, GetMapTileType(x, y), mapTile);
			}
		}

		//	players vehicle		
		vehicle = new Vehicle(atlas, new(MapWidth / 2, 0), TileSize, map)
		{		
			Pivot = new(0.5f, 1f),
			Anchor = new(0.5f, 0f),
		};
		AddChild(vehicle);

		//	light spot mask around vehicle	
		var spotMask = new VehicleSpotMask(atlas, FrameSpotMask, TileSize * 0.25f, TileSize * 2f)
		{
			Position = new(-TileSize, TileSize),
			Width = (MapWidth + 2) * TileSize,
			Height = (MapHeight + 2) * TileSize,
		};
		AddChild(spotMask);
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
		if (vehicle.Position.X < TileSize)
			vehicle.Position = new(TileSize, vehicle.Position.Y);
		if (vehicle.Position.X > TileSize * (MapWidth - 2))
			vehicle.Position = new(TileSize * (MapWidth - 2), vehicle.Position.Y);
		if (vehicle.Position.Y < 0)
			vehicle.Position = new(vehicle.Position.X, 0);
		if (vehicle.Position.Y > TileSize * (MapHeight - 1))
			vehicle.Position = new(vehicle.Position.X, TileSize * (MapHeight - 1));

		camera.target = vehicle.Position;
		camera.target.Y -= TileSize / 2;

		backGround.Position = new(vehicle.Position.X - halfOffset.X, -halfOffset.Y - TileSize);
	}

	public override void OnResize()
	{
		halfOffset = new Vector2(ScreenWidth / 2f, ScreenHeight / 2f);
		camera.offset = halfOffset;
		var spotMask = GetChildByName(VehicleSpotMask.NodeName) as VehicleSpotMask;
		spotMask?.UpdateViewport(halfOffset);
	}

	public bool IsTileWalkable((int x, int y) tilePosition)
	{
		if (tilePosition.x < 0 ||
		   tilePosition.x > MapWidth - 1 ||
		   tilePosition.y < 0 ||
		   tilePosition.y > MapHeight - 1) return false;

		var tile = map[tilePosition.x + tilePosition.y * MapWidth];
		return tile.TileType != TileType.Blocker;
	}

	private void RenderMenu()
	{
		DrawRectangle(5, 5, 300, 105, BLACK);
		DrawFPS(15, 10);
		DrawText("resolution:", 15, 40, 20, YELLOW); DrawText($"{ScreenWidth} x {ScreenHeight}", 130, 40, 20, WHITE);
		DrawText("position:", 15, 60, 20, YELLOW); DrawText($"({vehicle.Position.X:N0}, {vehicle.Position.Y:N0})", 130, 60, 20, WHITE);
		DrawText("tile:", 15, 80, 20, YELLOW); DrawText($"({vehicle.Position.X/TileSize:N0}, {vehicle.Position.Y/TileSize:N0})", 130, 80, 20, WHITE);
	}

	private Rectangle GetMapFrame(int x, int y)
	{
		var frame = x == 0 || x == MapWidth - 1 ? FrameGroundBrick :
					y == MapHeight - 1 ? FrameGroundBrick :
					y > 1 ? FrameGroundDirt :
					y == 0 ? FrameGroundLevel :
					x % 3 == 0 ? FrameGroundLine :
					FrameGroundGrass;
		return frame;
	}

	private TileType GetMapTileType(int x, int y)
	{
		var tileType = x == 0 || x == MapWidth - 1 ? TileType.Blocker :
					   y == MapHeight - 1 ? TileType.Blocker :
					   y > 0 ? TileType.Dirt : TileType.Ground;
		return tileType;
	}
}

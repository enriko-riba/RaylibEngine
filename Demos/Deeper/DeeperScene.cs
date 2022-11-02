namespace Deeper;

using RaylibEngine.Components;
using RaylibEngine.SceneManagement;
using System.Numerics;

internal class DeeperScene : Scene
{
	const int TileSize = 48;    //	dimensions of sprite frames inside the texture atlas
	const int VehicleSize = 64; //	dimensions of sprite frames inside the texture atlas
	const int xTiles = 100;
	const int yTiles = 100;

	//---------------------------------
	//	Atlas frame sizes
	//---------------------------------
	private readonly Rectangle FrameGroundLine = new(96, 64, TileSize, TileSize);
	private readonly Rectangle FrameGroundGrass = new(96, 0, TileSize, TileSize);
	private readonly Rectangle FrameGroundDirt = new(96, 192, TileSize, TileSize);
	private readonly Rectangle FrameGroundBrick = new(272, 64, TileSize, TileSize);
	private readonly Rectangle FrameSpotMask = new(0, 192, TileSize, TileSize);

	private readonly TilingSprite backGround;
	private readonly Sprite vehicle;
	private readonly Tile[] map = new Tile[xTiles * yTiles];
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
		SetTextureFilter(atlas, TextureFilter.TEXTURE_FILTER_BILINEAR);
		SetTextureWrap(atlas, TextureWrap.TEXTURE_WRAP_CLAMP);
		
		//	static background texture		
		backGround = new TilingSprite(atlas)
		{
			Frame = new Rectangle(448, 0, 64, 64),
			Width = ScreenWidth,
			Height = ScreenHeight / 2,
			Position = Vector2.Zero
		};
		AddChild(backGround);

		//	ground map
		for (int x = 0; x < xTiles; x++)
		{
			for (int y = 0; y < yTiles; y++)
			{
				var mapTile = new Sprite(atlas, new(x * TileSize, y * TileSize + TileSize / 2), TileSize, TileSize)
				{
					Frame = GetMapFrame(x, y),
					Anchor = new(0.5f, 0.5f),
				};
				AddChild(mapTile);
				map[x + y * xTiles] = new Tile(x, y, GetMapTileType(x, y), mapTile);
			}
		}

		//	players vehicle
		
		vehicle = new Sprite(atlas, new(TileSize * xTiles / 2, 0), TileSize, TileSize)
		{
			Frame = new Rectangle(192, 192, TileSize, TileSize),
			Pivot = new(0.5f, 1f),
			Anchor = new(0.5f, 1f),
		};
		AddChild(vehicle);

		//	light spot mask around vehicle	
		var spotMask = new VehicleSpotMask(atlas, FrameSpotMask, TileSize * 0.25f, TileSize * 2f)
		{
			Position = new(-TileSize, 0),
			Width = (xTiles + 2) * TileSize,
			Height = (yTiles + 2) * TileSize,
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

	public override void OnUpdate(float ellapsedSeconds)
	{
		const float Speed = TileSize * 15;
		//	update vehicle position
		var speed = Speed * ellapsedSeconds;
		vehicle.Position += IsKeyDown(KeyboardKey.KEY_RIGHT) ? new(speed, 0f) :
							IsKeyDown(KeyboardKey.KEY_LEFT) ? new(-speed, 0f) :
							IsKeyDown(KeyboardKey.KEY_UP) ? new(0f, -speed) :
							IsKeyDown(KeyboardKey.KEY_DOWN) ? new(0f, speed) :
							Vector2.Zero;

		if (vehicle.Position.X < TileSize)
			vehicle.Position = new(TileSize, vehicle.Position.Y);

		if (vehicle.Position.X > TileSize * (xTiles - 2))
			vehicle.Position = new(TileSize * (xTiles - 2), vehicle.Position.Y);

		if (vehicle.Position.Y < 0)
			vehicle.Position = new(vehicle.Position.X, 0);

		if (vehicle.Position.Y > TileSize * (yTiles - 1))
			vehicle.Position = new(vehicle.Position.X, TileSize * (yTiles - 1));

		camera.target = vehicle.Position;
		camera.target.Y -= TileSize / 2;

		backGround.Position = new(vehicle.Position.X - halfOffset.X, -halfOffset.Y);
	}

	public override void OnResize()
	{
		halfOffset = new Vector2(ScreenWidth / 2f, ScreenHeight / 2f);
		camera.offset = halfOffset;
		var fow = GetChildByName(VehicleSpotMask.NodeName) as VehicleSpotMask;
		fow?.UpdateViewport(halfOffset);
	}

	private void RenderMenu()
	{
		DrawRectangle(5, 5, 300, 85, BLACK);
		DrawFPS(15, 10);
		DrawText("resolution:", 15, 40, 20, YELLOW); DrawText($"{ScreenWidth} x {ScreenHeight}", 130, 40, 20, WHITE);
		DrawText("position:", 15, 60, 20, YELLOW); DrawText($"({vehicle.Position.X:N0}, {vehicle.Position.Y:N0})", 130, 60, 20, WHITE);
	}

	private Rectangle GetMapFrame(int x, int y)
	{
		var frame = x == 0 || x == xTiles - 1 ? FrameGroundBrick :
					y == yTiles - 1 ? FrameGroundBrick :
					y > 0 ? FrameGroundDirt :
					x % 3 == 0 ? FrameGroundLine :
					FrameGroundGrass;
		return frame;
	}

	private TileType GetMapTileType(int x, int y)
	{
		var tileType = x == 0 || x == xTiles - 1 ? TileType.Blocker :
					   y == yTiles - 1 ? TileType.Blocker :
					   y > 0 ? TileType.Dirt : TileType.Ground;
		return tileType;
	}
}

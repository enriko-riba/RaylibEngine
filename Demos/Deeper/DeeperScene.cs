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
	private readonly Rectangle FrameFow = new(0, 192, 32, 32);

	private readonly TilingSprite backGround;
	private readonly Sprite vehicle;
	private readonly Sprite[] map = new Sprite[xTiles * yTiles];
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

		//	static background texture
		var backTexture = LoadTexture("./Assets/Background/blue.png");
		backGround = new TilingSprite(backTexture)
		{
			Width = ScreenWidth,
			Height = ScreenHeight / 2,
			Position = Vector2.Zero
		};
		AddChild(backGround);

		//	ground map
		var groundAtlas = LoadTexture("./Assets/Terrain.png");
		SetTextureFilter(groundAtlas, TextureFilter.TEXTURE_FILTER_BILINEAR);
		SetTextureWrap(groundAtlas, TextureWrap.TEXTURE_WRAP_MIRROR_REPEAT);
		for (int x = 0; x < xTiles; x++)
		{
			for (int y = 0; y < yTiles; y++)
			{
				var mapTile = new Sprite(groundAtlas)
				{
					Frame = GetMapFrame(x, y),
					Position = new(x * TileSize, y * TileSize + TileSize / 2),
					Anchor = new(0.5f, 0.5f),
					Width = TileSize,
					Height = TileSize,
				};
				AddChild(mapTile);
				map[x + y * xTiles] = mapTile;
			}
		}

		//	players vehicle
		var atlas = LoadTexture("./Assets/spr.png");
		SetTextureFilter(atlas, TextureFilter.TEXTURE_FILTER_BILINEAR);
		vehicle = new Sprite(atlas)
		{
			Frame = new Rectangle(0, 0, VehicleSize, VehicleSize),
			Position = new(TileSize * xTiles / 2, 0),
			Pivot = new(0.5f, 1f),
			Anchor = new(0.5f, 1f),
			Width = TileSize,
			Height = TileSize,
		};
		AddChild(vehicle);

		//	Fog of war		
		var fow = new FowSpot(groundAtlas, FrameFow)
		{
			Position = new(-TileSize, 0),
			Width = (xTiles + 2) * TileSize,
			Height = yTiles * TileSize,
			Name = "Fow"
		};
		AddChild(fow);
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
		var fow = GetChildByName("Fow") as FowSpot;
		fow?.UpdateViewport(halfOffset);
	}

	private void RenderMenu()
	{
		DrawRectangle(5, 5, 300, 80, BLACK);
		DrawFPS(15, 10);
		DrawText($"position: ({vehicle.Position.X:N2}, {vehicle.Position.Y:N2})", 15, 40, 20, YELLOW);
		DrawText($"resolution: {ScreenWidth} x {ScreenHeight}", 15, 60, 20, YELLOW);
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
}

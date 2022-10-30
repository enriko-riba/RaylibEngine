namespace Deeper;

using RaylibEngine;
using RaylibEngine.Components;
using RaylibEngine.SceneManagement;
using System.Numerics;

internal class DeeperScene : Scene
{
	const int MOUSE_BUTTON_LEFT = 0;
	const int MOUSE_BUTTON_RIGHT = 1;

	const int TileSize = 48;    //	dimensions of sprite frames inside the texture atlas
	const int VehicleSize = 64; //	dimensions of sprite frames inside the texture atlas
	const int xTiles = 100;
	const int yTiles = 100;

	//---------------------------------
	//	Atlas frame sizes
	//---------------------------------
	private readonly Rectangle FrameGroundLine = new(96, 64, TileSize, TileSize);
	private readonly Rectangle FrameGroundGrass = new(96, 0, TileSize, TileSize);


	private readonly Sprite vehicle;
	private readonly Sprite[] map = new Sprite[xTiles * yTiles];
	private Camera2D camera;

	public DeeperScene(string name) : base(name)
	{
		WindowTitle = name;
		BackgroundColor = BLACK;
		var w = GetScreenWidth();
		var h = GetScreenHeight();

		camera = new()
		{
			offset = new Vector2(w / 2, h / 2),
			target = Vector2.Zero,
			zoom = 0.5f
		};

		var groundAtlas = LoadTexture("./Assets/Terrain.png");
		SetTextureFilter(groundAtlas, TextureFilter.TEXTURE_FILTER_TRILINEAR);
		SetTextureWrap(groundAtlas, TextureWrap.TEXTURE_WRAP_MIRROR_REPEAT);
		for (int x = 0; x < xTiles; x++)
		{
			for (int y = 0; y < yTiles; y++)
			{
				var mapTile = new Sprite(groundAtlas)
				{
					Frame = x % 3 == 0 ? FrameGroundLine : FrameGroundGrass,
					Position = new(x * TileSize, y * TileSize),
					Anchor = new(0.5f, 0f),
					Width = TileSize,
					Height = TileSize,
				};
				AddChild(mapTile);
				map[x + y * xTiles] = mapTile;
			}
		}

		//	create vehicle
		var atlas = LoadTexture("./Assets/spr.png");
		SetTextureFilter(atlas, TextureFilter.TEXTURE_FILTER_TRILINEAR);
		vehicle = new Sprite(atlas)
		{
			Frame = new Rectangle(0, 0, VehicleSize, VehicleSize),
			Position = new(TileSize * xTiles / 2, 0),
			Pivot = new(0.5f, 0.5f),
			Anchor = new(0.5f, 1f),
			Width = VehicleSize,
			Height = VehicleSize,
		};
		AddChild(vehicle);
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
		//	update sprite position
		vehicle.Position += IsKeyDown(KeyboardKey.KEY_RIGHT) ? new(1f, 0f) :
							IsKeyDown(KeyboardKey.KEY_LEFT) ? new(-1f, 0f) :
							IsKeyDown(KeyboardKey.KEY_UP) ? new(0f, -1f) :
							IsKeyDown(KeyboardKey.KEY_DOWN) ? new(0f, 1f) :
							Vector2.Zero;

		camera.target = vehicle.Position;
	}

	public override void OnResize(int width, int height)
	{	
		camera.offset = new Vector2(width / 2, height / 2);		
	}

	private void RenderMenu()
	{
		DrawRectangle(5, 5, 200, 70, DARKBLUE);
		DrawFPS(10, 10);
		DrawText($"position: ({vehicle.Position.X}, {vehicle.Position.Y})", 10, 40, 20, YELLOW);
	}
}

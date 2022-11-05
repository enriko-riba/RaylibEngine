namespace Deeper.Entities;

using RaylibEngine.Components;

internal class Map
{
	public const int Width = 100;
	public const int Height = 100;
	public const int TileSize = 48;

	private readonly Rectangle FrameGroundLine = new(96, 64, TileSize, TileSize);
	private readonly Rectangle FrameGrass = new(96, 0, TileSize, TileSize);
	private readonly Rectangle FrameDirt = new(96, 192, TileSize, TileSize);
	private readonly Rectangle FrameBrick = new(272, 64, TileSize, TileSize);

	private readonly Rectangle FrameTransparent = new(48, 192, TileSize, TileSize);
	private readonly Tile[] tiles = new Tile[Width * Height];

	public Map(Texture atlas)
	{
		//	ground map
		for (int x = 0; x < Width; x++)
		{
			for (int y = 0; y < Height; y++)
			{
				var mapTile = new Sprite(atlas, new(x * TileSize, y * TileSize + TileSize / 2), TileSize, TileSize)
				{
					Frame = GetMapFrame(x, y),
					Anchor = new(0.5f, 0.5f),
				};				
				tiles[x + y * Width] = new Tile(x, y, GetMapTileType(x, y), mapTile);
			}
		}
	}

	public Tile[] Tiles => tiles;

	public bool IsTileWalkable((int x, int y) tilePosition)
	{
		if (tilePosition.x < 0 ||
		   tilePosition.x > Width - 1 ||
		   tilePosition.y < 0 ||
		   tilePosition.y > Height - 1) return false;

		var tile = tiles[tilePosition.x + tilePosition.y * Width];
		return tile.TileType != TileType.Blocker;
	}

	private Rectangle GetMapFrame(int x, int y)
	{
		var frame = x == 0 || x == Width - 1 ? FrameBrick :
					y == Height - 1 ? FrameBrick :
					y > 1 ? FrameDirt :
					y == 0 ? FrameTransparent :
					x % 3 == 0 ? FrameGroundLine :
					FrameGrass;
		return frame;
	}

	private static TileType GetMapTileType(int x, int y)
	{
		var tileType = x == 0 || x == Width - 1 ? TileType.Blocker :
					   y == Height - 1 ? TileType.Blocker :
					   y > 0 ? TileType.Dirt : TileType.Ground;
		return tileType;
	}
}

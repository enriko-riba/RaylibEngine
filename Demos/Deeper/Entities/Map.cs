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
		for (int x = 0; x < Width; x++)
		{
			for (int y = 0; y < Height; y++)
			{
				var position = new TilePosition(x, y);
				var mapTile = new Sprite(atlas, new(x * TileSize, y * TileSize + TileSize / 2), TileSize, TileSize)
				{
					Frame = GetBasicMapLayoutFrame(position),
					Anchor = new(0.5f, 0.5f),
				};				
				tiles[x + y * Width] = new Tile(position, GetMapTileType(position), mapTile);
			}
		}
	}

	public Tile[] Tiles => tiles;

	/// <summary>
	/// Checks if the given tile position is walkable.
	/// </summary>
	/// <param name="position"></param>
	/// <returns></returns>
	public bool IsTileWalkable(TilePosition position)
	{
		if (position.X < 0 ||
		   position.X > Width - 1 ||
		   position.Y < 0 ||
		   position.Y > Height - 1) return false;

		var tile = tiles[position.X + position.Y * Width];
		return tile.TileType != TileType.Blocker;
	}

	/// <summary>
	/// Gets frames for outer borders, ground level, top row and dirt fill.
	/// </summary>
	/// <param name="x">x tile position</param>
	/// <param name="y">y tile position</param>
	/// <returns></returns>
	private Rectangle GetBasicMapLayoutFrame(TilePosition position)
	{
		var frame = position.X == 0 || position.X == Width - 1 ? FrameBrick :
					position.Y == Height - 1 ? FrameBrick :
					position.Y > 1 ? FrameDirt :
					position.Y == 0 ? FrameTransparent :
					position.X % 3 == 0 ? FrameGroundLine :
					FrameGrass;
		return frame;
	}

	/// <summary>
	/// Gets the basic tile types based on tile position.
	/// </summary>
	/// <param name="x">x tile position</param>
	/// <param name="y">y tile position</param>
	/// <returns></returns>
	private static TileType GetMapTileType(TilePosition position)
	{
		var tileType = position.X == 0 || position.X == Width - 1 ? TileType.Blocker :
					   position.Y == Height - 1 ? TileType.Blocker :
					   position.Y > 0 ? TileType.Dirt :
					   TileType.Ground;
		return tileType;
	}
}

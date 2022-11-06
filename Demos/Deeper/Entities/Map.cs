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
    private readonly Rectangle FrameDigged = new(0, 192, TileSize, TileSize);
    private readonly Rectangle FrameTransparent = new(0, 178, 12, 12);

    private readonly Tile[] tiles = new Tile[Width * Height];

    public Map(Texture atlas)
    {
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                TilePosition position = new(x, y);
                Sprite sprite = new(atlas, new(x * TileSize, y * TileSize + TileSize / 2), TileSize, TileSize)
                {
                    Frame = GetBasicMapLayoutFrame(position),
                    Anchor = new(0.5f, 0.5f),
                };
                tiles[x + y * Width] = new Tile(position, GetMapTileType(position), sprite);
            }
        }
    }

    public Tile[] Tiles => tiles;

    public Tile this[TilePosition tp] => tiles[tp.X + tp.Y * Width];

    /// <summary>
    /// Checks if the given tile position is walkable.
    /// </summary>
    /// <param name="position">tile position</param>
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
    /// Digs a tile at the given position.
    /// </summary>
    /// <param name="position">tile position</param>
    public void Dig(TilePosition position)
    {
        if (position.Y == 0) return;

        var index = position.X + position.Y * Width;
        var tile = tiles[index];
        if (tile.TileType != TileType.Blocker && tile.TileType != TileType.Empty && tile.Sprite is not null)
        {
            // TODO: add switch on tile type for special actions
            tile.Sprite.Frame = FrameDigged;
            tiles[index] = tile with { TileType = TileType.Empty };
        }
    }

    /// <summary>
    /// Gets frames for outer borders, ground level, top row and dirt fill.
    /// </summary>
    /// <param name="position">tile position</param>
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
    /// <param name="position">tile position</param>
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

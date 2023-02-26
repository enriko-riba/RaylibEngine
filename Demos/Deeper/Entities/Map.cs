namespace Deeper.Entities;

using RaylibEngine.Components;

internal class Map
{
    public const int Width = 100;
    public const int Height = 100;
    public const int TileSize = 48;

    /*
        Hardcoded tiles matching the atlas.png content.
     */
    private readonly Rectangle FrameGroundLine = new(96, 64, TileSize, TileSize);
    private readonly Rectangle FrameGrass = new(96, 0, TileSize, TileSize);
    private readonly Rectangle FrameDirt = new(96, 192, TileSize, TileSize);
    private readonly Rectangle FrameBrick = new(272, 64, TileSize, TileSize);
    private readonly Rectangle FrameEmpty = new(0, 192, TileSize, TileSize);
    private readonly Rectangle FrameTransparent = new(0, 178, 12, 12);
    private readonly Rectangle FrameBlocker = new(209, 81, 30, 30);

    private readonly Tile[] tiles = new Tile[Width * Height];
    private readonly Texture atlas;

    public Map(Texture atlas)
    {
        this.atlas = atlas;
        InitMap();
    }

    public Tile[] Tiles => tiles;

    public Tile this[MapLocation location] => tiles[location.X + location.Y * Width];

    /// <summary>
    /// Checks if the given tile position is walkable.
    /// </summary>
    /// <param name="location">tile position</param>
    /// <returns></returns>
    public bool IsTileWalkable(MapLocation location)
    {
        if (location.X < 0 ||
           location.X > Width - 1 ||
           location.Y < 0 ||
           location.Y > Height - 1) return false;

        var tile = tiles[location.X + location.Y * Width];
        return tile.TileType != TileType.Blocker;
    }

    /// <summary>
    /// Digs a tile at the given position.
    /// </summary>
    /// <param name="position">tile position</param>
    public void Dig(MapLocation position)
    {
        if (position.Y <= 0) return;

        var index = position.X + position.Y * Width;
        var tile = tiles[index];
        if (tile.TileType != TileType.Blocker && tile.TileType != TileType.Empty && tile.Sprite is not null)
        {
            // TODO: add switch on tile type for special actions
            tile.Sprite.Frame = FrameEmpty;
            tiles[index] = tile with { TileType = TileType.Empty };
        }
    }

    /// <summary>
    /// Generates blocker tiles at random positions.
    /// </summary>
    /// <param name="blockerCount"></param>
    public void GenerateBlockers(int blockerCount)
    {
        for (var i = 0; i < blockerCount; i++)
        {
            int x, y;
            Tile tile;
            do
            {
                x = Random.Shared.Next(1, Map.Width);
                y = Random.Shared.Next(5, Map.Height);
                tile = tiles[x + y * Width];
            } while (tile.TileType != TileType.Dirt);
            Sprite blocker = new(atlas, new(x * TileSize, y * TileSize + TileSize / 2), TileSize, TileSize)
            {
                Frame = FrameBlocker,
                Anchor = new(0.5f, 0.5f),
            };
            tiles[x + y * Width] = tile with { Sprite = blocker, TileType = TileType.Blocker };
        }
    }

    /// <summary>
    /// Initializes the map.
    /// </summary>
    private void InitMap()
    {
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                MapLocation position = new(x, y);
                Sprite sprite = new(atlas, new(x * TileSize, y * TileSize + TileSize / 2), TileSize, TileSize)
                {
                    Frame = GetBasicMapLayoutFrame(position),
                    Anchor = new(0.5f, 0.5f),
                };
                tiles[x + y * Width] = new Tile(position, GetMapTileType(position), sprite);
            }
        }
    }

    /// <summary>
    /// Gets frames for outer borders, ground level, top row and dirt fill.
    /// </summary>
    /// <param name="position">tile position</param>
    /// <returns></returns>
    private Rectangle GetBasicMapLayoutFrame(MapLocation position)
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
    private static TileType GetMapTileType(MapLocation position)
    {
        var tileType = position.X == 0 || position.X == Width - 1 ? TileType.Blocker :
                       position.Y == Height - 1 ? TileType.Blocker :
                       position.Y > 0 ? TileType.Dirt :
                       TileType.Ground;
        return tileType;
    }
}

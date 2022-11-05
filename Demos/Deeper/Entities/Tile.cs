
using RaylibEngine.Components;

namespace Deeper.Entities;
internal class TilePosition
{
    public TilePosition(int x, int y)
    {
        X = x;
        Y = y;
    }
    public int X { get; set; }
    public int Y { get; set; }
}

internal record Tile(TilePosition Position, TileType TileType, Sprite? Sprite);

internal enum TileType
{
    Empty,
    Ground,
    Dirt,
    Blocker,
}

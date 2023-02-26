namespace Deeper.Entities;

using RaylibEngine.Components;

/// <summary>
/// X and Y coordinates inside a map of tiles.
/// </summary>
internal class MapLocation
{
    public MapLocation(int x, int y)
    {
        X = x;
        Y = y;
    }
    public int X { get; set; }
    public int Y { get; set; }
    public override string ToString() => $"{X}:{Y}";
}

internal record Tile(MapLocation MapLocation, TileType TileType, Sprite? Sprite);

internal enum TileType
{
    Empty,
    Ground,
    Dirt,
    Blocker,
}

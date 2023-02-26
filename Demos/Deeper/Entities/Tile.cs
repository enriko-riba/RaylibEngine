namespace Deeper.Entities;

using RaylibEngine.Components;
using System.Diagnostics;

/// <summary>
/// X and Y coordinates inside a map of tiles.
/// </summary>
[DebuggerDisplay("x={X} y={Y}")]
internal class MapLocation
{
    public MapLocation(int x, int y)
    {
        X = x;
        Y = y;
    }
    public int X { get; set; }
    public int Y { get; set; }
}

internal record Tile(MapLocation MapLocation, TileType TileType, Sprite? Sprite);

internal enum TileType
{
    Empty,
    Ground,
    Dirt,
    Blocker,
}

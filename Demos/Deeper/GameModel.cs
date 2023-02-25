using Deeper.Entities;
using System.Numerics;

namespace Deeper;

internal class GameModel
{
    public GameModel(Map map)
    {
        Map = map;
    }

    public float Depth => TransitionPosition.Y * Map.TileSize;

    public Map Map { get; init; }

    public TilePosition VehicleTilePosition = new(0, 0);
    public Vector2 TransitionPosition { get; set; }

    public static Direction GetDirectionInput()
    {
        return IsKeyDown(KeyboardKey.KEY_RIGHT) ? Direction.East :
                IsKeyDown(KeyboardKey.KEY_LEFT) ? Direction.West :
                IsKeyDown(KeyboardKey.KEY_UP) ? Direction.North :
                IsKeyDown(KeyboardKey.KEY_DOWN) ? Direction.South : Direction.None;
    }

    public static TilePosition GetNextTilePosition(TilePosition position, Direction direction)
    {
        var x = position.X + direction switch
        {
            Direction.West => -1,
            Direction.East => 1,
            _ => 0
        };
        var y = position.Y + direction switch
        {
            Direction.North => -1,
            Direction.South => 1,
            _ => 0
        };
        return new TilePosition(x, y);
    }

    public enum Direction
    {
        None,
        North,
        East,
        South,
        West
    }
}

using Deeper.Entities;
using System.Numerics;

namespace Deeper;

internal class GameModel
{
    public const float Speed = Map.TileSize * 2.5f;

    public GameModel(Map map)
    {
        Map = map;
        map.GenerateBlockers(500);
        VehicleLocation = new(Map.Width / 2, 0);
    }

    /// <summary>
    /// World map.
    /// </summary>
    public Map Map { get; init; }

    /// <summary>
    /// Vehicle tile grid location.
    /// </summary>
    public MapLocation VehicleLocation;

    /// <summary>
    /// Position for moving between two tiles.
    /// </summary>
    public Vector2 TransitionPosition { get; set; }

    /// <summary>
    /// Digger depth.
    /// </summary>
    public float Depth => TransitionPosition.Y / 100.0f;

    #region helper methods
    public static Direction GetDirectionInput()
    {
        return IsKeyDown(KeyboardKey.KEY_RIGHT) ? Direction.East :
                IsKeyDown(KeyboardKey.KEY_LEFT) ? Direction.West :
                IsKeyDown(KeyboardKey.KEY_UP) ? Direction.North :
                IsKeyDown(KeyboardKey.KEY_DOWN) ? Direction.South : Direction.None;
    }

    public static MapLocation GetAdjacentLocation(MapLocation currentMapLocation, Direction direction)
    {
        var x = currentMapLocation.X + direction switch
        {
            Direction.West => -1,
            Direction.East => 1,
            _ => 0
        };
        var y = currentMapLocation.Y + direction switch
        {
            Direction.North => -1,
            Direction.South => 1,
            _ => 0
        };
        return new MapLocation(x, y);
    }
    #endregion   
}

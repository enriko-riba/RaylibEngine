using Deeper.Entities;
using System.Numerics;

namespace Deeper;

internal class GameModel
{
    public const float Speed = Map.TileSize*4;

    public GameModel(Map map)
    {
        Map = map;
        map.GenerateBlockers(1500);
        VehicleTilePosition = new(Map.Width / 2, 0);
    }
    
    /// <summary>
    /// World map.
    /// </summary>
    public Map Map { get; init; }

    /// <summary>
    /// Tile coordinates.
    /// </summary>
    public TilePosition VehicleTilePosition;

    /// <summary>
    /// Position for moving between two tiles.
    /// </summary>
    public Vector2 TransitionPosition { get; set; }

    /// <summary>
    /// Digger depth.
    /// </summary>
    public float Depth => TransitionPosition.Y;

    #region helper methods
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
    #endregion   
}

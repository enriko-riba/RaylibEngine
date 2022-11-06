namespace Deeper.Entities;

using RaylibEngine.Components;
using RaylibEngine.Core;
using System.Numerics;

internal class Vehicle : Sprite, IUpdateable
{
    const float Speed = Map.TileSize;

    private readonly Dictionary<Direction, Rectangle> frames = new();
    private readonly Map map;
    private readonly Sprite diggingBackground;

    private Direction direction;
    private readonly TilePosition currentTilePosition = new(0, 0);
    private TilePosition destinationTilePosition = new(0, 0);
    private Vector2 destinationPosition;
    private Vector2 transitionPosition;

    public Vehicle(Texture texture, TilePosition tilePosition, Map map)
        : base(texture, new(tilePosition.X * Map.TileSize, tilePosition.Y * Map.TileSize), Map.TileSize, Map.TileSize)
    {
        currentTilePosition = tilePosition;
        this.map = map;
        frames[Direction.West] = new(416, 160, Map.TileSize, Map.TileSize);
        frames[Direction.East] = new(464, 160, Map.TileSize, Map.TileSize);
        frames[Direction.North] = new(416, 208, Map.TileSize, Map.TileSize);
        frames[Direction.South] = new(464, 208, Map.TileSize, Map.TileSize);
        Frame = frames[Direction.East];
        Name = "Vehicle";

        diggingBackground = new Sprite(texture, Vector2.Zero, Map.TileSize + 6, Map.TileSize + 6)
        {
            Name = "DiggingBack",
            Frame = new(392, 96, Map.TileSize, Map.TileSize),
            Anchor = new(0.5f, 0f),
            Visible = false,
        };
        AddChild(diggingBackground);
    }

    public void Update(float ellapsedSeconds)
    {
        if (direction == Direction.None)
        {
            //	if we here is a movement request set destination and sprite frame
            var dir = GetDirectionInput();
            if (dir != Direction.None)
            {
                destinationTilePosition = GetNextTilePosition(currentTilePosition, dir);
                if (map.IsTileWalkable(destinationTilePosition))
                {
                    Frame = frames[dir];
                    destinationPosition = new Vector2(destinationTilePosition.X, destinationTilePosition.Y) * Map.TileSize;
                    transitionPosition = Position;
                    direction = dir;
                    var tileType = map[destinationTilePosition].TileType;
                    diggingBackground.Visible = tileType != TileType.Empty && tileType != TileType.Blocker && tileType != TileType.Ground;
                }                
            }
        }

        if (direction != Direction.None)
        {
            //	update vehicle position
            var distance = Vector2.Distance(Position, destinationPosition);
            if (distance > 0)
            {
                ApplyMovement(ellapsedSeconds, distance);
            }
            else
            {
                direction = Direction.None;
                currentTilePosition.X = (int)(destinationPosition.X / Map.TileSize);
                currentTilePosition.Y = (int)(destinationPosition.Y / Map.TileSize);
                Position = destinationPosition;
                map.Dig(destinationTilePosition);
                diggingBackground.Visible = false;
            }
        }
    }

    private void ApplyMovement(float ellapsedSeconds, float distance)
    {
        //	velocity depends on tile type, empty and above ground tiles get bonus velocity
        var nextTileType = map[destinationTilePosition].TileType;
        var velocity = Speed * (nextTileType == TileType.Empty || nextTileType == TileType.Ground ? 2.5f : 1);

        //	limit travel distance to destination distance
        var movement = Math.Min(velocity * ellapsedSeconds, distance);

        var tmp = transitionPosition;
        tmp += direction switch
        {
            Direction.East => new(movement, 0f),
            Direction.West => new(-movement, 0f),
            Direction.North => new(0f, -movement),
            Direction.South => new(0f, movement),
            _ => Vector2.Zero
        };
        transitionPosition = tmp;

        // vehicle position is rounded to avoid issues with tile border flickering
        // if rounded coordinates differ from sprite position update sprite
        tmp.X = (float)Math.Round(tmp.X);
        tmp.Y = (float)Math.Round(tmp.Y);
        if (tmp.X != Position.X || tmp.Y != Position.Y)
        {
            Position = tmp;
        }
    }

    private static Direction GetDirectionInput()
    {
        return IsKeyDown(KeyboardKey.KEY_RIGHT) ? Direction.East :
                IsKeyDown(KeyboardKey.KEY_LEFT) ? Direction.West :
                IsKeyDown(KeyboardKey.KEY_UP) ? Direction.North :
                IsKeyDown(KeyboardKey.KEY_DOWN) ? Direction.South : Direction.None;
    }

    private static TilePosition GetNextTilePosition(TilePosition position, Direction direction)
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
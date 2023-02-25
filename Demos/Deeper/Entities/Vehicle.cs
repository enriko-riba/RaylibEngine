namespace Deeper.Entities;

using RaylibEngine.Components;
using RaylibEngine.Core;
using System.Numerics;
using static Deeper.GameModel;

internal class Vehicle : Sprite, IUpdateable
{
    const float Speed = Map.TileSize;

    private readonly Dictionary<Direction, Rectangle> frames = new();
    private readonly Sprite diggingBackground;
    private readonly GameModel gameModel;

    private Direction direction;
    private TilePosition destinationTilePosition = new(0, 0);
    private Vector2 destinationPosition;

    public Vehicle(Texture texture, GameModel gameModel)
        : base(texture, new(gameModel.VehicleTilePosition.X * Map.TileSize, gameModel.VehicleTilePosition.Y * Map.TileSize), Map.TileSize, Map.TileSize)
    {
        this.gameModel = gameModel;
        frames[Direction.West] = new(416, 160, Map.TileSize, Map.TileSize);
        frames[Direction.East] = new(464, 160, Map.TileSize, Map.TileSize);
        frames[Direction.North] = new(416, 208, Map.TileSize, Map.TileSize);
        frames[Direction.South] = new(464, 208, Map.TileSize, Map.TileSize);
        Frame = frames[Direction.East];
        Name = "Vehicle";

        diggingBackground = new Sprite(texture, Vector2.Zero, Map.TileSize, Map.TileSize)
        {
            Name = "DiggingBack",
            Frame = new(392, 96, Map.TileSize, Map.TileSize),
            Anchor = new(0.5f, 1f),
            Visible = false,
        };
        AddChild(diggingBackground);
    }

    public void Update(float elapsedSeconds)
    {
        if (direction == Direction.None)
        {
            //	if there is a movement request set destination and sprite frame
            var dir = GetDirectionInput();
            if (dir != Direction.None)
            {
                destinationTilePosition = GetNextTilePosition(gameModel.VehicleTilePosition, dir);
                if (gameModel.Map.IsTileWalkable(destinationTilePosition))
                {
                    Frame = frames[dir];
                    destinationPosition = new Vector2(destinationTilePosition.X, destinationTilePosition.Y) * Map.TileSize;
                    gameModel.TransitionPosition = Position;
                    direction = dir;
                    var tileType = gameModel.Map[destinationTilePosition].TileType;
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
                ApplyMovement(elapsedSeconds, distance);
            }
            else
            {
                direction = Direction.None;
                gameModel.VehicleTilePosition.X = (int)(destinationPosition.X / Map.TileSize);
                gameModel.VehicleTilePosition.Y = (int)(destinationPosition.Y / Map.TileSize);
                Position = destinationPosition;
                gameModel.Map.Dig(destinationTilePosition);
                diggingBackground.Visible = false;
            }
        }
    }

    private void ApplyMovement(float elapsedSeconds, float distance)
    {
        //	velocity depends on tile type, empty and above ground tiles get bonus velocity
        var nextTileType = gameModel.Map[destinationTilePosition].TileType;
        var velocity = Speed * (nextTileType == TileType.Empty || nextTileType == TileType.Ground ? 2.5f : 1);

        //	limit travel distance to destination distance
        var movement = Math.Min(velocity * elapsedSeconds, distance);

        //var tmp = transitionPosition;
        gameModel.TransitionPosition += direction switch
        {
            Direction.East => new(movement, 0f),
            Direction.West => new(-movement, 0f),
            Direction.North => new(0f, -movement),
            Direction.South => new(0f, movement),
            _ => Vector2.Zero
        };
        //transitionPosition = tmp;

        // vehicle position is rounded to avoid issues with tile border flickering
        var tmp = gameModel.TransitionPosition;
        tmp.X = (float)Math.Round(tmp.X);
        tmp.Y = (float)Math.Round(tmp.Y);
        Position = tmp;
    }
}
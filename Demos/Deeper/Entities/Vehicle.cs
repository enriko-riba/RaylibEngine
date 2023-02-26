namespace Deeper.Entities;

using RaylibEngine.Components;
using RaylibEngine.Core;
using System.Numerics;
using static Deeper.GameModel;

internal class Vehicle : Sprite, IUpdateable
{
    private readonly Dictionary<Direction, Rectangle> frames = new();
    private readonly Sprite diggingBackground;
    private readonly GameModel gameModel;

    private MapLocation destinationLocation = new(0, 0);
    private Vector2 destinationPosition;
    private MoveState moveState;

    public Vehicle(Texture texture, GameModel gameModel)
        : base(texture, new(gameModel.VehicleLocation.X * Map.TileSize, gameModel.VehicleLocation.Y * Map.TileSize), Map.TileSize, Map.TileSize)
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
        moveState = new MoveState(MovePhase.Idle, Direction.None, gameModel.VehicleLocation, null, null);
    }

    public void Update(float elapsedSeconds)
    {
        var requestedDirection = GetDirectionInput();
        HandleMoveRequest(requestedDirection);
        HandleMoveState(elapsedSeconds);

        gameModel.VehicleLocation.X = (int)(Position.X / Map.TileSize);
        gameModel.VehicleLocation.Y = (int)(Position.Y / Map.TileSize);
    }

    /// <summary>
    /// Prepares movement and returns the new direction.
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    private void HandleMoveRequest(Direction dir)
    {
        if (dir == Direction.None || dir == moveState.RequestedDirection)
            return;

        //-------------------------------------------------
        //  if here there a direction change is requested
        //-------------------------------------------------

        //  exit if un-cancelable action is in progress
        if (moveState.Phase == MovePhase.Moving)
            return;

        // exit if trying to move on non walkable tile
        destinationLocation = GetAdjacentLocation(gameModel.VehicleLocation, dir);
        if (!gameModel.Map.IsTileWalkable(destinationLocation))
        {
            moveState = moveState with { Phase = MovePhase.Idle, RequestedDirection = Direction.None, EndLocation = null, EndsAt = null };
            return;
        }

        var tileType = gameModel.Map[destinationLocation].TileType;
        diggingBackground.Visible = false;
        switch (tileType)
        {
            case TileType.Empty:
            case TileType.Ground:
                moveState = moveState with
                {
                    Phase = MovePhase.Moving,
                    RequestedDirection = dir,
                    StartLocation = gameModel.VehicleLocation,
                    EndLocation = destinationLocation,
                    EndsAt = null
                };
                Tint = WHITE;
                break;
            case TileType.Dirt:
                moveState = moveState with
                {
                    Phase = MovePhase.DigRequest,
                    RequestedDirection = dir,
                    StartLocation = gameModel.VehicleLocation,
                    EndLocation = destinationLocation,
                    EndsAt = DateTime.Now.AddMilliseconds(750)
                };
                diggingBackground.Visible = true;
                Tint = ORANGE;
                break;
        }

        //moveState = moveState with { Phase = MovePhase.Idle, EndLocation = null, EndsAt = null };
        Frame = frames[dir];
        destinationPosition = new Vector2(destinationLocation.X, destinationLocation.Y) * Map.TileSize;
        gameModel.TransitionPosition = Position;
        return;

    }

    private void HandleMoveState(float elapsedSeconds)
    {
        switch (moveState.Phase)
        {
            case MovePhase.Idle:
                //  fall or do nothing
                var belowLocation = GetAdjacentLocation(gameModel.VehicleLocation, Direction.South);
                var tile = gameModel.Map[belowLocation];
                if (tile.TileType == TileType.Empty)
                {
                    moveState = moveState with { Phase = MovePhase.FreeFall, EndLocation = belowLocation, EndsAt = null };
                }
                break;

            case MovePhase.FreeFall:
                var tmp = Position;
                tmp.Y++;
                gameModel.TransitionPosition = tmp;
                Position = tmp;
                if (Position.Y >= moveState.EndLocation?.Y * Map.TileSize)
                {
                    moveState = moveState with { Phase = MovePhase.Idle, EndLocation = null, EndsAt = null };
                }
                break;

            case MovePhase.Moving:
                //  update position
                var distance = Vector2.Distance(Position, destinationPosition);
                if (distance > 0.2)
                {
                    ApplyMovement(elapsedSeconds, distance);
                }
                else
                {
                    gameModel.TransitionPosition = Position;
                    gameModel.VehicleLocation.X = (int)(destinationPosition.X / Map.TileSize);
                    gameModel.VehicleLocation.Y = (int)(destinationPosition.Y / Map.TileSize);
                    Position = destinationPosition;
                    gameModel.Map.Dig(destinationLocation);
                    diggingBackground.Visible = false;
                    moveState = moveState with { Phase = MovePhase.Idle, RequestedDirection = Direction.None, EndLocation = null, EndsAt = null };
                }
                break;

            case MovePhase.DigRequest:
                //  vertically center with destination tile, check for state completion time and transition to moving
                if (moveState.EndsAt < DateTime.Now)
                {
                    moveState = moveState with { Phase = MovePhase.Moving, EndsAt = null };
                    Tint = WHITE;
                }
                break;
        }
    }

    private void ApplyMovement(float elapsedSeconds, float distance)
    {
        //	velocity depends on tile type, empty and above ground tiles have no penalty while digging depends on (TODO: drill, engine etc)
        var nextTileType = gameModel.Map[destinationLocation].TileType;
        var velocity = Speed * (nextTileType == TileType.Empty || nextTileType == TileType.Ground ? 1 : 0.25);

        //	limit travel distance to destination distance
        var movement = (float)Math.Min(velocity * elapsedSeconds, distance);

        //  create normalized translation vector
        var dx = destinationPosition.X - gameModel.TransitionPosition.X;
        var dy = destinationPosition.Y - gameModel.TransitionPosition.Y;
        var nx = dx / distance;
        var ny = dy / distance;

        //  translate vector [dx, dy]
        dx = nx * movement;
        dy = ny * movement;

        gameModel.TransitionPosition += moveState.RequestedDirection switch
        {
            Direction.East => new(dx, dy),
            Direction.West => new(dx, dy),
            Direction.North => new(0f, dy),
            Direction.South => new(0f, dy),
            _ => Vector2.Zero
        };

        // vehicle position is rounded to avoid issues with tile border flickering
        var tmp = gameModel.TransitionPosition;
        tmp.X = (float)Math.Round(tmp.X);
        tmp.Y = (float)Math.Round(tmp.Y);
        Position = tmp;
    }
}
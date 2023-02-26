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
        //if (direction == Direction.None)
        //{
        //    //  read user input if not moving
        //    //var requestedDirection = GetDirectionInput();
        //    //direction = HandleMoveRequest(requestedDirection);

        //    //if (requestedDirection == Direction.None)
        //    //{
        //    //    //  simulate falling if no tile bellow 
        //    //    var belowLocation = GetAdjacentLocation(gameModel.VehicleLocation, Direction.South);
        //    //    var tile = gameModel.Map[belowLocation];
        //    //    if (tile.TileType == TileType.Empty)
        //    //    {
        //    //        var tmp = Position;
        //    //        tmp.Y++;
        //    //        Position = tmp;
        //    //        gameModel.TransitionPosition = tmp;
        //    //        moveState = moveState with { Phase = MovePhase.FreeFall, EndLocation = null, EndsAt = null };
        //    //    }
        //    //}
        //}
        var requestedDirection = GetDirectionInput();
        HandleMoveRequest(requestedDirection);
        HandleMoveState(elapsedSeconds);

        //if (direction != Direction.None)
        //{
        //    //	update vehicle position
        //    var distance = Vector2.Distance(Position, destinationPosition);
        //    if (distance > 0)
        //    {
        //        ApplyMovement(elapsedSeconds, distance);
        //    }
        //    else
        //    {
        //        direction = Direction.None;
        //        gameModel.TransitionPosition = Position;
        //        gameModel.VehicleLocation.X = (int)(destinationPosition.X / Map.TileSize);
        //        gameModel.VehicleLocation.Y = (int)(destinationPosition.Y / Map.TileSize);
        //        Position = destinationPosition;
        //        gameModel.Map.Dig(destinationTilePosition);
        //        diggingBackground.Visible = false;
        //    }
        //}

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
        {
            //  start digging by converting dig request into movement
            if (moveState.Phase == MovePhase.DigRequest && moveState.EndsAt < DateTime.Now)
            {
                moveState = moveState with { Phase = MovePhase.Moving, EndsAt = null };
            }
            return;
        }

        //-------------------------------------------------
        //  if here there a direction change is requested
        //-------------------------------------------------

        //  check if un-cancelable action is in progress
        if (moveState.Phase == MovePhase.Moving)
            return;

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
                break;
            case TileType.Dirt:
                moveState = moveState with
                {
                    Phase = MovePhase.DigRequest,
                    RequestedDirection = dir,
                    StartLocation = gameModel.VehicleLocation,
                    EndLocation = destinationLocation,
                    EndsAt = DateTime.Now.AddMilliseconds(250)
                };
                diggingBackground.Visible = true;
                break;
        }

        if (dir == Direction.None)
        {
            return;
        }
        else
        {
            //moveState = moveState with { Phase = MovePhase.Idle, EndLocation = null, EndsAt = null };
            Frame = frames[dir];
            destinationPosition = new Vector2(destinationLocation.X, destinationLocation.Y) * Map.TileSize;
            gameModel.TransitionPosition = Position;
            return;
        }
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
                if (distance > 0)
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

        //gameModel.TransitionPosition = new Vector2(Position.X + dx, Position.Y + dy);
        //var tmp = transitionPosition;
        gameModel.TransitionPosition += moveState.RequestedDirection switch
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
using Deeper.Entities;

namespace Deeper;

internal record MoveState(MovePhase Phase, Direction RequestedDirection, MapLocation StartLocation, MapLocation? EndLocation, DateTime? EndsAt);

internal enum MovePhase
{
    /// <summary>
    /// Standing idle.
    /// </summary>
    Idle,

    /// <summary>
    /// Moving on ground, through empty area or digging.
    /// Note: the action can not be canceled until the vehicle gets to target tile.
    /// </summary>
    Moving,

    /// <summary>
    /// Falling into abyss.
    /// Note: the action can be canceled by requesting movement in any direction.
    /// </summary>
    FreeFall,

    /// <summary>
    /// Movement requested into a non empty tile. This phase is used to vertically center the vehicle with the tile.
    /// Note: the action can be canceled by requesting movement in another direction.
    /// </summary>
    DigRequest
}

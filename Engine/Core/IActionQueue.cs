namespace RaylibEngine.Core;

internal interface IActionQueue
{
    Queue<Action> QueuedActions { get; }
}

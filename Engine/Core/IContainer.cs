namespace RaylibEngine.Core;

/// <summary>
/// Supports entity container operations.
/// </summary>
public interface IContainer
{
    string? Name { get; init; }
    IContainer? Parent { get; set; }
    void RemoveChild(IContainer child);
    void AddChild(IContainer child);
    IEnumerable<IContainer> Children { get; }
}

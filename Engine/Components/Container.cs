using RaylibEngine.Core;

namespace RaylibEngine.Components;
/// <summary>
/// Basic container implementation. The container is a collection of child nodes..
/// </summary>
public class Container : IContainer, IActionQueue
{
    private readonly List<IContainer> children = new();
    private readonly Queue<Action> queuedActions = new();
    private IContainer? parent;

    protected event Action? OnParentChanged;

    public IContainer? Parent
    {
        get => parent;
        set
        {
            parent = value;
            OnParentChanged?.Invoke();
        }
    }

    public IEnumerable<IContainer> Children => children;

    public string? Name { get; init; }

    /// <summary>
    /// Removes all children during the next update phase.    
    /// </summary>
    public void RemoveAllChildren()
    {
        var copy = children.ToList();
        queuedActions.Enqueue(() =>
        {
            foreach (var child in copy)
            {
                child.Parent = null;
            }
            copy.Clear();
        });
    }

    /// <summary>
    /// Removes the child from container node graph.
    /// </summary>
    /// <param name="child"></param>
    public void RemoveChild(IContainer child)
    {
        queuedActions.Enqueue(() =>
        {
            children.Remove(child);
            child.Parent = null;
        });
    }

    public void AddChild(IContainer child)
    {
        if (child.Parent != null && child.Parent != this) child.Parent.RemoveChild(child);
        if (child.Parent != this)
        {
            children.Add(child);
            child.Parent = this;
        }
    }

    public void AddChildAt(IContainer child, int index)
    {
        if (index < 0 || index >= children.Count) throw new ArgumentOutOfRangeException(nameof(index));
        if (child.Parent != null && child.Parent != this) child.Parent.RemoveChild(child);
        if (child.Parent != this)
        {
            children.Insert(index, child);
            child.Parent = this;
        }
    }

    public IContainer? GetChildByName(string name)
    {
        return children.FirstOrDefault(c => c.Name == name);
    }

    public Queue<Action> QueuedActions => queuedActions;

    public override string ToString() => $"name: '{Name}', children: {children.Count}";

}
using RaylibEngine.Core;

namespace RaylibEngine.Components;
/// <summary>
/// Basic container implementation. The container is a collection of child nodes..
/// </summary>
public class Container : IContainer, IActionQueue
{
    private readonly List<IContainer> children = new();
    private readonly Queue<Action> queuedActions = new();

    public IContainer? Parent { get; set; }
    public IEnumerable<IContainer> Children => children;

    public string? Name { get; init; }

    public void RemoveAllChildren()
    {
        queuedActions.Enqueue(() =>
        {
            foreach (var child in children)
            {
                child.Parent = null;
            }
            children.Clear();
        });
    }

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
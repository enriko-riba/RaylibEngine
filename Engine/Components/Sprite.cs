namespace RaylibEngine.Components;

using Raylib_CsLo;
using RaylibEngine.Core;
using System.Numerics;

/// <summary>
/// Basic 2D sprite, renders a texture or part of it and enables positioning, anchoring, scaling, pivoting, rotations and tinting.
/// </summary>
public class Sprite : SceneNode2D
{
    public Sprite(Texture texture) : base()
    {
        Visible = true;
        Texture = texture;
        frame = new(0, 0, texture.width, texture.height);
        aabb = new(0, 0, texture.width, texture.height);
        width = texture.width;
        height = texture.height;
        IsDirty = true;
        OnParentChanged += HandleOnParentChanged;
    }

    public Sprite(Texture texture, Vector2 position) : this(texture)
    {
        Position = new(position.X, position.Y);
    }

    public Sprite(Texture texture, Vector2 position, int width, int height) : this(texture, position)
    {
        this.width = width;
        this.height = height;
        IsDirty = true;
    }

    /// <summary>
    /// Renders the sprite texture.
    /// </summary>
    public override void Draw()
    {
        if (Parent is IDrawable2D drawable)
        {
            var tmp = new Vector2(drawable.Dst.X, drawable.Dst.Y) + Position;
            if (tmp != worldPosition)
            {
                worldPosition = tmp;
                IsDirty = true;
            }
        }

        if (IsDirty)
        {
            UpdateDestinationRectangle();
        }
        Raylib.DrawTexturePro(Texture, Frame, Dst, origin, Angle, Tint);
    }

    private void HandleOnParentChanged()
    {
        IsDirty = true;
    }

    protected void UpdateDestinationRectangle()
    {
        var ax = Helpers.Lerp(0, width, anchor.X);
        var ay = Helpers.Lerp(0, height, anchor.Y);
        var px = Helpers.Lerp(0, width, pivot.X);
        var py = Helpers.Lerp(0, height, pivot.Y);
        origin = new(px, py);
        dst = new Rectangle(worldPosition.X + px - ax, worldPosition.Y + py - ay, Width, Height);
        aabb = dst.CalcAabbForRotation(new(dst.X + px, dst.Y + py), Angle);
        aabb.x -= origin.X;
        aabb.y -= origin.Y;
        IsDirty = false;
    }

    public override string ToString() => $"{base.ToString()}, texture id: {Texture.id}, frame: ({Frame.x},{Frame.y},{Frame.width},{Frame.height})";
}

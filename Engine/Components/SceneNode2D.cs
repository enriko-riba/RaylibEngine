namespace RaylibEngine.Components;

using Raylib_CsLo;
using RaylibEngine.Core;
using System.Numerics;

public abstract class SceneNode2D : SceneNode, IDrawable2D
{
    protected Vector2 worldPosition = Vector2.Zero;
    protected float worldAngle = 0;

    protected Rectangle dst;
    protected Vector2 position = Vector2.Zero;
    protected Vector2 pivot = Vector2.Zero;
    protected Vector2 anchor = Vector2.Zero;
    protected int width;
    protected int height;
    protected Vector2 origin;
    protected float angle;
    protected Rectangle aabb;
    protected Rectangle frame;

    /// <summary>
    /// Returns the texture destination rectangle.
    /// </summary>
    public Rectangle Dst => dst;

    /// <summary>
    /// Axis aligned bounding box.
    /// </summary>
    public Rectangle Aabb => aabb;
   

    /// <summary>
    /// The texture to be rendered.
    /// </summary>
    public Texture Texture { get; set; }

    /// <summary>
    /// Sprite position.
    /// </summary>
    public Vector2 Position
    {
        get => position;
        set
        {
            if (position != value)
            {
                position = value;
                worldPosition = position;                
                IsDirty = true;
            }
        }
    }

    /// <summary>
    /// Rotation in degrees.
    /// </summary>
    public float Angle
    {
        get => angle;
        set
        {
            if(angle != value)
            {
                angle = value;
                worldAngle = angle;
                IsDirty = true;
            }
        }
    }

    /// <summary>
    /// Color filter. Default is white which equals to no filtering.
    /// </summary>
    public Color Tint { get; set; } = Raylib.WHITE;

    /// <summary>
    /// The source rectangle inside the texture that is rendered.
    /// Default is the whole texture.
    /// </summary>
    public Rectangle Frame
    {
        get => frame;
        set
        {
            frame = value;
            IsDirty = true;
            if (width == 0) width = (int)frame.width;
            if (height == 0) height = (int)frame.height;
        }
    }

    /// <summary>
    /// Center of rotation.
    /// </summary>
    public Vector2 Pivot
    {
        get => pivot;
        set
        {
            if (value.X < 0 || value.X > 1 || value.Y < 0 || value.Y > 1) throw new ArgumentOutOfRangeException(nameof(Pivot), "valid range is 0 to 1!");
            pivot = value;
            IsDirty = true;
        }
    }

    /// <summary>
    /// Node offset in regard to its position.
    /// </summary>
    public Vector2 Anchor
    {
        get => anchor;
        set
        {
            if (value.X < 0 || value.X > 1 || value.Y < 0 || value.Y > 1) throw new ArgumentOutOfRangeException(nameof(Anchor), "valid range is 0 to 1!");
            anchor = value;
            IsDirty = true;
        }
    }

    /// <summary>
    /// Sprite rendering width in pixels. This effectively scales the sprite.
    /// </summary>
    public int Width
    {
        get => width;
        set
        {
            width = value;
            IsDirty = true;
        }
    }

    /// <summary>
    /// Sprite rendering height in pixels. This effectively scales the sprite.
    /// </summary>
    public int Height
    {
        get => height;
        set
        {
            height = value;
            IsDirty = true;
        }
    }
}
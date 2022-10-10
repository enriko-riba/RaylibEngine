namespace RaylibEngine.Core;

using Raylib_CsLo;
using System.Numerics;

public class Sprite : Container, IDrawable
{
    private Rectangle dst;
    private Vector2 position = Vector2.Zero;
    private Vector2 pivot = Vector2.Zero;
    private Vector2 anchor = Vector2.Zero;
    private int width;
    private int height;
	private Vector2 origin;

    public Sprite(Texture texture) : base()
    {
        Texture = texture;
        Frame = new(0, 0, texture.width, texture.height);
        Width = texture.width;
        Height = texture.height;
		Pivot = Vector2.Zero;
    }

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
        set { position = value; UpdateDestinationRectangle(); }
    }

    /// <summary>
    /// Rotation in degrees.
    /// </summary>
    public float Angle { get; set; }

    /// <summary>
    /// Color filter. Default is white which equals to no filtering.
    /// </summary>
    public Color Tint { get; set; } = Raylib.WHITE;

    /// <summary>
    /// The source rectangle inside the texture that is rendered.
    /// Default is the whole texture.
    /// </summary>
    public Rectangle Frame { get; set; }

    /// <summary>
    /// Center of rotation.
    /// </summary>
    public Vector2 Pivot
    {
        get => pivot;
        set {
			if (value.X < 0 || value.X > 1 || value.Y < 0 || value.Y > 1) throw new ArgumentOutOfRangeException(nameof(Pivot), "valid range is 0 to 1!");
			pivot = value;
			UpdateDestinationRectangle(); 
		}
    }

    /// <summary>
    /// Center of rotation.
    /// </summary>
    public Vector2 Anchor
    {
        get => anchor;
        set
        {
            if (value.X < 0 || value.X > 1 || value.Y < 0 || value.Y > 1) throw new ArgumentOutOfRangeException(nameof(Anchor), "valid range is 0 to 1!");
            anchor = value;
            UpdateDestinationRectangle();
        }
    }

    /// <summary>
    /// Sprite rendering width in pixels. This effectively scales the sprite.
    /// </summary>
    public int Width
    {
        get => width;
        set { width = value; UpdateDestinationRectangle(); }
    }

    /// <summary>
    /// Sprite rendering height in pixels. This effectively scales the sprite.
    /// </summary>
    public int Height
    {
        get => height;
        set { height = value; UpdateDestinationRectangle(); }
    }


    public virtual void Draw()
    {
        Raylib.DrawTexturePro(Texture, Frame, dst, origin, Angle, Tint);
        foreach (var child in Children)
        {
            if (child is IDrawable rc) rc.Draw();
        }
    }

    private void UpdateDestinationRectangle()
    {
        var ax = Helpers.Lerp(0, Width, anchor.X);
        var ay = Helpers.Lerp(0, Height, anchor.Y);
		origin = new(Helpers.Lerp(0, Width, pivot.X), Helpers.Lerp(0, Height, pivot.Y));
		dst = new Rectangle(Position.X + origin.X - ax, Position.Y + origin.Y - ay, Width, Height);
    }
}

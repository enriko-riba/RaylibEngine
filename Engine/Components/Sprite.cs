namespace RaylibEngine.Components;

using RaylibEngine.Core;
using Raylib_CsLo;
using System.Numerics;

/// <summary>
/// Basic 2D sprite, renders a texture or part of it and enables positioning, anchoring, scaling, pivoting, rotations and tinting.
/// </summary>
public class Sprite : Container, IDrawable
{
	private Rectangle dst;
	private Vector2 position = Vector2.Zero;
	private Vector2 pivot = Vector2.Zero;
	private Vector2 anchor = Vector2.Zero;
	private int width;
	private int height;
	private Vector2 origin;
	private float angle;
	private Rectangle aabb;
	private Rectangle frame;

	public Sprite(Texture texture) : base()
	{
		Visible = true;
		Texture = texture;
		frame = new(0, 0, texture.width, texture.height);
		aabb = new(0, 0, texture.width, texture.height);
		width = texture.width;
		height = texture.height;
		UpdateDestinationRectangle();
	}

	public Sprite(Texture texture, Vector2 position) : this(texture)
	{		
		Position = position;
	}

	public Sprite(Texture texture, Vector2 position, int width, int height) : this(texture, position)
	{
		this.width = width;
		this.height = height;
		UpdateDestinationRectangle();
	}

	/// <summary>
	/// For debug only. Returns the texture destination rectangle.
	/// </summary>
	public Rectangle Dst => dst;

	/// <summary>
	/// Axis aligned bounding box.
	/// </summary>
	public Rectangle Aabb
	{
		get => aabb;
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
		set
		{
			position = value;
			UpdateDestinationRectangle();
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
			angle = value;
			UpdateDestinationRectangle();
		}
	}

	/// <summary>
	/// Returns the read-only texture origin.
	/// </summary>
	public Vector2 Origin => origin;

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
			if(width==0) width = (int)frame.width;
			if (height == 0) height = (int)frame.height;
			UpdateDestinationRectangle();
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
		set
		{
			width = value;
			UpdateDestinationRectangle();
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
			UpdateDestinationRectangle();
		}
	}

	public bool Visible { get; set; }

	/// <summary>
	/// Renders the sprite texture.
	/// </summary>
	public virtual void Draw()
	{
		Raylib.DrawTexturePro(Texture, Frame, dst, origin, Angle, Tint);		
	}

	private void UpdateDestinationRectangle()
	{
		var ax = Helpers.Lerp(0, width, anchor.X);
		var ay = Helpers.Lerp(0, height, anchor.Y);
		var px = Helpers.Lerp(0, width, pivot.X);
		var py = Helpers.Lerp(0, height, pivot.Y);
		origin = new(px, py);
		dst = new Rectangle(Position.X + px - ax, Position.Y + py - ay, Width, Height);
		aabb = dst.CalcAabbForRotation(new(dst.X + px, dst.Y + py), Angle);
		aabb.x -= origin.X;
		aabb.y -= origin.Y;
	}

	public override string ToString() => $"{base.ToString()}, texture id: {Texture.id}, frame: ({Frame.x},{Frame.y},{Frame.width},{Frame.height})";
}

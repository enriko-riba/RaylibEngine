namespace RaylibEngine.Components;

using Raylib_CsLo;
using RaylibEngine.Core;

/// <summary>
/// Basic 2D sprite, renders a texture or part of it and enables positioning, anchoring, scaling, pivoting, rotations and tinting.
/// </summary>
public class TilingSprite : Sprite, IDrawable
{
	public TilingSprite(Texture texture) : base(texture) { }

	/// <summary>
	/// Renders the sprite texture.
	/// </summary>
	public override void Draw()
	{
		Raylib.DrawTextureTiled(Texture, Frame, Dst, Origin, Angle, 1, Tint);
	}
}

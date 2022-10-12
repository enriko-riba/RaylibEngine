using Raylib_CsLo;
using System.Numerics;

namespace RaylibEngine;

public static class Helpers
{
	public const double DEGREE_2_RADIAN = Math.PI / 180.0;

    public static float Lerp(float a, float b, float t) => (1f - t) * a + t * b;

	#region Body2D

	/// <summary>
	/// Checks if the AABB contains the given point.
	/// </summary>
	/// <param name="aabb"></param>
	/// <param name="point"></param>
	/// <returns></returns>
	public static bool ContainsPoint(this Rectangle aabb, Vector2 point) => point.X >= aabb.X &&
												point.X <= (aabb.X + aabb.width) &&
												point.Y >= aabb.Y &&
												point.Y <= (aabb.Y + aabb.height);
	/// <summary>
	/// Checks if two AABB rectangles intersect.
	/// </summary>
	/// <param name="aabb"></param>
	/// <param name="otherAabb"></param>
	/// <returns></returns>
	public static bool IntersectsAabb(this Rectangle aabb, Rectangle otherAabb)
	{
		var maxx1 = Math.Max(aabb.x, aabb.x + aabb.width);
		var maxy1 = Math.Max(aabb.y, aabb.y + aabb.height);
		var minx1 = Math.Min(aabb.x, aabb.x + aabb.width);
		var miny1 = Math.Min(aabb.y, aabb.y + aabb.height);

		var minx2 = Math.Min(otherAabb.x, otherAabb.x + otherAabb.width);
		var miny2 = Math.Min(otherAabb.y, otherAabb.y + otherAabb.height);
		var maxx2 = Math.Max(otherAabb.x, otherAabb.x + otherAabb.width);
		var maxy2 = Math.Max(otherAabb.y, otherAabb.y + otherAabb.height);

		return maxx1 > minx2 && minx1 < maxx2 && maxy1 > miny2 && miny1 < maxy2;
	}

	/// <summary>
	/// Calculates the AABB of a rotated rectangle.
	/// </summary>
	/// <param name="originalRectangle">the unrotated rectangle for which the AABB is calculated</param>
	/// <param name="rotation">rectangle rotation in degrees</param>
	/// <param name="pivot">pivot point for the rotation</param>
	/// <returns>The AABB rectangle.</returns>
	public static Rectangle CalcAabbForRotation(this Rectangle originalRectangle, float rotation, Vector2 pivot)
	{
		var (sin, cos) = Math.SinCos(rotation * Helpers.DEGREE_2_RADIAN);
		var p1 = new Vector2(originalRectangle.x, originalRectangle.y);
		var p2 = new Vector2(originalRectangle.x + originalRectangle.width, originalRectangle.y);
		var p3 = new Vector2(originalRectangle.x + originalRectangle.width, originalRectangle.y + originalRectangle.height);
		var p4 = new Vector2(originalRectangle.x, originalRectangle.y + originalRectangle.height);

		(float x, float y)[] rotated = new (float x, float y)[] {
			RotatePoint(pivot, p1, (float)sin, (float)cos),
			RotatePoint(pivot, p2, (float)sin, (float)cos),
			RotatePoint(pivot, p3, (float)sin, (float)cos),
			RotatePoint(pivot, p4, (float)sin, (float)cos)
		};
		float x1 = float.MaxValue;
		float y1 = float.MaxValue;
		float x2 = float.MinValue;
		float y2 = float.MinValue;
		foreach (var (x, y) in rotated)
		{
			x1 = Math.Min(x1, x);
			y1 = Math.Min(y1, y);
			x2 = Math.Max(x2, x);
			y2 = Math.Max(y2, y);
		}
		Rectangle rect = new(x1, y1, Math.Abs(x2 - x1), Math.Abs(y2 - y1));
		return rect;
	}

	/// <summary>
	/// Rotates a point around the given pivot. 
	/// As the rotation is usually applied to batches of points the rotation angle is 
	/// substituted with pre-calculated sine and cosine of the rotation angle.
	/// </summary>
	/// <param name="pivot"></param>
	/// <param name="point"></param>
	/// <param name="sin"></param>
	/// <param name="cos"></param>
	/// <returns></returns>
	public static (float x, float y) RotatePoint(Vector2 pivot, Vector2 point, float sin, float cos)
	{
		var x = (cos * (point.X - pivot.X)) - (sin * (point.Y - pivot.Y)) + pivot.X;
		var y = (sin * (point.X - pivot.X)) + (cos * (point.Y - pivot.Y)) + pivot.Y;
		return (x, y);
	}
	#endregion
}

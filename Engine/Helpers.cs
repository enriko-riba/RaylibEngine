using Raylib_CsLo;
using System.Numerics;

namespace RaylibEngine;

public static class Helpers
{
	public const double DEGREE_2_RADIAN = Math.PI / 180.0;
	public const double RADIAN_2_DEGREE = 180.0 / Math.PI;

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
	/// <param name="pivot">pivot point for the rotation</param>
	/// <param name="rotation">rectangle rotation in degrees</param>
	/// <returns>The AABB rectangle.</returns>
	public static Rectangle CalcAabbForRotation(this Rectangle originalRectangle, Vector2 pivot, float rotation)
	{
		float x1 = float.MaxValue;
		float y1 = float.MaxValue;
		float x2 = float.MinValue;
		float y2 = float.MinValue;

		var rotated = GetRotatedVertices(originalRectangle, pivot, rotation);
		foreach (var vertex in rotated)
		{
			x1 = Math.Min(x1, vertex.X);
			y1 = Math.Min(y1, vertex.Y);
			x2 = Math.Max(x2, vertex.X);
			y2 = Math.Max(y2, vertex.Y);
		}
		return new(x1, y1, Math.Abs(x2 - x1), Math.Abs(y2 - y1));
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
	public static Vector2 RotatePoint(Vector2 pivot, Vector2 point, float sin, float cos)
	{
		var x = (cos * (point.X - pivot.X)) - (sin * (point.Y - pivot.Y)) + pivot.X;
		var y = (sin * (point.X - pivot.X)) + (cos * (point.Y - pivot.Y)) + pivot.Y;
		return new (x, y);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="rectangle"></param>
	/// <returns></returns>
	public static ReadOnlySpan<Vector2> GetVertices(this Rectangle rectangle)
	{
		return new Vector2[]
		{
			new (rectangle.x, rectangle.y),
			new (rectangle.x + rectangle.width, rectangle.y),
			new (rectangle.x + rectangle.width, rectangle.y + rectangle.height),
			new (rectangle.x, rectangle.y + rectangle.height),
		};
	}

	/// <summary>
	/// Returns the rectangle vertices's rotated around the given pivot point.
	/// </summary>
	/// <param name="rectangle"></param>
	/// <param name="pivot">the center of rotation</param>
	/// <param name="rotation">angle in degrees</param>
	/// <returns></returns>
	public static ReadOnlySpan<Vector2> GetRotatedVertices(this Rectangle rectangle, Vector2 pivot, float rotation)
	{
		var (sin, cos) = Math.SinCos(rotation * DEGREE_2_RADIAN);
		return new Vector2[]
		{
			RotatePoint(pivot, new (rectangle.x, rectangle.y), (float)sin, (float)cos),
			RotatePoint(pivot, new (rectangle.x + rectangle.width, rectangle.y), (float)sin, (float)cos),
			RotatePoint(pivot, new (rectangle.x + rectangle.width, rectangle.y + rectangle.height), (float)sin, (float)cos),
			RotatePoint(pivot, new (rectangle.x, rectangle.y + rectangle.height), (float)sin, (float)cos)
		};
	}
	#endregion
}

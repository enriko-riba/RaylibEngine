using System.Numerics;

namespace Box2DTest.Physics2DUtils;

internal static class DouglasPeuckerReduction
{
	public static IReadOnlyList<Vector2> ReducePoints(IReadOnlyList<Vector2> existingPolygon, double tolerance)
	{
		if (existingPolygon.Count < 3) return existingPolygon;

		int firstPoint = 0;
		int lastPoint = existingPolygon.Count - 1;
		List<int> pointIndexsToKeep = new()
		{
			//Add the first and last index to the keepers
			firstPoint,
			lastPoint
		};

		//The first and the last point cannot be the same
		while (existingPolygon[firstPoint].Equals(existingPolygon[lastPoint]))
		{
			lastPoint--;
		}

		ReducePoints(existingPolygon, firstPoint, lastPoint, tolerance, ref pointIndexsToKeep);
		pointIndexsToKeep.Sort();
		return pointIndexsToKeep.Select(index => existingPolygon[index]).ToArray();
	}

	/// <summary>
	/// The Douglas-Peucker based reduction.
	/// </summary>
	/// <param name="points">The points.</param>
	/// <param name="firstPoint">The first point.</param>
	/// <param name="lastPoint">The last point.</param>
	/// <param name="tolerance">The tolerance.</param>
	/// <param name="pointIndexesToKeep">The point index to keep.</param>
	private static void ReducePoints(IReadOnlyList<Vector2> points, int firstPoint, int lastPoint, double tolerance, ref List<int> pointIndexesToKeep)
	{
		double maxDistance = 0;
		int indexFarthest = 0;

		for (int index = firstPoint; index < lastPoint; index++)
		{
			double distance = PerpendicularDistance(points[firstPoint], points[lastPoint], points[index]);
			if (distance > maxDistance)
			{
				maxDistance = distance;
				indexFarthest = index;
			}
		}

		if (maxDistance > tolerance && indexFarthest != 0)
		{
			//Add the largest point that exceeds the tolerance
			pointIndexesToKeep.Add(indexFarthest);
			ReducePoints(points, firstPoint, indexFarthest, tolerance, ref pointIndexesToKeep);
			ReducePoints(points, indexFarthest, lastPoint, tolerance, ref pointIndexesToKeep);
		}
	}

	/// <summary>
	/// The distance of a point from a line made from point1 and point2.
	/// </summary>
	/// <param name="pt1">The PT1.</param>
	/// <param name="pt2">The PT2.</param>
	/// <param name="p">The p.</param>
	/// <returns></returns>
	private static double PerpendicularDistance(Vector2 Point1, Vector2 Point2, Vector2 Point)
	{
		double area = Math.Abs(.5 * (Point1.X * Point2.Y + Point2.X * Point.Y + Point.X * Point1.Y - Point2.X * Point1.Y - Point.X * Point2.Y - Point1.X * Point.Y));
		double bottom = Math.Sqrt(Math.Pow(Point1.X - Point2.X, 2) + Math.Pow(Point1.Y - Point2.Y, 2));
		double height = area / bottom * 2;
		return height;
	}
}

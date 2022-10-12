using RaylibEngine.Components;
using RaylibEngine.Core;

namespace RaylibEngine;

public class Helpers
{
    public static float Lerp(float a, float b, float t) => (1f - t) * a + t * b;

	public static IBody2D CreateFromSprite(Sprite sprite)
	{
		return new Body2D() { 
			aabb = sprite.Frame
		};
	}

	public const double DEGREE_2_RADIAN = Math.PI / 180.0;
}

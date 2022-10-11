using Raylib_CsLo;
using System.Numerics;

namespace RaylibEngine.Core;

public interface IBody2D
{
	bool ContainsPoint(Vector2 point);
	Rectangle Aabb { get; }
}
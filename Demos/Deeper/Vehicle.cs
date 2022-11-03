using RaylibEngine.Components;
using RaylibEngine.Core;
using System.Numerics;

namespace Deeper;

internal class Vehicle : Sprite, IUpdateable
{
	const float Speed = 800;

	public Vehicle(Texture texture, Vector2 position, int width, int height) : base(texture, position, width, height)
	{
	}

	public void Update(float ellapsedSeconds)
	{
		//	update vehicle position
		var speed = Speed * ellapsedSeconds;
		Position += IsKeyDown(KeyboardKey.KEY_RIGHT) ? new(speed, 0f) :
					   IsKeyDown(KeyboardKey.KEY_LEFT) ? new(-speed, 0f) :
					   IsKeyDown(KeyboardKey.KEY_UP) ? new(0f, -speed) :
					   IsKeyDown(KeyboardKey.KEY_DOWN) ? new(0f, speed) :
					   Vector2.Zero;
	}
}

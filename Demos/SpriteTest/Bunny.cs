
using RaylibEngine.Components;
using RaylibEngine.Core;
using System.Numerics;

namespace SpriteTest;
internal class Bunny : Sprite, IUpdateable
{
    private const float GRAVITY = 9.9780327f;
    private readonly Vector2 gravityDirection = new(0, 1);
    private float velocity = 0f;

    public Bunny(Texture texture) : base(texture) { }

    public void Update(float ellapsedSeconds)
    {
        // v = v0 + at
        // d = d0 + v0t + at^2/2, in our case d0 = 0 so we have d = v0t + at^2/2
        var v0 = velocity;
        var at = ellapsedSeconds * GRAVITY * Height;
        velocity = v0 + at;
        var distance = v0 * ellapsedSeconds + (at * ellapsedSeconds / 2f);
        Position += (gravityDirection * distance);

        if (Position.Y > GetScreenHeight() + Height)
        {
            Position = new(Random.Shared.Next(0, GetScreenWidth()), -Height);
            velocity = 0f;
        }
    }
}

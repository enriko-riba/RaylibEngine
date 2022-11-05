using RaylibEngine.Components;
using RaylibEngine.Core;
using System.Numerics;

namespace BunnyTest;

internal class Bunny : Sprite, IUpdateable
{
    const float gravity = 10f;
    private float speedX;
    private float speedY;

    private readonly int right;
    private readonly int left;
    private readonly int bottom;
    private readonly int top;

    public Bunny(Texture texture, int sceneWidth, int sceneHeight) : base(texture)
    {
        Pivot = new(0.5f, 0.5f);

        speedX = Random.Shared.Next(0, 10);
        speedY = Random.Shared.Next(0, 5);

        left = 20;
        right = sceneWidth - 20;
        top = 60;
        bottom = sceneHeight - 20;

    }

    public void Update(float ellapsedSeconds)
    {
        speedY += gravity * ellapsedSeconds;

        Position += new Vector2(speedX, speedY);
        if (Position.X > right)
        {
            speedX *= -1;
            Position = new(right, Position.Y);
        }
        if (Position.X < left)
        {
            speedX *= -0.85f;
            Position = new(left, Position.Y);
        }
        if (Position.Y > bottom)
        {
            speedY *= -1;
            Position = new(Position.X, bottom);
        }
        if (Position.Y < top)
        {
            Position = new(Position.X, top);
            speedY = 0;
        }
    }
}

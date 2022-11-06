
using RaylibEngine.Components;
using RaylibEngine.Core;
using System.Numerics;

namespace RaySnake.Logic;
internal class Ground : Container, IDrawable
{
   
    private readonly int width;
    private readonly int height;
    private readonly Color color;

    public Ground(int x, int y, int width, int height, Color color)
    {
        Position = new(x, y, 0);
        this.width = width;
        this.height = height;
        this.color = color;
        Visible = true;
    }

    public bool Visible { get; set; }

    public float Angle => 0;
   
    public Vector3 Position { get; }

    public bool IsDirty => false;

    public void Draw()
    {
        DrawRectangle((int)Position.X, (int)Position.Y, width, height, color);
    }
}

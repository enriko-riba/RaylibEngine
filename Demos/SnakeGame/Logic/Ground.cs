
using RaylibEngine.Components;
using RaylibEngine.Core;

namespace RaySnake.Logic;
internal class Ground : Container, IDrawable
{
    private readonly int x;
    private readonly int y;
    private readonly int width;
    private readonly int height;
    private readonly Color color;

    public Ground(int x, int y, int width, int height, Color color)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.color = color;
        Visible = true;
    }

    public bool Visible { get; set; }

    public void Draw()
    {
        DrawRectangle(x, y, width, height, color);
    }
}

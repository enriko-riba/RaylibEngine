
namespace Box2DTest;

using Box2D.NetStandard.Common;
using Box2D.NetStandard.Dynamics.World.Callbacks;
using Box2DTest.Physics2DUtils;
using System.Numerics;

internal class DebugRenderer : DebugDraw
{
    private readonly Vector2 world2Viewscale;

    public DebugRenderer(Vector2 world2Viewscale)
    {
        this.world2Viewscale = world2Viewscale;
    }

    [Obsolete()]
    public override void DrawCircle(in Vec2 center, float radius, in Box2D.NetStandard.Dynamics.World.Color color)
    {
        DrawCircleV(center * world2Viewscale, radius, color.ToRaylibColor());
    }

    public override void DrawPoint(in Vector2 position, float size, in Box2D.NetStandard.Dynamics.World.Color color)
    {
        DrawPixelV(position * world2Viewscale, color.ToRaylibColor());
    }

    [Obsolete()]
    public override void DrawPolygon(in Vec2[] vertices, int vertexCount, in Box2D.NetStandard.Dynamics.World.Color color)
    {
        var clr = color.ToRaylibColor();
        for (var i = 0; i < vertexCount - 1; i++)
        {
            var v1 = vertices[i] * world2Viewscale;
            var v2 = vertices[i + 1] * world2Viewscale;
            DrawLineV(v1, v2, clr);
        }
        DrawLineV(vertices[vertexCount - 1] * world2Viewscale, vertices[0] * world2Viewscale, clr);
    }

    [Obsolete()]
    public override void DrawSegment(in Vec2 p1, in Vec2 p2, in Box2D.NetStandard.Dynamics.World.Color color)
    {
        ;
    }
    [Obsolete()]
    public override void DrawSolidCircle(in Vec2 center, float radius, in Vec2 axis, in Box2D.NetStandard.Dynamics.World.Color color)
    {
        ;
    }

    [Obsolete()]
    public override void DrawSolidPolygon(in Vec2[] vertices, int vertexCount, in Box2D.NetStandard.Dynamics.World.Color color)
    {
        const float Thickness = 3f;
        var clr = color.ToRaylibColor();
        for (var i = 0; i < vertexCount - 1; i++)
        {
            var v1 = vertices[i] * world2Viewscale;
            var v2 = vertices[i + 1] * world2Viewscale;
            DrawLineEx(v1, v2, Thickness, clr);
        }
        DrawLineEx(vertices[vertexCount - 1] * world2Viewscale, vertices[0] * world2Viewscale, Thickness, clr);
    }

    public override void DrawTransform(in Transform xf)
    {
        DrawCircleV(xf.p * world2Viewscale, 5, PURPLE);
    }
}


using Box2D.NetStandard.Collision.Shapes;
using Box2D.NetStandard.Dynamics.Bodies;
using Box2D.NetStandard.Dynamics.Fixtures;
using Box2D.NetStandard.Dynamics.World;
using Box2DTest.Physics2DUtils;
using RaylibEngine;
using RaylibEngine.Components;
using RaylibEngine.SceneManagement;
using System.Numerics;

namespace Box2DTest;
internal class ColliderScene : Scene
{
    private const int SpriteSize = 64;  //	dimensions of sprite frame inside the texture atlas
    private const int BunnySize = 32;   //	dimensions of rendered sprite

    /// <summary>
    /// Converts physics world to view.
    /// </summary>
    private readonly Vector2 world2ViewScale = new(10f, 10f);
    private readonly Vector2 view2WorldScale = new(0.1f, 0.1f);

    private readonly Texture atlas;
    private readonly Sprite rotor;
    private readonly List<Sprite> bunnies = new();
    private readonly World world;
    private readonly DebugRenderer debugRenderer;
    private bool isDebugRenderingOn = true;

    private const float TimeStep = 1f / 75f;    //	fixed physics simulation time step
    float accumulator = 0f;                     //	physics simulation time accumulator
    double polygonReductionTolerance = 1.25;

    //--------------------------
    // for debug rendering
    //--------------------------
    private byte[] byteMap;
    private byte[] bitMap;
    private IReadOnlyList<Vector2> edges;
    private IReadOnlyList<Vector2> smoothEdges;

    private int bunnyTypeIndex = -1;

    public ColliderScene(string name) : base(name)
    {
        WindowTitle = name;
        BackgroundColor = new Raylib_CsLo.Color(20, 30, 25, 255);
        atlas = LoadTexture("./Assets/spr.png");
        SetTextureFilter(atlas, TextureFilter.TEXTURE_FILTER_POINT);
        SetTextureWrap(atlas, TextureWrap.TEXTURE_WRAP_CLAMP);

        var w = GetScreenWidth();
        var h = GetScreenHeight();

        world = new World(new(0, 9.9780327f));
        debugRenderer = new DebugRenderer(world2ViewScale);
        debugRenderer.AppendFlags(Box2D.NetStandard.Dynamics.World.Callbacks.DrawFlags.Shape);
        debugRenderer.AppendFlags(Box2D.NetStandard.Dynamics.World.Callbacks.DrawFlags.CenterOfMass);
        world.SetDebugDraw(debugRenderer);


        //	create rotating sprite from atlas
        rotor = new Sprite(atlas)
        {
            Frame = new Rectangle(0, 0, SpriteSize, SpriteSize),
            Position = new(w / 2, h - 128),
            Pivot = new(0.5f, 0.5f),
            Anchor = new(0.5f, 0.5f),
            Width = 128,
            Height = 128,
        };
        AddChild(rotor);

        FixtureDef fd = new()
        {
            friction = 1f,
            restitution = 0.001f,
            density = 10000f,
            isSensor = false
        };
        world.CreateBody(rotor, BodyType.Kinematic, new[] { fd }, false, (float)Math.PI / 2, view2WorldScale, -1);

        //	set ground box
        world.CreateGroundBox(new(w / 2, h - 25), w / 2, 10, view2WorldScale, null);

        AddBunny();
    }

    private void AddBunny(bool nextBunny = true)
    {
        const int AtlasBunnyStartX = 66;
        var w = GetScreenWidth();

        if (nextBunny)
        {
            bunnyTypeIndex = ++bunnyTypeIndex % 4;
        }
        var x = AtlasBunnyStartX + (bunnyTypeIndex % 2) * BunnySize;
        var y = (bunnyTypeIndex / 2) * BunnySize;
        Sprite bunny = new(atlas)
        {
            Frame = new Rectangle(x, y, BunnySize, BunnySize),
            Position = new((w - BunnySize) / 2, BunnySize / 2),
            Pivot = new(0.5f, 0.5f),
            Anchor = new(0.5f, 0.5f),
        };
        AddChild(bunny);
        bunnies.Add(bunny);

        var angularVelocity = (float)Random.Shared.NextDouble() * 2f - 1f;

        CalculateColliderPolygon();
        FixtureDef fd = new()
        {
            friction = 0.45f,
            restitution = 0.05f,
            density = 20f,
            isSensor = false,
            shape = new PolygonShape(smoothEdges.Take(8).Select(se => se * view2WorldScale).ToArray())
        };
        world.CreateBody(bunny, BodyType.Dynamic, new[] { fd }, true, angularVelocity, view2WorldScale, bunnies.Count - 1);
    }

    public override void OnEndDraw()
    {
        RenderMenu();
        world.DrawDebugData();

        const int Scale = 8;
        RenderSpriteColliderImages(Scale);
        RenderSpriteColliderEdges(Scale);
    }

    public override void OnBeginUpdate(float ellapsedSeconds)
    {
        accumulator += Math.Min(ellapsedSeconds, 0.25f);
        while (accumulator >= TimeStep)
        {
            world.Step(TimeStep, 20, 100);
            accumulator -= TimeStep;
        }

        var body = world.GetBodyList();
        while (body != null)
        {
            var spr = body.UserData switch
            {
                -1 => rotor,
                >= 0 => bunnies[(int)body.UserData],
                _ => null
            };
            if (spr != null)
            {
                spr.Position = body.Position * world2ViewScale;
                spr.Angle = (float)(body.GetAngle() * Helpers.RADIAN_2_DEGREE);

                if (spr == rotor)
                {
                    //	update sprite position
                    var movementVector = IsKeyDown(KeyboardKey.KEY_RIGHT) ? new(1f, 0f) :
                                    IsKeyDown(KeyboardKey.KEY_LEFT) ? new(-1f, 0f) :
                                    IsKeyDown(KeyboardKey.KEY_UP) ? new(0f, -1f) :
                                    IsKeyDown(KeyboardKey.KEY_DOWN) ? new(0f, 1f) :
                                    Vector2.Zero;
                    movementVector *= 40;
                    body.SetLinearVelocity(movementVector);
                }
            }
            body = body.GetNext();
        }

        var key = GetKeyPressed();
        if (key == (int)KeyboardKey.KEY_SPACE)
        {
            isDebugRenderingOn = !isDebugRenderingOn;
            if (isDebugRenderingOn)
            {
                debugRenderer.AppendFlags(Box2D.NetStandard.Dynamics.World.Callbacks.DrawFlags.Shape);
                debugRenderer.AppendFlags(Box2D.NetStandard.Dynamics.World.Callbacks.DrawFlags.CenterOfMass);
            }
            else
            {
                debugRenderer.ClearFlags(Box2D.NetStandard.Dynamics.World.Callbacks.DrawFlags.Shape);
                debugRenderer.ClearFlags(Box2D.NetStandard.Dynamics.World.Callbacks.DrawFlags.CenterOfMass);
            }
        }
        else if (key == (int)KeyboardKey.KEY_MINUS || key == (int)KeyboardKey.KEY_KP_SUBTRACT)
        {
            polygonReductionTolerance -= 0.25;
            CalculateColliderPolygon();
        }
        else if (key == (int)KeyboardKey.KEY_KP_ADD)
        {
            polygonReductionTolerance += 0.25;
            CalculateColliderPolygon();
        }

        if (IsMouseButtonPressed(0))
        {
            AddBunny();
        }
        else if (IsMouseButtonPressed(1))
        {
            AddBunny(false);
        }
    }

    private void CalculateColliderPolygon()
    {
        polygonReductionTolerance = Math.Clamp(polygonReductionTolerance, 0, 5);

        var bunny = bunnies.Last();
        var image = LoadImage("./Assets/spr.png");
        Physics2DCollider p2dcollider = new(image, bunny.Frame);
        byteMap = p2dcollider.BppImage;
        bitMap = p2dcollider.MorphedImage;
        edges = p2dcollider.DetectEdges();
        UnloadImage(image);
        smoothEdges = DouglasPeuckerReduction.ReducePoints(edges, polygonReductionTolerance);
    }

    private void RenderMenu()
    {
        DrawFPS(5, 25);
        DrawText($"bunny type: {bunnyTypeIndex}, polygon reduction tolerance: {polygonReductionTolerance}", 105, 25, 20, LIME);
        DrawText("+ / - to change polygon reduction tolerance", 5, 50, 20, WHITE);
        DrawText("arrows to move kinematic rotor", 5, 75, 20, WHITE);
        DrawText("space to toggle debug rendering", 5, 100, 20, WHITE);
    }

    private void RenderSpriteColliderImages(int scale)
    {
        var bunny = bunnies.Last();
        var spriteSize = BunnySize + 2; //	1 pixel padding
        DrawTexturePro(bunny.Texture, bunny.Frame, new(50, 200, spriteSize * scale, spriteSize * scale), Vector2.Zero, 0, WHITE);

        for (var x = 0; x < spriteSize; x++)
        {
            for (var y = 0; y < spriteSize; y++)
            {
                DrawRectangle(350 + x * scale, 200 + y * scale, scale, scale, byteMap[x + y * spriteSize] == 1 ? GREEN : DARKGRAY);
                DrawRectangle(400 + spriteSize * scale + x * scale, 200 + y * scale, scale, scale, bitMap[x + y * spriteSize] == 1 ? WHITE : DARKGRAY);
            }
        }
    }

    private void RenderSpriteColliderEdges(int scale)
    {
        const float LineThickness = 5.5f;
        const float VertexRadius = 2.5f;
        var VertexColor = YELLOW;

        Vector2 offset = new(370 + (BunnySize * scale / 2), 250 + BunnySize * scale + (BunnySize * scale / 2));
        Vector2 scaleVector = new(scale, scale);
        Vector2 v1, v2;

        for (var i = 0; i < edges.Count - 1; i++)
        {
            v1 = edges[i] * scaleVector + offset;
            v2 = edges[i + 1] * scaleVector + offset;
            var clrLerp = (byte)Helpers.Lerp(30, 255, i / (float)edges.Count);
            var clr = i == 0 ? RED : new Raylib_CsLo.Color(clrLerp, clrLerp, clrLerp, (byte)255);
            DrawLineEx(v1, v2, LineThickness, clr);
            DrawCircleV(v1, VertexRadius, VertexColor);
        }
        v1 = edges[edges.Count - 1] * scaleVector + offset;
        v2 = edges[0] * scaleVector + offset;
        DrawLineEx(v1, v2, LineThickness, DARKGREEN);
        DrawCircleV(v1, VertexRadius, VertexColor);
        DrawText("Vertices: " + edges.Count.ToString(), 350, offset.Y + BunnySize * scale * 0.5f, 15, WHITE);

        // smoothed vertices
        offset = new Vector2(420 + BunnySize * scale + (BunnySize * scale / 2), 250 + BunnySize * scale + (BunnySize * scale / 2));
        for (var i = 0; i < smoothEdges.Count - 1; i++)
        {
            v1 = smoothEdges[i] * scaleVector + offset;
            v2 = smoothEdges[i + 1] * scaleVector + offset;
            var clrLerp = (byte)Helpers.Lerp(30, 255, i / (float)smoothEdges.Count);
            var clr = i == 0 ? RED : new Raylib_CsLo.Color(clrLerp, clrLerp, clrLerp, (byte)255);
            DrawLineEx(v1, v2, LineThickness, clr);
            DrawCircleV(v1, VertexRadius, VertexColor);
        }
        v1 = smoothEdges[smoothEdges.Count - 1] * scaleVector + offset;
        v2 = smoothEdges[0] * scaleVector + offset;
        DrawLineEx(v1, v2, LineThickness, DARKGREEN);
        DrawCircleV(v1, VertexRadius, VertexColor);
        DrawText("Vertices: " + smoothEdges.Count.ToString(), 400 + BunnySize * scale, offset.Y + BunnySize * scale * 0.5f, 20, WHITE);
    }
}

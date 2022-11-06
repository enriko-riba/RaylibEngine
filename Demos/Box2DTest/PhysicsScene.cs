
using Box2D.NetStandard.Dynamics.Bodies;
using Box2D.NetStandard.Dynamics.Fixtures;
using Box2D.NetStandard.Dynamics.World;
using Box2DTest.Physics2DUtils;
using RaylibEngine;
using RaylibEngine.Components;
using RaylibEngine.SceneManagement;
using System.Numerics;

namespace Box2DTest;
internal class PhysicsScene : Scene
{
    private const int SpriteSize = 64;  //	dimensions of sprite frame inside the texture atlas
    private const int BunnySize = 32;   //	dimensions of rendered sprite

    private const int BunnyCount = 50;
    private int totalBunnies = 0;

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

    public PhysicsScene(string name) : base(name)
    {
        WindowTitle = name;
        BackgroundColor = DARKBROWN;
        atlas = LoadTexture("./Assets/spr.png");
        SetTextureFilter(atlas, TextureFilter.TEXTURE_FILTER_BILINEAR);
        SetTextureWrap(atlas, TextureWrap.TEXTURE_WRAP_CLAMP);

        var w = GetScreenWidth();
        var h = GetScreenHeight();

        world = new World(new(0, 9.9780327f));
        debugRenderer = new DebugRenderer(world2ViewScale);
        debugRenderer.AppendFlags(Box2D.NetStandard.Dynamics.World.Callbacks.DrawFlags.Shape);
        debugRenderer.AppendFlags(Box2D.NetStandard.Dynamics.World.Callbacks.DrawFlags.CenterOfMass);
        world.SetDebugDraw(debugRenderer);

        // create random bunnies from atlas texture
        AddBunnies();

        //	create rotating sprite from atlas
        rotor = new Sprite(atlas)
        {
            Frame = new Rectangle(0, 0, SpriteSize, SpriteSize),
            Position = new(w / 2, h / 2),
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
        world.CreateBody(rotor, BodyType.Kinematic, new[] { fd }, false, (float)Math.PI, view2WorldScale, -1);

        //	set ground box
        world.CreateGroundBox(new(w / 2, h - 25), w / 2, 10, view2WorldScale, null);
    }

    private void AddBunnies()
    {
        var w = GetScreenWidth();
        FixtureDef fd = new()
        {
            friction = 0.45f,
            restitution = 0.05f,
            density = 20f,
            isSensor = false,
        };

        for (var i = 0; i < BunnyCount; i++)
        {
            var x = SpriteSize + Random.Shared.Next(0, 2) * BunnySize;
            var y = Random.Shared.Next(0, 2) * BunnySize;
            Vector2 position = new(Random.Shared.Next(0, w - BunnySize), Random.Shared.Next(0, BunnySize * 2));
            Sprite bunny = new(atlas, position, BunnySize, BunnySize)
            {
                Frame = new Rectangle(x, y, BunnySize, BunnySize),
                Pivot = new(0.5f, 0.5f),
                Anchor = new(0.5f, 0.5f),
            };
            AddChild(bunny);
            bunnies.Add(bunny);
            var angularVelocity = (float)Random.Shared.NextDouble() * 2f - 1f;
            world.CreateBody(bunny, BodyType.Dynamic, new[] { fd }, false, angularVelocity, view2WorldScale, bunnies.Count - 1);
            totalBunnies++;
        }
    }

    public override void OnBeginDraw()
    {
        RenderAxes();
    }

    public override void OnEndDraw()
    {
        RenderMenu();
        world.DrawDebugData();
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
                var pos = body.Position * world2ViewScale;
                spr.Position = new Vector2(pos.X, pos.Y);
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

        if (GetKeyPressed() == (int)KeyboardKey.KEY_SPACE)
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

        if (IsMouseButtonPressed(0))
        {
            AddBunnies();
        }
    }

    private static void RenderAxes()
    {
        var w = GetScreenWidth();
        var h = GetScreenHeight();
        DrawLine(w / 2, 10, w / 2, h - 10, YELLOW);
        DrawLine(10, h / 2, w - 10, h / 2, BLUE);
    }

    private void RenderMenu()
    {
        DrawFPS(5, 10);
        DrawText($"bunny objects: {totalBunnies}", 130, 10, 20, LIME);
        DrawText("arrows to move kinematic rotor", 5, 50, 20, LIME);
        DrawText("space to toggle debug rendering", 5, 90, 20, LIME);
    }

}

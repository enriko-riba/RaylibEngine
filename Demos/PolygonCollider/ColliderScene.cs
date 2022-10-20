namespace Box2DTest;

using Box2D.NetStandard.Collision.Shapes;
using Box2D.NetStandard.Dynamics.Bodies;
using Box2D.NetStandard.Dynamics.Fixtures;
using Box2D.NetStandard.Dynamics.World;
using Box2DTest.Physics2DUtils;
using RaylibEngine;
using RaylibEngine.Components;
using RaylibEngine.SceneManagement;
using System.Numerics;

internal class ColliderScene : Scene
{
	private const int SpriteSize = 64;  //	dimensions of sprite frame inside the texture atlas
	private const int BunnySize = 32;   //	dimensions of sprite frame inside the texture atlas
	
	private const int BunnyCount = 50;

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

	private const float TimeStep = 1f / 75f;	//	fixed physics simulation time step
	float accumulator = 0f; //	physics simulation time accumulator

	
	//--------------------------
	// for debug rendering
	//--------------------------
	private byte[] byteMap;
	private byte[] bitMap;
	private IReadOnlyList<Vector2> edges;
	private IReadOnlyList<Vector2> smoothEdges;


	public ColliderScene(string name) : base(name)
	{
		WindowTitle = name;
		BackgroundColor = DARKBROWN;
		atlas = LoadTexture("./Assets/spr.png");
		SetTextureFilter(atlas, TextureFilter.TEXTURE_FILTER_TRILINEAR);

		var w = GetScreenWidth();
		var h = GetScreenHeight();

		world = new World(new(0, 9.9780327f));
		debugRenderer = new DebugRenderer(world2ViewScale);
		debugRenderer.AppendFlags(Box2D.NetStandard.Dynamics.World.Callbacks.DrawFlags.Shape);
		debugRenderer.AppendFlags(Box2D.NetStandard.Dynamics.World.Callbacks.DrawFlags.CenterOfMass);
		world.SetDebugDraw(debugRenderer);

		// create random bunnies from atlas texture
		AddBunny();

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
		world.CreateBody(rotor, BodyType.Kinematic, fd, false, (float)Math.PI, view2WorldScale, - 1);

		//	set ground box
		world.CreateGroundBox(new(w / 2, h - 25), w / 2, 10, view2WorldScale, null);
	}

	private void AddBunny()
	{
		var w = GetScreenWidth();
		FixtureDef fd = new()
		{
			friction = 0.45f,
			restitution = 0.05f,
			density = 20f,
			isSensor = false,
		};
		
		var x = SpriteSize + Random.Shared.Next(0, 2) * BunnySize;
		var y = Random.Shared.Next(0, 2) * BunnySize;
		var bunny = new Sprite(atlas)
		{
			Frame = new Rectangle(x, y, BunnySize, BunnySize),
			Position = new((w - BunnySize)/2, Random.Shared.Next(0, BunnySize * 2)),
			Pivot = new(0.5f, 0.5f),
			Anchor = new(0.5f, 0.5f),
		};
		AddChild(bunny);
		bunnies.Add(bunny);

		var angularVelocity = (float)Random.Shared.NextDouble() * 2f - 1f;
		
		var image = LoadImage("./Assets/spr.png");
		var p2dcollider = new Physics2DCollider(image, bunny.Frame);				
		byteMap = p2dcollider.BppImage;
		bitMap = p2dcollider.MorphedImage;
		edges = p2dcollider.DetectEdges();
		UnloadImage(image);

		smoothEdges = DouglasPeuckerReduction.ReducePoints(edges, 1.0);
		fd.shape = new PolygonShape(smoothEdges.Take(4).Select(se => se * view2WorldScale).ToArray());
		world.CreateBody(bunny, BodyType.Dynamic, fd, true, angularVelocity, view2WorldScale, bunnies.Count-1);
	}
	
	public override void OnBeginDraw()
	{
		RenderAxes();
	}

	public override void OnEndDraw()
	{
		RenderMenu();
		world.DrawDebugData();

		const int Scale = 8;
		RenderSpriteColliderImages(Scale);
		RenderSpriteColliderEdges(Scale);
	}
	
	public override void OnUpdate(float ellapsedSeconds)
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
			AddBunny();
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
		DrawText("arrows to move kinematic rotor", 5, 50, 20, LIME);
		DrawText("space to toggle debug rendering", 5, 90, 20, LIME);
	}

	private void RenderSpriteColliderImages(int scale)
	{
		for (int x = 0; x < BunnySize; x++)
		{
			for (int y = 0; y < BunnySize; y++)
			{
				DrawRectangle(100 + x * scale, 200 + y * scale, scale, scale, byteMap[x + y * BunnySize] == 1 ? RED : BLACK);
				DrawRectangle(150 + BunnySize * scale + x * scale, 200 + y * scale, scale, scale, bitMap[x + y * BunnySize] == 1 ? WHITE : BLACK);
			}
		}
	}

	private void RenderSpriteColliderEdges(int scale)
	{
		var offset = new Vector2(100 + (BunnySize*scale/2), 250 + BunnySize * scale + (BunnySize * scale / 2)) ;
		var scaleVector = new Vector2(scale, scale);
		Vector2 v1, v2;

		for (int i = 0; i < edges.Count - 1; i++)
		{
			v1 = edges[i] * scaleVector + offset;
			v2 = edges[i + 1] * scaleVector + offset;
			var clrLerp = (byte)Helpers.Lerp(0, 255, i / (float)edges.Count);
			var clr = i == 0 ? RED : new Raylib_CsLo.Color(clrLerp, clrLerp, clrLerp, (byte)255);
			DrawLineV(v1, v2, clr);
			DrawCircleV(v1, 2, BLUE);
		}
		v1 = edges[edges.Count - 1] * scaleVector + offset;
		v2 = edges[0] * scaleVector + offset;
		DrawLineV(v1, v2, DARKGREEN);

		// smoothed vertices
		offset = new Vector2(150 + BunnySize * scale + (BunnySize * scale / 2), 250 + BunnySize * scale + (BunnySize * scale / 2));
		for (int i = 0; i < smoothEdges.Count - 1; i++)
		{
			v1 = smoothEdges[i] * scaleVector + offset;
			v2 = smoothEdges[i + 1] * scaleVector + offset;
			var clrLerp = (byte)Helpers.Lerp(0, 255, i / (float)smoothEdges.Count);
			var clr = i == 0 ? RED : new Raylib_CsLo.Color(clrLerp, clrLerp, clrLerp, (byte)255);
			DrawLineV(v1, v2, clr);
			DrawCircleV(v1, 2, BLUE);
		}
		v1 = smoothEdges[smoothEdges.Count - 1] * scaleVector + offset;
		v2 = smoothEdges[0] * scaleVector + offset;
		DrawLineV(v1, v2, DARKGREEN);
	}
}

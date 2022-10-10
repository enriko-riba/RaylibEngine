using RaylibEngine.Core;
using RaylibEngine.SceneManagement;
using System.Numerics;
using System.Xml.Linq;

namespace SpriteTest;

internal class SpriteScene : Scene
{
	const int MOUSE_BUTTON_LEFT = 0;
	const int MOUSE_BUTTON_RIGHT = 1;

	private readonly Vector2[] Anchors = new Vector2[] {
		new(0, 0),
		new(0.5f, 1),
		new(1, 1),
		new(0.5f, 0.5f),
	};
	private int anchorId = 3;
	private readonly string[] AnchorNames = new string[] { "Top left", "Bottom mid", "Bottom right", "Center" };

	private readonly Texture texture;
	private Camera2D camera;

	public SpriteScene(string name) : base(name)
	{
		WindowTitle = name;
		BackgroundColor = DARKBROWN;
		texture = LoadTexture("./Assets/spr.png");
		var w = GetScreenWidth();
		var h = GetScreenHeight();

		var sprite = new Sprite(texture)
		{
			Position = new(w / 2, h / 2),
			Pivot = new(0.5f, 0.5f),
			Anchor = Anchors[anchorId],
			Width = 128,
			Height = 128,
		};
		AddChild(sprite);

		camera = new()
		{
			target = sprite.Position,
			//offset = new(w / 2f, h / 2f),
			rotation = 0.0f,
			zoom = 1.0f
		};
	}

	public override void OnBeginDraw()
	{
		var sprite = Children.First() as Sprite;
		sprite!.Angle = (sprite!.Angle + 0.1f) % 360;

		DrawFPS(5, 10);
		DrawText($"Pivot: {sprite.Pivot}", 5, 30, 20, LIME);
		DrawText($"Anchor: {AnchorNames[anchorId]}", 5, 50, 20, LIME);
		DrawText($"Angle: {sprite.Angle:N2}", 5, 70, 20, LIME);
		var w = GetScreenWidth();
		var h = GetScreenHeight();
		
		BeginMode2D(camera);
		camera.target = sprite.Position;
		camera.offset = new(w / 2f, h / 2f);
		DrawLine(w / 2, 10, w / 2, h-10, YELLOW);
		DrawLine(10, h / 2, w-10, h / 2, BLUE);
	}
	
	public override void OnEndDraw()
	{
		EndMode2D();
	}

	public override void OnUpdate(float ellapsedSeconds)
	{
		var sprite = Children.First() as Sprite;
		if (IsMouseButtonPressed(MOUSE_BUTTON_LEFT))
		{
			sprite!.Pivot = sprite.Pivot == Vector2.Zero ? new(0.5f, 0.5f) : Vector2.Zero;
		}
		if (IsMouseButtonPressed(MOUSE_BUTTON_RIGHT))
		{
			anchorId = ++anchorId % Anchors.Length;
			sprite!.Anchor = Anchors[anchorId];
		}

		var direction = IsKeyDown(KeyboardKey.KEY_RIGHT) ? Direction.East :
						IsKeyDown(KeyboardKey.KEY_LEFT) ? Direction.West :
						IsKeyDown(KeyboardKey.KEY_UP) ? Direction.North :
						IsKeyDown(KeyboardKey.KEY_DOWN) ? Direction.South :
						Direction.None;

		if(direction != Direction.None)
		{
			var dx = direction == Direction.East ? 1 : 
					 direction == Direction.West ? -1 : 0;
			var dy = direction == Direction.South? 1 :
					 direction == Direction.North ? -1 : 0;
			sprite!.Position += new Vector2(dx, dy);
		}
	}
}

public enum Direction
{
	North,
	East,
	South,
	West,
	None,
}

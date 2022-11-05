namespace Deeper.Entities;

using RaylibEngine.Components;
using RaylibEngine.Core;
using System.Numerics;

internal class Vehicle : Sprite, IUpdateable
{
	const float Speed = 400;

	private readonly Dictionary<Direction, Rectangle> frames = new();
	private readonly Map map;

	private Direction direction;
	private TilePosition currentTilePosition = new(0, 0);
	private TilePosition nextTilePosition = new(0, 0);
	private Vector2 nextPosition;

	public Vehicle(Texture texture, TilePosition tilePosition, Map map)
		: base(texture, new(tilePosition.X * Map.TileSize, tilePosition.Y * Map.TileSize), Map.TileSize, Map.TileSize)
	{
		currentTilePosition = tilePosition;
		this.map = map;
		frames[Direction.West] = new(416, 160, Map.TileSize, Map.TileSize);
		frames[Direction.East] = new(464, 160, Map.TileSize, Map.TileSize);
		frames[Direction.North] = new(416, 208, Map.TileSize, Map.TileSize);
		frames[Direction.South] = new(464, 208, Map.TileSize, Map.TileSize);
		Frame = frames[Direction.East];
	}

	public void Update(float ellapsedSeconds)
	{
		if (direction == Direction.None)
		{
			direction = GetDirectionInput();

			//	if changing from no movement to movement update next position and sprite frame
			if (direction != Direction.None)
			{
				nextTilePosition = GetNextTilePosition(currentTilePosition, direction);				
				if (!map.IsTileWalkable(nextTilePosition))
				{
					direction = Direction.None;
				}
				else
				{
					Frame = frames[direction];
					nextPosition = new Vector2(nextTilePosition.X, nextTilePosition.Y) * Map.TileSize;
				}
			}
		}

		if (direction != Direction.None)
		{
			//	update vehicle position
			var distance = Vector2.Distance(Position, nextPosition);
			if (distance > 2)
			{
				var movement = Math.Min(Speed * ellapsedSeconds, distance);
				var p = Position;
				p += direction switch
				{
					Direction.East => new(movement, 0f),
					Direction.West => new(-movement, 0f),
					Direction.North => new(0f, -movement),
					Direction.South => new(0f, movement),
					_ => Vector2.Zero
				};
				p.X = (float)Math.Round(p.X);
				p.Y = (float)Math.Round(p.Y);
				Position = p;
			}
			else
			{
				direction = Direction.None;
				currentTilePosition.X = (int)(nextPosition.X / Map.TileSize);
				currentTilePosition.Y = (int)(nextPosition.Y / Map.TileSize);
				Position = nextPosition;
			}
		}
	}

	private static Direction GetDirectionInput()
	{
		return IsKeyDown(KeyboardKey.KEY_RIGHT) ? Direction.East :
				IsKeyDown(KeyboardKey.KEY_LEFT) ? Direction.West :
				IsKeyDown(KeyboardKey.KEY_UP) ? Direction.North :
				IsKeyDown(KeyboardKey.KEY_DOWN) ? Direction.South : Direction.None;
	}

	private static TilePosition GetNextTilePosition(TilePosition position, Direction direction)
	{
		var x = position.X + direction switch
		{
			Direction.West => -1,
			Direction.East => 1,
			_ => 0
		};
		var y = position.Y + direction switch
		{
			Direction.North => -1,
			Direction.South => 1,
			_ => 0
		};
		return new TilePosition(x, y);
	}

	public enum Direction
	{
		None,
		North,
		East,
		South,
		West
	}
}
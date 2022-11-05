namespace Deeper.Entities;

using Deeper;
using RaylibEngine.Components;
using RaylibEngine.Core;
using System.Numerics;

internal class Vehicle : Sprite, IUpdateable
{
	const float Speed = 400;

	private readonly Dictionary<VehicleDirection, Rectangle> frames = new();
	private readonly int tileSize;
	private readonly Tile[] map;

	private VehicleDirection direction;
	private (int x, int y) currentTilePosition;
	private (int x, int y) nextTilePosition;
	private Vector2 nextPosition;

	public Vehicle(Texture texture, (int x, int y) tilePosition, int tileSize, Tile[] map)
		: base(texture, new(tilePosition.x * tileSize, tilePosition.y * tileSize), tileSize, tileSize)
	{
		currentTilePosition = tilePosition;
		this.tileSize = tileSize;
		this.map = map;
		frames[VehicleDirection.West] = new(416, 160, tileSize, tileSize);
		frames[VehicleDirection.East] = new(464, 160, tileSize, tileSize);
		frames[VehicleDirection.North] = new(416, 208, tileSize, tileSize);
		frames[VehicleDirection.South] = new(464, 208, tileSize, tileSize);
		Frame = frames[VehicleDirection.East];
	}

	public void Update(float ellapsedSeconds)
	{
		if (direction == VehicleDirection.None)
		{
			direction = IsKeyDown(KeyboardKey.KEY_RIGHT) ? VehicleDirection.East :
						IsKeyDown(KeyboardKey.KEY_LEFT) ? VehicleDirection.West :
						IsKeyDown(KeyboardKey.KEY_UP) ? VehicleDirection.North :
						IsKeyDown(KeyboardKey.KEY_DOWN) ? VehicleDirection.South : VehicleDirection.None;

			//	if changing from no movement to movement update next position and sprite frame
			if (direction != VehicleDirection.None)
			{
				nextTilePosition.x = currentTilePosition.x + direction switch
				{
					VehicleDirection.West => -1,
					VehicleDirection.East => 1,
					_ => 0
				};
				nextTilePosition.y = currentTilePosition.y + direction switch
				{
					VehicleDirection.North => -1,
					VehicleDirection.South => 1,
					_ => 0
				};
				if (!(Parent as DeeperScene)?.IsTileWalkable(nextTilePosition) ?? false)
				{
					direction = VehicleDirection.None;
				}
				else
				{
					Frame = frames[direction];
					nextPosition = new Vector2(nextTilePosition.x, nextTilePosition.y) * tileSize;
				}
			}
		}

		if (direction != VehicleDirection.None)
		{
			//	update vehicle position
			var distance = Vector2.Distance(Position, nextPosition);
			if (distance > 1)
			{
				var movement = Math.Min(Speed * ellapsedSeconds, distance);
				var p = Position;
				p += direction switch
				{
					VehicleDirection.East => new(movement, 0f),
					VehicleDirection.West => new(-movement, 0f),
					VehicleDirection.North => new(0f, -movement),
					VehicleDirection.South => new(0f, movement),
					_ => Vector2.Zero
				};
				p.X = (float)Math.Round(p.X);
				p.Y = (float)Math.Round(p.Y);
				Position = p;
			}
			else
			{
				direction = VehicleDirection.None;
				currentTilePosition.x = (int)(nextPosition.X / tileSize);
				currentTilePosition.y = (int)(nextPosition.Y / tileSize);
				Position = nextPosition;
			}
		}
	}
}

public enum VehicleDirection
{
	None,
	North,
	East,
	South,
	West
}

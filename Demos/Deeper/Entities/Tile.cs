namespace Deeper.Entities;

using RaylibEngine.Components;

internal record Tile(int X, int Y, TileType TileType, Sprite Sprite);

internal enum TileType
{
	Empty,
	Blocker,
	Ground,
	Dirt,
}

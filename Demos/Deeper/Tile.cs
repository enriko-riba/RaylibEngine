using RaylibEngine.Components;

namespace Deeper;

internal record Tile(int X, int Y, TileType TileType, Sprite Sprite);

internal enum TileType
{
	Empty,
	Blocker,
	Ground,
	Dirt,
}

namespace Box2DTest.Physics2DUtils;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

public class Physics2DCollider
{
	private readonly int maxIterations;
	private readonly int w;
	private readonly int h;
	private int firstNonEmptyIndex;

	public Physics2DCollider(Image image, Rectangle frame)
	{
		w = (int)frame.width;
		h = (int)frame.height;
		maxIterations = w * h * 2;
		Create1BppGrid(image, frame);
		CreateBitmaskGrid();
	}

	public byte[] BppImage { get; private set; }
	public byte[] MorphedImage { get; private set; }

	public IReadOnlyList<Vector2> DetectEdges()
	{
		var iteration = 0;
		var edges = new List<Vector2>();
		var id = GetDirection(firstNonEmptyIndex);
		var centerCorrection = new Vector2(w / 2, h / 2);
		while (iteration++ < maxIterations)
		{
			id = GetDirection(id);
			var vect = new Vector2(id % w - 1, id / w - 1);
			edges.Add(vect - centerCorrection);

			if (id == firstNonEmptyIndex) break;//done
		}
		edges.Reverse();
		return edges;
	}

	[MemberNotNull(nameof(BppImage))]
	private unsafe void Create1BppGrid(Image image, Rectangle frame)
	{
		Color* colors = LoadImageColors(image);
		var result = new byte[w * h];
		for (int x = 0; x < w; x++)
		{
			for (int y = 0; y < h; y++)
			{
				int colorIdx = (y + (int)frame.y) * image.width + x + (int)frame.X;
				Color pixel = colors[colorIdx];
				var isEmpty = pixel.a < 40 || pixel.r + pixel.g + pixel.b < 50;
				result[x + y * w] = (byte)(isEmpty ? 0 : 1);
			}
		}
		UnloadImageColors(colors);
		BppImage = result;
	}

	[MemberNotNull(nameof(MorphedImage))]
	private void CreateBitmaskGrid()
	{
		firstNonEmptyIndex = -1;
		byte[] bitmaskGrid = new byte[w * h];
		for (int i = 0; i < bitmaskGrid.Length; i++)
		{
			var sum = SumNeighbourValues(i);
			bitmaskGrid[i] = (byte)(sum > 4 ? 1:0);
			//bitmaskGrid[i] = (byte)(sum > 5 ? 1 :
			//						sum < 4 ? 0 :
			//						BppImage[i]);
			if (firstNonEmptyIndex < 0 && bitmaskGrid[i] == 1)
				firstNonEmptyIndex = i - w - 1;
		}
		MorphedImage = bitmaskGrid;
	}


	private int SumNeighbourValues(int id)
	{
		var sum = 0;
		var neighbours = NeighborIds(id);
		for (int i = 0; i < neighbours.Length; i++)
		{
			if (neighbours[i] >= 0 && neighbours[i] < BppImage.Length) sum += BppImage[neighbours[i]];
		}
		return sum;
	}

	private int[] NeighborIds(int id)
	{
		var to = id - w;//top
		var tr = to + 1;//top right
		var tl = to - 1;//top left

		var ri = id + 1;//right
		var le = id - 1;//left

		var bo = id + w;//bottom
		var br = bo + 1;//bottom right
		var bl = bo - 1;//bottom left

		return new int[] { tl, to, tr, le, id, ri, bl, bo, br };
	}

	private int GetDirection(int id)
	{
		var ri = id + 1;//	right pixel
		var bo = id + w;//	bottom pixel
		var br = bo + 1;//	bottom right pixel

		var x = 0;
		var y = 0;
		var key = 0;
		if (id >= 0 && id < MorphedImage.Length) key |= MorphedImage[id] << 3;
		if (ri >= 0 && ri < MorphedImage.Length) key |= MorphedImage[ri] << 2;
		if (bo >= 0 && bo < MorphedImage.Length) key |= MorphedImage[bo] << 1;
		if (br >= 0 && br < MorphedImage.Length) key |= MorphedImage[br];


		if (key == 1 || key == 3 || key == 11) x = 1;   // right
		if (key == 8 || key == 12 || key == 13) x = -1; // left
		if (key == 4 || key == 5 || key == 7) y = -1;   // up
		if (key == 2 || key == 10 || key == 14) y = 1;  // down
		return id + x + y * w;
	}
}
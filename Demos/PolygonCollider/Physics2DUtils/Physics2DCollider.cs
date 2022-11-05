using System.Numerics;

namespace Box2DTest.Physics2DUtils;
public class Physics2DCollider
{
    private readonly int maxIterations;
    private readonly int w;
    private readonly int h;
    private readonly int firstNonEmptyIndex;

    public Physics2DCollider(Image image, Rectangle frame)
    {
        w = (int)frame.width;
        h = (int)frame.height;
        maxIterations = w * h * 2;
        BppImage = Create1BppGrid(image, frame);
        MorphedImage = CreateBitmaskGrid(BppImage, out firstNonEmptyIndex);
        MorphedImage = BppImage;
    }

    public byte[] BppImage { get; private set; }
    public byte[] MorphedImage { get; private set; }

    public IReadOnlyList<Vector2> DetectEdges()
    {
        var iteration = 0;
        List<Vector2> edges = new();
        var id = GetDirection(firstNonEmptyIndex);
        Vector2 centerCorrection = new(w / 2, h / 2);
        while (iteration++ < maxIterations)
        {
            id = GetDirection(id);
            Vector2 vect = new(id % w - 1, id / w - 1);
            edges.Add(vect - centerCorrection);

            if (id == firstNonEmptyIndex) break;//done
        }
        edges.Reverse();
        return edges;
    }

    private unsafe byte[] Create1BppGrid(Image image, Rectangle frame)
    {
        var w = (int)frame.width;
        var h = (int)frame.height;
        var result = new byte[(w + 2) * (h + 2)];   //	1 pixel padding on every side

        var colors = LoadImageColors(image);
        for (var x = 0; x < w; x++)
        {
            for (var y = 0; y < h; y++)
            {
                var srcIdx = (y + (int)frame.y) * image.width + x + (int)frame.X;
                var pixel = colors[srcIdx];
                var isEmpty = pixel.a < 40 || pixel.r + pixel.g + pixel.b < 50;
                var dstIdx = x + 1 + (y + 1) * (w + 2);
                result[dstIdx] = (byte)(isEmpty ? 0 : 1);
            }
        }
        UnloadImageColors(colors);
        return result;
    }

    private byte[] CreateBitmaskGrid(byte[] srcGrid, out int firstNonEmptyIndex)
    {
        var dstGrid = new byte[srcGrid.Length];
        firstNonEmptyIndex = -1;
        for (var i = 0; i < srcGrid.Length; i++)
        {
            var sum = SumNeighbourValues(srcGrid, i);
            dstGrid[i] = sum switch
            {
                0 => 0,
                1 => 0,
                7 => 1,
                8 => 1,
                _ => srcGrid[i]
            };
            //dstGrid[i] = (byte)(sum > 4 ? 1:0);
            //dstGrid[i] = (byte)(sum > 6 ? 1 :
            //					sum < 4 ? 0 :
            //					srcGrid[i]);
            if (firstNonEmptyIndex < 0 && dstGrid[i] == 1)
                firstNonEmptyIndex = i - w - 1;
        }
        return dstGrid;
    }

    private int SumNeighbourValues(byte[] grid, int id)
    {
        var sum = 0;
        var neighbours = NeighborIds(id);
        for (var i = 0; i < neighbours.Length; i++)
        {
            if (neighbours[i] >= 0 && neighbours[i] < grid.Length) sum += grid[neighbours[i]];
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
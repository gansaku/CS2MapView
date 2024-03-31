namespace CS2MapView.Drawing.Terrain;

internal class LandVectorContourBuilder : AbstractVectorContourBuilder
{
    internal required float SeaLevel { get; init; }

    private float GetHeight(ReadOnlySpan<float> input, int x, int y)
    {
        if (x < 0 || x >= InputWidth)
        {
            return 0;
        }
        if (y < 0 || y >= InputHeight)
        {
            return 0;
        }
        return input[y * InputWidth + x] - SeaLevel;
    }


    protected override Dictionary<int, NodeInfo> CreateNodeInfo()
    {
        var result = new Dictionary<int, NodeInfo>();
        ReadOnlySpan<float> input = InputArray;

        for (int y = 0; y < InputHeight; y++)
        {
            for (int x = 0; x < InputWidth; x++)
            {
                float hThis = GetHeight(input, x, y);
                if (hThis >= TargetHeight)
                {
                    var ni = new NodeInfo
                    {
                        Up = GetHeight(input, x, y - 1) < TargetHeight,
                        Down = GetHeight(input, x, y + 1) < TargetHeight,
                        Left = GetHeight(input, x - 1, y) < TargetHeight,
                        Right = GetHeight(input, x + 1, y) < TargetHeight
                    };
                    if (ni.AnyMarked)
                    {
                        result.Add(y * InputWidth + x, ni);
                    }
                }
            }
        }

        return result;
    }

    protected override void SetPathCommand(IEnumerable<ContourSegments> segmentsGrouped)
    {
        var pathes = CreateSKPathes(segmentsGrouped);
        var contourColors = Context.Theme?.Colors?.LerpLandContourColors(TargetHeight);
        if (contourColors is null)
        {
            return;
        }
        var borderStyle = Context.ContourHeights.LandHeights?.FirstOrDefault(lc => lc.Height == TargetHeight);

        var stroke = borderStyle?.WithColor(contourColors.BorderColor);
        foreach (var path in pathes)
        {

            var cmd = new PathDrawCommand { Path = path, FillColor = contourColors.FillColor, StrokePaintFunc = stroke is null ? null : stroke.ToCacheKey };
            ResultCommands.Add(cmd);
        }
    }
}

namespace CS2MapView.Drawing.Terrain;

internal class WaterVectorContourBuilder : AbstractVectorContourBuilder
{
    internal required bool IsBottom { get; set; }
    protected override bool PathJoinRequired => IsBottom;

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
        return input[y * InputWidth + x];
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
                if (hThis >= TargetHeight + 0.2f)
                {
                    NodeInfo ni = new()
                    {
                        Up = GetHeight(input, x, y - 1) < TargetHeight + 0.2f,
                        Down = GetHeight(input, x, y + 1) < TargetHeight + 0.2f,
                        Left = GetHeight(input, x - 1, y) < TargetHeight + 0.2f,
                        Right = GetHeight(input, x + 1, y) < TargetHeight + 0.2f
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
        var pathes = IsBottom ? CreateSKPathesMerged(segmentsGrouped) : CreateSKPathes(segmentsGrouped);
        var contourColors = Context.Theme?.Colors?.LerpWaterContourColors(TargetHeight);
        if (contourColors is null)
        {
            return;
        }
        var borderStyle = Context.ContourHeights.WaterHeights?.FirstOrDefault(lc => lc.Height == TargetHeight);

        var stroke = borderStyle?.WithColor(contourColors.BorderColor);
        foreach (var path in pathes)
        {

            var cmd = new PathDrawCommand { Path = path, FillColor = contourColors.FillColor, StrokePaintFunc = stroke is null ? null : stroke.ToCacheKey };
            ResultCommands.Add(cmd);
        }

    }
}



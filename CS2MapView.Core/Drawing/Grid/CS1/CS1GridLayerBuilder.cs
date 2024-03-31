using CS2MapView.Data;
using CS2MapView.Drawing.Layer;
using SkiaSharp;

namespace CS2MapView.Drawing.Grid.CS1;

/// <summary>
/// CS1のグリッド描画
/// </summary>
/// <remarks>
/// コンストラクタ
/// </remarks>
/// <param name="AppRoot"></param>
public class CS1GridLayerBuilder(ICS2MapViewRoot AppRoot)
{
    private BasicLayer? ResultLayer { get; set; }

    /// <summary>
    /// 描画オブジェクトの構築
    /// </summary>
    /// <param name="loadProgressInfo"></param>
    /// <returns></returns>
    public Task<ILayer> BuildAsync(LoadProgressInfo? loadProgressInfo)
    {
        return Task.Run<ILayer>(() =>
        {
            ResultLayer = new BasicLayer(AppRoot, ILayer.LayerNameGrid, CS1MapType.CS1WorldRect);
            var color = AppRoot.Context.Theme.Colors?.GridLine;
            if (color is not null)
            {
                var stroke = AppRoot.Context.Theme.Strokes?.Grid?.WithColor(color.Value);
                if (stroke is not null)
                {

                    var wr = CS1MapType.CS1WorldRect;
                    for (int y = 0; y <= 9; y++)
                    {
                        float fy = wr.Top + wr.Height / 9f * y;
                        var path = new SKPath();
                        path.MoveTo(wr.Left, fy);
                        path.LineTo(wr.Right, fy);
                        ResultLayer.DrawCommands.Add(new PathDrawCommand { Path = path, StrokePaintFunc = stroke.ToCacheKey });
                    }
                    for (int x = 0; x <= 9; x++)
                    {
                        float fx = wr.Left + wr.Width / 9f * x;
                        var path = new SKPath();
                        path.MoveTo(fx, wr.Top);
                        path.LineTo(fx, wr.Bottom);
                        ResultLayer.DrawCommands.Add(new PathDrawCommand { Path = path, StrokePaintFunc = stroke.ToCacheKey });
                    }

                }
            }

            loadProgressInfo?.Progress(this, LoadProgressInfo.Process.BuildGrid, 1f, null);
            return ResultLayer;
        });
    }
}

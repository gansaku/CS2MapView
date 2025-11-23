using CS2MapView.Data;
using CS2MapView.Drawing.Layer;
using CS2MapView.Serialization;
using SkiaSharp;

namespace CS2MapView.Drawing.Grid.CS2;

/// <summary>
/// CS2のグリッド描画
/// </summary>
/// <remarks>
/// コンストラクタ
/// </remarks>
/// <param name="AppRoot"></param>
/// <param name="ImportData"></param>
public class CS2GridLayerBuilder(ICS2MapViewRoot AppRoot, CS2MapDataSet ImportData)
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
            // 正しい WorldRect を使用（MapExt2 対応）
            ReadonlyRect worldRect;
            if (ImportData.MainData?.WorldBounds != null)
            {
                var bounds = ImportData.MainData.WorldBounds;
                worldRect = new ReadonlyRect(bounds.MinX, bounds.MinZ, bounds.MaxX, bounds.MaxZ);
            }
            else
            {
                worldRect = CS2MapType.CS2WorldRect;
            }
            
            ResultLayer = new BasicLayer(AppRoot, ILayer.LayerNameGrid, worldRect);
            var color = AppRoot.Context.Theme.Colors?.GridLine;
            if (color is not null)
            {
                var stroke = AppRoot.Context.Theme.Strokes?.Grid?.WithColor(color.Value);
                if (stroke is not null)
                {
                    var wr = worldRect;
                    
                    // グリッド数を計算（実際のマップサイズに基づく）
                    // バニラのグリッドセルサイズは (14336/23) ≈ 623 メートルと仮定
                    const float vanillaGridSize = 14336f / 23f;
                    int gridCountX = (int)Math.Ceiling(wr.Width / vanillaGridSize);
                    int gridCountY = (int)Math.Ceiling(wr.Height / vanillaGridSize);
                    
                    // 水平線を描画
                    for (int y = 0; y <= gridCountY; y++)
                    {
                        float fy = wr.Top + wr.Height / gridCountY * y;
                        var path = new SKPath();
                        path.MoveTo(wr.Left, fy);
                        path.LineTo(wr.Right, fy);
                        ResultLayer.DrawCommands.Add(new PathDrawCommand { Path = path, StrokePaintFunc = stroke.ToCacheKey });
                    }
                    
                    // 垂直線を描画
                    for (int x = 0; x <= gridCountX; x++)
                    {
                        float fx = wr.Left + wr.Width / gridCountX * x;
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

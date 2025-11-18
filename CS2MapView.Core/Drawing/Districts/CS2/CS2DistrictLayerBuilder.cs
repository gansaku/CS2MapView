using CS2MapView.Data;
using CS2MapView.Drawing.Layer;
using CS2MapView.Serialization;
using CS2MapView.Theme;
using CS2MapView.Util.CS2;
using SkiaSharp;

namespace CS2MapView.Drawing.Districts.CS2
{

    public class CS2DistrictLayerBuilder(ICS2MapViewRoot AppRoot, CS2MapDataSet ExportData)
    {
        public Task<ILayer> BuildAsync(LoadProgressInfo? loadProgressInfo)
        {
            static IEnumerable<(SKPoint, SKPoint)> EnumLines(SKPath? path)
            {
                if (path is null)
                {
                    yield break;
                }
                for (int i = 0; i < path.Points.Length; i++)
                {
                    yield return (path.Points[i], path.Points[(i + 1) % path.Points.Length]);
                }
            }
            return Task.Run<ILayer>(() =>
            {
                // 正しい WorldRect を使用（MapExt2 対応）
                ReadonlyRect worldRect;
                if (ExportData.MainData?.WorldBounds != null)
                {
                    var bounds = ExportData.MainData.WorldBounds;
                    worldRect = new ReadonlyRect(bounds.MinX, bounds.MinZ, bounds.MaxX, bounds.MaxZ);
                }
                else
                {
                    worldRect = CS2MapType.CS2WorldRect;
                }
                
                var layer = new BasicLayer(AppRoot, ILayer.LayerNameDistricts, worldRect);
                var color = AppRoot.Context.Theme?.Colors?.DistrictBoundary ?? new SerializableColor();
                var stroke = AppRoot.Context.Theme?.Strokes?.DistrictBoundary?.WithColor(color);
                var pathes = (ExportData.Districts ?? []).Select(d => d.Geometry?.ToPath());

                var lines = pathes.SelectMany(EnumLines);
                lines = lines.Select(tpl =>
                {
                    if ((tpl.Item1.X > tpl.Item2.X) || (tpl.Item1.X == tpl.Item2.X && tpl.Item1.Y > tpl.Item2.Y))
                    {
                        return (tpl.Item2, tpl.Item1);
                    }
                    return tpl;
                }).Distinct();

                foreach (var w in lines)
                {
                    var path = new SKPath();
                    path.MoveTo(w.Item1);
                    path.LineTo(w.Item2);
                    layer.DrawCommands.Add(new PathDrawCommand
                    {
                        Path = path,
                        StrokePaintFunc = stroke is null ? null : stroke.ToCacheKey

                    });
                }

                loadProgressInfo?.Progress(this, LoadProgressInfo.Process.BuildDistricts, 1f, null);
                return layer;
            });


        }


    }
}

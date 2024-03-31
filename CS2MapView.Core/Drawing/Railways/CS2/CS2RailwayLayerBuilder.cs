using CS2MapView.Data;
using CS2MapView.Drawing.Labels;
using CS2MapView.Drawing.Layer;
using CS2MapView.Serialization;
using CS2MapView.Theme;
using CS2MapView.Util;
using CS2MapView.Util.CS2;
using SkiaSharp;

namespace CS2MapView.Drawing.Railways.CS2
{

    /// <summary>
    /// CS2線路描画
    /// </summary>
    public class CS2RailwayLayerBuilder
    {

        private ICS2MapViewRoot AppRoot { get; init; }
        private CS2MapDataSet ImportData { get; init; }
        private BasicLayer ResultLayer { get; init; }
      
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="appRoot"></param>
        /// <param name="importData"></param>
        public CS2RailwayLayerBuilder(ICS2MapViewRoot appRoot, CS2MapDataSet importData)
        {
            AppRoot = appRoot;
            ImportData = importData;
            ResultLayer = new BasicLayer(appRoot, ILayer.LayerNameRailways, CS2MapType.CS2WorldRect);
        }
        /// <summary>
        /// 初期構築
        /// </summary>
        /// <param name="loadProgressInfo"></param>
        /// <returns></returns>
        public Task<ILayer> BuildAsync(LoadProgressInfo? loadProgressInfo)
        {
            return Task.Run<ILayer>(() =>
            {
                var trainRails = (ImportData?.RailInfo?.RailSegments ?? []).Where(t => (t.RailType & CS2RailType.Train) != 0);
                var tramRails = (ImportData?.RailInfo?.RailSegments ?? []).Where(t => (t.RailType & CS2RailType.Tram) != 0);
                var subwayRails = (ImportData?.RailInfo?.RailSegments ?? []).Where(t => (t.RailType & CS2RailType.Subway) != 0);
                var nodes = ImportData?.RailInfo?.RailNodes ?? [];

                var trainsRailsG = CS2NetSegmentSupport.SimplifySegments(trainRails.Where(t => t.Elevation.X + t.Elevation.Y >= 0), nodes, (a, b) => a.IsStation == b.IsStation, out _);
                var trainsRailsTnG = CS2NetSegmentSupport.SimplifySegments(trainRails.Where(t => t.Elevation.X + t.Elevation.Y < 0), nodes, (a, b) => a.IsStation == b.IsStation, out _);
                var tramsRailsG = CS2NetSegmentSupport.SimplifySegments(tramRails, nodes, (a, b) => true, out _);
                var subwaysRailsG = CS2NetSegmentSupport.SimplifySegments(subwayRails, nodes, (a, b) => a.IsStation == b.IsStation, out _);

                var strokes = AppRoot.Context.Theme.Strokes ?? new();
                var colors = AppRoot.Context.Theme.Colors ?? new();

                var stationStroke = strokes.Station;

                static SKPath GetStationPath(List<CS2RailSegment> segmentGroup) => SKPathEx.CreateRoundBorderedLinePath(
                        new SKPoint(segmentGroup.First().Curve.X0, segmentGroup.First().Curve.Y0),
                        new SKPoint(segmentGroup.Last().Curve.X3, segmentGroup.Last().Curve.Y3),
                        16, false, false);
                ;


                void AddPathCommands(List<List<CS2RailSegment>> segmentGroups, StrokeStyleWithColor? sc1, StrokeStyleWithColor? sc2, StrokeStyleWithColor? scStation1 = null, SerializableColor? stationColor = null)
                {
                    if (sc1 is null && sc2 is null) return;

                    foreach (var seg in segmentGroups)
                    {

                        if ((scStation1 is not null || stationColor.HasValue) && seg.First().IsStation)
                        {
                            var path = GetStationPath(seg);

                            ResultLayer.DrawCommands.Add(new PathDrawCommand { Path = path, StrokePaintFunc = scStation1 is null ? null : scStation1.ToCacheKey, FillColor = stationColor });



                        }
                        else
                        {
                            var path = CS2NetSegmentSupport.GetGroupdSegmentsPath(seg);
                            if (sc1 is not null)
                            {
                                ResultLayer.DrawCommands.Add(new PathDrawCommand { Path = path, StrokePaintFunc = sc1.ToCacheKey });
                            }
                            if (sc2 is not null)
                            {
                                ResultLayer.DrawCommands.Add(new PathDrawCommand { Path = path, StrokePaintFunc = sc2.ToCacheKey });
                            }
                        }


                    }
                }
                static StrokeStyleWithColor? GetSSWC(StrokeStyle? ss, SerializableColor? color)
                {
                    if (ss is null || color is null) return null;
                    return ss.WithColor(color.Value);
                }

                static StrokeStyleWithColor? GetStationSC(StrokeStyle? stationStroke, SerializableColor? color)
                {
                    if (stationStroke is null || color is null)
                    {
                        return null;
                    }

                    return stationStroke.WithColor(color.Value);
                }
                var userSettings = AppRoot.Context.UserSettings;
                if (userSettings.RenderRailTram)
                {
                    AddPathCommands(tramsRailsG,
                        GetSSWC(strokes.TramRailway, colors.Tramway),
                        GetSSWC(strokes.TramRailwayDash, colors.TramwayDash));
                }
                if (userSettings.RenderRailMetro)
                {
                    AddPathCommands(subwaysRailsG,
                        GetSSWC(strokes.MetroRailway, colors.Metroway),
                        GetSSWC(strokes.MetroRailwayDash, colors.MetrowayDash),
                        GetStationSC(stationStroke, colors.Metroway),
                        colors.MetrowayDash);
                }
                if (userSettings.RenderRailTrain)
                {
                    AddPathCommands(trainsRailsTnG,
                        GetSSWC(strokes.TrainTunnel, colors.TrainTunnel),
                        GetSSWC(strokes.TrainTunnelDash, colors.TrainTunnelDash),
                        GetStationSC(stationStroke, colors.Train),
                        colors.TrainDash);
                    AddPathCommands(trainsRailsG,
                        GetSSWC(strokes.Train, colors.Train),
                        GetSSWC(strokes.TrainDash, colors.TrainDash),
                        GetStationSC(stationStroke, colors.Train),
                        colors.TrainDash);
                }

                loadProgressInfo?.Progress(this, LoadProgressInfo.Process.BuildRailways, 1f, null);
                return ResultLayer;

            });
        }

    }
}

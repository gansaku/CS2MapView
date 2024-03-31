using CS2MapView.Data;
using CS2MapView.Drawing.Layer;
using CS2MapView.Theme;
using CS2MapView.Util;
using CS2MapView.Util.CS1;
using CSLMapView.XML;
using SkiaSharp;

namespace CS2MapView.Drawing.Railways.CS1;


public class CS1RailWayLayerBuilder : IRebuildOnResizeLayerBuilder
{
    private ICS2MapViewRoot AppRoot { get; init; }
    private CSLExportXML ExportXML { get; init; }
    private RebuildableLayer ResultLayer { get; init; }
    private CS1SegmentManager SegmentManager { get; init; }


    private List<List<XMLSegment>> TrainSegments { get; set; }
    private List<List<XMLSegment>> TramSegments { get; set; }
    private List<List<XMLSegment>> MetroSegments { get; set; }
    private List<List<XMLSegment>> CableCarSegments { get; set; }
    private List<List<XMLSegment>> MonorailSegments { get; set; }
    private List<XMLSegment> StationSegments { get; } = [];

    private List<IDrawCommand>? MetroDrawCommands { get; set; }
    private List<IDrawCommand>? TramDrawCommands { get; set; }
    private List<IDrawCommand>? CableCarDrawCommands { get; set; } //require rebuild
    private List<IDrawCommand>? MonorailDrawCommands { get; set; } //require rebuild
    private List<IDrawCommand>? TrainDrawCommands { get; set; }
    private List<IDrawCommand>? StationDrawCommands { get; set; }

#pragma warning  disable CS8618
    internal CS1RailWayLayerBuilder(ICS2MapViewRoot appRoot, CSLExportXML xml)
    {
        AppRoot = appRoot;
        ExportXML = xml;
        SegmentManager = new CS1SegmentManager(xml);
        ResultLayer = new RebuildableLayer(AppRoot, ILayer.LayerNameRailways, CS1MapType.CS1WorldRect, this);
    }
#pragma warning restore
    public LoadProgressInfo.Process ProcessType => LoadProgressInfo.Process.BuildRailways;



    public Task<ILayer> BuildAsync(LoadProgressInfo? loadProgressInfo, ViewContext vc)
    {

        return Task.Run<ILayer>(() =>
        {
            lock (ResultLayer.DrawObjectsLock)
            {
                PrepareSegments();

                BuildOnce();
                BuildResizable(vc);
                SetCommandsToLayer();
                loadProgressInfo?.Progress(this, LoadProgressInfo.Process.BuildRailways, 1f, null);
                return ResultLayer;
            }
        });
    }
    public Task ResizeAsync(LoadProgressInfo? loadProgressInfo, ViewContext vc)
    {
        return Task.Run(() =>
        {
            lock (ResultLayer.DrawObjectsLock)
            {
                ResultLayer.DrawCommands.ClearContent(false);

                CableCarDrawCommands?.ForEach(t => t.Dispose());
                CableCarDrawCommands = null;
                BuildResizable(vc);
                SetCommandsToLayer();
                loadProgressInfo?.Progress(this, LoadProgressInfo.Process.BuildRailways, 1f, null);
            }
        });
    }
    private void PrepareSegments()
    {
        var trainList = new List<XMLSegment>();
        var tramsList = new List<XMLSegment>();
        var metroList = new List<XMLSegment>();
        var cablecarList = new List<XMLSegment>();
        var monorailList = new List<XMLSegment>();
        var settinigs = AppRoot.Context.UserSettings;
        foreach (XMLSegment seg in ExportXML.SegmentList)
        {

            WayType wayType = SegmentManager.GetWayType(seg);
            if (wayType.HasFlag(WayType.Transport))
            {
                continue;
            }
            if (wayType.HasFlag(WayType.Rail))
            {
                if (seg.Name.Contains("Station") || seg.Name.Contains("Train Cargo Track"))
                {
                    StationSegments.Add(seg);
                }
                else
                {
                    trainList.Add(seg);
                }
            }
            if (wayType.HasFlag(WayType.Tram))
            {
                tramsList.Add(seg);
            }
            if (wayType.HasFlag(WayType.Metro))
            {
                if (seg.Name.Contains("Station"))
                {
                    StationSegments.Add(seg);
                }
                else
                {
                    metroList.Add(seg);
                }
            }
            if (wayType.HasFlag(WayType.Monorail) && settinigs.RenderRailMonorail)
            {
                if (seg.Name.Contains("Station"))
                {
                    StationSegments.Add(seg);
                }
                else
                {
                    monorailList.Add(seg);
                }
            }
            if (wayType.HasFlag(WayType.CableCar) && settinigs.RenderRailCableCar)
            {
                if (seg.Name.Contains("Stop"))
                {
                    StationSegments.Add(seg);
                }
                else
                {
                    cablecarList.Add(seg);
                }
            }

        }
        TrainSegments = SegmentManager.GroupDirectRailway(trainList, WayType.Rail);
        TramSegments = SegmentManager.GroupDirectRailway(tramsList, WayType.Tram);
        MetroSegments = SegmentManager.GroupDirectRailway(metroList, WayType.Metro);
        CableCarSegments = SegmentManager.GroupDirectRailway(cablecarList, WayType.CableCar);
        MonorailSegments = SegmentManager.GroupDirectRailway(monorailList, WayType.Monorail);
    }
    private void BuildOnce()
    {
        var settings = AppRoot.Context.UserSettings;
        var theme = AppRoot.Context.Theme;
        var strokes = theme.Strokes;
        var colors = theme.Colors;
        if (strokes is null)
        {
            throw new InvalidOperationException("null..theme/stroke");
        }
        if (colors is null)
        {
            throw new InvalidOperationException("null..theme/colors");
        }

        if (settings.RenderRailMetro)
        {
            (var metroPathes, _) = BuildBasicRailwaysPathes(MetroSegments);
            MetroDrawCommands = GetBasicRailwayDrawCommands(metroPathes, strokes.MetroRailway, strokes.MetroRailwayDash, colors.Metroway, colors.MetrowayDash);
            MetroDrawCommands.AddRange(BuildAndGetMetroStationPathCommand());
        }
        if (settings.RenderRailTram)
        {
            (var tramPathes, _) = BuildBasicRailwaysPathes(TramSegments);
            TramDrawCommands = GetBasicRailwayDrawCommands(tramPathes, strokes.TramRailway, strokes.TramRailwayDash, colors.Tramway, colors.TramwayDash);
        }
        if (settings.RenderRailTrain)
        {
            (var railPathes, var railTunnelPathes) = BuildBasicRailwaysPathes(TrainSegments);
            TrainDrawCommands = GetBasicRailwayDrawCommands(railTunnelPathes, strokes.TrainTunnel, strokes.TrainTunnelDash, colors.TrainTunnel, colors.TrainTunnelDash);
            TrainDrawCommands.AddRange(GetBasicRailwayDrawCommands(railPathes, strokes.Train, strokes.TrainDash, colors.Train, colors.TrainDash));

        }
        StationDrawCommands = BuildAndGetStationsWithoutMetroPathCommands().ToList();


    }

    private void BuildResizable(ViewContext vc)
    {
        var theme = AppRoot.Context.Theme;
        var strokes = theme.Strokes;
        var colors = theme.Colors;

        var settings = AppRoot.Context.UserSettings;

        if (strokes is null)
        {
            throw new InvalidOperationException("null..theme/stroke");
        }
        if (colors is null)
        {
            throw new InvalidOperationException("null..theme/colors");
        }
        if (settings.RenderRailCableCar && strokes.CableCar is not null)
        {
            CableCarDrawCommands = BuildAndGetCableCarsMonorailPathCommand(vc, CableCarSegments, strokes.CableCar, colors.CableCarWay, theme.CableCarDecorationLength, theme.CableCarDecorationSpan);
        }
        if (settings.RenderRailMonorail && strokes.Monorail is not null)
        {
            MonorailDrawCommands = BuildAndGetCableCarsMonorailPathCommand(vc, MonorailSegments, strokes.Monorail, colors.MonorailWay, theme.MonorailDecorationLength, theme.MonorailDecorationSpan);
        }
    }

    private void SetCommandsToLayer()
    {
        ResultLayer.DrawCommands.AddRange(MetroDrawCommands);
        ResultLayer.DrawCommands.AddRange(TramDrawCommands);
        ResultLayer.DrawCommands.AddRange(CableCarDrawCommands);
        ResultLayer.DrawCommands.AddRange(MonorailDrawCommands);
        ResultLayer.DrawCommands.AddRange(TrainDrawCommands);
        ResultLayer.DrawCommands.AddRange(StationDrawCommands);
    }


    private List<IDrawCommand> GetBasicRailwayDrawCommands(List<SKPath> pathes, StrokeStyle? baseStroke, StrokeStyle? dashStroke, SKColor baseColor, SKColor dashColor)
    {
        List<IDrawCommand> result = [];
        SKPaintCache.StrokeKey? ToBaseKey(float scale, float worldScale) => baseStroke.WithColor(baseColor).ToCacheKey(scale, worldScale);
        SKPaintCache.StrokeKey? ToDashKey(float scale, float worldScale) => dashStroke.WithColor(dashColor).ToCacheKey(scale, worldScale);

        if (baseStroke is not null)
        {
            foreach (var path in pathes)
            {
                var cmd = new PathDrawCommand() { Path = path, StrokePaintFunc = ToBaseKey };
                result.Add(cmd);
            }
        }
        if (dashStroke is not null)
        {
            foreach (var path in pathes)
            {
                var cmd = new PathDrawCommand() { Path = path, StrokePaintFunc = ToDashKey };
                result.Add(cmd);
            }
        }
        return result;
    }

    private (List<SKPath> normal, List<SKPath> tunnel) BuildBasicRailwaysPathes(List<List<XMLSegment>> targetList)
    {
        var result = new List<SKPath>();
        var resultTunnel = new List<SKPath>();

        foreach (List<XMLSegment> segList in targetList)
        {

            XMLSegment seg0 = segList[0];
            WayType dwt = SegmentManager.GetWayType(seg0);

            bool isTunnel = dwt.HasBit(WayType.Rail) && dwt.HasBit(WayType.Tunnel);

            SKPath path = new();
            bool first = true;
            path.MoveTo(segList.First().Points[0].ToMapSpace());
            foreach (XMLSegment xmlseg in segList)
            {
                foreach (XMLVector3 p in xmlseg.Points)
                {
                    if (first)
                    {
                        path.MoveTo(p.ToMapSpace());
                        first = false;
                    }
                    else
                    {
                        path.LineTo(p.ToMapSpace());
                    }
                }
            }

            if (isTunnel)
            {
                resultTunnel.Add(path);
            }
            else
            {
                result.Add(path);
            }
        }
        return (result, resultTunnel);
    }


    private List<IDrawCommand> BuildAndGetMetroStationPathCommand()
    {
        var result = new List<IDrawCommand>();
        var origColor = AppRoot.Context.Theme.Colors!.Metroway;
        var origDashColor = AppRoot.Context.Theme.Colors!.MetrowayDash;

        var newBaseStroke = AppRoot.Context.Theme.Strokes!.Station!;
        var newStrokeStyle = newBaseStroke.WithColor(origColor);

        foreach (XMLSegment s in StationSegments)
        {
            WayType wayType = SegmentManager.GetWayType(s);
            if (wayType.HasFlag(WayType.Metro))
            {
                var path = SKPathEx.CreateRoundBorderedLinePath(s.Points[0].ToMapSpace(), s.Points[^1].ToMapSpace(), AppRoot.Context.Theme.MetroStationWidth, false, false);

                var cmd = new PathDrawCommand { Path = path, StrokePaintFunc = newStrokeStyle.ToCacheKey, FillColor = origDashColor };
                result.Add(cmd);

            }
        }
        return result;
    }
    private List<IDrawCommand> BuildAndGetCableCarsMonorailPathCommand(ViewContext vc, List<List<XMLSegment>> segments, StrokeStyle baseStroke, SKColor color, Width decorationLengthObj, float decorationSpan)
    {

        var resultPathes = new List<SKPath>();
        var resultDecorationPathes = new List<SKPath>();

        foreach (List<XMLSegment> segList in segments)
        {
            XMLSegment seg0 = segList[0];

            SKPath path = new();
            XMLVector3? last = null;
            List<SKPoint> points = [];
            bool first = true;
            foreach (XMLSegment seg in segList)
            {
                foreach (XMLVector3 vec in seg.Points)
                {
                    if (last == null || last != vec)
                    {
                        if (first)
                        {
                            path.MoveTo(vec.ToMapSpace());
                            first = false;
                        }
                        else
                        {
                            path.LineTo(vec.ToMapSpace());
                        }
                        points.Add(vec.ToMapSpace());
                        last = vec;
                    }
                }
            }

            resultPathes.Add(path);

            float inverseFactor = 1f / (vc.ScaleFactor * vc.WorldScaleFactor);

            var baseWidth = baseStroke.Width;
            var baseLineWidth = baseWidth.StrokeWidthByScale(vc.ScaleFactor, vc.WorldScaleFactor) ?? 0f;
            var decorationLenW = decorationLengthObj.StrokeWidthByScale(vc.ScaleFactor, vc.WorldScaleFactor);

            double decorationLength = (decorationLenW ?? 0) + baseLineWidth;
            double left = decorationSpan / 2;
            while (points.Count >= 2)
            {
                double lineLen = MathEx.Distance(points[0], points[1]);
                if (lineLen < left)
                {
                    left -= lineLen;
                    points.RemoveAt(0);
                }
                else
                {

                    double angle = MathEx.Angle(points[0], points[1]);
                    SKPoint point = MathEx.SlidePoint(points[0], angle, left);

                    SKPoint p1 = MathEx.SlidePoint(point, angle + Math.PI / 2, decorationLength);
                    SKPoint p2 = MathEx.SlidePoint(point, angle - Math.PI / 2, decorationLength);

                    SKPath dpath = new();
                    dpath.MoveTo(p1);
                    dpath.LineTo(p2);
                    resultDecorationPathes.Add(dpath);

                    points[0] = point;
                    left = decorationSpan / 2;
                }
            }


        }
        var stroke = baseStroke;
        var strokec = stroke!.WithColor(color);
        var result = new List<IDrawCommand>();
        foreach (var p in resultPathes)
        {
            result.Add(new PathDrawCommand { Path = p, StrokePaintFunc = strokec.ToCacheKey });
        }
        var stroke2 = AppRoot.Context.Theme.Strokes!.CableCarDecoration;
        var strokec2 = stroke2!.WithColor(color);
        foreach (var p in resultDecorationPathes)
        {
            result.Add(new PathDrawCommand { Path = p, StrokePaintFunc = strokec2.ToCacheKey });
        }
        return result;
    }


    #region implied
    /*
    internal DrawingGroup BuildRailShape(CSLExportXML xml)
    {



        Dictionary<uint, PathGeometry> objects = new Dictionary<uint, PathGeometry>();
        foreach (uint i in new uint[] { (uint)WayType.Rail, (uint)WayType.Rail | (uint)WayType.Tunnel, (uint)WayType.Metro, (uint)WayType.Tram,
            (uint)WayType.CableCar, (uint)WayType.Monorail ,(uint)WayType.CableCar | (uint)WayType.ReservedForRendering, (uint)WayType.Monorail|(uint)WayType.ReservedForRendering })
        {
            PathGeometry pg = new PathGeometry();
            objects.Add(i, pg);
        }

        BuildTrains(metros, objects, WayType.Metro);
        BuildTrains(trams, objects, WayType.Tram);
        BuildTrains(rails, objects, WayType.Rail);
        BuildCableCars(cableCars, objects);
        BuildMonorails(monorails, objects);

    using (DrawingContext dc = dgroup.Open())
    {

        PaintResources paint = new PaintResources(RContext);
        dc.DrawGeometry(null, paint.MetroPen, objects[(uint)WayType.Metro]);
        dc.DrawGeometry(null, paint.MetroDashPen, objects[(uint)WayType.Metro]);
        if (RContext.AppConfig.RenderRailMetro)
        {
            DrawMetroStation(dc, xml, paint);
        }

        dc.DrawGeometry(null, paint.TramPen, objects[(uint)WayType.Tram]);
        dc.DrawGeometry(null, paint.TramDashPen, objects[(uint)WayType.Tram]);
        dc.DrawGeometry(null, paint.CableCarPen, objects[(uint)WayType.CableCar]);
        dc.DrawGeometry(null, paint.CableCarDecorationPen, objects[(uint)WayType.CableCar | (uint)WayType.ReservedForRendering]);
        dc.DrawGeometry(null, paint.MonorailPen, objects[(uint)WayType.Monorail]);
        dc.DrawGeometry(null, paint.MonorailDecorationPen, objects[(uint)WayType.Monorail | (uint)WayType.ReservedForRendering]);

        dc.DrawGeometry(null, paint.RailTunnelPen, objects[(uint)WayType.Rail | (uint)WayType.Tunnel]);
        dc.DrawGeometry(null, paint.RailPen, objects[(uint)WayType.Rail]);
        dc.DrawGeometry(null, paint.RailDashPen, objects[(uint)WayType.Rail]);

        DrawStationOnGround(dc, xml, paint);

    }

    }
    */
    #endregion


    private IEnumerable<IDrawCommand> BuildAndGetStationsWithoutMetroPathCommands()
    {
        var strokes = AppRoot.Context.Theme.Strokes;
        if (strokes is null)
        {
            yield break;
        }
        var stroke = strokes.Station;
        var userSettings = AppRoot.Context.UserSettings;
        if (stroke is null)
        {
            yield break;
        }
        var colors = AppRoot.Context.Theme.Colors;
        if (colors is null)
        {
            yield break;
        }
        var pathes = new List<SKPath>();

        foreach (XMLSegment s in StationSegments)
        {
            WayType wayType = SegmentManager.GetWayType(s);

            if (wayType.HasFlag(WayType.Rail))
            {
                if (userSettings.RenderRailTrain && strokes.Train is not null)
                {
                    var width = AppRoot.Context.Theme.TrainStationWidth;
                    pathes.Add(SKPathEx.CreateRoundBorderedLinePath(s.Points[0].ToMapSpace(), s.Points[^1].ToMapSpace(), width, false, false));
                }
            }
            else if (wayType.HasFlag(WayType.Monorail))
            {
                if (userSettings.RenderRailMonorail && strokes.Monorail is not null)
                {
                    var width = AppRoot.Context.Theme.MonorailStationWidth;
                    pathes.Add(SKPathEx.CreateRoundBorderedLinePath(s.Points[0].ToMapSpace(), s.Points[^1].ToMapSpace(), width, false, false));

                }
            }
            else if (wayType.HasFlag(WayType.CableCar))
            {
                if (userSettings.RenderRailCableCar && strokes.CableCar is not null)
                {
                    var width = AppRoot.Context.Theme.CableCarStationWidth;
                    pathes.Add(SKPathEx.CreateRoundBorderedLinePath(s.Points[0].ToMapSpace(), s.Points[^1].ToMapSpace(), width, false, false));
                }
            }
        }
        yield return new PathDrawCommand { Path = SKPathEx.Union(pathes), FillColor = colors.TrainDash, StrokePaintFunc = stroke.WithColor(colors.Train).ToCacheKey };

    }

    /*
    private void DoubleWidthPen(Pen pen)
    {
        pen.Thickness *= 2;
        if (pen.DashStyle != null && pen.DashStyle.Dashes != null)
        {
            for (int i = 0; i < pen.DashStyle.Dashes.Count; i++)
            {
                pen.DashStyle.Dashes[i] /= 2;
            }
        }
    }
    
    private class PaintResources
    {
        internal Pen RailPen;
        internal Pen RailDashPen;
        internal Pen RailTunnelPen;
        internal Pen TramPen;
        internal Pen TramDashPen;
        internal Pen MetroPen;
        internal Pen MetroDashPen;
        internal Brush TrainStationBrush;
        internal Brush MetroStationBrush;
        internal Pen TrainStationBorderPen;
        internal Pen MetroStationBorderPen;

        internal Pen CableCarPen;
        internal Pen CableCarDecorationPen;
        internal Pen MonorailPen;
        internal Pen MonorailDecorationPen;

        internal PaintResources(RenderContext rcon)
        {
            MapStyle config = rcon.CurrentMapStyle;
            RailPen = new Pen(FreezedDrawingObjects.GetSolidBrush(config.Colors.Railway), config.WayWidth.TrainRail.CalcSize(rcon.RenderScale));
            RailDashPen = new Pen(FreezedDrawingObjects.GetSolidBrush(config.Colors.RailwayDash), config.WayWidth.TrainRailDash.CalcSize(rcon.RenderScale));
            RailTunnelPen = new Pen(FreezedDrawingObjects.GetSolidBrush(config.Colors.RailwayTunnel), config.WayWidth.TrainRailTunnel.CalcSize(rcon.RenderScale));
            TramPen = new Pen(FreezedDrawingObjects.GetSolidBrush(config.Colors.Tramway), config.WayWidth.TramRail.CalcSize(rcon.RenderScale));
            TramDashPen = new Pen(FreezedDrawingObjects.GetSolidBrush(config.Colors.TramwayDash), config.WayWidth.TramRailDash.CalcSize(rcon.RenderScale));
            MetroPen = new Pen(FreezedDrawingObjects.GetSolidBrush(config.Colors.Metroway), config.WayWidth.MetroRail.CalcSize(rcon.RenderScale));
            MetroDashPen = new Pen(FreezedDrawingObjects.GetSolidBrush(config.Colors.MetrowayDash), config.WayWidth.MetroRailDash.CalcSize(rcon.RenderScale));
            RailDashPen.DashStyle = new DashStyle(new double[] { 2d, 4d }, -0.5d);
            RailTunnelPen.DashStyle = new DashStyle(new double[] { 4d, 3d }, -0.5d);
            MetroDashPen.DashStyle = new DashStyle(new double[] { 2d, 4d }, -0.5d);
            TramDashPen.DashStyle = new DashStyle(new double[] { 2d, 4d }, -0.5d);
            TrainStationBrush = FreezedDrawingObjects.GetSolidBrush(config.Colors.RailwayDash);
            MetroStationBrush = FreezedDrawingObjects.GetSolidBrush(config.Colors.MetrowayDash);
            TrainStationBorderPen = rcon.CurrentMapStyle.StationBorder.ToPen(rcon.CurrentMapStyle.Colors.Metroway, rcon.RenderScale);
            MetroStationBorderPen = rcon.CurrentMapStyle.StationBorder.ToPen(rcon.CurrentMapStyle.Colors.Metroway, rcon.RenderScale);

            CableCarPen = new Pen(FreezedDrawingObjects.GetSolidBrush(config.Colors.CableCarWay), config.WayWidth.CableCarRail.CalcSize(rcon.RenderScale));
            CableCarPen.StartLineCap = PenLineCap.Round;
            CableCarPen.EndLineCap = PenLineCap.Round;
            CableCarDecorationPen = new Pen(FreezedDrawingObjects.GetSolidBrush(config.Colors.CableCarWay), config.WayWidth.CableCarDecorationWidth.CalcSize(rcon.RenderScale));

            MonorailPen = new Pen(FreezedDrawingObjects.GetSolidBrush(config.Colors.MonorailWay), config.WayWidth.MonorailRail.CalcSize(rcon.RenderScale));
            MonorailPen.StartLineCap = PenLineCap.Round;
            MonorailPen.EndLineCap = PenLineCap.Round;
            MonorailDecorationPen = new Pen(FreezedDrawingObjects.GetSolidBrush(config.Colors.MonorailWay), config.WayWidth.MonorailDecorationWidth.CalcSize(rcon.RenderScale));

            RailPen.Freeze();
            RailDashPen.Freeze();
            RailTunnelPen.Freeze();
            TramPen.Freeze();
            TramDashPen.Freeze();
            MetroPen.Freeze();
            MetroDashPen.Freeze();
            TrainStationBorderPen.Freeze();
            MetroStationBorderPen.Freeze();
            CableCarPen.Freeze();
            MonorailPen.Freeze();
        }

    }
    */

}

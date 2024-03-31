using CS2MapView.Config;
using CS2MapView.Data;
using CS2MapView.Drawing.Layer;
using CS2MapView.Theme;
using CS2MapView.Util;
using CS2MapView.Util.CS1;
using CSLMapView.XML;
using SkiaSharp;
using System.Data;
using static CS2MapView.Util.CS1.CSLMapViewCompatibility;

namespace CS2MapView.Drawing.Roads.CS1;
/// <summary>
/// 道路描画オブジェクト構築(CS1)
/// </summary>
public class CS1RoadLayerBuilder
{
    private ICS2MapViewRoot AppRoot { get; init; }
    private CSLExportXML ExportXML { get; init; }

    private CS1SegmentManager SegmentManager { get; init; }
    private BasicLayer ResultLayer { get; init; }
    private Dictionary<(WayType, float, bool), Func<float, float, SKPaintCache.StrokeKey?>> StrokeKeyFuncs { get; init; }
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="appRoot"></param>
    /// <param name="xml"></param>
    public CS1RoadLayerBuilder(ICS2MapViewRoot appRoot, CSLExportXML xml)
    {
        AppRoot = appRoot;
        ExportXML = xml;
        SegmentManager = new CS1SegmentManager(ExportXML);
        ResultLayer = new BasicLayer(AppRoot, ILayer.LayerNameRoads, CS1MapType.CS1WorldRect);
        StrokeKeyFuncs = [];

    }

    /// <summary>
    /// 構築の実行
    /// </summary>
    /// <param name="loadProgressInfo"></param>
    /// <returns></returns>
    public Task<ILayer> BuildAsync(LoadProgressInfo? loadProgressInfo)
    {
        return Task.Run<ILayer>(() =>
        {
            List<XMLSegment> tunnelList = [];
            List<XMLSegment> wayGroundList = [];
            List<XMLSegment> highwayGroundList = [];
            List<XMLSegment> beautificationGroundList = [];
            List<XMLSegment> elevatedList = [];

            foreach (var seg in ExportXML.SegmentList)
            {
                WayType wt = SegmentManager.GetWayType(seg);
                if (wt.HasBit(WayType.Road) || wt.HasBit(WayType.Beautification))
                {
                    if (CS1HiddenObjectInfo.Segments.IsHidden(seg.Name, seg.ItemClass))
                    {
                        continue;
                    }

                    if (wt.HasBit(WayType.Tunnel))
                    {
                        tunnelList.Add(seg);
                    }
                    else if (wt.HasBit(WayType.Elevated))
                    {
                        elevatedList.Add(seg);
                    }
                    else
                    {
                        if (wt.HasBit(WayType.Highway))
                        {
                            highwayGroundList.Add(seg);
                        }
                        else if (wt.HasBit(WayType.Beautification))
                        {
                            beautificationGroundList.Add(seg);
                        }
                        else
                        {
                            wayGroundList.Add(seg);
                        }
                    }

                }

            }
            List<Junction> junctionList = Junction.GetJunctions(SegmentManager, ExportXML.NodeList);


            //トンネル

            DrawWaysSequentially(tunnelList, junctionList);

            loadProgressInfo?.Progress(this, LoadProgressInfo.Process.BuildRoads, 0.2f, null);

            List<Junction> renderTarget =
            [
                .. DrawWaysAll(wayGroundList, junctionList, WayType.Road | WayType.Ground),
                .. DrawWaysAll(highwayGroundList, junctionList, WayType.Road | WayType.Highway | WayType.Ground),
                .. DrawWaysAll(beautificationGroundList, junctionList, WayType.Beautification | WayType.Ground),
            ];
            loadProgressInfo?.Progress(this, LoadProgressInfo.Process.BuildRoads, 0.8f, null);
            foreach (Junction j in renderTarget)
            {
                DrawJunctionFigure(j);
            }

            DrawWaysSequentially(elevatedList, junctionList);

            loadProgressInfo?.Progress(this, LoadProgressInfo.Process.BuildRoads, 1f, null);


            return ResultLayer;
        });
    }
    private static float GetAverageHeight(XMLSegment s) => s.Points.Select(t => t.Y).Average();




    private Func<float, float, SKPaintCache.StrokeKey?> CreateStrokeFunc(WayType wt, float meterWidth, bool isBorder)
    {
        var color = WayTypeToColor(wt, isBorder);
        meterWidth *= 0.9f;
        if (color is null)
        {
            return (_, _) => null;
        }
        if (isBorder)
        {
            float[]? dash = wt.HasBit(WayType.Tunnel) ? [0.5f, 0.5f] : null;

            return (viewScale, worldScale) => new SKPaintCache.StrokeKey(meterWidth, (SKColor)color, StrokeType.Round, dash);
        }
        else
        {
            meterWidth *= 0.8f;

            return (viewScale, worldScale) =>
            {
                return new SKPaintCache.StrokeKey(meterWidth, (SKColor)color, StrokeType.Round);
            };
        }

    }
    private Func<float, float, SKPaintCache.StrokeKey?> GetStrokeFunc(WayType wt, float meterWidth, bool isBorder)
    {
        var key = (wt, meterWidth, isBorder);
        if (StrokeKeyFuncs.TryGetValue(key, out var value))
        {
            return value;
        }
        else
        {
            StrokeKeyFuncs.Add(key, CreateStrokeFunc(wt, meterWidth, isBorder));
            return StrokeKeyFuncs[key];
        }
    }

    private void DrawWaysSequentially(List<XMLSegment> roadList, List<Junction> junctionList)
    {

        roadList.Sort((a, b) =>
        {
            double diff = GetAverageHeight(a) - GetAverageHeight(b);
            return diff < 0 ? -1 : diff == 0 ? 0 : 1;
        });

        foreach (XMLSegment seg in roadList)
        {
            SKPoint? lastPoint = null;
            WayType wt = SegmentManager.GetWayType(seg);

            //    geo.FillRule = FillRule.Nonzero;

            var width = SegmentManager.GetWidth(seg);
            int pointidx = 0;
            SKPath? path = null;

            foreach (XMLVector3 v in seg.Points)
            {
                SKPoint currentPoint = new(v.X, -v.Z);
                if (lastPoint != null)
                {
                    path!.LineTo(currentPoint);


                }
                else
                {
                    path = new SKPath();
                    path.MoveTo(currentPoint);
                }
                lastPoint = currentPoint;
                pointidx++;
            }
            var cmdBorder = new PathDrawCommand
            {
                Path = path,
                StrokePaintFunc = GetStrokeFunc(wt, width, true)
            };

            var cmdFill = new PathDrawCommand
            {
                Path = path,
                StrokePaintFunc = GetStrokeFunc(wt, width, false)
            };

            ResultLayer.DrawCommands.Add(cmdBorder);
            ResultLayer.DrawCommands.Add(cmdFill);


            List<Junction> renderTarget = [];
            Junction? temp = junctionList.FirstOrDefault(j => j.Id == seg.StartNodeId);
            if (temp?.AddRenderedSegment(seg) ?? false)
            {
                renderTarget.Add(temp);
            }
            temp = junctionList.FirstOrDefault(j => j.Id == seg.EndNodeId);
            if (temp?.AddRenderedSegment(seg) ?? false)
            {
                renderTarget.Add(temp);
            }

            foreach (Junction j in renderTarget)
            {
                DrawJunctionFigure(j);

            }


        }
    }


    private void DrawJunctionFigure(Junction junction)
    {

        junction.Connection.Sort((a, b) =>
        {
            WayType atype = SegmentManager.GetWayType(a).ToGroundModeOnly();
            WayType btype = SegmentManager.GetWayType(b).ToGroundModeOnly();
            int asort = (atype == WayType.Ground ? 2 : atype == WayType.Elevated ? 1 : 0);
            int bsort = (btype == WayType.Ground ? 2 : btype == WayType.Elevated ? 1 : 0);
            int result = asort - bsort;
            if (result != 0)
            {
                return result;
            }
            else
            {
                asort = (atype == WayType.Highway ? 1 : 0);
                bsort = (btype == WayType.Highway ? 1 : 0);
                return asort - bsort;
            }
        });

        foreach (XMLSegment s in junction.Connection)
        {

            XMLVector3? toPointStart = null;
            XMLVector3? toPointStart2 = null;
            XMLVector3? toPointEnd = null;
            XMLVector3? toPointEnd2 = null;
            if (s.Points[0].Equals(junction.Position))
            {
                toPointStart = s.Points[1];
                if (s.Points.Count > 2)
                {
                    toPointStart2 = s.Points[2];
                }
            }
            if (s.Points[^1].Equals(junction.Position))
            {
                toPointEnd = s.Points[^2];
                if (s.Points.Count > 2)
                {
                    toPointEnd2 = s.Points[^3];
                }
            }

            if (toPointStart == null && toPointEnd == null)
            {
                //   Log.Warn($"Junction Connection Not Found node={junction.Id}");
                return;
            }

            var color = WayTypeToColor(SegmentManager.GetWayType(s), false);



            DrawAction(toPointStart, toPointStart2);
            DrawAction(toPointEnd, toPointEnd2);

            void DrawAction(XMLVector3? p1, XMLVector3? p2)
            {
                if (p1 != null)
                {
                    float width = SegmentManager.GetWidth(s);


                    //直線
                    double distanceSqr = (junction.Position.X - p1.X) * (junction.Position.X - p1.X) + (junction.Position.Z - p1.Z) * (junction.Position.Z - p1.Z);
                    if (distanceSqr > 900)
                    {
                        double angle = AngleXZ(junction.Position, p1);
                        p1 = SlidePointXZ(junction.Position, angle, 30);
                        p2 = null;
                    }

                    SKPath path = new();
                    path.MoveTo(junction.Position.ToMapSpace());
                    path.LineTo(p1.ToMapSpace());

                    if (p2 != null)
                    {
                        distanceSqr = (p2.X - p1.X) * (p2.X - p1.X) + (p2.Z - p1.Z) * (p2.Z - p1.Z);
                        if (distanceSqr > 400)
                        {
                            double angle = AngleXZ(p1, p2);
                            p2 = SlidePointXZ(p1, angle, 20);
                        }
                        path.LineTo(p2.ToMapSpace());


                    }
                    var cmd = new PathDrawCommand
                    {
                        Path = path,
                        StrokePaintFunc = GetStrokeFunc(SegmentManager.GetWayType(s), width, false)
                    };
                    ResultLayer.DrawCommands.Add(cmd);

                }
            }
        }
    }

    private static void AddToLineDic(Dictionary<float, List<SKPath>> lineDic, float width, SKPath pf)
    {
        if (lineDic.TryGetValue(width, out var list))
        {
            list.Add(pf);
        }
        else
        {
            lineDic.Add(width, [pf]);
        }
    }

    private List<Junction> DrawWaysAll(List<XMLSegment> list, List<Junction> junctionList, WayType wt)
    {
        List<Junction> renderTarget = [];
        Dictionary<float, List<SKPath>> lineDic = [];
        foreach (XMLSegment seg in list)
        {
            SKPoint? lastPoint = null;
            SKPath path = new();

            float lastWidth = float.NaN;
            foreach (XMLVector3 v in seg.Points)
            {
                SKPoint currentPoint = v.ToMapSpace();
                if (lastPoint.HasValue)
                {
                    float width = SegmentManager.GetWidth(seg);
                    if (double.IsNaN(lastWidth) || width == lastWidth)
                    {
                        //  LineSegment line = new LineSegment(RContext.TranslateFromXML(v), true);
                        //  pf.Segments.Add(line);
                        path.LineTo(currentPoint);

                    }
                    else
                    {
                        AddToLineDic(lineDic, lastWidth, path);
                        path = new SKPath();
                        path.MoveTo(lastPoint.Value);
                        path.LineTo(currentPoint);

                    }

                    lastWidth = width;
                }
                else
                {
                    path.MoveTo(currentPoint);
                }
                lastPoint = currentPoint;
            }
            AddToLineDic(lineDic, lastWidth, path);




            Junction? temp = junctionList.FirstOrDefault(j => j.Id == seg.StartNodeId);
            if (temp?.AddRenderedSegment(seg) ?? false)
            {
                renderTarget.Add(temp);
            }
            temp = junctionList.FirstOrDefault(j => j.Id == seg.EndNodeId);
            if (temp?.AddRenderedSegment(seg) ?? false)
            {
                renderTarget.Add(temp);
            }
        }

        foreach (var t in lineDic.OrderBy(t => t.Key))
        {
            var width = t.Key;
            var pathes = t.Value;
            foreach (var p in pathes)
            {
                var dc = new PathDrawCommand { Path = p, StrokePaintFunc = GetStrokeFunc(wt, width, true) };
                ResultLayer.DrawCommands.Add(dc);
            }
        }
        foreach (var t in lineDic.OrderBy(t => t.Key))
        {
            var width = t.Key;
            var pathes = t.Value;
            foreach (var p in pathes)
            {
                var dc = new PathDrawCommand { Path = p, StrokePaintFunc = GetStrokeFunc(wt, width, false) };
                ResultLayer.DrawCommands.Add(dc);
            }
        }

        return renderTarget;
    }


    private SerializableColor? WayTypeToColor(WayType wt, bool isBorder)
    {
        var rule = AppRoot.Context.Theme.Colors!;
        if (wt.HasBit(WayType.Road))
        {
            if (wt.HasBit(WayType.Highway))
            {
                if (wt.HasBit(WayType.Tunnel))
                {
                    return isBorder ? rule.HighwayTunnelBorder : rule.HighwayTunnel;
                }
                else if (wt.HasBit(WayType.Elevated))
                {
                    return isBorder ? rule.HighwayElevatedBorder : rule.HighwayElevated;
                }
                else
                {
                    return isBorder ? rule.HighwayBorder : rule.Highway;
                }
            }
            else
            {
                if (wt.HasBit(WayType.Tunnel))
                {
                    return isBorder ? rule.RoadTunnelBorder : rule.RoadTunnel;
                }
                else if (wt.HasBit(WayType.Elevated))
                {
                    return isBorder ? rule.RoadElevatedBorder : rule.RoadElevated;
                }
                else
                {
                    return isBorder ? rule.RoadBorder : rule.Road;
                }
            }
        }
        else if (wt.HasBit(WayType.Beautification))
        {
            if (wt.HasBit(WayType.Tunnel))
            {
                return isBorder ? rule.BeautificationTunnelBorder : rule.BeautificationTunnel;
            }
            else if (wt.HasBit(WayType.Elevated))
            {
                return isBorder ? rule.BeautificationElevatedBorder : rule.BeautificationElevated;
            }
            else
            {
                return isBorder ? rule.BeautificationBorder : rule.Beautification;
            }
        }
        else
        {
            return null;
        }

    }
}

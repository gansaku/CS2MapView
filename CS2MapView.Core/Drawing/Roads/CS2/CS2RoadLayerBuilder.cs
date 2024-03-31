using CS2MapView.Data;
using CS2MapView.Drawing.Layer;
using CS2MapView.Serialization;
using CS2MapView.Theme;
using CS2MapView.Util;
using CS2MapView.Util.CS2;
using Gfw.Common;
using SkiaSharp;
using System.Linq;

namespace CS2MapView.Drawing.Roads.CS2;

public class CS2RoadLayerBuilder
{
    private ICS2MapViewRoot AppRoot { get; init; }
    private CS2MapDataSet ImportData { get; init; }
    private BasicLayer ResultLayer { get; init; }

    public CS2RoadLayerBuilder(ICS2MapViewRoot appRoot, CS2MapDataSet importData)
    {
        AppRoot = appRoot;
        ImportData = importData;
        ResultLayer = new BasicLayer(AppRoot, ILayer.LayerNameRoads, CS2MapType.CS2WorldRect);
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
            if (ImportData.RoadInfo?.RoadSegments is null || ImportData.RoadInfo?.RoadNodes is null) { return ResultLayer; }

            var onGroundSegments = ImportData.RoadInfo.RoadSegments.Where(t => t.Elevation.X + t.Elevation.Y == 0);
            var tunnnelSegments = ImportData.RoadInfo.RoadSegments.Where(t => t.Elevation.X + t.Elevation.Y < 0);
            var elevatedSegments = ImportData.RoadInfo.RoadSegments.Where(t => t.Elevation.X + t.Elevation.Y > 0);

            var prefabMap = (ImportData.RoadInfo.RoadPrefabs ?? []).ToDictionary(t => t.Entity);

            var onGroundGroupedSegments = CS2NetSegmentSupport.SimplifySegments(
                onGroundSegments,
                ImportData.RoadInfo.RoadNodes,
                (a, b) => prefabMap[a.Prefab].DefaultWidth == prefabMap[b.Prefab].DefaultWidth,
                out var deletedNode2);

            var colorRule = AppRoot.Context.Theme.Colors!;
            var strokeRule = AppRoot.Context.Theme.Strokes!;

            var tunnelPathes = new List<IDrawCommand>();

            var segmentsDic = ImportData.RoadInfo.RoadSegments.ToDictionary(t => t.Entity, t => t);
            //必要なnodeを抽出
            var targetSegments = onGroundGroupedSegments.Select(
                t => new { StartSegment = t.First().Entity, t.First().StartNode, EndSegment = t.Last().Entity, t.Last().EndNode, Elevation = 0f }).Concat(
                tunnnelSegments.Select(t => new { StartSegment = t.Entity, t.StartNode, EndSegment = t.Entity, t.EndNode, Elevation = t.Elevation.X + t.Elevation.Y })).Concat(
                elevatedSegments.Select(t => new { StartSegment = t.Entity, t.StartNode, EndSegment = t.Entity, t.EndNode, Elevation = t.Elevation.X + t.Elevation.Y })
                );
            var nodeConnectionAll = targetSegments.Select(t => t.StartNode).Union(targetSegments.Select(t => t.EndNode))
                .ToDictionary(t => t, t => new HashSet<int>());

            foreach (var seg in targetSegments)
            {
                nodeConnectionAll[seg.StartNode].Add(seg.StartSegment);
                nodeConnectionAll[seg.EndNode].Add(seg.EndSegment);
            }
            Dictionary<int, HashSet<int>> nodeConnectionDrawn = nodeConnectionAll.Select(t => t.Key).ToDictionary(t => t, t => new HashSet<int>());

            //セグメント描画後、ノードのすべてのセグメントが描画されたらノードを描画
            void AddNodePathes(int startSeg, int startNode, int endSeg, int endNode, SKColor color)
            {
                IEnumerable<CS2RoadNode> AddNode(int startSegment, int startNode, int endSegment, int endNode)
                {

                    var sn = ImportData.RoadInfo.RoadNodes.FirstOrDefault(t => t.Entity == startNode);
                    var en = ImportData.RoadInfo.RoadNodes.FirstOrDefault(t => t.Entity == endNode);
                    if (sn is not null)
                    {
                        nodeConnectionDrawn[startNode].Add(startSegment);
                    }
                    if (en is not null)
                    {
                        nodeConnectionDrawn[endNode].Add(endSegment);
                    }
                    if (nodeConnectionAll[startNode].Count == nodeConnectionDrawn[startNode].Count)
                    {
                        yield return sn!;
                    }
                    if (nodeConnectionAll[endNode].Count == nodeConnectionDrawn[endNode].Count)
                    {
                        yield return en!;
                    }
                }
                foreach (var drawnode in AddNode(startSeg, startNode, endSeg, endNode))
                {

                    foreach (var segno in nodeConnectionAll[drawnode.Entity])
                    {
                        if (segno == startSeg || segno == endSeg || !segmentsDic.TryGetValue(segno, out var seg))
                        {
                            continue;
                        }
                        using var tempPath = new SKPath();
                        if (seg.StartNode == drawnode.Entity)
                        {
                            tempPath.MoveAndCubicTo(seg.Curve);
                        }
                        else
                        {
                            tempPath.MoveAndCubicTo(seg.Curve.Reverse());
                        }
                        using var tempPath2 = tempPath.CloneWithTransform(t => t);

                        int max = Math.Clamp(16, 0, tempPath2.PointCount);

                        var nodesegpath = new SKPath();
                        nodesegpath.MoveTo(drawnode.Position.ToMapSpace());
                        for (int i = 1; i < max; i++)
                        {
                            nodesegpath.LineTo(tempPath2.Points[i]);
                        }
                        var prefab = prefabMap[seg.Prefab];
                        var segColor = (seg.Elevation.X + seg.Elevation.Y) switch
                        {
                            > 0 => prefab.IsHighway ? colorRule.HighwayElevated : colorRule.RoadElevated,
                            < 0 => prefab.IsHighway ? colorRule.HighwayTunnel : colorRule.RoadTunnel,
                            _ => prefab.IsHighway ? colorRule.Highway : colorRule.Road
                        };
                        var width = seg.Elevation.X + seg.Elevation.Y > 0 ? prefab.ElevatedWidth : prefab.DefaultWidth;
                        var pc4 = new PathDrawCommand { Path = nodesegpath, StrokePaintFunc = (a, b) => new SKPaintCache.StrokeKey(width - 4, segColor, Theme.StrokeType.Round) };
                        tunnelPathes.Add(pc4);
                    }


                }
            }

            foreach (var tunnelSeg in tunnnelSegments.OrderBy(t => t.Elevation.X + t.Elevation.Y))
            {
                var prefab = prefabMap[tunnelSeg.Prefab];
                var color = prefab.IsHighway ? colorRule.HighwayTunnelBorder : colorRule.RoadTunnelBorder;
                var color2 = prefab.IsHighway ? colorRule.HighwayTunnel : colorRule.RoadTunnel;

                var width = prefab.DefaultWidth;
                var path = CS2NetSegmentSupport.GetSegmentsPath(tunnelSeg);
                var pc = new PathDrawCommand { Path = path, StrokePaintFunc = (sc, wsc) => new SKPaintCache.StrokeKey(width, color, Theme.StrokeType.Round) };
                tunnelPathes.Add(pc);
                var pc2 = new PathDrawCommand { Path = path, StrokePaintFunc = (sc, wsc) => new SKPaintCache.StrokeKey(width - 4, color2, Theme.StrokeType.Round) };
                tunnelPathes.Add(pc2);
                //ノードのごまかし
                AddNodePathes(tunnelSeg.Entity, tunnelSeg.StartNode, tunnelSeg.Entity, tunnelSeg.EndNode, color2);

            }
            foreach (var onGroundSeg in onGroundGroupedSegments)
            {
                var prefab = prefabMap[onGroundSeg.First().Prefab];
                int count = onGroundSeg.Count;
                var path = CS2NetSegmentSupport.GetGroupdSegmentsPath(onGroundSeg);
                var color = prefab.IsHighway ? colorRule.HighwayBorder : colorRule.RoadBorder;
                var color2 = prefab.IsHighway ? colorRule.Highway : colorRule.Road;
                var width = prefab.DefaultWidth;

                var pc = new PathDrawCommand { Path = path, StrokePaintFunc = (sc, wsc) => new SKPaintCache.StrokeKey(width, color, Theme.StrokeType.Round) };
                tunnelPathes.Add(pc);
                var pc2 = new PathDrawCommand { Path = path, StrokePaintFunc = (sc, wsc) => new SKPaintCache.StrokeKey(width - 4, color2, Theme.StrokeType.Round) };
                tunnelPathes.Add(pc2);
                //ノードのごまかし
                AddNodePathes(onGroundSeg.First().Entity, onGroundSeg.First().StartNode, onGroundSeg.Last().Entity, onGroundSeg.Last().EndNode, color2);
            }
            foreach (var elevatedSeg in elevatedSegments.OrderBy(t => t.Elevation.X + t.Elevation.Y))
            {
                var prefab = prefabMap[elevatedSeg.Prefab];
                var color = prefab.IsHighway ? colorRule.HighwayElevatedBorder : colorRule.RoadElevatedBorder;
                var color2 = prefab.IsHighway ? colorRule.HighwayElevated : colorRule.RoadElevated;

                var width = prefab.ElevatedWidth;
                var path = CS2NetSegmentSupport.GetSegmentsPath(elevatedSeg);
                var pc = new PathDrawCommand { Path = path, StrokePaintFunc = (sc, wsc) => new SKPaintCache.StrokeKey(width, color, Theme.StrokeType.Round) };
                tunnelPathes.Add(pc);
                var pc2 = new PathDrawCommand { Path = path, StrokePaintFunc = (sc, wsc) => new SKPaintCache.StrokeKey(width - 4, color2, Theme.StrokeType.Round) };
                tunnelPathes.Add(pc2);
                //ノードのごまかし
                AddNodePathes(elevatedSeg.Entity, elevatedSeg.StartNode, elevatedSeg.Entity, elevatedSeg.EndNode, color2);

            }

            ResultLayer.DrawCommands.AddRange(tunnelPathes);

            loadProgressInfo?.Progress(this, LoadProgressInfo.Process.BuildRoads, 1f, null);
            return ResultLayer;
        });

    }
}

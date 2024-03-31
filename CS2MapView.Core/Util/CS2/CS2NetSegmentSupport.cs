using CS2MapView.Serialization;
using Gfw.Common;
using SkiaSharp;

namespace CS2MapView.Util.CS2
{
    internal static class CS2NetSegmentSupport
    {
        internal static List<List<SegmentType>> SimplifySegments<SegmentType, NodeType>(
            IEnumerable<SegmentType> targetSegments,
            IEnumerable<NodeType> allNodes,
            Func<SegmentType, SegmentType, bool> canConnect,
            out IEnumerable<int> deletedNodes)
            where SegmentType : CS2NetSegment, new() where NodeType : CS2NetNode
        {
            List<int> deleted = [];
            var nodeConnections = allNodes.Select(t => new { t.Entity, List = new HashSet<int>() })
                .ToDictionary(t => t.Entity, t => t.List);
            var pu = new PropertyUtil<SegmentType>();
            foreach (var seg in targetSegments)
            {
                nodeConnections[seg.StartNode].Add(seg.Entity);
                nodeConnections[seg.EndNode].Add(seg.Entity);
            }
            var flatSimpleNodes = nodeConnections.Where(t => t.Value.Count == 2).ToDictionary(t => t.Key, t => t.Value);
            var segmentMap = targetSegments.ToDictionary(t => t.Entity, t => new List<SegmentType> { t });
            while (true)
            {
                if (!flatSimpleNodes.Any(t => t.Value.Count == 2 && canConnect(segmentMap[t.Value.First()].First(), segmentMap[t.Value.Last()].First())))
                {
                    break;
                }
                var node = flatSimpleNodes.First(t => t.Value.Count == 2 && canConnect(segmentMap[t.Value.First()].First(), segmentMap[t.Value.Last()].First()));

                //循環検知はCount=2から外れるので大丈夫なはず

                var seg1Id = node.Value.First();
                var seg2Id = node.Value.Last();

                if (segmentMap[seg1Id].Last().EndNode == node.Key)
                {
                    //seg2を破棄してseg1の後ろにつける
                    IList<SegmentType> seg2Data = segmentMap[seg2Id];
                    if (seg2Data.First().StartNode == node.Key)
                    {
                        var seg2LastNode = seg2Data.Last().EndNode;
                        segmentMap[seg1Id].AddRange(seg2Data);
                        segmentMap.Remove(seg2Id);
                        flatSimpleNodes.Remove(node.Key);

                        if (flatSimpleNodes.TryGetValue(seg2LastNode, out HashSet<int>? segSet))
                        {
                            segSet.Remove(seg2Id);
                            segSet.Add(seg1Id);
                        }
                    }
                    else if (segmentMap[seg2Id].Last().EndNode == node.Key)
                    {
                        seg2Data = pu.ListPropertyClone<SegmentType>(seg2Data);
                        SegmentListSwap(seg2Data);
                        var seg2LastNode = seg2Data.Last().EndNode;
                        segmentMap[seg1Id].AddRange(seg2Data);
                        segmentMap.Remove(seg2Id);
                        flatSimpleNodes.Remove(node.Key);
                        if (flatSimpleNodes.TryGetValue(seg2LastNode, out HashSet<int>? segSet))
                        {
                            segSet.Remove(seg2Id);
                            segSet.Add(seg1Id);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("SimplifySegments loop inner 1A ");
                    }

                }
                else if (segmentMap[seg1Id].First().StartNode == node.Key)
                {
                    IList<SegmentType> seg1Data = segmentMap[seg1Id];

                    if (segmentMap[seg2Id].Last().EndNode == node.Key)
                    {
                        //seg1をseg2の後ろに足す(seg1廃棄)
                        var seg1LastNode = seg1Data.Last().EndNode;
                        segmentMap[seg2Id].AddRange(segmentMap[seg1Id]);
                        segmentMap.Remove(seg1Id);
                        flatSimpleNodes.Remove(node.Key);
                        if (flatSimpleNodes.TryGetValue(seg1LastNode, out HashSet<int>? segSet))
                        {
                            segSet.Remove(seg1Id);
                            segSet.Add(seg2Id);
                        }
                    }
                    else if (segmentMap[seg2Id].First().StartNode == node.Key)
                    {
                        //seg2をひっくり返してからseg1を足す(seg1廃棄)
                        var seg2Data = pu.ListPropertyClone<SegmentType>(segmentMap[seg2Id]);
                        SegmentListSwap(seg2Data);
                        var seg1LastNode = seg1Data.Last().EndNode;
                        segmentMap[seg2Id].Clear();
                        segmentMap[seg2Id].AddRange(seg2Data);
                        segmentMap[seg2Id].AddRange(seg1Data);
                        segmentMap.Remove(seg1Id);
                        flatSimpleNodes.Remove(node.Key);
                        if (flatSimpleNodes.TryGetValue(seg1LastNode, out HashSet<int>? segSet))
                        {
                            segSet.Remove(seg1Id);
                            segSet.Add(seg2Id);
                        }

                    }
                    else
                    {
                        throw new InvalidOperationException("SimplifySegments loop inner 1B ");
                    }
                }
                else
                {
                    throw new InvalidOperationException("SimplifySegments loop outer");
                }
                deleted.Add(node.Key);
            }
            deletedNodes = deleted;
            return [.. segmentMap.Values];

            static void SegmentListSwap(IList<SegmentType> list)
            {
                static void SegmentSwap(SegmentType seg)
                {
                    (seg.StartNode, seg.EndNode) = (seg.EndNode, seg.StartNode);
                    seg.Curve = seg.Curve.Reverse();
                }
                if (list.Count == 0)
                {
                    return;
                }
                if (list.Count == 1)
                {
                    SegmentSwap(list[0]);
                    return;
                }

                var reversed = list.Reverse().ToList();
                list.Clear();

                foreach (var segment in reversed)
                {
                    SegmentSwap(segment);
                    list.Add(segment);
                }
            }
        }

        internal static SKPath GetGroupdSegmentsPath<T>(IList<T> segments) where T : CS2NetSegment
        {
            int count = segments.Count;
            var path = new SKPath();
            for (int i = 0; i < count; i++)
            {
                var c = segments[i].Curve;
                if (i == 0)
                {
                    path.MoveTo(c.X0, c.Y0);

                }
                path.CubicTo(c);
            }
            return path;
        }
        internal static SKPath GetSegmentsPath<T>(T segment) where T : CS2NetSegment
        {
            var path = new SKPath();
            var c = segment.Curve;
            path.MoveTo(c.X0, c.Y0);
            path.CubicTo(c);
            return path;
        }
        internal static void MoveAndCubicTo(this SKPath path, CS2MapSpaceBezier4 bezier)
        {
            path.MoveTo(bezier.X0, bezier.Y0);
            path.CubicTo(bezier);
        }
        internal static void CubicTo(this SKPath path, CS2MapSpaceBezier4 bezier)
        {
            path.CubicTo(bezier.X1,bezier.Y1, bezier.X2,bezier.Y2,bezier.X3, bezier.Y3);
        }
        internal static void MoveAndCubicTo(this SKPath path, SKPoint[] bezier)
        {
            path.MoveTo(bezier[0]);
            path.CubicTo(bezier);
        }
        internal static void CubicTo(this SKPath path, SKPoint[] bezier)
        {
            path.CubicTo(bezier[1], bezier[2], bezier[3]);
        }

        internal static SKPoint[] ToPointArray(this CS2MapSpaceBezier4 curve)
        {
            return [new(curve.X0, curve.Y0), new(curve.X1, curve.Y1), new(curve.X2, curve.Y2), new(curve.X3, curve.Y3)];
        }
        internal static SKPoint[] ToReversedPointArray(this CS2MapSpaceBezier4 curve)
        {
            return [new(curve.X3, curve.Y3), new(curve.X2, curve.Y2), new(curve.X1, curve.Y1), new(curve.X0, curve.Y0)];
        }

        internal static SKColor ToSKColor(this CS2Color color)=>new(color.R, color.G, color.B, color.A);
        internal static SKColor ToSKColorWithAlpha(this CS2Color color,byte alpha) => new(color.R, color.G, color.B,alpha);
    }
}

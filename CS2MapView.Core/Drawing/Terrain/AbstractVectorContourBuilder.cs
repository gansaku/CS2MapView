using CS2MapView.Data;
using CS2MapView.Util;
using log4net;
using SkiaSharp;
using System.Diagnostics;
using System.Numerics;

namespace CS2MapView.Drawing.Terrain;

internal abstract class AbstractVectorContourBuilder : ITerrainDrawingsBuiderProcedure
{
    internal static ILog Logger = LogManager.GetLogger(typeof(AbstractVectorContourBuilder));

    protected class NodeInfo
    {
        internal bool Up;
        internal bool Down;
        internal bool Left;
        internal bool Right;
        internal bool SegmentCreated;

        internal bool AnyMarked => Up | Down | Left | Right;

        internal NodeInfo()
        {
            SegmentCreated = false;
        }
    }

    internal required float TargetHeight { get; init; }
    internal required int InputWidth { get; init; }
    internal required int InputHeight { get; init; }
    internal required float[] InputArray { get; init; }
    internal required ReadonlyRect MetricRect { get; init; }
    internal required TerrainProgress Progress;
    internal required RenderContext Context { get; init; }

    protected List<IDrawCommand> ResultCommands = [];

    public IEnumerable<IDrawCommand> GetResult() => ResultCommands;

    protected virtual bool PathJoinRequired => false;

    protected abstract void SetPathCommand(IEnumerable<ContourSegments> segmentsGrouped);

    void ITerrainDrawingsBuiderProcedure.Execute()
    {
        Stopwatch sw0 = Stopwatch.StartNew();
        Stopwatch sw1 = Stopwatch.StartNew();
        var nodeInfo = CreateNodeInfo();
        sw1.Stop();
        var createNodeInfo_ellapsed = sw1.ElapsedMilliseconds;
        sw1.Reset();
        sw1.Start();
        var segments = CreateSegments(nodeInfo);
        int createdSegmentCount = segments.Count();
        sw1.Stop();
        var createSegments_ellapsed = sw1.ElapsedMilliseconds;
        sw1.Reset();
        sw1.Start();

        var mergedSegments = MergeSegements(segments);
        sw1.Stop();
        var mergeSegments_ellapsed = sw1.ElapsedMilliseconds;

        sw1.Reset();
        sw1.Start();
        var groupSegments = GroupSegments(segments);
        sw1.Stop();
        var groupSegments_ellapsed = sw1.ElapsedMilliseconds;

        sw1.Reset();
        sw1.Start();
        SetPathCommand(groupSegments);
        sw1.Stop();
        var createSkPathes_ellapsed = sw1.ElapsedMilliseconds;
        Progress.IncrementCompleted();
        sw0.Stop();
        Logger.Debug($"{GetType().Name}: height={TargetHeight} node={nodeInfo.Count} segments={createdSegmentCount} merged={mergedSegments.Count()} group={groupSegments.Count} time: {createNodeInfo_ellapsed} {createSegments_ellapsed} {mergeSegments_ellapsed} {groupSegments_ellapsed} {createSkPathes_ellapsed} total {sw0.ElapsedMilliseconds}ms ");
    }

    protected abstract Dictionary<int, NodeInfo> CreateNodeInfo();

    private Vector3 GetCoordFromTerrain(float x, float y)
    {
        return new Vector3((x / InputWidth) * MetricRect.Width + MetricRect.Left,
            0,
            (y / InputHeight) * MetricRect.Height + MetricRect.Top);
    }

    private static bool GroupSegmentsFindForward(RegionalContourSegmentFinder copy, List<RegionalLookupNode<ContourSegment>> tempList)
    {
        var first = tempList.First();
        var current = tempList.Last();

        RegionalLookupNode<ContourSegment>? target;
        while ((target = copy.FindNextSegment(current.Data)) != null)
        {
            tempList.Add(target);
            copy.Remove(target);
            current = target;
        }
        return first.Data.Start == current.Data.End;

    }
    private static bool GroupSegmentsFindBackward(RegionalContourSegmentFinder copy, List<RegionalLookupNode<ContourSegment>> tempList)
    {
        var last = tempList.Last();
        var current = tempList.First();
        RegionalLookupNode<ContourSegment>? target;
        while ((target = copy.FindPrevSegment(current.Data)) != null)
        {
            tempList.Insert(0, target);
            copy.Remove(target);
            current = target;

        }
        return last.Data.End == current.Data.Start;
    }
    protected List<ContourSegments> GroupSegments(IEnumerable<ContourSegment> segments)
    {
        var finder = new RegionalContourSegmentFinder(new List<ContourSegment>(segments), MetricRect);
        var result = new List<ContourSegments>();

        var seg = finder.FirstOrDefault();

        while (seg is not null)
        {
            List<RegionalLookupNode<ContourSegment>> tempList =
            [
                seg
            ];
            finder.Remove(seg);
            bool error = false;
            if (!GroupSegmentsFindForward(finder, tempList))
            {
                error = !GroupSegmentsFindBackward(finder, tempList);
            }

            result.Add(new ContourSegments(tempList.Select(t => t.Data)) { NotClosed = error });

            seg = finder.FirstOrDefault();
        }


        return result;
    }

    private IEnumerable<ContourSegment> MergeSegements(IEnumerable<ContourSegment> segments)
    {
        var finder = new RegionalContourSegmentFinder(new List<ContourSegment>(segments), MetricRect);
        var result = new List<ContourSegment>();
        var seg = finder.FirstOrDefault();

        if (seg is not null)
        {
            finder.Remove(seg);
        }
        while (seg is not null)
        {
            double thisAngle = MathEx.AngleXZ(seg.Data.Start, seg.Data.End);

            RegionalLookupNode<ContourSegment>? next;
            while ((next = finder.FindSameAngleNextSegment(seg.Data, thisAngle)) != null)
            {
                seg.Data.End = next.Data.End;
                finder.Remove(next);
            }
            RegionalLookupNode<ContourSegment>? prev;
            while ((prev = finder.FindSameAnglePrevSegment(seg.Data, thisAngle)) != null)
            {
                seg.Data.Start = prev.Data.Start;
                finder.Remove(prev);
            }

            result.Add(seg.Data);
            seg = finder.FirstOrDefault();
            if (seg is not null)
            {
                finder.Remove(seg);
            }
        }
        return result;

    }

    private IEnumerable<ContourSegment> CreateSegments(Dictionary<int, NodeInfo> nodes)
    {
        NodeInfo? GetTodoNodeAt(int x, int y)
        {
            if (x < 0 || x >= InputWidth || y < 0 || y >= InputHeight)
            {
                return null;
            }
            if (nodes.TryGetValue(y * InputWidth + x, out var value))
            {

                return value;
            }
            else
            {
                return null;
            }
        }

        //左上→右下
        foreach (var kv in nodes)
        {
            var x = kv.Key % InputWidth;
            var y = kv.Key / InputWidth;

            var upLeft = GetTodoNodeAt(x - 1, y - 1);
            var upRight = GetTodoNodeAt(x + 1, y - 1);
            var right = GetTodoNodeAt(x + 1, y);
            var downLeft = GetTodoNodeAt(x - 1, y + 1);
            var down = GetTodoNodeAt(x, y + 1);
            var downRight = GetTodoNodeAt(x + 1, y + 1);

            bool leftDownStroke = false;
            bool rightDownStroke = false;
            if (kv.Value.Up)
            {

                if (right?.Up ?? false)
                {

                    yield return new ContourSegment
                    {
                        Start = GetCoordFromTerrain(x + 0.5f, y),
                        End = GetCoordFromTerrain(x + 1.5f, y)
                    };

                }

                if (kv.Value.Left)
                {
                    if (!(upLeft?.Right ?? false))
                    {
                        yield return new ContourSegment
                        {
                            Start = GetCoordFromTerrain(x, y + 0.5f),
                            End = GetCoordFromTerrain(x + 0.5f, y)
                        };
                    }

                }
                if (kv.Value.Right)
                {
                    if (!(upRight?.Left ?? false))
                    {
                        yield return new ContourSegment
                        {
                            Start = GetCoordFromTerrain(x + 0.5f, y),
                            End = GetCoordFromTerrain(x + 1f, y + 0.5f)
                        };
                    }

                }
            }

            if (kv.Value.Left)
            {
                if (kv.Value.Down)
                {
                    if (!(downLeft?.Up ?? false))
                    {
                        leftDownStroke = true;
                        yield return new ContourSegment
                        {
                            Start = GetCoordFromTerrain(x, y + 0.5f),
                            End = GetCoordFromTerrain(x + 0.5f, y + 1f)
                        };
                    }

                }
                if (down?.Left ?? false)
                {
                    yield return new ContourSegment
                    {
                        Start = GetCoordFromTerrain(x, y + 0.5f),
                        End = GetCoordFromTerrain(x, y + 1.5f)
                    };
                }
                if (downLeft?.Up ?? false)
                {
                    yield return new ContourSegment
                    {
                        Start = GetCoordFromTerrain(x, y + 0.5f),
                        End = GetCoordFromTerrain(x - 0.5f, y + 1f)
                    };
                }
            }
            if (kv.Value.Right)
            {
                if (down?.Right ?? false)
                {
                    yield return new ContourSegment
                    {
                        Start = GetCoordFromTerrain(x + 1f, y + 0.5f),
                        End = GetCoordFromTerrain(x + 1f, y + 1.5f)
                    };
                }
                if (downRight?.Up ?? false)
                {
                    yield return new ContourSegment
                    {
                        Start = GetCoordFromTerrain(x + 1f, y + 0.5f),
                        End = GetCoordFromTerrain(x + 1.5f, y + 1f)
                    };
                }
                if (kv.Value.Down)
                {
                    if (!(downRight?.Up ?? false))
                    {
                        rightDownStroke = true;
                        yield return new ContourSegment
                        {
                            Start = GetCoordFromTerrain(x + 1f, y + 0.5f),
                            End = GetCoordFromTerrain(x + 0.5f, y + 1f)
                        };
                    }

                }
            }
            if (kv.Value.Down)
            {
                if (right?.Down ?? false)
                {

                    yield return new ContourSegment
                    {
                        Start = GetCoordFromTerrain(x + 0.5f, y + 1f),
                        End = GetCoordFromTerrain(x + 1.5f, y + 1f)
                    };

                }
                if (downLeft?.Right ?? false)
                {
                    if (!leftDownStroke)
                    {
                        yield return new ContourSegment
                        {
                            Start = GetCoordFromTerrain(x + 0.5f, y + 1f),
                            End = GetCoordFromTerrain(x, y + 1.5f)
                        };
                    }
                }
                if (downRight?.Left ?? false)
                {
                    if (!rightDownStroke)
                    {
                        yield return new ContourSegment
                        {
                            Start = GetCoordFromTerrain(x + 0.5f, y + 1f),
                            End = GetCoordFromTerrain(x + 1f, y + 1.5f)
                        };
                    }
                }
            }
        }

        yield break;
    }


    protected static List<SKPath> CreateSKPathes(IEnumerable<ContourSegments> segments)
    {
        List<SKPath> pathes = [];
        foreach (var segs in segments)
        {
            int i = 0;
            int count = segs.Count;
            if (count == 0) continue;

            SKPath path = new() { FillType = SKPathFillType.Winding };

            if (count == 1)
            {
                var seg = segs.First();
                path.MoveTo(seg.Start.X, seg.Start.Z);
                path.LineTo(seg.End.X, seg.End.Z);
                path.Close();
            }
            else
            {
                foreach (var seg in segs)
                {
                    if (i == 0)
                    {
                        path.MoveTo(seg.Start.X, seg.Start.Z);
                        path.LineTo(seg.End.X, seg.End.Z);
                    }
                    else
                    {
                        if (i == count - 1)
                        {
                            path.LineTo(seg.End.X, seg.End.Z);
                            if (!segs.NotClosed)
                            {
                                path.Close();
                            }
                        }
                        else
                        {
                            path.LineTo(seg.End.X, seg.End.Z);
                        }
                    }
                    i++;
                }
            }

            path.Transform(SKMatrix.CreateScale(1, -1));
            pathes.Add(path);
        }
        return pathes;

    }

    protected static List<SKPath> CreateSKPathesMerged(IEnumerable<ContourSegments> segments)
    {

        List<SKPath> pathes = [];
        SKPath path = new() { FillType = SKPathFillType.EvenOdd };

        foreach (var segs in segments)
        {
            int i = 0;
            int count = segs.Count;
            if (count == 0) continue;


            if (count == 1)
            {
                var seg = segs.First();
                path.MoveTo(seg.Start.X, seg.Start.Z);
                path.LineTo(seg.End.X, seg.End.Z);
                path.Close();
            }
            else
            {
                foreach (var seg in segs)
                {
                    if (i == 0)
                    {
                        path.MoveTo(seg.Start.X, seg.Start.Z);
                        path.LineTo(seg.End.X, seg.End.Z);
                    }
                    else
                    {
                        if (i == count - 1)
                        {
                            path.LineTo(seg.End.X, seg.End.Z);
                            if (!segs.NotClosed)
                            {
                                path.Close();
                            }
                        }
                        else
                        {
                            path.LineTo(seg.End.X, seg.End.Z);
                        }
                    }
                    i++;
                }
            }
        }
        path.Transform(SKMatrix.CreateScale(1, -1));
        pathes.Add(path);
        return pathes;

    }


}

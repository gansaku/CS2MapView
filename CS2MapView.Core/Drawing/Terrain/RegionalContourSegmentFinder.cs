using CS2MapView.Util;
using SkiaSharp;

namespace CS2MapView.Drawing.Terrain
{
    internal class RegionalContourSegmentFinder : RegionalLookup<ContourSegment>
    {
        private static SKRect GetBindingInflatedRect(ContourSegment cs)
        {
            var rect = new SKRect(
                        Math.Min(cs.Start.X, cs.End.X),
                        Math.Min(cs.Start.Z, cs.End.Z),
                        Math.Max(cs.Start.X, cs.End.X),
                        Math.Max(cs.Start.Z, cs.End.Z));
            return InfratedBounds(rect);
        }

        internal RegionalContourSegmentFinder(List<ContourSegment> list, ReadonlyRect worldRect)
            : base(list, worldRect, GetBindingInflatedRect)
        { }

        internal IEnumerable<RegionalLookupNode<ContourSegment>> EnumerateNextSegment(ContourSegment cs)
        {
            return Blocks[GetIndex(cs.End)].Where(t => cs.End == t.Data.Start || cs.End == t.Data.End);
        }
        internal IEnumerable<RegionalLookupNode<ContourSegment>> EnumeratePrevSegment(ContourSegment cs)
        {
            return Blocks[GetIndex(cs.Start)].Where(t => cs.Start == t.Data.Start || cs.Start == t.Data.End);
        }

        internal RegionalLookupNode<ContourSegment>? FindNextSegment(ContourSegment cs)
        {
            foreach (var node in EnumerateNextSegment(cs))
            {
                if (cs.End == node.Data.End)
                {
                    node.Data.Swap();
                }
                return node;
            }
            return null;
        }
        internal RegionalLookupNode<ContourSegment>? FindPrevSegment(ContourSegment cs)
        {
            foreach (var node in EnumeratePrevSegment(cs))
            {
                if (cs.Start == node.Data.Start)
                {
                    node.Data.Swap();
                }
                return node;
            }
            return null;
        }

        internal RegionalLookupNode<ContourSegment>? FindSameAngleNextSegment(ContourSegment cs, double angle)
        {
            foreach (var node in EnumerateNextSegment(cs))
            {
                double nodeAngle;
                if (cs.End == node.Data.End)
                {
                    nodeAngle = MathEx.AngleXZ(node.Data.End, node.Data.Start);
                }
                else
                {
                    nodeAngle = MathEx.AngleXZ(node.Data.Start, node.Data.End);
                }
                if (nodeAngle == angle)
                {
                    if (cs.End == node.Data.End)
                    {
                        node.Data.Swap();
                    }
                    return node;
                }
            }
            return null;
        }
        internal RegionalLookupNode<ContourSegment>? FindSameAnglePrevSegment(ContourSegment cs, double angle)
        {

            foreach (var node in EnumeratePrevSegment(cs))
            {
                double nodeAngle;
                if (cs.Start == node.Data.Start)
                {
                    nodeAngle = MathEx.AngleXZ(node.Data.End, node.Data.Start);
                }
                else
                {
                    nodeAngle = MathEx.AngleXZ(node.Data.Start, node.Data.End);
                }


                if (nodeAngle == angle)
                {
                    if (cs.Start == node.Data.Start)
                    {
                        node.Data.Swap();
                    }
                    return node;
                }
            }
            return null;
        }


    }
}

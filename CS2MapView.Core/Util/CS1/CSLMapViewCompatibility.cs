using CSLMapView.XML;
using SkiaSharp;
using System.Numerics;
using System.Text;

namespace CS2MapView.Util.CS1
{
    internal static class CSLMapViewCompatibility
    {
        internal static Vector3 ToVector3(this XMLVector3 inVec) => new(inVec.X, inVec.Y, inVec.Z);

        internal static SKPoint ToMapSpace(this XMLVector3 inVec) => new(inVec.X, -inVec.Z);

        internal static XMLVector3 SlidePointXZ(XMLVector3 orig, double angle, double length)
        {
            return new XMLVector3((float)(orig.X + Math.Cos(angle) * length), 0, (float)(orig.Z + Math.Sin(angle) * length));
        }
        internal static double DistanceXZSqr(XMLVector3 a, XMLVector3 b)
        {
            return (a.X - b.X) * (a.X - b.X) + (a.Z - b.Z) * (a.Z - b.Z);
        }
        internal static double AngleXZ(XMLVector3 a, XMLVector3 b)
        {
            return Math.Atan2(b.Z - a.Z, b.X - a.X);
        }
        internal static XMLVector3 Center(this IEnumerable<XMLVector3> inVec)
        {
            return new XMLVector3(inVec.Average(t => t.X), inVec.Average(t => t.Y), inVec.Average(t => t.Z));
        }

        internal static string ToDetailedString(this XMLSegment me)
        {
            XMLVector3 start = me.Points.First();
            XMLVector3 end = me.Points.Last();
            StringBuilder sb = new();
            sb.Append($"S[{me.StartNodeId}](");
            if (me.StartConnections != null)
            {
                foreach (XMLSegment s in me.StartConnections)
                {
                    sb.Append(s.Id);
                    sb.Append(',');
                }
            }
            sb.Append($"),E[{me.EndNodeId}](");
            if (me.EndConnections != null)
            {
                foreach (XMLSegment s in me.EndConnections)
                {
                    sb.Append(s.Id);
                    sb.Append(',');
                }
            }
            sb.Append(')');
            return $"Segment[{me.Id}]({start.X},{start.Z})-({end.X},{end.Z}) {{{me.Name} ([Deprecated]{me.ClassicalWayType})}} Con={sb}}}";
        }



        internal static void SwapStartEnd(this XMLSegment me)
        {
            int tempNodeId = me.StartNodeId;
            XMLNode tempNode = me.StartNode;
            List<XMLSegment> tempConnections = me.StartConnections;

            me.StartNodeId = me.EndNodeId;
            me.StartNode = me.EndNode;
            me.StartConnections = me.EndConnections;

            me.EndNodeId = tempNodeId;
            me.EndNode = tempNode;
            me.EndConnections = tempConnections;

            me.Points.Reverse();
        }




        internal static void RemoveSegmentFromNode(this XMLSegment s)
        {

            if (s.StartNode?.Segments?.Contains(s) ?? false)
            {
                s.StartNode.Segments.Remove(s);
            }
            if (s.EndNode?.Segments?.Contains(s) ?? false)
            {
                s.EndNode.Segments.Remove(s);
            }
        }
        internal static void RemoveAllConnectionSegment(this XMLSegment seg)
        {
            if (seg.StartConnections != null)
            {
                foreach (XMLSegment target in seg.StartConnections)
                {
                    if (target.StartConnections?.Contains(seg) ?? false)
                    {
                        target.StartConnections.Remove(seg);
                    }
                    if (target.EndConnections?.Contains(seg) ?? false)
                    {
                        target.EndConnections.Remove(seg);
                    }
                }
                seg.StartConnections.Clear();
            }
            if (seg.EndConnections != null)
            {
                foreach (XMLSegment target in seg.EndConnections)
                {
                    if (target.StartConnections?.Contains(seg) ?? false)
                    {
                        target.StartConnections.Remove(seg);
                    }
                    if (target.EndConnections?.Contains(seg) ?? false)
                    {
                        target.EndConnections.Remove(seg);
                    }
                }
                seg.EndConnections.Clear();
            }
        }

        internal static void AddConnectionSegment(this XMLSegment seg, XMLSegment add)
        {
            if (seg == null || add == null || seg == add)
            {
                return;
            }
            seg.StartConnections ??= [];
            seg.EndConnections ??= [];
            XMLVector3 segStart = seg.Points[0];
            if (segStart.Equals(add.Points[0]) || segStart.Equals(add.Points.Last()))
            {
                if (!seg.StartConnections.Contains(add))
                {
                    seg.StartConnections.Add(add);
                }
            }
            XMLVector3 segEnd = seg.Points.Last();
            if (segEnd.Equals(add.Points[0]) || segEnd.Equals(add.Points.Last()))
            {
                if (!seg.EndConnections.Contains(add))
                {
                    seg.EndConnections.Add(add);
                }
            }

            add.StartConnections ??= [];
            add.EndConnections ??= [];

            segStart = add.Points[0];
            if (segStart.Equals(seg.Points[0]) || segStart.Equals(seg.Points.Last()))
            {
                if (!add.StartConnections.Contains(seg))
                {
                    add.StartConnections.Add(seg);
                }
            }
            segEnd = add.Points.Last();
            if (segEnd.Equals(seg.Points[0]) || segEnd.Equals(seg.Points.Last()))
            {
                if (!add.EndConnections.Contains(seg))
                {
                    add.EndConnections.Add(seg);
                }
            }
        }
        internal static string ToDetailedString(this XMLNode n)
        {
            StringBuilder sb = new();
            sb.Append("Segments(");
            if (n.Segments != null)
            {

                foreach (XMLSegment s in n.Segments)
                {
                    sb.Append(s.Id);
                    sb.Append(',');
                }
            }
            sb.Append(')');
            return $"Node[{n.Id}]({n.Position.X},{n.Position.Z}){{{n.SubService}}} {sb}";
        }
        /// <summary>
        /// ノードに接続するセグメントを追加します。このメソッドはインポート時に使用されます。
        /// </summary>
        /// <param name="node"></param>
        /// <param name="s"></param>
        internal static void AttachSegmentToNode(this XMLNode node, XMLSegment s)
        {
            node.Segments ??= [];
            if (!node.Segments.Contains(s))
            {
                node.Segments.Add(s);
            }
        }


    }
}

using CS2MapView.Util;
using CSLMapView.XML;

namespace CS2MapView.Drawing.Roads.CS1
{
    internal class Junction
    {
        public int Id { get; private set; }

        internal List<XMLSegment> Connection { get; } = [];
        internal List<XMLSegment> Drawed { get; } = [];
        internal required XMLVector3 Position { get; set; }
        internal Junction(int id, XMLVector3 position, IEnumerable<XMLSegment> connection)
        {
            Id = id;
            Position = position;
            Connection.AddRange(connection);
        }
        internal bool AddRenderedSegment(XMLSegment s)
        {
            if (!Drawed.Contains(s))
            {
                Drawed.Add(s);
            }

            if (Drawed.Count >= Connection.Count)
            {
                return true;
            }
            return false;
        }
        internal Junction(int id)
        {
            Id = id;
        }


        internal static List<Junction> GetJunctions(CS1SegmentManager segMgr, List<XMLNode> nodeList)
        {
            List<Junction> result = [];
            foreach (XMLNode n in nodeList)
            {
                if (n.Service == "Road" || n.Service == "Beautification" || n.SubService == "PublicTransportPlane")
                {

                    int connects = 0;
                    uint flg = 0;
                    Junction j = new(n.Id) { Position = n.Position };

                    int highwayCount = 0;
                    if (n.Segments != null)
                    {
                        foreach (XMLSegment s in n.Segments)
                        {
                            WayType wayType = segMgr.GetWayType(s);
                            if (wayType.HasBit(WayType.Road) || wayType.HasBit(WayType.Beautification))
                            {
                                if (wayType.HasBit(WayType.Tunnel))
                                {
                                    flg |= 1;
                                }
                                else if (wayType.HasBit(WayType.Ground))
                                {
                                    flg |= 2;
                                }
                                else if (wayType.HasBit(WayType.Elevated))
                                {
                                    flg |= 4;
                                }
                                if (wayType.HasBit(WayType.Highway))
                                {
                                    highwayCount++;
                                }

                                j.Connection.Add(s);
                                connects++;
                            }
                        }
                    }
                    if (flg == 0)
                    {
                        continue;
                    }
                    else if (flg == 1)
                    {
                        if (connects > 1)
                        {
                            result.Add(j);
                        }
                    }
                    else if (flg == 2)
                    {
                        if (highwayCount != 0 && highwayCount != connects)
                        {
                            result.Add(j);
                        }
                    }
                    else if (flg == 3)
                    {
                        result.Add(j);
                    }
                    else
                    {
                        if (connects > 1 || flg != 4)
                        {
                            result.Add(j);
                        }
                    }
                }
            }
            return result;
        }
    }
}

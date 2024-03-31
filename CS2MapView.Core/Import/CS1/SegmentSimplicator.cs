using CS2MapView.Util;
using CS2MapView.Util.CS1;
using CSLMapView.XML;

namespace CS2MapView.Import.CS1
{
    internal static class SegmentSimplicator
    {
        internal static void SimplifySegments(CSLExportXML xml, bool invert)
        {
            CS1SegmentManager mgr = new(xml);

            //一本道を同じセグメントにまとめる（地下・高架の道路、および道路幅の違うものを除く）
            if (invert)
            {
                xml.SegmentList.Reverse();
            }
            List<List<XMLSegment>> grouped = SimplifySegmentsImpl.GroupDirectSegment(mgr, xml.SegmentList);
            foreach (List<XMLSegment> group in grouped)
            {
                XMLSegment seg0 = group.First();
                WayType seg0waytype = mgr.GetWayType(seg0);
                if (seg0waytype.HasFlag(WayType.Road) && !seg0waytype.HasFlag(WayType.Ground))
                {
                    continue;
                }
                //  RoadWidthType seg0Width = RoadWidthManager.GetWidthType( seg0 );

                for (int i = 1; i < group.Count; i++)
                {
                    XMLSegment current = group[i];

                    seg0.Points.AddRange(current.Points.GetRange(1, current.Points.Count - 1));
                    //     if( RoadWidthManager.GetWidthType( current ) != seg0Width ) {
                    //         Log.Warn( $"segment[{current.Id}]'s width = {rcon.GetRoadWidth( current )} but seg0's width={seg0Width}" );
                    //     }
                    if (i == group.Count - 1)
                    {
                        seg0.EndNodeId = current.EndNodeId;
                        seg0.EndNode = current.EndNode;
                        current.EndNode.AttachSegmentToNode(seg0);
                        foreach (XMLSegment ss in current.EndConnections)
                        {
                            seg0.AddConnectionSegment(ss);
                        }

                    }
                    current.RemoveSegmentFromNode();
                    current.RemoveAllConnectionSegment();
                    xml.SegmentList.Remove(current);
                }
            }
            List<XMLNode> toDelete = [];
            foreach (XMLNode n in xml.NodeList)
            {
                if ((n.Segments == null || n.Segments.Count == 0) && (n.Service == "Road" || n.Service == "Water" || n.Service == "Electricity"))
                {
                    toDelete.Add(n);
                }
            }
            foreach (XMLNode n in toDelete)
            {

                xml.NodeList.Remove(n);
            }
            //セグメント内の直線をまとめる
            int ignoredPoint = 0;
            foreach (XMLSegment seg in xml.SegmentList)
            {
                if (seg.Points.Count < 3)
                {
                    continue;
                }

                List<XMLVector3> result = [];
                XMLVector3? oldV = null;
                double lastAngle = double.NaN;

                for (int i = 0; i < seg.Points.Count; i++)
                {
                    XMLVector3 v = seg.Points[i];

                    if (oldV != null)
                    {
                        double angle = CSLMapViewCompatibility.AngleXZ(oldV, v);//.Atan2( v.Z - oldV.Z, v.X - oldV.X );
                        if (!double.IsNaN(lastAngle))
                        {
                            if (Math.Abs(angle - lastAngle) > 0.05d * (Math.PI / 180d))
                            {
                                result.Add(seg.Points[i - 1]);
                            }
                            else
                            {
                                ignoredPoint++;
                            }
                        }
                        lastAngle = angle;
                        if (i == seg.Points.Count - 1)
                        {
                            result.Add(v);
                        }
                    }
                    else
                    {
                        result.Add(v);
                    }

                    oldV = v;
                }
                seg.Points = result;
            }
            //    Log.Debug($"ignored points={ignoredPoint}");
        }

        private static class SimplifySegmentsImpl
        {

            /// <summary>
            /// 一本道をグループ化して、方向もそろえる
            /// </summary>
            /// <param name="segMgr"></param>
            /// <param name="roadList"></param>
            /// <returns></returns>
            internal static List<List<XMLSegment>> GroupDirectSegment(CS1SegmentManager segMgr, List<XMLSegment> roadList)
            {
                List<XMLSegment> tempList = new(roadList);
                List<List<XMLSegment>> result = [];

                while (tempList.Count > 0)
                {
                    XMLSegment seg = tempList[0];
                    if (segMgr.GetWayType(seg).HasBit(WayType.Tunnel))
                    {
                        seg.ForceTunnel = true;
                    }

                    tempList.Remove(seg);
                    List<XMLSegment> nextList = FindNextRoads(segMgr, seg, tempList);
                    result.Add(nextList);
                }

                return result;
            }
            private static List<XMLSegment> FindNextRoads(CS1SegmentManager segMgr, XMLSegment seg, List<XMLSegment> searchTargetList)
            {

                List<XMLSegment> result = [seg];
                if ((seg.StartConnections?.Count ?? 0) == 0 && (seg.EndConnections?.Count ?? 0) == 0)
                {
                    return result;
                }

                List<XMLSegment> FilteredStartConnections = FilterSameWayVisual(segMgr, seg, seg.StartConnections);
                List<XMLSegment> FilteredEndConnections = FilterSameWayVisual(segMgr, seg, seg.EndConnections);

                //接続ノードが1つだけで、しかも連結対象であれば
                if ((seg.StartConnections?.Count ?? 0) == 1 && FilteredStartConnections.Count == 1)
                {
                    XMLSegment con = FilteredStartConnections[0];

                    if (searchTargetList.Contains(con))
                    {
                        searchTargetList.Remove(con);

                        if (!con.Points.Last().Equals(seg.Points[0]))
                        {
                            con.SwapStartEnd();
                        }
                        List<XMLSegment> nextList = FindNextRoads(segMgr, con, searchTargetList);
                        result.Clear();
                        result.AddRange(nextList);
                        result.Add(seg);
                    }

                }

                if ((seg.EndConnections?.Count ?? 0) == 1 && FilteredEndConnections.Count == 1)
                {
                    XMLSegment con = FilteredEndConnections[0];

                    if (searchTargetList.Contains(con))
                    {
                        searchTargetList.Remove(con);
                        if (!con.Points[0].Equals(seg.Points.Last()))
                        {
                            con.SwapStartEnd();
                        }

                        List<XMLSegment> nextList = FindNextRoads(segMgr, con, searchTargetList);

                        result.AddRange(nextList);
                    }

                }

                //Log.Write( $"segment[{seg.Id}] returns connections count={result.Count}" );
                return result;
            }
            /// <summary>
            /// 鉄道のトンネルとその他を同じ一本道扱いしないための対応
            /// </summary>
            /// <param name="segMgr"></param>
            /// <param name="me"></param>
            /// <param name="list"></param>
            /// <returns></returns>
            private static List<XMLSegment> FilterSameWayVisual(CS1SegmentManager segMgr, XMLSegment me, List<XMLSegment>? list)
            {
                WayType t = segMgr.GetWayType(me);
                WayType service = t.ToServiceOnly();
                WayType groundMode = t.ToGroundModeOnly();
                List<XMLSegment> result = [];
                if (list == null)
                {
                    return result;
                }

                foreach (XMLSegment seg in list)
                {
                    WayType targetWayType = segMgr.GetWayType(seg);
                    if (targetWayType.HasBit(WayType.Transport))
                    {
                        continue;
                    }
                    if (me.CustomName != seg.CustomName)
                    {
                        continue;
                    }
                    if (targetWayType.ToServiceOnly() == service)
                    {
                        WayType targetGroundType = targetWayType.ToGroundModeOnly();

                        if (service == WayType.Rail)
                        {
                            bool thisIsStation = me.Name.Contains("Station", StringComparison.InvariantCulture) || me.Name.Contains("Train Cargo Track", StringComparison.InvariantCulture);
                            bool thatIsStation = seg.Name.Contains("Station", StringComparison.InvariantCulture) || seg.Name.Contains("Train Cargo Track", StringComparison.InvariantCulture);
                            if (groundMode == WayType.Tunnel)
                            {
                                if (targetGroundType == WayType.Tunnel)
                                {
                                    seg.ForceTunnel = true;
                                    result.Add(seg);
                                }
                            }
                            else
                            {
                                if (targetGroundType != WayType.Tunnel)
                                {
                                    if (thisIsStation == thatIsStation)
                                    {
                                        result.Add(seg);
                                    }
                                }
                            }

                        }
                        else if (service == WayType.Tram)
                        {
                            //トラムは トラム＋道路を除く。
                            result.Add(seg);
                        }
                        else if (service == WayType.Metro || service == WayType.Monorail || service == WayType.CableCar)
                        {
                            //地下鉄・モノレール・ケーブルカーは駅以外まとめる
                            if (me.Name == seg.Name)
                            {
                                result.Add(seg);
                            }
                        }
                        else if (t == targetWayType && segMgr.GetWidth(me) == segMgr.GetWidth(seg))
                        {

                            //道路・複合セグメント
                            result.Add(seg);
                        }
                    }
                }

                return result;
            }
        }
    }
}

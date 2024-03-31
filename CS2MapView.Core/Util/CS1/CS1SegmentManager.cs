using CS2MapView.Util.CS1;
using CSLMapView.XML;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CS2MapView.Util
{
    internal partial class CS1SegmentManager
    {
        private class CalculatedSegmentType
        {
            internal WayType WayType { get; set; }
            internal float Width { get; set; }
        }

        private readonly Dictionary<string, CalculatedSegmentType> cache = [];

        internal bool IsClassicalMode { get; private set; }

        private readonly List<XMLSegmentType> segmentTypes;

        public CS1SegmentManager(CSLExportXML xml)
        {
            if (!decimal.TryParse(xml.FileVersion, CultureInfo.InvariantCulture, out decimal ver))
            {
                ver = 0.0m;
            }
            IsClassicalMode = ver < 3.61m;
            segmentTypes = xml.SegmentTypeList;
            Init();
        }

        private void Init()
        {
            if (IsClassicalMode)
            {
                return;
            }
            foreach (var t in segmentTypes)
            {
                var obj = new CalculatedSegmentType();
                WayType wt = 0u;

                float min = float.MaxValue;
                float max = float.MinValue;



                if (t.Highway)
                {
                    wt |= WayType.Highway;
                }

                bool forceNotVisible = false;

                foreach (var lane in t.Lanes)
                {
                    float tempMin = lane.Position - lane.Width / 2;
                    float tempMax = lane.Position + lane.Width / 2;
                    min = Math.Min(min, tempMin);
                    max = Math.Max(max, tempMax);

                    if (lane.Type == "Vehicle")
                    {
                        string[] vehicleTypes = lane.VehicleType.Split(',');
                        for (int i = 0; i < vehicleTypes.Length; i++)
                        {
                            string trimmedType = vehicleTypes[i].Trim();
                            switch (trimmedType)
                            {
                                case "Car":
                                    wt |= WayType.Road;
                                    break;
                                case "Metro":
                                    wt |= WayType.Metro;
                                    break;
                                case "Train":
                                    wt |= WayType.Rail;
                                    break;
                                case "Tram":
                                    wt |= WayType.Tram;
                                    break;
                                case "Monorail":
                                    wt |= WayType.Monorail;
                                    break;
                                case "CableCar":
                                    wt |= WayType.CableCar;
                                    break;
                                case "Plane":
                                    wt |= WayType.Road;
                                    break;
                                case "Blimp":
                                case "Ferry":
                                    forceNotVisible = true;
                                    break;
                                case "Bicycle":
                                    wt |= WayType.Beautification;
                                    break;
                            }
                        }

                    }
                    else if (lane.Type == "Pedestrian")
                    {
                        wt |= WayType.Beautification;
                    }

                }


                if (wt != WayType.Beautification)
                {
                    wt &= ~WayType.Beautification;
                }
                if (forceNotVisible)
                {
                    wt = WayType.None;
                }

                obj.Width = Math.Max((int)(max - min), 3);
                obj.WayType = wt;
                cache.Add(t.Name, obj);
            }
        }

        [GeneratedRegex("^(Train|Tram|Metro|CableCar|Monorail) Line$")]
        private static partial Regex ItemClassTransportRegex();

        internal float GetWidth(XMLSegment seg)
        {
            if (IsClassicalMode)
            {
                CS1ClassicalRoadWidthType widthType = CS1ClassicalRoadWidthManager.GetWidthType(seg);
                return widthType switch
                {
                    CS1ClassicalRoadWidthType.Beautification => 3f,
                    CS1ClassicalRoadWidthType.Highway => 16f,
                    CS1ClassicalRoadWidthType.HighwayRamp => 12f,
                    CS1ClassicalRoadWidthType.HighwayWide => 32f,
                    CS1ClassicalRoadWidthType.Narrow => 8f,
                    CS1ClassicalRoadWidthType.Normal => 16f,
                    CS1ClassicalRoadWidthType.Wide => 32f,
                    CS1ClassicalRoadWidthType.VeryWide => 36f,
                    _ => 0f,
                };
            }
            else
            {
                return cache[seg.Name].Width;
            }
        }
        internal WayType GetWayType(XMLSegment seg)
        {
            if (IsClassicalMode)
            {
                return seg.ClassicalWayType;
            }
            else
            {

                WayType wtNew = cache[seg.Name].WayType;
                if (seg.PathUnit != null || ItemClassTransportRegex().IsMatch(seg.ItemClass))
                {
                    wtNew |= WayType.Transport;
                }
                if (seg.StartNode.UnderGround || seg.EndNode.UnderGround || seg.ForceTunnel)
                {
                    wtNew |= WayType.Tunnel;
                }
                else if (!seg.StartNode.OnGround || !seg.EndNode.OnGround)
                {
                    wtNew |= WayType.Elevated;
                }
                else
                {
                    wtNew |= WayType.Ground;
                }
                return wtNew;
            }
        }


        #region cslmapview.SegmentGrouping
        internal List<List<XMLSegment>> GroupDirectRailway(List<XMLSegment> segmentList, WayType searchWayType)
        {
            List<XMLSegment> tempList = new(segmentList);
            List<List<XMLSegment>> result = [];

            while (tempList.Count > 0)
            {
                XMLSegment seg = tempList[0];

                tempList.Remove(seg);
                List<XMLSegment> nextList = FindNextSegments(seg, tempList, searchWayType);
                result.Add(nextList);
            }
            return result;
        }

        private List<XMLSegment> FindNextSegments(XMLSegment seg, List<XMLSegment> searchTargetList, WayType searchWayType)
        {
            List<XMLSegment> result = [seg];
            List<XMLSegment>? PrevFilter(List<XMLSegment> list)
            {
                if (list == null)
                {
                    return null;
                }
                List<XMLSegment> prevResult = [];
                foreach (XMLSegment connectionSeg in list)
                {
                    if ((GetWayType(connectionSeg) & searchWayType) != WayType.None)
                    {
                        prevResult.Add(connectionSeg);
                    }
                }
                return prevResult;
            }


            int startConnectionCount = PrevFilter(seg.StartConnections)?.Count ?? 0;
            int endConnectionCount = PrevFilter(seg.EndConnections)?.Count ?? 0;


            if (startConnectionCount == 0 && endConnectionCount == 0)
            {
                return result;
            }
            if (startConnectionCount != 1 && endConnectionCount != 1)
            {
                return result;
            }

            List<XMLSegment> FilteredStartConnections = FilterSameVisual(seg, seg.StartConnections, searchWayType);
            List<XMLSegment> FilteredEndConnections = FilterSameVisual(seg, seg.EndConnections, searchWayType);

            //接続ノードが1つだけで、しかも連結対象であれば実行。
            //以降、セグメントをひっくり返す場合があるが、本物のセグメントをいじると道路（場合によっては他の線路も）の描画に不具合を起こすため、コピーを作って実行。
            if (startConnectionCount == 1 && FilteredStartConnections.Count == 1)
            {
                XMLSegment conorig = FilteredStartConnections[0];
                XMLSegment con = conorig;
                if (searchTargetList.Contains(conorig))
                {
                    searchTargetList.Remove(conorig);

                    if (!con.Points.Last().Equals(seg.Points[0]))
                    {
                        con = new XMLSegment
                        {
                            Id = conorig.Id,
                            Name = conorig.Name,
                            ItemClass = conorig.ItemClass,
                            CustomName = conorig.CustomName,
                            EndConnections = conorig.StartConnections,
                            Points = new List<XMLVector3>(conorig.Points),
                            EndNode = conorig.StartNode,
                            EndNodeId = conorig.StartNodeId,
                            PathUnit = conorig.PathUnit,
                            StartConnections = conorig.EndConnections,
                            StartNode = conorig.EndNode,
                            StartNodeId = conorig.EndNodeId,
                            Width = conorig.Width
                        };

                        con.Points.Reverse();
                    }
                    List<XMLSegment> nextList = FindNextSegments(con, searchTargetList, searchWayType);
                    result.Clear();
                    result.AddRange(nextList);
                    result.Add(seg);
                }

            }

            if (endConnectionCount == 1 && FilteredEndConnections.Count == 1)
            {
                XMLSegment conorig = FilteredEndConnections[0];
                XMLSegment con = conorig;
                if (searchTargetList.Contains(conorig))
                {
                    searchTargetList.Remove(conorig);

                    if (!con.Points[0].Equals(seg.Points.Last()))
                    {
                        con = new XMLSegment
                        {
                            Id = conorig.Id,
                            Name = conorig.Name,
                            ItemClass = conorig.ItemClass,
                            CustomName = conorig.CustomName,
                            EndConnections = conorig.StartConnections,
                            Points = new List<XMLVector3>(conorig.Points),
                            EndNode = conorig.StartNode,
                            EndNodeId = conorig.StartNodeId,
                            PathUnit = conorig.PathUnit,
                            StartConnections = conorig.EndConnections,
                            StartNode = conorig.EndNode,
                            StartNodeId = conorig.EndNodeId,
                            Width = conorig.Width
                        };
                        con.Points.Reverse();
                    }

                    List<XMLSegment> nextList = FindNextSegments(con, searchTargetList, searchWayType);

                    result.AddRange(nextList);
                }

            }

            //Log.Write( $"segment[{seg.Id}] returns connections count={result.Count}" );
            return result;
        }

        private List<XMLSegment> FilterSameVisual(XMLSegment seg, List<XMLSegment> targetList, WayType searchWayType)
        {

            WayType thisSegWayType = GetWayType(seg);

            bool thisisTransport = thisSegWayType.HasBit(WayType.Transport);
            List<XMLSegment> result = [];
            WayType thisGroundMode = thisSegWayType.ToGroundModeOnly();
            if (thisGroundMode == WayType.Elevated)
            {
                thisGroundMode = WayType.Ground;
            }

            foreach (XMLSegment target in targetList)
            {

                WayType targetWayType = GetWayType(target);
                if (!targetWayType.HasBit(searchWayType))
                {
                    continue;
                }
                WayType targetGroundMode = targetWayType.ToGroundModeOnly();
                bool targetTransport = targetWayType.HasFlag(WayType.Transport);

                if (searchWayType == WayType.Rail)
                {

                    bool thisIsStation = seg.Name.Contains("Station", StringComparison.InvariantCulture) || seg.Name.Contains("Train Cargo Track", StringComparison.InvariantCulture);
                    bool thatIsStation = target.Name.Contains("Station", StringComparison.InvariantCulture) || target.Name.Contains("Train Cargo Track", StringComparison.InvariantCulture);

                    if (targetGroundMode == WayType.Elevated)
                    {
                        targetGroundMode = WayType.Ground;
                    }
                    if (thisIsStation == thatIsStation && thisGroundMode == targetGroundMode && thisisTransport == targetTransport)
                    {
                        result.Add(target);
                    }


                }
                else if (searchWayType == WayType.Metro || searchWayType == WayType.Monorail)
                {
                    bool thisIsStation = seg.Name.Contains("Station", StringComparison.InvariantCulture);
                    bool thatIsStation = target.Name.Contains("Station", StringComparison.InvariantCulture);

                    if (thatIsStation == thisIsStation && thisisTransport == targetTransport)
                    {
                        result.Add(target);
                    }

                }
                else if (searchWayType == WayType.CableCar)
                {
                    bool thisIsStation = seg.Name.Contains("Stop", StringComparison.InvariantCulture);
                    bool thatIsStation = target.Name.Contains("Stop", StringComparison.InvariantCulture);

                    if (thatIsStation == thisIsStation && thisisTransport == targetTransport)
                    {
                        result.Add(target);
                    }

                }
                else
                {

                    if (thisisTransport == targetTransport)
                    {
                        result.Add(target);
                    }

                }
            }
            return result;
        }

        #endregion
    }


}

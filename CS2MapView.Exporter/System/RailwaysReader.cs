using CS2MapView.Serialization;
using Game.Net;
using Game.Prefabs;
using Game.UI;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using Unity.Collections;
using Unity.Entities;

namespace CS2MapView.Exporter.System
{

    internal class RailwaysReader
    {

        private SystemRefs SystemRefs { get; set; }

        private NameSystem NameSystem { get; set; }
        private PrefabSystem PrefabSystem { get; set; }

        internal RailwaysReader(SystemRefs systemRefs)
        {
            SystemRefs = systemRefs;

            NameSystem = SystemRefs.GetOrCreateSystemManaged<NameSystem>();
            PrefabSystem = SystemRefs.GetOrCreateSystemManaged<PrefabSystem>();
        }
        internal void ReadRails(ZipArchive zip)
        {
            CS2RailsData result = new CS2RailsData
            {
                RailSegments = new List<CS2RailSegment>(),
                RailNodes = new List<CS2RailNode>()
            };
            using var rails = SystemRefs.Queries.AllRails.ToEntityArray(Allocator.Persistent);

            var nodes = new HashSet<Entity>();
            CS2RailType GetRailType(Entity e)
            {
                CS2RailType railType = CS2RailType.None;

                void AddType<T>(CS2RailType flg) where T : unmanaged, IComponentData
                {
                    if (SystemRefs.HasComponent<T>(e))
                    {
                        railType |= flg;
                    }
                }
                AddType<TrainTrack>(CS2RailType.Train);
                AddType<TramTrack>(CS2RailType.Tram);
                AddType<SubwayTrack>(CS2RailType.Subway);
                return railType;
            }

            foreach (var railEntity in rails)
            {
                if (SystemRefs.TryGetComponent<Curve>(railEntity, out var ccurve))
                {

                    NameSystem.TryGetCustomName(railEntity, out var customName);
                    var seg = new CS2RailSegment
                    {
                        Entity = railEntity.Index,
                        CustomName = customName,
                        Length = ccurve.m_Length,
                        Curve = GeometryUtil.ToMapSpaceBezier4(ccurve.m_Bezier),
                        RailType = GetRailType(railEntity)

                    };

                    CS2Float2 elev = new CS2Float2();
                    if (SystemRefs.TryGetComponent<Elevation>(railEntity, out var elevation))
                    {
                        (elev.X, elev.Y) = (elevation.m_Elevation.x, elevation.m_Elevation.y);
                    }
                    seg.Elevation = elev;

                    if (SystemRefs.TryGetComponent<Edge>(railEntity, out var edge))
                    {
                        seg.StartNode = edge.m_Start.Index;
                        seg.EndNode = edge.m_End.Index;
                        nodes.Add(edge.m_Start);
                        nodes.Add(edge.m_End);
                    }

                    if(SystemRefs.TryGetBuffer<Game.Net.SubLane>(railEntity,true, out var lanes))
                    {
                        foreach(var subLaneEntity in lanes)
                        {
                            if(SystemRefs.TryGetComponent<Game.Net.TrackLane>(subLaneEntity.m_SubLane,out var trackLane))
                            {
                                if((trackLane.m_Flags & TrackLaneFlags.Station) != 0)
                                {
                                    seg.IsStation = true;
                                }
                            }
                        }
                       
                    }
                    result.RailSegments.Add(seg);

                }



            }
            foreach (var nodeEntity in nodes)
            {
                if (SystemRefs.TryGetComponent<Node>(nodeEntity, out var cnode))
                {
                    var elev = new CS2Float2();
                    if (SystemRefs.TryGetComponent<Elevation>(nodeEntity, out var elevation))
                    {
                        (elev.X, elev.Y) = (elevation.m_Elevation.x, elevation.m_Elevation.y);
                    }
                    var node = new CS2RailNode
                    {
                        Entity = nodeEntity.Index,
                        Position = GeometryUtil.ConvertForSerialize(cnode.m_Position),
                        Elevation = elev,
                        Rotation = GeometryUtil.ConvertForSerialize(cnode.m_Rotation),
                        RailType = GetRailType(nodeEntity)
                    };
                    result.RailNodes.Add(node);
                }
            }

           
            ZipDataWriter.WriteZipXmlEntry(zip, CS2MapDataZipEntryKeys.RailsXml, result);

        }
    }
}
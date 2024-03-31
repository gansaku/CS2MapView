using Colossal.Entities;
using CS2MapView.Serialization;
using Game.Net;
using Game.Prefabs;
using Game.Routes;
using Game.Tools;
using Game.UI;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Unity.Collections;
using Unity.Entities;
using static Game.Prefabs.TriggerPrefabData;
using static Game.UI.NameSystem;

namespace CS2MapView.Exporter.System
{
    internal class RoadsReader
    {
        private SystemRefs SystemRefs { get; set; }

        private NameSystem NameSystem { get; set; }
        private PrefabSystem PrefabSystem { get; set; }

        internal RoadsReader(SystemRefs systemRefs)
        {
            SystemRefs = systemRefs;
            
            NameSystem = SystemRefs.GetOrCreateSystemManaged<NameSystem>();
            PrefabSystem = SystemRefs.GetOrCreateSystemManaged<PrefabSystem>();
        }

        internal void ReadRoads(ZipArchive zip)
        {
            var prefabs = new HashSet<Entity>();

           
            CS2RoadsData result = new CS2RoadsData
            {
                RoadSegments = new List<CS2RoadSegment>(),
                RoadPrefabs = new List<CS2RoadPrefab>(),
                RoadNodes = new List<CS2RoadNode>()
            };

            using var roadEntities = SystemRefs.Queries.AllRoads.ToEntityArray(Allocator.Persistent);
            foreach (var roadEntity in roadEntities)
            {
                //Roadに役立つ情報なし

                if (SystemRefs.HasComponent<Composition>(roadEntity))
                {
                    var prefabRef = SystemRefs.GetComponentData<PrefabRef>(roadEntity);
                    prefabs.Add(prefabRef.m_Prefab);

                    string? customName = null;
                    if(SystemRefs.TryGetComponent<Aggregated>(roadEntity, out var aggregated))
                    {
                        NameSystem.TryGetCustomName(aggregated.m_Aggregate, out customName);
                    }

                   
                    
                    var road = new CS2RoadSegment
                    {
                        Entity = roadEntity.Index,
                        CustomName = customName,
                        Prefab = prefabRef.m_Prefab.Index
                    };
            

                    if (SystemRefs.TryGetComponent<Curve>(roadEntity, out var ccurve))
                    {
                        road.Curve = GeometryUtil.ToMapSpaceBezier4(ccurve.m_Bezier);
                        road.Length = ccurve.m_Length;
                    }
                    CS2Float2 elev = new CS2Float2();
                    if (SystemRefs.TryGetComponent<Elevation>(roadEntity, out var elevation))
                    {
                        (elev.X, elev.Y) = (elevation.m_Elevation.x, elevation.m_Elevation.y);
                    }
                    road.Elevation = elev;

                    if (SystemRefs.TryGetComponent<Edge>(roadEntity, out var edge))
                    {
                        road.StartNode = edge.m_Start.Index;
                        road.EndNode = edge.m_End.Index;

                    }

                    CS2RoadLaneType laneType = CS2RoadLaneType.None;
                    if (SystemRefs.TryGetBuffer<Game.Net.SubLane>(roadEntity, true, out var subLanes))
                    {
                        foreach(var e in subLanes)
                        {
                            if( SystemRefs.TryGetComponent<PrefabRef>(e.m_SubLane,out var lanePrefabRef))
                            {
                                if(SystemRefs.TryGetComponent<NetLaneData>(lanePrefabRef.m_Prefab,out var netLaneData))
                                {
                                    if((netLaneData.m_Flags & LaneFlags.Road )!= 0)
                                    {
                                        laneType |= CS2RoadLaneType.Car;
                                    }
                                    if((netLaneData.m_Flags & LaneFlags.Pedestrian) != 0)
                                    {
                                        laneType |= CS2RoadLaneType.Pedestrian;
                                    }

                                }
                                if (SystemRefs.TryGetComponent<TrackLaneData>(lanePrefabRef.m_Prefab, out var trackLaneData))
                                {
                                    if( (trackLaneData.m_TrackTypes & TrackTypes.Tram) != 0)
                                    {

                                        laneType |= CS2RoadLaneType.Tram;
                                    }
                                }

                            }
                        }
                    }

                    road.LaneType = laneType;


                    result.RoadSegments.Add(road);
                }
                else if (SystemRefs.HasComponent<Node>(roadEntity))
                {
               //     var prefabRef = SystemRefs.GetComponentData<PrefabRef>(roadEntity);

              //      prefabs.Add(prefabRef.m_Prefab);

                
                    var cnode = SystemRefs.GetComponentData<Node>(roadEntity);
                    CS2Float2 elev = new CS2Float2();
                    if (SystemRefs.TryGetComponent<Elevation>(roadEntity, out var elevation))
                    {
                        (elev.X, elev.Y) = (elevation.m_Elevation.x, elevation.m_Elevation.y);
                    }
                    var node = new CS2RoadNode
                    {
                        Entity = roadEntity.Index,
                        Position = GeometryUtil.ConvertForSerialize(cnode.m_Position),
                        Rotation = GeometryUtil.ConvertForSerialize(cnode.m_Rotation),
                        Elevation = elev,
              //          Prefab = prefabRef.m_Prefab.Index
                    };
                    result.RoadNodes.Add(node);
                }
                else
                {
                    continue;
                }
            }



            foreach (var prefab in prefabs)
            {

                var roadPrefab = PrefabSystem.GetPrefab<RoadPrefab>(prefab);

                var prefabInfo = new CS2RoadPrefab
                {
                    Entity = prefab.Index,
                    Name = roadPrefab.name,
                    IsHighway = roadPrefab.m_HighwayRules
                };

                if (SystemRefs.TryGetComponent<NetGeometryData>(prefab, out var netGeoData))
                {

                    prefabInfo.DefaultWidth = netGeoData.m_DefaultWidth;
                    prefabInfo.ElevatedWidth = netGeoData.m_ElevatedWidth;
                }

                result.RoadPrefabs.Add(prefabInfo);
            }

            ZipDataWriter.WriteZipXmlEntry(zip, CS2MapDataZipEntryKeys.RoadsXml, result);
        }
    }
}

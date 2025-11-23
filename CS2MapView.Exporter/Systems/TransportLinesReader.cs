using Colossal.Serialization.Entities;
using CS2MapView.Serialization;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Routes;
using Game.UI;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Unity.Collections;
using Unity.Entities;

namespace CS2MapView.Exporter.Systems
{
    internal class TransportLinesReader
    {
        private SystemRefs SystemRefs { get; set; }
        private PrefabSystem PrefabSystem { get; set; }
        private NameSystem NameSystem { get; set; }

        internal TransportLinesReader(SystemRefs systemRefs)
        {
            SystemRefs = systemRefs;
            PrefabSystem = SystemRefs.GetOrCreateSystemManaged<PrefabSystem>();
            NameSystem = SystemRefs.GetOrCreateSystemManaged<NameSystem>();
        }

        internal void ReadRoutes(ZipArchive zip)
        {
            var result = new CS2TransportLineData
            {
                TransportLines = new List<CS2TransportLine>(),
                TransportStops = new List<CS2TransportStop>()
            };
            using var entities = SystemRefs.Queries.AllTransportLines.ToEntityArray(Allocator.Persistent);

            var debugConnectRoutes = new List<Entity>();

            var routeWaypointMap = new Dictionary<int, List<int>>();

            foreach (var entity in entities)
            {
                if (!SystemRefs.TryGetComponent<PrefabRef>(entity, out var prefabRef))
                {
                    continue;
                }
                var tlPrefab = PrefabSystem.GetPrefab<TransportLinePrefab>(prefabRef);
                if (tlPrefab is null)
                {
                    continue;
                }
                CS2TransportType type = tlPrefab.m_TransportType switch
                {
                    TransportType.Airplane => CS2TransportType.Airplane,
                    TransportType.Bus => CS2TransportType.Bus,
                    TransportType.Train => CS2TransportType.Train,
                    TransportType.Tram => CS2TransportType.Tram,
                    TransportType.Subway => CS2TransportType.Subway,
                    TransportType.Ship => CS2TransportType.Ship,
                    TransportType.Ferry => CS2TransportType.Ferry,
                    _ => CS2TransportType.None
                };
                if (type == CS2TransportType.None)
                {
                    continue;
                }

                var route = new CS2TransportLine {
                    Entity = entity.Index, 
                    TransportType = type, 
                    Segments = new List<CS2RouteSegment>(), 
                    IsCargo = tlPrefab.m_CargoTransport };

                if (SystemRefs.TryGetComponent<Game.Routes.Color>(entity, out var color))
                {
                    var co = color.m_Color;
                    route.Color = new CS2Color { A = co.a, R = co.r, G = co.g, B = co.b };
                }
                if (SystemRefs.TryGetComponent<RouteNumber>(entity, out var routeNumber))
                {
                    route.Number = routeNumber.m_Number;
                }
                route.IsCustomName = NameSystem.TryGetCustomName(entity, out _);
                route.Name = NameSystem.GetRenderedLabelName(entity);

                if (SystemRefs.TryGetBuffer<RouteSegment>(entity, true, out var routSegments))
                {
                    foreach (var rseg in routSegments)
                    {
                        var seg = new CS2RouteSegment { Curves = new List<CS2MapSpaceBezier4>() };
                        if (SystemRefs.TryGetBuffer<CurveElement>(rseg.m_Segment, true, out var curves))
                        {
                            foreach (var curve in curves)
                            {
                                seg.Curves.Add(GeometryUtil.ToMapSpaceBezier4(curve.m_Curve));
                            }
                        }
                        route.Segments.Add(seg);
                    }
                }
                if (SystemRefs.TryGetBuffer<RouteWaypoint>(entity, true, out var routewaypoints))
                {
                    routeWaypointMap.Add(entity.Index, new List<int>());
                    foreach (var waypoint in routewaypoints)
                    {
                        routeWaypointMap[entity.Index].Add(waypoint.m_Waypoint.Index);
                    }
                }

                result.TransportLines.Add(route);

            }
            //stops
            using var stopEntities = SystemRefs.Queries.AllTransportStops.ToEntityArray(Allocator.Persistent);
            foreach (var stopEntity in stopEntities)
            {
                var stop = new CS2TransportStop { Entity = stopEntity.Index, Lines = new List<int>() };
                if(SystemRefs.TryGetComponent<PrefabRef>(stopEntity,out var prefabRef))
                {
                    if (SystemRefs.TryGetComponent<TransportStopData>(prefabRef.m_Prefab,out var ts))
                    {
                        stop.IsCargo = !ts.m_PassengerTransport;
                    }
                   
                }

                if (SystemRefs.TryGetComponent<Transform>(stopEntity, out var transform))
                {
                    stop.Position = GeometryUtil.ConvertForSerialize(transform.m_Position);
                }
                if (SystemRefs.HasComponent<TrainStop>(stopEntity))
                {
                    stop.TransportType = CS2TransportType.Train;
                }
                if (SystemRefs.HasComponent<TramStop>(stopEntity))
                {
                    stop.TransportType = CS2TransportType.Tram;
                }
                if (SystemRefs.HasComponent<BusStop>(stopEntity))
                {
                    stop.TransportType = CS2TransportType.Bus;
                }
                if (SystemRefs.HasComponent<ShipStop>(stopEntity))
                {
                    stop.TransportType = CS2TransportType.Ship;
                }
                if (SystemRefs.HasComponent<FerryStop>(stopEntity))
                {
                    stop.TransportType = CS2TransportType.Ferry;
                }
                if (SystemRefs.HasComponent<SubwayStop>(stopEntity))
                {
                    stop.TransportType = CS2TransportType.Subway;
                }
                if (SystemRefs.HasComponent<AirplaneStop>(stopEntity))
                {
                    stop.TransportType = CS2TransportType.Airplane;
                }
                if (stop.TransportType == CS2TransportType.None)
                {
                    continue;
                }
                stop.IsCustomName = NameSystem.TryGetCustomName(stopEntity, out _);
                stop.Name = NameSystem.GetRenderedLabelName(stopEntity);
                result.TransportStops.Add(stop);

                if (SystemRefs.TryGetBuffer<ConnectedRoute>(stopEntity, true, out var crarray))
                {
                    foreach (var cr in crarray)
                    {
                        var lines = routeWaypointMap.Where(t => t.Value.Contains(cr.m_Waypoint.Index));
                        foreach (var l in lines)
                        {
                            if (!stop.Lines.Contains(l.Key))
                            {
                                stop.Lines.Add(l.Key);
                            }
                        }

                    }
                }
            }
            /*
            CS2MapViewSystem.DebugStringList!.Add("[TransportStop/ConnectedRoute/Connected]");
            CS2MapViewSystem.DebugStringList!.AddRange(SystemRefs.DebugGetComponentTypeGroup(debugConnectRoutes.ToArray()));
            */
            //waypoint→routewaypoint

            ZipDataWriter.WriteZipXmlEntry(zip, CS2MapDataZipEntryKeys.TransportLinesXml, result);

        }
    }
}

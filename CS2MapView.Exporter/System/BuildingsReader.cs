using CS2MapView.Serialization;
using Game.Areas;
using Game.Objects;
using Game.UI;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using Unity.Collections;
using Unity.Entities;
using GA = Game.Areas;
using GP = Game.Prefabs;
using GB = Game.Buildings;
using GO = Game.Objects;
using Colossal.Entities;
using Game.Prefabs;
using Game.Tools;
using Game.Buildings;
using System.Linq;


namespace CS2MapView.Exporter.System
{
    internal class BuildingsReader
    {
        private SystemRefs SystemRefs { get; set; }

        private NameSystem NameSystem { get; set; }
        private PrefabSystem PrefabSystem { get; set; }

        internal BuildingsReader(SystemRefs systemRefs)
        {
            SystemRefs = systemRefs;
    
            NameSystem = SystemRefs.GetOrCreateSystemManaged<NameSystem>();
            PrefabSystem = SystemRefs.GetOrCreateSystemManaged<PrefabSystem>();
        }

        private static CS2BuildingTypes GetBuildingTypeFromBuilding(SystemRefs eMgr,Entity buildingEntity)
        {
            var type = CS2BuildingTypes.None;
            void AddType<T>(CS2BuildingTypes addType)
            {
                if (eMgr.HasComponent<T>(buildingEntity))
                {
                    type |= addType;
                }
            }

            AddType<GB.Park>(CS2BuildingTypes.Park);
            AddType<GB.Hospital>(CS2BuildingTypes.Hospital);
            AddType<GB.CargoTransportStation>(CS2BuildingTypes.CargoTransportStation);
            AddType<GB.CommercialProperty>(CS2BuildingTypes.CommercialBuilding);
            AddType<GB.ParkingFacility>(CS2BuildingTypes.ParkingFacility);
            AddType<GB.IndustrialProperty>(CS2BuildingTypes.IndustrialBuilding);
            AddType<GB.ExtractorProperty>(CS2BuildingTypes.ExtractorBuilding);
            AddType<GO.UniqueObject>(CS2BuildingTypes.UniqueBuilding);
            AddType<GB.ResidentialProperty>(CS2BuildingTypes.ResidentialBuilding);
            AddType<GB.School>(CS2BuildingTypes.School);
            AddType<GB.AdminBuilding>(CS2BuildingTypes.AdminBuilding);
            AddType<GB.TelecomFacility>(CS2BuildingTypes.TelecomFacility);
            AddType<GB.OfficeProperty>(CS2BuildingTypes.OfficeBuilding);
            AddType<GB.EmergencyShelter>(CS2BuildingTypes.EmergencyShelter);
            AddType<GB.PoliceStation>(CS2BuildingTypes.PoliceStation);
            AddType<GB.WaterPumpingStation>(CS2BuildingTypes.FreshWaterBuilding);
            AddType<GB.SewageOutlet>(CS2BuildingTypes.SewageBuilding);
            AddType<GB.TransportDepot>(CS2BuildingTypes.TransportDepot);
            AddType<GB.DeathcareFacility>(CS2BuildingTypes.DeathcareFacility);
            AddType<GB.FireStation>(CS2BuildingTypes.FireStation);
            AddType<GB.ElectricityProducer>(CS2BuildingTypes.PowerPlant);
            AddType<GB.PublicTransportStation>(CS2BuildingTypes.TransportStation);
            AddType<GB.PostFacility>(CS2BuildingTypes.PostFacility);
            AddType<GB.RoadMaintenance>(CS2BuildingTypes.RoadMaintenanceDepot);
            AddType<GB.ParkMaintenance>(CS2BuildingTypes.ParkMaintenanceDepot);
            AddType<GB.WelfareOffice>(CS2BuildingTypes.WelfareOffice);
            AddType<GB.GarbageFacility>(CS2BuildingTypes.GarbageFacility);
            AddType<GB.Battery>(CS2BuildingTypes.Battery);


            return type;
        }
       
        internal void ReadAndWriteBuildings(ZipArchive zip)
        {
            var list = new List<CS2Building>();
            var prefabEntities = new HashSet<Entity>();
            using var buildingEntities = SystemRefs.Queries.AllBuildings.ToEntityArray(Allocator.Persistent);

            var debugSubObject = new List<Entity>();
            
            var subAreaTargets = CS2BuildingTypes.ExtractorBuilding | CS2BuildingTypes.TransportStation | CS2BuildingTypes.UniqueBuilding;
            var subBuildings = new Dictionary<int,int>();
            foreach (var entity in buildingEntities)
            {
                var type = GetBuildingTypeFromBuilding(SystemRefs, entity);
           
                var currentDistrict = SystemRefs.GetComponentData<CurrentDistrict>(entity);
                var prefabRef = SystemRefs.GetComponentData<GP.PrefabRef>(entity);
                prefabEntities.Add(prefabRef.m_Prefab);
                var transform = SystemRefs.GetComponentData<Transform>(entity);

                CS2Geometry? subAreaGeo = null;
                if ((type & subAreaTargets) != 0)
                {
                    subAreaGeo = new CS2Geometry { Triangles = new List<CS2MapSpaceTriangle>() };
                    if (SystemRefs.TryGetBuffer<GA.SubArea>(entity, true, out var subAreaBuf))
                    {

                        foreach (var subArea in subAreaBuf)
                        {
                            Entity area = subArea.m_Area;

                            if (!SystemRefs.HasComponent<Geometry>(area))
                            {
                                continue;
                            }
                            var geometry = SystemRefs.GetComponentData<Geometry>(area);
                            if (!SystemRefs.TryGetBuffer<Triangle>(area, true, out var triangles))
                            {
                                continue;
                            }
                            if (!SystemRefs.TryGetBuffer<Node>(area, true, out var nodes))
                            {
                                continue;
                            }
                            foreach (var tri in triangles)
                            {
                                var xtr = new CS2MapSpaceTriangle();
                                (xtr.X0, xtr.Y0) = GeometryUtil.ToMapSpacePoint(nodes[tri.m_Indices.x].m_Position);
                                (xtr.X1, xtr.Y1) = GeometryUtil.ToMapSpacePoint(nodes[tri.m_Indices.y].m_Position);
                                (xtr.X2, xtr.Y2) = GeometryUtil.ToMapSpacePoint(nodes[tri.m_Indices.z].m_Position);

                                subAreaGeo.Triangles.Add(xtr);
                            }
                        }
                    }
                }

                if(SystemRefs.TryGetBuffer<GO.SubObject>(entity,true,out var subObjects))
                {
                    for (int i = 0; i < subObjects.Length; i++)
                    {
                        debugSubObject.Add(subObjects[i].m_SubObject);
                        if(SystemRefs.TryGetComponent<Building>(subObjects[i].m_SubObject,out var subBuilding))
                        {
                            subBuildings.Add(subObjects[i].m_SubObject.Index,entity.Index);
                        }
                    }
                }

                var bi = new CS2Building
                {
                    Entity = entity.Index,
                    Prefab = prefabRef.m_Prefab.Index,
                    Name = NameSystem.GetRenderedLabelName(entity),
                    IsCustomName = NameSystem.TryGetCustomName(entity, out _),
                    CurrentDistrict = currentDistrict.m_District.Index,
                    Rotation = GeometryUtil.ConvertForSerialize(transform.m_Rotation),
                    Position = GeometryUtil.ConvertForSerialize(transform.m_Position),
                    AreaGeometry = subAreaGeo,
                    BuildingType = type
                };
                list.Add(bi);
            }

            foreach(var entry in subBuildings)
            {
                list.First(t=>t.Entity == entry.Key).ParentBuilding = entry.Value;
            }

            CS2MapViewSystem.DebugStringList?.Add("[Building]");
            CS2MapViewSystem.DebugStringList?.AddRange(SystemRefs.DebugGetComponentTypeGroup(buildingEntities));
            
            ZipDataWriter.WriteZipXmlEntry(zip, CS2MapDataZipEntryKeys.BuildingsXml, new CS2BuildingsData { 
                Buildings = list ,
                BuildingPrefabs = GetPrefabs(prefabEntities) });

        }

        private List<CS2BuildingPrefab> GetPrefabs(IEnumerable<Entity> prefabEntities)
        {
            var result = new List<CS2BuildingPrefab>();
            foreach (var pfentity in prefabEntities)
            {

                var objGeo = SystemRefs.GetComponentData<GP.ObjectGeometryData>(pfentity);
                var prefab = PrefabSystem.GetPrefab<BuildingPrefab>(pfentity);
                var bpi = new CS2BuildingPrefab
                {
                    Entity = pfentity.Index,
                    Name = prefab.name,
                    Size = GeometryUtil.ConvertForSerialize(objGeo.m_Size),
                };
                if (SystemRefs.TryGetComponent<SchoolData>(pfentity,out var sc))
                {
                    bpi.SubInfo = new CS2BuildingPrefabSubInfo { EducationLevel = sc.m_EducationLevel };
                }
                
                result.Add(bpi);
            }
            return result;
        }
    }
}

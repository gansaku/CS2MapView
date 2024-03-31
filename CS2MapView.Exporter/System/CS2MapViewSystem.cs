using Colossal.Entities;
using Colossal.Logging;
using CS2MapView.Serialization;
using GB = Game.Buildings;
using Game.City;
using Game.Simulation;
using Game.Tools;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using System.Collections.Generic;
using Game.UI;
using Game.Areas;
using Unity.Mathematics;
using Game.Objects;
using GP = Game.Prefabs;
using Game.Net;
using Game.Vehicles;
using System.Runtime.CompilerServices;
using Game.Prefabs;
using Game.Buildings;
using System.Data;
using Game.Routes;
using System.Reflection;


namespace CS2MapView.Exporter.System
{
    public partial class CS2MapViewSystem : COSystemBase
    {
        public static ILog Log = LogManager.GetLogger(Mod.ModPackageName).SetShowsErrorsInUI(false);

        public static CS2MapViewSystem? Instance { get; private set; }

        private CityConfigurationSystem? CityConfigurationSystem { get; set; }
        private NameSystem? NameSystem { get; set; }
       
        private TerrainReader? TerrainReader { get; set; }
        private bool LoadErrorDetected { get; set; } = false;
        private SystemRefs? SystemRefs { get; set; }


        internal static List<string>? DebugStringList { get; set; }


        protected override void OnCreate()
        {
            Log.Info($"CS2MapViewSystem OnCreate");
            base.OnCreate();
            Instance = this;


            SystemRefs = new SystemRefs(World, EntityManager);
            TerrainReader = new TerrainReader(SystemRefs);

            CityConfigurationSystem = World.GetOrCreateSystemManaged<CityConfigurationSystem>();
            NameSystem = World.GetOrCreateSystemManaged<NameSystem>();
            
            if (CityConfigurationSystem is null || NameSystem is null)
            {
                LoadErrorDetected = true;
                Log.Error($"CS2MapViewSystem LoadError");

            }
            EntityQuery GetQueryWithoutTemp<T>()
            {
                return GetEntityQuery( ComponentType.ReadOnly<T>(), ComponentType.Exclude<Temp>() );
            }
            SystemRefs.Queries.AllDistricts = GetQueryWithoutTemp<District>();
            SystemRefs.Queries.AllRoads = GetQueryWithoutTemp<Road>();
            SystemRefs.Queries.AllNodes = GetQueryWithoutTemp<Game.Net.Node>();
            SystemRefs.Queries.AllBuildings = GetQueryWithoutTemp<Building>();
            using var eb = new EntityQueryBuilder(Allocator.Temp).WithAny<TrainTrack, SubwayTrack, TramTrack>().WithNone<Temp>();
            SystemRefs.Queries.AllRails = GetEntityQuery(eb);
            SystemRefs.Queries.AllTransportLines = GetQueryWithoutTemp<TransportLine>();
            SystemRefs.Queries.AllTransportStops = GetQueryWithoutTemp<Game.Routes.TransportStop>();
        }
        protected override void OnUpdate()
        {

        }
        protected override void OnDestroy()
        {
            Log.Info($"CS2MapViewSystem OnDestroy");
            base.OnDestroy();
            Instance = null;
        }

        public Task<string?> RunExport(string? dir, CS2MapViewModSettings.ResolutionRestriction heightMapRestriction, bool addTimestamp)
        {
            return Task.Run((Func<string?>)(() =>
            {
                if (LoadErrorDetected)
                {
                    return null;
                }


                var ex = new CS2MainData();
                DebugStringList = new List<string>();
                ReadCityConfiguration(ex);

                var timestamp = addTimestamp ? $"_{DateTime.Now:yyyyMMddHHmmss}" : string.Empty;

                var fileName = Path.Combine(dir, $"{SafeFileName.GetSafeFileName(ex.CityName)}{timestamp}.cs2map");
                using var fs = new FileStream(fileName, FileMode.Create);
                using var zipArchive = new ZipArchive(fs, ZipArchiveMode.Create);


                ReadAndWriteDistricts(zipArchive);
                new BuildingsReader(SystemRefs!).ReadAndWriteBuildings(zipArchive);
                new RoadsReader(SystemRefs!).ReadRoads(zipArchive);
                new RailwaysReader(SystemRefs!).ReadRails(zipArchive);
                new TransportLinesReader(SystemRefs!).ReadRoutes(zipArchive);

                TerrainReader!.ReadAndWriteTerrain(ex, zipArchive, heightMapRestriction);
                TerrainReader.ReadAndWriteWater(ex, zipArchive, heightMapRestriction);

                var ver = Assembly.GetAssembly(typeof(CS2MainData)).GetName().Version;
                ex.FileVersion = ver.ToString();
#if DEBUG
                ex.TestList = DebugStringList;
#endif 
                ZipDataWriter.WriteZipXmlEntry(zipArchive, CS2MapDataZipEntryKeys.MainXml, ex);

                return fileName;
            }));

        }





        private void ReadCityConfiguration(CS2MainData data)
        {
            data.CityName = CityConfigurationSystem!.cityName;

        }


        private void ReadAndWriteDistricts(ZipArchive zip)
        {
            var list = new List<CS2DistrictInfo>();
            
            using var districtEntities = SystemRefs!.Queries.AllDistricts.ToEntityArray(Allocator.Persistent);
            foreach (var entity in districtEntities)
            {

                var geometry = EntityManager.GetComponentData<Geometry>(entity);
                var triangles = EntityManager.GetBuffer<Triangle>(entity, true);
                var nodes = EntityManager.GetBuffer<Game.Areas.Node>(entity, true);

                var geo = new CS2Geometry { Triangles = new List<CS2MapSpaceTriangle>() };
                foreach (var tri in triangles)
                {
                    var xtr = new CS2MapSpaceTriangle();
                    (xtr.X0, xtr.Y0) = GeometryUtil.ToMapSpacePoint(nodes[tri.m_Indices.x].m_Position);
                    (xtr.X1, xtr.Y1) = GeometryUtil.ToMapSpacePoint(nodes[tri.m_Indices.y].m_Position);
                    (xtr.X2, xtr.Y2) = GeometryUtil.ToMapSpacePoint(nodes[tri.m_Indices.z].m_Position);

                    geo.Triangles.Add(xtr);
                }

                list.Add(new CS2DistrictInfo
                {
                    Entity = entity.Index,
                    Name = NameSystem!.GetRenderedLabelName(entity),
                    IsCustomName = NameSystem.TryGetCustomName(entity, out _),
                    CenterPosition = GeometryUtil.ConvertForSerialize(geometry.m_CenterPosition),
                    Geometry = geo
                });
            }
            ZipDataWriter.WriteZipXmlEntry(zip, CS2MapDataZipEntryKeys.DistrictsXml, new CS2DistrictsData { Districts = list });
        }


   


    }

   
}

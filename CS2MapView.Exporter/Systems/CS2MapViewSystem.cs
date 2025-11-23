using Colossal.Entities;
using Colossal.Logging;
using CS2MapView.Serialization;
using Game.Areas;
using Game.Buildings;
using Game.City;
using Game.Net;
using Game.Routes;
using Game.Tools;
using Game.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;


namespace CS2MapView.Exporter.Systems
{
    public partial class CS2MapViewSystem : COSystemBase
    {
        public static ILog Log = LogManager.GetLogger(Mod.ModPackageName).SetShowsErrorsInUI(true);

        public static CS2MapViewSystem? Instance { get; private set; }

        private CityConfigurationSystem? CityConfigurationSystem { get; set; }
        private NameSystem? NameSystem { get; set; }

        private TerrainReader? TerrainReader { get; set; }
        private WorldBoundsReader? WorldBoundsReader { get; set; }
        private bool LoadErrorDetected { get; set; } = false;
        private SystemRefs? SystemRefs { get; set; }

        public static event Action<string?>? ExportFinished;

        internal static List<string>? DebugStringList { get; set; }


        protected override void OnCreate()
        {
            base.OnCreate();
            Instance = this;

            SystemRefs = new SystemRefs(World, EntityManager);
            TerrainReader = new TerrainReader(SystemRefs);
            WorldBoundsReader = new WorldBoundsReader(SystemRefs);

            CityConfigurationSystem = World.GetOrCreateSystemManaged<CityConfigurationSystem>();
            NameSystem = World.GetOrCreateSystemManaged<NameSystem>();

            if (CityConfigurationSystem is null || NameSystem is null)
            {
                LoadErrorDetected = true;
                Log.Error($"CS2MapViewSystem: Failed to get required systems");
            }
            
            EntityQuery GetQueryWithoutTemp<T>()
            {
                return GetEntityQuery(ComponentType.ReadOnly<T>(), ComponentType.Exclude<Temp>());
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
            // This system doesn't need regular updates
            // Export is triggered manually via RequestExport()
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            Instance = null;
        }

        /// <summary>
        /// Request an export operation. This runs synchronously on the calling thread.
        /// </summary>
        public void RequestExport(string? dir, CS2MapViewModSettings.ResolutionRestriction heightMapRestriction, bool addTimestamp)
        {
            string? result = null;
            try
            {
                result = ExecuteExport(dir, heightMapRestriction, addTimestamp);
            }
            catch (Exception ex)
            {
                Log.Error($"Export failed: {ex}");
                result = null;
            }
            finally
            {
                try { ExportFinished?.Invoke(result); } 
                catch (Exception ex) { Log.Error($"Error in ExportFinished callback: {ex}"); }
            }
        }

        /// <summary>
        /// Check if a city is loaded and ready for export
        /// </summary>
        private bool IsCityLoaded()
        {
            if (SystemRefs == null)
            {
                Log.Error("SystemRefs is null");
                return false;
            }

            // Force recalculate entity counts (don't use cached values)
            int buildingCount = SystemRefs.Queries.AllBuildings.CalculateEntityCount();
            int roadCount = SystemRefs.Queries.AllRoads.CalculateEntityCount();
            int nodeCount = SystemRefs.Queries.AllNodes.CalculateEntityCount();

            // Check city name
            string cityName = CityConfigurationSystem?.cityName ?? "";
            bool hasCityName = !string.IsNullOrEmpty(cityName);
            bool hasEntities = nodeCount > 0 || buildingCount > 0 || roadCount > 0;

            if (!hasCityName && !hasEntities)
            {
                Log.Error("No city loaded. Please load a city before exporting.");
                return false;
            }

            return true;
        }

        private string? ExecuteExport(string? dir, CS2MapViewModSettings.ResolutionRestriction heightMapRestriction, bool addTimestamp)
        {
            if (LoadErrorDetected)
            {
                Log.Error("System not properly initialized");
                return null;
            }

            if (!IsCityLoaded())
            {
                return null;
            }

            var ex = new CS2MainData();
            DebugStringList = new List<string>();
            
            ReadCityConfiguration(ex);

            var timestamp = addTimestamp ? $"_{DateTime.Now:yyyyMMddHHmmss}" : string.Empty;
            var fileName = Path.Combine(dir!, $"{SafeFileName.GetSafeFileName(ex.CityName)}{timestamp}.cs2map");
            
            using var fs = new FileStream(fileName, FileMode.Create);
            using var zipArchive = new ZipArchive(fs, ZipArchiveMode.Create);

            ReadAndWriteDistricts(zipArchive);
            new BuildingsReader(SystemRefs!).ReadAndWriteBuildings(zipArchive);
            new RoadsReader(SystemRefs!).ReadRoads(zipArchive);
            new RailwaysReader(SystemRefs!).ReadRails(zipArchive);
            new TransportLinesReader(SystemRefs!).ReadRoutes(zipArchive);
            TerrainReader!.ReadAndWriteTerrain(ex, zipArchive, heightMapRestriction);
            TerrainReader.ReadAndWriteWater(ex, zipArchive, heightMapRestriction);

            // 世界境界を読み取る（MapExt2対応）
            try
            {
                ex.WorldBounds = WorldBoundsReader!.GetWorldBounds();
                Log.Info($"Exported world bounds: ({ex.WorldBounds.MinX}, {ex.WorldBounds.MinZ}) to ({ex.WorldBounds.MaxX}, {ex.WorldBounds.MaxZ})");
            }
            catch (Exception boundsEx)
            {
                Log.Warn($"Failed to read world bounds, will use default: {boundsEx.Message}");
                ex.WorldBounds = null;
            }

            var ver = Assembly.GetAssembly(typeof(CS2MainData))!.GetName().Version;
            ex.FileVersion = ver!.ToString();
#if DEBUG
            ex.TestList = DebugStringList;
#endif 
            ZipDataWriter.WriteZipXmlEntry(zipArchive, CS2MapDataZipEntryKeys.MainXml, ex);

            return fileName;
        }

        public Task<string?> RunExport(string? dir, CS2MapViewModSettings.ResolutionRestriction heightMapRestriction, bool addTimestamp)
        {
            RequestExport(dir, heightMapRestriction, addTimestamp);
            return Task.FromResult<string?>(null);
        }

        private void ReadCityConfiguration(CS2MainData data)
        {
            string cityName = CityConfigurationSystem!.cityName;
            if (string.IsNullOrEmpty(cityName))
            {
                cityName = "Unnamed City";
            }
            data.CityName = cityName;
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

using CS2MapView.Data;
using CS2MapView.Drawing.Layer;
using CS2MapView.Serialization;

namespace CS2MapView.Drawing.Terrain.CS2
{
    public class CS2TerrainLayerBuilder(ICS2MapViewRoot AppRoot, CS2MapDataSet ExportData)
    {
        /// <summary>
        /// 内容の構築
        /// </summary>
        /// <param name="loadProgressInfo"></param>
        /// <returns></returns>
        public Task<ILayer> BuildAsync(LoadProgressInfo? loadProgressInfo)
        {
            return Task.Run<ILayer>(() =>
            {
                bool vectorizeLand = AppRoot.Context.UserSettings.VectorizeTerrainLand;
                bool vectorizeWater = AppRoot.Context.UserSettings.VectorizeTerrainWater;

                float terrainScale = Math.Min(1f,AppRoot.Context.UserSettings.TerrainMaxResolution / ExportData.MainData!.Terrain!.Resolution.Z);
                float waterScale = Math.Min(1f, AppRoot.Context.UserSettings.WaterMaxResolution / ExportData.MainData!.Water!.Resolution.Z);
                var procParam = new TerrainDrawingsBuilder.Parameters
                {
                    TerrainInputArrayHeight = (int)ExportData.MainData!.Terrain!.Resolution.Z,
                    TerrainInputArrayWidth = (int)ExportData.MainData!.Terrain!.Resolution.Z,
                    WaterInputArrayHeight = (int)ExportData.MainData!.Water!.Resolution.Z,
                    WaterInputArrayWidth = (int)ExportData.MainData!.Water!.Resolution.Z,
                    AppRoot = AppRoot,
                    SeaLevel = 0f,
                    ExecuteLandScale = terrainScale,
                    ExecuteWaterScale = waterScale,
                    InputLandArray = ExportData.TerrainArray,
                    InputWaterArray = ExportData.WaterArray,
                    LandHeights = AppRoot.Context.ContourHeights.LandHeights?.Select(lh => lh.Height),
                    WaterHeights = AppRoot.Context.ContourHeights.WaterHeights?.Select(wh => wh.Height),
                    VectolizeLand = vectorizeLand,
                    VectolizeWater = vectorizeWater,
                    MetricalRect = CS2MapType.CS2WorldRect
                };
                var terrainBuilder = new TerrainDrawingsBuilder(procParam);
                return terrainBuilder.Execute(loadProgressInfo);

            });
        }
    }
}

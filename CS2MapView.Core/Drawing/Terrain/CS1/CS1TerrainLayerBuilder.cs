using CS2MapView.Data;
using CS2MapView.Drawing.Layer;
using CSLMapView.XML;
using System.Data;

namespace CS2MapView.Drawing.Terrain.CS1;
/// <summary>
/// 地形ビルダー(CS1)
/// </summary>
/// <remarks>
/// コンストラクタ
/// </remarks>
/// <param name="AppRoot"></param>
/// <param name="Xml"></param>
public class CS1TerrainLayerBuilder(ICS2MapViewRoot AppRoot, CSLExportXML Xml)
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

            

            var procParam = new TerrainDrawingsBuilder.Parameters
            {
                TerrainInputArrayHeight = CSLExportXML.TERRAIN_RESOLUTION,
                TerrainInputArrayWidth = CSLExportXML.TERRAIN_RESOLUTION,
                WaterInputArrayHeight = CSLExportXML.TERRAIN_RESOLUTION,
                WaterInputArrayWidth = CSLExportXML.TERRAIN_RESOLUTION,
                AppRoot = AppRoot,
                SeaLevel = Xml.SeaLevel,
                ExecuteLandScale = 1f,
                ExecuteWaterScale = 1f,
                InputLandArray = Xml.LandArray,
                InputWaterArray = Xml.WaterArray,
                LandHeights = AppRoot.Context.ContourHeights.LandHeights?.Select(lh => lh.Height),
                WaterHeights = AppRoot.Context.ContourHeights.WaterHeights?.Select(wh => wh.Height),
                VectolizeLand = vectorizeLand,
                VectolizeWater = vectorizeWater,
                MetricalRect = CS1MapType.CS1WorldRect
            };
            var terrainBuilder = new TerrainDrawingsBuilder(procParam);
            return terrainBuilder.Execute(loadProgressInfo);

        });
    }
}

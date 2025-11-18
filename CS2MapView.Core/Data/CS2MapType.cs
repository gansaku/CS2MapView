using CS2MapView.Drawing;
using CS2MapView.Drawing.Buildings.CS2;
using CS2MapView.Drawing.Districts.CS2;
using CS2MapView.Drawing.Grid.CS2;
using CS2MapView.Drawing.Labels;
using CS2MapView.Drawing.Labels.CS2;
using CS2MapView.Drawing.Layer;
using CS2MapView.Drawing.Railways.CS2;
using CS2MapView.Drawing.Roads.CS2;
using CS2MapView.Drawing.Terrain.CS2;
using CS2MapView.Drawing.Transport.CS2;
using CS2MapView.Drawing.Transport;
using CS2MapView.Serialization;
using System.Net.Http.Headers;

namespace CS2MapView.Data
{
    /// <summary>
    /// CS2データ処理セット
    /// </summary>
    public class CS2MapType : SourceMapTypeEx<CS2MapDataSet>
    {
        /// <inheritdoc/>
        public override MapType MapType => MapType.CitiesSkylines2;
        
        /// <summary>
        /// Cities:Skilines2の街サイズ
        /// </summary>
        public static readonly ReadonlyRect CS2WorldRect = new(-7168f, -7168f, 7168f, 7168f);
        
        private readonly ReadonlyRect _worldRect;
        
        /// <summary>
        /// 実際の街サイズ（MapExt2 等の拡張対応）
        /// </summary>
        public override ReadonlyRect WorldRect => _worldRect;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importData"></param>
        public CS2MapType(CS2MapDataSet importData) : base(importData)
        {
            _worldRect = GetWorldRectFromData(importData);
            
            // デバッグ情報をログに出力
            var log = log4net.LogManager.GetLogger(typeof(CS2MapType));
            if (importData?.MainData?.WorldBounds != null)
            {
                var bounds = importData.MainData.WorldBounds;
                log.Info($"[CS2MapView] WorldBounds detected: ({bounds.MinX}, {bounds.MinZ}) to ({bounds.MaxX}, {bounds.MaxZ})");
                log.Info($"[CS2MapView] Map size: {bounds.MaxX - bounds.MinX}m x {bounds.MaxZ - bounds.MinZ}m");
            }
            else
            {
                log.Info("[CS2MapView] No WorldBounds found, using default vanilla size (±7168)");
            }
        }

        /// <summary>
        /// データから実際のワールド座標範囲を取得
        /// WorldBounds が存在する場合はそれを使用、なければデフォルト値
        /// </summary>
        private static ReadonlyRect GetWorldRectFromData(CS2MapDataSet importData)
        {
            // WorldBounds が存在する場合はそれを使用
            if (importData?.MainData?.WorldBounds != null)
            {
                var bounds = importData.MainData.WorldBounds;
                return new ReadonlyRect(bounds.MinX, bounds.MinZ, bounds.MaxX, bounds.MaxZ);
            }
            
            // フォールバック：デフォルトサイズ（原版）
            return CS2WorldRect;
        }

        /// <inheritdoc/>
        protected override string? GetCityNameFromImportData(object data) => (data as CS2MapDataSet)?.MainData?.CityName;

        private class DummyRebuildableLayer(LoadProgressInfo.Process process) : IRebuildableLayerBuilder
        {
            public LoadProgressInfo.Process ProcessType => process;
        }
        
        private Task<ILayer> DummyLayer(ICS2MapViewRoot root, string name, LoadProgressInfo.Process process)
        {
            return Task.Run<ILayer>(() => new RebuildableLayer(root, name, WorldRect, new DummyRebuildableLayer(process)));
        }

        /// <inheritdoc/>
        public override Task<ILayer> BuildBuildingsLayerAsync(ICS2MapViewRoot appRoot, LoadProgressInfo? lpi)
            => new CS2BuildingLayerBuilder(appRoot, GetImportData()).BuildAsync(lpi);
        /// <inheritdoc/>
        public override Task<ILayer> BuildGridLayerAsync(ICS2MapViewRoot appRoot, LoadProgressInfo? lpi)
            => new CS2GridLayerBuilder(appRoot, GetImportData()).BuildAsync(lpi);

        /// <inheritdoc/>
        public override Task BuildLabelContentsAsync(ICS2MapViewRoot appRoot, LabelContentsManager manager)
            => new CS2LabelContentBuilder(appRoot, GetImportData(), manager).BuildAsync(appRoot.Context.ViewContext);
        /// <inheritdoc/>
        public override Task<ILayer> BuildLabelLayerAsync(ICS2MapViewRoot appRoot, LabelContentsManager manager, LoadProgressInfo? lpi)
            => new GeneralLabelLayerBuilder(appRoot, appRoot.Context.ViewContext, manager).BuildAsync(lpi);
        /// <inheritdoc/>
        public override Task<ILayer> BuildRailwaysLayerAsync(ICS2MapViewRoot appRoot, LoadProgressInfo? lpi)
            => new CS2RailwayLayerBuilder(appRoot, GetImportData()).BuildAsync(lpi);

        /// <inheritdoc/>
        public override Task<ILayer> BuildRoadsLayerAsync(ICS2MapViewRoot appRoot, LoadProgressInfo? lpi)
            => new CS2RoadLayerBuilder(appRoot, GetImportData()).BuildAsync(lpi);

        /// <inheritdoc/>
        public override Task<ILayer> BuildTerrainLayerAsync(ICS2MapViewRoot appRoot, LoadProgressInfo? lpi)
            => new CS2TerrainLayerBuilder(appRoot, GetImportData()).BuildAsync(lpi);

        /// <inheritdoc/>
        public override Task<ILayer> BuildTransportLinesLayerAsync(ICS2MapViewRoot appRoot, LoadProgressInfo? lpi)
            => new CS2TransportLineLayerBuilder(appRoot, GetImportData()).BuildAsync(lpi);
        /// <inheritdoc/>
        public override Task<ILayer> BuildDistrictLayerAsync(ICS2MapViewRoot appRoot, LoadProgressInfo? lpi)
            => new CS2DistrictLayerBuilder(appRoot, GetImportData()).BuildAsync(lpi);
    }
}

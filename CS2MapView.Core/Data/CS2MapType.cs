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
        /// <summary>
        /// Cities:Skilines2の街サイズ
        /// </summary>
        public override ReadonlyRect WorldRect => CS2WorldRect;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importData"></param>
        public CS2MapType(CS2MapDataSet importData) : base(importData)
        {
        }

        /// <inheritdoc/>
        protected override string? GetCityNameFromImportData(object data) => (data as CS2MapDataSet)?.MainData?.CityName;

        private class DummyRebuildableLayer(LoadProgressInfo.Process process) : IRebuildableLayerBuilder
        {
            public LoadProgressInfo.Process ProcessType => process;
        }
        private static Task<ILayer> DummyLayer(ICS2MapViewRoot root, string name, LoadProgressInfo.Process process)
        {
            return Task.Run<ILayer>(() => new RebuildableLayer(root, name, CS2WorldRect, new DummyRebuildableLayer(process)));
        }

        /// <inheritdoc/>
        public override Task<ILayer> BuildBuildingsLayerAsync(ICS2MapViewRoot appRoot, LoadProgressInfo? lpi)
            => new CS2BuildingLayerBuilder(appRoot, GetImportData()).BuildAsync(lpi);
        /// <inheritdoc/>
        public override Task<ILayer> BuildGridLayerAsync(ICS2MapViewRoot appRoot, LoadProgressInfo? lpi)
            => new CS2GridLayerBuilder(appRoot).BuildAsync(lpi);

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

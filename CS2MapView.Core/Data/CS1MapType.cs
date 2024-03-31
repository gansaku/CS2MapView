using CS2MapView.Drawing.Buildings.CS1;
using CS2MapView.Drawing.Districts.CS1;
using CS2MapView.Drawing.Grid.CS1;
using CS2MapView.Drawing.Labels;
using CS2MapView.Drawing.Labels.CS1;
using CS2MapView.Drawing.Layer;
using CS2MapView.Drawing.Railways.CS1;
using CS2MapView.Drawing.Roads.CS1;
using CS2MapView.Drawing.Terrain.CS1;
using CS2MapView.Drawing.Transport;
using CS2MapView.Drawing.Transport.CS1;
using CSLMapView.XML;

namespace CS2MapView.Data
{
    /// <summary>
    /// CSLMapView用
    /// </summary>
    public class CS1MapType : SourceMapTypeEx<CSLExportXML>
    {
        /// <inheritdoc/>
        public override MapType MapType => MapType.CitiesSkylines;
        /// <summary>
        /// Cities:Skilinesの街サイズ
        /// </summary>
        public static readonly ReadonlyRect CS1WorldRect = new(-8640f, -8640f, 8640f, 8640f);
        /// <inheritdoc/>
        public override ReadonlyRect WorldRect => CS1WorldRect;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="importData"></param>
        public CS1MapType(CSLExportXML importData) : base(importData)
        {
        }
        /// <inheritdoc/>
        protected override string? GetCityNameFromImportData(object data) => GetImportData()?.City;
        /// <inheritdoc/>
        public override Task<ILayer> BuildTerrainLayerAsync(ICS2MapViewRoot appRoot, LoadProgressInfo? lpi)
            => new CS1TerrainLayerBuilder(appRoot, GetImportData()).BuildAsync(lpi);
        /// <inheritdoc/>
        public override Task<ILayer> BuildGridLayerAsync(ICS2MapViewRoot appRoot, LoadProgressInfo? lpi)
            => new CS1GridLayerBuilder(appRoot).BuildAsync(lpi);
        /// <inheritdoc/>
        public override Task<ILayer> BuildRoadsLayerAsync(ICS2MapViewRoot appRoot, LoadProgressInfo? lpi)
            => new CS1RoadLayerBuilder(appRoot, GetImportData()).BuildAsync(lpi);
        /// <inheritdoc/>
        public override Task<ILayer> BuildBuildingsLayerAsync(ICS2MapViewRoot appRoot, LoadProgressInfo? lpi)
            => new CS1BuildingLayerBuilder(appRoot, GetImportData()).BuildAsync(lpi);
        /// <inheritdoc/>
        public override Task<ILayer> BuildRailwaysLayerAsync(ICS2MapViewRoot appRoot, LoadProgressInfo? lpi)
            => new CS1RailWayLayerBuilder(appRoot, GetImportData()).BuildAsync(lpi, appRoot.Context.ViewContext);
        /// <inheritdoc/>
        public override Task<ILayer> BuildLabelLayerAsync(ICS2MapViewRoot appRoot, LabelContentsManager manager, LoadProgressInfo? lpi)
            => new GeneralLabelLayerBuilder(appRoot, appRoot.Context.ViewContext, manager).BuildAsync(lpi);
        /// <inheritdoc/>
        public override Task BuildLabelContentsAsync(ICS2MapViewRoot appRoot, LabelContentsManager manager)
            => new CS1LabelContentBuilder(appRoot, GetImportData(), manager).BuildAsync(appRoot.Context.ViewContext);
        /// <inheritdoc/>
        public override Task<ILayer> BuildTransportLinesLayerAsync(ICS2MapViewRoot appRoot, LoadProgressInfo? lpi)
            => new CS1TransportLineLayerBuilder(appRoot).BuildAsync(lpi, appRoot.Context.ViewContext);

        /// <inheritdoc/>
        public override Task<ILayer> BuildDistrictLayerAsync(ICS2MapViewRoot appRoot, LoadProgressInfo? lpi)
            => new CS1DistrictLayerBuilder(appRoot).BuildAsync(lpi);
    }
}

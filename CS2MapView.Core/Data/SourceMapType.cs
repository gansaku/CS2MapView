using CS2MapView.Drawing.Labels;
using CS2MapView.Drawing.Layer;
using CS2MapView.Drawing.Transport;

namespace CS2MapView.Data
{
    /// <summary>
    /// 入力元のゲーム別処理
    /// </summary>
    public abstract class SourceMapType
    {
        /// <summary>
        /// 入力データのソース
        /// </summary>
        public abstract MapType MapType { get; }
        /// <summary>
        /// ワールド領域
        /// </summary>
        public abstract ReadonlyRect WorldRect { get; }
        /// <summary>
        /// インポートデータ
        /// </summary>
        public object ImportData { get; set; }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="importData"></param>
        protected SourceMapType(object importData)
        {
            ImportData = importData;
        }
        /// <summary>
        /// 設定を元に、描画対象のレイヤーをすべて作成
        /// </summary>
        /// <param name="appRoot"></param>
        /// <param name="lpi"></param>
        /// <returns></returns>
        public async Task<MapData> BuildAll(ICS2MapViewRoot appRoot, LoadProgressInfo? lpi)
        {
            Task<ILayer>? terrainTask = null;
            Task<ILayer>? gridTask = null;
            Task<ILayer>? districtTask = null;
            Task<ILayer>? roadTask = null;
            Task<ILayer>? buildingTask = null;
            Task<ILayer>? railwaysTask = null;
            Task<ILayer>? labelTask = null;
            Task<ILayer>? transportLineTask = null;

            var us = appRoot.Context.UserSettings;

            appRoot.Context.ViewContext.WorldRect = WorldRect;

            appRoot.Context.ViewContext.ViewLeftTop = (Math.Max(appRoot.Context.ViewContext.ViewLeftTop.x, WorldRect.Left), Math.Max(appRoot.Context.ViewContext.ViewLeftTop.y, WorldRect.Top));

            var labelManager = new LabelContentsManager(appRoot.Context.ViewContext.TextWorldRect);
        
            if (us.IsLayerVisible(ILayer.LayerNameTerrain))
            {
                terrainTask = BuildTerrainLayerAsync(appRoot, lpi);
            }
            else
            {
                lpi?.Progress(this, LoadProgressInfo.Process.BuildTerrain, 1f, null);
            }
            if (us.IsLayerVisible(ILayer.LayerNameGrid))
            {
                gridTask = BuildGridLayerAsync(appRoot, lpi);
            }
            else
            {
                lpi?.Progress(this, LoadProgressInfo.Process.BuildGrid, 1f, null);
            }
            if (us.IsLayerVisible(ILayer.LayerNameDistricts))
            {
                districtTask = BuildDistrictLayerAsync(appRoot, lpi);
            }
            else
            {
                lpi?.Progress(this, LoadProgressInfo.Process.BuildDistricts, 1f, null);
            }
            if (us.IsLayerVisible(ILayer.LayerNameRoads))
            {
                roadTask = BuildRoadsLayerAsync(appRoot, lpi);
            }
            else
            {
                lpi?.Progress(this, LoadProgressInfo.Process.BuildRoads, 1f, null);
            }

            if (us.IsLayerVisible(ILayer.LayerNameBuildings))
            {
                buildingTask = BuildBuildingsLayerAsync(appRoot, lpi);
            }
            else
            {
                lpi?.Progress(this, LoadProgressInfo.Process.BuildBuildings, 1f, null);
            }

            if (us.IsLayerVisible(ILayer.LayerNameRailways))
            {
                railwaysTask = BuildRailwaysLayerAsync(appRoot, lpi);
            }
            else
            {
                lpi?.Progress(this, LoadProgressInfo.Process.BuildRailways, 1f, null);
            }

            if (us.IsLayerVisible(ILayer.LayerNameLabels))
            {
                BuildLabelContentsAsync(appRoot, labelManager).Wait();
                labelTask = BuildLabelLayerAsync(appRoot, labelManager, lpi);
            }
            else
            {
                lpi?.Progress(this, LoadProgressInfo.Process.BuildLabels, 1f, null);
            }

            if (us.IsLayerVisible(ILayer.LayerNameTransportLines))
            {
                transportLineTask = BuildTransportLinesLayerAsync(appRoot, lpi);
            }
            else
            {
                lpi?.Progress(this, LoadProgressInfo.Process.BuildTransportLines, 1f, null);
            }

            var result = new MapData
            {
                MapType = MapType,
                MapName = GetCityNameFromImportData(ImportData),
                WorldRect = WorldRect,
                LabelContentsManager = labelManager
            };
            //描画内容構築
            if (terrainTask is not null) result.Layers.Add(await terrainTask);
            if (gridTask is not null) result.Layers.Add(await gridTask);
            if (roadTask is not null) result.Layers.Add(await roadTask);
            if (districtTask is not null) result.Layers.Add(await districtTask);
            if (buildingTask is not null) result.Layers.Add(await buildingTask);
            if (railwaysTask is not null) result.Layers.Add(await railwaysTask);
            if (labelTask is not null) result.Layers.Add(await labelTask);
            if (transportLineTask is not null) result.Layers.Add(await transportLineTask);

            GC.Collect();

            return result;
        }
        /// <summary>
        /// インポートデータから都市名を取得
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract string? GetCityNameFromImportData(object data);
        /// <summary>
        /// 地形レイヤーの作成
        /// </summary>
        /// <param name="appRoot"></param>
        /// <param name="lpi"></param>
        /// <returns></returns>
        public abstract Task<ILayer> BuildTerrainLayerAsync(ICS2MapViewRoot appRoot, LoadProgressInfo? lpi);
        /// <summary>
        /// グリッドレイヤーの作成
        /// </summary>
        /// <param name="appRoot"></param>
        /// <param name="lpi"></param>
        /// <returns></returns>
        public abstract Task<ILayer> BuildGridLayerAsync(ICS2MapViewRoot appRoot, LoadProgressInfo? lpi);
        /// <summary>
        /// 道路レイヤーの作成
        /// </summary>
        /// <param name="appRoot"></param>
        /// <param name="lpi"></param>
        /// <returns></returns>
        public abstract Task<ILayer> BuildRoadsLayerAsync(ICS2MapViewRoot appRoot, LoadProgressInfo? lpi);
        /// <summary>
        /// 建物レイヤーの作成
        /// </summary>
        /// <param name="appRoot"></param>
        /// <param name="lpi"></param>
        /// <returns></returns>
        public abstract Task<ILayer> BuildBuildingsLayerAsync(ICS2MapViewRoot appRoot, LoadProgressInfo? lpi);
        /// <summary>
        /// 鉄道レイヤーの作成
        /// </summary>
        /// <param name="appRoot"></param>
        /// <param name="lpi"></param>
        /// <returns></returns>
        public abstract Task<ILayer> BuildRailwaysLayerAsync(ICS2MapViewRoot appRoot, LoadProgressInfo? lpi);
        /// <summary>
        /// ラベルレイヤーの作成
        /// </summary>
        /// <param name="appRoot"></param>
        /// <param name="labelManager"></param>
        /// <param name="lpi"></param>
        /// <returns></returns>
        public abstract Task<ILayer> BuildLabelLayerAsync(ICS2MapViewRoot appRoot, LabelContentsManager labelManager, LoadProgressInfo? lpi);
        /// <summary>
        /// ラベル内容の作成
        /// </summary>
        /// <param name="appRoot"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public abstract Task BuildLabelContentsAsync(ICS2MapViewRoot appRoot, LabelContentsManager manager);

        /// <summary>
        /// 交通機関経路の作成
        /// </summary>
        /// <param name="appRoot"></param>
        /// <param name="lpi"></param>
        /// <returns></returns>
        public abstract Task<ILayer> BuildTransportLinesLayerAsync(ICS2MapViewRoot appRoot, LoadProgressInfo? lpi);
        /// <summary>
        /// 区域境界線
        /// </summary>
        /// <param name="appRoot"></param>
        /// <param name="lpi"></param>
        /// <returns></returns>
        public abstract Task<ILayer> BuildDistrictLayerAsync(ICS2MapViewRoot appRoot, LoadProgressInfo? lpi);
    }
    /// <summary>
    /// インポートデータの型指定つき
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SourceMapTypeEx<T> : SourceMapType where T : class
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="importData"></param>
        protected SourceMapTypeEx(T importData) : base(importData)
        {
        }

        private static T ThrowInvalidCast(object data) =>
            throw new InvalidCastException($"Couldn't cast {data.GetType().FullName} to {typeof(T).FullName}");

        private static T ThrowArgNull() => throw new ArgumentNullException(nameof(ImportData));

        /// <summary>
        /// インポートデータを型ありで取得
        /// </summary>
        /// <returns></returns>
        public T GetImportData() => ImportData is null ? ThrowArgNull() : ImportData as T ?? ThrowInvalidCast(ImportData);


    }
}

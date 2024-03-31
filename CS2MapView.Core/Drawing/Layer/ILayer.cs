using CS2MapView.Data;
using SkiaSharp;

namespace CS2MapView.Drawing.Layer
{
    /// <summary>
    /// 描画用のレイヤー
    /// </summary>
    public interface ILayer : IDisposable
    {
        /// <summary>
        /// 地形レイヤー名
        /// </summary>
        public const string LayerNameTerrain = "terrain";
        /// <summary>
        /// 道路レイヤー名
        /// </summary>
        public const string LayerNameRoads = "roads";
        /// <summary>
        /// 建物レイヤー名
        /// </summary>
        public const string LayerNameBuildings = "buildings";
        /// <summary>
        /// 鉄道レイヤー名
        /// </summary>
        public const string LayerNameRailways = "railways";
        /// <summary>
        /// グリッドレイヤー名
        /// </summary>
        public const string LayerNameGrid = "grid";
   
        /// <summary>
        /// ラベルレイヤー名
        /// </summary>
        public const string LayerNameLabels = "labels";
        /// <summary>
        /// 地域レイヤー名
        /// </summary>
        public const string LayerNameDistricts = "districts";

        /// <summary>
        /// 交通機関路線レイヤー名
        /// </summary>
        public const string LayerNameTransportLines = "transportLines";

        /// <summary>
        /// レイヤー名
        /// </summary>
        public string LayerName { get; }
        /// <summary>
        /// 描画を実行する
        /// </summary>
        /// <param name="canvas">描画対象のキャンバス</param>
        /// <param name="viewContext">ビュー設定</param>
        /// <param name="visibleWorldRect">画面内に表示される（可能性のある）範囲。ワールド座標で指定される。回転の都合により、実際の都市の範囲をはみ出す場合がある。</param>
        /// <param name="canvasRect">描画対象の領域のサイズ(0,0,width,height)</param>
        public void DrawLayer(SKCanvas canvas, ViewContext viewContext, SKRect visibleWorldRect, SKRect canvasRect);
    }
}

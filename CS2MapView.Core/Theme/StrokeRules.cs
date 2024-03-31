namespace CS2MapView.Theme
{
    /// <summary>
    /// 線の描画ルール
    /// </summary>
    public class StrokeRules
    {
        /// <summary>
        /// 地域境界
        /// </summary>
        public StrokeStyle? DistrictBoundary { get; set; }
        /// <summary>
        /// 建物
        /// </summary>
        public StrokeStyle? Building { get; set; }
        /// <summary>
        /// 駅
        /// </summary>
        public StrokeStyle? Station { get; set; }
        /// <summary>
        /// グリッド
        /// </summary>
        public StrokeStyle? Grid { get; set; }
        /// <summary>
        /// 地下鉄
        /// </summary>
        public StrokeStyle? MetroRailway { get; set; }
        /// <summary>
        /// 地下鉄2
        /// </summary>
        public StrokeStyle? MetroRailwayDash { get; set; }
        /// <summary>
        /// トラム
        /// </summary>
        public StrokeStyle? TramRailway { get; set; }
        /// <summary>
        /// トラム2
        /// </summary>
        public StrokeStyle? TramRailwayDash { get; set; }
        /// <summary>
        /// ロープウェイ
        /// </summary>
        public StrokeStyle? CableCar { get; set; }
        /// <summary>
        /// ロープウェイ2
        /// </summary>
        public StrokeStyle? CableCarDecoration { get; set; }
        /// <summary>
        /// モノレール
        /// </summary>
        public StrokeStyle? Monorail { get; set; }
        /// <summary>
        /// モノレール2
        /// </summary>
        public StrokeStyle? MonorailDecoration { get; set; }
        /// <summary>
        /// 鉄道
        /// </summary>
        public StrokeStyle? Train { get; set; }
        /// <summary>
        /// 鉄道2
        /// </summary>
        public StrokeStyle? TrainDash { get; set; }
        /// <summary>
        /// 鉄道トンネル
        /// </summary>
        public StrokeStyle? TrainTunnel { get; set; }
        /// <summary>
        /// 鉄道トンネル2
        /// </summary>
        public StrokeStyle? TrainTunnelDash { get; set; }
        /// <summary>
        /// 停留所
        /// </summary>
        public StrokeStyle? TransportStop { get; set; }

        /// <summary>
        /// 既定値
        /// </summary>
        public static StrokeRules Default => new()
        {
            Grid = new(new(1.3f, 0.5f, 0.3f, 2f, 0f), [2f, 1f], StrokeType.Flat),
            Building = new(new(0.8f, 1f, 0.5f, 1f, 0.5f)),
            MetroRailway = new(new(2.7f, 1f, 1f, 19f, 0f), null, StrokeType.Flat),
            MetroRailwayDash = new(new(2f, 1f, 1f, 16f, 0f), [2f, 2f], StrokeType.Flat),
            TramRailway = new(new(2.3f, 1f, 1f, 16f, 0f), null, StrokeType.Flat),
            TramRailwayDash = new(new(1.6f, 1f, 1f, 14f, 0f), [2f, 2f], StrokeType.Flat),
            CableCar = new(new(1.2f, 1f, 0.5f, 5f, 0f), null, StrokeType.Flat),
            CableCarDecoration = new(new(1f, 0.8f, 0.5f, 2f, 0.5f), null, StrokeType.Flat),
            Monorail = new(new(1.5f, 1f, 0.5f, 7f, 0f), null, StrokeType.Flat),
            MonorailDecoration = new(new(1f, 0.8f, 0.5f, 2f, 0.5f), null, StrokeType.Flat),
            Train = new(new(2.7f, 1f, 1f, 19f, 0f), null, StrokeType.Flat),
            TrainDash = new(new(2f, 1f, 1f, 16f, 0f), [2f, 2f], StrokeType.Flat),
            TrainTunnel = null,
            TrainTunnelDash = new(new(2f, 1f, 0.7f, 10f, 0f), [5f, 3f], StrokeType.Flat),
            Station = new(new(1f, 1f, 0.5f, 2f, 0.3f)),
            DistrictBoundary = new(new(1.5f, 0.5f, 0.8f, 2f, 0f), [4f, 2f, 1f, 2f])
        };

    }
}

namespace CS2MapView.Config
{
    /// <summary>
    /// 路線図設定
    /// </summary>
    public class TransportRouteMapConfig
    {
        /// <summary>
        /// 停留所・駅のマージする範囲
        /// </summary>
        public float StopsMergingRange { get; set; } = 48f;

        /// <summary>
        /// バス路線表示有無
        /// </summary>
        public bool RenderBusLine { get; set; } = true;
        /// <summary>
        /// トラム路線表示有無
        /// </summary>
        public bool RenderTramLine { get; set; } = false;
        /// <summary>
        /// 地下鉄路線表示有無
        /// </summary>
        public bool RenderMetroLine { get; set; } = false;
        /// <summary>
        /// 鉄道路線表示有無
        /// </summary>
        public bool RenderTrainLine { get; set; } = false;
        /// <summary>
        /// モノレール路線表示有無
        /// </summary>
        public bool RenderMonorailLine { get; set; } = false;
        /// <summary>
        /// 飛行機路線表示有無
        /// </summary>
        public bool RenderAirplaneLine { get; set; } = false;
        /// <summary>
        /// 船路線表示有無
        /// </summary>
        public bool RenderShipLine { get; set; } = false;


        /// <summary>
        /// バス停表示有無
        /// </summary>
        public bool RenderBusStop { get; set; } = true;
        /// <summary>
        /// 電停表示有無
        /// </summary>
        public bool RenderTramStop { get; set; } = true;

        /// <summary>
        /// 地下鉄駅表示有無
        /// </summary>
        public bool RenderMetroStop { get; set; } = false;
        /// <summary>
        /// 鉄道駅表示有無
        /// </summary>
        public bool RenderTrainStop { get; set; } = false;

        /// <summary>
        /// モノレール駅表示有無
        /// </summary>
        public bool RenderMonorailStop { get; set; } = false;

        /// <summary>
        /// 飛行機乗り場表示有無
        /// </summary>
        public bool RenderAirplaneStop { get; set; } = false;

        /// <summary>
        /// 船乗り場表示有無
        /// </summary>
        public bool RenderShipStop { get; set; } = false;

        /// <summary>
        /// 貨物非表示
        /// </summary>
        public bool HideCargoLines { get; set; } = true;

        /*
        /// <summary>
        /// バスとトラムの停留所をマージするか
        /// </summary>
        public bool MergeBusTramStop { get; set; } = true;
        /// <summary>
        /// 鉄道とメトロの駅をマージするか
        /// </summary>
        public bool MergeTrainMetroStaion { get; set; } = false;
        /// <summary>
        /// 色の自動設定を使用するか
        /// </summary>
        public bool AutoColoring { get; set; } = false;
        /// <summary>
        /// 経路共有時、幅を広くして全部表示するか
        /// </summary>
        public bool WidenOnSharedLines { get; set; } = true;

        /// <summary>
        /// 終点のループを表示しない
        /// </summary>
        public bool DetectEndLoop { get; set; } = false;
        /// <summary>
        /// 一方通行の路線をマークする
        /// </summary>
        public bool MarkOneWayRoutes { get; set; } = false;
        /// <summary>
        /// 系統番号をマージして表示
        /// </summary>
        public bool UseMergedRouteNumberings { get; set; } = true;
        */
    }
}

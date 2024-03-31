using CS2MapView.Data;

namespace CS2MapView.Drawing
{
    /// <summary>
    /// 回転時に内容再構築可能なレイヤーのビルダー
    /// </summary>
    public interface IRebuildOnRotateLayerBuilder : IRebuildableLayerBuilder
    {
        /// <summary>
        /// 回転時の再構築を実行します。
        /// </summary>
        /// <param name="loadProgressInfo"></param>
        /// <param name="vc"></param>
        /// <returns></returns>
        public Task RotateAsync(LoadProgressInfo? loadProgressInfo, ViewContext vc);
    }
}

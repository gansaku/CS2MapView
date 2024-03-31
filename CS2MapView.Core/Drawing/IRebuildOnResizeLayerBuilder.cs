using CS2MapView.Data;

namespace CS2MapView.Drawing
{
    /// <summary>
    /// ズーム変更時に再構築可能なビルダー
    /// </summary>
    public interface IRebuildOnResizeLayerBuilder : IRebuildableLayerBuilder
    {
        /// <summary>
        /// ズーム変更時の再構築を実行します。
        /// </summary>
        /// <param name="loadProgressInfo"></param>
        /// <param name="vc"></param>
        /// <returns></returns>
        Task ResizeAsync(LoadProgressInfo? loadProgressInfo, ViewContext vc);
    }
}

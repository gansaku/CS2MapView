namespace CS2MapView.Drawing
{
    /// <summary>
    /// 一部のみ内容を再構築可能なLayerBuilder
    /// </summary>
    public interface IRebuildableLayerBuilder
    {
        /// <summary>
        /// 進捗表示用の処理タイプ
        /// </summary>
        public LoadProgressInfo.Process ProcessType { get; }

    }
}

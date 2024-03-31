using CS2MapView.Data;

namespace CS2MapView
{
    /// <summary>
    /// CS2MapView.Coreを使用するクライアントプログラムに必要なプロパティ、メソッド
    /// </summary>
    public interface ICS2MapViewRoot
    {
        /// <summary>
        /// 描画設定を保持。
        /// </summary>
        public RenderContext Context { get; }
        /// <summary>
        /// 読み込んだマップのデータ。
        /// ファイル読み込み時に、このプロパティに結果がセットされる。
        /// </summary>
        public MapData? MapData { get; set; }

        /// <summary>
        /// ファイル読み込みの進捗が進んだときに呼ばれる。
        /// 別スレッドから呼ばれるかも。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void ReceiveProgressChanged(object sender, LoadProgressEventArgs args);
    }
}

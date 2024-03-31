using CS2MapView.Drawing.Labels;
using CS2MapView.Drawing.Layer;
using CS2MapView.Drawing.Transport;
using Gfw.Common;

namespace CS2MapView.Data
{
    /// <summary>
    /// 読み込んだマップの種類
    /// </summary>
    public enum MapType
    {
        /// <summary>
        /// Cities:Skylines 
        /// </summary>
        CitiesSkylines,
        /// <summary>
        /// Cities:Skylines2
        /// </summary>
        CitiesSkylines2
    }
    /// <summary>
    /// 読み込んだマップに関するデータ
    /// </summary>
    public sealed class MapData : IDisposable
    {
        /// <summary>
        /// マップの種類
        /// </summary>
        public MapType MapType { get; set; }
        /// <summary>
        /// マップ名
        /// </summary>
        public string? MapName { get; set; }
        /// <summary>
        /// 読み込んだファイル名
        /// </summary>
        public string? FileName { get; set; }
        /// <summary>
        /// マップの座標全域
        /// </summary>
        public ReadonlyRect WorldRect { get; set; }
        /// <summary>
        /// レイヤー
        /// </summary>
        public List<ILayer> Layers { get; } = [];

        /// <summary>
        /// ラベル内容マネージャ
        /// </summary>
        public LabelContentsManager? LabelContentsManager { get; set; }

        private async Task RebuildImpl(ViewContext vc, LoadProgressEventHandler? progressCallback, Func<RebuildableLayer, bool> where)
        {
            LoadProgressInfo lpi = new();
            if (progressCallback is not null)
            {
                lpi.LoadProgress += progressCallback;

            }

            List<Task> tasks = [];
            var targets = Layers.Where(
                l => l is RebuildableLayer rr
                && where(rr)
                && rr.Builder is not null).Select(l => (RebuildableLayer)l);

            if (LabelContentsManager is not null)
            {
                await LabelContentsManager.RebuildAsync(vc);

            }

            targets.ForEach(l => lpi.RegistProcess(l.Builder!.ProcessType));
            targets.ForEach(l => tasks.Add(l.ResizeAsync(lpi, vc)));

            foreach (var item in tasks)
            {
                await item;
            }
        }
        /// <summary>
        /// ビュー拡大率変更時の描画内容再構築
        /// </summary>
        /// <param name="vc"></param>
        /// <param name="progressCallback"></param>
        /// <returns></returns>
        public Task RebuildLayersOnResize(ViewContext vc, LoadProgressEventHandler? progressCallback)
        {
            return RebuildImpl(vc, progressCallback, r => r.Resizable);

        }
        /// <summary>
        /// ビュー回転変更時の描画内容再構築
        /// </summary>
        /// <param name="vc"></param>
        /// <param name="progressCallback"></param>
        /// <returns></returns>
        public Task RebuildLayersOnRotate(ViewContext vc, LoadProgressEventHandler? progressCallback)
        {
            return RebuildImpl(vc, progressCallback, r => r.Rotatable);
        }
        /// <summary>
        /// ビュー変更時の描画内容再構築
        /// </summary>
        /// <param name="vc"></param>
        /// <param name="progressCallback"></param>
        /// <returns></returns>
        public Task RebuildLayersOnResizeOrRotate(ViewContext vc, LoadProgressEventHandler? progressCallback)
        {
            return RebuildImpl(vc, progressCallback, r => r.Rotatable || r.Resizable);
        }
        /// <inheritdoc/>
        public void Dispose()
        {
            foreach (var layer in Layers)
            {
                layer.Dispose();
            }
            Layers.Clear();
        }
    }
}

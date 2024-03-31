using CS2MapView.Data;
using System.Diagnostics.CodeAnalysis;

namespace CS2MapView.Drawing.Layer
{
    /// <summary>
    /// 拡大（回転）時に描画内容を変更するレイヤー
    /// </summary>
    public class RebuildableLayer : BasicLayer
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appRoot"></param>
        /// <param name="layerName"></param>
        /// <param name="worldRect"></param>
        [SetsRequiredMembers]
        public RebuildableLayer(ICS2MapViewRoot appRoot, string layerName, ReadonlyRect worldRect) : base(appRoot, layerName, worldRect) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appRoot"></param>
        /// <param name="layerName"></param>
        /// <param name="worldRect"></param>
        /// <param name="builder"></param>
        [SetsRequiredMembers]
        public RebuildableLayer(ICS2MapViewRoot appRoot, string layerName, ReadonlyRect worldRect, IRebuildableLayerBuilder? builder) : base(appRoot, layerName, worldRect)
        {
            Builder = builder;
        }
        /// <summary>
        /// 描画内容の変更に使用するビルダー
        /// </summary>
        public IRebuildableLayerBuilder? Builder { get; set; }
        /// <summary>
        /// trueの場合、拡大率変更の際に再構築が必要であることを示す。
        /// </summary>
        public bool Resizable => Builder as IRebuildOnResizeLayerBuilder is not null;
        /// <summary>
        /// trueの場合、回転の際に再構築が必要であることを示す。
        /// </summary>
        public bool Rotatable => Builder as IRebuildOnRotateLayerBuilder is not null;
        /// <summary>
        /// 拡大率変更の際の再構築を実行する。
        /// </summary>
        /// <param name="lpi"></param>
        /// <param name="vc"></param>
        /// <returns></returns>
        public Task ResizeAsync(LoadProgressInfo? lpi, ViewContext vc)
        {
            if (Builder is IRebuildOnResizeLayerBuilder b)
            {
                return b.ResizeAsync(lpi, vc);
            }
            else
            {
                return Task.CompletedTask;
            }
        }
        /// <summary>
        /// 回転の際の再構築を実行する。
        /// </summary>
        /// <param name="lpi"></param>
        /// <param name="vc"></param>
        /// <returns></returns>
        public Task RotateAsync(LoadProgressInfo? lpi, ViewContext vc)
        {
            if (Builder is IRebuildOnRotateLayerBuilder b)
            {
                return b.RotateAsync(lpi, vc);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

    }
}

using SkiaSharp;

namespace CS2MapView.Drawing
{
    /// <summary>
    /// Layerが使用する描画コマンド
    /// </summary>
    public interface IDrawCommand : IDisposable
    {
        /// <summary>
        /// 描画する領域を包含する長方形（検索用）
        /// </summary>
        SKRect BoundingRect { get; }
    }
}

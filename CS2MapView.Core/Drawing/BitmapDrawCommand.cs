using SkiaSharp;

namespace CS2MapView.Drawing
{
    /// <summary>
    /// ビットマップ描画コマンド
    /// </summary>
    public struct BitmapDrawCommand : IDrawCommand
    {
        /// <summary>
        /// ビットマップ
        /// </summary>
        public required SKBitmap Bitmap { get; init; }
        /// <summary>
        /// 描画先の矩形
        /// </summary>
        public required SKRect TargetRect { get; init; }
        /// <summary>
        /// 描画時に使用するビットマップの範囲
        /// </summary>
        public SKRect? SrcRect { get; set; }

        public readonly SKRect BoundingRect => TargetRect;

        public readonly void Draw(SKCanvas canvas)
        {
            if (SrcRect is null)
            {
                canvas.DrawBitmap(Bitmap, TargetRect);
            }
            else
            {
                canvas.DrawBitmap(Bitmap, SrcRect.Value, TargetRect);
            }

        }
#pragma warning disable IDE0251
        public void Dispose()
        {
            Bitmap.Dispose();

        }
#pragma warning restore
    }
}

using SkiaSharp;

namespace CS2MapView.Drawing
{
    /// <summary>
    /// SKPictureを描画するコマンド
    /// </summary>
    public struct SKPictureDrawCommand : IDrawCommand
    {
        /// <summary>
        /// ビットマップ
        /// </summary>
        public required SKPicture Picture { get; init; }
        /// <summary>
        /// 描画位置
        /// </summary>
        public required SKPoint Location { get; init; }
        /// <summary>
        /// 拡大率
        /// </summary>
        public required float Scale { get; init; }

        /// <summary>
        /// 作成するときにあらかじめ計算しておく
        /// </summary>
        public required SKRect BoundingRect { get; init; }

        /// <summary>
        /// Dispose時にPictureをDisposeするならtrue。既定ではしない。
        /// </summary>
        public bool DisposeRequired { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SKPictureDrawCommand()
        {
            DisposeRequired = false;
        }

        /// <summary>
        /// 描画の実行
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="scaleFactor"></param>
        /// <param name="worldScaleFactor"></param>
        public readonly void Draw(SKCanvas canvas, float scaleFactor, float worldScaleFactor)
        {
            var t = scaleFactor * worldScaleFactor;
            var s = t * Scale;

            var matrix =
                SKMatrix.CreateScale(s, s).PostConcat(
                SKMatrix.CreateTranslation(Location.X, Location.Y));

            canvas.DrawPicture(Picture, ref matrix);


        }
        /// <summary>
        /// オブジェクトの破棄
        /// </summary>
        public readonly void Dispose()
        {
            if (DisposeRequired)
            {
                Picture?.Dispose();
            }
        }
    }
}

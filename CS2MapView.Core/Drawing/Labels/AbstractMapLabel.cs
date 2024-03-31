using SkiaSharp;

namespace CS2MapView.Drawing.Labels
{
    /// <summary>
    /// クライアント座標で処理して平行移動だけして表示する
    /// </summary>
    internal abstract class AbstractMapLabel
    {
        /// <summary>
        /// 場所調整で動かない
        /// </summary>
        internal abstract bool Freezed { get; }
        /// <summary>
        /// 表示が重なったら非表示にしてもよい
        /// </summary>
        internal abstract bool MayYield { get; }
        /// <summary>
        /// 非表示化の優先度
        /// </summary>
        internal abstract int YieldPriority { get; }
        /// <summary>
        /// 位置
        /// </summary>
        internal SKPoint OriginalPosition;

        internal SKPoint DisplayPosition;

        internal SKPoint Speed;

        internal (float width, float height) Size;

        internal bool Yielded;


        internal virtual SKRect Bounds => new(DisplayPosition.X - Size.width / 2f, DisplayPosition.Y - Size.height / 2f, DisplayPosition.X + Size.width / 2f, DisplayPosition.Y + Size.height / 2f);


        internal void MoveOriginalPosition(float x, float y)
        {
            OriginalPosition.X += x;
            OriginalPosition.Y += y;
            DisplayPosition.X += x;
            DisplayPosition.Y += y;
        }

        internal static bool Intersects(AbstractMapLabel a, AbstractMapLabel b)
            => new SKRect(
                a.DisplayPosition.X - a.Size.width / 2,
                a.DisplayPosition.Y - a.Size.height / 2,
                a.DisplayPosition.X + a.Size.width / 2,
                a.DisplayPosition.Y + a.Size.height / 2).IntersectsWith(
            new SKRect(
                b.DisplayPosition.X - b.Size.width / 2,
                b.DisplayPosition.Y - b.Size.height / 2,
                b.DisplayPosition.X + b.Size.width / 2,
                b.DisplayPosition.Y + b.Size.height / 2));

    }
}

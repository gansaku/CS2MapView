using SkiaSharp;

namespace CS2MapView.Drawing.Labels
{
    internal abstract class AbstractTextOnPathLabel : AbstractMapLabel
    {
        internal enum PathPointType
        {
            Line,
            CubicBesier
        }
        /// <summary>
        /// PathかPathPointsどちらかを設定する
        /// </summary>
        internal SKPoint[]? PathPoints { get; set; }

        internal PathPointType PathType { get; set; } = PathPointType.Line;

        internal string? Text;

        internal float Ascent;

        internal float TextOffset;

        internal override SKRect Bounds
        {
            get
            {
                var val = Math.Max(Size.width, Size.height) / 2;
                var r = new SKRect(DisplayPosition.X - val, DisplayPosition.Y - val, DisplayPosition.X + val, DisplayPosition.Y + val);
                return r;
                /*
                float xmin = PathPoints.Min(t => t.X);
                float xmax = PathPoints.Max(t => t.X);
                float ymin = PathPoints.Min(t => t.Y);
                float ymax = PathPoints.Max(t => t.Y);
                var r = new SKRect(xmin, ymin, xmax, ymax);
                r.Inflate(Size.width,Size.height);
                return r;*/
            }
        }


    }
}

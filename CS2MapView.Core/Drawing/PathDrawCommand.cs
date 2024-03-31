using SkiaSharp;

namespace CS2MapView.Drawing
{
    public struct PathDrawCommand : IDrawCommand
    {
        public SKPath? Path;
        public SKColor? FillColor;
        public Func<float, float, SKPaintCache.StrokeKey?>? StrokePaintFunc;

        public readonly SKRect BoundingRect
        {
            get
            {
                if (Path is null)
                {
                    return new SKRect();
                }
                var b = Path.Bounds;
                if (b.Width == 0 || b.Height == 0)
                {
                    b.Inflate(0.1f, 0.1f);
                }
                return b;

            }
        }

        public readonly void Draw(SKCanvas canvas, float scaleFactor, float worldScaleFactor)
        {
            if (FillColor.HasValue)
            {
                canvas.DrawPath(Path, SKPaintCache.Instance.GetFill(FillColor.Value));

            }
            if (StrokePaintFunc is not null)
            {
                var key = StrokePaintFunc(scaleFactor, worldScaleFactor);
                if (key is not null)
                {
                    canvas.DrawPath(Path, SKPaintCache.Instance.GetStroke(key));

                }
            }
        }

        public void Dispose()
        {
            Path?.Dispose();
            Path = null;
        }
    }
}

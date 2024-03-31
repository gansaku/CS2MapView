using SkiaSharp;

namespace CS2MapView.Drawing
{
    public struct TextOnPathDrawCommand : IDrawCommand
    {

        public required SKPath Path;
        public required string Text;
        public required float Ascent;
        public required float TextOffset;
        public Func<float, float, SKPaintCache.TextFillKey?>? TextFillPaintFunc;
        public Func<float, float, SKPaintCache.TextStrokeKey?>? TextStrokePaintFunc;
        /// <inheritdoc/>
        public readonly SKRect BoundingRect => Path?.Bounds ?? SKRect.Empty;


        public readonly void Draw(SKCanvas canvas, float scaleFactor, float worldScaleFactor)
        {
            if (TextStrokePaintFunc is not null)
            {
                var key = TextStrokePaintFunc(scaleFactor, worldScaleFactor);
                if (key is not null)
                {
                    canvas.DrawTextOnPath(Text, Path, TextOffset, -Ascent / 2, SKPaintCache.Instance.GetTextStroke(key));

                }
            }
            if (TextFillPaintFunc is not null)
            {
                var key = TextFillPaintFunc(scaleFactor, worldScaleFactor);
                if (key is not null)
                {
                    canvas.DrawTextOnPath(Text, Path, TextOffset, -Ascent / 2, SKPaintCache.Instance.GetTextFill(key));
                }
            }


        }
        /// <inheritdoc/>
        public void Dispose()
        {

            Path?.Dispose();
            Path = null!;
        }
    }
}

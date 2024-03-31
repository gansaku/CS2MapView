using SkiaSharp;

namespace CS2MapView.Drawing
{
    public struct TextDrawCommand : IDrawCommand
    {
        /// <summary>
        /// 描画位置 left,baseline
        /// </summary>
        public required SKPoint Location;
        public required string Text;

        //text入れるときにmeasureではかる
        public SKRect BoundingRect { get; set; }

        public TextDrawCommand()
        {


        }

        public Func<float, float, SKPaintCache.TextFillKey?>? TextFillPaintFunc;
        public Func<float, float, SKPaintCache.TextStrokeKey?>? TextStrokePaintFunc;

        public readonly void Draw(SKCanvas canvas, float scaleFactor, float worldScaleFactor)
        {
            if (TextStrokePaintFunc is not null)
            {
                var key = TextStrokePaintFunc(scaleFactor, worldScaleFactor);
                if (key is not null)
                {
                    canvas.DrawText(Text, Location, SKPaintCache.Instance.GetTextStroke(key));
                }
            }
            if (TextFillPaintFunc is not null)
            {
                var key = TextFillPaintFunc(scaleFactor, worldScaleFactor);
                if (key is not null)
                {
                    canvas.DrawText(Text, Location, SKPaintCache.Instance.GetTextFill(key));
                }
            }


        }

        public readonly void Dispose()
        {
        }
    }
}

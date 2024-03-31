using SkiaSharp;

namespace CS2MapView.Drawing.Labels
{
    internal abstract class AbstractTextLabel : AbstractMapLabel
    {
        internal string? Text;

        internal float Ascent;

        internal static T Create<T>(string text, SKPoint center, SKPaint textPaint, float viewScaleFromWorld) where T : AbstractTextLabel, new()
        {
            SKRect textBound = new();
            textPaint.MeasureText(text, ref textBound);

            var width = textBound.Width * viewScaleFromWorld;
            var height = textBound.Height * viewScaleFromWorld;
            return new T()
            {
                Text = text,
                OriginalPosition = center,
                DisplayPosition = center,
                Size = (width, height),
                Ascent = textBound.Top * viewScaleFromWorld
            };
        }
        public override string ToString()
        {
            return $"{GetType().Name} {Text} {OriginalPosition}";
        }
    }
}

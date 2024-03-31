using SkiaSharp;

namespace CS2MapView.Drawing
{
    public struct DelegateDrawCommand : IDrawCommand
    {

        public required Action<SKCanvas, SKRect> DrawAction;

        public required SKRect BoundingRect { get; set; }

        public readonly void Draw(SKCanvas canvas, SKRect visibleWorldRect)
        {
            DrawAction(canvas, visibleWorldRect);
        }

        public readonly void Dispose()
        {
        }
    }
}

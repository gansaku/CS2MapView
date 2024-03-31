using CS2MapView.Data;
using CS2MapView.Util;
using SkiaSharp;
using System.Diagnostics.CodeAnalysis;

namespace CS2MapView.Drawing.Layer
{
    /// <summary>
    /// 座標変換がスクロール以外終わっているオブジェクトを表示するレイヤー。
    /// 本クラスでの座標の取り扱いは親クラスと異なる。
    /// </summary>
    public class RotatedLayer : RebuildableLayer
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appRoot"></param>
        /// <param name="layerName"></param>
        /// <param name="transformedWorldBound">親クラスとは異なり、ワールド座標に現在の拡大・回転をかけた四角形を内接する、座標に平行な四角形を指定する。</param>
        /// <param name="builder"></param>
        [SetsRequiredMembers]
        public RotatedLayer(ICS2MapViewRoot appRoot, string layerName, ReadonlyRect transformedWorldBound, IRebuildOnResizeLayerBuilder? builder = null) : base(appRoot, layerName, transformedWorldBound)
        {
            Builder = builder;
        }
        /// <summary>
        /// DrawCommandsを初期化する。
        /// </summary>
        /// <param name="transformedWorldBound"></param>
        /// <param name="disposePath"></param>
        public void ClearAndResizeCommandsList(ReadonlyRect transformedWorldBound, bool disposePath = true)
        {
            lock (DrawObjectsLock)
            {
                DrawCommands?.ClearContent(disposePath);
                DrawCommands = new DrawCommandLookup(transformedWorldBound);
            }
        }
        /// <summary>
        /// すでにかかっている変換を元に戻したうえで、既定の変換を適用する。
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="vc"></param>
        protected override void SetMatrix(SKCanvas canvas, ViewContext vc)
        {
            var scalemerged = vc.ScaleFactor * vc.WorldScaleFactor;

            var invert = SKMatrix.CreateRotation(vc.Angle).PostConcat(SKMatrix.CreateScale(scalemerged, scalemerged)).Invert();

            var newMatrix = invert.PostConcat(vc.ViewTransform);

            canvas.SetMatrix(newMatrix);
        }
        /// <inheritdoc/>
        protected override IEnumerable<IDrawCommand> GetTargetDrawCommands(SKRect visibleWorldRect, SKRect canvasRect, ViewContext vc)
        {
            //  Debug.Print($"visibleWorldRect={visibleWorldRect}");
            //   Debug.Print($"canvasRect={canvasRect}");

            //元の実装はworld座標
            //現在のオブジェクトはそれに回転、拡大をかけた座標が入っている

            var scalemerged = vc.ScaleFactor * vc.WorldScaleFactor;

            var mat = SKMatrix.CreateRotation(vc.Angle).PostConcat(SKMatrix.CreateScale(scalemerged, scalemerged));
            var points = mat.MapPoints(SKPathEx.RectToPath(visibleWorldRect).Points);
            var rect = new SKRect(points.Min(p => p.X), points.Min(p => p.Y), points.Max(p => p.X), points.Max(p => p.Y));

            if (rect.Width < DrawCommands.WorldRect.Width / 2 || rect.Height < DrawCommands.WorldRect.Height)
            {
                return DrawCommands.GetOrderedObjects(rect);
            }
            else
            {
                return DrawCommands.GetAllOrderedObjects();
            }
        }

    }
}

using CS2MapView.Data;
using SkiaSharp;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace CS2MapView.Drawing.Layer
{
    /// <summary>
    /// 拡大・回転で内容の変化しないレイヤー。
    /// 本クラス内での座標はワールド座標。
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <param name="appRoot">クライアントアプリケーションクラス</param>
    /// <param name="layerName">レイヤー名</param>
    /// <param name="worldRect">都市の領域サイズ</param>
    /// /// <summary>
    /// 拡大・回転で内容の変化しないレイヤー。
    /// 本クラス内での座標はワールド座標。
    /// </summary>
    [method: SetsRequiredMembers]
    public class BasicLayer(ICS2MapViewRoot appRoot, string layerName, ReadonlyRect worldRect) : ILayer
    {
        /// <summary>
        /// レイヤーの保持しているコマンドを変更する場合はこれでロックすること
        /// </summary>
        internal object DrawObjectsLock = new();
        /// <inheritdoc/>
        public required string LayerName { get; init; } = layerName;
        /// <summary>
        /// クライアントアプリケーションクラス
        /// </summary>
        public required ICS2MapViewRoot AppRoot { get; init; } = appRoot;
        /// <summary>
        /// 描画命令
        /// </summary>
        internal DrawCommandLookup DrawCommands { get; set; } = new(worldRect);
        /// <summary>
        /// 都市の領域サイズ
        /// </summary>
        protected ReadonlyRect WorldMetricalRect { get; init; } = worldRect;

        /// <summary>
        /// 設定通りの行列か、またはサブクラスでは別の行列をキャンバスに設定する。
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="vc"></param>
        protected virtual void SetMatrix(SKCanvas canvas, ViewContext vc) => canvas.SetMatrix(vc.ViewTransform);
        /// <summary>
        /// 描画対象のIDrawCommandを取得する。
        /// </summary>
        /// <param name="visibleWorldRect"></param>
        /// <param name="canvasRect"></param>
        /// <param name="vc"></param>
        /// <returns></returns>
        protected virtual IEnumerable<IDrawCommand> GetTargetDrawCommands(SKRect visibleWorldRect, SKRect canvasRect, ViewContext vc)
        {
            if (visibleWorldRect.Width < WorldMetricalRect.Width / 2 || visibleWorldRect.Height < WorldMetricalRect.Height)
            {
                return DrawCommands.GetOrderedObjects(visibleWorldRect);
            }
            else
            {
                return DrawCommands.GetAllOrderedObjects();
            }
        }
        /// <summary>
        /// 描画を実行する
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="vc"></param>
        /// <param name="visibleWorldRect"></param>
        /// <param name="canvasRect"></param>
        public virtual void DrawLayer(SKCanvas canvas, ViewContext vc, SKRect visibleWorldRect, SKRect canvasRect)
        {

            int i = 0;
            SetMatrix(canvas, vc);

            var sw = Stopwatch.StartNew();
            if (Monitor.TryEnter(DrawObjectsLock, 10))
            {
                try
                {
                    IEnumerable<IDrawCommand>? commands = GetTargetDrawCommands(visibleWorldRect, canvasRect, vc);

                    foreach (var cmd in commands)
                    {
                        if (cmd is PathDrawCommand pcmd)
                        {
                            pcmd.Draw(canvas, vc.ScaleFactor, vc.WorldScaleFactor);
                        }
                        else if (cmd is BitmapDrawCommand bcmd)
                        {
                            bcmd.Draw(canvas);
                        }
                        else if (cmd is DelegateDrawCommand dcmd)
                        {
                            dcmd.Draw(canvas, visibleWorldRect);
                        }
                        else if (cmd is TextDrawCommand tcmd)
                        {
                            tcmd.Draw(canvas, vc.ScaleFactor, vc.WorldScaleFactor);
                        }
                        else if (cmd is SKPictureDrawCommand ccmd)
                        {
                            ccmd.Draw(canvas, vc.ScaleFactor, vc.WorldScaleFactor);
                        }
                        else if (cmd is TextOnPathDrawCommand topcmd)
                        {
                            topcmd.Draw(canvas, vc.ScaleFactor, vc.WorldScaleFactor);
                        }
                        i++;
                    }
                }
                finally
                {
                    Monitor.Exit(DrawObjectsLock);
                }
            }
            else
            {
                Debug.Print($"avoid lock {LayerName}");
            }
            sw.Stop();
            Debug.Print($"Layer[{LayerName}].Draw Commands={i} {sw.ElapsedMilliseconds}ms");
            canvas.ResetMatrix();

        }


        #region dispose

        private bool disposedValue;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)

                    DrawCommands.ClearContent(true);
                    DrawCommands = null!;
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // TODO: 大きなフィールドを null に設定します
                disposedValue = true;
            }
        }

        // // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
        // ~RoadsLayer()
        // {
        //     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        //     Dispose(disposing: false);
        // }

        /// <inheritdoc/>
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

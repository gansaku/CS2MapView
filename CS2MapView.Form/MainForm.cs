using CS2MapView.Data;
using CS2MapView.Drawing;
using CS2MapView.Form.Command;
using CS2MapView.Import;
using CS2MapView.Util;
using Gfw.Common;
using log4net;
using OpenTK.Graphics.ES20;
using SkiaSharp;
using System.Diagnostics;
using System.Drawing.Text;
using System.Security.Cryptography;

namespace CS2MapView.Form;
/// <summary>
/// メインフォーム
/// </summary>
public partial class MainForm : System.Windows.Forms.Form, ICS2MapViewRoot
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(MainForm));
    private FileCommands FileCommands { get; init; }
    private HelpCommands HelpCommands { get; init; }
    /// <summary>
    /// 描画コンテキスト
    /// </summary>
    public RenderContext Context { get; init; }
    /// <summary>
    /// マップデータ
    /// </summary>
    public MapData? MapData { get; set; }

    private static readonly float[] WheelScalesCandidate = [16f, 14f, 12f, 10f, 8f, 6f, 4f, 3f, 2f, 1.75f, 1.5f, 1.25f, 1.1f, 1.0f, 0.9f, 0.8f, 0.7f, 0.6f, 0.5f, 0.4f, 0.3f, 0.25f, 0.2f, 0.15f, 0.1f];

    #region form init & close
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public MainForm()
    {
        Context = new RenderContext(Config.UserSettings.LoadOrDefault(true));
        InitializeComponent();
        FileCommands = new FileCommands(this);
        HelpCommands = new HelpCommands(this);

        MouseWheel += MainForm_MouseWheel;
    }
    private void MainForm_Load(object sender, EventArgs e)
    {
        LayoutStatusStrip();

        Logger.Info($"available fonts={string.Join(',', SKFontManager.Default.GetFontFamilies())}");

    }
    private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        SKPaintCache.Instance.DisposeAll();
        MapSymbolPictureManager.DisposeIfInitialized();
    }
    #endregion


    /// <summary>
    /// 進捗イベントの受信
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void ReceiveProgressChanged(object sender, LoadProgressEventArgs args)
    {
        if (MainStatusStrip.InvokeRequired)
        {
            Invoke(() => { ReceiveProgressChanged(sender, args); });
            return;
        }
        if (args.Message != null)
        {
            StatusBarMessageLabel.Text = args.Message;
        }
        if (args.Progress >= 1f)
        {
            StatusBarProgressBar.Value = 0;
            StatusBarMessageLabel.Text = MapData?.MapName;
        }
        else
        {
            StatusBarProgressBar.Value = (int)(args.Progress * 100);
        }

    }

    #region paint

    internal void InvalidateSkia()
    {
        if (SkiaControl.InvokeRequired)
        {
            Invoke(SkiaControl.Invalidate);
        }
        else
        {
            SkiaControl.Invalidate();
        }
    }

    private void SkiaControl_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs e)
    {

        var canvas = e.Surface.Canvas;
        var rect = e.Info.Rect;
        if (MapData is null)
        {
            canvas.Clear(SKColors.LightGray);
            using var paint = new SKPaint(new SKFont(SKTypeface.Default, 32)) { Color = SKColors.Black, IsStroke = false, IsAntialias = true };
            float width = paint.MeasureText("CS2MapView");
            canvas.DrawText("CS2MapView", new SKPoint((rect.Width - width) / 2, rect.Height / 2 - 20), paint);
        }
        else
        {
            canvas.Clear();
            Context?.DrawLayers(MapData!, canvas, rect);
        }


    }
    #endregion
    #region key event
    private bool IsCtrlPressing = false;
    private void SkiaControl_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.ControlKey)
        {
            IsCtrlPressing = true;
        }
    }

    private void SkiaControl_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.ControlKey)
        {
            IsCtrlPressing = false;
        }

    }
    #endregion
    #region size , scroll & rotate
    private async void MainForm_MouseWheel(object? sender, MouseEventArgs e)
    {
        SKPoint mousePositionClient = new(e.X, e.Y);

        if (IsCtrlPressing && e.Delta != 0)
        {
            await WheelRotate(mousePositionClient, e.Delta);

        }
        else if (e.Delta != 0)
        {
            await WheelZoom(mousePositionClient, e.Delta);
        }
    }

    private async Task WheelRotate(SKPoint mousePositionClient, int delta)
    {
        SKPoint? mousePositionWorld = null;
        if (Context.ViewContext.ViewTransform.TryInvert(out var inverse))
        {
            mousePositionWorld = inverse.MapPoint(mousePositionClient);
        }
        if (delta < 0)
        {
            Context.ViewContext.Angle -= (float)(Math.PI / 24);
        }
        else
        {
            Context.ViewContext.Angle += (float)(Math.PI / 24);
        }

        //マウス位置を中心にズームするよう調整
        if (mousePositionWorld.HasValue)
        {
            if (Context.ViewContext.ViewTransform.TryInvert(out var inverseAfter))
            {
                var newPositionWorld = inverseAfter.MapPoint(mousePositionClient);
                var diff = new SKPoint(mousePositionWorld.Value.X - newPositionWorld.X, mousePositionWorld.Value.Y - newPositionWorld.Y);
                var matrix = SKMatrix.CreateRotation(Context.ViewContext.Angle);
                diff = matrix.MapPoint(diff);
                Context.ViewContext.ViewLeftTop = (Context.ViewContext.ViewLeftTop.x + diff.X, Context.ViewContext.ViewLeftTop.y + diff.Y);
            }
        }

        if (MapData is not null)
        {
            await MapData.RebuildLayersOnRotate(Context.ViewContext, ReceiveProgressChanged);
        }
        OnZoomOrViewPositionChanged();
        InvalidateSkia();
    }

    private async Task WheelZoom(SKPoint mousePositionClient, int delta)
    {
        SKPoint? mousePositionWorld = null;
        if (Context.ViewContext.ViewTransform.TryInvert(out var inverse))
        {
            mousePositionWorld = inverse.MapPoint(mousePositionClient);
        }

        if (delta > 0)
        {
            var prev = WheelScalesCandidate.FirstOrDefault(ws => Context.ViewContext.ScaleFactor >= ws);
            if (prev == 0f)
            {
                prev = WheelScalesCandidate.Last();
            }
            var next = WheelScalesCandidate.LastOrDefault(ws => Context.ViewContext.ScaleFactor < ws);
            if (next == 0f)
            {
                next = WheelScalesCandidate.First();
            }
            Context.ViewContext.ScaleFactor = next;
        }
        else if (delta < 0)
        {
            var prev = WheelScalesCandidate.LastOrDefault(ws => Context.ViewContext.ScaleFactor <= ws);
            if (prev == 0f)
            {
                prev = WheelScalesCandidate.First();
            }
            var next = WheelScalesCandidate.FirstOrDefault(ws => Context.ViewContext.ScaleFactor > ws);
            if (next == 0f)
            {
                next = WheelScalesCandidate.Last();
            }
            Context.ViewContext.ScaleFactor = next;
        }


        //マウス位置を中心にズームするよう調整
        if (mousePositionWorld.HasValue)
        {
            if (Context.ViewContext.ViewTransform.TryInvert(out var inverseAfter))
            {
                var newPositionWorld = inverseAfter.MapPoint(mousePositionClient);
                var diff = new SKPoint(mousePositionWorld.Value.X - newPositionWorld.X, mousePositionWorld.Value.Y - newPositionWorld.Y);
                var matrix = SKMatrix.CreateRotation(Context.ViewContext.Angle);
                diff = matrix.MapPoint(diff);
                Context.ViewContext.ViewLeftTop = (Context.ViewContext.ViewLeftTop.x + diff.X, Context.ViewContext.ViewLeftTop.y + diff.Y);
            }
        }

        OnZoomOrViewPositionChanged();
        if (MapData is not null)
        {
            await MapData.RebuildLayersOnResize(Context.ViewContext, ReceiveProgressChanged);
        }

        InvalidateSkia();
    }

    private void RefreshScrollBars(SKRect? worldBound)
    {

        static void SetEnabledScrollBarValues(ScrollBar scrollBar, float min, float max, float displayableLength, float value)
        {
            scrollBar.Enabled = true;
            scrollBar.Minimum = (int)min;
            scrollBar.Maximum = (int)max;
            scrollBar.LargeChange = (int)displayableLength;
            scrollBar.SmallChange = (int)displayableLength / 5;
            scrollBar.Value = (int)value;
        }


        var viewBound = TransformedBound(SkiaControl.DisplayRectangle);
        if (!viewBound.HasValue || !worldBound.HasValue)
        {
            HScrollBar.Enabled = false;
            VScrollBar.Enabled = false;
        }
        else
        {
            //表示できるサイズ
            var (displayableWidth, displayableHeight) = ViewSizeInWorldScale();

            if (displayableWidth >= worldBound.Value.Width)
            {
                HScrollBar.Enabled = false;
                Context.ViewContext.ViewLeftTop = (worldBound.Value.Left, Context.ViewContext.ViewLeftTop.y);
            }
            else
            {
                SetEnabledScrollBarValues(HScrollBar,
                    worldBound.Value.Left, worldBound.Value.Right, displayableWidth, Context.ViewContext.ViewLeftTop.x);
            }

            if (displayableHeight >= worldBound.Value.Height)
            {
                VScrollBar.Enabled = false;
                Context.ViewContext.ViewLeftTop = (Context.ViewContext.ViewLeftTop.x, worldBound.Value.Top);
            }
            else
            {
                SetEnabledScrollBarValues(VScrollBar,
                    worldBound.Value.Top, worldBound.Value.Bottom, displayableHeight, Context.ViewContext.ViewLeftTop.y);

            }

        }
    }
    /// <summary>
    /// ズーム、フォームサイズが変更された、マップが読み込まれた、位置が移動された等の際の処理。
    /// ただし、スクロールバー移動時には呼んではいけません。
    /// </summary>
    internal void OnZoomOrViewPositionChanged()
    {
        if (InvokeRequired)
        {
            Invoke(OnZoomOrViewPositionChanged);
        }
        else
        {
            //表示できるサイズ
            var (width, height) = ViewSizeInWorldScale();
            var worldSize = RotatedBound(Context.ViewContext.WorldRect);

            var xMax = Math.Max(worldSize.Left, worldSize.Right - width);
            var yMax = Math.Max(worldSize.Top, worldSize.Bottom - height);
            //     Debug.Print($"OnZoomOrViewPositionChanged:xmax={xMax} ymax={yMax}");
            Context.ViewContext.ViewLeftTop = (Math.Clamp(Context.ViewContext.ViewLeftTop.x, worldSize.Left, xMax),
                Math.Clamp(Context.ViewContext.ViewLeftTop.y, worldSize.Top, yMax));
            //       Debug.Print($"Context.ViewLeftTop ={Context.ViewLeftTop}");

            RefreshScrollBars(worldSize);
            RefreshDisplayInfoLabel();
        }
    }
    private (float width, float height) ViewSizeInWorldScale()
    {
        var r = 1f / (Context.ViewContext.ScaleFactor * Context.ViewContext.WorldScaleFactor);
        return (SkiaControl.DisplayRectangle.Width * r, SkiaControl.DisplayRectangle.Height * r);

    }
    private void RefreshDisplayInfoLabel()
    {
        StatusBarDisplayInfoLabel.Text = $"{Context.ViewContext.ScaleFactor * 100:0}%  {Context.ViewContext.Angle * 180 / Math.PI:0.0}°";

    }

    private SKRect? TransformedBound(Rectangle r)
    {
        if (Context.ViewContext.ViewTransform.TryInvert(out var inverse))
        {
            return SKPathEx.GetTransformedRectBound(new SKRect(r.Left, r.Top, r.Right, r.Bottom), inverse);
        }
        return null;
    }
    private SKRect RotatedBound(SKRect r)
    {
        SKMatrix mat = SKMatrix.CreateRotation(Context.ViewContext.Angle).Invert();
        return SKPathEx.GetTransformedRectBound(r, mat);
    }


    private void VScrollBar_Scroll(object sender, ScrollEventArgs e)
    {
        if (e.OldValue == e.NewValue)
        {
            return;
        }

        Context.ViewContext.ViewLeftTop = (Context.ViewContext.ViewLeftTop.x, (float)e.NewValue);
        RefreshDisplayInfoLabel();
        InvalidateSkia();
    }

    private void HScrollBar_Scroll(object sender, ScrollEventArgs e)
    {
        if (e.OldValue == e.NewValue)
        {
            return;
        }
        Context.ViewContext.ViewLeftTop = (e.NewValue, Context.ViewContext.ViewLeftTop.y);
        RefreshDisplayInfoLabel();
        InvalidateSkia();
    }
    private void LayoutStatusStrip()
    {

        int width = MainStatusStrip.Width;
        StatusBarMessageLabel.Width = width - StatusBarProgressBar.Width - StatusBarDisplayInfoLabel.Width - 30;
    }
    private void MainStatusStrip_SizeChanged(object sender, EventArgs e) => LayoutStatusStrip();



    private void MainForm_SizeChanged(object sender, EventArgs e)
    {

        OnZoomOrViewPositionChanged();
    }
    #endregion

    #region mouse

    private Point? MousePrevLocation = null;

    private void SkiaControl_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            Cursor = Cursors.NoMove2D;
            MousePrevLocation = e.Location;
        }
    }

    private void CalcMouseMove(MouseEventArgs e)
    {
        int dx = e.X - MousePrevLocation!.Value.X;
        int dy = e.Y - MousePrevLocation!.Value.Y;

        Context.ViewContext.ViewLeftTop = (Context.ViewContext.ViewLeftTop.x - dx / Context.ViewContext.ViewScaleFromWorld, Context.ViewContext.ViewLeftTop.y - dy / Context.ViewContext.ViewScaleFromWorld);
        //   Debug.Print($"dx,dy={dx},{dy} {Context.ViewLeftTop}");
        OnZoomOrViewPositionChanged();
        InvalidateSkia();
        //   Debug.Print($"after=  {Context.ViewLeftTop}");
    }

    private void SkiaControl_MouseMove(object sender, MouseEventArgs e)
    {
        if (MousePrevLocation.HasValue && MapData is not null)
        {
            CalcMouseMove(e);
            MousePrevLocation = e.Location;

        }
    }

    private void SkiaControl_MouseUp(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left && MousePrevLocation.HasValue)
        {
            CalcMouseMove(e);
            Cursor = Cursors.Default;
            MousePrevLocation = null;
        }
    }
    #endregion

    #region file menu
    /// <summary>
    /// ファイルをオープンします。ファイル選択後の処理は元スレッドをブロックしません。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OpenFileMenuItem_Click(object sender, EventArgs e) => await FileCommands.OpenFileAsync();

    private void CloseMenuItem_Click(object sender, EventArgs e) => Close();

    #endregion
    #region view menu
    private void OptionMenuItem_Click(object sender, EventArgs e)
    {
        ConfigForm cf = new() { AppRoot = this };
        cf.ShowDialog();

    }
    private void SaveImageMenuItem_Click(object sender, EventArgs e)
    {
        var form = new SaveImageForm() { AppRoot = this };
        form.ShowDialog();
    }
    #endregion
    #region help menu
    private void VersionInfoMenuItem_Click(object sender, EventArgs e) => HelpCommands.ShowVersionInfo();
    private void GitHubMenuItem_Click(object sender, EventArgs e)
    {
        using (Process p = new Process())
        {
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = "https://github.com/gansaku/CS2MapView";
            p.Start();
        }
            
    }
    #endregion
    #region filedrop

    private void MainForm_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data?.GetDataPresent(DataFormats.FileDrop) ?? false)
        {
            e.Effect = DragDropEffects.Copy;
        }
        else
        {
            e.Effect = DragDropEffects.None;
        }

    }

    private async void MainForm_DragDrop(object sender, DragEventArgs e)
    {
        var d = e.Data?.GetData(DataFormats.FileDrop, false);
        var path = d as string[];
        if (path is null)
        {
            return;
        }
        await new ExportFileImporter(this).ImportAsync(path[0]);
        OnZoomOrViewPositionChanged();
        InvalidateSkia();

        Context.UserSettings.OpenFileDir = Path.GetDirectoryName(path[0])!;
        await Context.UserSettings.SaveAsync();

    }
    #endregion


 
}
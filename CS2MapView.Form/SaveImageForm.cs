
using CS2MapView.Drawing;
using CS2MapView.Theme;
using CS2MapView.Util;
using Gfw.Common;
using SkiaSharp;
using SD = System.Drawing;

namespace CS2MapView.Form
{
    public partial class SaveImageForm : System.Windows.Forms.Form
    {
        public required ICS2MapViewRoot AppRoot { get; init; }

        private SKBitmap? MapImage;

        private SKRect PrintArea;

        private const int MinImageWidth = 32;
        private const int MaxImageWidth = 16384;
        private const int DefaultImageWidth = 2048;
        private const int PrintAreaMinWidth = 20;
        private const int PrintAreaMinHeight = 20;

        public SaveImageForm()
        {
            InitializeComponent();
        }

        private async void SaveImageForm_Load(object sender, EventArgs e)
        {
            var rect = new SKRect(0, 0, SubSkiaControl.DisplayRectangle.Width, SubSkiaControl.DisplayRectangle.Height);
            MapImage = new SKBitmap(SubSkiaControl.DisplayRectangle.Width, SubSkiaControl.DisplayRectangle.Height);
            var canvas = new SKCanvas(MapImage);
            canvas.Clear();
            if (AppRoot.MapData is not null)
            {
                var vc = AppRoot.Context.ViewContext.Clone();


                var worldSize = SKPathEx.GetTransformedRectBound(vc.WorldRect, SKMatrix.CreateRotation(vc.Angle));
                vc.ScaleFactor = (rect.Width / worldSize.Width) / vc.WorldScaleFactor;


                vc.ViewLeftTop = (worldSize.Left, worldSize.Top);

                await AppRoot.MapData.RebuildLayersOnResizeOrRotate(vc, null);
                AppRoot.Context.DrawLayers(AppRoot.MapData, vc, canvas, rect);

            }
            PrintArea = rect;
            var size = AppRoot.Context.UserSettings.OutputImageSize;
            if (size == 0)
            {
                size = DefaultImageWidth;
            }
            size = Math.Clamp(size, MinImageWidth, MaxImageWidth);
            OutputImageSizeTextbox.Text = $"{size}";
            RefreshPrintAreaTextboxes();

            OutputFileFormatCombobox.Items.AddRange(["png", "bmp", "jpeg", "svg"]);
            OutputFileFormatCombobox.Text = AppRoot.Context.UserSettings.OutputImageFormat ?? "png";
            if (AppRoot.Context.UserSettings.SaveImageDir is not null && Directory.Exists(AppRoot.Context.UserSettings.SaveImageDir))
            {
                OutputPathTextbox.Text = AppRoot.Context.UserSettings.SaveImageDir;
            }
            else
            {
                OutputPathTextbox.Text = AppContext.BaseDirectory;
            }
            string? mapName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(AppRoot.MapData?.FileName))?? AppRoot.MapData?.MapName;
            OutputFileNameTextbox.Text = $"{mapName}.png";
        }

        private void RefreshPrintAreaTextboxes()
        {
            PrintAreaLeftTextbox.Text = $"{PrintArea.Left}";
            PrintAreaTopTextbox.Text = $"{PrintArea.Top}";
            PrintAreaRightTextbox.Text = $"{PrintArea.Right}";
            PrintAreaBottomTextbox.Text = $"{PrintArea.Bottom}";

            var outputSize = OutputImageSizeTextbox.Text.ParseIntInvariant();
            if (outputSize.HasValue)
            {
                float rate = PrintArea.Height / PrintArea.Width;
                if (rate >= 1f)
                {
                    OutputImageSizeLabel.Text = $"{(int)(outputSize.Value / rate)}x{outputSize.Value}";
                }
                else
                {
                    OutputImageSizeLabel.Text = $"{outputSize.Value}x{(int)(outputSize.Value * rate)}";
                }


            }
        }

        private void SubSkiaControl_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            //  var rect = e.Info.Rect;
            canvas.Clear();
            if (MapImage is not null)
            {
                canvas.DrawBitmap(MapImage, 0, 0);
            }
            canvas.DrawRect(PrintArea, SKPaintCache.Instance.GetStroke(new SKPaintCache.StrokeKey(2f, SKColors.Red, StrokeType.Flat)));
            canvas.DrawRect(PrintArea, SKPaintCache.Instance.GetFill(new SKColor(100, 255, 100, 80)));

        }

        private void SaveImageForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            MapImage?.Dispose();
        }

        #region printarea
        private (MoveType moveType, Cursor cursor) CalcCursor(Point mousePosition)
        {
            const int marginOuter = 2;
            const int marginInner = 5;
            if (mousePosition.X >= PrintArea.Left - marginOuter && mousePosition.X <= PrintArea.Left + marginInner)
            {
                if (mousePosition.Y >= PrintArea.Top - marginOuter && mousePosition.Y <= PrintArea.Top + marginInner)
                {
                    return (MoveType.LeftTop, Cursors.SizeNWSE);
                }
                else if (mousePosition.Y >= PrintArea.Bottom - marginInner && mousePosition.Y <= PrintArea.Bottom + marginOuter)
                {
                    return (MoveType.LeftBottom, Cursors.SizeNESW);
                }
                else
                {
                    return (MoveType.Left, Cursors.SizeWE);
                }
            }
            else if (mousePosition.X >= PrintArea.Right - marginOuter && mousePosition.X <= PrintArea.Right + marginInner)
            {
                if (mousePosition.Y >= PrintArea.Top - marginOuter && mousePosition.Y <= PrintArea.Top + marginInner)
                {
                    return (MoveType.RightTop, Cursors.SizeNESW);
                }
                else if (mousePosition.Y >= PrintArea.Bottom - marginInner && mousePosition.Y <= PrintArea.Bottom + marginOuter)
                {
                    return (MoveType.RightBottom, Cursors.SizeNWSE);
                }
                else
                {
                    return (MoveType.Right, Cursors.SizeWE);
                }
            }
            else if (mousePosition.Y >= PrintArea.Top - marginOuter && mousePosition.Y <= PrintArea.Top + marginInner)
            {
                return (MoveType.Top, Cursors.SizeNS);
            }
            else if (mousePosition.Y >= PrintArea.Bottom - marginInner && mousePosition.Y <= PrintArea.Bottom + marginOuter)
            {
                return (MoveType.Bottom, Cursors.SizeNS);
            }
            else if (mousePosition.X >= PrintArea.Left && mousePosition.X <= PrintArea.Right && mousePosition.Y >= PrintArea.Top && mousePosition.Y <= PrintArea.Bottom)
            {
                return (MoveType.Move, Cursors.Hand);
            }
            else
            {
                return (MoveType.None, Cursors.Default);
            }
        }

        private void SubSkiaControl_MouseMove(object sender, MouseEventArgs e)
        {

            MouseUpOrMove(e);


            Cursor = CalcCursor(e.Location).cursor;



        }

        private void MouseUpOrMove(MouseEventArgs e)
        {

            if (!LastMouseMovePoint.HasValue)
            {
                return;
            }
            int dx = e.X - LastMouseMovePoint.Value.X;
            int dy = e.Y - LastMouseMovePoint.Value.Y;
            var rect = PrintArea;
            if (CurrentMoveType.HasFlag(MoveType.Left))
            {
                rect.Left += dx;
                if (rect.Width < PrintAreaMinWidth)
                {
                    rect.Left = rect.Right - PrintAreaMinWidth;
                }
            }
            if (CurrentMoveType.HasFlag(MoveType.Right))
            {
                rect.Right += dx;
                if (rect.Width < PrintAreaMinWidth)
                {
                    rect.Right = rect.Left + PrintAreaMinWidth;
                }
            }
            if (CurrentMoveType.HasFlag(MoveType.Top))
            {
                rect.Top += dy;
                if (rect.Height < PrintAreaMinHeight)
                {
                    rect.Top = rect.Bottom - PrintAreaMinHeight;
                }
            }
            if (CurrentMoveType.HasFlag(MoveType.Bottom))
            {
                rect.Bottom += dy;
                if (rect.Height < PrintAreaMinHeight)
                {
                    rect.Bottom = rect.Top + PrintAreaMinHeight;
                }
            }

            if (rect.Left < 0)
            {
                rect.Left = 0;
                if (rect.Width < PrintAreaMinWidth)
                {
                    rect.Right = PrintAreaMinWidth;
                }
            }
            if (rect.Right > SubSkiaControl.DisplayRectangle.Width)
            {
                rect.Right = SubSkiaControl.DisplayRectangle.Width;
                if (rect.Width < PrintAreaMinWidth)
                {
                    rect.Left = rect.Right - PrintAreaMinWidth;
                }
            }
            if (rect.Top < 0)
            {
                rect.Top = 0;
                if (rect.Height < PrintAreaMinHeight)
                {
                    rect.Bottom = PrintAreaMinHeight;
                }
            }
            if (rect.Bottom > SubSkiaControl.DisplayRectangle.Height)
            {
                rect.Bottom = SubSkiaControl.DisplayRectangle.Height;
                if (rect.Height < PrintAreaMinHeight)
                {
                    rect.Top = rect.Bottom - PrintAreaMinHeight;
                }
            }


            PrintArea = rect;
            RefreshPrintAreaTextboxes();
            LastMouseMovePoint = e.Location;
            SubSkiaControl.Invalidate();
        }

        private Point? LastMouseMovePoint;
        private MoveType CurrentMoveType = MoveType.None;
        private enum MoveType
        {
            None = 0x0,
            Left = 0x1,
            Right = 0x2,
            Top = 0x4,
            Bottom = 0x8,
            LeftTop = Left | Top,
            RightTop = Right | Top,
            LeftBottom = Left | Bottom,
            RightBottom = Right | Bottom,
            Move = Left | Right | Top | Bottom
        }

        private void SubSkiaControl_MouseDown(object sender, MouseEventArgs e)
        {
            LastMouseMovePoint = e.Location;
            CurrentMoveType = CalcCursor(e.Location).moveType;
        }

        private void SubSkiaControl_MouseUp(object sender, MouseEventArgs e)
        {

            MouseUpOrMove(e);

            LastMouseMovePoint = null;
            CurrentMoveType = MoveType.None;
        }
        #endregion

        private void SubSkiaControl_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void PrintAreaTextboxs_Leave(object sender, EventArgs e)
        {
            if (sender is not TextBox t)
            {
                return;
            }
            bool vertical = t == PrintAreaTopTextbox || t == PrintAreaBottomTextbox;
            int max = vertical ? SubSkiaControl.DisplayRectangle.Height : SubSkiaControl.DisplayRectangle.Width;

            var val = t.Text.Trim().ParseIntInvariant();
            if (!val.HasValue)
            {
                if (t.Name == PrintAreaTopTextbox.Name)
                {
                    val = (int)PrintArea.Top;

                }
                else if (t.Name == PrintAreaLeftTextbox.Name)
                {
                    val = (int)PrintArea.Left;
                }
                else if (t.Name == PrintAreaRightTextbox.Name)
                {
                    val = (int)PrintArea.Right;
                }
                else
                {
                    val = (int)PrintArea.Bottom;
                }
                t.Text = $"{val}";
            }
            else
            {
                if (val.Value < 0)
                {
                    t.Text = "0";

                }
                else if (val.Value > max)
                {
                    t.Text = $"{max}";

                }
            }


            var left = PrintAreaLeftTextbox.Text.ParseIntInvariant();
            var right = PrintAreaRightTextbox.Text.ParseIntInvariant();
            var top = PrintAreaTopTextbox.Text.ParseIntInvariant();
            var bottom = PrintAreaBottomTextbox.Text.ParseIntInvariant();
            if (left > right)
            {
                if (right < PrintAreaMinWidth)
                {
                    PrintAreaRightTextbox.Text = $"{PrintAreaMinWidth}";
                    PrintAreaLeftTextbox.Text = "0";
                }
                else
                {
                    PrintAreaLeftTextbox.Text = $"{right - PrintAreaMinWidth}";
                }
            }
            if (top > bottom)
            {
                if (bottom < PrintAreaMinHeight)
                {
                    PrintAreaBottomTextbox.Text = $"{PrintAreaMinHeight}";
                    PrintAreaTopTextbox.Text = "0";
                }
                else
                {
                    PrintAreaTopTextbox.Text = $"{bottom - PrintAreaMinHeight}";
                }

            }

            if (!PrintAreaLeftTextbox.Text.ParseIntInvariant().HasValue
                || !PrintAreaTopTextbox.Text.ParseIntInvariant().HasValue
                || !PrintAreaRightTextbox.Text.ParseIntInvariant().HasValue
                || !PrintAreaBottomTextbox.Text.ParseIntInvariant().HasValue)
            {
                return;
            }
            PrintArea = new(PrintAreaLeftTextbox.Text.ParseIntInvariant()!.Value,
                PrintAreaTopTextbox.Text.ParseIntInvariant()!.Value,
                PrintAreaRightTextbox.Text.ParseIntInvariant()!.Value,
                PrintAreaBottomTextbox.Text.ParseIntInvariant()!.Value);

            RefreshPrintAreaTextboxes();
            SubSkiaControl.Invalidate();
        }



        private void OutputImageSizeTextbox_Leave(object sender, EventArgs e)
        {
            var val = OutputImageSizeTextbox.Text?.ParseIntInvariant();
            if (!val.HasValue)
            {
                OutputImageSizeTextbox.Text = $"{DefaultImageWidth}";
            }
            else
            {
                if (val.Value < MinImageWidth)
                {
                    OutputImageSizeTextbox.Text = $"{MinImageWidth}";
                }
                else if (val.Value > MaxImageWidth)
                {
                    OutputImageSizeTextbox.Text = $"{MaxImageWidth}";
                }
            }
            RefreshPrintAreaTextboxes();
        }

        private void OpenFileDialogButton_Click(object sender, EventArgs e)
        {
            var dir = string.IsNullOrEmpty(OutputPathTextbox.Text) ? AppContext.BaseDirectory : Path.GetDirectoryName(OutputPathTextbox.Text);

            var sfd = new SaveFileDialog()
            {
                AddExtension = true,
                AddToRecent = true,
                Filter = $"*.{OutputFileFormatCombobox.Text}|*.{OutputFileFormatCombobox.Text}",
                InitialDirectory = dir,

            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                OutputPathTextbox.Text = Path.GetDirectoryName(sfd.FileName);
                OutputFileNameTextbox.Text = Path.GetFileName(sfd.FileName);
            }
        }

        private unsafe static Bitmap ToWinBitmap(SKBitmap bitmap)
        {

            Bitmap winBmp = new(bitmap.Width, bitmap.Height, SD.Imaging.PixelFormat.Format32bppArgb);
            var bitmapData = winBmp.LockBits(new Rectangle(0, 0, winBmp.Width, winBmp.Height), SD.Imaging.ImageLockMode.ReadWrite, winBmp.PixelFormat);
            Span<byte> lines = new(bitmapData.Scan0.ToPointer(), bitmapData.Height * bitmapData.Stride);
            //中身同じであることを前提とする
            ReadOnlySpan<byte> origSpan = new(bitmap.GetPixels().ToPointer(), bitmapData.Height * bitmapData.Stride);
            origSpan.CopyTo(lines);
            winBmp.UnlockBits(bitmapData);
            return winBmp;
        }

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            SaveButton.Enabled = false;
            try
            {
                if (AppRoot.MapData is null)
                {
                    //TODO
                    MessageBox.Show($"Map was not loaded.");
                    return;
                }
                var outputSize = OutputImageSizeTextbox.Text.ParseIntInvariant();
                var format = OutputFileFormatCombobox.Text;

                if (string.IsNullOrEmpty(format))
                {
                    //TODO
                    MessageBox.Show($"Format was undefined.");
                    return;
                }
                var outDir = OutputPathTextbox.Text;
                if (string.IsNullOrEmpty(outDir))
                {
                    //TODO
                    MessageBox.Show($"File path  was undefined.");
                    return;
                }
                var outFn = OutputFileNameTextbox.Text;
                if (string.IsNullOrEmpty(outFn))
                {
                    //TODO
                    MessageBox.Show($"File name was undefined.");
                    return;
                }
                var fn = Path.Combine(outDir, outFn);

                SKRect imageSize;
                if (outputSize.HasValue)
                {
                    float rate = PrintArea.Height / PrintArea.Width;
                    if (rate >= 1f)
                    {
                        imageSize = new(0, 0, (int)(outputSize.Value / rate), outputSize.Value);
                    }
                    else
                    {
                        imageSize = new(0, 0, outputSize.Value, (int)(outputSize.Value * rate));
                    }

                }
                else
                {
                    //TODO
                    MessageBox.Show($"Image size was undefined");
                    return;
                }
                var rotationTransform = SKMatrix.CreateRotation(AppRoot.Context.ViewContext.Angle);

                var rotatedWorldRect = SKPathEx.GetTransformedRectBound(AppRoot.Context.ViewContext.WorldRect, rotationTransform);
                var vc = AppRoot.Context.ViewContext.Clone();
                float viewWorldWidth = rotatedWorldRect.Width * (PrintArea.Width / SubSkiaControl.DisplayRectangle.Width);
                float viewWorldHeight = rotatedWorldRect.Height * (PrintArea.Height / SubSkiaControl.DisplayRectangle.Height);
                //幅viewWorldWidthのものをimageSize.widthに入れる
                float scale = Math.Max(imageSize.Width, imageSize.Height) / Math.Max(viewWorldWidth, viewWorldHeight);
                scale /= vc.WorldScaleFactor;
                vc.ScaleFactor = scale;
                //TODO translate
                // 角度→平行移動→倍率
                var center = Math.Min(SubSkiaControl.DisplayRectangle.Width, SubSkiaControl.DisplayRectangle.Height) / 2;
                var scale2 = rotatedWorldRect.Width / Math.Min(SubSkiaControl.DisplayRectangle.Width, SubSkiaControl.DisplayRectangle.Height);

                // var rotationTransformInvert = SKMatrix.CreateTranslation(-center, -center).PostConcat(SKMatrix.CreateScale(scale2, scale2)).PostConcat( rotationTransform.Invert());

                // var points = rotationTransformInvert.MapPoints(SKPathEx.RectToPath(PrintArea).Points);
                var leftTopPoint = SKMatrix.CreateTranslation(-center, -center).MapPoint(PrintArea.Location);
                leftTopPoint = SKMatrix.CreateScale(scale2, scale2).MapPoint(leftTopPoint);

                vc.ViewLeftTop = (leftTopPoint.X, leftTopPoint.Y);



                await AppRoot.MapData.RebuildLayersOnResizeOrRotate(vc, null);


                if (format == "svg")
                {
                    using var stream = new SKFileWStream(fn);
                    using var canvas = SKSvgCanvas.Create(imageSize, stream);
                    AppRoot.Context.DrawLayers(AppRoot.MapData, vc, canvas, imageSize);
                    MessageBox.Show($"Image saved to \n{fn}");
                }
                else
                {
                    using var bitmap = new SKBitmap((int)imageSize.Width, (int)imageSize.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
                    using var canvas = new SKCanvas(bitmap);
                    AppRoot.Context.DrawLayers(AppRoot.MapData, vc, canvas, imageSize);
                    if (format == "bmp")
                    {
                        using var winBmp = ToWinBitmap(bitmap);
                        using var fs = new FileStream(fn, FileMode.Create);
                        winBmp.Save(fs, SD.Imaging.ImageFormat.Bmp);
                    }
                    else
                    {
                        using SKImage img = SKImage.FromBitmap(bitmap);
                        using var stream = new FileStream(fn, FileMode.Create);
                        SKData encodedData = format switch
                        {
                            "png" => img.Encode(SKEncodedImageFormat.Png, 100),
                            "jpeg" => img.Encode(SKEncodedImageFormat.Jpeg, 80),
                            _ => throw new InvalidOperationException("Format unknown")
                        };

                        encodedData.SaveTo(stream);
                    }


                    MessageBox.Show($"Image saved to \n{fn}");
                    AppRoot.Context.UserSettings.SaveImageDir = outDir;
                    AppRoot.Context.UserSettings.OutputImageFormat = OutputFileFormatCombobox.Text;
                    AppRoot.Context.UserSettings.OutputImageSize = OutputImageSizeTextbox.Text?.ParseIntInvariant() ?? 0;
                    await AppRoot.Context.UserSettings.SaveAsync();
                }
            }
            finally
            {
                SaveButton.Enabled = true;
            }
        }

        private async void SaveImageForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (AppRoot.MapData is not null)
            {
                await AppRoot.MapData.RebuildLayersOnResizeOrRotate(AppRoot.Context.ViewContext, null);
            }
        }

        private void OutputFileFormatCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(OutputFileNameTextbox.Text))
            {
                var ext = OutputFileFormatCombobox.Text;
                var fnbase = Path.GetFileNameWithoutExtension(OutputFileNameTextbox.Text);
                OutputFileNameTextbox.Text = $"{fnbase}.{ext}";
            }
        }
    }
}

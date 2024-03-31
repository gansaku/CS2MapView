using CS2MapView.Theme;
using SkiaSharp;

namespace CS2MapView.Drawing.Terrain
{
    internal class RasterHeightMapBuilder : ITerrainDrawingsBuiderProcedure
    {

        internal required int TerrainInputWidth { get; init; }
        internal required int TerrainInputHeight { get; init; }
        internal required int WaterInputWidth { get; init; }
        internal required int WaterInputHeight { get; init; }

        internal required float SeaLevel { get; init; }
        internal float[]? InputLandArray { get; init; }
        internal float[]? InputWaterArray { get; init; }

        internal required ReadonlyRect MetricRect { get; init; }
        internal required DrawingTheme Theme { get; init; }
        internal required bool DrawLand;
        internal required bool DrawWater;
        internal List<IDrawCommand> ResultCommands = [];

        internal required TerrainProgress Progress;

        void ITerrainDrawingsBuiderProcedure.Execute()
        {
            SKBitmap? bitmap = null;
            if (DrawLand && DrawWater)
            {
                var landBitmap = CreateLandBitmap();
                using var waterBitmap = CreateWaterBitmap();
                WriteWaterOnLand(landBitmap, waterBitmap);
                bitmap = landBitmap;
            }
            else if (DrawLand)
            {
                bitmap = CreateLandBitmap();
            }
            else if (DrawWater)
            {
                bitmap = CreateWaterBitmap();
            }
            else
            {
                throw new ArgumentException("Nothing to draw.");
            }

            var bdc = new BitmapDrawCommand { Bitmap = bitmap, TargetRect = MetricRect };
            ResultCommands.Add(bdc);
            Progress.IncrementCompleted();
        }
        private SKBitmap CreateLandBitmap()
        {
            if (InputLandArray is null)
            {
                throw new ArgumentNullException(nameof(InputLandArray));
            }
            if (Theme.Colors is null)
            {
                throw new ArgumentNullException("Theme.Colors");
            }

            unsafe
            {

                var bitmap = new SKBitmap(TerrainInputWidth, TerrainInputHeight, SKColorType.Bgra8888, SKAlphaType.Unpremul);
                var bit0 = bitmap.GetPixels();

                ReadOnlySpan<float> inputSpan = InputLandArray;
                var outPutSpan = new Span<uint>(bit0.ToPointer(), inputSpan.Length);

                for (int y = 0; y < TerrainInputHeight; y++)
                {
                    for (int x = 0; x < TerrainInputWidth; x++)
                    {
                        float height = inputSpan[y * TerrainInputWidth + x] - SeaLevel;
                        var lc = Theme.Colors.LerpLandContourColors(height).FillColor.ToBgra8888();
                        outPutSpan[(TerrainInputHeight - y - 1) * TerrainInputWidth + x] = lc;

                    }

                }
                return bitmap;
            }
        }
        private SKBitmap CreateWaterBitmap()
        {

            if (InputWaterArray is null)
            {
                throw new ArgumentNullException(nameof(InputWaterArray));
            }
            if (Theme.Colors is null)
            {
                throw new ArgumentNullException("Theme.Colors");
            }
            unsafe
            {
                var bitmap = new SKBitmap(WaterInputWidth, WaterInputHeight, SKColorType.Bgra8888, SKAlphaType.Unpremul);
                var bit0 = bitmap.GetPixels();
                var heightTotal = 0f;
                var zeroCount = 0;

                ReadOnlySpan<float> inputSpan = InputWaterArray;
                var outPutSpan = new Span<uint>(bit0.ToPointer(), inputSpan.Length);

                for (int y = 0; y < WaterInputHeight; y++)
                {
                    for (int x = 0; x < WaterInputWidth; x++)
                    {
                        float height = inputSpan[y * WaterInputWidth + x];
                        heightTotal += height;
                        if (height >= 0.2f)
                        {
                            var wc = Theme.Colors.LerpWaterContourColors(height - 0.2f).FillColor.ToBgra8888();
                            outPutSpan[(WaterInputHeight - y - 1) * WaterInputWidth + x] = wc;
                        }
                        else
                        {
                            zeroCount++;
                            outPutSpan[(WaterInputHeight - y - 1) * WaterInputWidth + x] = 0;
                        }
                    }
                }
                var avg = heightTotal / inputSpan.Length;
                return bitmap;
            }
        }


        private static void WriteWaterOnLand(SKBitmap land, SKBitmap water)
        {
            using var c = new SKCanvas(land);
            c.DrawBitmap(water, new SKRect(0, 0, land.Width, land.Height));
        }

        public IEnumerable<IDrawCommand> GetResult() => ResultCommands;
    }
}

using CS2MapView.Drawing.Layer;
using CS2MapView.Util;
using log4net;
using System.Diagnostics;

namespace CS2MapView.Drawing.Terrain;
/// <summary>
/// 地形描画情報の構築
/// </summary>
public class TerrainDrawingsBuilder
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(TerrainDrawingsBuilder));

    private interface ITerrainBuilderInternal
    {
        void Execute();
    }

    /// <summary>
    /// パラメータ
    /// </summary>
    public class Parameters
    {
        /// <summary>
        /// 入力配列の幅
        /// </summary>
        public required int TerrainInputArrayWidth;
        /// <summary>
        /// 入力配列の高さ
        /// </summary>
        public required int TerrainInputArrayHeight;
        /// <summary>
        /// 入力配列の幅
        /// </summary>
        public required int WaterInputArrayWidth;
        /// <summary>
        /// 入力配列の高さ
        /// </summary>
        public required int WaterInputArrayHeight;
        /// <summary>
        /// 出力のワールド座標範囲
        /// </summary>
        public ReadonlyRect MetricalRect;
        /// <summary>
        /// 処理前の縮小拡大率
        /// </summary>
        public float ExecuteLandScale = 1f;
        /// <summary>
        /// 処理前の縮小拡大率
        /// </summary>
        public float ExecuteWaterScale = 1f;
        /// <summary>
        /// 水面の高さ
        /// </summary>
        public required float SeaLevel;
        /// <summary>
        /// 入力標高情報
        /// </summary>
        public float[]? InputLandArray;
        /// <summary>
        /// 入力水域情報
        /// </summary>
        public float[]? InputWaterArray;
        /// <summary>
        /// 等高線描画用基準標高
        /// </summary>
        public IEnumerable<float>? LandHeights;
        /// <summary>
        /// 等高線描画用基準水深
        /// </summary>
        public IEnumerable<float>? WaterHeights;
        /// <summary>
        /// 陸地のベクトル化
        /// </summary>
        public bool VectolizeLand { get; set; } = true;
        /// <summary>
        /// 水域のベクトル化
        /// </summary>
        public bool VectolizeWater { get; set; } = true;
        /// <summary>
        /// アプリケーション
        /// </summary>
        public required ICS2MapViewRoot AppRoot { get; init; }

        internal int TempLandWidth => (int)(TerrainInputArrayWidth * ExecuteLandScale);
        internal int TempLandHeight => (int)(TerrainInputArrayHeight * ExecuteLandScale);
        internal int TempWaterWidth => (int)(WaterInputArrayWidth * ExecuteWaterScale);
        internal int TempWaterHeight => (int)(WaterInputArrayHeight * ExecuteWaterScale);
    }

    private readonly Parameters Params;

    private float[]? LandArray;
    private float[]? WaterArray;

    private TerrainProgress Progress { get; init; }

    private LoadProgressInfo? CurrentProgressInfo { get; set; }


    private void RaiseProgressChangeEvent()
    {
        if (Progress.LayersCount == 0)
        {
            CurrentProgressInfo?.Progress(this, LoadProgressInfo.Process.BuildTerrain, 0f, "Initializing terrain...");
            return;
        }
        var pg = (float)Progress.CompletedCount / Progress.LayersCount;

        CurrentProgressInfo?.Progress(this, LoadProgressInfo.Process.BuildTerrain, pg, $"Bulding contours ({Progress.CompletedCount}/{Progress.LayersCount}) {pg * 100:0.0}% done");

    }
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="param"></param>
    public TerrainDrawingsBuilder(Parameters param)
    {
        Params = param;
        Progress = new TerrainProgress();
        Progress.ProgressUpdated += RaiseProgressChangeEvent;
    }
    /// <summary>
    /// 処理の実行
    /// </summary>
    /// <param name="loadProgressInfo"></param>
    /// <returns>描画内容構築済みレイヤー</returns>
    public BasicLayer Execute(LoadProgressInfo? loadProgressInfo)
    {
        CurrentProgressInfo = loadProgressInfo;
        RaiseProgressChangeEvent();
        var sw = Stopwatch.StartNew();
        if (Params.InputLandArray != null)
        {
            if (Params.ExecuteLandScale != 1f)
            {
                LandArray = GetScaledArray(Params.InputLandArray, Params.TerrainInputArrayWidth, Params.TerrainInputArrayHeight, Params.TempLandWidth, Params.TempLandHeight);
            }
            else
            {
                LandArray = Params.InputLandArray;
            }
        }
        if (Params.InputWaterArray != null)
        {
            if (Params.ExecuteWaterScale != 1f)
            {
                WaterArray = GetScaledArray(Params.InputWaterArray, Params.WaterInputArrayWidth, Params.WaterInputArrayHeight, Params.TempWaterWidth, Params.TempWaterHeight);
            }
            else
            {
                WaterArray = Params.InputWaterArray;
            }
        }

        sw.Stop();
        Logger.Debug($"arrays initialized in {sw.ElapsedMilliseconds}ms");

        var calculators = new List<ITerrainDrawingsBuiderProcedure>();
        if (Params.VectolizeWater && WaterArray is not null && Params.WaterHeights is not null)
        {
            bool bottomAdded = false;
            foreach (var wc in Params.WaterHeights.Order())
            {
                var ac = new WaterVectorContourBuilder
                {
                    InputArray = WaterArray,
                    InputHeight = Params.TempWaterHeight,
                    InputWidth = Params.TempWaterWidth,
                    IsBottom = !bottomAdded,
                    TargetHeight = wc,
                    MetricRect = Params.MetricalRect,
                    Progress = Progress,
                    Context = Params.AppRoot.Context
                };
                calculators.Add(ac);

                bottomAdded = true;
            }

        }
        if (Params.VectolizeLand && LandArray is not null && Params.LandHeights is not null)
        {
            calculators.AddRange(Params.LandHeights.Select(lc => (ITerrainDrawingsBuiderProcedure)new LandVectorContourBuilder
            {
                InputArray = LandArray,
                InputHeight = Params.TempLandHeight,
                InputWidth = Params.TempLandWidth,
                SeaLevel = Params.SeaLevel,
                TargetHeight = lc,
                MetricRect = Params.MetricalRect,
                Progress = Progress,
                Context = Params.AppRoot.Context
            }));
        }

        if (!Params.VectolizeLand || !Params.VectolizeWater)
        {
            calculators.Add(new RasterHeightMapBuilder
            {
                InputLandArray = Params.InputLandArray,
                InputWaterArray = Params.InputWaterArray,
                DrawLand = !Params.VectolizeLand,
                DrawWater = !Params.VectolizeWater,
                TerrainInputWidth = Params.TerrainInputArrayWidth,
                TerrainInputHeight = Params.TerrainInputArrayHeight,
                WaterInputWidth = Params.WaterInputArrayWidth,
                WaterInputHeight = Params.WaterInputArrayHeight,
                Progress = Progress,
                SeaLevel = Params.SeaLevel,
                Theme = Params.AppRoot.Context.Theme,
                MetricRect = Params.MetricalRect
            });
        }
        Progress.LayersCount = calculators.Count;
        RaiseProgressChangeEvent();

        sw.Reset();
        sw.Start();
        Parallel.ForEach(calculators, (calculator, ct) => { calculator.Execute(); });
        // calculators.ForEach(c => c.Execute());
        sw.Stop();

        var result = new BasicLayer(Params.AppRoot, ILayer.LayerNameTerrain, Params.MetricalRect);

        //並び替え
        List<ITerrainDrawingsBuiderProcedure> ordered = [];
        if (!Params.VectolizeLand)
        {
            ordered.AddRange(calculators.Where(t => t is RasterHeightMapBuilder));
        }
        else
        {
            ordered.AddRange(calculators.Where(t => t is LandVectorContourBuilder).OrderBy(t => (t as LandVectorContourBuilder)?.TargetHeight));
        }
        if (!Params.VectolizeWater)
        {
            if (Params.VectolizeLand)
            {
                ordered.AddRange(calculators.Where(t => t is RasterHeightMapBuilder));
            }
        }
        else
        {
            ordered.AddRange(calculators.Where(t => t is WaterVectorContourBuilder).OrderBy(t => (t as WaterVectorContourBuilder)?.TargetHeight));
        }


        if (Params.VectolizeLand)
        {

            result.DrawCommands.Add(new PathDrawCommand { Path = SKPathEx.RectToPath(Params.MetricalRect), FillColor = Params.AppRoot.Context.Theme.Colors?.LerpLandContourColors(-30000f).FillColor });
        }
        foreach (var calculator in ordered)
        {
            result.DrawCommands.AddRange(calculator.GetResult());
        }

        Logger.Debug($"Parallel.ForEach {sw.ElapsedMilliseconds}ms");
        CurrentProgressInfo = null;
        return result;
    }

    private static float[] GetScaledArray(float[] input, int inputWidth, int inputHeight, int outputWidth, int outputHeight)
    {
        float[] result = new float[outputWidth * outputHeight];
        ReadOnlySpan<float> inputSpan = input;
        Span<float> target = result;

        float GetValue(ReadOnlySpan<float> rs, int x, int y)
        {
            float rx = (float)inputWidth / outputWidth;
            float ry = (float)inputHeight / outputHeight;

            float xstart = x * rx;
            float xend = (x + 1) * rx;
            float ystart = y * ry;
            float yend = (y + 1) * ry;

            float sum = 0f;
            float addedPixels = 0f;

            for (int sy = (int)ystart; sy < Math.Ceiling(yend); sy++)
            {
                float yrate;
                if (ystart - sy > 0)
                {
                    yrate = ystart - sy;
                }
                else if (yend - sy < 1f)
                {
                    yrate = yend - sy;
                }
                else
                {
                    yrate = 1f;
                }
                for (int sx = (int)xstart; sx < Math.Ceiling(xend); sx++)
                {
                    float xrate;
                    if (xstart - sx > 0)
                    {
                        xrate = xstart - sx;
                    }
                    else if (xend - sx < 1f)
                    {
                        xrate = xend - sx;
                    }
                    else
                    {
                        xrate = 1f;
                    }
                    sum += rs[sy * inputWidth + sx] * yrate * xrate;
                    addedPixels += yrate * xrate;
                }
            }
            return sum / addedPixels;
        }


        for (int y = 0; y < outputHeight; y++)
        {
            for (int x = 0; x < outputWidth; x++)
            {
                target[y * outputWidth + x] = GetValue(inputSpan, x, y);

            }
        }


        return result;
    }


}

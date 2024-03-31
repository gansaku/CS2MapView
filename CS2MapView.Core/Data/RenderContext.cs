using CS2MapView.Config;
using CS2MapView.Theme;
using SkiaSharp;
using System.Diagnostics.CodeAnalysis;

namespace CS2MapView.Data
{
    /// <summary>
    /// 描画コンテキスト(?)
    /// </summary>
    public class RenderContext
    {
        /// <summary>
        /// ユーザ設定
        /// </summary>
        public UserSettings UserSettings { get; set; }
        /// <summary>
        /// ビュー状態
        /// </summary>
        public ViewContext ViewContext { get; set; }
        /// <summary>
        /// テーマファイルフォルダ
        /// </summary>
        public static string ThemeDirectory => Path.Combine(AppContext.BaseDirectory, "theme");
        /// <summary>
        /// 読み込まれているマップの種類
        /// </summary>
        public SourceMapType? SourceMapType { get; set; }
        /// <summary>
        /// テーマ
        /// </summary>
        public DrawingTheme Theme { get; set; }
        /// <summary>
        /// 文字列テーマ
        /// </summary>
        public StringTheme StringTheme { get; set; }
        /// <summary>
        /// テーマ選択肢
        /// </summary>
        public IEnumerable<DrawingTheme> ThemeCandidates { get; set; }
        /// <summary>
        /// 文字列テーマ選択肢
        /// </summary>
        public IEnumerable<StringTheme> StringThemeCandidates { get; set; }
        /// <summary>
        /// 等高線間隔
        /// </summary>
        public ContourHeights ContourHeights { get; set; }
        /// <summary>
        /// 等高線間隔選択肢
        /// </summary>
        public IEnumerable<ContourHeights> ContourHeightsCandidate { get; } = ContourHeights.DefaultCandidate;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="userSettings"></param>
        public RenderContext(UserSettings userSettings)
        {
            UserSettings = userSettings;
            PrepareTheme(userSettings.Theme, userSettings.StringTheme, userSettings.ContourHeights);
            ViewContext = new();

        }
        [MemberNotNull(nameof(Theme), nameof(ThemeCandidates), nameof(ContourHeights), nameof(ContourHeightsCandidate), nameof(StringTheme), nameof(StringThemeCandidates))]
        private async void PrepareTheme(string? themeName, string? stringThemeName, string? contourHeightsName)
        {
#nullable disable
            ThemeCandidates = await PrepareThemeCandidates();
            Theme = ThemeCandidates.FirstOrDefault(tc => tc.Name == themeName, DrawingTheme.Default);
            StringThemeCandidates = await PrepareStringThemeCandidates();
            StringTheme = StringThemeCandidates.FirstOrDefault(t => t.Name == stringThemeName, StringTheme.Default);
            ContourHeights = ContourHeightsCandidate.FirstOrDefault(ch => ch.Name == contourHeightsName);
            ContourHeights ??= ContourHeights.Default;
#nullable restore
        }

        private static async Task<IEnumerable<DrawingTheme>> PrepareThemeCandidates()
        {
            if (!Path.Exists(ThemeDirectory))
            {
                Directory.CreateDirectory(ThemeDirectory);
            }
            var defaultThemeFile = Path.Combine(ThemeDirectory, "default.c2theme");
            if (!File.Exists(defaultThemeFile))
            {
                await DrawingTheme.Default.SaveAsync(defaultThemeFile);
            }
            var whiteThemeFile = Path.Combine(ThemeDirectory, "white.c2theme");
            if (!File.Exists(whiteThemeFile))
            {
                await DrawingTheme.White.SaveAsync(whiteThemeFile);
            }
            var themeCandidates = new List<DrawingTheme>();
            foreach (var f in Directory.EnumerateFiles(ThemeDirectory).Where(fn => fn.EndsWith(".c2theme", StringComparison.OrdinalIgnoreCase)))
            {
                DrawingTheme? dt = await DrawingTheme.LoadAsync(f);
                if (dt is not null)
                {
                    themeCandidates.Add(dt);
                }
            }
            return themeCandidates;
        }
        private static async Task<IEnumerable<StringTheme>> PrepareStringThemeCandidates()
        {
            if (!Path.Exists(ThemeDirectory))
            {
                Directory.CreateDirectory(ThemeDirectory);
            }
            var defaultThemeFile = Path.Combine(ThemeDirectory, "default.c2stringtheme");
            if (!File.Exists(defaultThemeFile))
            {
                await StringTheme.Default.SaveAsync(defaultThemeFile);
            }
            var themeCandidates = new List<StringTheme>();
            foreach (var f in Directory.EnumerateFiles(ThemeDirectory).Where(fn => fn.EndsWith(".c2stringtheme", StringComparison.OrdinalIgnoreCase)))
            {
                StringTheme? dt = await StringTheme.LoadAsync(f);
                if (dt is not null)
                {
                    themeCandidates.Add(dt);
                }
            }
            return themeCandidates;
        }
        /// <summary>
        /// 表示対象となっているレイヤーを描画
        /// </summary>
        /// <param name="mapData"></param>
        /// <param name="vc"></param>
        /// <param name="canvas"></param>
        /// <param name="canvasRect">キャンバスのサイズ</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void DrawLayers(MapData mapData, ViewContext vc, SKCanvas canvas, SKRect canvasRect)
        {
            var transform = vc.ViewTransform;
            canvas.ClipRect(canvasRect);

            var invertMatrix = vc.ViewTransform.Invert();
            var worldRectVisible = invertMatrix.MapRect(canvasRect);
            worldRectVisible.Right = Math.Min(vc.WorldRect.Right, worldRectVisible.Right);
            worldRectVisible.Bottom = Math.Min(vc.WorldRect.Bottom, worldRectVisible.Bottom);

            if (UserSettings.LayerDrawingOrder is null)
            {
                throw new InvalidOperationException("UserSettings.LayerDrawingOrder was null.");
            }

            var layers = mapData.Layers?.OrderBy(
                layer => UserSettings.LayerDrawingOrder.FirstOrDefault(
                    lo => lo.Name == layer.LayerName)?.Order);
            if (layers is not null)
            {
                foreach (var layer in layers)
                {
                    layer.DrawLayer(canvas, vc, worldRectVisible, canvasRect);
                }
            }
        }
        /// <summary>
        /// 表示対象となっているレイヤーをデフォルトのビュー設定で描画
        /// </summary>
        /// <param name="mapData"></param>
        /// <param name="canvas"></param>
        /// <param name="canvasRect"></param>
        public void DrawLayers(MapData mapData, SKCanvas canvas, SKRect canvasRect)
            => DrawLayers(mapData, ViewContext, canvas, canvasRect);

    }
}

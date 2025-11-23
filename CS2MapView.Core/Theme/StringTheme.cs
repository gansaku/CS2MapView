using SkiaSharp;
using System.Xml.Serialization;

namespace CS2MapView.Theme
{
    /// <summary>
    /// 文字列スタイル
    /// </summary>
    public class StringTheme
    {
        private const string FontYuGothic = "Yu Gothic";
        private const string FontMeiryo = "Meiryo";
        private const string FontMicrosoftYaHei = "Microsoft YaHei";
        private const string FontSimHei = "SimHei";

        /// <summary>
        /// スタイル名
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 建物名
        /// </summary>
        public StringStyle? BuildingName { get; set; }
        /// <summary>
        /// 地区名
        /// </summary>
        public StringStyle? DistrictName { get; set; }
        /// <summary>
        /// 道路名
        /// </summary>
        public StringStyle? StreetName { get; set; }
        /// <summary>
        /// 設定の保存
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Task SaveAsync(string path)
        {
            return Task.Run(() =>
            {
                var ser = new XmlSerializer(typeof(StringTheme));
                using var fs = new FileStream(path, FileMode.Create);
                ser.Serialize(fs, this);
            });
        }
        /// <summary>
        /// 設定の読み込み
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Task<StringTheme?> LoadAsync(string path)
        {
            return Task.Run(() =>
            {
                var ser = new XmlSerializer(typeof(StringTheme));
                using var fs = new FileStream(path, FileMode.Open);
                return ser.Deserialize(fs) as StringTheme;
            });
        }
        /// <summary>
        /// 初期設定値
        /// </summary>
        [XmlIgnore]
        public static StringTheme Default => new()
        {
            Name = "default",
            BuildingName = new(GetDefaultStringThemeFontFamily( FontYuGothic), new Width(12.5f, 1f, 12.5f, 12.5f, 8f), new StrokeStyle(new(3f, 1f, 3f, 3f, 0f))),
            DistrictName = new(GetDefaultStringThemeFontFamily( FontMeiryo), new Width(18f, 1f, 18f, 18f, 8f), new StrokeStyle(new(4f, 1f, 4f, 4f, 0f))),
            StreetName = new(GetDefaultStringThemeFontFamily( FontYuGothic), new Width(12f, 1f, 12f, 12f, 8f), new StrokeStyle(new(2f, 1f, 2f, 2f, 0f)))
        };

        /// <summary>
        /// カスタムフォント設定を適用
        /// </summary>
        /// <param name="buildingFont">建物名のフォント</param>
        /// <param name="districtFont">地区名のフォント</param>
        /// <param name="streetFont">道路名のフォント</param>
        public void ApplyCustomFonts(string? buildingFont, string? districtFont, string? streetFont)
        {
            if (!string.IsNullOrEmpty(buildingFont) && BuildingName != null)
            {
                BuildingName.FontFamily = GetSafeFontFamily(buildingFont);
            }
            if (!string.IsNullOrEmpty(districtFont) && DistrictName != null)
            {
                DistrictName.FontFamily = GetSafeFontFamily(districtFont);
            }
            if (!string.IsNullOrEmpty(streetFont) && StreetName != null)
            {
                StreetName.FontFamily = GetSafeFontFamily(streetFont);
            }
        }

        private static string GetSafeFontFamily(string fontFamily)
        {
            var tf = SKFontManager.Default.MatchFamily(fontFamily);
            if (tf is null)
            {
                return SKTypeface.Default.FamilyName;
            }
            else
            {
                return fontFamily;
            }
        }

        private static string[] SimplifiedChineseLocale = { "zh-Hans", "zh", "zh-CN", "zh-SG" };
        private static bool? _PreferSimplifiedChineseFont;
        private static bool PreferSimplifiedChineseFont => _PreferSimplifiedChineseFont ??= SimplifiedChineseLocale.Contains(System.Globalization.CultureInfo.CurrentCulture.Name);

        /// <summary>
        /// 最適なフォントを取得します。優先フォントがあればそれを使用し、
        /// なければ代替フォントを試します。
        /// 簡体字中国語ロケールの場合はYaheiを優先して返します。
        /// </summary>
        /// <param name="fontfamily">フォント名</param>
        /// <returns>利用可能なフォント名</returns>
        private static string GetDefaultStringThemeFontFamily(string fontfamily)
        {

            SKTypeface? tf = null;
            if (PreferSimplifiedChineseFont)
            {
                tf = SKFontManager.Default.MatchFamily(FontMicrosoftYaHei);
                if (tf is not null)
                {
                    return FontMicrosoftYaHei;
                }
            }


            tf = SKFontManager.Default.MatchFamily(fontfamily);
            if (tf is not null)
            {
                return fontfamily;
            }

            // どちらも利用できない場合は、システムのデフォルトフォントを返す
            return SKTypeface.Default.FamilyName;
        }

        /// <summary>
        /// システムで利用可能なすべてのフォント一覧を取得 / Get all available system fonts
        /// </summary>
        /// <returns>フォント名のリスト</returns>
        public static List<string> GetAvailableFonts()
        {
            var fonts = new HashSet<string>();
            var fontManager = SKFontManager.Default;

            // すべてのフォントファミリーを取得
            var familyCount = fontManager.FontFamilyCount;
            for (int i = 0; i < familyCount; i++)
            {
                var familyName = fontManager.GetFamilyName(i);
                if (!string.IsNullOrEmpty(familyName))
                {
                    fonts.Add(familyName);
                }
            }

            var result = fonts.OrderBy(f => f).ToList();

            var commonChineseFonts = new[] {
                FontMicrosoftYaHei,
                FontSimHei,
                "SimSun",
                "NSimSun",
                "Microsoft JhengHei",
                FontYuGothic,
                FontMeiryo
            };

            foreach (var font in commonChineseFonts)
            {
                if (SKFontManager.Default.MatchFamily(font) != null && !result.Contains(font))
                {
                    result.Insert(0, font);
                }
            }

            return result;
        }
    }
}

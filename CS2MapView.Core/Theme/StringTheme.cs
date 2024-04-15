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
            BuildingName = new(GetSafeFontFamily(FontYuGothic), new Width(12.5f, 1f, 12.5f, 12.5f, 8f), new StrokeStyle(new(3f, 1f, 3f, 3f, 0f))),
            DistrictName = new(GetSafeFontFamily(FontMeiryo), new Width(18f, 1f, 18f, 18f, 8f), new StrokeStyle(new(4f, 1f, 4f, 4f, 0f))),
            StreetName = new(GetSafeFontFamily(FontYuGothic), new Width(12f, 1f, 12f, 12f, 8f), new StrokeStyle(new(2f, 1f, 2f, 2f, 0f)))
        };

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
    }
}

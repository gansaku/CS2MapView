using CS2MapView.Drawing;

namespace CS2MapView.Theme
{
    /// <summary>
    /// 文字列描画のスタイル
    /// </summary>
    public class StringStyle
    {
        /// <summary>
        /// フォント
        /// </summary>
        public string? FontFamily { get; set; }
        /// <summary>
        /// 輪郭
        /// </summary>
        public StrokeStyle? Stroke { get; set; }
        /// <summary>
        /// フォントサイズ
        /// </summary>
        public Width FontSize { get; set; }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public StringStyle() { }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="fontFamily"></param>
        /// <param name="fontSize"></param>
        /// <param name="stroke"></param>
        public StringStyle(string fontFamily, Width fontSize, StrokeStyle? stroke)
        {
            FontFamily = fontFamily;
            FontSize = fontSize;
            Stroke = stroke;
        }
        /// <summary>
        /// SKPaintCache用のキー
        /// </summary>
        /// <param name="viewScale"></param>
        /// <param name="worldScale"></param>
        /// <returns></returns>
        public SKPaintCache.FontKey? ToFontCacheKey(float viewScale, float worldScale)
        {
            var w = FontSize.StrokeWidthByScale(viewScale, worldScale);

            if (FontFamily is null)
            {
                return null;
            }
            if (!w.HasValue)
            {
                return null;
            }

            return new SKPaintCache.FontKey(FontFamily, w.Value);
        }
    }
    /// <summary>
    /// 文字列描画スタイル＋色
    /// </summary>
    public class TextStyleWithColor : StringStyle
    {
        /// <summary>
        /// 塗り
        /// </summary>
        public SerializableColor? FillColor { get; set; }
        /// <summary>
        /// 輪郭
        /// </summary>
        public SerializableColor? BorderColor { get; set; }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TextStyleWithColor() { }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TextStyleWithColor(StringStyle ts, SerializableColor? fillColor, SerializableColor? borderColor)
        {
            FillColor = fillColor;
            BorderColor = borderColor;
            FontFamily = ts.FontFamily;
            FontSize = ts.FontSize;
            Stroke = ts.Stroke;
        }
        /// <summary>
        /// SKPaintCache用のキー
        /// </summary>
        /// <param name="viewScale">画面表示時のユーザ指定の倍率</param>
        /// <param name="worldScale">一定の基準倍率</param>
        /// <returns></returns>
        public SKPaintCache.TextStrokeKey? ToStrokeCacheKey(float viewScale, float worldScale)
        {
            if (Stroke is null || BorderColor is null)
            {
                return null;
            }
            var fontKey = ToFontCacheKey(viewScale, worldScale);
            if (fontKey is null)
            {
                return null;
            }

            var w = Stroke.Width.StrokeWidthByScale(viewScale, worldScale);
            if (!w.HasValue)
            {
                return null;
            }
            return new SKPaintCache.TextStrokeKey(fontKey, BorderColor.Value.ToSKColor(), w.Value, Stroke.StrokeType, Stroke.DashEffect);
        }
        /// <summary>
        /// SKPaintCache用のキー
        /// </summary>
        /// <param name="viewScale"></param>
        /// <param name="worldScale"></param>
        /// <returns></returns>
        public SKPaintCache.TextFillKey? ToFillCacheKey(float viewScale, float worldScale)
        {
            if (FillColor is null)
            {
                return null;
            }
            var fontKey = ToFontCacheKey(viewScale, worldScale);
            if (fontKey is null)
            {
                return null;
            }
            return new SKPaintCache.TextFillKey(fontKey, FillColor.Value.ToSKColor());

        }
    }

}

using CS2MapView.Theme;
using SkiaSharp;
using System.Collections.Concurrent;

namespace CS2MapView.Drawing
{
    /// <summary>
    /// 描画設定による拡大を加味して描画時にSKPaintを作って返す機能付きのSKPaint(とSKFont)のキャッシュ。
    /// 取得したオブジェクトをDisposeしないこと。(SKPaintCache.DisposeAllですべてまとめてやる）
    /// </summary>
    public class SKPaintCache
    {
        /// <summary>
        /// 線描画用のSKPaintのキャッシュを保持するためのキー。ダッシュはWidthに対する比率で取り扱う
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Color"></param>
        /// <param name="StrokeType"></param>
        /// <param name="Dash"></param>
        public record class StrokeKey(float Width, SKColor Color, StrokeType StrokeType, float[]? Dash = null);
        /// <summary>
        /// フォントのキャッシュ保持キー。
        /// </summary>
        /// <param name="FontFamily"></param>
        /// <param name="Size"></param>
        public record class FontKey(string FontFamily, float Size);
        /// <summary>
        /// フォント塗りつぶし用のキャッシュ保持キー。
        /// </summary>
        /// <param name="FontKey"></param>
        /// <param name="Color"></param>
        public record class TextFillKey(FontKey FontKey, SKColor Color);
        /// <summary>
        /// フォント輪郭用のキャッシュ保持キー
        /// </summary>
        /// <param name="FontKey"></param>
        /// <param name="Color"></param>
        /// <param name="Width"></param>
        /// <param name="StrokeType"></param>
        /// <param name="Dash"></param>
        public record class TextStrokeKey(FontKey FontKey, SKColor Color, float Width, StrokeType StrokeType, float[]? Dash = null);

        private static void SetStrokeType(SKPaint paint, StrokeType strokeType)
        {
            if (strokeType == StrokeType.Round)
            {
                paint.StrokeCap = SKStrokeCap.Round;
                paint.StrokeJoin = SKStrokeJoin.Round;
            }
            else if(strokeType == StrokeType.Butt)
            {
                paint.StrokeCap = SKStrokeCap.Butt;
                paint.StrokeJoin = SKStrokeJoin.Bevel;
            }
            else
            {
                paint.StrokeCap = SKStrokeCap.Butt;
                paint.StrokeJoin = SKStrokeJoin.Round;
            }
        }

        private static readonly SKPaintCache _inst = new();
        /// <summary>
        /// このクラスの唯一のインスタンスを返します。
        /// </summary>
        public static SKPaintCache Instance => _inst;

        private readonly ConcurrentDictionary<StrokeKey, SKPaint> StrokeCache = new();
        private readonly ConcurrentDictionary<SKColor, SKPaint> FillCache = new();
        private readonly ConcurrentDictionary<FontKey, SKFont> FontCache = new();
        private readonly ConcurrentDictionary<TextFillKey, SKPaint> TextFillCache = new();
        private readonly ConcurrentDictionary<TextStrokeKey, SKPaint> TextStrokeCache = new();
        /// <summary>
        /// 線描画用のSKPaintを取得します。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public SKPaint GetStroke(StrokeKey key)
        {
            if (StrokeCache.TryGetValue(key, out var val))
            {
                return val;
            }
            SKPaint skPaint = new()
            {
                IsStroke = true,
                StrokeWidth = key.Width,
                Color = key.Color,
                IsAntialias = true
            };
            SetStrokeType(skPaint, key.StrokeType);

            if (key.Dash is not null && key.Dash.Length > 0)
            {
                skPaint.PathEffect = SKPathEffect.CreateDash(key.Dash.Select(t => t * key.Width).ToArray(), 0f);
            }
            StrokeCache[key] = skPaint;
            return skPaint;
        }
        /// <summary>
        /// 塗りつぶし用のSKPaintを取得します。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public SKPaint GetFill(SKColor key)
        {
            if (FillCache.TryGetValue(key, out var val))
            {
                return val;
            }
            SKPaint p = new()
            {
                IsStroke = false,
                Color = key
            };
            FillCache[key] = p;
            return p;
        }
        /// <summary>
        /// SKFontを取得します。
        /// </summary>
        /// <param name="fontKey"></param>
        /// <returns></returns>
        public SKFont GetFont(FontKey fontKey)
        {
            if (FontCache.TryGetValue(fontKey, out var val))
            {
                return val;
            }
            var typeface = SKFontManager.Default.MatchFamily(fontKey.FontFamily, SKFontStyle.Bold);
            var p = new SKFont(typeface, fontKey.Size);
            FontCache[fontKey] = p;
            return p;

        }
        /// <summary>
        /// 文字列の輪郭描画用のSKPaintを取得します。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public SKPaint GetTextStroke(TextStrokeKey key)
        {
            if (TextStrokeCache.TryGetValue(key, out var val))
            {
                return val;
            }
            var p = new SKPaint(GetFont(key.FontKey))
            {
                Color = key.Color,
                IsStroke = true,
                StrokeWidth = key.Width,
                IsAntialias = true
            };
            SetStrokeType(p, key.StrokeType);
            if (key.Dash is not null && key.Dash.Length > 0)
            {
                p.PathEffect = SKPathEffect.CreateDash(key.Dash.Select(t => t * key.Width).ToArray(), 0f);
            }
            TextStrokeCache[key] = p;
            return p;
        }
        /// <summary>
        /// 文字列の塗りつぶし用のSKPaintを取得します。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public SKPaint GetTextFill(TextFillKey key)
        {
            if (TextFillCache.TryGetValue(key, out var val))
            {
                return val;
            }
            var p = new SKPaint(GetFont(key.FontKey))
            {
                Color = key.Color,
                IsStroke = false,
                IsAntialias = true
            };
            TextFillCache[key] = p;
            return p;
        }


        private SKPaintCache() { }


        /// <summary>
        /// 現在保持している描画用オブジェクトをすべて破棄します。
        /// </summary>
        public void DisposeAll()
        {
            foreach (var obj in StrokeCache.Values)
            {
                obj.PathEffect?.Dispose();
                obj.Dispose();
            }
            StrokeCache.Clear();
            foreach (var item in FillCache.Values)
            {
                item.Dispose();
            }
            FillCache.Clear();
            foreach (var obj in TextFillCache.Values)
            {
                obj.Dispose();
            }
            TextFillCache.Clear();
            foreach (var obj in TextStrokeCache.Values)
            {
                obj.PathEffect?.Dispose();
                obj.Dispose();
            }
            TextStrokeCache.Clear();
            foreach (var obj in FontCache.Values)
            {
                obj.Dispose();
            }
            FontCache.Clear();
        }
    }
}

using CS2MapView.Drawing;
using System.Xml.Serialization;

namespace CS2MapView.Theme
{
    /// <summary>
    /// 色つきストロークスタイル
    /// </summary>
    public class StrokeStyleWithColor : StrokeStyle
    {
        /// <summary>
        /// 色
        /// </summary>
        public SerializableColor Color { get; set; }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public StrokeStyleWithColor() { }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="color"></param>
        /// <param name="width"></param>
        public StrokeStyleWithColor(SerializableColor color, Width width)
        {
            Color = color;
            Width = width;
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ss"></param>
        /// <param name="color"></param>
        public StrokeStyleWithColor(StrokeStyle ss, SerializableColor color)
        {
            Color = color;
            Width = ss.Width;
            DashEffect = ss.DashEffect;
        }
        /// <summary>
        /// SKPaintキャッシュのキー
        /// </summary>
        /// <param name="viewScale">ユーザー指定の倍率</param>
        /// <param name="worldScale">一定の基準倍率</param>
        /// <returns></returns>
        internal SKPaintCache.StrokeKey? ToCacheKey(float viewScale, float worldScale)
        {

            var w = Width.StrokeWidthByScale(viewScale, worldScale);
            if (!w.HasValue)
            {
                return null;
            }

            return new SKPaintCache.StrokeKey(w.Value, Color.ToSKColor(), StrokeType, DashEffect);
        }



    }
    /// <summary>
    /// 等高線：線のスタイル
    /// </summary>
    public class ContourStrokeStyle : StrokeStyle
    {
        /// <summary>
        /// 標高または水深
        /// </summary>
        [XmlAttribute("height")]
        public float Height { get; set; }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ContourStrokeStyle() { }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        public ContourStrokeStyle(float height, Width width) : base(width)
        {
            Height = height;
        }
    }
    /// <summary>
    /// ストロークのタイプ
    /// </summary>
    public enum StrokeType
    {
        /// <summary>
        /// 角型
        /// </summary>
        Flat,
        /// <summary>
        /// 丸型
        /// </summary>
        Round,
        Butt
    }
    /// <summary>
    /// ストロークスタイル
    /// </summary>
    public class StrokeStyle : ICloneable
    {
        /// <summary>
        /// 幅
        /// </summary>
        public Width Width { get; set; }
        /// <summary>
        /// ダッシュ効果（幅に対する割合の配列）
        /// </summary>
        public float[]? DashEffect { get; set; }
        /// <summary>
        /// ストローク端の描画方法
        /// </summary>
        public StrokeType StrokeType { get; set; }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public StrokeStyle() { }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="width"></param>
        public StrokeStyle(Width width)
        {

            Width = width;
            StrokeType = StrokeType.Round;
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="width"></param>
        /// <param name="dashEffect"></param>
        /// <param name="strokeType"></param>
        public StrokeStyle(Width width, float[]? dashEffect, StrokeType strokeType = StrokeType.Round) : this(width)
        {
            DashEffect = dashEffect;
            StrokeType = strokeType;
        }
        /// <summary>
        /// 色付きのストロークスタイルに変換
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public StrokeStyleWithColor WithColor(SerializableColor color)
        {
            return new StrokeStyleWithColor(this, color);
        }
        /// <inheritdoc/>
        public object Clone()
        {
            return new StrokeStyle { Width = Width, DashEffect = DashEffect, StrokeType = StrokeType };
        }
    }
}

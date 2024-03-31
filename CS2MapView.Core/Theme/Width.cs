using System.Xml.Serialization;

namespace CS2MapView.Theme
{
    /// <summary>
    /// 拡大制御付き幅
    /// </summary>
    public struct Width
    {
        /// <summary>
        /// 基本サイズ
        /// </summary>
        [XmlAttribute("base")]
        public float Base { get; set; }
        /// <summary>
        /// 画面拡大への追従度
        /// </summary>
        [XmlAttribute("coefficient")]
        public float Coefficient { get; set; }
        /// <summary>
        /// 最小サイズ
        /// </summary>
        [XmlAttribute("min")]
        public float Min { get; set; }
        /// <summary>
        /// 最大サイズ
        /// </summary>
        [XmlAttribute("max")]
        public float Max { get; set; }
        /// <summary>
        /// 表示される最小サイズ
        /// </summary>
        [XmlAttribute("visibleMin")]
        public float VisibleMin { get; set; }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Width() { }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="base"></param>
        /// <param name="coefficient"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="visibleMin"></param>
        public Width(float @base, float coefficient, float min, float max, float visibleMin)
        {
            Base = @base; Coefficient = coefficient; Min = min; Max = max; VisibleMin = visibleMin;
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="base"></param>
        public Width(float @base)
        {
            Base = @base;
            Coefficient = 1f;
            Min = 1f;
            Max = 1f;
            VisibleMin = 0.5f;
        }
        /// <summary>
        /// 指定したピクセル数で表示するために設定すべきSKPaint.StrokeWidthを返します。
        /// </summary>
        /// <param name="viewScale"></param>
        /// <param name="worldScale"></param>
        /// <returns></returns>
        public readonly float? StrokeWidthByScale(float viewScale, float worldScale)
        {
            float vs = 1f + (viewScale - 1f) * Coefficient;
            float vw = Base * vs;
            if (vw < VisibleMin)
            {
                return null;
            }
            float w = Math.Clamp(vw, Min, Max);

            return w * (1f / (viewScale * worldScale));
        }
    }
}

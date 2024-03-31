namespace CS2MapView.Theme
{
    /// <summary>
    /// 塗りつぶしのスタイル
    /// </summary>
    public class FillStyle
    {
        /// <summary>
        /// 塗りつぶし色
        /// </summary>
        public SerializableColor Color { get; set; }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FillStyle() { }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="color"></param>
        public FillStyle(SerializableColor color) { Color = color; }
        //  public SKPaintCache.FillKey ToCacheKey() => new(Color);
    }


}

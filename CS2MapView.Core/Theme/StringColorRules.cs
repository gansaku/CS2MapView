namespace CS2MapView.Theme
{
    /// <summary>
    /// 文字列の表示色設定
    /// 他図形とは異なり、Stroke→Fillの順に描画される。
    /// </summary>
    public class StringColorRules
    {
        /// <summary>
        /// 建物名
        /// </summary>
        public SerializableColor? BuildingNameFill { get; set; }
        /// <summary>
        /// 建物名
        /// </summary>
        public SerializableColor? BuildingNameStroke { get; set; }
        /// <summary>
        /// 地名
        /// </summary>
        public SerializableColor? DistrictNameFill { get; set; }
        /// <summary>
        /// 地名
        /// </summary>
        public SerializableColor? DistrictNameStroke { get; set; }
        /// <summary>
        /// 道路名
        /// </summary>
        public SerializableColor? StreetNameFill { get; set; }
        /// <summary>
        /// 道路名
        /// </summary>
        public SerializableColor? StreetNameStroke { get; set; }
        /// <summary>
        /// デフォルト設定値
        /// </summary>
        public static StringColorRules Default => new()
        {
            BuildingNameFill = SerializableColor.FromARGB(255, 70, 70, 70),
            BuildingNameStroke = SerializableColor.FromARGB(180, 255, 255, 255),
            DistrictNameFill = SerializableColor.FromARGB(255, 90, 90, 90),
            DistrictNameStroke = SerializableColor.FromARGB(140, 255, 255, 255),
            StreetNameFill = SerializableColor.FromARGB(255, 70, 70, 220),
            StreetNameStroke = SerializableColor.FromARGB(255, 255, 255, 255)
        };

    }
}

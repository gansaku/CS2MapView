using log4net;
using System.Xml.Serialization;

namespace CS2MapView.Theme
{
    /// <summary>
    /// 色設定
    /// </summary>
    public class ColorRules
    {
        [XmlIgnore]
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ColorRules));

        /// <summary>
        /// 等高線の色
        /// </summary>
        [XmlArray("LandContours")]
        [XmlArrayItem("Contour")]
        public List<ContourColor>? LandContourColors { get; set; }
        /// <summary>
        /// 海岸線・水深の色
        /// </summary>
        [XmlArray("WaterContours")]
        [XmlArrayItem("Contour")]
        public List<ContourColor>? WaterContourColors { get; set; }
        /// <summary>
        /// 区域境界
        /// </summary>
        public SerializableColor DistrictBoundary { get; set; }
        /// <summary>
        /// グリッドの色
        /// </summary>
        public SerializableColor GridLine { get; set; }
        /// <summary>
        /// 建物の枠線の色
        /// </summary>
        public SerializableColor BuildingBorder { get; set; }
        /// <summary>
        /// 住宅の色
        /// </summary>
        public SerializableColor ResidentialBuilding { get; set; }
        /// <summary>
        /// 商業の色
        /// </summary>
        public SerializableColor CommercialBuilding { get; set; }
        /// <summary>
        /// 産業の色
        /// </summary>
        public SerializableColor IndustrialBuilding { get; set; }
        /// <summary>
        /// オフィスの色
        /// </summary>
        public SerializableColor OfficeBuilding { get; set; }
        /// <summary>
        /// 交通機関の色
        /// </summary>
        public SerializableColor PublicTransportBuilding { get; set; }
        /// <summary>
        /// 公共の建物の色
        /// </summary>
        public SerializableColor PublicBuilding { get; set; }
        /// <summary>
        /// 修景の色
        /// </summary>
        public SerializableColor BeautificationBuilding { get; set; }
        /// <summary>
        /// 線路(黒部分)の色
        /// </summary>
        public SerializableColor Train { get; set; }
        /// <summary>
        /// 線路(白抜き)の色
        /// </summary>
        public SerializableColor TrainDash { get; set; }
        /// <summary>
        /// 線路(トンネル)の色
        /// </summary>
        public SerializableColor TrainTunnel { get; set; }
        /// <summary>
        /// 線路(トンネル)の色
        /// </summary>
        public SerializableColor TrainTunnelDash { get; set; }
        /// <summary>
        /// 地下鉄の色
        /// </summary>
        public SerializableColor Metroway { get; set; }
        /// <summary>
        /// 地下鉄(白抜き)の色
        /// </summary>
        public SerializableColor MetrowayDash { get; set; }
        /// <summary>
        /// 路面電車の色
        /// </summary>
        public SerializableColor Tramway { get; set; }
        /// <summary>
        /// 路面電車(白抜き)の色
        /// </summary>
        public SerializableColor TramwayDash { get; set; }
        /// <summary>
        /// 道路の色
        /// </summary>
        public SerializableColor Road { get; set; }
        /// <summary>
        /// 道路(枠線)の色
        /// </summary>
        public SerializableColor RoadBorder { get; set; }
        /// <summary>
        /// 道路(トンネル)の色
        /// </summary>
        public SerializableColor RoadTunnel { get; set; }
        /// <summary>
        /// 道路(トンネル)の枠線の色
        /// </summary>
        public SerializableColor RoadTunnelBorder { get; set; }
        /// <summary>
        /// 道路(高架)の色
        /// </summary>
        public SerializableColor RoadElevated { get; set; }
        /// <summary>
        /// 道路(高架)の枠線の色
        /// </summary>
        public SerializableColor RoadElevatedBorder { get; set; }
        /// <summary>
        /// 高速道路の色
        /// </summary>
        public SerializableColor Highway { get; set; }
        /// <summary>
        /// 高速道路の枠線の色
        /// </summary>
        public SerializableColor HighwayBorder { get; set; }
        /// <summary>
        /// 高速道路(トンネル)の色
        /// </summary>
        public SerializableColor HighwayTunnel { get; set; }
        /// <summary>
        /// 高速道路(トンネル)の枠線の色
        /// </summary>
        public SerializableColor HighwayTunnelBorder { get; set; }
        /// <summary>
        /// 高速道路(高架)の色
        /// </summary>
        public SerializableColor HighwayElevated { get; set; }
        /// <summary>
        /// 高速道路(高架)の枠線の色
        /// </summary>
        public SerializableColor HighwayElevatedBorder { get; set; }
        /// <summary>
        /// 修景の色
        /// </summary>
        public SerializableColor Beautification { get; set; }
        /// <summary>
        /// 修景の枠線の色
        /// </summary>
        public SerializableColor BeautificationBorder { get; set; }
        /// <summary>
        /// 修景(トンネル)の色
        /// </summary>
        public SerializableColor BeautificationTunnel { get; set; }
        /// <summary>
        /// 修景(トンネル)の枠線の色
        /// </summary>
        public SerializableColor BeautificationTunnelBorder { get; set; }
        /// <summary>
        /// 修景(高架)の色
        /// </summary>
        public SerializableColor BeautificationElevated { get; set; }
        /// <summary>
        /// 修景(高架)の枠線
        /// </summary>
        public SerializableColor BeautificationElevatedBorder { get; set; }
        /// <summary>
        /// 森の色(αは調整されます)
        /// </summary>
        public SerializableColor ForestBase { get; set; }
        /// <summary>
        /// ケーブルカーの線路の色
        /// </summary>
        public SerializableColor CableCarWay { get; set; }
        /// <summary>
        /// モノレールの線路の色
        /// </summary>
        public SerializableColor MonorailWay { get; set; }
        /// <summary>
        /// 停留所（枠線）
        /// </summary>
        public SerializableColor TransportStopStroke { get; set; }
        /// <summary>
        /// 停留所（中）
        /// </summary>
        public SerializableColor TransportStopFill { get; set; }


        internal static ColorRules Default => new()
        {
            LandContourColors =
                    [
                        new(850f, SerializableColor.FromARGB(255, 166, 116, 52), SerializableColor.FromARGB(255, 181, 131, 67)),
                        new(50f, SerializableColor.FromARGB(255, 207, 233, 163), SerializableColor.FromARGB(255, 222, 248, 178)),
                        new(-10f, SerializableColor.FromARGB(255, 104, 161, 73), SerializableColor.FromARGB(255, 113, 174, 84)),
                        new(float.MinValue, SerializableColor.FromARGB(255, 93, 154, 64), SerializableColor.FromARGB(255, 103, 167, 75)),

                    ],
            WaterContourColors = [
                        new(0f, SerializableColor.FromARGB(255, 40, 110, 170), SerializableColor.FromARGB(255, 127, 220, 255)),
                new(100f, SerializableColor.FromARGB(0, 0, 0, 0), SerializableColor.FromARGB(255, 79, 140, 175))
                    ],
            DistrictBoundary = SerializableColor.FromARGB(120, 0, 0, 0),
            GridLine = SerializableColor.FromARGB(150, 70, 200, 30),
            BuildingBorder = SerializableColor.FromARGB(255, 80, 80, 80),
            ResidentialBuilding = SerializableColor.FromARGB(255, 230, 255, 230),
            CommercialBuilding = SerializableColor.FromARGB(255, 230, 230, 255),
            IndustrialBuilding = SerializableColor.FromARGB(255, 255, 255, 230),
            OfficeBuilding = SerializableColor.FromARGB(255, 220, 255, 255),
            PublicTransportBuilding = SerializableColor.FromARGB(255, 230, 200, 255),
            PublicBuilding = SerializableColor.FromARGB(255, 255, 210, 210),
            BeautificationBuilding = SerializableColor.FromARGB(255, 180, 255, 180),
            Train = SerializableColor.FromARGB(255, 0, 0, 0),
            TrainDash = SerializableColor.FromARGB(255, 255, 255, 255),
            TrainTunnel = SerializableColor.FromARGB(255, 0, 0, 0),
            TrainTunnelDash = SerializableColor.FromARGB(255, 0, 0, 0),
            Metroway = SerializableColor.FromARGB(180, 0, 140, 180),
            MetrowayDash = SerializableColor.FromARGB(180, 255, 255, 255),
            Tramway = SerializableColor.FromARGB(255, 0, 180, 140),
            TramwayDash = SerializableColor.FromARGB(255, 255, 255, 255),
            Road = SerializableColor.FromARGB(255, 230, 230, 230),
            RoadBorder = SerializableColor.FromARGB(255, 90, 90, 90),
            RoadTunnel = SerializableColor.FromARGB(255, 240, 240, 240),
            RoadTunnelBorder = SerializableColor.FromARGB(255, 100, 100, 100),
            RoadElevated = SerializableColor.FromARGB(255, 220, 220, 220),
            RoadElevatedBorder = SerializableColor.FromARGB(255, 70, 70, 70),
            Highway = SerializableColor.FromARGB(255, 255, 225, 100),
            HighwayBorder = SerializableColor.FromARGB(255, 200, 100, 100),
            HighwayTunnel = SerializableColor.FromARGB(255, 255, 235, 130),
            HighwayTunnelBorder = SerializableColor.FromARGB(255, 200, 100, 100),
            HighwayElevated = SerializableColor.FromARGB(255, 255, 225, 100),
            HighwayElevatedBorder = SerializableColor.FromARGB(255, 200, 100, 100),
            Beautification = SerializableColor.FromARGB(255, 230, 210, 210),
            BeautificationBorder = SerializableColor.FromARGB(255, 110, 100, 100),
            BeautificationTunnel = SerializableColor.FromARGB(255, 220, 200, 200),
            BeautificationTunnelBorder = SerializableColor.FromARGB(255, 110, 100, 100),
            BeautificationElevated = SerializableColor.FromARGB(255, 240, 220, 220),
            BeautificationElevatedBorder = SerializableColor.FromARGB(255, 110, 100, 100),
            ForestBase = SerializableColor.FromARGB(255, 20, 120, 20),
            CableCarWay = SerializableColor.FromARGB(255, 40, 40, 40),
            MonorailWay = SerializableColor.FromARGB(255, 0, 0, 0),
            TransportStopStroke= SerializableColor.FromARGB(255,0,0,0),
            TransportStopFill = SerializableColor.FromARGB(255,255,255,255)
        };

        internal static ColorRules White => new()
        {
            LandContourColors =
                    [
                        new(float.MinValue, SerializableColor.GrayScale(170), SerializableColor.GrayScale(255)),
                    ],
            WaterContourColors = [
                        new(0f, SerializableColor.GrayScale(100), SerializableColor.GrayScale(210))
                    ],
            DistrictBoundary = SerializableColor.GrayScale(200,70),
            GridLine = SerializableColor.GrayScale(180, 150),
            BuildingBorder = SerializableColor.GrayScale( 80),
            ResidentialBuilding = SerializableColor.GrayScale(230),
            CommercialBuilding = SerializableColor.GrayScale(230),
            IndustrialBuilding = SerializableColor.GrayScale(230),
            OfficeBuilding = SerializableColor.GrayScale(230),
            PublicTransportBuilding = SerializableColor.GrayScale(230),
            PublicBuilding = SerializableColor.GrayScale(230),
            BeautificationBuilding = SerializableColor.GrayScale(230),
            Train = SerializableColor.FromARGB(255, 0, 0, 0),
            TrainDash = SerializableColor.FromARGB(255, 255, 255, 255),
            TrainTunnel = SerializableColor.FromARGB(255, 0, 0, 0),
            TrainTunnelDash = SerializableColor.FromARGB(255, 0, 0, 0),
            Metroway = SerializableColor.FromARGB(180, 0, 140, 180),
            MetrowayDash = SerializableColor.FromARGB(180, 255, 255, 255),
            Tramway = SerializableColor.FromARGB(255, 0, 180, 140),
            TramwayDash = SerializableColor.FromARGB(255, 255, 255, 255),
            Road = SerializableColor.FromARGB(255, 230, 230, 230),
            RoadBorder = SerializableColor.FromARGB(255, 90, 90, 90),
            RoadTunnel = SerializableColor.FromARGB(255, 240, 240, 240),
            RoadTunnelBorder = SerializableColor.FromARGB(255, 100, 100, 100),
            RoadElevated = SerializableColor.FromARGB(255, 220, 220, 220),
            RoadElevatedBorder = SerializableColor.FromARGB(255, 70, 70, 70),
            Highway = SerializableColor.FromARGB(255, 255, 225, 100),
            HighwayBorder = SerializableColor.FromARGB(255, 200, 100, 100),
            HighwayTunnel = SerializableColor.FromARGB(255, 255, 235, 130),
            HighwayTunnelBorder = SerializableColor.FromARGB(255, 200, 100, 100),
            HighwayElevated = SerializableColor.FromARGB(255, 255, 225, 100),
            HighwayElevatedBorder = SerializableColor.FromARGB(255, 200, 100, 100),
            Beautification = SerializableColor.FromARGB(255, 230, 210, 210),
            BeautificationBorder = SerializableColor.FromARGB(255, 110, 100, 100),
            BeautificationTunnel = SerializableColor.FromARGB(255, 220, 200, 200),
            BeautificationTunnelBorder = SerializableColor.FromARGB(255, 110, 100, 100),
            BeautificationElevated = SerializableColor.FromARGB(255, 240, 220, 220),
            BeautificationElevatedBorder = SerializableColor.FromARGB(255, 110, 100, 100),
            ForestBase = SerializableColor.FromARGB(255, 20, 120, 20),
            CableCarWay = SerializableColor.FromARGB(255, 40, 40, 40),
            MonorailWay = SerializableColor.FromARGB(255, 0, 0, 0),
            TransportStopStroke = SerializableColor.FromARGB(255, 0, 0, 0),
            TransportStopFill = SerializableColor.FromARGB(255, 255, 255, 255)
        };

        /// <summary>
        /// 指定した標高に対応した色を作成して返します。
        /// </summary>
        /// <param name="height">標高</param>
        /// <returns></returns>
        public ContourColor LerpLandContourColors(float height) => LerpContourColors(LandContourColors, height);
        /// <summary>
        /// 指定した水深に対応した色を作成して返します。
        /// </summary>
        /// <param name="height">水深</param>
        /// <returns></returns>
        public ContourColor LerpWaterContourColors(float height) => LerpContourColors(WaterContourColors, height);

        /// <summary>
        /// 指定した標高または水深に対応した色を作成して返します。
        /// </summary>
        /// <param name="list">全設定内容</param>
        /// <param name="height">標高</param>
        /// <returns></returns>
        private static ContourColor LerpContourColors(IList<ContourColor>? list, float height)
        {
            if (list is null)
            {
                Logger.Warn($"List of ContourColor was null");
                return new ContourColor(height, SerializableColor.FromARGB(255, 0, 0, 0), SerializableColor.FromARGB(255, 0, 0, 0));
            }
            var c1 = list!.OrderBy(s => s.Height).LastOrDefault(s => s.Height <= height);
            c1 ??= list!.OrderBy(s => s.Height).First();
            var c2 = list!.OrderByDescending(s => s.Height).LastOrDefault(s => s.Height >= height);
            c2 ??= list!.OrderByDescending(s => s.Height).First();
            if (c1.Height == c2.Height)
            {
                return c1;
            }
            var c2fr = (height - c1.Height) / (c2.Height - c1.Height);
            var c1fr = 1f - c2fr;

            SerializableColor CalcColor(SerializableColor a1, SerializableColor a2)
            {
                /*   byte Element(Func<SerializableColor, byte> pick)
                   {
                       float color = pick(a1) * c1fr + pick(a2) * c2fr;
                       color = Math.Clamp(color, 0, 255);
                       return (byte)color;
                   }
                   return new SerializableColor
                   {
                       A = Element(t => t.A),
                       R = Element(t => t.R),
                       G = Element(t => t.G),
                       B = Element(t => t.B)
                   };
                */
                return new SerializableColor
                {
                    A = (byte)Math.Clamp(a1.A * c1fr + a2.A * c2fr, 0, 255),
                    R = (byte)Math.Clamp(a1.R * c1fr + a2.R * c2fr, 0, 255),
                    G = (byte)Math.Clamp(a1.G * c1fr + a2.G * c2fr, 0, 255),
                    B = (byte)Math.Clamp(a1.B * c1fr + a2.B * c2fr, 0, 255),
                };
            }

            var borderColor = CalcColor(c1.BorderColor, c2.BorderColor);
            var fillColor = CalcColor(c1.FillColor, c2.FillColor);

            return new ContourColor(height, borderColor, fillColor);
        }
    }
}

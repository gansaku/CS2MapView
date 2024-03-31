using System.Xml.Serialization;

namespace CS2MapView.Theme
{
    /// <summary>
    /// 描画テーマ
    /// </summary>
    public class DrawingTheme
    {
        /// <summary>
        /// テーマ名
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 色設定
        /// </summary>
        public ColorRules? Colors { get; set; }
        /// <summary>
        /// 線設定
        /// </summary>
        public StrokeRules? Strokes { get; set; }

        /// <summary>
        /// ロープウェイ装飾の間隔
        /// </summary>
        public float CableCarDecorationSpan { get; set; }
        /// <summary>
        /// ロープウェイ装飾の長さ
        /// </summary>
        public Width CableCarDecorationLength { get; set; }
        /// <summary>
        /// モノレール装飾の間隔
        /// </summary>
        public float MonorailDecorationSpan { get; set; }
        /// <summary>
        /// モノレール装飾の長さ
        /// </summary>
        public Width MonorailDecorationLength { get; set; }
        /// <summary>
        /// 鉄道駅の描画幅
        /// </summary>
        public float TrainStationWidth { get; set; }
        /// <summary>
        /// モノレール駅の描画幅
        /// </summary>
        public float MonorailStationWidth { get; set; }
        /// <summary>
        /// 地下鉄駅の描画幅
        /// </summary>
        public float MetroStationWidth { get; set; }
        /// <summary>
        /// ロープウェイ駅の描画幅
        /// </summary>
        public float CableCarStationWidth { get; set; }

        /// <summary>
        /// 地図記号のサイズ（短辺）
        /// </summary>
        public Width MapSymbolSize { get; set; }

        /// <summary>
        /// テーマの保存
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Task SaveAsync(string path)
        {
            return Task.Run(() =>
            {
                var ser = new XmlSerializer(typeof(DrawingTheme));
                using var fs = new FileStream(path, FileMode.Create);
                ser.Serialize(fs, this);
            });
        }
        /// <summary>
        /// テーマの読み込み
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Task<DrawingTheme?> LoadAsync(string path)
        {
            return Task.Run(() =>
            {
                var ser = new XmlSerializer(typeof(DrawingTheme));
                using var fs = new FileStream(path, FileMode.Open);
                return ser.Deserialize(fs) as DrawingTheme;
            });
        }


        /// <summary>
        /// デフォルト設定値
        /// </summary>
        [XmlIgnore]
        public static DrawingTheme Default
        {
            get
            {
                return new DrawingTheme
                {
                    Name = "default",
                    Strokes = StrokeRules.Default,
                    Colors = ColorRules.Default,
                    CableCarDecorationSpan = 64f,
                    CableCarDecorationLength = new(0.5f, 0.9f, 0.5f, 3f, 0.5f),
                    MonorailDecorationSpan = 64f,
                    MonorailDecorationLength = new(0.5f, 0.9f, 0.5f, 3f, 0.5f),
                    TrainStationWidth = 19f,
                    MetroStationWidth = 17f,
                    CableCarStationWidth = 9f,
                    MonorailStationWidth = 9f,
                    MapSymbolSize = new(14f, 1f, 14f, 14f, 8f)

                };
            }
        }
        /// <summary>
        /// 白地図テーマ
        /// </summary>
        [XmlIgnore]
        public static DrawingTheme White
        {
            get
            {
                return new DrawingTheme
                {
                    Name = "white",
                    Strokes = StrokeRules.Default,
                    Colors = ColorRules.White,
                    CableCarDecorationSpan = 64f,
                    CableCarDecorationLength = new(0.5f, 0.9f, 0.5f, 3f, 0.5f),
                    MonorailDecorationSpan = 64f,
                    MonorailDecorationLength = new(0.5f, 0.9f, 0.5f, 3f, 0.5f),
                    TrainStationWidth = 19f,
                    MetroStationWidth = 17f,
                    CableCarStationWidth = 9f,
                    MonorailStationWidth = 9f,
                    MapSymbolSize = new(14f, 1f, 14f, 14f, 8f)

                };
            }
        }
    }

}

using CS2MapView.Drawing.Layer;
using log4net;
using System.Xml.Serialization;

namespace CS2MapView.Config
{
    /// <summary>
    /// 動作設定のうち、ファイル保存するもの
    /// </summary>
    public class UserSettings
    {
        [XmlIgnore]
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UserSettings));
        private const string DefaultFileName = "cs2mapview.usersettings";

        /// <summary>
        /// テーマ名
        /// </summary>
        public string? Theme { get; set; }
        /// <summary>
        /// 文字列テーマ
        /// </summary>
        public string? StringTheme { get; set; }
        /// <summary>
        /// 地図記号画像セット
        /// </summary>
        public string? DefaultMapSymbolType { get; set; }
        /// <summary>
        /// 等高線間隔
        /// </summary>
        public string? ContourHeights { get; set; }
        /// <summary>
        /// 地形等高線
        /// </summary>
        public bool VectorizeTerrainLand { get; set; } = true;
        /// <summary>
        /// 水域ベクター化
        /// </summary>
        public bool VectorizeTerrainWater { get; set; } = true;
        /// <summary>
        /// 地形等高線計算時の最大解像度
        /// </summary>
        public int TerrainMaxResolution { get; set; } = 4096;
        /// <summary>
        /// 水域計算時の最大解像度
        /// </summary>
        public int WaterMaxResolution { get; set; } = 4096;
        /// <summary>
        /// 建物の枠線非表示
        /// </summary>
        public bool DisableBuildingBorder { get; set; } = false;
        /// <summary>
        /// RICO非表示
        /// </summary>
        public bool HideRICOBuildingsShape { get; set; } = false;
        /// <summary>
        /// 建物ラベル表示
        /// </summary>
        public bool RenderBuildingNameLabel { get; set; } = true;
        /// <summary>
        /// 地図記号表示
        /// </summary>
        public bool RenderMapSymbol { get; set; } = true;
        /// <summary>
        /// 地域名表示
        /// </summary>
        public bool RenderDistrictNameLabel { get; set; } = true;
        /// <summary>
        /// 道路名表示
        /// </summary>
        public bool RenderStreetNames { get; set; } = true;

        /// <summary>
        /// 鉄道線路表示
        /// </summary>
        public bool RenderRailTrain { get; set; } = true;
        /// <summary>
        /// トラム線路表示
        /// </summary>
        public bool RenderRailTram { get; set; } = true;
        /// <summary>
        /// 地下鉄線路表示
        /// </summary>
        public bool RenderRailMetro { get; set; } = true;
        /// <summary>
        /// モノレール線路表示
        /// </summary>
        public bool RenderRailMonorail { get; set; } = true;
        /// <summary>
        /// ロープウェイ線路表示
        /// </summary>
        public bool RenderRailCableCar { get; set; } = true;
        /// <summary>
        /// 建物名の代わりにプレハブの名前を使用
        /// </summary>
        public bool UsePrefabBuildingName { get; set; } = false;

        /// <summary>
        /// 建物名のカスタムフォント / Custom font for building names
        /// </summary>
        public string? CustomBuildingNameFont { get; set; }

        /// <summary>
        /// 地区名のカスタムフォント / Custom font for district names
        /// </summary>
        public string? CustomDistrictNameFont { get; set; }

        /// <summary>
        /// 道路名のカスタムフォント / Custom font for street names
        /// </summary>
        public string? CustomStreetNameFont { get; set; }

        /// <summary>
        /// レイヤー表示順・非表示情報
        /// </summary>
        [XmlArray("Layers")]
        [XmlArrayItem("Item")]
        public List<LayerSettings>? LayerDrawingOrder { get; set; }
        /// <summary>
        /// 建物ごとの表示設定（CS1用)
        /// </summary>
        [XmlArray("CS1BuildingPreferences")]
        [XmlArrayItem("Building")]
        public List<BuildingPreference>? CS1BuildingPreferences { get; set; }

        /// <summary>
        /// 開くダイアログの初期ディレクトリ
        /// </summary>
        public string? OpenFileDir { get; set; }
        /// <summary>
        /// 画像保存ダイアログの初期ディレクトリ
        /// </summary>
        public string? SaveImageDir { get; set; }
        /// <summary>
        /// 画像保存ダイアログの画像サイズ
        /// </summary>
        public int OutputImageSize { get; set; }
        /// <summary>
        /// 画像保存ダイアログの画像フォーマット
        /// </summary>
        public string? OutputImageFormat { get; set; }

        /// <summary>
        /// 路線図描画設定
        /// </summary>
        public required TransportRouteMapConfig TransportRouteMapConfig { get; set; }

        /// <summary>
        /// 設定ファイルを読み込みます。ファイル名指定なしの場合はデフォルトのファイル名を使用します。設定ファイルが存在しない場合はデフォルト設定を返します。
        /// </summary>
        /// <param name="createNew"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static UserSettings LoadOrDefault(bool createNew, string? fileName = null)
        {
            var fn = fileName;
            fn ??= Path.Combine(AppContext.BaseDirectory, DefaultFileName);

            if (File.Exists(fn))
            {
                try
                {
                    XmlSerializer ser = new(typeof(UserSettings));
                    using var fs = new FileStream(fn, FileMode.Open);
                    {
                        var obj = ser.Deserialize(fs) as UserSettings;
                        return obj ?? Default;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error($"Failed reading user setting {fn}", e);
                    return Default;
                }
            }
            else
            {
                var d = Default;
                if (createNew)
                {
                    d.SaveAsync(fileName).Wait();
                }
                return d;
            }
        }
        /// <summary>
        /// デフォルトのユーザ設定を返します。
        /// </summary>
        public static UserSettings Default => new()
        {
            Theme = "default",
            StringTheme = "default",
            ContourHeights = "default",
            DefaultMapSymbolType = "ja+",
            LayerDrawingOrder = [
                new(ILayer.LayerNameTerrain, 0, true),
                new(ILayer.LayerNameDistricts, 1, true),
                new(ILayer.LayerNameGrid, 2, true),
                new(ILayer.LayerNameBuildings, 3, true),
                new(ILayer.LayerNameRoads, 4, true),
                new(ILayer.LayerNameRailways, 5, true),
                new(ILayer.LayerNameTransportLines, 6, true),
                new(ILayer.LayerNameLabels, 7, true),

            ],
            OutputImageSize = 4096,
            OutputImageFormat = "png",
            TransportRouteMapConfig = new()
        };
        /// <summary>
        /// 指定したレイヤーが可視であるかを取得します。
        /// </summary>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public bool IsLayerVisible(string layerName)
        {
            if (LayerDrawingOrder is null)
            {
                return false;
            }
            return LayerDrawingOrder.FirstOrDefault(ld => ld.Name == layerName)?.Visible ?? false;


        }
        /// <summary>
        /// 設定ファイルを保存します。
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Task SaveAsync(string? fileName = null)
        {
            return Task.Run(() =>
            {
                var fn = fileName ?? Path.Combine(AppContext.BaseDirectory, DefaultFileName);
                try
                {
                    XmlSerializer ser = new(typeof(UserSettings));
                    using var fs = new FileStream(fn, FileMode.Create);
                    {
                        ser.Serialize(fs, this);
                    }
                    Logger.Debug("User settings were saved.");
                }
                catch (Exception e)
                {
                    Logger.Error($"Failed writing user setting {fn}", e);

                }
            });
        }
    }
}

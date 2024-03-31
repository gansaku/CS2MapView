using System.Xml.Serialization;

namespace CS2MapView.Config
{
    /// <summary>
    /// 設定ファイル内での真偽、null値を指定します。
    /// </summary>
    public enum ThreeState
    {
        /// <summary>
        /// false
        /// </summary>
        @false,
        /// <summary>
        /// true
        /// </summary>
        @true,
        /// <summary>
        /// null
        /// </summary>
        @null

    }
    /// <summary>
    /// 建物の表示設定
    /// </summary>
    public class BuildingPreference
    {
        /// <summary>
        /// ワークショップID
        /// </summary>
        [XmlAttribute("steamId")]
        public string? SteamId { get; set; }
        /// <summary>
        /// 名前
        /// </summary>
        [XmlAttribute("name")]
        public string? Name { get; set; }
        /// <summary>
        /// 非表示
        /// </summary>
        [XmlIgnore]
        public bool? HideShape { get; set; }
        /// <summary>
        /// 建物の形状の表示有無を指定します。
        /// </summary>
        [XmlAttribute("hideShape")]
        public ThreeState HideShapeEnum
        {
            get
            {
                if (HideShape.HasValue)
                {
                    if (HideShape.Value)
                    {
                        return ThreeState.@true;
                    }
                    else
                    {
                        return ThreeState.@false;
                    }
                }
                else
                {
                    return ThreeState.@null;
                }
            }
            set
            {
                HideShape = value switch
                {
                    ThreeState.@true => true,
                    ThreeState.@false => false,
                    _ => null
                };
            }
        }
        /// <summary>
        /// 建物の名前の表示有無を指定します。
        /// </summary>
        [XmlAttribute("nameVisibility")]
        public NameVisibility NameVisibility { get; set; }
        /// <summary>
        /// 地図記号
        /// </summary>
        [XmlAttribute("symbol")]
        public string? MapSymbol { get; set; }

        /// <summary>
        /// 設定内容がすべてデフォルトであり、削除されるべきエントリであることを示します。
        /// </summary>
        public bool IsSettingEmpty => Name == null && HideShape == null && NameVisibility == NameVisibility.Undefined;
    }

    /// <summary>
    /// 建物名の可視性。このenumの名前は設定ファイルに書き込まれます。名前をリファクタリングすることはできません。
    /// </summary>
    public enum NameVisibility
    {
        /// <summary>
        /// 設定なし
        /// </summary>
        Undefined,
        /// <summary>
        /// ユーザ設定の名前が表示されることを示します。
        /// </summary>
        Visible,
        /// <summary>
        /// ユーザ設定されていない場合、アセット短縮名を表示することを示します。
        /// </summary>
        FullVisible,
        /// <summary>
        /// 建物名は表示されません。
        /// </summary>
        Invisible
    }

}

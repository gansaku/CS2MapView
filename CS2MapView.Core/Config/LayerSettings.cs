using System.Xml.Serialization;

namespace CS2MapView.Config
{
    /// <summary>
    /// レイヤー描画情報
    /// </summary>
    public class LayerSettings
    {
        /// <summary>
        /// レイヤー名
        /// </summary>
        [XmlAttribute("name")]
        public string? Name { get; set; }
        /// <summary>
        /// 描画順序
        /// </summary>
        [XmlAttribute("order")]
        public int Order { get; set; }
        /// <summary>
        /// 可視・非可視
        /// </summary>
        [XmlAttribute("visible")]
        public bool Visible { get; set; }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LayerSettings() { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name"></param>
        /// <param name="order"></param>
        /// <param name="visible"></param>
        public LayerSettings(string name, int order, bool visible)
        {
            Name = name;
            Order = order;
            Visible = visible;
        }
    }
}

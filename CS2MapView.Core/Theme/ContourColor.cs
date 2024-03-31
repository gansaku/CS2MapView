using System.Xml.Serialization;

namespace CS2MapView.Theme;
/// <summary>
/// 等高線の色情報
/// </summary>
public class ContourColor
{
    /// <summary>
    /// 高さ（標高または水深）
    /// </summary>
    [XmlAttribute("height")]
    public float Height { get; set; }
    /// <summary>
    /// 線の色
    /// </summary>
    [XmlElement("Stroke")]
    public SerializableColor BorderColor { get; set; }
    /// <summary>
    /// 塗りの色
    /// </summary>
    [XmlElement("Fill")]
    public SerializableColor FillColor { get; set; }
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ContourColor() { }
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="height"></param>
    /// <param name="borderColor"></param>
    /// <param name="fillColor"></param>
    public ContourColor(float height, SerializableColor borderColor, SerializableColor fillColor)
    {
        Height = height;
        BorderColor = borderColor;
        FillColor = fillColor;
    }
}

using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace CS2MapView.Config;
/// <summary>
/// CS1用の描画対象外オブジェクト情報
/// </summary>
public class CS1HiddenObjectInfo
{
    /// <summary>
    /// 建物
    /// </summary>
    public static CS1HiddenObjectInfo Buildings { get; } = CreateDefaultIgnoreBuildings();
    /// <summary>
    /// セグメント
    /// </summary>
    public static CS1HiddenObjectInfo Segments { get; } = CreateDefaultIgnoreSegments();
    /// <summary>
    /// 地図記号
    /// </summary>
    public static CS1HiddenObjectInfo MapSymbols { get; } = CreateDefaultIgnoreMapSymbols();


    /// <summary>
    /// 無視するアイテムクラスのリスト
    /// </summary>
    [XmlArray("ByItemClass"), XmlArrayItem("ItemClassRegex", typeof(string))]
    public List<string> IgnoreByItemClass { get; set; } = [];
    /// <summary>
    /// 無視する名称のリスト
    /// </summary>
    [XmlArray("ByName"), XmlArrayItem("NameRegex", typeof(string))]
    public List<string> IgnoreByName { get; set; } = [];

    private List<Regex> IgnoreByItemClassRegex = null!;

    private List<Regex> IgnoreByNameRegex = null!;



    private CS1HiddenObjectInfo() { }

    internal bool IsHidden(string name, string itemClass)
    {
        if (itemClass != null)
        {
            foreach (Regex re in IgnoreByItemClassRegex!)
            {
                if (re.IsMatch(itemClass))
                {
                    return true;
                }
            }
        }
        if (name != null)
        {
            foreach (Regex re in IgnoreByNameRegex!)
            {
                if (re.IsMatch(name))
                {
                    return true;
                }
            }
        }
        return false;

    }

    private void PrepareRegex()
    {

        IgnoreByItemClassRegex = [];
        foreach (string s in IgnoreByItemClass)
        {
            IgnoreByItemClassRegex.Add(new Regex(s, RegexOptions.Compiled));
        }


        IgnoreByNameRegex = [];
        foreach (string s in IgnoreByName)
        {
            IgnoreByNameRegex.Add(new Regex(s, RegexOptions.Compiled));
        }

    }


    private static CS1HiddenObjectInfo CreateDefaultIgnoreBuildings()
    {
        CS1HiddenObjectInfo obj = new();
        obj.IgnoreByItemClass.AddRange([
            "^Electricity Wind Turbine$",
            "^Tsunami Buoy$",
            "^Radio$",
            "^Firewatch$",
            "^Earthquake Sensor$"]);
        obj.IgnoreByName.AddRange([
            "^(Road|Train|Metro|Ship|Airplane) Connection$",
            "^Boulder \\d+$",
            "^Rock (Area|Formation) \\d+( [A-Z])?$",
            "^Wildlife Spawn Point$",
            "^Ship Wreck \\d+$",
            "^Dam Power House$",
            "^Dam Node Building$",
            "^.* Ruins \\d+$",
            "^Abandoned .*\\d+$",
            "^Water (Intake|Outlet|Treatment Plant)$",
            "^Ancient Cemetery \\d+$",
            "^Power Relay$",
            "^Tetrapod 01 - 1x1$",
            "^R69 Tetrapods (Pile|Pattern)$",
            "^Cave \\d+$",
            "^Cliff \\d+$"
        ]);
        obj.PrepareRegex();
        return obj;
    }
    //#pragma warning disable CA1861

    private static CS1HiddenObjectInfo CreateDefaultIgnoreSegments()
    {
        CS1HiddenObjectInfo obj = new();
        obj.IgnoreByName.AddRange([
            "^Castle Wall.*$",
            "^Quay$",
            "^Trench Ruins \\d+$",
            "^Airplane Stop$"
        ]);
        obj.IgnoreByItemClass.AddRange([
            "^Landscaping Canal$",
            "^Landscaping Flood Wall$"]);
        obj.PrepareRegex();
        return obj;
    }

    private static CS1HiddenObjectInfo CreateDefaultIgnoreMapSymbols()
    {
        CS1HiddenObjectInfo obj = new();
        obj.IgnoreByName.AddRange([
            "^Power Relay$",
            "^Tetrapod 01 - 1x1$",
            "^R69 Tetrapods (Pile|Pattern)$",
            "^Radio Mast Tall - Wire Anchor$"]);
        obj.PrepareRegex();
        return obj;
    }
}

using System.Xml.Serialization;

namespace CS2MapView.Theme;
/// <summary>
/// 等高線を描画する標高とその描画情報
/// </summary>
public class ContourHeights
{
    /// <summary>
    /// 設定名
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// 地形
    /// </summary>
    public List<ContourStrokeStyle>? LandHeights { get; set; }
    /// <summary>
    /// 水場
    /// </summary>
    public List<ContourStrokeStyle>? WaterHeights { get; set; }

    /// <summary>
    /// 既定の選択可能項目
    /// </summary>
    [XmlIgnore]
    public static readonly IEnumerable<ContourHeights> DefaultCandidate = new List<ContourHeights>() { Default, Low, None };

    [XmlIgnore]
    private static Width NarrowWidth => new(0.7f, 1f, 0.7f, 0.7f, 0.5f);
    private static Width WideWidth => new(1f, 0.8f, 1f, 1.75f, 0.5f);
    private static float[] NarrowDashEffect => new[] { 1f, 2f };
    /// <summary>
    /// 詳細度：標準
    /// </summary>
    [XmlIgnore]
    public static ContourHeights Default
    {
        get
        {
            static IEnumerable<float> DefaultContourIntervals()
            {
                yield return -1000f;
                for (float f = -12.5f; f < 500f; f += 12.5f)
                {
                    yield return f;
                }
                for (float f = 500f; f <= 1000f; f += 25f)
                {
                    yield return f;
                }
            }

            return Create("default", DefaultContourIntervals());
        }
    }
    /// <summary>
    /// 詳細度：低
    /// </summary>
    [XmlIgnore]
    public static ContourHeights Low
    {
        get
        {
            static IEnumerable<float> LowContourIntervals()
            {
                yield return -1000f;
                yield return -12.5f;
                for (float f = 0f; f < 500f; f += 25f)
                {
                    yield return f;
                }
                for (float f = 500f; f <= 1000f; f += 50f)
                {
                    yield return f;
                }
            }
            return Create("low", LowContourIntervals());

        }
    }
    /// <summary>
    /// 等高線なし
    /// </summary>
    [XmlIgnore]
    public static ContourHeights None
    {
        get
        {
            return Create("none", Array.Empty<float>());
        }
    }


    private static ContourHeights Create(string name, IEnumerable<float> heights)
    {
        var landHeights = new List<ContourStrokeStyle>();
        foreach (var h in heights)
        {
            Width w;
            float[]? d = null;
            int hn = float.IsNegativeInfinity(h) ? int.MinValue : (int)h;
            if (hn % 100 == 0)
            {
                w = WideWidth;
            }
            else
            {
                w = NarrowWidth;
                if (hn % 50 != 0)
                {
                    d = NarrowDashEffect;
                }
            }
            var c = new ContourStrokeStyle(h, w) { DashEffect = d };
            landHeights.Add(c);
        }



        return new ContourHeights
        {
            Name = name,
            LandHeights = landHeights,
            WaterHeights =
            [
                new ContourStrokeStyle(0f, new Width(1f, 0.7f, 0.5f, 2f, 0.1f))
            ]
        };
    }
}

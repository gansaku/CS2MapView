using CSLMapView.XML;

namespace CS2MapView.Util.CS1;

/// <summary>
/// [後方互換用 ver.3.61以降のファイルに対しては使用不可]
/// </summary>
internal enum CS1ClassicalRoadWidthType
{
    Undefined = 0,
    Normal,
    Narrow,
    Wide,
    VeryWide,
    HighwayRamp,
    Highway,
    HighwayWide,
    Beautification
}
/// <summary>
/// [後方互換用 ver.3.61以降のファイルに対しては使用不可]
/// </summary>
internal static class CS1ClassicalRoadWidthManager
{
    internal static CS1ClassicalRoadWidthType GetWidthType(XMLSegment n)
    {
        WayType type = n.ClassicalWayType;

        string name = n.Name;

        if (type.HasBit(WayType.Road))
        {
            if (type.HasBit(WayType.Highway))
            {
                if (name.StartsWith("HighwayRamp"))
                {
                    return CS1ClassicalRoadWidthType.HighwayRamp;
                }
                else if (name.StartsWith("Two Lane Highway") || name.Contains("Rural Highway", StringComparison.InvariantCulture))
                {
                    return CS1ClassicalRoadWidthType.Highway;
                }
                else
                {
                    return CS1ClassicalRoadWidthType.HighwayWide;
                }

            }
            else
            {
                if (name.StartsWith("WideAvenue"))
                {
                    return CS1ClassicalRoadWidthType.VeryWide;
                }
                else if (name.StartsWith("Four-Lane") || name.StartsWith("Six-Lane") || name.StartsWith("Eight-Lane")
                     || name.StartsWith("Large") || name.StartsWith("Medium") || name.StartsWith("Avenue Large")
                     || name.StartsWith("FourDevidedLaneAvenue") || name.StartsWith("AsymAvenueL2R3") || name.StartsWith("AsymAvenueL2R4"))
                {
                    return CS1ClassicalRoadWidthType.Wide;
                }
                else if (name.StartsWith("One-Lane") && !name.StartsWith("One-Lane Oneway with bicycle lanes and parking")
                    || name.StartsWith("Zonable Pedestrian") || name == "Two-Lane Oneway" || name == "Two-Lane Alley" || name.StartsWith("Gravel Road") || name == "PlainStreet2L")
                {
                    return CS1ClassicalRoadWidthType.Narrow;
                }
                else
                {
                    return CS1ClassicalRoadWidthType.Normal;
                }
            }
        }
        else if (type.HasBit(WayType.Beautification))
        {
            return CS1ClassicalRoadWidthType.Beautification;
        }
        return CS1ClassicalRoadWidthType.Undefined;
    }
}

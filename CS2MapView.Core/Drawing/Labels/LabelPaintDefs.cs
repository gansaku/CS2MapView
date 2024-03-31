using CS2MapView.Theme;
using SkiaSharp;
using System.Diagnostics;

namespace CS2MapView.Drawing.Labels
{
    internal class LabelPaintDefs
    {
        internal SKPaint? BuildingNameStroke;
        internal SKPaint? BuildingNameFill;
        internal SKPaint? DistrictNameStroke;
        internal SKPaint? DistrictNameFill;
        internal SKPaint? StreetNameStroke;
        internal SKPaint? StreetNameFill;

        internal LabelPaintDefs(StringTheme? tr, float scale, float worldScale)
        {
            Init(tr, scale, worldScale);
        }

        internal void Init(StringTheme? tr, float scale, float worldScale)
        {
            if (tr?.Colors is null)
            {
                return;
            }

            TextStyleWithColor? GetWithColor(StringStyle? ts, SKColor? fillColor, SKColor? strokeColor)
            {
                if (ts?.FontFamily is null || ts?.FontSize is null)
                {
                    return null;
                }
                return new TextStyleWithColor(ts, fillColor, strokeColor);
            }

            SKPaint? GetStrokePaint(TextStyleWithColor? twc)
            {
                if (twc is null)
                {
                    return null;
                }
                var key = twc.ToStrokeCacheKey(scale, worldScale);
                if (key is null)
                {
                    Debug.Print($"Init.GetStrokePaint key is null");
                    return null;
                }
                return SKPaintCache.Instance.GetTextStroke(key);
            }
            SKPaint? GetFillPaint(TextStyleWithColor? twc)
            {
                if (twc is null)
                {
                    return null;
                }
                var key = twc.ToFillCacheKey(scale, worldScale);
                if (key is null)
                {
                    return null;
                }
                return SKPaintCache.Instance.GetTextFill(key);
            }


            var building = GetWithColor(tr.BuildingName, tr.Colors.BuildingNameFill, tr.Colors.BuildingNameStroke);
            BuildingNameStroke = GetStrokePaint(building);
            BuildingNameFill = GetFillPaint(building);
            var district = GetWithColor(tr.DistrictName, tr.Colors.DistrictNameFill, tr.Colors.DistrictNameStroke);
            DistrictNameStroke = GetStrokePaint(district);
            DistrictNameFill = GetFillPaint(district);
            var streetName = GetWithColor(tr.StreetName, tr.Colors.StreetNameFill, tr.Colors.StreetNameStroke);
            StreetNameStroke = GetStrokePaint(streetName);
            StreetNameFill = GetFillPaint(streetName);

        }
    }
}

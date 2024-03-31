using CS2MapView.Config;
using CS2MapView.Data;
using CS2MapView.Drawing.Buildings.CS1;
using CS2MapView.Util;
using CS2MapView.Util.CS1;
using CSLMapView.XML;
using SkiaSharp;
using System.Text.RegularExpressions;

namespace CS2MapView.Drawing.Labels.CS1
{
    internal partial class CS1LabelContentBuilder : AbstractLabelContentBuilder
    {

        private CSLExportXML ExportXML { get; init; }

        private CS1BuildingRenderingManager BuildingRenderingManager;


        internal CS1LabelContentBuilder(ICS2MapViewRoot appRoot, CSLExportXML xml, LabelContentsManager manager) : base(appRoot, manager)
        {
            ExportXML = xml;
            BuildingRenderingManager = new(appRoot);
        }


        private SKPoint ApplyRotateScaleTransform(ViewContext vc, XMLVector3 v)
        {
            var p = v.ToMapSpace();
            return vc.TextWorldTransform.MapPoint(p);
        }

        protected override void AddContents(ViewContext vc)
        {
            LabelContentManager.Contents.AddRange(BuildStreetNameLabels(vc));
            LabelContentManager.Contents.AddRange(BuildBuildingNameLabels(vc));
            LabelContentManager.Contents.AddRange(BuildDistrictNameLabels(vc));
        }



        private List<DistrictNameLabel> BuildDistrictNameLabels(ViewContext vc)
        {

            var result = new List<DistrictNameLabel>();
            if (!AppRoot.Context.UserSettings.RenderDistrictNameLabel)
            {
                return result;
            }
            var paint = FontPaintCache!.DistrictNameStroke ?? FontPaintCache.DistrictNameFill;
            if (paint is null)
            {
                return result;
            }
            foreach (var distr in ExportXML.DistrictList)
            {
                if (distr.Position.X == -4915.2f && distr.Position.Z == -4915.2f)
                {
                    continue;
                }
                var label = AbstractTextLabel.Create<DistrictNameLabel>(
                    distr.Name,
                    ApplyRotateScaleTransform(vc, distr.Position),
                    paint,
                    vc.ViewScaleFromWorld);
                result.Add(label);
            }
            return result;
        }


        private List<AbstractMapLabel> BuildBuildingNameLabels(ViewContext vc)
        {
            var p = MapSymbolPictureManager.Instance;
            var result = new List<AbstractMapLabel>();
            if (!AppRoot.Context.UserSettings.RenderBuildingNameLabel)
            {
                return result;
            }

            var mgr = new CS1BuildingRenderingManager(AppRoot);

            var paint = FontPaintCache!.BuildingNameStroke ?? FontPaintCache.BuildingNameFill;
            if (paint is null)
            {
                return result;
            }
            var transportList = new List<BuildingNameLabel>();

            var mapSymbolTargetSize = AppRoot.Context.Theme.MapSymbolSize.StrokeWidthByScale(vc.ScaleFactor, vc.WorldScaleFactor);

            foreach (var b in ExportXML.BuildingList)
            {
                var param = mgr.GetShapeRenderingParam(b.SteamId, b.ShortName, b.ItemClass, b.Service);
                bool hasName = false;
                BuildingNameLabel? label = null;
                //名称
                if (param.NameVisibility == NameVisibility.FullVisible || param.NameVisibility == NameVisibility.Visible && !string.IsNullOrEmpty(b.MyName))
                {
                    var name = b.MyName ?? b.ShortName;

                    if (!string.IsNullOrEmpty(name))
                    {

                        var center = ApplyRotateScaleTransform(vc, b.Points.Center());
                        label = AbstractTextLabel.Create<BuildingNameLabel>(name, center, paint, vc.ViewScaleFromWorld);

                        if (b.Service == "PublicTransport")
                        {
                            string trimedBuildingName = CleanUpTransportMyName(b.MyName);
                            if (trimedBuildingName.Length > 0)
                            {
                                label.Text = trimedBuildingName;
                                transportList.Add(label);
                                hasName = true;
                            }
                        }
                        else
                        {
                            result.Add(label);
                            hasName = true;
                        }
                    }
                }
                //地図記号
                if (AppRoot.Context.UserSettings.RenderMapSymbol && b.ParentBuildingId == null)
                {
                    var (reason, symbolName, svg) = BuildingRenderingManager.GetMapSymbol(b.SteamId, b.ShortName, b.ItemClass, b.Service, b.SubService);

                    if (svg is not null)
                    {
                        var origWidth = svg.Picture!.CullRect.Width;
                        var origHeight = svg.Picture!.CullRect.Height;

                        bool verticallyLong = origHeight > origWidth;


                        if (!mapSymbolTargetSize.HasValue)
                        {
                            continue;
                        }
                        float targetWidth, targetHeight, scale;
                        if (verticallyLong)
                        {
                            targetWidth = mapSymbolTargetSize.Value;
                            targetHeight = targetWidth * (origHeight / origWidth);
                            scale = targetWidth / origWidth;
                        }
                        else
                        {
                            targetHeight = mapSymbolTargetSize.Value;
                            targetWidth = targetHeight * (origWidth / origHeight);
                            scale = targetHeight / origHeight;
                        }

                        var symbol = new MapSymbol
                        {
                            Svg = svg,
                            OriginalPosition = ApplyRotateScaleTransform(vc, b.Points.Center()),
                            DisplayPosition = ApplyRotateScaleTransform(vc, b.Points.Center()),
                            Size = (targetWidth * vc.ViewScaleFromWorld, targetHeight * vc.ViewScaleFromWorld),
                            ImageScale = scale
                        };

                        if (hasName)
                        {
                            label!.MoveOriginalPosition(0, -targetHeight / 2);
                            symbol.MoveOriginalPosition(0, targetHeight / 2);
                        }

                        result.Add(symbol);
                    }
                }
            }

            //交通機関の建物（駅を想定）は同名の場合統合して中央に表示
            foreach (var g in transportList.GroupBy(t => t.Text))
            {
                var obj = g.First();
                obj.OriginalPosition = new SKPoint(g.Average(t => t.OriginalPosition.X), g.Average(t => t.OriginalPosition.Y));
                result.Add(obj);
            }
            return result;
        }
        /// <summary>
        /// 道路名
        /// </summary>
        /// <returns></returns>
        private List<StreetNameLabel> BuildStreetNameLabels(ViewContext vc)
        {
            var result = new List<StreetNameLabel>();
            if (!AppRoot.Context.UserSettings.RenderStreetNames)
            {
                return result;
            }
            var paint = FontPaintCache!.StreetNameStroke ?? FontPaintCache.StreetNameFill;
            if (paint is null)
            {
                return result;
            }
            foreach (XMLSegment seg in ExportXML.SegmentList)
            {
                if (!string.IsNullOrEmpty(seg.CustomName))
                {
                    SKRect textBound = new();
                    paint.MeasureText(seg.CustomName, ref textBound);

                    var textWidth = textBound.Width * vc.ViewScaleFromWorld;
                    var textHeight = textBound.Height * vc.ViewScaleFromWorld;
                    double segmentLength = 0d;
                    for (int i = 1; i < seg.Points.Count; i++)
                    {
                        segmentLength += Math.Sqrt(CSLMapViewCompatibility.DistanceXZSqr(seg.Points[i - 1], seg.Points[i]));

                    }

                    if (segmentLength * vc.ViewScaleFromWorld < textWidth)
                    {
                        continue;
                    }


                    double current = 0d;
                    int idx = -1;
                    double currentNodeLength = 0d;
                    for (int i = 1; i < seg.Points.Count; i++)
                    {
                        currentNodeLength = Math.Sqrt(CSLMapViewCompatibility.DistanceXZSqr(seg.Points[i - 1], seg.Points[i]));
                        current += currentNodeLength;
                        if (current >= segmentLength / 2)
                        {
                            idx = i - 1;
                            break;
                        }
                    }
                    /*
                    if (CslMapViewCompatibility.DistanceXZSqr(seg.Points[idx], seg.Points[idx + 1]) < 8 * 8)
                    {
                        continue;

                    }*/
                    double angleOrig = CSLMapViewCompatibility.AngleXZ(seg.Points[idx], seg.Points[idx + 1]);
                    double leftLength = segmentLength / 2 - (current - currentNodeLength);
                    XMLVector3 centerPoint = new((float)(seg.Points[idx].X + leftLength * Math.Cos(angleOrig)), 0,
                        (float)(seg.Points[idx].Z + leftLength * Math.Sin(angleOrig)));

                    double wAngle = MathEx.Angle(seg.Points[idx].ToMapSpace(), seg.Points[idx + 1].ToMapSpace());
                    bool reverse = ((2 * Math.PI + wAngle + vc.Angle) % (Math.PI * 2)) switch
                    {
                        >= Math.PI / 2 and < Math.PI * 3 / 2 => true,
                        _ => false
                    };
                    // XMLVector3 center = new XMLVector3((seg.Points[idx].X + seg.Points[idx + 1].X) / 2, 0, (seg.Points[idx].Z + seg.Points[idx + 1].Z) / 2);
                    IEnumerable<SKPoint> pointsen = seg.Points.Select(t => ApplyRotateScaleTransform(vc, t));
                    SKPoint[] points = (reverse ? pointsen.Reverse() : pointsen).ToArray();

                    StreetNameLabel st = new()
                    {
                        Text = seg.CustomName,
                        OriginalPosition = ApplyRotateScaleTransform(vc, centerPoint), /*new SKPoint(points.Average(t => t.X), points.Average(t => t.Y))*/
                        DisplayPosition = ApplyRotateScaleTransform(vc, centerPoint),
                        PathPoints = points,
                        Ascent = textBound.Top * vc.ViewScaleFromWorld,
                        Size = (textWidth, textHeight),
                        TextOffset = (float)((segmentLength * vc.ViewScaleFromWorld - textWidth) / 4f) // /4は謎
                    };

                    result.Add(st);

                }
            }
            return result;
        }


        private string CleanUpTransportMyName(string? name)
        {
            if (name == null)
            {
                return string.Empty;
            }
            string stripped = TransportNameFormatRegex().Replace(name, string.Empty);
            return stripped.Trim();
        }


        [GeneratedRegex("\\([^\\)]*\\)")]
        private static partial Regex TransportNameFormatRegex();


    }
}

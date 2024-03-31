using CS2MapView.Data;
using CS2MapView.Drawing.Buildings.CS2;
using CS2MapView.Serialization;
using CS2MapView.Util;
using CS2MapView.Util.CS1;
using CS2MapView.Util.CS2;
using SkiaSharp;
using System.Diagnostics;
using System.Numerics;

namespace CS2MapView.Drawing.Labels.CS2
{
    internal class CS2LabelContentBuilder : AbstractLabelContentBuilder
    {

        private CS2MapDataSet ExportData { get; init; }
        private CS2BuildingRenderingManager BuildingRenderingManager { get; init; }

        internal CS2LabelContentBuilder(ICS2MapViewRoot appRoot, CS2MapDataSet exportData, LabelContentsManager labelContentManager)
            : base(appRoot, labelContentManager)
        {
            ExportData = exportData;
            BuildingRenderingManager = new(appRoot);
        }
        protected override void AddContents(ViewContext vc)
        {
            LabelContentManager.Contents.AddRange(BuildDistrictNameLabels(vc));
            LabelContentManager.Contents.AddRange(BuildBuildingNameLabels(vc));
            LabelContentManager.Contents.AddRange(BuildStreetNameLabels(vc));
        }

        private SKPoint ApplyRotateScaleTransform(ViewContext vc, SKPoint p)
        {
            return vc.TextWorldTransform.MapPoint(p);
        }
        private SKPoint ApplyRotateScaleTransform(ViewContext vc, Vector3 vec)
        {
            var p = vec.ToMapSpace();
            return vc.TextWorldTransform.MapPoint(p);
        }

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
            foreach (var seg in ExportData.RoadInfo?.RoadSegments ?? [])
            {
                if (string.IsNullOrEmpty(seg.CustomName))
                {
                    continue;
                }

                SKRect textBound = new();
                paint.MeasureText(seg.CustomName, ref textBound);

                var textWidth = textBound.Width * vc.ViewScaleFromWorld;
                var textHeight = textBound.Height * vc.ViewScaleFromWorld;

                if (seg.Length * vc.ViewScaleFromWorld < textWidth)
                {
                    continue;
                }

                SKPoint start = new(seg.Curve.X0, seg.Curve.Y0);
                SKPoint end = new(seg.Curve.X3, seg.Curve.Y3);
                double angle = MathEx.Angle(start, end);
                bool reverse = ((2 * Math.PI + angle + vc.Angle) % (Math.PI * 2)) switch
                {
                    >= Math.PI / 2 and < Math.PI * 3 / 2 => true,
                    _ => false
                };
                SKPoint[] pathPoints;
                if (reverse)
                {
                    pathPoints = vc.TextWorldTransform.MapPoints(seg.Curve.ToReversedPointArray());
                }
                else
                {
                    pathPoints = vc.TextWorldTransform.MapPoints(seg.Curve.ToPointArray());
                }

                using var tempPath = new SKPath();
                tempPath.MoveTo(pathPoints[0]);
                tempPath.CubicTo(pathPoints[1], pathPoints[2], pathPoints[3]);
                var tightBounds = tempPath.TightBounds;
                StreetNameLabel st = new()
                {
                    Text = seg.CustomName,
                    OriginalPosition = tightBounds.Center(), /*new SKPoint(points.Average(t => t.X), points.Average(t => t.Y))*/
                    DisplayPosition = tightBounds.Center(),
                    PathPoints = pathPoints,
                    PathType = AbstractTextOnPathLabel.PathPointType.CubicBesier,
                    Ascent = textBound.Top * vc.ViewScaleFromWorld,
                    Size = (tightBounds.Width, tightBounds.Height),
                    TextOffset = (seg.Length * vc.ViewScaleFromWorld - textWidth) / 4f // /4は謎
                };

                result.Add(st);
            }
            return result;
        }

        private List<AbstractMapLabel> BuildBuildingNameLabels(ViewContext vc)
        {
            var result = new List<AbstractMapLabel>();

            if (!AppRoot.Context.UserSettings.RenderBuildingNameLabel && !AppRoot.Context.UserSettings.RenderMapSymbol)
            {
                return result;
            }
            var paint = FontPaintCache!.BuildingNameStroke ?? FontPaintCache.BuildingNameFill;
            if (paint is null)
            {
                return result;
            }
            var mapSymbolTargetSize = AppRoot.Context.Theme.MapSymbolSize.StrokeWidthByScale(vc.ScaleFactor, vc.WorldScaleFactor);

            foreach (var bld in ExportData.Buildings ?? [])
            {
                BuildingNameLabel? label = null;
                SKPoint buildingPos = ApplyRotateScaleTransform(vc, bld.Position);
                CS2BuildingPrefab? prefab = null;
                if (AppRoot.Context.UserSettings.RenderBuildingNameLabel)
                {
                    if (!AppRoot.Context.UserSettings.UsePrefabBuildingName && bld.IsCustomName && !string.IsNullOrEmpty(bld.Name))
                    {
                        label = AbstractTextLabel.Create<BuildingNameLabel>(
                           bld.Name,
                           buildingPos,
                           paint,
                           vc.ViewScaleFromWorld);
                        result.Add(label);
                    }
                    else if (AppRoot.Context.UserSettings.UsePrefabBuildingName)
                    {
                        prefab = ExportData.BuildingPrefabs?.FirstOrDefault(t => t.Entity == bld.Prefab);
                        if (prefab is not null && !string.IsNullOrEmpty(prefab.Name))
                        {
                            label = AbstractTextLabel.Create<BuildingNameLabel>(
                                  prefab.Name,
                                  buildingPos,
                                  paint,
                                  vc.ViewScaleFromWorld);
                            result.Add(label);
                        }


                    }

                }
                if (AppRoot.Context.UserSettings.RenderMapSymbol && bld.ParentBuilding < 0)
                {
                    prefab ??= ExportData.BuildingPrefabs?.FirstOrDefault(t => t.Entity == bld.Prefab);
                    var (reason, symbolName, svg) = BuildingRenderingManager.GetMapSymbol(bld, prefab);
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
                            OriginalPosition = buildingPos,
                            DisplayPosition = buildingPos,
                            Size = (targetWidth * vc.ViewScaleFromWorld, targetHeight * vc.ViewScaleFromWorld),
                            ImageScale = scale
                        };

                        if (label is not null)
                        {
                            float h = Math.Max(label?.Size.height ?? 0, targetHeight * vc.ViewScaleFromWorld) * 1.1f;

                            label?.MoveOriginalPosition(0, -h / 2);
                            symbol.MoveOriginalPosition(0, h / 2);
                        }

                        result.Add(symbol);
                    }

                }

            }
            return result;
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
            foreach (var distr in ExportData.Districts ?? [])
            {
                if (string.IsNullOrEmpty(distr.Name))
                {
                    continue;
                }
                var label = AbstractTextLabel.Create<DistrictNameLabel>(
                    distr.Name,
                    ApplyRotateScaleTransform(vc, distr.CenterPosition),
                    paint,
                    vc.ViewScaleFromWorld);
                result.Add(label);
            }
            return result;
        }


    }
}

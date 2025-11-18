using CS2MapView.Data;
using CS2MapView.Drawing.Buildings.CS2;
using CS2MapView.Serialization;
using CS2MapView.Util;
using CS2MapView.Util.CS1;
using CS2MapView.Util.CS2;
using SkiaSharp;
using System.Numerics;
using log4net;

namespace CS2MapView.Drawing.Labels.CS2
{
    internal class CS2LabelContentBuilder : AbstractLabelContentBuilder
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CS2LabelContentBuilder));
        
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
            float samplingRatio = CalculateLabelSamplingRatio(vc);
            
            var districts = BuildDistrictNameLabels(vc, samplingRatio);
            var buildings = BuildBuildingNameLabels(vc, samplingRatio);
            var streets = BuildStreetNameLabels(vc, samplingRatio);
            
            LabelContentManager.Contents.AddRange(districts);
            LabelContentManager.Contents.AddRange(buildings);
            LabelContentManager.Contents.AddRange(streets);
        }
        
        private float CalculateLabelSamplingRatio(ViewContext vc)
        {
            var textRect = vc.TextWorldRect;
            float currentArea = textRect.Width * textRect.Height;
            
            const float BaseMapArea = 14336f * 14336f;
            const float MaxReasonableArea = BaseMapArea * 16f;
            
            if (currentArea > MaxReasonableArea)
            {
                float ratio = (float)Math.Sqrt(MaxReasonableArea / currentArea);
                return Math.Max(0.25f, ratio);
            }
            
            return 1.0f;
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

        private List<StreetNameLabel> BuildStreetNameLabels(ViewContext vc, float samplingRatio = 1.0f)
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
            
            var segments = ExportData.RoadInfo?.RoadSegments ?? [];
            int step = (int)Math.Ceiling(1.0f / samplingRatio);
            int index = 0;
            
            foreach (var seg in segments)
            {
                if (samplingRatio < 1.0f && (index++ % step) != 0)
                {
                    continue;
                }
                
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
                    OriginalPosition = tightBounds.Center(),
                    DisplayPosition = tightBounds.Center(),
                    PathPoints = pathPoints,
                    PathType = AbstractTextOnPathLabel.PathPointType.CubicBesier,
                    Ascent = textBound.Top * vc.ViewScaleFromWorld,
                    Size = (tightBounds.Width, tightBounds.Height),
                    TextOffset = (seg.Length * vc.ViewScaleFromWorld - textWidth) / 4f
                };

                result.Add(st);
            }
            return result;
        }

        private List<AbstractMapLabel> BuildBuildingNameLabels(ViewContext vc, float samplingRatio = 1.0f)
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

            var buildings = ExportData.Buildings ?? [];
            int step = (int)Math.Ceiling(1.0f / samplingRatio);
            int index = 0;
            
            foreach (var bld in buildings)
            {
                bool isImportantBuilding = AppRoot.Context.UserSettings.RenderMapSymbol && bld.ParentBuilding < 0;
                
                if (!isImportantBuilding && samplingRatio < 1.0f && (index++ % step) != 0)
                {
                    continue;
                }
                
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
                if (isImportantBuilding)
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

        private List<DistrictNameLabel> BuildDistrictNameLabels(ViewContext vc, float samplingRatio = 1.0f)
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

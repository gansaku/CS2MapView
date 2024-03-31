using CS2MapView.Data;
using CS2MapView.Drawing.Layer;
using CS2MapView.Serialization;
using CS2MapView.Theme;
using CS2MapView.Util;
using CS2MapView.Util.CS2;
using SkiaSharp;

namespace CS2MapView.Drawing.Buildings.CS2
{
    public class CS2BuildingLayerBuilder(ICS2MapViewRoot AppRoot, CS2MapDataSet ExportData)
    {
        private BasicLayer ResultLayer { get; init; } = new BasicLayer(AppRoot, ILayer.LayerNameBuildings, CS2MapType.CS2WorldRect);

        public Task<ILayer> BuildAsync(LoadProgressInfo? loadProgressInfo)
        {
            return Task.Run<ILayer>(() =>
            {
                var strokeStyle = AppRoot.Context.Theme.Strokes?.Building;
                var borderColor = AppRoot.Context.Theme.Colors?.BuildingBorder;
                StrokeStyleWithColor? sswc = null;
                if (strokeStyle is not null && borderColor.HasValue && !AppRoot.Context.UserSettings.DisableBuildingBorder)
                {
                    sswc = strokeStyle.WithColor(borderColor.Value);

                }

                List<IDrawCommand> surfacePathes = [];
                List<IDrawCommand> buildingsPathes = [];

                var buildingTargets = (ExportData.Buildings ?? []).Where(t =>
                    !(t.BuildingType.HasFlag(CS2BuildingTypes.ExtractorBuilding) && !t.BuildingType.HasFlag(CS2BuildingTypes.IndustrialBuilding)));

                var subBuildings = buildingTargets.Where(t => buildingTargets.FirstOrDefault(s=>s.Entity==t.ParentBuilding) is not null ).ToList();
                var groupedBuildings = buildingTargets.Except(subBuildings).Select(
                    (CS2Building parent,List<CS2Building> children)(t) => (t, new List<CS2Building>())).ToList();
                subBuildings.ForEach(sb => groupedBuildings.First(t => t.parent.Entity == sb.ParentBuilding).children.Add(sb));

                

                foreach (var (parent, children) in groupedBuildings)
                {
                    if (AppRoot.Context.UserSettings.HideRICOBuildingsShape)
                    {
                        if((parent.BuildingType & (CS2BuildingTypes.ResidentialBuilding | CS2BuildingTypes.CommercialBuilding | CS2BuildingTypes.IndustrialBuilding | CS2BuildingTypes.OfficeBuilding)) != 0)
                        {
                            continue;
                        }
                    }
                    var color = ServiceToFillColor(parent.BuildingType);

                    var path = parent.AreaGeometry?.ToPath();
                    if (path is not null && parent.BuildingType.HasFlag(CS2BuildingTypes.ExtractorBuilding))
                    {
                        var transColor = color.WithAlpha(127);

                        surfacePathes.Add(new PathDrawCommand
                        {
                            Path = path,
                            StrokePaintFunc = sswc is null ? null : sswc.ToCacheKey,
                            FillColor = transColor
                        });
                    }
                  //  if ((parent.BuildingType & CS2BuildingTypes.TransportStation) == 0)
                    {
                        List<CS2Building> rectsTarget = [parent,..children];
                        List<SKPath> pathElements = [];
                     
                        foreach(var b in rectsTarget)
                        {
                            var prefab = ExportData.BuildingPrefabs?.FirstOrDefault(t => t.Entity == b.Prefab);

                            var pos = b.Position.ToMapSpace();
                            if (prefab is null)
                            {
                                //TODO log
                                continue;
                            }
                            var size = prefab.Size.ToMapSpace();
                           
                            SKRect r = new(pos.X - size.X / 2, pos.Y - size.Y / 2, pos.X + size.X / 2, pos.Y + size.Y / 2);
                           
                            var path2 = SKPathEx.RectToPath(r);
                            var tr = SKMatrix.CreateRotation(b.Rotation.RotationY(), pos.X, pos.Y);
                            path2.Transform(tr);
                            pathElements.Add(path2);
                         
                        }

                        SKPath? buildingPath = null;
                        if (pathElements.Count > 1)
                        {
                               
                            buildingPath = SKPathEx.Union(pathElements);
                            foreach(var p in pathElements)
                            {
                                p.Dispose();
                            }
                           
                        }
                        else if(pathElements.Count == 0)
                        {
                            continue;
                            //TODO log
                        }
                        else
                        {
                            buildingPath = pathElements[0];
                        }
                        /*
                        var prefab = ExportData.BuildingPrefabs?.FirstOrDefault(t => t.Entity == bi.Prefab);
                        if (prefab is null)
                        {
                            //TODO log
                            continue;
                        }
                        var size = prefab.Size.ToMapSpace();
                        SKRect r = new(pos.X - size.X / 2, pos.Y - size.Y / 2, pos.X + size.X / 2, pos.Y + size.Y / 2);
                        var path2 = SKPathEx.RectToPath(r);

                        var tr = SKMatrix.CreateRotation(bi.Rotation.RotationY(), pos.X, pos.Y);
                        path2.Transform(tr);
                        */
                        buildingsPathes.Add(new PathDrawCommand
                        {
                            Path = buildingPath,
                            StrokePaintFunc = sswc is null ? null : sswc.ToCacheKey,// (s, ws) => new(2f, SKColors.Red, Theme.StrokeType.Flat),
                            FillColor = color

                        });
                    }






                }
                ResultLayer.DrawCommands.AddRange(surfacePathes);
                ResultLayer.DrawCommands.AddRange(buildingsPathes);


                loadProgressInfo?.Progress(this, LoadProgressInfo.Process.BuildBuildings, 1f, null);
                return ResultLayer;
            });
        }
        private SKColor ServiceToFillColor(CS2BuildingTypes types)
        {
            if (AppRoot.Context.Theme.Colors is null)
            {
                return SKColors.Gray;
            }
            ColorRules colors = AppRoot.Context.Theme.Colors;

            if (types.HasFlag(CS2BuildingTypes.ResidentialBuilding))
            {
                return colors.ResidentialBuilding;
            }
            else if (types.HasFlag(CS2BuildingTypes.CommercialBuilding))
            {
                return colors.CommercialBuilding;
            }
            else if (types.HasFlag(CS2BuildingTypes.IndustrialBuilding))
            {
                return colors.IndustrialBuilding;
            }
            else if (types.HasFlag(CS2BuildingTypes.OfficeBuilding))
            {
                return colors.OfficeBuilding;
            }
            else if (types.HasFlag(CS2BuildingTypes.TransportStation))
            {
                return colors.PublicTransportBuilding;
            }
            else if (types.HasFlag(CS2BuildingTypes.Park))
            {
                return colors.Beautification;
            }
            else
            {
                return colors.PublicBuilding;
            }

        }
    }
}

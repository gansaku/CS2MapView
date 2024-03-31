using CS2MapView.Serialization;
using Svg.Skia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2MapView.Drawing.Labels.CS2
{
    internal static class CS2MapSymbolDefs
    {
        internal static (string? name, SKSvg? svg) GetDefaultSymbolImage(string? defaultType, CS2Building building, CS2BuildingPrefab? prefab)
        {
            if (defaultType is null)
            {
                return (null, null);
            }
            if (defaultType == "maki")
            {
                return GetMakiSymbolImage(building, prefab);

            }
            else if (defaultType == "ja")
            {
                return GetJaSymbolImage(building, prefab);

            }
            else if (defaultType == "ja+")
            {
                return GetJaplusSymbolImage(building, prefab);
            }

            return (null, null);

        }

        private static (string? name, SKSvg? svg) GetJaSymbolImage(CS2Building building, CS2BuildingPrefab? prefab)
        {
            var dir = MapSymbolPictureManager.Instance.GetSymbols("ja");
            if (dir is null)
            {
                return (null, null);
            }

            (string?, SKSvg?) GetSafeImage(string? name)
            {
                if (name is null)
                {
                    return (null, null);
                }
                if (dir.TryGetValue(name, out var symbol))
                {
                    return (name, symbol);
                }
                return (null, null);
            }


            if (building.BuildingType.HasFlag(CS2BuildingTypes.School))
            {
                return GetSafeImage((prefab?.SubInfo?.EducationLevel ?? 0) switch
                {
                    1 => "elementarySchool",
                    2 => "highSchool",
                    > 2 => "university",
                    _ => null
                });
               
            }
            else if (prefab?.Name?.Contains("CityHall") ?? false)
            {
                return GetSafeImage("cityHall");
            }

            else if (building.BuildingType.HasFlag(CS2BuildingTypes.FireStation))
            {
                return GetSafeImage("fireStation");
            }
            else if (building.BuildingType.HasFlag(CS2BuildingTypes.PoliceStation))
            {
                return GetSafeImage("policeStation");
            }
            else if (prefab?.Name?.Contains("Cemetery") ?? false)
            {
                return GetSafeImage("graveyard");
            }

            else if (building.BuildingType.HasFlag(CS2BuildingTypes.PostFacility))
            {
                return GetSafeImage("postOffice");
            }
            else if (building.BuildingType.HasFlag(CS2BuildingTypes.Hospital))
            {
                return GetSafeImage("hospital");
            }
            else if (building.BuildingType.HasFlag(CS2BuildingTypes.PowerPlant))
            {
                if (prefab?.Name?.Contains("WindTurbine") ?? false)
                {
                    return GetSafeImage("windmill");
                }
                return GetSafeImage("powerPlant");
            }
            else if (prefab?.Name?.Contains("TelecomTower") ?? false)
            {
                return GetSafeImage("electricWaveTower");
            }
            else if (prefab?.Name?.Contains("RadioMast") ?? false)
            {
                return GetSafeImage("electricWaveTower");
            }
            else if (prefab?.Name == "Agriculture Area Placeholder - Vegetable")
            {
                return GetSafeImage("field");
            }
            else if (prefab?.Name == "Agriculture Area Placeholder - Grain")
            {
                return GetSafeImage("riceField");
            }
            else if (prefab?.Name == "Ore Area Placeholder - Stone")
            {
                return GetSafeImage("quarry");
            }
            else if (prefab?.Name == "Ore Area Placeholder - Coal" || prefab?.Name == "Ore Area Placeholder - Ore")
            {
                return GetSafeImage("mine");
            }
            else if (prefab?.Name == "Oil Area Placeholder")
            {
                return GetSafeImage("oilWell");
            }
            else if (prefab?.Name == "Forestry Area Placeholder")
            {
                return GetSafeImage("coniferousTrees");
            }
            else if (prefab?.Name?.Contains("Harbor") ?? false)
            {
                return GetSafeImage("localPort");
            }
            else if (prefab?.Name?.Contains("FirewatchTower") ?? false)
            {
                return GetSafeImage("highTower");
            }

            //空港はない


            return (null, null);
        }

        private static (string? name, SKSvg? svg) GetMakiSymbolImage(CS2Building building, CS2BuildingPrefab? prefab)
        {
            var dir = MapSymbolPictureManager.Instance.GetSymbols("maki");
            if (dir is null)
            {
                return (null, null);
            }

            (string?, SKSvg?) GetSafeImage(string? name)
            {
                if (name is null)
                {
                    return (null, null);
                }
                if (dir.TryGetValue(name, out var symbol))
                {
                    return (name, symbol);
                }
                return (null, null);
            }


            if (building.BuildingType.HasFlag(CS2BuildingTypes.School))
            {
                return GetSafeImage((prefab?.SubInfo?.EducationLevel ?? 0) switch
                {
                    1 => "school",
                    2 => "school",
                    > 2 => "college",
                    _ => null
                });
            }
            if (prefab?.Name?.Contains("Airport") ?? false)
            {
                return GetSafeImage("airport");
            }
            else if (building.BuildingType.HasFlag(CS2BuildingTypes.FireStation))
            {
                return GetSafeImage("fire-station");
            }
            else if (building.BuildingType.HasFlag(CS2BuildingTypes.PoliceStation))
            {
                return GetSafeImage("police");
            }
            else if (prefab?.Name?.Contains("Prison") ?? false)
            {
                return GetSafeImage("prison");
            }
            else if (prefab?.Name?.Contains("Cemetery") ?? false)
            {
                return GetSafeImage("cemetery");
            }
            else if (building.BuildingType.HasFlag(CS2BuildingTypes.PostFacility))
            {
                return GetSafeImage("post");
            }
            else if (building.BuildingType.HasFlag(CS2BuildingTypes.Hospital))
            {
                return GetSafeImage("hospital");
            }
            else if (building.BuildingType.HasFlag(CS2BuildingTypes.GarbageFacility))
            {
                return GetSafeImage("recycling");
            }
            else if (prefab?.Name?.Contains("Harbor") ?? false)
            {
                return GetSafeImage("harbor");
            }
            else if (prefab?.Name?.Contains("RadioMast") ?? false)
            {
                return GetSafeImage("communications-tower");
            }
            else if (prefab?.Name?.Contains("WindTurbine") ?? false)
            {
                return GetSafeImage("windmill");
            }
            else if (prefab?.Name?.Contains("TelecomTower") ?? false)
            {
                return GetSafeImage("communications-tower");
            }

          
            return (null, null);

        }
        private static (string? name, SKSvg? svg) GetJaplusSymbolImage(CS2Building building, CS2BuildingPrefab? prefab)
        {
            var dir = MapSymbolPictureManager.Instance.GetSymbols("maki");
            if (dir is null)
            {
                return (null, null);
            }

            (string?, SKSvg?) GetSafeImage(string name)
            {
                if (name is null)
                {
                    return (null, null);
                }
                if (dir.TryGetValue(name, out var symbol))
                {
                    return (name, symbol);
                }
                return (null, null);
            }

            if (prefab?.Name?.Contains("Airport") ?? false)
            {
                return GetSafeImage("airport");
            }
            else if (building.BuildingType.HasFlag(CS2BuildingTypes.GarbageFacility))
            {
                return GetSafeImage("recycling");
            }
            else if (prefab?.Name?.Contains("Prison") ?? false)
            {
                return GetSafeImage("prison");
            }

            return GetJaSymbolImage(building, prefab);

        }
    }
}

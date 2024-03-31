using Svg.Skia;

namespace CS2MapView.Drawing.Labels.CS1
{
    /// <summary>
    /// CS1用のデフォルト地図記号定義
    /// </summary>
    internal static class CS1MapSymbolDefs
    {
        private static (string? name, SKSvg? svg) GetJaSymbolImage(string name, string itemClass, string service, string subService)
        {
            var dir = MapSymbolPictureManager.Instance.GetSymbols("ja");
            if (dir is null)
            {
                return (null, null);
            }

            (string?, SKSvg?) GetSafeImage(string name)
            {
                if (dir.TryGetValue(name, out var symbol))
                {
                    return (name, symbol);
                }
                return (null, null);
            }

            if (name.StartsWith("Castle Ruins "))
            {
                return GetSafeImage("castle");
            }
            else if (name.StartsWith("Ancient Cemetery ") || name.StartsWith("Ship Wreck "))
            {
                return GetSafeImage("historicalSite");
            }
            else if (itemClass == "Firewatch")
            {
                return GetSafeImage("highTower");
            }
            else if (itemClass == "DeathCare Facility")
            {
                return GetSafeImage("graveyard");
            }
            else if (itemClass == "Electricity Wind Turbine")
            {
                return GetSafeImage("windmill");
            }
            else if ((itemClass == "Ship Cargo Facility" || itemClass == "Ship Facility"))
            {
                return GetSafeImage("localPort");
            }
            else if (itemClass == "Weather Radar")
            {
                return GetSafeImage("meteorologicalObservatory");
            }
            else if (itemClass == "Radio" || itemClass == "Space Radar")
            {
                return GetSafeImage("electricWaveTower");
            }
            else if (service == "FireDepartment")
            {
                return GetSafeImage("fireStation");
            }
            else if (service == "HealthCare")
            {
                return GetSafeImage("hospital");
            }
            else if (service == "PoliceDepartment")
            {
                return GetSafeImage("policeStation");
            }
            else if (service == "Education")
            {

                if (itemClass == "Elementary School")
                {
                    return GetSafeImage("elementarySchool");
                }
                else if (itemClass == "High School")
                {
                    return GetSafeImage("highSchool");

                }
                else
                {
                    return GetSafeImage("university");
                }
            }
            else if (service == "Electricity" && name != "Electricity Pole" && name != "Dam Power House")
            {
                return GetSafeImage("powerPlant");
            }
            else if (subService == "PublicTransportPost")
            {
                return GetSafeImage("postOffice");
            }
            return (null, null);
        }

        private static (string? name, SKSvg? svg) GetMakiSymbolImage(string name, string itemClass, string service, string subService)
        {
            var dir = MapSymbolPictureManager.Instance.GetSymbols("maki");
            if (dir is null)
            {
                return (null, null);
            }
            (string?, SKSvg?) GetSafeImage(string name)
            {
                if (dir.TryGetValue(name, out var symbol))
                {
                    return (name, symbol);
                }
                return (null, null);
            }
            if (name.StartsWith("Castle Ruins "))
            {
                return GetSafeImage("castle");
            }
            else if (name.StartsWith("Ancient Cemetery ") || name.StartsWith("Ship Wreck "))
            {
                return GetSafeImage("attraction");
            }
            else if (itemClass == "DeathCare Facility")
            {
                return GetSafeImage("cemetery");
            }
            else if ((itemClass == "Ship Cargo Facility" || itemClass == "Ship Facility"))
            {
                return GetSafeImage("harbor");
            }
            else if (service == "FireDepartment")
            {
                return GetSafeImage("fire-station");
            }
            else if (service == "HealthCare")
            {
                return GetSafeImage("hospital");
            }
            else if (service == "Garbage")
            {
                return GetSafeImage("recycling");
            }
            else if (service == "PoliceDepartment")
            {
                if (itemClass == "Prison Facility")
                {
                    return GetSafeImage("prison");
                }
                else
                {
                    return GetSafeImage("police");
                }
            }
            else if (service == "Education")
            {

                if (itemClass == "Elementary School")
                {
                    return GetSafeImage("school");
                }
                else if (itemClass == "High School")
                {
                    return GetSafeImage("school");

                }
                else
                {
                    return GetSafeImage("college");
                }
            }
            else if (itemClass == "Airplane Facility")
            {
                return GetSafeImage("airport");
            }
            else if (subService == "PublicTransportPost")
            {
                return GetSafeImage("post");
            }

            return (null, null);

        }
        private static (string? name, SKSvg? svg) GetJaplusSymbolImage(string name, string itemClass, string service, string subService)
        {
            var dir = MapSymbolPictureManager.Instance.GetSymbols("maki");
            if (dir is null)
            {
                return (null, null);
            }
            (string?, SKSvg?) GetSafeImage(string name)
            {
                if (dir.TryGetValue(name, out var symbol))
                {
                    return (name, symbol);
                }
                return (null, null);
            }


            if (service == "Garbage")
            {
                return GetSafeImage("recycling");
            }
            else if (service == "PoliceDepartment" && itemClass == "Prison Facility")
            {
                return GetSafeImage("prison");
            }
            else if (itemClass == "Airplane Facility")
            {
                return GetSafeImage("airport");
            }
            else
            {
                return GetJaSymbolImage(name, itemClass, service, subService);
            }
        }

        internal static (string? name, SKSvg? svg) GetDefaultSymbolImage(string? defaultType, string name, string itemClass, string service, string subService)
        {
            if (defaultType is null)
            {
                return (null, null);
            }
            if (defaultType == "maki")
            {
                return GetMakiSymbolImage(name, itemClass, service, subService);

            }
            else if (defaultType == "ja")
            {
                return GetJaSymbolImage(name, itemClass, service, subService);

            }
            else if (defaultType == "ja+")
            {
                return GetJaplusSymbolImage(name, itemClass, service, subService);
            }

            return (null, null);

        }
    }
}

using CS2MapView.Config;
using CS2MapView.Drawing.Labels.CS1;
using Svg.Skia;

namespace CS2MapView.Drawing.Buildings.CS1
{
    internal class CS1BuildingRenderingManager
    {

        private readonly string[] DefaultBuildingNameTargetServices = ["PublicTransport",
            "Monument",
            "Beautification",
            "Education",
            "Electricity",
            "FireDepartment",
            "PoliceDepartment",
            "HealthCare",
            "Garbage",
            "Road",
            "PlayerIndustry",
            "Water",
            "Disaster"];


        private readonly ICS2MapViewRoot AppRoot;
        private readonly CS1HiddenObjectInfo HiddenObjectInfo;
        private readonly CS1HiddenObjectInfo HiddenMapSymbolInfo;


        internal CS1BuildingRenderingManager(ICS2MapViewRoot appRoot)
        {
            AppRoot = appRoot;
            HiddenObjectInfo = CS1HiddenObjectInfo.Buildings;
            HiddenMapSymbolInfo = CS1HiddenObjectInfo.MapSymbols;

        }

        internal (RenderingSelectionReason? reason, string? symbolName, SKSvg? svg) GetMapSymbol(string steamId, string name, string itemClass, string service, string subService)
        {

            static string Strip(string? v) => v == null ? string.Empty : v.Trim();

            foreach (var t in AppRoot.Context.UserSettings.CS1BuildingPreferences ?? [])
            {

                if (Strip(steamId) == Strip(t.SteamId) && Strip(t.Name) == Strip(name))
                {
                    string? symbolName = t.MapSymbol;
                    if (symbolName is null)
                    {
                        break;
                    }

                    return (RenderingSelectionReason.Customized, symbolName, MapSymbolPictureManager.Instance.GetSvg(symbolName));
                }
            }
            //以降デフォルト
            if (HiddenMapSymbolInfo.IsHidden(name, itemClass))
            {
                return (RenderingSelectionReason.Default, null, null);

            }
            var result = CS1MapSymbolDefs.GetDefaultSymbolImage(AppRoot.Context.UserSettings.DefaultMapSymbolType, name, itemClass, service, subService);
            return (RenderingSelectionReason.Default, result.name, result.svg);
        }
        internal static string InternalServiceCategoryName(string pService)
        {
            if (pService == "Residential")
            {
                return "Residential";
            }
            else if (pService == "Commercial")
            {
                return "Commercial";
            }
            else if (pService == "Industrial" || pService == "PlayerIndustry")
            {
                return "Industrial";
            }
            else if (pService == "Office")
            {
                return "Office";
            }
            else if (pService == "PublicTransport")
            {
                return "PublicTransport";
            }
            else if (pService == "Beautification" || pService == "Monument")
            {
                return "Beautification";
            }
            else
            {
                return "Public";
            }
        }

        internal BuildingShapeRenderingParam GetShapeRenderingParam(string steamId, string name, string itemClass, string service)
        {

            string Strip(string? v) => v == null ? string.Empty : v.Trim();
            bool GetDefaultShapeVisibility() => !HiddenObjectInfo.IsHidden(name, itemClass);
            NameVisibility GetDefaultNameVisibility()
            {
                if (DefaultBuildingNameTargetServices.Contains(service))
                {
                    return NameVisibility.Visible;
                }
                else
                {
                    return NameVisibility.Invisible;
                }
            };

            BuildingShapeRenderingParam result = new()
            {
                ServiceForRender = InternalServiceCategoryName(service)
            };

            var pref = AppRoot.Context.UserSettings.CS1BuildingPreferences?.FirstOrDefault(p => Strip(steamId) == Strip(p.SteamId) && Strip(name) == Strip(p.Name));



            if (pref is not null)
            {
                bool? hide = pref.HideShape;
                if (!hide.HasValue)
                {
                    result.ShapeRenderingReason = RenderingSelectionReason.Default;
                    result.Visible = GetDefaultShapeVisibility();
                }
                else
                {
                    result.ShapeRenderingReason = RenderingSelectionReason.Customized;
                    result.Visible = !hide.Value;
                }

                var namevisible = pref.NameVisibility;
                if (namevisible == NameVisibility.Undefined)
                {
                    result.NameRenderingReason = RenderingSelectionReason.Default;
                    result.NameVisibility = GetDefaultNameVisibility();
                }
                else
                {
                    result.NameRenderingReason = RenderingSelectionReason.Customized;
                    result.NameVisibility = namevisible;
                }

                return result;
            }
            else
            {
                //以降デフォルト
                result.ShapeRenderingReason = RenderingSelectionReason.Default;
                result.Visible = GetDefaultShapeVisibility();
                result.NameRenderingReason = RenderingSelectionReason.Default;
                result.NameVisibility = GetDefaultNameVisibility();
                return result;
            }
        }

    }
}

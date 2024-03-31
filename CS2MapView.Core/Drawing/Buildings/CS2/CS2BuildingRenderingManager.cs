
using CS2MapView.Drawing.Labels.CS2;
using CS2MapView.Serialization;
using Svg.Skia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CSLMapView.XML.CSConstants;

namespace CS2MapView.Drawing.Buildings.CS2
{
    internal class CS2BuildingRenderingManager
    {
        private readonly ICS2MapViewRoot AppRoot;

        internal CS2BuildingRenderingManager(ICS2MapViewRoot appRoot)
        {
            AppRoot = appRoot;
        }

        internal (RenderingSelectionReason? reason, string? symbolName, SKSvg? svg) GetMapSymbol(CS2Building building, CS2BuildingPrefab? prefab)
        {
            var result = CS2MapSymbolDefs.GetDefaultSymbolImage(AppRoot.Context.UserSettings.DefaultMapSymbolType, building, prefab);
            return (RenderingSelectionReason.Default, result.name, result.svg);
        }
    }
}

using CS2MapView.Config;

namespace CS2MapView.Drawing.Buildings
{
    internal enum RenderingSelectionReason
    {
        Default = 0, Customized
    }

    internal class BuildingShapeRenderingParam
    {
        internal bool Visible { get; set; }

        internal required string ServiceForRender { get; set; }
        internal RenderingSelectionReason ShapeRenderingReason { get; set; }
        internal NameVisibility NameVisibility { get; set; }
        internal RenderingSelectionReason NameRenderingReason { get; set; }
    }
}

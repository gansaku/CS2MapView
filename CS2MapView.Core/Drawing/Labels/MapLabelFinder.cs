using SkiaSharp;

namespace CS2MapView.Drawing.Labels
{
    internal class MapLabelFinder : OrderedRegionalLookup<AbstractMapLabel>
    {
        internal MapLabelFinder(ReadonlyRect searchRect) : base(searchRect, ObjToRect) { }

        private static SKRect ObjToRect(Indexed<AbstractMapLabel> c) => c.Obj.Bounds;


    }
}

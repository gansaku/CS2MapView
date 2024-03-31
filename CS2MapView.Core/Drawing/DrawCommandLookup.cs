using SkiaSharp;

namespace CS2MapView.Drawing
{

    internal class DrawCommandLookup : OrderedRegionalLookup<IDrawCommand>
    {

        internal DrawCommandLookup(ReadonlyRect worldRect, int division = DivisionDefault)
            : base(worldRect, ObjToRect, division)
        {
        }

        private static SKRect ObjToRect(Indexed<IDrawCommand> c) => c.Obj.BoundingRect;


        /// <inheritdoc/>
        internal override void ClearContent(bool dispose)
        {
            if (dispose)
            {
                foreach (var item in Orig)
                {
                    item.Obj?.Dispose();
                }
            }
            Orig.Clear();
            for (int i = 0; i < Blocks.Length; i++)
            {

                Blocks[i] = [];
            }
        }
    }
}


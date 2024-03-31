using SkiaSharp;
using System.Collections;
using System.Numerics;

namespace CS2MapView.Drawing
{
    internal class RegionalLookupNode<T>
    {
        internal required T Data;
        internal List<int> Indices = [];
    }
    /// <summary>
    /// 指定の座標つきオブジェクトを、領域ごとに分割して保持します。
    /// </summary>
    /// <typeparam name="TObj"></typeparam>
    internal class RegionalLookup<TObj> : IEnumerable<RegionalLookupNode<TObj>>
    {
        protected const int DivisionDefault = 10;
        protected readonly int Division;
        protected readonly List<TObj> Orig;
        internal readonly ReadonlyRect WorldRect;
        protected readonly List<RegionalLookupNode<TObj>>[] Blocks;
        protected readonly SKRect[] AreaRects;

        internal delegate SKRect ObjToRectFunc(TObj obj);
        protected readonly ObjToRectFunc ObjToRectAction;


        protected static SKRect InfratedBounds(SKRect rect)
        {
            var r = rect;
            r.Inflate(0.01f, 0.01f);
            return r;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="orig">元のリスト。中身は処理の進行によって削除される場合があります。</param>
        /// <param name="worldRect">許容される座標範囲</param>
        /// <param name="objToRectAction">内容の座標をSKRectとして取得するメソッド</param>
        /// <param name="division">縦横それぞれの分割数。</param>
        /// <exception cref="InvalidDataException"></exception>
        internal RegionalLookup(List<TObj> orig, ReadonlyRect worldRect, ObjToRectFunc objToRectAction, int division = DivisionDefault)
        {
            Orig = orig;
            WorldRect = worldRect;
            Division = division;
            Blocks = new List<RegionalLookupNode<TObj>>[Division * Division];
            AreaRects = new SKRect[Division * Division];
            ObjToRectAction = objToRectAction;

            for (int by = 0; by < Division; by++)
            {
                float areatop = WorldRect.Top + (WorldRect.Height / Division) * by;
                float areabottom = WorldRect.Top + (WorldRect.Height / Division) * (by + 1);
                for (int bx = 0; bx < Division; bx++)
                {
                    float arealeft = WorldRect.Left + (WorldRect.Width / Division) * bx;
                    float arearight = WorldRect.Left + (WorldRect.Width / Division) * (bx + 1);
                    var r = new SKRect(arealeft, areatop, arearight, areabottom);
                    AreaRects[by * Division + bx] = r;
                    Blocks[by * Division + bx] = [];
                }
            }
            foreach (var cs in orig)
            {
                var node = new RegionalLookupNode<TObj> { Data = cs };
                var rect = ObjToRectAction(cs);

                for (int i = 0; i < AreaRects.Length; i++)
                {
                    if (rect.IntersectsWith(AreaRects[i]))
                    {
                        node.Indices.Add(i);
                        Blocks[i].Add(node);
                    }
                }
                if (node.Indices.Count == 0)
                {
                    throw new InvalidDataException($"座標範囲外 {rect}");
                }
            }
            ObjToRectAction = objToRectAction;
        }

        protected int GetIndex(Vector3 v)
        {
            float divwidth = WorldRect.Width / Division;
            int x = (int)((v.X - WorldRect.Left) / divwidth);
            int y = (int)((v.Z - WorldRect.Top) / divwidth);
            x = x >= Division ? Division - 1 : x;
            y = y >= Division ? Division - 1 : y;

            return y * Division + x;
        }



        internal void Remove(RegionalLookupNode<TObj> node)
        {
            var indices = node.Indices;
            foreach (var index in indices)
            {
                Blocks[index].Remove(node);
            }
            Orig.Remove(node.Data);
        }

        public IEnumerator<RegionalLookupNode<TObj>> GetEnumerator()
        {
            for (int i = 0; i < Blocks.Length; i++)
            {
                var innerList = Blocks[i];
                foreach (var obj in innerList)
                {
                    if (!obj.Indices.Where(t => t < i).Any())
                    {
                        yield return obj;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        /// <summary>
        /// 再利用のために、内部に保持したオブジェクトを全削除しますが、ブロック情報はそのまま保持します。
        /// </summary>
        internal virtual void ClearContent(bool dispose)
        {
            if (dispose && typeof(TObj).IsAssignableTo(typeof(IDisposable)))
            {
                foreach (var obj in Orig)
                {
                    (obj as IDisposable)?.Dispose();
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

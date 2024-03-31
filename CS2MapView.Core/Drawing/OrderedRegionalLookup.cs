using SkiaSharp;
using System.Diagnostics;

namespace CS2MapView.Drawing
{
    internal class Indexed<T>
    {
        internal int Index;
        internal T Obj;

        internal Indexed(int index, T obj)
        {
            Index = index;
            Obj = obj;
        }

        //等価は参照で判定する。
    }

    internal abstract class OrderedRegionalLookup<T> : RegionalLookup<Indexed<T>>
    {
        private int NextIndex = 0;

        internal OrderedRegionalLookup(ReadonlyRect worldRect, ObjToRectFunc objToRectFunc, int division = DivisionDefault)
            : base([], worldRect, objToRectFunc, division)
        {
        }

        internal bool Add(T obj)
        {
            var indices = new List<int>();
            var bound = ObjToRectAction(new Indexed<T>(0, obj));
            for (int i = 0; i < AreaRects.Length; i++)
            {
                if (bound.IntersectsWith(AreaRects[i]))
                {
                    indices.Add(i);
                }
            }
            if (indices.Count == 0)
            {
                Debug.Print($"領域不適合。region={(SKRect)WorldRect} obj={obj}@{bound}");
                return false;
            }

            Indexed<T> objwi = new(NextIndex++, obj);
            RegionalLookupNode<Indexed<T>> node = new() { Data = objwi };
            node.Indices.AddRange(indices);
            foreach (var i in indices)
            {
                Blocks[i].Add(node);
            }
            Orig.Add(objwi);
            return true;
        }
        internal int AddRange(IEnumerable<T>? values)
        {
            if (values is null)
            {
                return 0;
            }
            int count = 0;
            foreach (var v in values)
            {
                count += Add(v) ? 1 : 0;
            }
            return count;
        }
        private IEnumerable<Indexed<T>> GetObjectsInRegion(SKRect rect)
        {
            for (int i = 0; i < AreaRects.Length; i++)
            {
                SKRect ar = AreaRects[i];
                if (rect.IntersectsWith(ar))
                {
                    foreach (var obj in Blocks[i])
                    {
                        yield return obj.Data;
                    }
                }
            }
        }
        /// <summary>
        /// 指定した矩形内に含まれるパスを描画するコマンドを列挙します。
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        internal IEnumerable<T> GetOrderedObjects(SKRect rect)
        {
            return GetObjectsInRegion(rect).Distinct().OrderBy(s => s.Index).Select(s => s.Obj);
        }
        /// <summary>
        /// すべてのコマンドを列挙します。
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<T> GetAllOrderedObjects()
        {
            return Orig.Select(t => t.Obj);
        }


        internal override void ClearContent(bool dispose)
        {
            if (dispose && typeof(T).IsAssignableTo(typeof(IDisposable)))
            {
                foreach (var item in Orig)
                {
                    (item.Obj as IDisposable)?.Dispose();
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

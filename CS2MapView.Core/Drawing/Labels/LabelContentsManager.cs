using CS2MapView.Data;
using Gfw.Common;
using SkiaSharp;
using System.Diagnostics;

namespace CS2MapView.Drawing.Labels
{
    /// <summary>
    /// ラベルオブジェクトを保持します。
    /// </summary>
    public class LabelContentsManager
    {
        internal delegate void RebuildRequiredEventHandler(ViewContext vc);
        internal readonly object ContentsLock = new();
        internal MapLabelFinder Contents { get; private set; }

        internal LabelContentsManager(ReadonlyRect searchRect)
        {
            Contents = new(searchRect);
        }

        internal event RebuildRequiredEventHandler? RebuildRequired;
        /// <summary>
        /// 再構築します。
        /// </summary>
        /// <returns></returns>
        public Task RebuildAsync(ViewContext vc)
        {
            lock (ContentsLock)
            {
                Clear(vc.TextWorldRect);
                return Task.Run(() => RebuildRequired?.Invoke(vc));
            }
        }
        /// <summary>
        /// 位置調整不可のオブジェクトが重なっている場合、片方を非表示にする
        /// </summary>
        public void PreYield()
        {

            foreach (var item in Contents.GetAllOrderedObjects().Where(t => t.MayYield))
            {
                if (item.Yielded)
                {
                    continue;
                }
                foreach (var other in Contents.GetOrderedObjects(item.Bounds).Where(t => t.Freezed).OrderBy(t => t.YieldPriority))
                {
                    if (item == other)
                    {
                        continue;
                    }
                    if (item.Yielded)
                    {
                        break;
                    }
                    if (other.Yielded)
                    {
                        continue;
                    }
                    if (!item.Bounds.IntersectsWith(other.Bounds))
                    {
                        continue;
                    }
                    if (item.YieldPriority < other.YieldPriority)
                    {
                        item.Yielded = true;
                    }
                    else
                    {
                        if (other.MayYield)
                        {
                            other.Yielded = true;
                        }
                    }
                }
            }
        }

        private void Clear(ReadonlyRect searchRect)
        {
            Contents = new(searchRect);
        }

        /// <summary>
        /// 表示位置の調整
        /// </summary>
        /// <returns></returns>
        public void ArrangePositions()
        {

            int moved = 0;
            int loopCnt = 0;
            Contents.GetAllOrderedObjects().ForEach(t => t.DisplayPosition = t.OriginalPosition);

            do
            {
                moved = 0;
                Contents.GetAllOrderedObjects().Where(t => !t.Freezed && !t.Yielded).ForEach(t => t.Speed = SKPoint.Empty);

                if (loopCnt > 100)
                {
                    break;
                }

                foreach (var t in Contents.GetAllOrderedObjects().Where(t => !t.Freezed && !t.Yielded))
                {
                    foreach (var s in Contents.GetOrderedObjects(t.Bounds).Where(t => !t.Yielded))
                    {

                        if (t == s)
                        {
                            continue; ;
                        }
                        if (AbstractMapLabel.Intersects(t, s))
                        {

                            if (t.DisplayPosition == s.DisplayPosition)
                            {
                                float power2 = 6f;
                                if (!s.Freezed)
                                {
                                    s.Speed.X += power2 / 2.1f;
                                    s.Speed.Y += power2 / 2;
                                }
                                power2 /= 2;
                                t.Speed.X -= power2;
                                t.Speed.Y -= power2 / 1.1f;

                                continue;
                            }

                            var dx = s.DisplayPosition.X - t.DisplayPosition.X;
                            var dy = s.DisplayPosition.Y - t.DisplayPosition.Y;
                            float power = 0.3f;
                            if (!s.Freezed)
                            {
                                s.Speed.X += power / 2 * dx;
                                s.Speed.Y += power / 2 * dy;
                                power /= 2;
                            }
                            t.Speed.X -= power / 2 * dx;
                            t.Speed.Y -= power / 2 * dy;

                        }

                    }

                }

                Contents.GetAllOrderedObjects().Where(t => !t.Freezed).ForEach(t =>
                {
                    if (t.Speed != SKPoint.Empty)
                    {
                        t.DisplayPosition += t.Speed;
                        moved++;
                    }
                });

                Debug.Print($"LabelContentsManager.ArrangePositions loop[{loopCnt}] moved[{moved}]");
                loopCnt++;
            } while (moved > 0);


        }
    }
}

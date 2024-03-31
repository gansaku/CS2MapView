using CS2MapView.Data;
using CS2MapView.Util;
using Gfw.Common;
using System.Diagnostics;

namespace CS2MapView.Drawing.Labels
{
    internal abstract class AbstractLabelContentBuilder
    {
        protected ICS2MapViewRoot AppRoot { get; init; }

        protected LabelPaintDefs FontPaintCache { get; private set; }

        protected LabelContentsManager LabelContentManager;



        internal AbstractLabelContentBuilder(ICS2MapViewRoot appRoot, LabelContentsManager manager)
        {
            AppRoot = appRoot;


            LabelContentManager = manager;
            LabelContentManager.RebuildRequired += Build;


        }

        public Task BuildAsync(ViewContext vc)
        {
            return Task.Run(() => Build(vc));
        }


        private void Build(ViewContext vc)
        {
            Debug.Print($"{GetType().Name}.Build vc={vc.TextWorldRect}");

            FontPaintCache = new(AppRoot.Context.StringTheme, vc.ScaleFactor, vc.WorldScaleFactor);
            //対象物抽出
            lock (LabelContentManager.ContentsLock)
            {
                AddContents(vc);
                LabelContentManager.PreYield();
                DeleteSameStreetNamesNearby(vc);
                LabelContentManager.ArrangePositions();
            }
        }

        protected abstract void AddContents(ViewContext vc);
        /// <summary>
        /// 近くに同じ道路名がある場合、削除
        /// </summary>
        private void DeleteSameStreetNamesNearby(ViewContext vc)
        {
            foreach (var item in LabelContentManager.Contents.SelectNotNull(t => t.Data.Obj as StreetNameLabel).Where(t => !t!.Yielded))
            {
                if (item!.Yielded)
                {
                    continue;
                }
                foreach (var other in LabelContentManager.Contents.SelectNotNull(t => t.Data.Obj as StreetNameLabel)
                    .Where(t => t != item && t!.Text == item!.Text && !t!.Yielded))
                {
                    if (MathEx.DistanceSqr(item!.OriginalPosition, other!.OriginalPosition) < Math.Pow(200 * vc.ViewScaleFromWorld, 2))
                    {
                        item.Yielded = true;
                        break;
                    }
                }

            }

        }
    }
}

using CS2MapView.Data;
using CS2MapView.Drawing.Layer;
using SkiaSharp;

namespace CS2MapView.Drawing.Transport.CS1
{
    internal class CS1TransportLineLayerBuilder : IRebuildOnResizeLayerBuilder,IRebuildOnRotateLayerBuilder
    {
        private ICS2MapViewRoot AppRoot { get; init; }
        private RotatedLayer ResultLayer { get; init; }

        public LoadProgressInfo.Process ProcessType => LoadProgressInfo.Process.BuildTransportLines;

        public CS1TransportLineLayerBuilder(ICS2MapViewRoot appRoot)
        {
            AppRoot = appRoot;
            ResultLayer = new RotatedLayer(AppRoot, ILayer.LayerNameTransportLines, CS1MapType.CS1WorldRect);
            ResultLayer.Builder = this;

        }

        public Task<ILayer> BuildAsync(LoadProgressInfo? loadProgressInfo, ViewContext vc)
        {
            return Task.Run<ILayer>(() =>
            {
                Build(loadProgressInfo, vc);
                return ResultLayer;
            });
        }

        public void Build(LoadProgressInfo? loadProgressInfo, ViewContext vc)
        {
            loadProgressInfo?.Progress(this, LoadProgressInfo.Process.BuildTransportLines, 1f, null);
#if false
            /*
            foreach(var ps in ContentsManager.ParentSegmentMap)
            {
                var path = new SKPath();
                path.MoveTo(ps.Value.Points[0]);
                for(int i = 1; i < ps.Value.Points.Count; i++)
                {
                    path.LineTo(ps.Value.Points[i]);
                }

                var cmd = new PathDrawCommand {
                    StrokePaintFunc = (scale, worldScale) => new(20f/scale, SKColors.Green, Theme.StrokeType.Round, [2f,1f]) ,
                Path= path};
                ResultLayer.DrawCommands.Add(cmd);
            }*/
            foreach (var ti in ContentsManager.TransportList)
            {
                foreach (var st in ti.Stops ?? [])
                {
                    if ((st.DrawRoute?.Count ?? 0) > 0)
                    {
                        foreach (var dr in st.DrawRoute!)
                        {
                            if ((dr.Points?.Count ?? 0) < 2)
                            {
                                continue;
                            }
                            var path = new SKPath();
                            path.MoveTo(dr.Points![0]);
                            for (int i = 1; i < dr.Points.Count; i++)
                            {
                                path.LineTo(dr.Points[i]);
                            }
                            var cmd = new PathDrawCommand
                            {
                                StrokePaintFunc = (scale, worldScale) => new(20f / scale, SKColors.Green, Theme.StrokeType.Round),
                                Path = path
                            };
                            ResultLayer.DrawCommands.Add(cmd);
                        }

                    }

                }
            }
#endif
        }

        public Task ResizeAsync(LoadProgressInfo? loadProgressInfo, ViewContext vc)
        {
            return Task.Run(() =>
            {

                loadProgressInfo?.Progress(this, LoadProgressInfo.Process.BuildTransportLines, 1f, null);
            });
        }

        public Task RotateAsync(LoadProgressInfo? loadProgressInfo, ViewContext vc) => ResizeAsync(loadProgressInfo, vc);
    }
}

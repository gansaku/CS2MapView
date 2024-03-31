using CS2MapView.Data;
using CS2MapView.Drawing.Labels;
using CS2MapView.Drawing.Layer;
using CS2MapView.Serialization;
using CS2MapView.Util;
using CS2MapView.Util.CS2;
using ExCSS;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2MapView.Drawing.Transport.CS2
{
    public class CS2TransportLineLayerBuilder : IRebuildOnResizeLayerBuilder, IRebuildOnRotateLayerBuilder
    {
        public LoadProgressInfo.Process ProcessType => LoadProgressInfo.Process.BuildTransportLines;

        private ICS2MapViewRoot AppRoot { get; init; }
        private CS2MapDataSet ImportData { get; init; }
        private RotatedLayer ResultLayer { get; init; }
        private LabelContentsManager LabelContentsManager { get; init; }
        private SKPicture StopPicture { get; set; }

        public CS2TransportLineLayerBuilder(ICS2MapViewRoot appRoot, CS2MapDataSet importData)
        {
            AppRoot = appRoot;
            ImportData = importData;
            ResultLayer = new RotatedLayer(AppRoot, ILayer.LayerNameTransportLines, CS2MapType.CS2WorldRect, this);
            LabelContentsManager = new(AppRoot.Context.ViewContext.TextWorldRect);

            using var recorder = new SKPictureRecorder();
            using var canvas = recorder.BeginRecording(new(0, 0, 6, 6));
            var paint = SKPaintCache.Instance.GetStroke(new SKPaintCache.StrokeKey(1f, SKColors.Black, Theme.StrokeType.Round));
            var fillStyle = AppRoot.Context.Theme.Colors?.TransportStopFill;
            if (fillStyle is not null)
            {
                canvas.DrawCircle(new(3, 3), 2.5f, SKPaintCache.Instance.GetFill(fillStyle.Value));
            }
            var strokeStyle = AppRoot.Context.Theme.Colors?.TransportStopStroke;
            if (strokeStyle is not null)
            {
                canvas.DrawCircle(new(3, 3), 2.5f, SKPaintCache.Instance.GetStroke(new(1f, strokeStyle.Value, Theme.StrokeType.Flat)));
            }

            StopPicture = recorder.EndRecording();
        }

        ~CS2TransportLineLayerBuilder()
        {
            StopPicture?.Dispose();
        }

        public Task ResizeAsync(LoadProgressInfo? loadProgressInfo, ViewContext vc)
        {
            return Task.Run<ILayer>(() =>
            {
                lock (ResultLayer.DrawObjectsLock)
                {
                    ResultLayer.ClearAndResizeCommandsList(vc.TextWorldRect, true);
                    Build(vc);

                    loadProgressInfo?.Progress(this, ProcessType, 1f, null);
                    return ResultLayer;
                }
            });
        }

        public Task RotateAsync(LoadProgressInfo? loadProgressInfo, ViewContext vc) => ResizeAsync(loadProgressInfo, vc);

        public Task<ILayer> BuildAsync(LoadProgressInfo? loadProgressInfo)
        {
            return Task.Run<ILayer>(() =>
            {
                lock (ResultLayer.DrawObjectsLock)
                {
                    PreBuild();
                    Build(AppRoot.Context.ViewContext);
                    loadProgressInfo?.Progress(this, ProcessType, 1f, null);
                    return ResultLayer;
                }
            });

        }
        private void PreBuild()
        {

        }
        private void Build(ViewContext vc)
        {
            var ts = AppRoot.Context.UserSettings.TransportRouteMapConfig;
            bool isDrawingTargetLine(CS2TransportLine t)
            {
              
                return t.TransportType switch
                {
                    CS2TransportType.Bus => ts.RenderBusLine,
                    CS2TransportType.Train => ts.RenderTrainLine,
                    CS2TransportType.Tram => ts.RenderTramLine,
                    CS2TransportType.Subway => ts.RenderMetroLine,
                    CS2TransportType.Ship => ts.RenderShipLine,
                    CS2TransportType.Airplane => ts.RenderAirplaneLine,
                    _=>false
                } ;
            }
            bool isDrawingTargetStop(CS2TransportStop t)
            {
               
                return t.TransportType switch
                {
                    CS2TransportType.Bus => ts.RenderBusStop,
                    CS2TransportType.Train => ts.RenderTrainStop,
                    CS2TransportType.Tram => ts.RenderTramStop,
                    CS2TransportType.Subway => ts.RenderMetroStop,
                    CS2TransportType.Ship => ts.RenderShipStop,
                    CS2TransportType.Airplane => ts.RenderAirplaneStop,
                    _ => false
                };
            }
            LabelContentsManager.RebuildAsync(vc).Wait();
            foreach (var t in ImportData.TransportInfo?.TransportLines ?? [])
            {
                if (t.IsCargo && ts.HideCargoLines)
                {
                    continue;
                }
                if (!isDrawingTargetLine(t))
                {
                    continue;
                }
                foreach (var seg in t.Segments ?? [])
                {
                    var path = new SKPath();
                    var first = true;
                    foreach (var curve in seg.Curves ?? [])
                    {
                        var points = vc.TextWorldTransform.MapPoints(curve.ToPointArray());
                        if (first)
                        {
                            path.MoveAndCubicTo(points);
                        }
                        else
                        {
                            path.CubicTo(points);
                        }
                        SKPaint p = new();

                    }
                    ResultLayer.DrawCommands.Add(new PathDrawCommand
                    {
                        Path = path,
                        StrokePaintFunc = (s, ws) => new SKPaintCache.StrokeKey(3f, t.Color.ToSKColorWithAlpha(180), Theme.StrokeType.Flat)
                    }); ;
                }
            }
            foreach (var stop in ImportData?.TransportInfo?.TransportStops?.Where(t => t?.Lines?.Count > 0) ?? [])
            {
                if (stop.IsCargo && ts.HideCargoLines)
                {
                    continue;
                }
                if (!isDrawingTargetStop(stop))
                {
                    continue;
                }
                var stopMark = new TransportStopSymbol
                {
                    Picture = StopPicture,
                    ImageScale = 1f / (vc.ScaleFactor * vc.WorldScaleFactor),
                    Size = (StopPicture.CullRect.Width * vc.ViewScaleFromWorld, StopPicture.CullRect.Height * vc.ViewScaleFromWorld),
                    OriginalPosition = vc.TextWorldTransform.MapPoint(stop.Position.ToMapSpace()),
                    DisplayPosition = vc.TextWorldTransform.MapPoint(stop.Position.ToMapSpace())
                };
                LabelContentsManager.Contents.Add(stopMark);
            }
            LabelContentsManager.PreYield();
            LabelContentsManager.ArrangePositions();
            foreach (var t in LabelContentsManager.Contents.GetAllOrderedObjects())
            {
                var st = t as TransportStopSymbol;
                if (st is null || st.Yielded)
                {
                    continue;
                }

                ResultLayer.DrawCommands.Add(new SKPictureDrawCommand
                {
                    Picture = st.Picture,
                    BoundingRect = st.Bounds,
                    DisposeRequired = false,
                    Location = st.DisplayPosition - new SKPoint(st.Bounds.Width / 2, st.Bounds.Height / 2),
                    Scale = st.ImageScale
                });
            }

            /*
           if(AppRoot.Context.Theme.Colors?.TransportStopFill is null && AppRoot.Context.Theme.Colors?.TransportStopStroke is null)
           {
               return;
           }

           var sk = new SKPaintCache.StrokeKey(1f, AppRoot.Context.Theme.Colors.TransportStopStroke, Theme.StrokeType.Flat);
           SKPaintCache.StrokeKey? keyFunc(float a,float b)=> sk;
           SKColor? color = AppRoot.Context.Theme.Colors?.TransportStopFill is null ? null : AppRoot.Context.Theme.Colors.TransportStopFill;
           foreach (var t in ImportData.TransportInfo?.TransportStops ?? [])
           {
               var pos = vc.TextWorldTransform.MapPoint(t.Position.ToMapSpace());
               var path = new SKPath();
               path.AddCircle(pos.X, pos.Y, 3);
               ResultLayer.DrawCommands.Add(new PathDrawCommand {Path = path, StrokePaintFunc= keyFunc, FillColor = color });
           }
           */
        }
    }
}

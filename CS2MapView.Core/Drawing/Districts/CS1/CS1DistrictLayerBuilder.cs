using CS2MapView.Data;
using CS2MapView.Drawing.Layer;

namespace CS2MapView.Drawing.Districts.CS1
{
    public class CS1DistrictLayerBuilder(ICS2MapViewRoot AppRoot)
    {
        public Task<ILayer> BuildAsync(LoadProgressInfo? loadProgressInfo)
        {
            loadProgressInfo?.Progress(this, LoadProgressInfo.Process.BuildDistricts, 1f, null);

            return Task.Run<ILayer>(() => new BasicLayer(AppRoot, ILayer.LayerNameDistricts, CS1MapType.CS1WorldRect));
        }
    }
}

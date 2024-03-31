using System;
using System.Collections.Generic;
using System.Text;

namespace CS2MapView.Serialization
{
    /// <summary>
    /// このクラスは読み込み後にのみ使用されます。
    /// </summary>
    public class CS2MapDataSet
    {
        public bool LoadError { get; set; } = false;
        public string? LoadErrorMessage { get; set; }
        public CS2MainData? MainData { get; set; }
        
        public List<CS2Building>? Buildings { get; set; }
        public List<CS2BuildingPrefab>? BuildingPrefabs { get; set; }
        public List<CS2DistrictInfo>? Districts { get; set; }
        public CS2RoadsData? RoadInfo { get; set; }
        public CS2RailsData? RailInfo { get; set; }
        public CS2TransportLineData? TransportInfo { get; set; }
        public float[]? TerrainArray { get; set; }
        public float[]? WaterArray { get; set; }
    }
}

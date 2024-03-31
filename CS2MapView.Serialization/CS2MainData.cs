using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;

namespace CS2MapView.Serialization
{
    public class CS2MainData
    {
        public string? FileVersion { get; set; }

        public string? CityName { get; set; }

        public CS2TerrainWaterDataInfo? Terrain { get; set; }

        public CS2TerrainWaterDataInfo? Water { get; set; }

        public float SeaLevel { get; set; }

        public List<string>? TestList { get; set; }

    }
}

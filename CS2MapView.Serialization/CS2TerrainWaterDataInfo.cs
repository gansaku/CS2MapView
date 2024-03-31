using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Xml.Serialization;

namespace CS2MapView.Serialization
{
    public class CS2TerrainWaterDataInfo
    {
        public Vector3 Offset { get; set; }
       
        public Vector3 Scale { get; set; }
        
        public Vector3 Resolution { get; set; }

    }
}

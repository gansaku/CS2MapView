using System;
using System.Numerics;
using System.Text;
using System.Xml.Serialization;

namespace CS2MapView.Serialization
{
    public class CS2DistrictInfo
    {
        [XmlAttribute("entity")]
        public int Entity { get; set; }

        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("customName")]
        public bool IsCustomName { get; set; }

        public Vector3 CenterPosition { get; set; }

        public CS2Geometry? Geometry { get; set; }
    }
}

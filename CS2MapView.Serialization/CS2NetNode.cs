using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CS2MapView.Serialization
{
    public abstract class CS2NetNode
    {
        [XmlAttribute("entity")]
        public int Entity { get; set; }

        public CS2Vector3 Position { get; set; }
        public CS2Quaternion Rotation { get; set; }
        public CS2Float2 Elevation { get; set; }
    }
    public abstract class CS2NetSegment
    {
        [XmlAttribute("entity")]
        public int Entity { get; set; }
        [XmlAttribute("name")]
        public string? CustomName { get; set; }
       
        [XmlAttribute("length")]
        public float Length { get; set; }
        public CS2Float2 Elevation { get; set; }
        public CS2MapSpaceBezier4 Curve { get; set; }
        [XmlAttribute("startNode")]
        public int StartNode { get; set; }
        [XmlAttribute("endNode")]
        public int EndNode { get; set; }
       
    }
}

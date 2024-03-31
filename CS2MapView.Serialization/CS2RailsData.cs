using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CS2MapView.Serialization
{
    public class CS2RailsData
    {
        [XmlArrayItem("Segment", typeof(CS2RailSegment))]
        [XmlArray("RailSegments")]
        public List<CS2RailSegment>? RailSegments { get; set; }
        [XmlArrayItem("Node", typeof(CS2RailNode))]
        [XmlArray("RailNodes")]
        public List<CS2RailNode>? RailNodes { get; set; }
    }

    public class CS2RailNode : CS2NetNode
    {
        [XmlIgnore]
        public CS2RailType RailType { get => (CS2RailType)RailTypeSerialized; set => RailTypeSerialized = (uint)value; }
        [XmlAttribute("type")]
        public uint RailTypeSerialized { get; set; }
    }

    public class CS2RailSegment : CS2NetSegment
    {
        [XmlIgnore]
        public CS2RailType RailType { get => (CS2RailType)RailTypeSerialized; set => RailTypeSerialized = (uint)value; }
        [XmlAttribute("type")]
        public uint RailTypeSerialized { get; set; }
        [XmlAttribute("station")]
        public bool IsStation { get; set; }
    }

    [Flags]
    public enum CS2RailType : uint
    {
        None = 0,
        Train = 1,
        Tram = 2,
        Subway = 4
    }
}

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Xml.Serialization;

namespace CS2MapView.Serialization
{
    public class CS2TransportLineData
    {
        [XmlArrayItem("Line",typeof(CS2TransportLine))]
        [XmlArray("Lines")]
        public List<CS2TransportLine>? TransportLines { get; set; }
        [XmlArrayItem("Stop", typeof(CS2TransportStop))]
        [XmlArray("Stops")]
        public List<CS2TransportStop>? TransportStops { get; set; }
      
    }

    public struct CS2Color
    {
        [XmlAttribute("a")]
        public byte A { get; set; }
        [XmlAttribute("r")]
        public byte R { get; set; }
        [XmlAttribute("g")]
        public byte G { get; set; }
        [XmlAttribute("b")]
        public byte B { get; set; }
    }

    public class CS2TransportLine
    {
        [XmlAttribute("entity")]
        public int Entity { get; set; }
        [XmlAttribute("no")]
        public int Number { get; set; }
        [XmlAttribute("name")]
        public string? Name { get; set; }
        [XmlAttribute("custom")]
        public bool IsCustomName { get; set; }
        [XmlAttribute("cargo")]
        public bool IsCargo { get; set; }

        [XmlIgnore]
        public CS2TransportType TransportType { get; set; } = CS2TransportType.None;

        [XmlAttribute("type")]
        public int TransportTypeSerialized { get => (int)TransportType;set => TransportType = (CS2TransportType)value; }

        [XmlArrayItem("Segment",typeof(CS2RouteSegment))]
        [XmlArray("Segments")]
        public List<CS2RouteSegment>? Segments { get; set; }

        public CS2Color Color { get; set; }
        
    }

    public class CS2RouteSegment 
    {
        [XmlArrayItem("Curve",typeof(CS2MapSpaceBezier4))]
        [XmlArray("Curves")]
        public List<CS2MapSpaceBezier4>? Curves { get; set; }
    }

    public class CS2TransportStop
    {
        [XmlAttribute("entity")]
        public int Entity { get; set; }
        public Vector3 Position { get; set; }

        [XmlAttribute("name")]
        public string? Name { get; set; }
        [XmlAttribute("custom")]
        public bool IsCustomName { get; set; }
        [XmlAttribute("cargo")]
        public bool IsCargo { get; set; }


        [XmlArrayItem("Line",typeof(int))]
        [XmlArray("Lines")]
        public List<int>? Lines { get; set; }

        [XmlIgnore]
        public CS2TransportType TransportType { get; set; } = CS2TransportType.None;

        [XmlAttribute("type")]
        public int TransportTypeSerialized { get => (int)TransportType; set => TransportType = (CS2TransportType)value; }
    }

    public enum CS2TransportType
    {
        None = -1,
        Bus,
        Train,
        Taxi,
        Tram,
        Ship,
        Post,
        Helicopter,
        Airplane,
        Subway,
        Rocket,
        Count
    }
}

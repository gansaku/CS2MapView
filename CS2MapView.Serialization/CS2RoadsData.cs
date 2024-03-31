using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CS2MapView.Serialization
{
    public class CS2RoadsData
    {
        [XmlArrayItem("Prefab", typeof(CS2RoadPrefab))]
        [XmlArray("RoadPrefabs")]
        public List<CS2RoadPrefab>? RoadPrefabs { get; set; }
        [XmlArrayItem("Segment", typeof(CS2RoadSegment))]
        [XmlArray("RoadSegments")]
        public List<CS2RoadSegment>? RoadSegments { get; set; }
        [XmlArrayItem("Node", typeof(CS2RoadNode))]
        [XmlArray("RoadNodes")]
        public List<CS2RoadNode>? RoadNodes { get; set; }

    }

    public class CS2RoadNode : CS2NetNode
    {
      /*  [XmlAttribute("prefab")]
        public int Prefab { get; set; }*/
    }

    public class CS2RoadSegment : CS2NetSegment
    {
        [XmlAttribute("prefab")]
        public int Prefab { get; set; }

        [XmlAttribute("lanes")]
        public uint LaneTypeSerialized { get; set; }
        [XmlIgnore]
        public CS2RoadLaneType LaneType
        {
            get => (CS2RoadLaneType)LaneTypeSerialized;
            set => LaneTypeSerialized = (uint)value;
        }
    }

    [Flags]
    public enum CS2RoadLaneType : uint
    {
        None = 0x0,
        Car = 0x1,
        Pedestrian=0x2,
        Tram = 0x4
    }

   

    public class CS2RoadPrefab
    {
        [XmlAttribute("entity")]
        public int Entity { get; set; }

        [XmlAttribute("name")]
        public string? Name { get; set; }
        [XmlAttribute("highway")]
        public bool IsHighway { get; set; }
        public float DefaultWidth { get; set; }
        public float ElevatedWidth { get; set; }
    }
}

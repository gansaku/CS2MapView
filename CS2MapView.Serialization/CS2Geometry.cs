using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Xml.Serialization;

namespace CS2MapView.Serialization
{
    public class CS2Geometry
    {
        [XmlArrayItem("Triangle", typeof(CS2MapSpaceTriangle))]
        [XmlArray("Triangles")]
        public List<CS2MapSpaceTriangle>? Triangles { get; set; }
    }

    public struct CS2MapSpaceTriangle
    {
        [XmlAttribute("x0")]
        public float X0 { get; set; }
        [XmlAttribute("y0")]
        public float Y0 { get; set; }
        [XmlAttribute("x1")]
        public float X1 { get; set; }
        [XmlAttribute("y1")]
        public float Y1 { get; set; }
        [XmlAttribute("x2")]
        public float X2 { get; set; }
        [XmlAttribute("y2")]
        public float Y2 { get; set; }

    }

    public struct CS2MapSpaceBezier4
    {
        [XmlAttribute("x0")]
        public float X0 { get; set; }
        [XmlAttribute("y0")]
        public float Y0 { get; set; }
        [XmlAttribute("x1")]
        public float X1 { get; set; }
        [XmlAttribute("y1")]
        public float Y1 { get; set; }
        [XmlAttribute("x2")]
        public float X2 { get; set; }
        [XmlAttribute("y2")]
        public float Y2 { get; set; }
        [XmlAttribute("x3")]
        public float X3 { get; set; }
        [XmlAttribute("y3")]
        public float Y3 { get; set; }

        
    }

    public struct CS2Float2
    {
        [XmlAttribute("x")]
        public float X { get; set; }

        [XmlAttribute("y")]
        public float Y { get; set; }
    }

    public struct CS2Vector3
    {
        [XmlAttribute("x")]
        public float X { get; set; }
        [XmlAttribute("y")]
        public float Y { get; set; }
        [XmlAttribute("z")]
        public float Z { get; set; }

        public CS2Vector3(float x, float y, float z)
        {
            X = x; Y = y; Z = z;
        }
        public static implicit operator Vector3(CS2Vector3 v) => new Vector3(v.X, v.Y, v.Z);

    }

    public struct CS2Quaternion
    {
        [XmlAttribute("x")]
        public float X { get; set; }
        [XmlAttribute("y")]
        public float Y { get; set; }
        [XmlAttribute("z")]
        public float Z { get; set; }
        [XmlAttribute("w")]
        public float W { get; set; }

        public CS2Quaternion(float x,float y,float z,float w)
        {
            X = x; Y = y;Z = z; W = w;
        }

        public static implicit operator Quaternion(CS2Quaternion v) => new Quaternion(v.X,v.Y,v.Z,v.W);
    }
}

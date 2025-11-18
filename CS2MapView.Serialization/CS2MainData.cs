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

        /// <summary>
        /// 実際のワールド境界（MapExt2 などのマップ拡張 MOD をサポートするため）
        /// null の場合、ビューアはデフォルトのバニラ境界を使用します
        /// </summary>
        public CS2WorldBounds? WorldBounds { get; set; }

        public List<string>? TestList { get; set; }

    }

    /// <summary>
    /// ワールド境界情報（ワールド座標）
    /// </summary>
    public class CS2WorldBounds
    {
        /// <summary>
        /// 最小 X 座標
        /// </summary>
        [XmlAttribute("minX")]
        public float MinX { get; set; }

        /// <summary>
        /// 最小 Z 座標（マップ空間の Y に対応）
        /// </summary>
        [XmlAttribute("minZ")]
        public float MinZ { get; set; }

        /// <summary>
        /// 最大 X 座標
        /// </summary>
        [XmlAttribute("maxX")]
        public float MaxX { get; set; }

        /// <summary>
        /// 最大 Z 座標（マップ空間の Y に対応）
        /// </summary>
        [XmlAttribute("maxZ")]
        public float MaxZ { get; set; }
    }
}

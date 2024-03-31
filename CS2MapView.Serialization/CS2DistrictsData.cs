using System.Collections.Generic;
using System.Xml.Serialization;

namespace CS2MapView.Serialization
{
    /// <summary>
    /// ただのXMLのルート
    /// </summary>
    public class CS2DistrictsData
    {
        [XmlArrayItem("District", typeof(CS2DistrictInfo))]
        [XmlArray("Districts")]
        public List<CS2DistrictInfo>? Districts { get; set; }
    }
}

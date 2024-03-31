using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Xml.Serialization;

namespace CS2MapView.Serialization
{
    public class CS2Building
    {
        [XmlAttribute("entity")]
        public int Entity { get; set; }
        [XmlAttribute("prefab")]
        public int Prefab { get; set; }
        [XmlAttribute("name")]
        public string? Name { get; set; }
        [XmlAttribute("customName")]
        public bool IsCustomName { get; set; }
        [XmlAttribute("district")]
        public int CurrentDistrict { get; set; }
        [XmlAttribute("parent")]
        public int ParentBuilding { get; set; } = -1;
        [XmlIgnore]
        public CS2BuildingTypes BuildingType
        {
            get =>(CS2BuildingTypes)BuildingTypeSerialized;
            set =>BuildingTypeSerialized = (ulong)value;
        }
        [XmlAttribute("type")]
        public ulong BuildingTypeSerialized { get; set; } = 0;

        public CS2Vector3 Position { get; set; }

        public CS2Quaternion Rotation { get; set; }

        public CS2Geometry? AreaGeometry { get; set; }

    }

    public class CS2BuildingPrefab
    {
        [XmlAttribute("entity")]
        public int Entity { get; set; }

        [XmlAttribute("name")]
        public string? Name { get; set; }

        public CS2Vector3 Size { get; set; }
        
        public CS2BuildingPrefabSubInfo? SubInfo { get; set; }

    }
    public class CS2BuildingPrefabSubInfo
    {
        public byte EducationLevel { get; set; }
    }
    /// <summary>
    /// ただのXMLのルート
    /// </summary>
    public class CS2BuildingsData
    {
        
      
        [XmlArrayItem("Building", typeof(CS2Building))]
        [XmlArray("Buildings")]
        public List<CS2Building>? Buildings { get; set; }
        [XmlArrayItem("BuildingPrefab", typeof(CS2BuildingPrefab))]
        [XmlArray("BuildingPrefabs")]
        public List<CS2BuildingPrefab>? BuildingPrefabs { get; set; }
    }


    [Flags]
    public enum CS2BuildingTypes: ulong
    {
        None = 0,
        Hospital = 0x40000000,
        PowerPlant = 0x1,
        Transformer = 0x2,
        FreshWaterBuilding = 0x4,
        SewageBuilding = 0x8,
      //  StormWaterBuilding,
        TransportDepot = 0x10,
        TransportStation = 0x20,
        GarbageFacility = 0x40,
        FireStation= 0x80,
        PoliceStation = 0x100,
        RoadMaintenanceDepot = 0x200,
        PostFacility = 0x400,
        TelecomFacility = 0x800,
        School = 0x1000,
        EmergencyShelter = 0x2000,
     //   DisasterFacility,
     //   FirewatchTower,
        Park = 0x4000,
        DeathcareFacility = 0x8000,
        Prison = 0x10000,
        AdminBuilding = 0x20000,
        WelfareOffice = 0x40000,
        ResearchFacility = 0x80000,
        ParkMaintenanceDepot = 0x100000,
        ParkingFacility = 0x200000,
        Battery = 0x400000,
        ResidentialBuilding = 0x800000,
        CommercialBuilding = 0x1000000,
        IndustrialBuilding = 0x2000000,
        OfficeBuilding = 0x4000000,
     //   SignatureResidential,
        ExtractorBuilding = 0x8000000,
      //  SignatureCommercial,
      //  SignatureIndustrial,
     //   SignatureOffice,

        CargoTransportStation = 0x10000000,
        UniqueBuilding = 0x20000000
    }
}

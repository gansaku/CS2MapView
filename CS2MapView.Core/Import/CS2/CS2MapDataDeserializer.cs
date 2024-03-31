using CS2MapView.Serialization;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Xml.Serialization;

namespace CS2MapView.Import.CS2
{
    public static class CS2MapDataDeserializer
    {

        public static Task<CS2MapDataSet> Deserialize(ZipArchive zip)
        {
            var ret = new CS2MapDataSet();

            bool ReadXmlEntry<T>(string fileName,
                [NotNullWhen(true)] out T? value,
                [NotNullWhen(false)] out string? loadErrorMessage) where T : class
            {
                var xmlEntry = zip.GetEntry(fileName);

                value = null;
                loadErrorMessage = null;
                if (xmlEntry is null)
                {
                    loadErrorMessage = $"entry {fileName} was not found.";
                    return false;
                }
                XmlSerializer xs = new(typeof(T));
                using Stream stream = xmlEntry.Open();
                if (xs.Deserialize(stream) is T md)
                {
                    value = md;
                    return true;
                }
                else
                {
                    loadErrorMessage = "could not deserialize cs2map file.";
                    return false;
                }
            }
            string? error;


            CS2MapDataSet ReturnWhenError()
            {
                ret!.LoadError = true;
                ret.LoadErrorMessage = error;
                return ret;
            }

            return Task.Run(() =>
            {
                if (ReadXmlEntry<CS2MainData>(CS2MapDataZipEntryKeys.MainXml, out var data, out error))
                {
                    if (data.Terrain is null || data.Water is null)
                    {
                        ret.LoadError = true;
                        ret.LoadErrorMessage = "terrain/water info was null.";
                        return ret;
                    }
                    ret.MainData = data;

                }
                else
                {
                    return ReturnWhenError();
                }

                if (ReadXmlEntry<CS2BuildingsData>(CS2MapDataZipEntryKeys.BuildingsXml, out var buildingData, out error))
                {
                    ret.Buildings = [.. buildingData.Buildings];
                    ret.BuildingPrefabs = [.. buildingData.BuildingPrefabs];
                }
                else
                {
                    return ReturnWhenError();
                }
                if (ReadXmlEntry<CS2DistrictsData>(CS2MapDataZipEntryKeys.DistrictsXml, out var districtsData, out error))
                {
                    ret.Districts = [.. districtsData.Districts];
                }
                else
                {
                    return ReturnWhenError();
                }

                if (ReadXmlEntry<CS2RoadsData>(CS2MapDataZipEntryKeys.RoadsXml, out var roadsData, out error))
                {
                    ret.RoadInfo = roadsData;
                }
                else
                {
                    return ReturnWhenError();
                }
                if (ReadXmlEntry<CS2RailsData>(CS2MapDataZipEntryKeys.RailsXml, out var railsData, out error))
                {
                    ret.RailInfo = railsData;
                }
                else
                {
                    return ReturnWhenError();
                }
                if(ReadXmlEntry<CS2TransportLineData>(CS2MapDataZipEntryKeys.TransportLinesXml,out var transportLineData, out error))
                {
                    ret.TransportInfo = transportLineData;
                }
                else
                {
                    return ReturnWhenError();
                }

                var terrainEntry = zip.GetEntry(CS2MapDataZipEntryKeys.TerrainData);
                if (terrainEntry is null)
                {
                    ret.LoadError = true;
                    ret.LoadErrorMessage = "entry terrain.dat was not found.";
                    return ret;
                }
                var waterEntry = zip.GetEntry(CS2MapDataZipEntryKeys.WaterData);
                if (waterEntry is null)
                {
                    ret.LoadError = true;
                    ret.LoadErrorMessage = "entry water.dat was not found.";
                    return ret;
                }

                ret.TerrainArray = TerrainToFloatArray(
                    ReadArray<ushort>(terrainEntry, (int)Math.Pow(ret.MainData.Terrain.Resolution.Z, 2)),
                    ret.MainData.Terrain, ret.MainData.SeaLevel);
                var waterArray = ReadArray<float>(waterEntry, (int)Math.Pow(ret.MainData.Water.Resolution.Z, 2));
                RewriteWaterArray(waterArray, ret.MainData.Water);
                ret.WaterArray = waterArray;

                return ret;
            });
        }
        private unsafe static T[] ReadArray<T>(ZipArchiveEntry entry, int arraySize) where T : unmanaged
        {
            using Stream stream = entry.Open();


            T[] buf = new T[arraySize];
            fixed (void* pbuf = buf)
            {
                var span = new Span<byte>(pbuf, arraySize * sizeof(T));
                stream.ReadExactly(span);
            }
            return buf;
        }
        private static float[] TerrainToFloatArray(ushort[] input, CS2TerrainWaterDataInfo terrainInfo, float seaLevel)
        {
            float[] ret = new float[input.Length];
            var rspan = new ReadOnlySpan<ushort>(input);
            var wspan = new Span<float>(ret);
            var min = float.MaxValue;
            var max = float.MinValue;
            var origMin = ushort.MaxValue;
            var origMax = ushort.MinValue;
            for (int i = 0; i < input.Length; i++)
            {
                wspan[i] = ToWorldSpaceHeight(rspan[i], terrainInfo.Scale.Y, terrainInfo.Offset.Y) - seaLevel;
                if (wspan[i] < min) min = wspan[i];
                if (wspan[i] > max) max = wspan[i];
                if (rspan[i] < origMin) origMin = rspan[i];
                if (rspan[i] > origMax) origMax = rspan[i];
            }
            Debug.Print($"terrain orig:{origMin}~{origMax} calced:{min}~{max} scale={terrainInfo.Scale} offset={terrainInfo.Offset}");
            return ret;
        }

        private static void RewriteWaterArray(Span<float> span, CS2TerrainWaterDataInfo waterInfo)
        {
            for (int i = 0; i < span.Length; i++)
            {
                span[i] = ToWorldSpaceHeight(span[i], waterInfo.Scale.Y, waterInfo.Offset.Y);
            }
        }


        private static float ToWorldSpaceHeight(float rawVal, float dataScale, float dataOffset)
        {
            return rawVal / dataScale - dataOffset;
        }
    }
}

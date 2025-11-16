using CS2MapView.Serialization;
using Game.Simulation;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using System.Numerics;
using Unity.Collections.LowLevel.Unsafe;
using System.Linq;

namespace CS2MapView.Exporter.Systems
{

    internal class TerrainReader
    {
        private readonly TerrainSystem m_terrainSystem;
        private readonly WaterSystem m_waterSystem;

        internal TerrainReader(SystemRefs systemRefs)
        {
            m_terrainSystem = systemRefs.GetOrCreateSystemManaged<TerrainSystem>();
            m_waterSystem = systemRefs.GetOrCreateSystemManaged<WaterSystem>();
        }
        internal unsafe void ReadAndWriteTerrain(CS2MainData data, ZipArchive zip, CS2MapViewModSettings.ResolutionRestriction heightMapRestriction)
        {
            var hd = m_terrainSystem!.GetHeightData(true);
            var resolutionXz = hd.resolution.x;
            var scaleXz = hd.scale.x;

            int maxSize = heightMapRestriction switch
            {
                CS2MapViewModSettings.ResolutionRestriction.Width1024 => 1024,
                CS2MapViewModSettings.ResolutionRestriction.Width2048 => 2048,
                CS2MapViewModSettings.ResolutionRestriction.Width4096 => 4096,
                _ => int.MaxValue
            };
            if (maxSize >= resolutionXz)
            {
                ZipDataWriter.WriteZipBinaryEntry(zip, CS2MapDataZipEntryKeys.TerrainData, hd.heights.AsReadOnly().GetUnsafeReadOnlyPtr(), sizeof(ushort) * resolutionXz * resolutionXz);
            }
            else
            {
                
                var resizedArray = HeightMapResizer.Resize(hd.heights.AsReadOnlySpan(), resolutionXz, resolutionXz, maxSize, maxSize);
                resolutionXz = maxSize;
                scaleXz /= hd.resolution.x / maxSize;
                fixed (void* ptr = resizedArray)
                {
                    ZipDataWriter.WriteZipBinaryEntry(zip, CS2MapDataZipEntryKeys.TerrainData, ptr, sizeof(ushort) * maxSize * maxSize);
                }
            }

            
            data.Terrain = new CS2TerrainWaterDataInfo
            {
                Resolution = new Vector3(resolutionXz, hd.resolution.y, resolutionXz),
                Offset = new Vector3(hd.offset.x, hd.offset.y, hd.offset.z),
                Scale = new Vector3(scaleXz, hd.scale.y, scaleXz)
            };
        }

        internal unsafe void ReadAndWriteWater(CS2MainData data, ZipArchive zip, CS2MapViewModSettings.ResolutionRestriction heightMapRestriction)
        {

            var sd = m_waterSystem!.GetSurfaceData(out var _);
            var array = sd.depths.Select(t => t.m_Depth).ToArray();
            var resolutionXz = sd.resolution.x;
            var scaleXz = sd.scale.x;
            int maxSize = heightMapRestriction switch
            {
                CS2MapViewModSettings.ResolutionRestriction.Width1024 => 1024,
                CS2MapViewModSettings.ResolutionRestriction.Width2048 => 2048,
                CS2MapViewModSettings.ResolutionRestriction.Width4096 => 4096,
                _ => int.MaxValue
            };

            if (maxSize < resolutionXz)
            {
                var resizedArray = HeightMapResizer.Resize(array.AsSpan(), resolutionXz, resolutionXz, maxSize, maxSize);
                resolutionXz = maxSize;
                scaleXz /= sd.resolution.x / maxSize;
                fixed (void* p = resizedArray)
                {
                    ZipDataWriter.WriteZipBinaryEntry(zip, CS2MapDataZipEntryKeys.WaterData, p, sizeof(float) * maxSize * maxSize);
                }
            }
            else
            {
                fixed (void* p = array)
                {
                    ZipDataWriter.WriteZipBinaryEntry(zip, CS2MapDataZipEntryKeys.WaterData, p, sizeof(float) * sd.resolution.z * sd.resolution.z);
                }
            }


            data.SeaLevel = m_waterSystem.SeaLevel;
            data.Water = new CS2TerrainWaterDataInfo
            {
                Resolution = new Vector3(resolutionXz, sd.resolution.y, resolutionXz),
                Offset = new Vector3(sd.offset.x, sd.offset.y, sd.offset.z),
                Scale = new Vector3(scaleXz, sd.scale.y, scaleXz)
            };
        }


       
    }
}

using CS2MapView.Serialization;
using Game.Simulation;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using System.Numerics;
using Unity.Collections.LowLevel.Unsafe;
using System.Linq;
using System.Reflection; // added for reflection
using System.Collections; // added for IEnumerable handling

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
            // The game updated and removed/renamed WaterSurfaceData. We now access data via reflection to
            // avoid a hard type dependency and stay backward/forward compatible.
            try
            {
                if (m_waterSystem == null)
                {
                    throw new InvalidOperationException("WaterSystem was null");
                }

                static object? GetFieldOrProp(object obj, string name)
                {
                    var t = obj.GetType();
                    var fi = t.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                    if (fi != null) return fi.GetValue(obj);
                    var pi = t.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                    if (pi != null) return pi.GetValue(obj);
                    return null;
                }

                var wsType = m_waterSystem.GetType();
                var surfaceMethod = wsType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .FirstOrDefault(m => m.Name == "GetSurfaceData");
                if (surfaceMethod == null)
                {
                    throw new MissingMethodException(wsType.FullName, "GetSurfaceData");
                }

                object[] args;
                var parameters = surfaceMethod.GetParameters();
                if (parameters.Length == 1 && parameters[0].IsOut)
                {
                    var pType = parameters[0].ParameterType.GetElementType() ?? parameters[0].ParameterType;
                    object outArg = pType.IsValueType ? Activator.CreateInstance(pType)! : null!;
                    args = new[] { outArg };
                }
                else if (parameters.Length == 0)
                {
                    args = Array.Empty<object>();
                }
                else
                {
                    args = parameters.Select(p => p.ParameterType.IsValueType ? Activator.CreateInstance(p.ParameterType)! : null!).ToArray();
                }

                var surfaceData = surfaceMethod.Invoke(m_waterSystem, args);
                if (surfaceData == null)
                {
                    throw new InvalidOperationException("GetSurfaceData returned null");
                }

                var resolutionObj = GetFieldOrProp(surfaceData, "resolution");
                int resX = 0, resY = 0, resZ = 0;
                if (resolutionObj != null)
                {
                    resX = Convert.ToInt32(GetFieldOrProp(resolutionObj, "x") ?? 0);
                    resY = Convert.ToInt32(GetFieldOrProp(resolutionObj, "y") ?? 0);
                    resZ = Convert.ToInt32(GetFieldOrProp(resolutionObj, "z") ?? resX);
                }

                var scaleObj = GetFieldOrProp(surfaceData, "scale");
                float scaleX = 1f, scaleY = 1f, scaleZ = 1f;
                if (scaleObj != null)
                {
                    scaleX = Convert.ToSingle(GetFieldOrProp(scaleObj, "x") ?? 1f);
                    scaleY = Convert.ToSingle(GetFieldOrProp(scaleObj, "y") ?? 1f);
                    scaleZ = Convert.ToSingle(GetFieldOrProp(scaleObj, "z") ?? scaleX);
                }

                var offsetObj = GetFieldOrProp(surfaceData, "offset");
                float offX = 0f, offY = 0f, offZ = 0f;
                if (offsetObj != null)
                {
                    offX = Convert.ToSingle(GetFieldOrProp(offsetObj, "x") ?? 0f);
                    offY = Convert.ToSingle(GetFieldOrProp(offsetObj, "y") ?? 0f);
                    offZ = Convert.ToSingle(GetFieldOrProp(offsetObj, "z") ?? 0f);
                }

                var depthsObj = GetFieldOrProp(surfaceData, "depths");
                if (depthsObj == null)
                {
                    throw new MissingFieldException(surfaceData.GetType().FullName, "depths");
                }

                var depthValues = new List<float>();
                foreach (var item in (IEnumerable)depthsObj)
                {
                    object? val = GetFieldOrProp(item, "m_Depth") ?? GetFieldOrProp(item, "depth");
                    if (val != null)
                    {
                        depthValues.Add(Convert.ToSingle(val));
                    }
                }

                if (depthValues.Count == 0 || resX == 0)
                {
                    throw new InvalidOperationException("No water depth data extracted");
                }

                int resolutionXz = resX;
                float scaleXz = scaleX;

                int maxSize = heightMapRestriction switch
                {
                    CS2MapViewModSettings.ResolutionRestriction.Width1024 => 1024,
                    CS2MapViewModSettings.ResolutionRestriction.Width2048 => 2048,
                    CS2MapViewModSettings.ResolutionRestriction.Width4096 => 4096,
                    _ => int.MaxValue
                };

                float[] array = depthValues.ToArray();
                if (maxSize < resolutionXz)
                {
                    var resizedArray = HeightMapResizer.Resize(array.AsSpan(), resolutionXz, resolutionXz, maxSize, maxSize);
                    resolutionXz = maxSize;
                    scaleXz /= (float)resX / maxSize;
                    fixed (void* p = resizedArray)
                    {
                        ZipDataWriter.WriteZipBinaryEntry(zip, CS2MapDataZipEntryKeys.WaterData, p, sizeof(float) * maxSize * maxSize);
                    }
                }
                else
                {
                    fixed (void* p = array)
                    {
                        ZipDataWriter.WriteZipBinaryEntry(zip, CS2MapDataZipEntryKeys.WaterData, p, sizeof(float) * resolutionXz * resolutionXz);
                    }
                }

                data.SeaLevel = m_waterSystem.SeaLevel;
                data.Water = new CS2TerrainWaterDataInfo
                {
                    Resolution = new Vector3(resolutionXz, resY, resolutionXz),
                    Offset = new Vector3(offX, offY, offZ),
                    Scale = new Vector3(scaleXz, scaleY, scaleXz)
                };
            }
            catch (Exception ex)
            {
                // Fallback: create an empty water dataset matching terrain resolution so importer keeps working.
                CS2MapViewSystem.Log.Warn($"[TerrainReader] Water export failed (will fallback to blank water): {ex.Message}");
                try
                {
                    int resolution = (int)(data.Terrain?.Resolution.X ?? 0);
                    if (resolution <= 0)
                    {
                        resolution = 256; // arbitrary small fallback
                    }
                    float[] zeros = new float[resolution * resolution];
                    fixed (void* p = zeros)
                    {
                        ZipDataWriter.WriteZipBinaryEntry(zip, CS2MapDataZipEntryKeys.WaterData, p, sizeof(float) * zeros.Length);
                    }

                    data.Water = new CS2TerrainWaterDataInfo
                    {
                        Resolution = new Vector3(resolution, data.Terrain?.Resolution.Y ?? 0, resolution),
                        Offset = new Vector3(0, 0, 0),
                        Scale = new Vector3(data.Terrain?.Scale.X ?? 1f, data.Terrain?.Scale.Y ?? 1f, data.Terrain?.Scale.Z ?? 1f)
                    };
                }
                catch (Exception inner)
                {
                    CS2MapViewSystem.Log.Error($"[TerrainReader] Secondary failure while writing fallback water data: {inner}");
                }
            }
        }
    }
}

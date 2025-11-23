using CS2MapView.Serialization;
using Game.Simulation;
using Unity.Mathematics;

namespace CS2MapView.Exporter.Systems
{
    /// <summary>
    /// Read world boundary information
    /// Supports both vanilla and MapExt2 extended maps
    /// </summary>
    internal class WorldBoundsReader
    {
        private readonly TerrainSystem m_terrainSystem;

        internal WorldBoundsReader(SystemRefs systemRefs)
        {
            m_terrainSystem = systemRefs.GetOrCreateSystemManaged<TerrainSystem>();
        }

        /// <summary>
        /// Get world bounds from the terrain system
        /// Properly handles MapExt2 map extensions
        /// </summary>
        internal CS2WorldBounds GetWorldBounds()
        {
            // Use TerrainSystem.GetTerrainBounds() method
            // This method is patched by MapExt2 and returns the correct bounds
            var bounds3 = m_terrainSystem.GetTerrainBounds();
            
            return new CS2WorldBounds
            {
                MinX = bounds3.min.x,
                MinZ = bounds3.min.z,
                MaxX = bounds3.max.x,
                MaxZ = bounds3.max.z
            };
        }
    }
}

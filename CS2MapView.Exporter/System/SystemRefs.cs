using System;
using System.Collections.Generic;
using System.Text;
using Unity.Entities;
using Colossal.Entities;
using System.Linq;
using Unity.Collections;

namespace CS2MapView.Exporter.System
{
    internal class SystemRefs
    {
        private World World { get; set; }
        private EntityManager EntityManager { get; set; }

        internal class EntityQueries
        {
            internal EntityQuery AllDistricts { get; set; }
            internal EntityQuery AllBuildings { get; set; }
            internal EntityQuery AllNodes { get; set; }
            internal EntityQuery AllRoads { get; set; }
            internal EntityQuery AllRails { get; set; }
            internal EntityQuery AllTransportLines { get; set; }
            internal EntityQuery AllTransportStops { get; set; }
        }

        internal EntityQueries Queries { get; } = new EntityQueries();

        internal delegate EntityQuery CreateQueryDelegate(params ComponentType[] entities);
        internal SystemRefs(World world, EntityManager entityManager)
        {
            World = world;
            EntityManager = entityManager;

        }

        internal T GetOrCreateSystemManaged<T>() where T : SystemBase => World.GetOrCreateSystemManaged<T>();
        internal bool HasComponent<T>(Entity entity) => EntityManager.HasComponent<T>(entity);
        internal T GetComponentData<T>(Entity entity) where T : unmanaged, IComponentData => EntityManager.GetComponentData<T>(entity);
        internal bool TryGetBuffer<T>(Entity entity, bool isReadOnly, out DynamicBuffer<T> buffer) where T : unmanaged, IBufferElementData => EntityManager.TryGetBuffer(entity, isReadOnly, out buffer);
        internal bool TryGetComponent<T>(Entity entity, out T component) where T : unmanaged, IComponentData => EntityManager.TryGetComponent(entity, out component);

        private IEnumerable<string> DebugGetComponentType(Entity entity)
        {
            using var types = EntityManager.GetComponentTypes(entity, Allocator.Persistent);
            foreach (var t in types)
            {
                yield return t.GetManagedType().FullName;
            }
        }
        internal IEnumerable<string> DebugGetComponentTypeGroup(ReadOnlySpan<Entity> span)
        {
            List<string> archtypes = new List<string>();
            foreach (var entity in span)
            {
                archtypes.AddRange(DebugGetComponentType(entity));
            }
            return archtypes.GroupBy(t => t).Select(t => $"{t.Key},{t.Count()}").OrderBy(t => t);
        }
    }
}

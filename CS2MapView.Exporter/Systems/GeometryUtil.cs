using Colossal.Mathematics;
using CS2MapView.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Mathematics;

namespace CS2MapView.Exporter.Systems
{
    internal static class GeometryUtil
    {
        internal static CS2Vector3 ConvertForSerialize(float3 f) => new CS2Vector3(f.x, f.y, f.z);

        internal static CS2Quaternion ConvertForSerialize(quaternion q) => new CS2Quaternion(q.value.x, q.value.y, q.value.z, q.value.w);

        internal static CS2MapSpaceBezier4 ToMapSpaceBezier4(Bezier4x3 b) => new CS2MapSpaceBezier4
        {
            X0 = b.a.x,
            Y0 = -b.a.z,
            X1 = b.b.x,
            Y1 = -b.b.z,
            X2 = b.c.x,
            Y2 = -b.c.z,
            X3 = b.d.x,
            Y3 = -b.d.z
        };

        internal static (float, float) ToMapSpacePoint(float3 value) => (value.x, -value.z);
    }
}

using CS2MapView.Serialization;
using Gfw.Common;
using SkiaSharp;
using System.Numerics;

namespace CS2MapView.Util.CS2
{
    internal static class CS2GeometoryEx
    {
        /// <summary>
        /// ゲーム内の座標を地図用の座標に変換する
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        internal static SKPoint ToMapSpace(this Vector3 vec) => new(vec.X, -vec.Z);
        /// <summary>
        /// ゲーム内の座標を地図用の座標に変換する
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        internal static SKPoint ToMapSpace(this CS2Vector3 vec) => new(vec.X, -vec.Z);
        /// <summary>
        /// Y軸を軸に回転させるQuaternionから角度を抜き出す
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        internal static float RotationY(this CS2Quaternion q) => (float)Math.Atan2(q.Y, q.W) * 2f;// => (float)Math.Asin(q.Y) * 2f;
        /// <summary>
        /// Y軸を軸に回転させるQuaternionを表すVector4から角度を抜き出す
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        internal static float RotationY(this Vector4 v) => (float)Math.Asin(v.Y) * 2f;
        /// <summary>
        /// ベジエを逆方向に変換
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        internal static CS2MapSpaceBezier4 Reverse(this CS2MapSpaceBezier4 b) => new() { X0 = b.X3, Y0 = b.Y3, X1 = b.X2, Y1 = b.Y2, X2 = b.X1, Y2 = b.Y1, X3 = b.X0, Y3 = b.Y0 };

        /// <summary>
        /// ジオメトリを表示用のパスに変換します。
        /// </summary>
        /// <param name="xmlGeo"></param>
        /// <returns></returns>
        internal static SKPath ToPath(this CS2Geometry xmlGeo)
        {
            var pathes = (xmlGeo.Triangles ?? []).Select(tri =>
            {
                var p = new SKPath();
                p.MoveTo(new(tri.X0, tri.Y0));
                p.LineTo(new(tri.X1, tri.Y1));
                p.LineTo(new(tri.X2, tri.Y2));
                p.LineTo(new(tri.X0, tri.Y0));
                p.Close();
                return p;
            });

            var ret = SKPathEx.Union(pathes);
            pathes.ForEach(p => p.Dispose());
            return ret;
        }

    }
}

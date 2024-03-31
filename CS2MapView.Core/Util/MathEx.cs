using SkiaSharp;
using System.Numerics;

namespace CS2MapView.Util;

internal static class MathEx
{


    internal static double Angle(SKPoint a, SKPoint b)
    {
        return Math.Atan2(b.Y - a.Y, b.X - a.X);
    }
    internal static double AngleXZ(Vector3 a, Vector3 b)
    {
        return Math.Atan2(b.Z - a.Z, b.X - a.X);
    }



    internal static double Distance(SKPoint p1, SKPoint p2)
    {
        return Math.Sqrt(DistanceSqr(p1, p2));
    }

    internal static SKPoint SlidePoint(SKPoint from, double angle, double length)
    {
        return new SKPoint((float)(from.X + Math.Cos(angle) * length), (float)(from.Y + Math.Sin(angle) * length));
    }

    internal static double DistanceSqr(SKPoint a, SKPoint b)
    {
        return (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y);
    }

    internal static double RadianToDegree(double rad)
    {
        return rad * 180 / Math.PI;
    }
}

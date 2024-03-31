using SkiaSharp;

namespace CS2MapView.Util
{
    /// <summary>
    /// SKPathを使用するユーティリティ
    /// </summary>
    public static class SKPathEx
    {
        /// <summary>
        /// 複数のパスの和集合であるパスを作成します。
        /// </summary>
        /// <param name="pathes"></param>
        /// <returns></returns>
        public static SKPath Union(IEnumerable<SKPath> pathes)
        {
            var count = pathes.Count();
            if (count == 0)
            {
                return new SKPath();
            }
            else if (count == 1)
            {
                return pathes.First();
            }
            SKPath result = pathes.First();

            foreach (var p in pathes.TakeLast(count - 1))
            {
                var tmpResult = result.Op(p, SKPathOp.Union);
                if (tmpResult is null)
                {
                    return result;
                }
                result.Dispose();
                result = tmpResult;

            }
            return result;
        }

        /// <summary>
        /// 複数のパスの和集合であるパスを作成します。
        /// </summary>
        /// <param name="pathes"></param>
        /// <returns></returns>
        public static SKPath Union(params SKPath[] pathes) => Union((IEnumerable<SKPath>)pathes);
        /// <summary>
        /// 角の丸い長方形のパスを作成します。
        /// </summary>
        /// <param name="p1">点1</param>
        /// <param name="p2">点2</param>
        /// <param name="width">幅</param>
        /// <param name="startRound">点1側を丸めるか</param>
        /// <param name="endRound">点2側を丸めるか</param>
        /// <returns></returns>
        public static SKPath CreateRoundBorderedLinePath(SKPoint p1, SKPoint p2, float width, bool startRound, bool endRound)
        {
            SKPath path = new() { FillType = SKPathFillType.Winding };
            double angle = Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
            double anglea = angle + Math.PI / 2;//+90
            double angleb = angle - Math.PI / 2;//-90
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);
            double cosa = Math.Cos(anglea);
            double sina = Math.Sin(anglea);
            double cosb = Math.Cos(angleb);
            double sinb = Math.Sin(angleb);

            var p1a = new SKPoint((float)(p1.X + width * cosa / 2), (float)(p1.Y + width * sina / 2));
            var p1b = new SKPoint((float)(p1.X + width * cosb / 2), (float)(p1.Y + width * sinb / 2));
            var p2a = new SKPoint((float)(p2.X + width * cosa / 2), (float)(p2.Y + width * sina / 2));
            var p2b = new SKPoint((float)(p2.X + width * cosb / 2), (float)(p2.Y + width * sinb / 2));
            var p2a2b_b1 = new SKPoint((float)(p2a.X + width * cos / 2), (float)(p2a.Y + width * sin / 2));
            var p2a2b_b2 = new SKPoint((float)(p2b.X + width * cos / 2), (float)(p2b.Y + width * sin / 2));
            var p1b1a_b1 = new SKPoint((float)(p1b.X - width * cos / 2), (float)(p1b.Y - width * sin / 2));
            var p1b1a_b2 = new SKPoint((float)(p1a.X - width * cos / 2), (float)(p1a.Y - width * sin / 2));

            path.MoveTo(p1a);
            path.LineTo(p2a);
            if (endRound)
            {
                path.CubicTo(p2a2b_b1, p2a2b_b2, p2b);
            }
            else
            {
                path.LineTo(p2b);
            }
            path.LineTo(p1b);

            if (startRound)
            {
                path.CubicTo(p1b1a_b1, p1b1a_b2, p1a);
            }
            else
            {
                path.LineTo(p1a);
            }
            path.Close();

            return path;

        }
        /// <summary>
        /// Rectと同一のPathを返します。
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static SKPath RectToPath(SKRect rect)
        {
            var path = new SKPath();
            path.MoveTo(rect.Left, rect.Top);
            path.LineTo(rect.Left, rect.Bottom);
            path.LineTo(rect.Right, rect.Bottom);
            path.LineTo(rect.Right, rect.Top);
            path.Close();
            return path;
        }
        /// <summary>
        /// 長方形に指定の変換をかけ、それを含む最小の軸に平行な長方形を返します。
        /// </summary>
        /// <param name="r"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static SKRect GetTransformedRectBound(SKRect r, SKMatrix matrix)
        {
            var points = new SKPoint[] {
                new(r.Left, r.Top),
                new(r.Left, r.Bottom),
                new(r.Right, r.Bottom),
                new(r.Right,r.Top)};
            SKPoint[] transformed = points.Select(matrix.MapPoint).ToArray();
            using var path = new SKPath();
            path.MoveTo(transformed[0]);
            path.LineTo(transformed[1]);
            path.LineTo(transformed[2]);
            path.LineTo(transformed[3]);
            path.Close();
            return path.Bounds;
        }

        /// <summary>
        /// 矩形の中心を返します。
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static SKPoint Center(this SKRect rect)
        {
            return new(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
        }

        //https://learn.microsoft.com/ja-jp/xamarin/xamarin-forms/user-interface/graphics/skiasharp/curves/information

        private static SKPoint[] Interpolate(SKPoint pt0, SKPoint pt1)
        {
            int count = (int)Math.Max(1, Length(pt0, pt1));
            SKPoint[] points = new SKPoint[count];

            for (int i = 0; i < count; i++)
            {
                float t = (i + 1f) / count;
                float x = (1 - t) * pt0.X + t * pt1.X;
                float y = (1 - t) * pt0.Y + t * pt1.Y;
                points[i] = new SKPoint(x, y);
            }

            return points;
        }

        private static SKPoint[] FlattenCubic(SKPoint pt0, SKPoint pt1, SKPoint pt2, SKPoint pt3)
        {
            int count = (int)Math.Max(1, Length(pt0, pt1) + Length(pt1, pt2) + Length(pt2, pt3));
            SKPoint[] points = new SKPoint[count];

            for (int i = 0; i < count; i++)
            {
                float t = (i + 1f) / count;
                float x = (1 - t) * (1 - t) * (1 - t) * pt0.X +
                            3 * t * (1 - t) * (1 - t) * pt1.X +
                            3 * t * t * (1 - t) * pt2.X +
                            t * t * t * pt3.X;
                float y = (1 - t) * (1 - t) * (1 - t) * pt0.Y +
                            3 * t * (1 - t) * (1 - t) * pt1.Y +
                            3 * t * t * (1 - t) * pt2.Y +
                            t * t * t * pt3.Y;
                points[i] = new SKPoint(x, y);
            }

            return points;
        }

        private static SKPoint[] FlattenQuadratic(SKPoint pt0, SKPoint pt1, SKPoint pt2)
        {
            int count = (int)Math.Max(1, Length(pt0, pt1) + Length(pt1, pt2));
            SKPoint[] points = new SKPoint[count];

            for (int i = 0; i < count; i++)
            {
                float t = (i + 1f) / count;
                float x = (1 - t) * (1 - t) * pt0.X + 2 * t * (1 - t) * pt1.X + t * t * pt2.X;
                float y = (1 - t) * (1 - t) * pt0.Y + 2 * t * (1 - t) * pt1.Y + t * t * pt2.Y;
                points[i] = new SKPoint(x, y);
            }

            return points;
        }

        private static SKPoint[] FlattenConic(SKPoint pt0, SKPoint pt1, SKPoint pt2, float weight)
        {
            int count = (int)Math.Max(1, Length(pt0, pt1) + Length(pt1, pt2));
            SKPoint[] points = new SKPoint[count];

            for (int i = 0; i < count; i++)
            {
                float t = (i + 1f) / count;
                float denominator = (1 - t) * (1 - t) + 2 * weight * t * (1 - t) + t * t;
                float x = (1 - t) * (1 - t) * pt0.X + 2 * weight * t * (1 - t) * pt1.X + t * t * pt2.X;
                float y = (1 - t) * (1 - t) * pt0.Y + 2 * weight * t * (1 - t) * pt1.Y + t * t * pt2.Y;
                x /= denominator;
                y /= denominator;
                points[i] = new SKPoint(x, y);
            }

            return points;
        }

        private static double Length(SKPoint pt0, SKPoint pt1)
        {
            return Math.Sqrt(Math.Pow(pt1.X - pt0.X, 2) + Math.Pow(pt1.Y - pt0.Y, 2));
        }

        public static SKPath CloneWithTransform(this SKPath pathIn, Func<SKPoint, SKPoint> transform)
        {
            SKPath pathOut = new SKPath();

            using (SKPath.RawIterator iterator = pathIn.CreateRawIterator())
            {
                SKPoint[] points = new SKPoint[4];
                SKPathVerb pathVerb = SKPathVerb.Move;
                SKPoint firstPoint = new SKPoint();
                SKPoint lastPoint = new SKPoint();

                while ((pathVerb = iterator.Next(points)) != SKPathVerb.Done)
                {
                    switch (pathVerb)
                    {
                        case SKPathVerb.Move:
                            pathOut.MoveTo(transform(points[0]));
                            firstPoint = lastPoint = points[0];
                            break;

                        case SKPathVerb.Line:
                            SKPoint[] linePoints = Interpolate(points[0], points[1]);

                            foreach (SKPoint pt in linePoints)
                            {
                                pathOut.LineTo(transform(pt));
                            }

                            lastPoint = points[1];
                            break;

                        case SKPathVerb.Cubic:
                            SKPoint[] cubicPoints = FlattenCubic(points[0], points[1], points[2], points[3]);

                            foreach (SKPoint pt in cubicPoints)
                            {
                                pathOut.LineTo(transform(pt));
                            }

                            lastPoint = points[3];
                            break;

                        case SKPathVerb.Quad:
                            SKPoint[] quadPoints = FlattenQuadratic(points[0], points[1], points[2]);

                            foreach (SKPoint pt in quadPoints)
                            {
                                pathOut.LineTo(transform(pt));
                            }

                            lastPoint = points[2];
                            break;

                        case SKPathVerb.Conic:
                            SKPoint[] conicPoints = FlattenConic(points[0], points[1], points[2], iterator.ConicWeight());

                            foreach (SKPoint pt in conicPoints)
                            {
                                pathOut.LineTo(transform(pt));
                            }

                            lastPoint = points[2];
                            break;

                        case SKPathVerb.Close:
                            SKPoint[] closePoints = Interpolate(lastPoint, firstPoint);

                            foreach (SKPoint pt in closePoints)
                            {
                                pathOut.LineTo(transform(pt));
                            }

                            firstPoint = lastPoint = new SKPoint(0, 0);
                            pathOut.Close();
                            break;
                    }
                }
            }
            return pathOut;
        }
    }
}

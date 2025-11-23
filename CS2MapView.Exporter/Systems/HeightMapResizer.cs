using System;

namespace CS2MapView.Exporter.Systems
{
    internal class HeightMapResizer
    {
        internal static ushort[] Resize(ReadOnlySpan<ushort> inputSpan, int inputWidth, int inputHeight, int outputWidth, int outputHeight)
        {
            ushort[] result = new ushort[outputWidth * outputHeight];

            Span<ushort> target = result;

            ushort GetValue(ReadOnlySpan<ushort> rs, int x, int y)
            {
                float rx = (float)inputWidth / outputWidth;
                float ry = (float)inputHeight / outputHeight;

                float xstart = x * rx;
                float xend = (x + 1) * rx;
                float ystart = y * ry;
                float yend = (y + 1) * ry;

                float sum = 0f;
                float addedPixels = 0f;

                for (int sy = (int)ystart; sy < Math.Ceiling(yend); sy++)
                {
                    float yrate;
                    if (ystart - sy > 0)
                    {
                        yrate = ystart - sy;
                    }
                    else if (yend - sy < 1f)
                    {
                        yrate = yend - sy;
                    }
                    else
                    {
                        yrate = 1f;
                    }
                    for (int sx = (int)xstart; sx < Math.Ceiling(xend); sx++)
                    {
                        float xrate;
                        if (xstart - sx > 0)
                        {
                            xrate = xstart - sx;
                        }
                        else if (xend - sx < 1f)
                        {
                            xrate = xend - sx;
                        }
                        else
                        {
                            xrate = 1f;
                        }
                        sum += rs[sy * inputWidth + sx] * yrate * xrate;
                        addedPixels += yrate * xrate;
                    }
                }

                float v = sum / addedPixels;
                return (ushort)v;
            }


            for (int y = 0; y < outputHeight; y++)
            {
                for (int x = 0; x < outputWidth; x++)
                {
                    target[y * outputWidth + x] = GetValue(inputSpan, x, y);

                }
            }


            return result;
        }
        internal static float[] Resize(ReadOnlySpan<float> inputSpan, int inputWidth, int inputHeight, int outputWidth, int outputHeight)
        {
            float[] result = new float[outputWidth * outputHeight];

            Span<float> target = result;

            float GetValue(ReadOnlySpan<float> rs, int x, int y)
            {
                float rx = (float)inputWidth / outputWidth;
                float ry = (float)inputHeight / outputHeight;

                float xstart = x * rx;
                float xend = (x + 1) * rx;
                float ystart = y * ry;
                float yend = (y + 1) * ry;

                float sum = 0f;
                float addedPixels = 0f;

                for (int sy = (int)ystart; sy < Math.Ceiling(yend); sy++)
                {
                    float yrate;
                    if (ystart - sy > 0)
                    {
                        yrate = ystart - sy;
                    }
                    else if (yend - sy < 1f)
                    {
                        yrate = yend - sy;
                    }
                    else
                    {
                        yrate = 1f;
                    }
                    for (int sx = (int)xstart; sx < Math.Ceiling(xend); sx++)
                    {
                        float xrate;
                        if (xstart - sx > 0)
                        {
                            xrate = xstart - sx;
                        }
                        else if (xend - sx < 1f)
                        {
                            xrate = xend - sx;
                        }
                        else
                        {
                            xrate = 1f;
                        }
                        sum += rs[sy * inputWidth + sx] * yrate * xrate;
                        addedPixels += yrate * xrate;
                    }
                }

                return sum / addedPixels;

            }


            for (int y = 0; y < outputHeight; y++)
            {
                for (int x = 0; x < outputWidth; x++)
                {
                    target[y * outputWidth + x] = GetValue(inputSpan, x, y);

                }
            }


            return result;
        }
    }
}

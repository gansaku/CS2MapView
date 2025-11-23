using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CS2MapView.Exporter.Systems
{
    internal class SafeFileName
    {
        internal static readonly string[] InvalidFileNames = { "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
            "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };
        
        internal static string GetSafeFileName(string? cityName)
        {
            if (string.IsNullOrEmpty(cityName))
            {
                return "_";
            }
            foreach (var s in InvalidFileNames)
            {
                if (cityName.Equals(s, StringComparison.OrdinalIgnoreCase))
                {
                    return cityName + "_";
                }
            }
            var sb = new StringBuilder(cityName);

            foreach (var c in Path.GetInvalidFileNameChars())
            {
                sb.Replace(c, '_');
            }
            return sb.ToString();

        }
    }
}

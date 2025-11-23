using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using System.Xml.Serialization;

namespace CS2MapView.Exporter.Systems
{
    internal static class ZipDataWriter
    {
        internal static void WriteZipXmlEntry<T>(ZipArchive zip, string fileName, T data)
        {
            var entry = zip.CreateEntry(fileName);
            using var zipstream = entry.Open();
            XmlSerializer xs = new XmlSerializer(typeof(T));
            xs.Serialize(zipstream, data);
        }

        internal static unsafe void WriteZipBinaryEntry(ZipArchive zip, string fileName, void* buffer, int byteLength)
        {
            var entry = zip.CreateEntry(fileName);
            using var zipstream = entry.Open();
            zipstream.Write(new ReadOnlySpan<byte>(buffer, byteLength));
        } 
    }
}

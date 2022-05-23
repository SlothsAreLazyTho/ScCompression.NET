using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using SevenZip.Compression.LZMA;

namespace ScCompression.Core.Compressions
{
    public sealed class LzmaCompressor : ICompressor
    {
        
        public Stream Decompress(SupercellReader reader, int offset = 0)
        {
            reader.Seek(offset, SeekOrigin.Begin);
            
            var decoder = new Decoder();
            var lzmaProperties = reader.ReadBytes(5);
            var uncompressedSize = reader.Read<int>();
            var compressedBuffer = reader.ReadBytes(reader.Length - 9);
            var uncompressedStream = new MemoryStream(uncompressedSize);
            using var compressedStream = new MemoryStream(compressedBuffer);

            try
            {
                decoder.SetDecoderProperties(lzmaProperties);
                decoder.Code(compressedStream, uncompressedStream, compressedStream.Length, uncompressedSize, null);
            }
            catch(Exception e)
            {
                Console.WriteLine($"[ERROR] {e.Message}");
            }
            return uncompressedStream;
        }
        
    }
}
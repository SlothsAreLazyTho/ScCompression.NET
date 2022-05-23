using System.IO;
using SevenZip.Compression.LZMA;

namespace ScCompression.Core.Compressions
{
    public sealed class LzmaCompressor : ICompressor
    {
        
        public static byte[] Decompress(byte[] buffer)
        {
            var reader             = new SupercellReader(buffer);
            var decoder            = new Decoder();
            
            var lzmaProperties     = reader.ReadBytes(5);
            var uncompressedSize   = reader.Read<int>();
            var compressedBuffer   = reader.ReadBytes(reader.Length - 9);
            var uncompressedStream = new MemoryStream(uncompressedSize);
            var compressedStream   = new MemoryStream(compressedBuffer);
            decoder.SetDecoderProperties(lzmaProperties);
            decoder.Code(compressedStream, uncompressedStream, compressedStream.Length, uncompressedSize, null);
            return uncompressedStream.ToArray();
        }
        
    }
}
using System.IO;

namespace ScCompression.Core.Compressions
{
    public sealed class ScCompressor : ICompressor
    {

        public static byte[] Decompress(byte[] buffer)
        {
            var reader       = new SupercellReader(buffer);
            reader.Seek(2, SeekOrigin.Begin);

            var fileVersion = reader.Read<int>();

            if (fileVersion >= 4)
                fileVersion  = reader.Read<int>();

            var hashLength   = reader.ReadInt32BE();
            var hash         = reader.ReadBytes(hashLength);
            
            var possibleSignatureMatch = reader.ReadSignature();
            
            if (possibleSignatureMatch == CompressionType.SCLZ)
                return SclzCompressor.Decompress(buffer[34..buffer.Length]);
            
            var content = new MemoryStream(buffer[(10 + hashLength)..]);
            return content.ToArray();
        }

    }
}
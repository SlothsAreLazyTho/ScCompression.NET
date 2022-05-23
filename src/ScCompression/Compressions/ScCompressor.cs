using System.IO;

namespace ScCompression.Core.Compressions
{
    public sealed class ScCompressor : ICompressor
    {

        public Stream Decompress(SupercellReader reader, int offset = 2)
        {
            reader.Seek(2, SeekOrigin.Begin);

            var fileVersion = reader.ReadInt32BE();

            if (fileVersion >= 4)
                fileVersion = reader.ReadInt32BE();

            var hashLength = reader.ReadInt32BE();
            var hash = reader.ReadBytes(hashLength);
            var possibleSignatureMatch = reader.ReadSignature();
            
            if (possibleSignatureMatch == CompressionType.SCLZ)
                return new SclzCompressor().Decompress(reader, 0);

            var contentBuffer = reader.ReadBytes(reader.Length - reader.Position);
            return new MemoryStream(contentBuffer);
        }

    }
}
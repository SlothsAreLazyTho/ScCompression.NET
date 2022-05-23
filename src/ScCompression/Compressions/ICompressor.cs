using System.IO;

namespace ScCompression.Core.Compressions
{
    public interface ICompressor
    {
        
        Stream Decompress(SupercellReader reader, int offset = 0);

    }
}
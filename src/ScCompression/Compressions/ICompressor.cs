using System.Threading.Tasks;

namespace ScCompression.Core.Compressions
{
    public interface ICompressor
    {

        static byte[] Compress(byte[] buffer)
        {
            return new byte[0];
        }
        
        static byte[] Decompress(byte[] buffer)
        {
            return new byte[0];
        }

    }
}
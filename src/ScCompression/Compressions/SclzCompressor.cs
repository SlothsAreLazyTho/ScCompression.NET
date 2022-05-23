using System.Runtime.CompilerServices;
using LzhamWrapper;
using LzhamWrapper.Enums;

namespace ScCompression.Core.Compressions
{
    public sealed class SclzCompressor : ICompressor
    {
        
        public static byte[] Decompress(byte[] buffer)
        {
            var reader           = new SupercellReader(buffer);
            var dictionarySize   = reader.Read<byte>();
            var uncompressedSize = reader.Read<int>(); /* Lzham Compression is required to decompress this one */
            
            var decompressionParameters = new DecompressionParameters()
            {
                DictionarySize = dictionarySize,
                UpdateRate = TableUpdateRate.Default
            };
            
            var content      = reader.ReadBytes(reader.Length - 5);
            var outputBuffer = new byte[uncompressedSize];
            var adler32      = (uint) 0x0000000;
            
            Lzham.DecompressMemory(decompressionParameters, 
                content, content.Length, 0, 
                outputBuffer, ref uncompressedSize, 0, ref adler32);
            
            return outputBuffer;
        }
    }
}
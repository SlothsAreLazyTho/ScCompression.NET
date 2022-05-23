using System;
using System.IO;
using System.Runtime.CompilerServices;
using LzhamWrapper;
using LzhamWrapper.Enums;

namespace ScCompression.Core.Compressions
{
    public sealed class SclzCompressor : ICompressor
    {

        private uint _adler32 = 0x000000;
        
        
        public Stream Decompress(SupercellReader reader, int offset = 0)
        {
            reader.Seek(offset, SeekOrigin.Begin);
            
            var dictionarySize   = reader.Read<byte>();
            var uncompressedSize = reader.Read<int>(); /* Lzham Compression is required to decompress this one */
            
            var decompressionParameters = new DecompressionParameters()
            {
                DictionarySize = dictionarySize,
                UpdateRate = TableUpdateRate.Default
            };
            
            var content = reader.ReadBytes(reader.Length - reader.Position);
            var outputBuffer = new byte[uncompressedSize];
            
            Lzham.DecompressMemory(decompressionParameters,
                content, content.Length, 0,
                outputBuffer, ref uncompressedSize, 0, ref _adler32);

            return new MemoryStream(outputBuffer);
        }
    }
}
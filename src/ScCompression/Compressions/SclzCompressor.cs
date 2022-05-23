using System;
using System.IO;
using System.Runtime.CompilerServices;
using LzhamWrapper;
using LzhamWrapper.Enums;

namespace ScCompression.Core.Compressions
{
    public sealed class SclzCompressor : ICompressor
    {
        
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
            
            var content      = reader.ReadBytes(reader.Length - 5);
            var outputBuffer = new byte[uncompressedSize];
            var adler32      = (uint) 0x0000000;

            try
            {
                Lzham.DecompressMemory(decompressionParameters,
                    content, content.Length, 0,
                    outputBuffer, ref uncompressedSize, 0, ref adler32);
            }
            catch(Exception e)
            {
                Console.WriteLine($"[ERROR] {e.Message}");
            }

            return new MemoryStream(outputBuffer);
        }
    }
}
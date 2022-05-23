using System;
using System.IO;
using ScCompression.Core.Compressions;

namespace ScCompression.Core
{
    internal class CompressorFactory
    {

        public static ICompressor Create(SupercellReader reader, CompressionType type)
        {
            return type switch
            {
                CompressionType.LZMA => new LzmaCompressor(),
                CompressionType.SC   => new ScCompressor(),
                CompressionType.SCLZ => new SclzCompressor(),
                CompressionType.SIG  => new LzmaCompressor(),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

    }
}
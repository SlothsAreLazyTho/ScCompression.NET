using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using LzhamWrapper;
using LzhamWrapper.Enums;

using SevenZip.Compression.LZMA;

namespace ScCompression.Core
{
    public static class SupercellDecoder
    {
        
        /*
         * TODO Write Tests
         * TODO Work more with Cancellation Tokens.
         * TODO Add CompressionOptions
         * TODO Add Compress Ability
         */
        
        internal static readonly Decoder _decoder = new();
        
        internal static readonly Dictionary<uint, CompressionType> Signatures = new()
        {
            { 0x5D000004, CompressionType.LZMA },
            { 0x53430000, CompressionType.SC   },
            { 0x53434C5A, CompressionType.SCLZ },
            { 0x5369673A, CompressionType.SIG  },
        };
        
        /// <summary>
        /// Decompress a supercell file. Supports SC, LZMA, LZHAM.
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <param name="options">Compression Options</param>
        /// <returns>Compression Result</returns>
        /// <exception cref="FileNotFoundException">Occurs when file is not found</exception>
        public static async Task<CompressionResult> LoadAsync(string filePath)
        {
            Console.WriteLine($"Working on {filePath}");
            
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File does not exist.");

            var buffer = await File.ReadAllBytesAsync(filePath, default);
            
            var type = AutoDetect(buffer[..4]);

            var stream = type switch
            {
                CompressionType.LZMA => DecompressLzma(buffer),
                CompressionType.SC   => DecompressSc(buffer),
                CompressionType.SCLZ => DecompressSclz(buffer[4..buffer.Length]),
                CompressionType.SIG  => DecompressLzma(buffer[68..buffer.Length]),
                CompressionType.NONE => buffer
            };

            return new CompressionResult(filePath, stream, type);
        }

        public static byte[] DecompressLzma(byte[] buffer)
        {
            var uncompressedSize   = Unsafe.ReadUnaligned<int>(ref buffer[5]);
            var lzmaProperties     = buffer[..5];
            var uncompressedStream = new MemoryStream(uncompressedSize);
            var compressedStream   = new MemoryStream(buffer[9..]);
            _decoder.SetDecoderProperties(lzmaProperties);
            _decoder.Code(compressedStream, uncompressedStream, compressedStream.Length, uncompressedSize, null);
            return uncompressedStream.ToArray();
        }
        
        public static byte[] DecompressSc(byte[] buffer)
        {
            var fileVersion  = Unsafe.ReadUnaligned<int>(ref buffer[2]);

            if (fileVersion >= 4)
                fileVersion  = Unsafe.ReadUnaligned<int>(ref buffer[6]);

            var hashLengthBE = buffer[10..].Reverse().ToArray();
            var hashLength   = Unsafe.ReadUnaligned<int>(ref hashLengthBE[0]);
            var hash         = buffer[14..hashLength];
            
            var possibleSignatureMatch = AutoDetect(buffer[30..34]);
            
            if (possibleSignatureMatch == CompressionType.SCLZ)
                return DecompressSclz(buffer[34..buffer.Length]);
            
            var content = new MemoryStream(buffer[(10 + hashLength)..]);
            return content.ToArray();
        }
        
        public static byte[] DecompressSclz(byte[] buffer)
        {
            var dictionarySize   = Unsafe.ReadUnaligned<byte>(ref buffer[0]);
            var uncompressedSize = Unsafe.ReadUnaligned<int>(ref buffer[1]); /* Lzham Compression is required to decompress this one */
            
            var decompressionParameters = new DecompressionParameters()
            {
                DictionarySize = dictionarySize,
                UpdateRate = TableUpdateRate.Default
            };

            var content      = buffer[5..];
            var outputBuffer = new byte[uncompressedSize];
            var adler32      = (uint) 0x0000000;
            
            Lzham.DecompressMemory(decompressionParameters, 
                content, content.Length, 0, 
                outputBuffer, ref uncompressedSize, 0, ref adler32);
            
            return outputBuffer;
        }

        public static CompressionType AutoDetect(byte[] data)
        {
            var signatureBlock = data[..4].Reverse().ToArray();
            var unsignedInt = Unsafe.ReadUnaligned<uint>(ref signatureBlock[0]); /* Reader reads Little Endians and we want Big Endians */
            var result = Signatures.ContainsKey(unsignedInt) ? Signatures[unsignedInt] : CompressionType.NONE;
            return result;
        }
        
    }
    
}
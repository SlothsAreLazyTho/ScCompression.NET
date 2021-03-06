using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using LzhamWrapper;
using LzhamWrapper.Enums;
using ScCompression.Core.Compressions;
using SevenZip.Compression.LZMA;

namespace ScCompression.Core
{
    public class SupercellDecoder
    {
        
        /*
         * TODO Write Tests
         * TODO Add Compress Ability
         */
        
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
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File does not exist.");

            var buffer = await File.ReadAllBytesAsync(filePath, default);
            var reader = new SupercellReader(buffer);
            var type = reader.ReadSignature();
            
            if(type.Equals(CompressionType.NONE))
                return new CompressionResult(filePath, File.Open(filePath, FileMode.Open), type);
            
            var compressor = CompressorFactory.Create(reader, type);
            
            return new CompressionResult(filePath, compressor.Decompress(reader, type == CompressionType.SIG ? 68 : default), type);
        }
        
    }
    
}
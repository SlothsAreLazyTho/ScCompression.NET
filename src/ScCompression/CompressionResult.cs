﻿using System;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace ScCompression.Core
{
    public class CompressionResult
    {

        public CompressionResult(string path, byte[] buffer, CompressionType type = CompressionType.NONE)
        {
            Name =  path[(path.LastIndexOf(Path.DirectorySeparatorChar) + 1)..];
            Extension = Path.GetExtension(Name);
            FilePath = path;
            Content = buffer;
            Type = type;
            
#if DEBUG
            //Console.WriteLine($"[DEBUG] {Name}, Type: {Type}, Content Length: {Content.Length}");
#endif
        }

        /// <summary>
        /// Represents the file name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// File Extension
        /// </summary>
        public string Extension { get; set; }
        
        /// <summary>
        /// Absolute file path
        /// </summary>
        public string FilePath { get; set; }
        
        /// <summary>
        /// Content of the file
        /// </summary>
        [JsonIgnore]
        public byte[] Content { get; set; }
        
        /// <summary>
        /// Type the file is compressed with
        /// </summary>
        public CompressionType Type { get; set; }
        
        /// <summary>
        /// Read the buffer as string
        /// </summary>
        /// <returns>Content</returns>
        public string ReadAsString() => Encoding.UTF8.GetString(Content);

    }
}
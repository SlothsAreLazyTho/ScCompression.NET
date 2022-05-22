using System;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace ScCompression.Core
{
    public class CompressionResult
    {

        public CompressionResult(string path, Stream content, CompressionType type = CompressionType.NONE)
        {
            Name =  path[(path.LastIndexOf(Path.DirectorySeparatorChar) + 1)..];
            Extension = Path.GetExtension(Name);
            FilePath = path;
            Content = content;
            Type = type;

            if (type == CompressionType.SC)
            {
                Console.Write("");
            }

#if DEBUG
            Console.WriteLine($"[DEBUG] {Name}, Type: {Type}, Content Length: {Content.Length}");
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
        public Stream Content { get; set; }
        
        /// <summary>
        /// Type the file is compressed with
        /// </summary>
        public CompressionType Type { get; set; }


        /// <summary>
        /// Read Stream as Byte Array
        /// </summary>
        /// <returns>Buffer</returns>
        public async Task<byte[]> ReadAsByteArrayAsync()
        {
            var memory = new byte[Content.Length];
            var result = await Content.ReadAsync(memory, 0, memory.Length).ConfigureAwait(false);
            return memory;
        }
        
        public byte[] ReadAsByteArray()
        {
            var memory = new byte[Content.Length];
            var result = Content.Read(memory, 0, memory.Length);
            return memory;
        }
        
    }
}
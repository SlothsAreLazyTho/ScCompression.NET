using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using ScCompression.Core;

namespace ScCompression.Example
{
    public static class Program
    {
        
        public static async Task Main(string[] args)
        {
            Directory.CreateDirectory("Output");
            
            var stopwatch = Stopwatch.StartNew();
            var file = Path.Combine(Directory.GetCurrentDirectory(), "assets.csv");
            var decompressed = await SupercellDecoder.LoadAsync(file);
            await File.WriteAllBytesAsync("output.csv", await decompressed.ReadAsByteArrayAsync());
            
            stopwatch.Stop();
            Console.WriteLine($"Time Elapsed: {stopwatch.Elapsed}");
            
        }

    }
}
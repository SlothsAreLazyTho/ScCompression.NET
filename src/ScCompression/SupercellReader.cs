using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ScCompression.Core
{
    public class SupercellReader
    {

        private readonly byte[] _buffer;

        public int Position;
        public readonly int Length;
        
        public SupercellReader(byte[] buffer)
        {
            _buffer = buffer;
            Length = buffer.Length;
            Position = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Seek(int offset, SeekOrigin origin = SeekOrigin.Current)
        {
            return Position = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => offset + Position,
                SeekOrigin.End => _buffer.Length + offset,
                _ => throw new ArgumentOutOfRangeException(nameof(origin), origin, null)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Read<T>() where T : unmanaged
        {
            var size = Unsafe.SizeOf<T>();
            var bytes = ReadBytes(size);
            Seek(size, SeekOrigin.Current);
            return Unsafe.ReadUnaligned<T>(ref bytes[0]);
        }

        public int ReadInt32BE()
        {
            var buffer = ReadBytes(4).Reverse().ToArray();
            return Unsafe.ReadUnaligned<int>(ref buffer[0]);
        }

        public CompressionType ReadSignature()
        {
            var signatureBlock = ReadBytes(4).Reverse().ToArray();
            var unsignedInt = Unsafe.ReadUnaligned<uint>(ref signatureBlock[0]); /* Reader reads Little Endians and we want Big Endians */
            var result = SupercellDecoder.Signatures.ContainsKey(unsignedInt) ? SupercellDecoder.Signatures[unsignedInt] : CompressionType.NONE;
            Seek(Position - 4, SeekOrigin.Begin);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ReadBytes(int length)
        {
            byte[] buffer = new byte[length];
            Unsafe.CopyBlockUnaligned(ref buffer[0], ref _buffer[Position], (uint) length);
            Seek(length);
            return buffer;
        }
    }
}
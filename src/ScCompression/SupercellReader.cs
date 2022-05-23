using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ScCompression.Core
{
    public class SupercellReader : IDisposable
    {
        
        private readonly byte[] _buffer;

        private int _position;

        public int Position
        {
            get => _position;
            set => _position = value;
        }
        
        public readonly int Length;
        
        public SupercellReader(byte[] buffer, int offset = 0)
        {
            _buffer = buffer;
            Length = buffer.Length;
            Position = offset;
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
            return Unsafe.ReadUnaligned<T>(ref bytes[0]);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Read<T>(int offset, SeekOrigin origin = SeekOrigin.Current) where T : unmanaged
        {
            Seek(offset, origin);
            return Read<T>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt32BE()
        {
            var buffer = ReadBytes(4).Reverse().ToArray();
            return Unsafe.ReadUnaligned<int>(ref buffer[0]);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt32BE()
        {
            var buffer = ReadBytes(4).Reverse().ToArray();
            return Unsafe.ReadUnaligned<uint>(ref buffer[0]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CompressionType ReadSignature()
        {
            var unsignedInt = ReadUInt32BE();
            var result = SupercellDecoder.Signatures.ContainsKey(unsignedInt) ? SupercellDecoder.Signatures[unsignedInt] : CompressionType.NONE;
            Seek(Position - 4, SeekOrigin.Begin);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ReadBytes(int length)
        {
            byte[] buffer = new byte[length];
            Unsafe.CopyBlockUnaligned(ref buffer[0], ref _buffer[_position], (uint) length);
            Seek(length);
            return buffer;
        }
        
        public byte[] ToArray() => _buffer;
        
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        
    }
}
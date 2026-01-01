using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.CompilerServices;

namespace PbfLite;

/// <summary>
/// Reader for Protocol Buffers binary data from streams.
/// </summary>
public partial class PbfStreamReader
{
    private readonly Stream _stream;
    private bool _disposed;

    /// <summary>
    /// Creates a new <see cref="PbfStreamReader"/> instance for the provided stream.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <exception cref="ArgumentNullException">Thrown if stream is null.</exception>
    public PbfStreamReader(Stream stream)
    {
        _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        _disposed = false;
    }

    /// <summary>
    /// Reads the next field header from the stream and returns the field number
    /// and wire type. If the end of the stream is reached, the returned field
    /// number will be zero and the wire type will be <see cref="WireType.None"/>.
    /// </summary>
    /// <returns>Tuple of (fieldNumber, wireType).</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (int fieldNumber, WireType wireType) ReadFieldHeader()
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(PbfStreamReader));

        if (!TryReadVarInt32(out uint header))
        {
            return (0, WireType.None);
        }

        if (header != 0)
        {
            return ((int)(header >> 3), (WireType)(header & 7));
        }
        else
        {
            return (0, WireType.None);
        }
    }

    /// <summary>
    /// Skips a field with the specified wire type.
    /// </summary>
    /// <param name="wireType">The wire type of the field to skip.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SkipField(WireType wireType)
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(PbfStreamReader));

        switch (wireType)
        {
            case WireType.VarInt:
                ReadVarInt64();
                break;
            case WireType.Fixed32:
                ReadFixed32();
                break;
            case WireType.Fixed64:
                ReadFixed64();
                break;
            case WireType.String:
                var length = ReadVarInt64();
                SkipBytes((int)length);
                break;
            default:
                throw new ArgumentException($"Unable to skip field. '{wireType}' is unknown WireType");
        }
    }

    /// <summary>
    /// Reads exactly 4 bytes as a little-endian 32-bit unsigned integer.
    /// </summary>
    /// <returns>The 32-bit unsigned integer read.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadFixed32()
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(PbfStreamReader));

        Span<byte> buffer = stackalloc byte[4];
        ReadExactBytes(buffer);
        return BinaryPrimitives.ReadUInt32LittleEndian(buffer);
    }

    /// <summary>
    /// Reads exactly 8 bytes as a little-endian 64-bit unsigned integer.
    /// </summary>
    /// <returns>The 64-bit unsigned integer read.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadFixed64()
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(PbfStreamReader));

        Span<byte> buffer = stackalloc byte[8];
        ReadExactBytes(buffer);
        return BinaryPrimitives.ReadUInt64LittleEndian(buffer);
    }

    /// <summary>
    /// Reads a 32-bit unsigned integer encoded as a base-128 varint.
    /// </summary>
    /// <returns>The decoded 32-bit unsigned integer.</returns>
    public uint ReadVarInt32()
    {
        if (!TryReadVarInt32(out uint value))
        {
            throw new EndOfStreamException("Unexpected end of stream while reading varint32");
        }
        return value;
    }

    /// <summary>
    /// Reads a 64-bit unsigned integer encoded as a base-128 varint.
    /// </summary>
    /// <returns>The decoded 64-bit unsigned integer.</returns>
    public ulong ReadVarInt64()
    {
        if (!TryReadVarInt64(out ulong value))
        {
            throw new EndOfStreamException("Unexpected end of stream while reading varint64");
        }
        return value;
    }

    /// <summary>
    /// Reads a length-prefixed sequence of bytes and returns it as a byte array.
    /// </summary>
    /// <returns>A byte array containing the length-prefixed data.</returns>
    public byte[] ReadLengthPrefixedBytes()
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(PbfStreamReader));

        var length = ReadVarInt32();
        byte[] buffer = new byte[length];
        ReadExactBytes(buffer);
        return buffer;
    }

    /// <summary>
    /// Tries to read a 32-bit unsigned integer encoded as a base-128 varint.
    /// </summary>
    /// <param name="value">The decoded value, or 0 if end of stream reached.</param>
    /// <returns>True if a complete varint was read; false if end of stream reached before completing the varint.</returns>
    private bool TryReadVarInt32(out uint value)
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(PbfStreamReader));

        int byteValue = _stream.ReadByte();
        if (byteValue < 0)
        {
            value = 0;
            return false;
        }

        value = (uint)byteValue;
        if ((value & 0x80) == 0)
        {
            return true;
        }

        value &= 0x7F;
        
        uint chunk = (uint)ReadByteOrThrow();
        value |= (chunk & 0x7F) << 7;
        if ((chunk & 0x80) == 0)
        {
            return true;
        }
       
        chunk = (uint)ReadByteOrThrow();
        value |= (chunk & 0x7F) << 14;
        if ((chunk & 0x80) == 0)
        {
            return true;
        }

        chunk = (uint)ReadByteOrThrow();
        value |= (chunk & 0x7F) << 21;
        if ((chunk & 0x80) == 0)
        {
            return true;
        }

        chunk = (uint)ReadByteOrThrow();
        value |= chunk << 28; // can only use 4 bits from this chunk
        if ((chunk & 0xF0) == 0)
        {
            return true;
        }

        if ((chunk & 0xF0) == 0xF0 &&
            ReadByteOrThrow() == 0xFF &&
            ReadByteOrThrow() == 0xFF &&
            ReadByteOrThrow() == 0xFF &&
            ReadByteOrThrow() == 0xFF &&
            ReadByteOrThrow() == 0x01)
        {
            return true;
        }

        throw new InvalidOperationException("Malformed varint");
    }

    /// <summary>
    /// Tries to read a 64-bit unsigned integer encoded as a base-128 varint.
    /// </summary>
    /// <param name="value">The decoded value, or 0 if end of stream reached.</param>
    /// <returns>True if a complete varint was read; false if end of stream reached before completing the varint.</returns>
    private bool TryReadVarInt64(out ulong value)
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(PbfStreamReader));

        int byteValue = _stream.ReadByte();
        if (byteValue < 0)
        {
            value = 0;
            return false;
        }

        value = (ulong)byteValue;
        if ((value & 0x80) == 0)
        {
            return true;
        }

        value &= 0x7F;

        byteValue = _stream.ReadByte();
        if (byteValue < 0) throw new EndOfStreamException("Unexpected end of stream while reading varint64");
        
        ulong chunk = (ulong)byteValue;
        value |= (chunk & 0x7F) << 7;
        if ((chunk & 0x80) == 0)
            return true;

        byteValue = _stream.ReadByte();
        if (byteValue < 0) throw new EndOfStreamException("Unexpected end of stream while reading varint64");
        
        chunk = (ulong)byteValue;
        value |= (chunk & 0x7F) << 14;
        if ((chunk & 0x80) == 0)
            return true;

        byteValue = _stream.ReadByte();
        if (byteValue < 0) throw new EndOfStreamException("Unexpected end of stream while reading varint64");
        
        chunk = (ulong)byteValue;
        value |= (chunk & 0x7F) << 21;
        if ((chunk & 0x80) == 0)
            return true;

        byteValue = _stream.ReadByte();
        if (byteValue < 0) throw new EndOfStreamException("Unexpected end of stream while reading varint64");
        
        chunk = (ulong)byteValue;
        value |= (chunk & 0x7F) << 28;
        if ((chunk & 0x80) == 0)
            return true;

        byteValue = _stream.ReadByte();
        if (byteValue < 0) throw new EndOfStreamException("Unexpected end of stream while reading varint64");
        
        chunk = (ulong)byteValue;
        value |= (chunk & 0x7F) << 35;
        if ((chunk & 0x80) == 0)
            return true;

        byteValue = _stream.ReadByte();
        if (byteValue < 0) throw new EndOfStreamException("Unexpected end of stream while reading varint64");
        
        chunk = (ulong)byteValue;
        value |= (chunk & 0x7F) << 42;
        if ((chunk & 0x80) == 0)
            return true;

        byteValue = _stream.ReadByte();
        if (byteValue < 0) throw new EndOfStreamException("Unexpected end of stream while reading varint64");
        
        chunk = (ulong)byteValue;
        value |= (chunk & 0x7F) << 49;
        if ((chunk & 0x80) == 0)
            return true;

        byteValue = _stream.ReadByte();
        if (byteValue < 0) throw new EndOfStreamException("Unexpected end of stream while reading varint64");
        
        chunk = (ulong)byteValue;
        value |= (chunk & 0x7F) << 56;
        if ((chunk & 0x80) == 0)
            return true;

        byteValue = _stream.ReadByte();
        if (byteValue < 0) throw new EndOfStreamException("Unexpected end of stream while reading varint64");
        
        chunk = (ulong)byteValue;
        value |= chunk << 63;

        if ((chunk & ~(ulong)0x01) != 0)
        {
            throw new InvalidOperationException("Malformed varint");
        }

        return true;
    }

    /// <summary>
    /// Reads a single byte from the stream, throwing if end is reached.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte ReadByteOrThrow()
    {
        int byteValue = _stream.ReadByte();
        if (byteValue < 0)
        {
            throw new EndOfStreamException("Unexpected end of stream");
        }

        return (byte)byteValue;
    }

    /// <summary>
    /// Reads exactly the specified number of bytes into a span.
    /// </summary>
    private void ReadExactBytes(Span<byte> buffer)
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(PbfStreamReader));

        int totalRead = 0;
        while (totalRead < buffer.Length)
        {
            int read = _stream.Read(buffer.Slice(totalRead));
            if (read == 0)
                throw new EndOfStreamException($"Unexpected end of stream. Expected {buffer.Length} bytes, but got {totalRead}");
            totalRead += read;
        }
    }

    /// <summary>
    /// Skips the specified number of bytes from the stream.
    /// </summary>
    private void SkipBytes(int count)
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(PbfStreamReader));

        if (_stream.CanSeek)
        {
            long newPosition = _stream.Position + count;
            if (newPosition > _stream.Length)
            {
                throw new EndOfStreamException($"Unexpected end of stream while skipping {count} bytes");
            }

            _stream.Seek(count, SeekOrigin.Current);
            return;
        }

        Span<byte> buffer = stackalloc byte[Math.Min(4096, count)];
        int remaining = count;

        while (remaining > 0)
        {
            int toRead = Math.Min(buffer.Length, remaining);
            int read = _stream.Read(buffer.Slice(0, toRead));
            if (read == 0)
            {
                throw new EndOfStreamException($"Unexpected end of stream while skipping {count} bytes");
            }
            remaining -= read;
        }
    }

    /// <summary>
    /// Closes the stream and releases resources.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _stream?.Dispose();
            _disposed = true;
        }
    }
}

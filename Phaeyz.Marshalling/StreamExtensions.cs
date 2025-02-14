namespace Phaeyz.Marshalling;

/// <summary>
/// Utility methods for reading and writing intrinsic values from a stream.
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    /// Reads a signed 8-bit value from the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to read from.
    /// </param>
    /// <returns>
    /// A signed 8-bit value read from the stream.
    /// </returns>
    /// <remarks>
    /// This is not named <c>ReadSByte</c> to avoid confusion with the existing <c>ReadByte</c> method
    /// contract on <see cref="System.IO.Stream"/>.
    /// </remarks>
    /// <exception cref="System.IO.EndOfStreamException">
    /// The end of the stream is reached before reading the required number of bytes.
    /// </exception>
    public static sbyte ReadInt8(this Stream @this)
    {
        ArgumentNullException.ThrowIfNull(@this);
        Span<byte> buffer = stackalloc byte[sizeof(sbyte)];
        @this.ReadExactly(buffer);
        return ByteConverter.SystemEndian.ToSByte(buffer);
    }

    /// <summary>
    /// Reads a signed 8-bit value from the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to read from.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the request.
    /// </param>
    /// <returns>
    /// A signed 8-bit value read from the stream.
    /// </returns>
    /// <remarks>
    /// This is not named <c>ReadSByteAsync</c> to avoid confusion with the existing <c>ReadByte</c> method
    /// contract on <see cref="System.IO.Stream"/>.
    /// </remarks>
    /// <exception cref="System.IO.EndOfStreamException">
    /// The end of the stream is reached before reading the required number of bytes.
    /// </exception>
    public static async Task<sbyte> ReadInt8Async(this Stream @this, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@this);
        byte[] buffer = ReusableNumericBuffer.Get();
        try
        {
            await @this.ReadExactlyAsync(buffer, 0, 1, cancellationToken).ConfigureAwait(false);
            return ByteConverter.SystemEndian.ToSByte(buffer);
        }
        finally
        {
            ReusableNumericBuffer.Release(buffer);
        }
    }

    /// <summary>
    /// Reads an unsigned 8-bit value from the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to read from.
    /// </param>
    /// <returns>
    /// An unsigned 8-bit value read from the stream.
    /// </returns>
    /// <remarks>
    /// This is not named <c>ReadByte</c> to avoid confusion with the existing <c>ReadByte</c> method
    /// contract on <see cref="System.IO.Stream"/>.
    /// </remarks>
    /// <exception cref="System.IO.EndOfStreamException">
    /// The end of the stream is reached before reading the required number of bytes.
    /// </exception>
    public static byte ReadUInt8(this Stream @this)
    {
        ArgumentNullException.ThrowIfNull(@this);
        Span<byte> buffer = stackalloc byte[sizeof(byte)];
        @this.ReadExactly(buffer);
        return ByteConverter.SystemEndian.ToByte(buffer);
    }

    /// <summary>
    /// Reads an unsigned 8-bit value from the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to read from.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the request.
    /// </param>
    /// <returns>
    /// An unsigned 8-bit value read from the stream.
    /// </returns>
    /// <remarks>
    /// This is not named <c>ReadByteAsync</c> to avoid confusion with the existing <c>ReadByte</c> method
    /// contract on <see cref="System.IO.Stream"/>.
    /// </remarks>
    /// <exception cref="System.IO.EndOfStreamException">
    /// The end of the stream is reached before reading the required number of bytes.
    /// </exception>
    public static async Task<byte> ReadUInt8Async(this Stream @this, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@this);
        byte[] buffer = ReusableNumericBuffer.Get();
        try
        {
            await @this.ReadExactlyAsync(buffer, 0, 1, cancellationToken).ConfigureAwait(false);
            return ByteConverter.SystemEndian.ToByte(buffer);
        }
        finally
        {
            ReusableNumericBuffer.Release(buffer);
        }
    }

    /// <summary>
    /// Reads a signed 16-bit value from the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to read from.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when reading the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    /// <returns>
    /// A signed 16-bit value read from the stream.
    /// </returns>
    /// <exception cref="System.IO.EndOfStreamException">
    /// The end of the stream is reached before reading the required number of bytes.
    /// </exception>
    public static short ReadInt16(this Stream @this, ByteConverter? byteConverter = null)
    {
        ArgumentNullException.ThrowIfNull(@this);
        Span<byte> buffer = stackalloc byte[sizeof(short)];
        @this.ReadExactly(buffer);
        return (byteConverter ?? ByteConverter.SystemEndian).ToInt16(buffer);
    }

    /// <summary>
    /// Reads a signed 16-bit value from the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to read from.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when reading the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the request.
    /// </param>
    /// <returns>
    /// A signed 16-bit value read from the stream.
    /// </returns>
    /// <exception cref="System.IO.EndOfStreamException">
    /// The end of the stream is reached before reading the required number of bytes.
    /// </exception>
    public static async Task<short> ReadInt16Async(this Stream @this, ByteConverter? byteConverter = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@this);
        byte[] buffer = ReusableNumericBuffer.Get();
        try
        {
            await @this.ReadExactlyAsync(buffer, 0, 2, cancellationToken).ConfigureAwait(false);
            return (byteConverter ?? ByteConverter.SystemEndian).ToInt16(buffer);
        }
        finally
        {
            ReusableNumericBuffer.Release(buffer);
        }
    }

    /// <summary>
    /// Reads an unsigned 16-bit value from the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to read from.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when reading the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    /// <returns>
    /// An unsigned 16-bit value read from the stream.
    /// </returns>
    /// <exception cref="System.IO.EndOfStreamException">
    /// The end of the stream is reached before reading the required number of bytes.
    /// </exception>
    public static ushort ReadUInt16(this Stream @this, ByteConverter? byteConverter = null)
    {
        ArgumentNullException.ThrowIfNull(@this);
        Span<byte> buffer = stackalloc byte[sizeof(ushort)];
        @this.ReadExactly(buffer);
        return (byteConverter ?? ByteConverter.SystemEndian).ToUInt16(buffer);
    }

    /// <summary>
    /// Reads an unsigned 16-bit value from the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to read from.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when reading the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the request.
    /// </param>
    /// <returns>
    /// An unsigned 16-bit value read from the stream.
    /// </returns>
    /// <exception cref="System.IO.EndOfStreamException">
    /// The end of the stream is reached before reading the required number of bytes.
    /// </exception>
    public static async Task<ushort> ReadUInt16Async(this Stream @this, ByteConverter? byteConverter = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@this);
        byte[] buffer = ReusableNumericBuffer.Get();
        try
        {
            await @this.ReadExactlyAsync(buffer, 0, 2, cancellationToken).ConfigureAwait(false);
            return (byteConverter ?? ByteConverter.SystemEndian).ToUInt16(buffer);
        }
        finally
        {
            ReusableNumericBuffer.Release(buffer);
        }
    }

    /// <summary>
    /// Reads an signed 32-bit value from the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to read from.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when reading the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    /// <returns>
    /// A signed 32-bit value read from the stream.
    /// </returns>
    /// <exception cref="System.IO.EndOfStreamException">
    /// The end of the stream is reached before reading the required number of bytes.
    /// </exception>
    public static int ReadInt32(this Stream @this, ByteConverter? byteConverter = null)
    {
        ArgumentNullException.ThrowIfNull(@this);
        Span<byte> buffer = stackalloc byte[sizeof(int)];
        @this.ReadExactly(buffer);
        return (byteConverter ?? ByteConverter.SystemEndian).ToInt32(buffer);
    }

    /// <summary>
    /// Reads an signed 32-bit value from the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to read from.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when reading the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the request.
    /// </param>
    /// <returns>
    /// A signed 32-bit value read from the stream.
    /// </returns>
    /// <exception cref="System.IO.EndOfStreamException">
    /// The end of the stream is reached before reading the required number of bytes.
    /// </exception>
    public static async Task<int> ReadInt32Async(this Stream @this, ByteConverter? byteConverter = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@this);
        byte[] buffer = ReusableNumericBuffer.Get();
        try
        {
            await @this.ReadExactlyAsync(buffer, 0, 4, cancellationToken).ConfigureAwait(false);
            return (byteConverter ?? ByteConverter.SystemEndian).ToInt32(buffer);
        }
        finally
        {
            ReusableNumericBuffer.Release(buffer);
        }
    }

    /// <summary>
    /// Reads an unsigned 32-bit value from the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to read from.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when reading the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    /// <returns>
    /// An unsigned 32-bit value read from the stream.
    /// </returns>
    /// <exception cref="System.IO.EndOfStreamException">
    /// The end of the stream is reached before reading the required number of bytes.
    /// </exception>
    public static uint ReadUInt32(this Stream @this, ByteConverter? byteConverter = null)
    {
        ArgumentNullException.ThrowIfNull(@this);
        Span<byte> buffer = stackalloc byte[sizeof(uint)];
        @this.ReadExactly(buffer);
        return (byteConverter ?? ByteConverter.SystemEndian).ToUInt32(buffer);
    }

    /// <summary>
    /// Reads an unsigned 32-bit value from the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to read from.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when reading the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the request.
    /// </param>
    /// <returns>
    /// An unsigned 32-bit value read from the stream.
    /// </returns>
    /// <exception cref="System.IO.EndOfStreamException">
    /// The end of the stream is reached before reading the required number of bytes.
    /// </exception>
    public static async Task<uint> ReadUInt32Async(this Stream @this, ByteConverter? byteConverter = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@this);
        byte[] buffer = ReusableNumericBuffer.Get();
        try
        {
            await @this.ReadExactlyAsync(buffer, 0, 4, cancellationToken).ConfigureAwait(false);
            return (byteConverter ?? ByteConverter.SystemEndian).ToUInt32(buffer);
        }
        finally
        {
            ReusableNumericBuffer.Release(buffer);
        }
    }

    /// <summary>
    /// Reads a signed 64-bit value from the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to read from.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when reading the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    /// <returns>
    /// A signed 64-bit value read from the stream.
    /// </returns>
    /// <exception cref="System.IO.EndOfStreamException">
    /// The end of the stream is reached before reading the required number of bytes.
    /// </exception>
    public static long ReadInt64(this Stream @this, ByteConverter? byteConverter = null)
    {
        ArgumentNullException.ThrowIfNull(@this);
        Span<byte> buffer = stackalloc byte[sizeof(long)];
        @this.ReadExactly(buffer);
        return (byteConverter ?? ByteConverter.SystemEndian).ToInt64(buffer);
    }

    /// <summary>
    /// Reads a signed 64-bit value from the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to read from.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when reading the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the request.
    /// </param>
    /// <returns>
    /// A signed 64-bit value read from the stream.
    /// </returns>
    /// <exception cref="System.IO.EndOfStreamException">
    /// The end of the stream is reached before reading the required number of bytes.
    /// </exception>
    public static async Task<long> ReadInt64Async(this Stream @this, ByteConverter? byteConverter = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@this);
        byte[] buffer = ReusableNumericBuffer.Get();
        try
        {
            await @this.ReadExactlyAsync(buffer, 0, 8, cancellationToken).ConfigureAwait(false);
            return (byteConverter ?? ByteConverter.SystemEndian).ToInt64(buffer);
        }
        finally
        {
            ReusableNumericBuffer.Release(buffer);
        }
    }

    /// <summary>
    /// Reads an unsigned 64-bit value from the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to read from.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when reading the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    /// <returns>
    /// An unsigned 64-bit value read from the stream.
    /// </returns>
    /// <exception cref="System.IO.EndOfStreamException">
    /// The end of the stream is reached before reading the required number of bytes.
    /// </exception>
    public static ulong ReadUInt64(this Stream @this, ByteConverter? byteConverter = null)
    {
        ArgumentNullException.ThrowIfNull(@this);
        Span<byte> buffer = stackalloc byte[sizeof(ulong)];
        @this.ReadExactly(buffer);
        return (byteConverter ?? ByteConverter.SystemEndian).ToUInt64(buffer);
    }

    /// <summary>
    /// Reads an unsigned 64-bit value from the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to read from.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when reading the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the request.
    /// </param>
    /// <returns>
    /// An unsigned 64-bit value read from the stream.
    /// </returns>
    /// <exception cref="System.IO.EndOfStreamException">
    /// The end of the stream is reached before reading the required number of bytes.
    /// </exception>
    public static async Task<ulong> ReadUInt64Async(this Stream @this, ByteConverter? byteConverter = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@this);
        byte[] buffer = ReusableNumericBuffer.Get();
        try
        {
            await @this.ReadExactlyAsync(buffer, 0, 8, cancellationToken).ConfigureAwait(false);
            return (byteConverter ?? ByteConverter.SystemEndian).ToUInt64(buffer);
        }
        finally
        {
            ReusableNumericBuffer.Release(buffer);
        }
    }

    /// <summary>
    /// Reads a 32-bit floating-point value from the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to read from.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when reading the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    /// <returns>
    /// A 32-bit floating-point value read from the stream.
    /// </returns>
    /// <exception cref="System.IO.EndOfStreamException">
    /// The end of the stream is reached before reading the required number of bytes.
    /// </exception>
    public static float ReadSingle(this Stream @this, ByteConverter? byteConverter = null)
    {
        ArgumentNullException.ThrowIfNull(@this);
        Span<byte> buffer = stackalloc byte[sizeof(float)];
        @this.ReadExactly(buffer);
        return (byteConverter ?? ByteConverter.SystemEndian).ToSingle(buffer);
    }

    /// <summary>
    /// Reads a 32-bit floating-point value from the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to read from.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when reading the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the request.
    /// </param>
    /// <returns>
    /// A 32-bit floating-point value read from the stream.
    /// </returns>
    /// <exception cref="System.IO.EndOfStreamException">
    /// The end of the stream is reached before reading the required number of bytes.
    /// </exception>
    public static async Task<float> ReadSingleAsync(this Stream @this, ByteConverter? byteConverter = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@this);
        byte[] buffer = ReusableNumericBuffer.Get();
        try
        {
            await @this.ReadExactlyAsync(buffer, 0, 4, cancellationToken).ConfigureAwait(false);
            return (byteConverter ?? ByteConverter.SystemEndian).ToSingle(buffer);
        }
        finally
        {
            ReusableNumericBuffer.Release(buffer);
        }
    }

    /// <summary>
    /// Reads a 64-bit floating-point value from the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to read from.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when reading the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    /// <returns>
    /// A 64-bit floating-point value read from the stream.
    /// </returns>
    /// <exception cref="System.IO.EndOfStreamException">
    /// The end of the stream is reached before reading the required number of bytes.
    /// </exception>
    public static double ReadDouble(this Stream @this, ByteConverter? byteConverter = null)
    {
        ArgumentNullException.ThrowIfNull(@this);
        Span<byte> buffer = stackalloc byte[sizeof(double)];
        @this.ReadExactly(buffer);
        return (byteConverter ?? ByteConverter.SystemEndian).ToDouble(buffer);
    }

    /// <summary>
    /// Reads a 64-bit floating-point value from the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to read from.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when reading the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the request.
    /// </param>
    /// <returns>
    /// A 64-bit floating-point value read from the stream.
    /// </returns>
    /// <exception cref="System.IO.EndOfStreamException">
    /// The end of the stream is reached before reading the required number of bytes.
    /// </exception>
    public static async Task<double> ReadDoubleAsync(this Stream @this, ByteConverter? byteConverter = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@this);
        byte[] buffer = ReusableNumericBuffer.Get();
        try
        {
            await @this.ReadExactlyAsync(buffer, 0, 8, cancellationToken).ConfigureAwait(false);
            return (byteConverter ?? ByteConverter.SystemEndian).ToDouble(buffer);
        }
        finally
        {
            ReusableNumericBuffer.Release(buffer);
        }
    }

    /// <summary>
    /// Writes a signed 8-bit value to the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to write to.
    /// </param>
    /// <param name="value">
    /// The value to write.
    /// </param>
    /// <remarks>
    /// This is not named <c>WriteSByte</c> to avoid confusion with the existing <c>WriteByte</c> method
    /// contract on <see cref="System.IO.Stream"/>.
    /// </remarks>
    public static void WriteInt8(this Stream @this, sbyte value)
    {
        ArgumentNullException.ThrowIfNull(@this);
        Span<byte> buffer = [(byte)value];
        @this.Write(buffer);
    }

    /// <summary>
    /// Writes a signed 8-bit value to the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to write to.
    /// </param>
    /// <param name="value">
    /// The value to write.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the request.
    /// </param>
    /// <remarks>
    /// This is not named <c>WriteSByteAsync</c> to avoid confusion with the existing <c>WriteByte</c> method
    /// contract on <see cref="System.IO.Stream"/>.
    /// </remarks>
    public static async Task WriteInt8Async(this Stream @this, sbyte value, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@this);
        byte[] buffer = ReusableNumericBuffer.Get();
        try
        {
            buffer[0] = (byte)value;
            await @this.WriteAsync(buffer.AsMemory(0, 1), cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            ReusableNumericBuffer.Release(buffer);
        }
    }

    /// <summary>
    /// Writes an unsigned 8-bit value to the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to write to.
    /// </param>
    /// <param name="value">
    /// The value to write.
    /// </param>
    /// <remarks>
    /// This is not named <c>WriteByte</c> to avoid confusion with the existing <c>WriteByte</c> method
    /// contract on <see cref="System.IO.Stream"/>.
    /// </remarks>
    public static void WriteUInt8(this Stream @this, byte value)
    {
        ArgumentNullException.ThrowIfNull(@this);
        Span<byte> buffer = [value];
        @this.Write(buffer);
    }

    /// <summary>
    /// Writes an unsigned 8-bit value to the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to write to.
    /// </param>
    /// <param name="value">
    /// The value to write.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the request.
    /// </param>
    /// <remarks>
    /// This is not named <c>WriteByteAsync</c> to avoid confusion with the existing <c>WriteByte</c> method
    /// contract on <see cref="System.IO.Stream"/>.
    /// </remarks>
    public static async Task WriteUInt8Async(this Stream @this, byte value, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@this);
        byte[] buffer = ReusableNumericBuffer.Get();
        try
        {
            buffer[0] = value;
            await @this.WriteAsync(buffer.AsMemory(0, 1), cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            ReusableNumericBuffer.Release(buffer);
        }
    }

    /// <summary>
    /// Writes a signed 16-bit value to the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to write to.
    /// </param>
    /// <param name="value">
    /// The value to write.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when writing the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    public static void WriteInt16(this Stream @this, short value, ByteConverter? byteConverter = null)
    {
        ArgumentNullException.ThrowIfNull(@this);
        Span<byte> buffer = stackalloc byte[sizeof(short)];
        (byteConverter ?? ByteConverter.SystemEndian).FromInt16(value, buffer);
        @this.Write(buffer);
    }

    /// <summary>
    /// Writes a signed 16-bit value to the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to write to.
    /// </param>
    /// <param name="value">
    /// The value to write.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when writing the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the request.
    /// </param>
    public static async Task WriteInt16Async(this Stream @this, short value, ByteConverter? byteConverter = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@this);
        byte[] buffer = ReusableNumericBuffer.Get();
        try
        {
            (byteConverter ?? ByteConverter.SystemEndian).FromInt16(value, buffer);
            await @this.WriteAsync(buffer.AsMemory(0, 2), cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            ReusableNumericBuffer.Release(buffer);
        }
    }

    /// <summary>
    /// Writes an unsigned 16-bit value to the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to write to.
    /// </param>
    /// <param name="value">
    /// The value to write.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when writing the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    public static void WriteUInt16(this Stream @this, ushort value, ByteConverter? byteConverter = null)
    {
        ArgumentNullException.ThrowIfNull(@this);
        Span<byte> buffer = stackalloc byte[sizeof(ushort)];
        (byteConverter ?? ByteConverter.SystemEndian).FromUInt16(value, buffer);
        @this.Write(buffer);
    }

    /// <summary>
    /// Writes an unsigned 16-bit value to the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to write to.
    /// </param>
    /// <param name="value">
    /// The value to write.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when writing the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the request.
    /// </param>
    public static async Task WriteUInt16Async(this Stream @this, ushort value, ByteConverter? byteConverter = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@this);
        byte[] buffer = ReusableNumericBuffer.Get();
        try
        {
            (byteConverter ?? ByteConverter.SystemEndian).FromUInt16(value, buffer);
            await @this.WriteAsync(buffer.AsMemory(0, 2), cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            ReusableNumericBuffer.Release(buffer);
        }
    }

    /// <summary>
    /// Writes a signed 32-bit value to the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to write to.
    /// </param>
    /// <param name="value">
    /// The value to write.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when writing the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    public static void WriteInt32(this Stream @this, int value, ByteConverter? byteConverter = null)
    {
        ArgumentNullException.ThrowIfNull(@this);
        Span<byte> buffer = stackalloc byte[sizeof(int)];
        (byteConverter ?? ByteConverter.SystemEndian).FromInt32(value, buffer);
        @this.Write(buffer);
    }

    /// <summary>
    /// Writes a signed 32-bit value to the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to write to.
    /// </param>
    /// <param name="value">
    /// The value to write.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when writing the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the request.
    /// </param>
    public static async Task WriteInt32Async(this Stream @this, int value, ByteConverter? byteConverter = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@this);
        byte[] buffer = ReusableNumericBuffer.Get();
        try
        {
            (byteConverter ?? ByteConverter.SystemEndian).FromInt32(value, buffer);
            await @this.WriteAsync(buffer.AsMemory(0, 4), cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            ReusableNumericBuffer.Release(buffer);
        }
    }

    /// <summary>
    /// Writes an unsigned 32-bit value to the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to write to.
    /// </param>
    /// <param name="value">
    /// The value to write.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when writing the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    public static void WriteUInt32(this Stream @this, uint value, ByteConverter? byteConverter = null)
    {
        ArgumentNullException.ThrowIfNull(@this);
        Span<byte> buffer = stackalloc byte[sizeof(uint)];
        (byteConverter ?? ByteConverter.SystemEndian).FromUInt32(value, buffer);
        @this.Write(buffer);
    }

    /// <summary>
    /// Writes an unsigned 32-bit value to the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to write to.
    /// </param>
    /// <param name="value">
    /// The value to write.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when writing the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the request.
    /// </param>
    public static async Task WriteUInt32Async(this Stream @this, uint value, ByteConverter? byteConverter = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@this);
        byte[] buffer = ReusableNumericBuffer.Get();
        try
        {
            (byteConverter ?? ByteConverter.SystemEndian).FromUInt32(value, buffer);
            await @this.WriteAsync(buffer.AsMemory(0, 4), cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            ReusableNumericBuffer.Release(buffer);
        }
    }

    /// <summary>
    /// Writes a signed 64-bit value to the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to write to.
    /// </param>
    /// <param name="value">
    /// The value to write.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when writing the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    public static void WriteInt64(this Stream @this, long value, ByteConverter? byteConverter = null)
    {
        ArgumentNullException.ThrowIfNull(@this);
        Span<byte> buffer = stackalloc byte[sizeof(long)];
        (byteConverter ?? ByteConverter.SystemEndian).FromInt64(value, buffer);
        @this.Write(buffer);
    }

    /// <summary>
    /// Writes a signed 64-bit value to the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to write to.
    /// </param>
    /// <param name="value">
    /// The value to write.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when writing the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the request.
    /// </param>
    public static async Task WriteInt64Async(this Stream @this, long value, ByteConverter? byteConverter = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@this);
        byte[] buffer = ReusableNumericBuffer.Get();
        try
        {
            (byteConverter ?? ByteConverter.SystemEndian).FromInt64(value, buffer);
            await @this.WriteAsync(buffer.AsMemory(0, 8), cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            ReusableNumericBuffer.Release(buffer);
        }
    }

    /// <summary>
    /// Writes an unsigned 64-bit value to the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to write to.
    /// </param>
    /// <param name="value">
    /// The value to write.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when writing the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    public static void WriteUInt64(this Stream @this, ulong value, ByteConverter? byteConverter = null)
    {
        ArgumentNullException.ThrowIfNull(@this);
        Span<byte> buffer = stackalloc byte[sizeof(ulong)];
        (byteConverter ?? ByteConverter.SystemEndian).FromUInt64(value, buffer);
        @this.Write(buffer);
    }

    /// <summary>
    /// Writes an unsigned 64-bit value to the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to write to.
    /// </param>
    /// <param name="value">
    /// The value to write.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when writing the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the request.
    /// </param>
    public static async Task WriteUInt64Async(this Stream @this, ulong value, ByteConverter? byteConverter = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@this);
        byte[] buffer = ReusableNumericBuffer.Get();
        try
        {
            (byteConverter ?? ByteConverter.SystemEndian).FromUInt64(value, buffer);
            await @this.WriteAsync(buffer.AsMemory(0, 8), cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            ReusableNumericBuffer.Release(buffer);
        }
    }

    /// <summary>
    /// Writes a 32-bit floating-point value to the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to write to.
    /// </param>
    /// <param name="value">
    /// The value to write.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when writing the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    public static void WriteSingle(this Stream @this, float value, ByteConverter? byteConverter = null)
    {
        ArgumentNullException.ThrowIfNull(@this);
        Span<byte> buffer = stackalloc byte[sizeof(float)];
        (byteConverter ?? ByteConverter.SystemEndian).FromSingle(value, buffer);
        @this.Write(buffer);
    }

    /// <summary>
    /// Writes a 32-bit floating-point value to the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to write to.
    /// </param>
    /// <param name="value">
    /// The value to write.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when writing the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the request.
    /// </param>
    public static async Task WriteSingleAsync(this Stream @this, float value, ByteConverter? byteConverter = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@this);
        byte[] buffer = ReusableNumericBuffer.Get();
        try
        {
            (byteConverter ?? ByteConverter.SystemEndian).FromSingle(value, buffer);
            await @this.WriteAsync(buffer.AsMemory(0, 4), cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            ReusableNumericBuffer.Release(buffer);
        }
    }

    /// <summary>
    /// Writes a 64-bit floating-point value to the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to write to.
    /// </param>
    /// <param name="value">
    /// The value to write.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when writing the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    public static void WriteDouble(this Stream @this, double value, ByteConverter? byteConverter = null)
    {
        ArgumentNullException.ThrowIfNull(@this);
        Span<byte> buffer = stackalloc byte[sizeof(double)];
        (byteConverter ?? ByteConverter.SystemEndian).FromDouble(value, buffer);
        @this.Write(buffer);
    }

    /// <summary>
    /// Writes a 64-bit floating-point value to the stream.
    /// </summary>
    /// <param name="this">
    /// The stream to write to.
    /// </param>
    /// <param name="value">
    /// The value to write.
    /// </param>
    /// <param name="byteConverter">
    /// The byte converter to use when writing the value.
    /// If <c>null</c>, the system's endianness is used.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the request.
    /// </param>
    public static async Task WriteDoubleAsync(this Stream @this, double value, ByteConverter? byteConverter = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@this);
        byte[] buffer = ReusableNumericBuffer.Get();
        try
        {
            (byteConverter ?? ByteConverter.SystemEndian).FromDouble(value, buffer);
            await @this.WriteAsync(buffer.AsMemory(0, 8), cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            ReusableNumericBuffer.Release(buffer);
        }
    }

    /// <summary>
    /// Allows to very efficiently reuse a small buffer for repeated use.
    /// </summary>
    /// <remarks>
    /// In most cases this will prevent unnecessary allocations of buffers. In some multithreaded scenarios it is possible some buffers could be allocated,
    /// but its only possible if both threads are calling on these methods at the same time. Even in that scenario, extra allocations are mitigated.
    /// This amount of mitigation and optimization is pretty high, and eliminates having to use a much more heavy construct such as ArrayPool.
    /// </remarks>
    private static class ReusableNumericBuffer
    {
        private static byte[]? s_buffer = null;

        /// <summary>
        /// Gets a small buffer to use for a numeric operation.
        /// </summary>
        /// <returns>
        /// A small buffer to use for a numeric operation.
        /// </returns>
        public static byte[] Get() => Interlocked.Exchange(ref s_buffer, null) ?? new byte[8];

        /// <summary>
        /// Restores the small buffer used for a numeric operation.
        /// </summary>
        /// <param name="buffer">
        /// The buffer to restore.
        /// </param>
        public static void Release(byte[] buffer) => Interlocked.Exchange(ref s_buffer, buffer);
    }
}
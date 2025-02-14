using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Phaeyz.Marshalling;

/// <summary>
/// Utility methods for efficiently reading and writing intrinsic values to and from a byte sequence,
/// while honoring the byte order of the values.
/// </summary>
public class ByteConverter
{
    #region Endian
    /// <summary>
    /// Gets an instance of ByteConverter which reads values with a big endian byte order.
    /// </summary>
    public static ByteConverter BigEndian { get; } = new ByteConverter(false);

    /// <summary>
    /// Gets an instance of ByteConverter which reads values with a little endian byte order.
    /// </summary>
    public static ByteConverter LittleEndian { get; } = new ByteConverter(true);

    /// <summary>
    /// Gets an instance of ByteConverter which reads values with the byte order of the native platform.
    /// </summary>
    public static ByteConverter SystemEndian => BitConverter.IsLittleEndian ? LittleEndian : BigEndian;

    /// <summary>
    /// Creates and initializes a new ByteConverter instance.
    /// </summary>
    /// <param name="littleEndian">
    /// If <c>true</c>, values are read with a little endian byte order. If <c>false</c>, values are read with a big endian byte order.
    /// </param>
    private ByteConverter(bool littleEndian) => IsLittleEndian = littleEndian;

    /// <summary>
    /// Determines whether or not the ByteConverter instance is little endian byte order.
    /// </summary>
    public bool IsLittleEndian { get; }
    #endregion

    #region FromBoolean
    /// <summary>
    /// Writes boolean (a one-byte value where zero is <c>false</c>, non-zero is <c>true</c>) to a buffer.
    /// </summary>
    /// <param name="value">
    /// The boolean value to write to the buffer.
    /// </param>
    /// <param name="buffer">
    /// The buffer to write the value to.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void FromBoolean(bool value, Span<byte> buffer) => FromByte(value ? (byte)1 : (byte)0, buffer);
    #endregion FromByte

    #region FromByte
    /// <summary>
    /// Writes an unsigned 8-bit value to a buffer.
    /// </summary>
    /// <param name="value">
    /// The unsigned 8-bit value to write to the buffer.
    /// </param>
    /// <param name="buffer">
    /// The buffer to write the value to.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable CA1822 // Mark members as static
    public void FromByte(byte value, Span<byte> buffer)
#pragma warning restore CA1822 // Mark members as static
    {
        if (buffer.Length < sizeof(byte))
        {
            throw new ArgumentOutOfRangeException(nameof(buffer));
        }
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(buffer), value);
    }
    #endregion FromByte

    #region FromSByte
    /// <summary>
    /// Writes a signed 8-bit value to a buffer.
    /// </summary>
    /// <param name="value">
    /// The signed 8-bit value to write to the buffer.
    /// </param>
    /// <param name="buffer">
    /// The buffer to write the value to.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable CA1822 // Mark members as static
    public void FromSByte(sbyte value, Span<byte> buffer)
#pragma warning restore CA1822 // Mark members as static
    {
        if (buffer.Length < sizeof(sbyte))
        {
            throw new ArgumentOutOfRangeException(nameof(buffer));
        }
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(buffer), (byte)value);
    }
    #endregion FromByte

    #region FromUInt16
    /// <summary>
    /// Writes an unsigned 8-bit value to a buffer.
    /// </summary>
    /// <param name="value">
    /// The unsigned 8-bit value to write to the buffer.
    /// </param>
    /// <param name="buffer">
    /// The buffer to write the value to.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void FromUInt16(ushort value, Span<byte> buffer)
    {
        if (buffer.Length < sizeof(ushort))
        {
            throw new ArgumentOutOfRangeException(nameof(buffer));
        }
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(buffer), FixEndian(value));
    }
    #endregion FromUInt16

    #region FromInt16
    /// <summary>
    /// Writes a signed 8-bit value to a buffer.
    /// </summary>
    /// <param name="value">
    /// The signed 8-bit value to write to the buffer.
    /// </param>
    /// <param name="buffer">
    /// The buffer to write the value to.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void FromInt16(short value, Span<byte> buffer)
    {
        if (buffer.Length < sizeof(short))
        {
            throw new ArgumentOutOfRangeException(nameof(buffer));
        }
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(buffer), FixEndian(value));
    }
    #endregion FromInt16

    #region FromUInt32
    /// <summary>
    /// Writes an unsigned 32-bit value to a buffer.
    /// </summary>
    /// <param name="value">
    /// The unsigned 32-bit value to write to the buffer.
    /// </param>
    /// <param name="buffer">
    /// The buffer to write the value to.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void FromUInt32(uint value, Span<byte> buffer)
    {
        if (buffer.Length < sizeof(uint))
        {
            throw new ArgumentOutOfRangeException(nameof(buffer));
        }
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(buffer), FixEndian(value));
    }
    #endregion FromUInt32

    #region FromInt32
    /// <summary>
    /// Writes a signed 32-bit value to a buffer.
    /// </summary>
    /// <param name="value">
    /// The signed 32-bit value to write to the buffer.
    /// </param>
    /// <param name="buffer">
    /// The buffer to write the value to.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void FromInt32(int value, Span<byte> buffer)
    {
        if (buffer.Length < sizeof(int))
        {
            throw new ArgumentOutOfRangeException(nameof(buffer));
        }
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(buffer), FixEndian(value));
    }
    #endregion FromInt32

    #region FromUInt64
    /// <summary>
    /// Writes an unsigned 64-bit value to a buffer.
    /// </summary>
    /// <param name="value">
    /// The unsigned 64-bit value to write to the buffer.
    /// </param>
    /// <param name="buffer">
    /// The buffer to write the value to.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void FromUInt64(ulong value, Span<byte> buffer)
    {
        if (buffer.Length < sizeof(ulong))
        {
            throw new ArgumentOutOfRangeException(nameof(buffer));
        }
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(buffer), FixEndian(value));
    }
    #endregion FromUInt64

    #region FromInt64
    /// <summary>
    /// Writes a signed 64-bit value to a buffer.
    /// </summary>
    /// <param name="value">
    /// The signed 64-bit value to write to the buffer.
    /// </param>
    /// <param name="buffer">
    /// The buffer to write the value to.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void FromInt64(long value, Span<byte> buffer)
    {
        if (buffer.Length < sizeof(long))
        {
            throw new ArgumentOutOfRangeException(nameof(buffer));
        }
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(buffer), FixEndian(value));
    }
    #endregion FromInt64

    #region FromSingle
    /// <summary>
    /// Writes a 32-bit floating-point value to a buffer.
    /// </summary>
    /// <param name="value">
    /// The 32-bit floating-point value to write to the buffer.
    /// </param>
    /// <param name="buffer">
    /// The buffer to write the value to.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void FromSingle(float value, Span<byte> buffer)
    {
        if (buffer.Length < sizeof(float))
        {
            throw new ArgumentOutOfRangeException(nameof(buffer));
        }
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(buffer), value);
        if (IsLittleEndian != BitConverter.IsLittleEndian)
        {
            buffer[..sizeof(float)].Reverse();
        }
    }
    #endregion FromSingle

    #region FromDouble
    /// <summary>
    /// Writes a 64-bit floating-point value to a buffer.
    /// </summary>
    /// <param name="value">
    /// The 64-bit floating-point value to write to the buffer.
    /// </param>
    /// <param name="buffer">
    /// The buffer to write the value to.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void FromDouble(double value, Span<byte> buffer)
    {
        if (buffer.Length < sizeof(double))
        {
            throw new ArgumentOutOfRangeException(nameof(buffer));
        }
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(buffer), value);
        if (IsLittleEndian != BitConverter.IsLittleEndian)
        {
            buffer[..sizeof(double)].Reverse();
        }
    }
    #endregion FromDouble

    #region ToBoolean
    /// <summary>
    /// Reads the zero offset of a span as a boolean (a one-byte value where zero is <c>false</c>, non-zero is <c>true</c>).
    /// </summary>
    /// <param name="value">
    /// The span which contains the value at the zero offset.
    /// </param>
    /// <returns>
    /// A boolean value read from the span.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ToBoolean(ReadOnlySpan<byte> value) => ToByte(value) != 0;
    #endregion ToBoolean

    #region ToByte
    /// <summary>
    /// Reads the zero offset of a span as an unsigned 8-bit value.
    /// </summary>
    /// <param name="buffer">
    /// The span which contains the value at the zero offset.
    /// </param>
    /// <returns>
    /// An unsigned 8-bit value read from the span.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable CA1822 // Mark members as static
    public byte ToByte(ReadOnlySpan<byte> buffer)
#pragma warning restore CA1822 // Mark members as static
    {
        if (buffer.Length < sizeof(byte))
        {
            throw new ArgumentOutOfRangeException(nameof(buffer));
        }
        return Unsafe.ReadUnaligned<byte>(ref MemoryMarshal.GetReference(buffer));
    }
    #endregion ToByte

    #region ToSByte
    /// <summary>
    /// Reads the zero offset of a span as a signed 8-bit value.
    /// </summary>
    /// <param name="buffer">
    /// The span which contains the value at the zero offset.
    /// </param>
    /// <returns>
    /// A signed 8-bit value read from the span.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable CA1822 // Mark members as static
    public sbyte ToSByte(ReadOnlySpan<byte> buffer)
#pragma warning restore CA1822 // Mark members as static
    {
        if (buffer.Length < sizeof(byte))
        {
            throw new ArgumentOutOfRangeException(nameof(buffer));
        }
        return Unsafe.ReadUnaligned<sbyte>(ref MemoryMarshal.GetReference(buffer));
    }
    #endregion ToSByte

    #region ToUInt16
    /// <summary>
    /// Reads the zero offset of a span as an unsigned 16-bit value.
    /// </summary>
    /// <param name="buffer">
    /// The span which contains the value at the zero offset.
    /// </param>
    /// <returns>
    /// An unsigned 16-bit value read from the span.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort ToUInt16(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < sizeof(ushort))
        {
            throw new ArgumentOutOfRangeException(nameof(buffer));
        }
        return FixEndian(Unsafe.ReadUnaligned<ushort>(ref MemoryMarshal.GetReference(buffer)));
    }
    #endregion ToUInt16

    #region ToInt16
    /// <summary>
    /// Reads the zero offset of a span as a signed 16-bit value.
    /// </summary>
    /// <param name="buffer">
    /// The span which contains the value at the zero offset.
    /// </param>
    /// <returns>
    /// A signed 16-bit value read from the span.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ToInt16(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < sizeof(short))
        {
            throw new ArgumentOutOfRangeException(nameof(buffer));
        }
        return FixEndian(Unsafe.ReadUnaligned<short>(ref MemoryMarshal.GetReference(buffer)));
    }
    #endregion ToInt16

    #region ToUInt32
    /// <summary>
    /// Reads the zero offset of a span as an unsigned 32-bit value.
    /// </summary>
    /// <param name="buffer">
    /// The span which contains the value at the zero offset.
    /// </param>
    /// <returns>
    /// An unsigned 32-bit value read from the span.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ToUInt32(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < sizeof(uint))
        {
            throw new ArgumentOutOfRangeException(nameof(buffer));
        }
        return FixEndian(Unsafe.ReadUnaligned<uint>(ref MemoryMarshal.GetReference(buffer)));
    }
    #endregion ToUInt32

    #region ToInt32
    /// <summary>
    /// Reads the zero offset of a span as a signed 32-bit value.
    /// </summary>
    /// <param name="buffer">
    /// The span which contains the value at the zero offset.
    /// </param>
    /// <returns>
    /// A signed 32-bit value read from the span.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ToInt32(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < sizeof(int))
        {
            throw new ArgumentOutOfRangeException(nameof(buffer));
        }
        return FixEndian(Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(buffer)));
    }
    #endregion ToInt32

    #region ToUInt64
    /// <summary>
    /// Reads the zero offset of a span as an unsigned 64-bit value.
    /// </summary>
    /// <param name="buffer">
    /// The span which contains the value at the zero offset.
    /// </param>
    /// <returns>
    /// An unsigned 64-bit value read from the span.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ToUInt64(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < sizeof(ulong))
        {
            throw new ArgumentOutOfRangeException(nameof(buffer));
        }
        return FixEndian(Unsafe.ReadUnaligned<ulong>(ref MemoryMarshal.GetReference(buffer)));
    }
    #endregion ToUInt64

    #region ToInt64
    /// <summary>
    /// Reads the zero offset of a span as a signed 64-bit value.
    /// </summary>
    /// <param name="buffer">
    /// The span which contains the value at the zero offset.
    /// </param>
    /// <returns>
    /// A signed 64-bit value read from the span.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ToInt64(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < sizeof(long))
        {
            throw new ArgumentOutOfRangeException(nameof(buffer));
        }
        return FixEndian(Unsafe.ReadUnaligned<long>(ref MemoryMarshal.GetReference(buffer)));
    }
    #endregion ToInt64

    #region ToSingle
    /// <summary>
    /// Reads the zero offset of a span as a 32-bit floating-point value.
    /// </summary>
    /// <param name="buffer">
    /// The span which contains the value at the zero offset.
    /// </param>
    /// <returns>
    /// A 32-bit floating-point value read from the span.
    /// </returns>
    public float ToSingle(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < sizeof(float))
        {
            throw new ArgumentOutOfRangeException(nameof(buffer));
        }
        if (IsLittleEndian == BitConverter.IsLittleEndian)
        {
            return Unsafe.ReadUnaligned<float>(ref MemoryMarshal.GetReference(buffer));
        }
        Span<byte> reversedBytes = stackalloc byte[sizeof(float)];
        buffer[..sizeof(float)].CopyTo(reversedBytes);
        reversedBytes.Reverse();
        return Unsafe.ReadUnaligned<float>(ref MemoryMarshal.GetReference(reversedBytes));
    }
    #endregion ToSingle

    #region ToDouble
    /// <summary>
    /// Reads the zero offset of a span as a 64-bit floating-point value.
    /// </summary>
    /// <param name="buffer">
    /// The span which contains the value at the zero offset.
    /// </param>
    /// <returns>
    /// A 64-bit floating-point value read from the span.
    /// </returns>
    public double ToDouble(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < sizeof(double))
        {
            throw new ArgumentOutOfRangeException(nameof(buffer));
        }
        if (IsLittleEndian == BitConverter.IsLittleEndian)
        {
            return Unsafe.ReadUnaligned<double>(ref MemoryMarshal.GetReference(buffer));
        }
        Span<byte> reversedBytes = stackalloc byte[sizeof(double)];
        buffer[..sizeof(double)].CopyTo(reversedBytes);
        reversedBytes.Reverse();
        return Unsafe.ReadUnaligned<double>(ref MemoryMarshal.GetReference(reversedBytes));
    }
    #endregion ToDouble

    #region SwapEndian
    /// <summary>
    /// Swaps the byte order of the value.
    /// </summary>
    /// <param name="value">
    /// The value to change the byte order.
    /// </param>
    /// <returns>
    /// The value with the byte order swapped.
    /// </returns>
    public static ushort SwapEndian(ushort value) => (ushort)((value >> 8) | (value << 8));

    /// <summary>
    /// Swaps the byte order of the value.
    /// </summary>
    /// <param name="value">
    /// The value to change the byte order.
    /// </param>
    /// <returns>
    /// The value with the byte order swapped.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short SwapEndian(short value) => (short)SwapEndian((ushort)value);

    /// <summary>
    /// Swaps the byte order of the value.
    /// </summary>
    /// <param name="value">
    /// The value to change the byte order.
    /// </param>
    /// <returns>
    /// The value with the byte order swapped.
    /// </returns>
    public static uint SwapEndian(uint value) => (value >> 24) | ((value >> 8) & 0x0000FF00) | ((value << 8) & 0x00FF0000) | (value << 24);

    /// <summary>
    /// Swaps the byte order of the value.
    /// </summary>
    /// <param name="value">
    /// The value to change the byte order.
    /// </param>
    /// <returns>
    /// The value with the byte order swapped.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SwapEndian(int value) => (int)SwapEndian((uint)value);

    /// <summary>
    /// Swaps the byte order of the value.
    /// </summary>
    /// <param name="value">
    /// The value to change the byte order.
    /// </param>
    /// <returns>
    /// The value with the byte order swapped.
    /// </returns>
    public static ulong SwapEndian(ulong value) =>
        (value >> 56) |
        ((value >> 40) & 0x000000000000FF00) |
        ((value >> 24) & 0x0000000000FF0000) |
        ((value >> 8) & 0x00000000FF000000) |
        ((value << 8) & 0x000000FF00000000) |
        ((value << 24) & 0x0000FF0000000000) |
        ((value << 40) & 0x00FF000000000000) |
        (value << 56);

    /// <summary>
    /// Swaps the byte order of the value.
    /// </summary>
    /// <param name="value">
    /// The value to change the byte order.
    /// </param>
    /// <returns>
    /// The value with the byte order swapped.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long SwapEndian(long value) => (long)SwapEndian((ulong)value);
    #endregion SwapEndian

    #region FixEndian
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ushort FixEndian(ushort value) => BitConverter.IsLittleEndian == IsLittleEndian ? value : SwapEndian(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private short FixEndian(short value) => BitConverter.IsLittleEndian == IsLittleEndian ? value : SwapEndian(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private uint FixEndian(uint value) => BitConverter.IsLittleEndian == IsLittleEndian ? value : SwapEndian(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int FixEndian(int value) => BitConverter.IsLittleEndian == IsLittleEndian ? value : SwapEndian(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ulong FixEndian(ulong value) => BitConverter.IsLittleEndian == IsLittleEndian ? value : SwapEndian(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private long FixEndian(long value) => BitConverter.IsLittleEndian == IsLittleEndian ? value : SwapEndian(value);
    #endregion FixEndian

    #region CopyTo byte[] -> byte[]
    /// <summary>
    /// Copies a byte sequence to a target byte sequence.
    /// </summary>
    /// <param name="source">
    /// The source byte sequence to copy.
    /// </param>
    /// <param name="dest">
    /// The target byte sequence.
    /// </param>
    /// <param name="count">
    /// The number of elements to copy. If null the entire source is copied.
    /// </param>
    /// <returns>
    /// The number of elements copied to the target span.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Either source length is less than the count, or the destination length is less than the count.
    /// </exception>
#pragma warning disable CA1822 // Mark members as static
    public int CopyTo(ReadOnlySpan<byte> source, Span<byte> dest, int? count = null)
#pragma warning restore CA1822 // Mark members as static
    {
        if (count is null)
        {
            count = source.Length;
        }
        else if (source.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "The source is too short.");
        }
        if (dest.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(dest), "The destination is too short.");
        }
        source[..count.Value].CopyTo(dest[..count.Value]);
        return count.Value;
    }
    #endregion CopyTo byte[] -> byte[]

    #region CopyTo bool[] -> byte[]
    /// <summary>
    /// Copies a span of bool values to a target byte sequence.
    /// </summary>
    /// <param name="source">
    /// The source span of bool values to copy.
    /// </param>
    /// <param name="dest">
    /// The target byte sequence.
    /// </param>
    /// <param name="count">
    /// The number of elements to copy. If null the entire source is copied.
    /// </param>
    /// <returns>
    /// The number of elements copied to the target byte sequence.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Either source length is less than the count, or the destination length is less than the count.
    /// </exception>
#pragma warning disable CA1822 // Mark members as static
    public int CopyTo(ReadOnlySpan<bool> source, Span<byte> dest, int? count = null)
#pragma warning restore CA1822 // Mark members as static
    {
        if (count is null)
        {
            count = source.Length;
        }
        else if (source.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "The source is too short.");
        }
        if (dest.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(dest), "The destination is too short.");
        }
        for (int i = 0; i < count; i++)
        {
            dest[i] = source[i] ? (byte)1 : (byte)0;
        }
        return count.Value;
    }
    #endregion CopyTo bool[] -> byte[]

    #region CopyTo sbyte[] -> byte[]
    /// <summary>
    /// Copies a span of signed 8-bit values to a target byte sequence.
    /// </summary>
    /// <param name="source">
    /// The source span of signed 8-bit values to copy.
    /// </param>
    /// <param name="dest">
    /// The target byte sequence.
    /// </param>
    /// <param name="count">
    /// The number of elements to copy. If null the entire source is copied.
    /// </param>
    /// <returns>
    /// The number of elements copied to the target byte sequence.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Either source length is less than the count, or the destination length is less than the count.
    /// </exception>
#pragma warning disable CA1822 // Mark members as static
    public int CopyTo(ReadOnlySpan<sbyte> source, Span<byte> dest, int? count = null)
#pragma warning restore CA1822 // Mark members as static
    {
        if (count is null)
        {
            count = source.Length;
        }
        else if (source.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "The source is too short.");
        }
        if (dest.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(dest), "The destination is too short.");
        }
        for (int i = 0; i < count; i++)
        {
            dest[i] = (byte)source[i];
        }
        return count.Value;
    }
    #endregion CopyTo sbyte[] -> byte[]

    #region CopyTo ushort[] -> byte[]
    /// <summary>
    /// Copies a span of unsigned 16-bit values to a target byte sequence.
    /// </summary>
    /// <param name="source">
    /// The source span of unsigned 16-bit values to copy.
    /// </param>
    /// <param name="dest">
    /// The target byte sequence.
    /// </param>
    /// <param name="count">
    /// The number of elements to copy. If null the entire source is copied.
    /// </param>
    /// <returns>
    /// The number of elements copied to the target byte sequence.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Either source length is less than the count, or the destination length is less than the count.
    /// </exception>
    public int CopyTo(ReadOnlySpan<ushort> source, Span<byte> dest, int? count = null)
    {
        Span<ushort> newDest = MemoryMarshal.Cast<byte, ushort>(dest);
        if (count is null)
        {
            count = source.Length;
        }
        else if (source.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "The source is too short.");
        }
        if (dest.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(dest), "The destination is too short.");
        }
        for (int i = 0; i < count; i++)
        {
            newDest[i] = FixEndian(source[i]);
        }
        return count.Value;
    }
    #endregion CopyTo ushort[] -> byte[]

    #region CopyTo short[] -> byte[]
    /// <summary>
    /// Copies a span of signed 16-bit values to a target byte sequence.
    /// </summary>
    /// <param name="source">
    /// The source span of signed 16-bit values to copy.
    /// </param>
    /// <param name="dest">
    /// The target byte sequence.
    /// </param>
    /// <param name="count">
    /// The number of elements to copy. If null the entire source is copied.
    /// </param>
    /// <returns>
    /// The number of elements copied to the target byte sequence.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Either source length is less than the count, or the destination length is less than the count.
    /// </exception>
    public int CopyTo(ReadOnlySpan<short> source, Span<byte> dest, int? count = null)
    {
        Span<short> newDest = MemoryMarshal.Cast<byte, short>(dest);
        if (count is null)
        {
            count = source.Length;
        }
        else if (source.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "The source is too short.");
        }
        if (dest.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(dest), "The destination is too short.");
        }
        for (int i = 0; i < count; i++)
        {
            newDest[i] = FixEndian(source[i]);
        }
        return count.Value;
    }
    #endregion CopyTo short[] -> byte[]

    #region CopyTo uint[] -> byte[]
    /// <summary>
    /// Copies a span of unsigned 32-bit values to a target byte sequence.
    /// </summary>
    /// <param name="source">
    /// The source span of unsigned 32-bit values to copy.
    /// </param>
    /// <param name="dest">
    /// The target byte sequence.
    /// </param>
    /// <param name="count">
    /// The number of elements to copy. If null the entire source is copied.
    /// </param>
    /// <returns>
    /// The number of elements copied to the target byte sequence.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Either source length is less than the count, or the destination length is less than the count.
    /// </exception>
    public int CopyTo(ReadOnlySpan<uint> source, Span<byte> dest, int? count = null)
    {
        Span<uint> newDest = MemoryMarshal.Cast<byte, uint>(dest);
        if (count is null)
        {
            count = source.Length;
        }
        else if (source.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "The source is too short.");
        }
        if (dest.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(dest), "The destination is too short.");
        }
        for (int i = 0; i < count; i++)
        {
            newDest[i] = FixEndian(source[i]);
        }
        return count.Value;
    }
    #endregion CopyTo uint[] -> byte[]

    #region CopyTo int[] -> byte[]
    /// <summary>
    /// Copies a span of signed 32-bit values to a target byte sequence.
    /// </summary>
    /// <param name="source">
    /// The source span of signed 32-bit values to copy.
    /// </param>
    /// <param name="dest">
    /// The target byte sequence.
    /// </param>
    /// <param name="count">
    /// The number of elements to copy. If null the entire source is copied.
    /// </param>
    /// <returns>
    /// The number of elements copied to the target byte sequence.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Either source length is less than the count, or the destination length is less than the count.
    /// </exception>
    public int CopyTo(ReadOnlySpan<int> source, Span<byte> dest, int? count = null)
    {
        Span<int> newDest = MemoryMarshal.Cast<byte, int>(dest);
        if (count is null)
        {
            count = source.Length;
        }
        else if (source.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "The source is too short.");
        }
        if (dest.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(dest), "The destination is too short.");
        }
        for (int i = 0; i < count; i++)
        {
            newDest[i] = FixEndian(source[i]);
        }
        return count.Value;
    }
    #endregion CopyTo int[] -> byte[]

    #region CopyTo ulong[] -> byte[]
    /// <summary>
    /// Copies a span of unsigned 64-bit values to a target byte sequence.
    /// </summary>
    /// <param name="source">
    /// The source span of unsigned 64-bit values to copy.
    /// </param>
    /// <param name="dest">
    /// The target byte sequence.
    /// </param>
    /// <param name="count">
    /// The number of elements to copy. If null the entire source is copied.
    /// </param>
    /// <returns>
    /// The number of elements copied to the target byte sequence.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Either source length is less than the count, or the destination length is less than the count.
    /// </exception>
    public int CopyTo(ReadOnlySpan<ulong> source, Span<byte> dest, int? count = null)
    {
        Span<ulong> newDest = MemoryMarshal.Cast<byte, ulong>(dest);
        if (count is null)
        {
            count = source.Length;
        }
        else if (source.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "The source is too short.");
        }
        if (dest.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(dest), "The destination is too short.");
        }
        for (int i = 0; i < count; i++)
        {
            newDest[i] = FixEndian(source[i]);
        }
        return count.Value;
    }
    #endregion CopyTo ulong[] -> byte[]

    #region CopyTo long[] -> byte[]
    /// <summary>
    /// Copies a span of signed 64-bit values to a target byte sequence.
    /// </summary>
    /// <param name="source">
    /// The source span of signed 64-bit values to copy.
    /// </param>
    /// <param name="dest">
    /// The target byte sequence.
    /// </param>
    /// <param name="count">
    /// The number of elements to copy. If null the entire source is copied.
    /// </param>
    /// <returns>
    /// The number of elements copied to the target byte sequence.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Either source length is less than the count, or the destination length is less than the count.
    /// </exception>
    public int CopyTo(ReadOnlySpan<long> source, Span<byte> dest, int? count = null)
    {
        Span<long> newDest = MemoryMarshal.Cast<byte, long>(dest);
        if (count is null)
        {
            count = source.Length;
        }
        else if (source.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "The source is too short.");
        }
        if (dest.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(dest), "The destination is too short.");
        }
        for (int i = 0; i < count; i++)
        {
            newDest[i] = FixEndian(source[i]);
        }
        return count.Value;
    }
    #endregion CopyTo long[] -> byte[]

    #region CopyTo float[] -> byte[]
    /// <summary>
    /// Copies a span of 32-bit floating-point values to a target byte sequence.
    /// </summary>
    /// <param name="source">
    /// The source span of 32-bit floating-point values to copy.
    /// </param>
    /// <param name="dest">
    /// The target byte sequence.
    /// </param>
    /// <param name="count">
    /// The number of elements to copy. If null the entire source is copied.
    /// </param>
    /// <returns>
    /// The number of elements copied to the target byte sequence.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Either source length is less than the count, or the destination length is less than the count.
    /// </exception>
    public int CopyTo(ReadOnlySpan<float> source, Span<byte> dest, int? count = null)
    {
        if (count is null)
        {
            count = source.Length;
        }
        else if (source.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "The source is too short.");
        }
        if (dest.Length / sizeof(float) < count)
        {
            throw new ArgumentOutOfRangeException(nameof(dest), "The destination is too short.");
        }
        for (int i = 0; i < count; i++)
        {
            FromSingle(source[i], dest.Slice(i * sizeof(float), sizeof(float)));   
        }
        return count.Value;
    }
    #endregion CopyTo float[] -> byte[]

    #region CopyTo double[] -> byte[]
    /// <summary>
    /// Copies a span of 64-bit floating-point values to a target byte sequence.
    /// </summary>
    /// <param name="source">
    /// The source span of 64-bit floating-point values to copy.
    /// </param>
    /// <param name="dest">
    /// The target byte sequence.
    /// </param>
    /// <param name="count">
    /// The number of elements to copy. If null the entire source is copied.
    /// </param>
    /// <returns>
    /// The number of elements copied to the target byte sequence.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Either source length is less than the count, or the destination length is less than the count.
    /// </exception>
    public int CopyTo(ReadOnlySpan<double> source, Span<byte> dest, int? count = null)
    {
        if (count is null)
        {
            count = source.Length;
        }
        else if (source.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "The source is too short.");
        }
        if (dest.Length / sizeof(double) < count)
        {
            throw new ArgumentOutOfRangeException(nameof(dest), "The destination is too short.");
        }
        for (int i = 0; i < count; i++)
        {
            FromDouble(source[i], dest.Slice(i * sizeof(double), sizeof(double)));
        }
        return count.Value;
    }
    #endregion CopyTo double[] -> byte[]

    #region CopyTo byte[] -> bool[]
    /// <summary>
    /// Copies a byte sequence to a target span of bool values.
    /// </summary>
    /// <param name="source">
    /// The source byte sequence to copy.
    /// </param>
    /// <param name="dest">
    /// The target span of bool values.
    /// </param>
    /// <param name="count">
    /// The number of elements to copy. If null the entire source is copied.
    /// </param>
    /// <returns>
    /// The number of elements copied to the target span.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Either source length is less than the count, or the destination length is less than the count.
    /// </exception>
#pragma warning disable CA1822 // Mark members as static
    public int CopyTo(ReadOnlySpan<byte> source, Span<bool> dest, int? count = null)
#pragma warning restore CA1822 // Mark members as static
    {
        if (count is null)
        {
            count = source.Length;
        }
        else if (source.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "The source is too short.");
        }
        if (dest.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(dest), "The destination is too short.");
        }
        for (int i = 0; i < count; i++)
        {
            dest[i] = source[i] != 0;
        }
        return count.Value;
    }
    #endregion CopyTo byte[] -> bool[]

    #region CopyTo byte[] -> sbyte[]
    /// <summary>
    /// Copies a byte sequence to a target span of 8-bit signed values.
    /// </summary>
    /// <param name="source">
    /// The source byte sequence to copy.
    /// </param>
    /// <param name="dest">
    /// The target span of 8-bit signed values.
    /// </param>
    /// <param name="count">
    /// The number of elements to copy. If null the entire source is copied.
    /// </param>
    /// <returns>
    /// The number of elements copied to the target span.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Either source length is less than the count, or the destination length is less than the count.
    /// </exception>
#pragma warning disable CA1822 // Mark members as static
    public int CopyTo(ReadOnlySpan<byte> source, Span<sbyte> dest, int? count = null)
#pragma warning restore CA1822 // Mark members as static
    {
        if (count is null)
        {
            count = source.Length;
        }
        else if (source.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "The source is too short.");
        }
        if (dest.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(dest), "The destination is too short.");
        }
        for (int i = 0; i < count; i++)
        {
            dest[i] = (sbyte)source[i];
        }
        return count.Value;
    }
    #endregion CopyTo byte[] -> sbyte[]

    #region CopyTo byte[] -> ushort[]
    /// <summary>
    /// Copies a byte sequence to a target span of 16-bit unsigned values.
    /// </summary>
    /// <param name="source">
    /// The source byte sequence to copy.
    /// </param>
    /// <param name="dest">
    /// The target span of 16-bit unsigned values.
    /// </param>
    /// <param name="count">
    /// The number of elements to copy. If null the entire source is copied.
    /// </param>
    /// <returns>
    /// The number of elements copied to the target span.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Either source length is less than the count, or the destination length is less than the count.
    /// </exception>
    public int CopyTo(ReadOnlySpan<byte> source, Span<ushort> dest, int? count = null)
    {
        ReadOnlySpan<ushort> newSource = MemoryMarshal.Cast<byte, ushort>(source);
        if (count is null)
        {
            count = newSource.Length;
        }
        else if (newSource.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "The source is too short.");
        }
        if (dest.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(dest), "The destination is too short.");
        }
        for (int i = 0; i < count; i++)
        {
            dest[i] = FixEndian(newSource[i]);
        }
        return count.Value;
    }
    #endregion CopyTo byte[] -> ushort[]

    #region CopyTo byte[] -> short[]
    /// <summary>
    /// Copies a byte sequence to a target span of 16-bit signed values.
    /// </summary>
    /// <param name="source">
    /// The source byte sequence to copy.
    /// </param>
    /// <param name="dest">
    /// The target span of 16-bit signed values.
    /// </param>
    /// <param name="count">
    /// The number of elements to copy. If null the entire source is copied.
    /// </param>
    /// <returns>
    /// The number of elements copied to the target span.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Either source length is less than the count, or the destination length is less than the count.
    /// </exception>
    public int CopyTo(ReadOnlySpan<byte> source, Span<short> dest, int? count = null)
    {
        ReadOnlySpan<short> newSource = MemoryMarshal.Cast<byte, short>(source);
        if (count is null)
        {
            count = newSource.Length;
        }
        else if (newSource.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "The source is too short.");
        }
        if (dest.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(dest), "The destination is too short.");
        }
        for (int i = 0; i < count; i++)
        {
            dest[i] = FixEndian(newSource[i]);
        }
        return count.Value;
    }
    #endregion CopyTo byte[] -> short[]

    #region CopyTo byte[] -> uint[]
    /// <summary>
    /// Copies a byte sequence to a target span of 32-bit unsigned values.
    /// </summary>
    /// <param name="source">
    /// The source byte sequence to copy.
    /// </param>
    /// <param name="dest">
    /// The target span of 32-bit unsigned values.
    /// </param>
    /// <param name="count">
    /// The number of elements to copy. If null the entire source is copied.
    /// </param>
    /// <returns>
    /// The number of elements copied to the target span.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Either source length is less than the count, or the destination length is less than the count.
    /// </exception>
    public int CopyTo(ReadOnlySpan<byte> source, Span<uint> dest, int? count = null)
    {
        ReadOnlySpan<uint> newSource = MemoryMarshal.Cast<byte, uint>(source);
        if (count is null)
        {
            count = newSource.Length;
        }
        else if (newSource.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "The source is too short.");
        }
        if (dest.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(dest), "The destination is too short.");
        }
        for (int i = 0; i < count; i++)
        {
            dest[i] = FixEndian(newSource[i]);
        }
        return count.Value;
    }
    #endregion CopyTo byte[] -> uint[]

    #region CopyTo byte[] -> int[]
    /// <summary>
    /// Copies a byte sequence to a target span of 32-bit signed values.
    /// </summary>
    /// <param name="source">
    /// The source byte sequence to copy.
    /// </param>
    /// <param name="dest">
    /// The target span of 32-bit signed values.
    /// </param>
    /// <param name="count">
    /// The number of elements to copy. If null the entire source is copied.
    /// </param>
    /// <returns>
    /// The number of elements copied to the target span.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Either source length is less than the count, or the destination length is less than the count.
    /// </exception>
    public int CopyTo(ReadOnlySpan<byte> source, Span<int> dest, int? count = null)
    {
        ReadOnlySpan<int> newSource = MemoryMarshal.Cast<byte, int>(source);
        if (count is null)
        {
            count = newSource.Length;
        }
        else if (newSource.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "The source is too short.");
        }
        if (dest.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(dest), "The destination is too short.");
        }
        for (int i = 0; i < count; i++)
        {
            dest[i] = FixEndian(newSource[i]);
        }
        return count.Value;
    }
    #endregion CopyTo byte[] -> int[]

    #region CopyTo byte[] -> ulong[]
    /// <summary>
    /// Copies a byte sequence to a target span of 64-bit unsigned values.
    /// </summary>
    /// <param name="source">
    /// The source byte sequence to copy.
    /// </param>
    /// <param name="dest">
    /// The target span of 64-bit unsigned values.
    /// </param>
    /// <param name="count">
    /// The number of elements to copy. If null the entire source is copied.
    /// </param>
    /// <returns>
    /// The number of elements copied to the target span.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Either source length is less than the count, or the destination length is less than the count.
    /// </exception>
    public int CopyTo(ReadOnlySpan<byte> source, Span<ulong> dest, int? count = null)
    {
        ReadOnlySpan<ulong> newSource = MemoryMarshal.Cast<byte, ulong>(source);
        if (count is null)
        {
            count = newSource.Length;
        }
        else if (newSource.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "The source is too short.");
        }
        if (dest.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(dest), "The destination is too short.");
        }
        for (int i = 0; i < count; i++)
        {
            dest[i] = FixEndian(newSource[i]);
        }
        return count.Value;
    }
    #endregion CopyTo byte[] -> ulong[]

    #region CopyTo byte[] -> long[]
    /// <summary>
    /// Copies a byte sequence to a target span of 64-bit signed values.
    /// </summary>
    /// <param name="source">
    /// The source byte sequence to copy.
    /// </param>
    /// <param name="dest">
    /// The target span of 64-bit signed values.
    /// </param>
    /// <param name="count">
    /// The number of elements to copy. If null the entire source is copied.
    /// </param>
    /// <returns>
    /// The number of elements copied to the target span.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Either source length is less than the count, or the destination length is less than the count.
    /// </exception>
    public int CopyTo(ReadOnlySpan<byte> source, Span<long> dest, int? count = null)
    {
        ReadOnlySpan<long> newSource = MemoryMarshal.Cast<byte, long>(source);
        if (count is null)
        {
            count = newSource.Length;
        }
        else if (newSource.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "The source is too short.");
        }
        if (dest.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(dest), "The destination is too short.");
        }
        for (int i = 0; i < count; i++)
        {
            dest[i] = FixEndian(newSource[i]);
        }
        return count.Value;
    }
    #endregion CopyTo byte[] -> long[]

    #region CopyTo byte[] -> float[]
    /// <summary>
    /// Copies a byte sequence to a target span of 32-bit floating-point values.
    /// </summary>
    /// <param name="source">
    /// The source byte sequence to copy.
    /// </param>
    /// <param name="dest">
    /// The target span of 32-bit floating-point values.
    /// </param>
    /// <param name="count">
    /// The number of elements to copy. If null the entire source is copied.
    /// </param>
    /// <returns>
    /// The number of elements copied to the target span.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Either source length is less than the count, or the destination length is less than the count.
    /// </exception>
    public int CopyTo(ReadOnlySpan<byte> source, Span<float> dest, int? count = null)
    {
        if (count is null)
        {
            count = source.Length / sizeof(float);
        }
        else if ((source.Length / sizeof(float)) < count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "The source is too short.");
        }
        if (dest.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(dest), "The destination is too short.");
        }
        for (int i = 0; i < count; i++)
        {
            dest[i] = ToSingle(source.Slice(i * sizeof(float), sizeof(float)));
        }
        return count.Value;
    }
    #endregion CopyTo byte[] -> float[]

    #region CopyTo byte[] -> double[]
    /// <summary>
    /// Copies a byte sequence to a target span of 64-bit floating-point values.
    /// </summary>
    /// <param name="source">
    /// The source byte sequence to copy.
    /// </param>
    /// <param name="dest">
    /// The target span of 64-bit floating-point values.
    /// </param>
    /// <param name="count">
    /// The number of elements to copy. If null the entire source is copied.
    /// </param>
    /// <returns>
    /// The number of elements copied to the target span.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Either source length is less than the count, or the destination length is less than the count.
    /// </exception>
    public int CopyTo(ReadOnlySpan<byte> source, Span<double> dest, int? count = null)
    {
        if (count is null)
        {
            count = source.Length / sizeof(double);
        }
        else if ((source.Length / sizeof(double)) < count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "The source is too short.");
        }
        if (dest.Length < count)
        {
            throw new ArgumentOutOfRangeException(nameof(dest), "The destination is too short.");
        }
        for (int i = 0; i < count; i++)
        {
            dest[i] = ToDouble(source.Slice(i * sizeof(double), sizeof(double)));
        }
        return count.Value;
    }
    #endregion CopyTo byte[] -> double[]
}
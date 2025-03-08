using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Phaeyz.Marshalling;

/// <summary>
/// An implementation of a 32-bit CRC (Cyclic Redundancy Check).
/// </summary>
/// <remarks>
/// See ISO 3309 [ISO-3309] or ITU-T V.42 [ITU-T-V.42] (https://www.itu.int/rec/T-REC-V.42-200203-I/en)
/// for a formal specification. It is also documented in the PNG specification at (https://www.w3.org/TR/png-3/#5CRC-algorithm).
/// </remarks>
public struct Crc32
{
    /// <summary>
    /// The CRC table used within <c>Update</c> methods.
    /// </summary>
    private static readonly Lazy<ReadOnlyMemory<uint>> s_table = new(CreateTable);

    /// <summary>
    /// Stores the current internal CRC value.
    /// </summary>
    private uint _value;

    /// <summary>
    /// Creates a new <see cref="Phaeyz.Marshalling.Crc32"/> instance.
    /// </summary>
    public Crc32() => _value = uint.MaxValue;

    /// <summary>
    /// Creates a new <see cref="Phaeyz.Marshalling.Crc32"/> instance and initializes
    /// it to a previously known CRC value.
    /// </summary>
    /// <param name="value">
    /// The previously known CRC value.
    /// </param>
    public Crc32(uint value) => _value = value ^ uint.MaxValue;

    /// <summary>
    /// Builds the CRC table.
    /// </summary>
    /// <returns>
    /// The CRC table.
    /// </returns>
    private static ReadOnlyMemory<uint> CreateTable()
    {
        uint[] table = new uint[256];

        for (int byteIndex = 0; byteIndex < 256; ++byteIndex)
        {
            uint value = (uint)byteIndex;

            for (uint bitIndex = 0; bitIndex < 8; ++bitIndex)
            {
                value = (value & 1) != 0 ? (0xEDB88320 ^ (value >> 1)) : (value >> 1);
            }

            table[byteIndex] = value;
        }
        return table;
    }

    /// <summary>
    /// Updates the CRC value with an individual byte.
    /// </summary>
    /// <param name="value">
    /// The individual byte to update the CRC value with.
    /// </param>
    /// <returns>
    /// Returns the current instance.
    /// </returns>
    public Crc32 Update(byte value)
    {
        _value = s_table.Value.Span[(int)((_value ^ value) & 0xFF)] ^ (_value >> 8);
        return this;
    }

    /// <summary>
    /// Updates the CRC value with a series of bytes.
    /// </summary>
    /// <param name="bytes">
    /// The series of bytes to update the CRC value with.
    /// </param>
    /// <returns>
    /// Returns the current instance.
    /// </returns>
    public Crc32 Update(ReadOnlySpan<byte> bytes)
    {
        ref byte bytesRef = ref MemoryMarshal.GetReference(bytes);
        for (int i = 0; i < bytes.Length; i++)
        {
            Update(Unsafe.Add(ref bytesRef, i));
        }
        return this;
    }

    /// <summary>
    /// Resets the CRC back to a default state.
    /// </summary>
    /// <returns>
    /// Returns the current instance.
    /// </returns>
    public Crc32 Reset()
    {
        _value = uint.MaxValue;
        return this;
    }

    /// <summary>
    /// Gets the current CRC value.
    /// </summary>
    public readonly uint Value => _value ^ uint.MaxValue;
}
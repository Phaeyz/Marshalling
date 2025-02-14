namespace Phaeyz.Marshalling.Test;

internal class StreamExtensionsTest
{
    private static readonly byte[] s_positiveIntegerData = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

    private static readonly byte[] s_negativeIntegerData = [254, 253, 252, 251, 250, 249, 248, 247, 246, 245];

    private static readonly byte[] s_positiveSingleLittleEndian = [0x52, 0x06, 0x9E, 0x3F]; // 1.23456789f

    private static readonly byte[] s_negativeSingleLittleEndian = [0x52, 0x06, 0x9E, 0xBF]; // -1.23456789f

    private static readonly byte[] s_positiveDoubleLittleEndian = [0x1B, 0xDE, 0x83, 0x42, 0xCA, 0xC0, 0xF3, 0x3F]; // 1.23456789d

    private static readonly byte[] s_negativeDoubleLittleEndian = [0x1B, 0xDE, 0x83, 0x42, 0xCA, 0xC0, 0xF3, 0xBF]; // -1.23456789d

    private static byte[] Slice(byte[] source, int startIndex, int? length = null) =>
        source[startIndex..(startIndex + (length ?? (source.Length - startIndex)))];

    #region ReadInt8

    [Test]
    public async Task ReadInt8_PositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadInt8()).IsEqualTo((sbyte)0x01);
        await Assert.That(stream.Position).IsEqualTo(1);
    }

    [Test]
    public async Task ReadInt8_NegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadInt8()).IsEqualTo(unchecked((sbyte)0xFE));
        await Assert.That(stream.Position).IsEqualTo(1);
    }

    [Test]
    public async Task ReadUInt8_PositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadUInt8()).IsEqualTo((byte)0x01);
        await Assert.That(stream.Position).IsEqualTo(1);
    }

    [Test]
    public async Task ReadUInt8_NegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadUInt8()).IsEqualTo((byte)0xFE);
        await Assert.That(stream.Position).IsEqualTo(1);
    }

    [Test]
    public async Task ReadInt8Async_PositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadInt8Async()).IsEqualTo((sbyte)0x01);
        await Assert.That(stream.Position).IsEqualTo(1);
    }

    [Test]
    public async Task ReadInt8Async_NegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadInt8Async()).IsEqualTo(unchecked((sbyte)0xFE));
        await Assert.That(stream.Position).IsEqualTo(1);
    }

    [Test]
    public async Task ReadUInt8Async_PositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadUInt8Async()).IsEqualTo((byte)0x01);
        await Assert.That(stream.Position).IsEqualTo(1);
    }

    [Test]
    public async Task ReadUInt8Async_NegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadUInt8Async()).IsEqualTo((byte)0xFE);
        await Assert.That(stream.Position).IsEqualTo(1);
    }

    #endregion ReadInt8

    #region ReadInt16 Little Endian

    [Test]
    public async Task ReadInt16_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadInt16(ByteConverter.LittleEndian)).IsEqualTo((short)0x0201);
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task ReadInt16_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadInt16(ByteConverter.LittleEndian)).IsEqualTo(unchecked((short)0xFDFE));
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task ReadUInt16_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadUInt16(ByteConverter.LittleEndian)).IsEqualTo((ushort)0x0201);
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task ReadUInt16_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadUInt16(ByteConverter.LittleEndian)).IsEqualTo((ushort)0xFDFE);
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task ReadInt16Async_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadInt16Async(ByteConverter.LittleEndian)).IsEqualTo((short)0x0201);
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task ReadInt16Async_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadInt16Async(ByteConverter.LittleEndian)).IsEqualTo(unchecked((short)0xFDFE));
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task ReadUInt16Async_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadUInt16Async(ByteConverter.LittleEndian)).IsEqualTo((ushort)0x0201);
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task ReadUInt16Async_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadUInt16Async(ByteConverter.LittleEndian)).IsEqualTo((ushort)0xFDFE);
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    #endregion ReadInt16 Little Endian

    #region ReadInt16 Big Endian

    [Test]
    public async Task ReadInt16_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadInt16(ByteConverter.BigEndian)).IsEqualTo((short)0x0102);
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task ReadInt16_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadInt16(ByteConverter.BigEndian)).IsEqualTo(unchecked((short)0xFEFD));
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task ReadUInt16_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadUInt16(ByteConverter.BigEndian)).IsEqualTo((ushort)0x0102);
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task ReadUInt16_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadUInt16(ByteConverter.BigEndian)).IsEqualTo((ushort)0xFEFD);
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task ReadInt16Async_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadInt16Async(ByteConverter.BigEndian)).IsEqualTo((short)0x0102);
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task ReadInt16Async_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadInt16Async(ByteConverter.BigEndian)).IsEqualTo(unchecked((short)0xFEFD));
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task ReadUInt16Async_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadUInt16Async(ByteConverter.BigEndian)).IsEqualTo((ushort)0x0102);
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task ReadUInt16Async_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadUInt16Async(ByteConverter.BigEndian)).IsEqualTo((ushort)0xFEFD);
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    #endregion ReadInt16 Big Endian

    #region ReadInt32 Little Endian

    [Test]
    public async Task ReadInt32_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadInt32(ByteConverter.LittleEndian)).IsEqualTo((int)0x04030201);
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task ReadInt32_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadInt32(ByteConverter.LittleEndian)).IsEqualTo(unchecked((int)0xFBFCFDFE));
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task ReadUInt32_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadUInt32(ByteConverter.LittleEndian)).IsEqualTo((uint)0x04030201);
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task ReadUInt32_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadUInt32(ByteConverter.LittleEndian)).IsEqualTo((uint)0xFBFCFDFE);
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task ReadInt32Async_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadInt32Async(ByteConverter.LittleEndian)).IsEqualTo((int)0x04030201);
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task ReadInt32Async_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadInt32Async(ByteConverter.LittleEndian)).IsEqualTo(unchecked((int)0xFBFCFDFE));
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task ReadUInt32Async_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadUInt32Async(ByteConverter.LittleEndian)).IsEqualTo((uint)0x04030201);
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task ReadUInt32Async_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadUInt32Async(ByteConverter.LittleEndian)).IsEqualTo((uint)0xFBFCFDFE);
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    #endregion ReadInt32 Little Endian

    #region ReadInt32 Big Endian

    [Test]
    public async Task ReadInt32_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadInt32(ByteConverter.BigEndian)).IsEqualTo((int)0x01020304);
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task ReadInt32_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadInt32(ByteConverter.BigEndian)).IsEqualTo(unchecked((int)0xFEFDFCFB));
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task ReadUInt32_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadUInt32(ByteConverter.BigEndian)).IsEqualTo((uint)0x01020304);
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task ReadUInt32_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadUInt32(ByteConverter.BigEndian)).IsEqualTo((uint)0xFEFDFCFB);
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task ReadInt32Async_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadInt32Async(ByteConverter.BigEndian)).IsEqualTo((int)0x01020304);
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task ReadInt32Async_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadInt32Async(ByteConverter.BigEndian)).IsEqualTo(unchecked((int)0xFEFDFCFB));
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task ReadUInt32Async_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadUInt32Async(ByteConverter.BigEndian)).IsEqualTo((uint)0x01020304);
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task ReadUInt32Async_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadUInt32Async(ByteConverter.BigEndian)).IsEqualTo((uint)0xFEFDFCFB);
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    #endregion ReadInt32 Big Endian

    #region ReadInt64 Little Endian

    [Test]
    public async Task ReadInt64_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadInt64(ByteConverter.LittleEndian)).IsEqualTo((long)0x0807060504030201);
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task ReadInt64_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadInt64(ByteConverter.LittleEndian)).IsEqualTo(unchecked((long)0xF7F8F9FAFBFCFDFE));
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task ReadUInt64_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadUInt64(ByteConverter.LittleEndian)).IsEqualTo((ulong)0x0807060504030201);
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task ReadUInt64_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadUInt64(ByteConverter.LittleEndian)).IsEqualTo((ulong)0xF7F8F9FAFBFCFDFE);
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task ReadInt64Async_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadInt64Async(ByteConverter.LittleEndian)).IsEqualTo((long)0x0807060504030201);
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task ReadInt64Async_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadInt64Async(ByteConverter.LittleEndian)).IsEqualTo(unchecked((long)0xF7F8F9FAFBFCFDFE));
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task ReadUInt64Async_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadUInt64Async(ByteConverter.LittleEndian)).IsEqualTo((ulong)0x0807060504030201);
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task ReadUInt64Async_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadUInt64Async(ByteConverter.LittleEndian)).IsEqualTo((ulong)0xF7F8F9FAFBFCFDFE);
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    #endregion ReadInt64 Little Endian

    #region ReadInt64 Big Endian

    [Test]
    public async Task ReadInt64_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadInt64(ByteConverter.BigEndian)).IsEqualTo((long)0x0102030405060708);
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task ReadInt64_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadInt64(ByteConverter.BigEndian)).IsEqualTo(unchecked((long)0xFEFDFCFBFAF9F8F7));
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task ReadUInt64_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadUInt64(ByteConverter.BigEndian)).IsEqualTo((ulong)0x0102030405060708);
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task ReadUInt64_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadUInt64(ByteConverter.BigEndian)).IsEqualTo((ulong)0xFEFDFCFBFAF9F8F7);
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task ReadInt64Async_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadInt64Async(ByteConverter.BigEndian)).IsEqualTo((long)0x0102030405060708);
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task ReadInt64Async_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadInt64Async(ByteConverter.BigEndian)).IsEqualTo(unchecked((long)0xFEFDFCFBFAF9F8F7));
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task ReadUInt64Async_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveIntegerData, false);
        await Assert.That(stream.ReadUInt64Async(ByteConverter.BigEndian)).IsEqualTo((ulong)0x0102030405060708);
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task ReadUInt64Async_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeIntegerData, false);
        await Assert.That(stream.ReadUInt64Async(ByteConverter.BigEndian)).IsEqualTo((ulong)0xFEFDFCFBFAF9F8F7);
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    #endregion ReadInt64 Big Endian

    #region ReadSingle Little Endian

    [Test]
    public async Task ReadSingle_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveSingleLittleEndian, false);
        await Assert.That(stream.ReadSingle(ByteConverter.LittleEndian)).IsEqualTo(1.23456789f);
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task ReadSingle_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeSingleLittleEndian, false);
        await Assert.That(stream.ReadSingle(ByteConverter.LittleEndian)).IsEqualTo(-1.23456789f);
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task ReadSingleAsync_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveSingleLittleEndian, false);
        await Assert.That(stream.ReadSingleAsync(ByteConverter.LittleEndian)).IsEqualTo(1.23456789f);
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task ReadSingleAsync_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeSingleLittleEndian, false);
        await Assert.That(stream.ReadSingleAsync(ByteConverter.LittleEndian)).IsEqualTo(-1.23456789f);
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    #endregion ReadSingle Little Endian

    #region ReadSingle Big Endian

    [Test]
    public async Task ReadSingle_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveSingleLittleEndian.Reverse().ToArray(), false);
        await Assert.That(stream.ReadSingle(ByteConverter.BigEndian)).IsEqualTo(1.23456789f);
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task ReadSingle_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeSingleLittleEndian.Reverse().ToArray(), false);
        await Assert.That(stream.ReadSingle(ByteConverter.BigEndian)).IsEqualTo(-1.23456789f);
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task ReadSingleAsync_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveSingleLittleEndian.Reverse().ToArray(), false);
        await Assert.That(stream.ReadSingleAsync(ByteConverter.BigEndian)).IsEqualTo(1.23456789f);
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task ReadSingleAsync_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeSingleLittleEndian.Reverse().ToArray(), false);
        await Assert.That(stream.ReadSingleAsync(ByteConverter.BigEndian)).IsEqualTo(-1.23456789f);
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    #endregion ReadSingle Big Endian

    #region ReadDouble Little Endian

    [Test]
    public async Task ReadDouble_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveDoubleLittleEndian, false);
        await Assert.That(stream.ReadDouble(ByteConverter.LittleEndian)).IsEqualTo(1.23456789d);
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task ReadDouble_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeDoubleLittleEndian, false);
        await Assert.That(stream.ReadDouble(ByteConverter.LittleEndian)).IsEqualTo(-1.23456789d);
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task ReadDoubleAsync_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveDoubleLittleEndian, false);
        await Assert.That(stream.ReadDoubleAsync(ByteConverter.LittleEndian)).IsEqualTo(1.23456789d);
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task ReadDoubleAsync_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeDoubleLittleEndian, false);
        await Assert.That(stream.ReadDoubleAsync(ByteConverter.LittleEndian)).IsEqualTo(-1.23456789d);
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    #endregion ReadDouble Little Endian

    #region ReadDouble Big Endian

    [Test]
    public async Task ReadDouble_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveDoubleLittleEndian.Reverse().ToArray(), false);
        await Assert.That(stream.ReadDouble(ByteConverter.BigEndian)).IsEqualTo(1.23456789d);
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task ReadDouble_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeDoubleLittleEndian.Reverse().ToArray(), false);
        await Assert.That(stream.ReadDouble(ByteConverter.BigEndian)).IsEqualTo(-1.23456789d);
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task ReadDoubleAsync_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_positiveDoubleLittleEndian.Reverse().ToArray(), false);
        await Assert.That(stream.ReadDoubleAsync(ByteConverter.BigEndian)).IsEqualTo(1.23456789d);
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task ReadDoubleAsync_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new(s_negativeDoubleLittleEndian.Reverse().ToArray(), false);
        await Assert.That(stream.ReadDoubleAsync(ByteConverter.BigEndian)).IsEqualTo(-1.23456789d);
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    #endregion ReadDouble Big Endian

    #region WriteInt8

    [Test]
    public async Task WriteInt8_PositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteInt8(1);
        await Assert.That(stream.ToArray()).IsEquivalentTo(new byte[] { 0x01 });
    }

    [Test]
    public async Task WriteInt8_NegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteInt8(-1);
        await Assert.That(stream.ToArray()).IsEquivalentTo(new byte[] { 0xFF });
    }

    [Test]
    public async Task WriteUInt8_PositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteUInt8(1);
        await Assert.That(stream.ToArray()).IsEquivalentTo(new byte[] { 0x01 });
    }

    [Test]
    public async Task WriteUInt8_NegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteUInt8(unchecked((byte)-1));
        await Assert.That(stream.ToArray()).IsEquivalentTo(new byte[] { 0xFF });
    }

    [Test]
    public async Task WriteInt8Async_PositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteInt8Async(1);
        await Assert.That(stream.ToArray()).IsEquivalentTo(new byte[] { 0x01 });
    }

    [Test]
    public async Task WriteInt8Async_NegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteInt8Async(-1);
        await Assert.That(stream.ToArray()).IsEquivalentTo(new byte[] { 0xFF });
    }

    [Test]
    public async Task WriteUInt8Async_PositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteUInt8Async(1);
        await Assert.That(stream.ToArray()).IsEquivalentTo(new byte[] { 0x01 });
    }

    [Test]
    public async Task WriteUInt8Async_NegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteUInt8Async(unchecked((byte)-1));
        await Assert.That(stream.ToArray()).IsEquivalentTo(new byte[] { 0xFF });
    }

    #endregion WriteInt8

    #region WriteInt16 Little Endian

    [Test]
    public async Task WriteInt16_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteInt16(0x0201, ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveIntegerData, 0, 2));
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task WriteInt16_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteInt16(unchecked((short)0xFDFE), ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeIntegerData, 0, 2));
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task WriteUInt16_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteUInt16(0x0201, ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveIntegerData, 0, 2));
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task WriteUInt16_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteUInt16(0xFDFE, ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeIntegerData, 0, 2));
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task WriteInt16Async_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteInt16Async(0x0201, ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveIntegerData, 0, 2));
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task WriteInt16Async_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteInt16Async(unchecked((short)0xFDFE), ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeIntegerData, 0, 2));
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task WriteUInt16Async_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteUInt16Async(0x0201, ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveIntegerData, 0, 2));
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task WriteUInt16Async_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteUInt16Async(0xFDFE, ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeIntegerData, 0, 2));
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    #endregion WriteInt16 Little Endian

    #region WriteInt16 Big Endian

    [Test]
    public async Task WriteInt16_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteInt16(0x0201, ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveIntegerData, 0, 2).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task WriteInt16_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteInt16(unchecked((short)0xFDFE), ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeIntegerData, 0, 2).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task WriteUInt16_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteUInt16(0x0201, ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveIntegerData, 0, 2).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task WriteUInt16_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteUInt16(0xFDFE, ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeIntegerData, 0, 2).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task WriteInt16Async_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteInt16Async(0x0201, ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveIntegerData, 0, 2).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task WriteInt16Async_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteInt16Async(unchecked((short)0xFDFE), ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeIntegerData, 0, 2).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task WriteUInt16Async_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteUInt16Async(0x0201, ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveIntegerData, 0, 2).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    [Test]
    public async Task WriteUInt16Async_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteUInt16Async(0xFDFE, ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeIntegerData, 0, 2).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(2);
    }

    #endregion WriteInt16 Big Endian

    #region WriteInt32 Little Endian

    [Test]
    public async Task WriteInt32_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteInt32(0x04030201, ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveIntegerData, 0, 4));
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task WriteInt32_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteInt32(unchecked((int)0xFBFCFDFE), ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeIntegerData, 0, 4));
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task WriteUInt32_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteUInt32(0x04030201, ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveIntegerData, 0, 4));
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task WriteUInt32_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteUInt32(0xFBFCFDFE, ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeIntegerData, 0, 4));
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task WriteInt32Async_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteInt32Async(0x04030201, ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveIntegerData, 0, 4));
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task WriteInt32Async_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteInt32Async(unchecked((int)0xFBFCFDFE), ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeIntegerData, 0, 4));
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task WriteUInt32Async_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteUInt32Async(0x04030201, ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveIntegerData, 0, 4));
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task WriteUInt32Async_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteUInt32Async(0xFBFCFDFE, ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeIntegerData, 0, 4));
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    #endregion WriteInt32 Little Endian

    #region WriteInt32 Big Endian

    [Test]
    public async Task WriteInt32_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteInt32(0x04030201, ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveIntegerData, 0, 4).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task WriteInt32_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteInt32(unchecked((int)0xFBFCFDFE), ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeIntegerData, 0, 4).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task WriteUInt32_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteUInt32(0x04030201, ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveIntegerData, 0, 4).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task WriteUInt32_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteUInt32(0xFBFCFDFE, ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeIntegerData, 0, 4).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task WriteInt32Async_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteInt32Async(0x04030201, ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveIntegerData, 0, 4).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task WriteInt32Async_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteInt32Async(unchecked((int)0xFBFCFDFE), ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeIntegerData, 0, 4).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task WriteUInt32Async_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteUInt32Async(0x04030201, ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveIntegerData, 0, 4).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task WriteUInt32Async_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteUInt32Async(0xFBFCFDFE, ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeIntegerData, 0, 4).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    #endregion WriteInt32 Big Endian

    #region WriteInt64 Little Endian

    [Test]
    public async Task WriteInt64_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteInt64(0x0807060504030201, ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveIntegerData, 0, 8));
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task WriteInt64_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteInt64(unchecked((long)0xF7F8F9FAFBFCFDFE), ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeIntegerData, 0, 8));
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task WriteUInt64_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteUInt64(0x0807060504030201, ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveIntegerData, 0, 8));
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task WriteUInt64_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteUInt64(0xF7F8F9FAFBFCFDFE, ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeIntegerData, 0, 8));
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task WriteInt64Async_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteInt64Async(0x0807060504030201, ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveIntegerData, 0, 8));
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task WriteInt64Async_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteInt64Async(unchecked((long)0xF7F8F9FAFBFCFDFE), ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeIntegerData, 0, 8));
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task WriteUInt64Async_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteUInt64Async(0x0807060504030201, ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveIntegerData, 0, 8));
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task WriteUInt64Async_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteUInt64Async(0xF7F8F9FAFBFCFDFE, ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeIntegerData, 0, 8));
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    #endregion WriteInt64 Little Endian

    #region WriteInt64 Big Endian

    [Test]
    public async Task WriteInt64_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteInt64(0x0807060504030201, ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveIntegerData, 0, 8).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task WriteInt64_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteInt64(unchecked((long)0xF7F8F9FAFBFCFDFE), ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeIntegerData, 0, 8).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task WriteUInt64_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteUInt64(0x0807060504030201, ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveIntegerData, 0, 8).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task WriteUInt64_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteUInt64(0xF7F8F9FAFBFCFDFE, ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeIntegerData, 0, 8).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task WriteInt64Async_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteInt64Async(0x0807060504030201, ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveIntegerData, 0, 8).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task WriteInt64Async_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteInt64Async(unchecked((long)0xF7F8F9FAFBFCFDFE), ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeIntegerData, 0, 8).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task WriteUInt64Async_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteUInt64Async(0x0807060504030201, ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveIntegerData, 0, 8).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task WriteUInt64Async_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteUInt64Async(0xF7F8F9FAFBFCFDFE, ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeIntegerData, 0, 8).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    #endregion WriteInt64 Big Endian

    #region WriteSingle Little Endian

    [Test]
    public async Task WriteSingle_LittleEndianPositiveValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteSingle(1.23456789f, ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveSingleLittleEndian, 0, 4));
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task WriteSingle_LittleEndianNegativeValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteSingle(-1.23456789f, ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeSingleLittleEndian, 0, 4));
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task WriteSingleAsync_LittleEndianPositiveValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteSingleAsync(1.23456789f, ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveSingleLittleEndian, 0, 4));
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task WriteSingleAsync_LittleEndianNegativeValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteSingleAsync(-1.23456789f, ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeSingleLittleEndian, 0, 4));
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    #endregion WriteSingle Little Endian

    #region WriteSingle Big Endian

    [Test]
    public async Task WriteSingle_BigEndianPositiveValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteSingle(1.23456789f, ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveSingleLittleEndian, 0, 4).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task WriteSingle_BigEndianNegativeValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteSingle(-1.23456789f, ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeSingleLittleEndian, 0, 4).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task WriteSingleAsync_BigEndianPositiveValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteSingleAsync(1.23456789f, ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveSingleLittleEndian, 0, 4).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    [Test]
    public async Task WriteSingleAsync_BigEndianNegativeValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteSingleAsync(-1.23456789f, ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeSingleLittleEndian, 0, 4).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(4);
    }

    #endregion WriteSingle Big Endian

    #region WriteDouble Little Endian

    [Test]
    public async Task WriteDouble_LittleEndianPositiveValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteDouble(1.23456789d, ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveDoubleLittleEndian, 0, 8));
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task WriteDouble_LittleEndianNegativeValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteDouble(-1.23456789d, ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeDoubleLittleEndian, 0, 8));
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task WriteDoubleAsync_LittleEndianPositiveValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteDoubleAsync(1.23456789d, ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveDoubleLittleEndian, 0, 8));
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task WriteDoubleAsync_LittleEndianNegativeValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteDoubleAsync(-1.23456789d, ByteConverter.LittleEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeDoubleLittleEndian, 0, 8));
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    #endregion WriteDouble Little Endian

    #region WriteDouble Big Endian

    [Test]
    public async Task WriteDouble_BigEndianPositiveValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteDouble(1.23456789d, ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveDoubleLittleEndian, 0, 8).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task WriteDouble_BigEndianNegativeValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        stream.WriteDouble(-1.23456789d, ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeDoubleLittleEndian, 0, 8).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task WriteDoubleAsync_BigEndianPositiveValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteDoubleAsync(1.23456789d, ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_positiveDoubleLittleEndian, 0, 8).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    [Test]
    public async Task WriteDoubleAsync_BigEndianNegativeValues_YieldExpectedValues()
    {
        using MemoryStream stream = new();
        await stream.WriteDoubleAsync(-1.23456789d, ByteConverter.BigEndian);
        await Assert.That(stream.ToArray()).IsEquivalentTo(Slice(s_negativeDoubleLittleEndian, 0, 8).Reverse().ToArray());
        await Assert.That(stream.Position).IsEqualTo(8);
    }

    #endregion WriteDouble Big Endian
}

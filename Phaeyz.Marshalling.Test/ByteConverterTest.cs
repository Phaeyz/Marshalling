namespace Phaeyz.Marshalling.Test;

internal class ByteConverterTest
{
    private static readonly byte[] s_positiveIntegerData = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

    private static readonly byte[] s_negativeIntegerData = [254, 253, 252, 251, 250, 249, 248, 247, 246, 245];

    private static readonly byte[] s_positiveSingleLittleEndian = [0x52, 0x06, 0x9E, 0x3F]; // 1.23456789f

    private static readonly byte[] s_negativeSingleLittleEndian = [0x52, 0x06, 0x9E, 0xBF]; // -1.23456789f

    private static readonly byte[] s_positiveDoubleLittleEndian = [0x1B, 0xDE, 0x83, 0x42, 0xCA, 0xC0, 0xF3, 0x3F]; // 1.23456789d

    private static readonly byte[] s_negativeDoubleLittleEndian = [0x1B, 0xDE, 0x83, 0x42, 0xCA, 0xC0, 0xF3, 0xBF]; // -1.23456789d

    #region FromNumber

    [Test]
    public async Task FromNumber_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        var buffer = new byte[10];
        await RunToBytesTest(buffer, () => ByteConverter.LittleEndian.FromBoolean(true, buffer.AsSpan()), [0x01]);
        await RunToBytesTest(buffer, () => ByteConverter.LittleEndian.FromByte(0x01, buffer.AsSpan()), [0x01]);
        await RunToBytesTest(buffer, () => ByteConverter.LittleEndian.FromSByte(0x01, buffer.AsSpan()), [0x01]);
        await RunToBytesTest(buffer, () => ByteConverter.LittleEndian.FromUInt16(0x0102, buffer.AsSpan()), [0x02, 0x01]);
        await RunToBytesTest(buffer, () => ByteConverter.LittleEndian.FromInt16(0x0102, buffer.AsSpan()), [0x02, 0x01]);
        await RunToBytesTest(buffer, () => ByteConverter.LittleEndian.FromUInt32(0x01020304, buffer.AsSpan()), [0x04, 0x03, 0x02, 0x01]);
        await RunToBytesTest(buffer, () => ByteConverter.LittleEndian.FromInt32(0x01020304, buffer.AsSpan()), [0x04, 0x03, 0x02, 0x01]);
        await RunToBytesTest(buffer, () => ByteConverter.LittleEndian.FromUInt64(0x0102030405060708, buffer.AsSpan()), [0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01]);
        await RunToBytesTest(buffer, () => ByteConverter.LittleEndian.FromInt64(0x0102030405060708, buffer.AsSpan()), [0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01]);
    }

    [Test]
    public async Task FromNumber_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        var buffer = new byte[10];
        await RunToBytesTest(buffer, () => ByteConverter.BigEndian.FromBoolean(true, buffer.AsSpan()), [0x01]);
        await RunToBytesTest(buffer, () => ByteConverter.BigEndian.FromByte(0x01, buffer.AsSpan()), [0x01]);
        await RunToBytesTest(buffer, () => ByteConverter.BigEndian.FromSByte(0x01, buffer.AsSpan()), [0x01]);
        await RunToBytesTest(buffer, () => ByteConverter.BigEndian.FromUInt16(0x0102, buffer.AsSpan()), [0x01, 0x02]);
        await RunToBytesTest(buffer, () => ByteConverter.BigEndian.FromInt16(0x0102, buffer.AsSpan()), [0x01, 0x02]);
        await RunToBytesTest(buffer, () => ByteConverter.BigEndian.FromUInt32(0x01020304, buffer.AsSpan()), [0x01, 0x02, 0x03, 0x04]);
        await RunToBytesTest(buffer, () => ByteConverter.BigEndian.FromInt32(0x01020304, buffer.AsSpan()), [0x01, 0x02, 0x03, 0x04]);
        await RunToBytesTest(buffer, () => ByteConverter.BigEndian.FromUInt64(0x0102030405060708, buffer.AsSpan()), [0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08]);
        await RunToBytesTest(buffer, () => ByteConverter.BigEndian.FromInt64(0x0102030405060708, buffer.AsSpan()), [0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08]);
    }

    [Test]
    public async Task FromNumber_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        var buffer = new byte[10];
        await RunToBytesTest(buffer, () => ByteConverter.LittleEndian.FromBoolean(false, buffer.AsSpan()), [0x00]);
        await RunToBytesTest(buffer, () => ByteConverter.LittleEndian.FromByte(0xFE, buffer.AsSpan()), [0xFE]);
        await RunToBytesTest(buffer, () => ByteConverter.LittleEndian.FromSByte(unchecked((sbyte)0xFE), buffer.AsSpan()), [0xFE]);
        await RunToBytesTest(buffer, () => ByteConverter.LittleEndian.FromUInt16(0xFEFD, buffer.AsSpan()), [0xFD, 0xFE]);
        await RunToBytesTest(buffer, () => ByteConverter.LittleEndian.FromInt16(unchecked((short)0xFEFD), buffer.AsSpan()), [0xFD, 0xFE]);
        await RunToBytesTest(buffer, () => ByteConverter.LittleEndian.FromUInt32(0xFEFDFCFB, buffer.AsSpan()), [0xFB, 0xFC, 0xFD, 0xFE]);
        await RunToBytesTest(buffer, () => ByteConverter.LittleEndian.FromInt32(unchecked((int)0xFEFDFCFB), buffer.AsSpan()), [0xFB, 0xFC, 0xFD, 0xFE]);
        await RunToBytesTest(buffer, () => ByteConverter.LittleEndian.FromUInt64(0xFEFDFCFBFAF9F8F7, buffer.AsSpan()), [0xF7, 0xF8, 0xF9, 0xFA, 0xFB, 0xFC, 0xFD, 0xFE]);
        await RunToBytesTest(buffer, () => ByteConverter.LittleEndian.FromInt64(unchecked((long)0xFEFDFCFBFAF9F8F7), buffer.AsSpan()), [0xF7, 0xF8, 0xF9, 0xFA, 0xFB, 0xFC, 0xFD, 0xFE]);
    }

    [Test]
    public async Task FromNumber_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        var buffer = new byte[10];
        await RunToBytesTest(buffer, () => ByteConverter.BigEndian.FromBoolean(false, buffer.AsSpan()), [0x00]);
        await RunToBytesTest(buffer, () => ByteConverter.BigEndian.FromByte(0xFE, buffer.AsSpan()), [0xFE]);
        await RunToBytesTest(buffer, () => ByteConverter.BigEndian.FromSByte(unchecked((sbyte)0xFE), buffer.AsSpan()), [0xFE]);
        await RunToBytesTest(buffer, () => ByteConverter.BigEndian.FromUInt16(0xFEFD, buffer.AsSpan()), [0xFE, 0xFD]);
        await RunToBytesTest(buffer, () => ByteConverter.BigEndian.FromInt16(unchecked((short)0xFEFD), buffer.AsSpan()), [0xFE, 0xFD]);
        await RunToBytesTest(buffer, () => ByteConverter.BigEndian.FromUInt32(0xFEFDFCFB, buffer.AsSpan()), [0xFE, 0xFD, 0xFC, 0xFB]);
        await RunToBytesTest(buffer, () => ByteConverter.BigEndian.FromInt32(unchecked((int)0xFEFDFCFB), buffer.AsSpan()), [0xFE, 0xFD, 0xFC, 0xFB]);
        await RunToBytesTest(buffer, () => ByteConverter.BigEndian.FromUInt64(0xFEFDFCFBFAF9F8F7, buffer.AsSpan()), [0xFE, 0xFD, 0xFC, 0xFB, 0xFA, 0xF9, 0xF8, 0xF7]);
        await RunToBytesTest(buffer, () => ByteConverter.BigEndian.FromInt64(unchecked((long)0xFEFDFCFBFAF9F8F7), buffer.AsSpan()), [0xFE, 0xFD, 0xFC, 0xFB, 0xFA, 0xF9, 0xF8, 0xF7]);
    }

    [Test]
    public async Task FromNumber_LittleEndianFloatingPoint_YieldExpectedValues()
    {
        var buffer = new byte[10];
        await RunToBytesTest(buffer, () => ByteConverter.LittleEndian.FromSingle(1.23456789f, buffer.AsSpan()), s_positiveSingleLittleEndian);
        await RunToBytesTest(buffer, () => ByteConverter.LittleEndian.FromSingle(-1.23456789f, buffer.AsSpan()), s_negativeSingleLittleEndian);
        await RunToBytesTest(buffer, () => ByteConverter.LittleEndian.FromDouble(1.23456789d, buffer.AsSpan()), s_positiveDoubleLittleEndian);
        await RunToBytesTest(buffer, () => ByteConverter.LittleEndian.FromDouble(-1.23456789d, buffer.AsSpan()), s_negativeDoubleLittleEndian);
    }

    [Test]
    public async Task FromNumber_BigEndianFloatingPoint_YieldExpectedValues()
    {
        var buffer = new byte[10];
        await RunToBytesTest(buffer, () => ByteConverter.BigEndian.FromSingle(1.23456789f, buffer.AsSpan()), s_positiveSingleLittleEndian.Reverse().ToArray());
        await RunToBytesTest(buffer, () => ByteConverter.BigEndian.FromSingle(-1.23456789f, buffer.AsSpan()), s_negativeSingleLittleEndian.Reverse().ToArray());
        await RunToBytesTest(buffer, () => ByteConverter.BigEndian.FromDouble(1.23456789d, buffer.AsSpan()), s_positiveDoubleLittleEndian.Reverse().ToArray());
        await RunToBytesTest(buffer, () => ByteConverter.BigEndian.FromDouble(-1.23456789d, buffer.AsSpan()), s_negativeDoubleLittleEndian.Reverse().ToArray());
    }

    #endregion FromNumber

    #region ToNumber

    [Test]
    public async Task ToNumber_LittleEndianPositiveIntegerValues_YieldExpectedValues()
    {
        await Assert.That(ByteConverter.LittleEndian.ToBoolean(s_positiveIntegerData.AsSpan())).IsEqualTo(true);
        await Assert.That(ByteConverter.LittleEndian.ToByte(s_positiveIntegerData.AsSpan())).IsEqualTo((byte)0x01);
        await Assert.That(ByteConverter.LittleEndian.ToSByte(s_positiveIntegerData.AsSpan())).IsEqualTo((sbyte)0x01);
        await Assert.That(ByteConverter.LittleEndian.ToUInt16(s_positiveIntegerData.AsSpan())).IsEqualTo((ushort)0x0201);
        await Assert.That(ByteConverter.LittleEndian.ToInt16(s_positiveIntegerData.AsSpan())).IsEqualTo((short)0x0201);
        await Assert.That(ByteConverter.LittleEndian.ToUInt32(s_positiveIntegerData.AsSpan())).IsEqualTo((uint)0x04030201);
        await Assert.That(ByteConverter.LittleEndian.ToInt32(s_positiveIntegerData.AsSpan())).IsEqualTo((int)0x04030201);
        await Assert.That(ByteConverter.LittleEndian.ToUInt64(s_positiveIntegerData.AsSpan())).IsEqualTo((ulong)0x0807060504030201);
        await Assert.That(ByteConverter.LittleEndian.ToInt64(s_positiveIntegerData.AsSpan())).IsEqualTo((long)0x0807060504030201);
    }

    [Test]
    public async Task ToNumber_BigEndianPositiveIntegerValues_YieldExpectedValues()
    {
        await Assert.That(ByteConverter.BigEndian.ToBoolean(s_positiveIntegerData.AsSpan())).IsEqualTo(true);
        await Assert.That(ByteConverter.BigEndian.ToByte(s_positiveIntegerData.AsSpan())).IsEqualTo((byte)0x01);
        await Assert.That(ByteConverter.BigEndian.ToSByte(s_positiveIntegerData.AsSpan())).IsEqualTo((sbyte)0x01);
        await Assert.That(ByteConverter.BigEndian.ToUInt16(s_positiveIntegerData.AsSpan())).IsEqualTo((ushort)0x0102);
        await Assert.That(ByteConverter.BigEndian.ToInt16(s_positiveIntegerData.AsSpan())).IsEqualTo((short)0x0102);
        await Assert.That(ByteConverter.BigEndian.ToUInt32(s_positiveIntegerData.AsSpan())).IsEqualTo((uint)0x01020304);
        await Assert.That(ByteConverter.BigEndian.ToInt32(s_positiveIntegerData.AsSpan())).IsEqualTo((int)0x01020304);
        await Assert.That(ByteConverter.BigEndian.ToUInt64(s_positiveIntegerData.AsSpan())).IsEqualTo((ulong)0x0102030405060708);
        await Assert.That(ByteConverter.BigEndian.ToInt64(s_positiveIntegerData.AsSpan())).IsEqualTo((long)0x0102030405060708);
    }

    [Test]
    public async Task ToNumber_LittleEndianNegativeIntegerValues_YieldExpectedValues()
    {
        await Assert.That(ByteConverter.LittleEndian.ToBoolean(s_negativeIntegerData.AsSpan())).IsEqualTo(true);
        await Assert.That(ByteConverter.LittleEndian.ToByte(s_negativeIntegerData.AsSpan())).IsEqualTo((byte)0xFE);
        await Assert.That(ByteConverter.LittleEndian.ToSByte(s_negativeIntegerData.AsSpan())).IsEqualTo(unchecked((sbyte)0xFE));
        await Assert.That(ByteConverter.LittleEndian.ToUInt16(s_negativeIntegerData.AsSpan())).IsEqualTo((ushort)0xFDFE);
        await Assert.That(ByteConverter.LittleEndian.ToInt16(s_negativeIntegerData.AsSpan())).IsEqualTo(unchecked((short)0xFDFE));
        await Assert.That(ByteConverter.LittleEndian.ToUInt32(s_negativeIntegerData.AsSpan())).IsEqualTo((uint)0xFBFCFDFE);
        await Assert.That(ByteConverter.LittleEndian.ToInt32(s_negativeIntegerData.AsSpan())).IsEqualTo(unchecked((int)0xFBFCFDFE));
        await Assert.That(ByteConverter.LittleEndian.ToUInt64(s_negativeIntegerData.AsSpan())).IsEqualTo((ulong)0xF7F8F9FAFBFCFDFE);
        await Assert.That(ByteConverter.LittleEndian.ToInt64(s_negativeIntegerData.AsSpan())).IsEqualTo(unchecked((long)0xF7F8F9FAFBFCFDFE));
    }

    [Test]
    public async Task ToNumber_BigEndianNegativeIntegerValues_YieldExpectedValues()
    {
        await Assert.That(ByteConverter.BigEndian.ToBoolean(s_negativeIntegerData.AsSpan())).IsEqualTo(true);
        await Assert.That(ByteConverter.BigEndian.ToByte(s_negativeIntegerData.AsSpan())).IsEqualTo((byte)0xFE);
        await Assert.That(ByteConverter.BigEndian.ToSByte(s_negativeIntegerData.AsSpan())).IsEqualTo(unchecked((sbyte)0xFE));
        await Assert.That(ByteConverter.BigEndian.ToUInt16(s_negativeIntegerData.AsSpan())).IsEqualTo((ushort)0xFEFD);
        await Assert.That(ByteConverter.BigEndian.ToInt16(s_negativeIntegerData.AsSpan())).IsEqualTo(unchecked((short)0xFEFD));
        await Assert.That(ByteConverter.BigEndian.ToUInt32(s_negativeIntegerData.AsSpan())).IsEqualTo((uint)0xFEFDFCFB);
        await Assert.That(ByteConverter.BigEndian.ToInt32(s_negativeIntegerData.AsSpan())).IsEqualTo(unchecked((int)0xFEFDFCFB));
        await Assert.That(ByteConverter.BigEndian.ToUInt64(s_negativeIntegerData.AsSpan())).IsEqualTo((ulong)0xFEFDFCFBFAF9F8F7);
        await Assert.That(ByteConverter.BigEndian.ToInt64(s_negativeIntegerData.AsSpan())).IsEqualTo(unchecked((long)0xFEFDFCFBFAF9F8F7));
    }

    [Test]
    public async Task ToNumber_LittleEndianFloatingPoint_YieldExpectedValues()
    {
        await Assert.That(ByteConverter.LittleEndian.ToSingle(s_positiveSingleLittleEndian.AsSpan())).IsEqualTo(1.23456789f);
        await Assert.That(ByteConverter.LittleEndian.ToSingle(s_negativeSingleLittleEndian.AsSpan())).IsEqualTo(-1.23456789f);
        await Assert.That(ByteConverter.LittleEndian.ToDouble(s_positiveDoubleLittleEndian.AsSpan())).IsEqualTo(1.23456789d);
        await Assert.That(ByteConverter.LittleEndian.ToDouble(s_negativeDoubleLittleEndian.AsSpan())).IsEqualTo(-1.23456789d);
    }

    [Test]
    public async Task ToNumber_BigEndianFloatingPoint_YieldExpectedValues()
    {
        await Assert.That(ByteConverter.BigEndian.ToSingle(s_positiveSingleLittleEndian.Reverse().ToArray().AsSpan())).IsEqualTo(1.23456789f);
        await Assert.That(ByteConverter.BigEndian.ToSingle(s_negativeSingleLittleEndian.Reverse().ToArray().AsSpan())).IsEqualTo(-1.23456789f);
        await Assert.That(ByteConverter.BigEndian.ToDouble(s_positiveDoubleLittleEndian.Reverse().ToArray().AsSpan())).IsEqualTo(1.23456789d);
        await Assert.That(ByteConverter.BigEndian.ToDouble(s_negativeDoubleLittleEndian.Reverse().ToArray().AsSpan())).IsEqualTo(-1.23456789d);
    }

    #endregion ToNumber

    #region SwapEndian

    [Test]
    public async Task SwapEndian_AllIntegerTypes_YieldExpectedValues()
    {
        await Assert.That(ByteConverter.SwapEndian((short)0x0102)).IsEqualTo((short)0x0201);
        await Assert.That(ByteConverter.SwapEndian((ushort)0x0102)).IsEqualTo((ushort)0x0201);
        await Assert.That(ByteConverter.SwapEndian((int)0x01020304)).IsEqualTo((int)0x04030201);
        await Assert.That(ByteConverter.SwapEndian((uint)0x01020304)).IsEqualTo((uint)0x04030201);
        await Assert.That(ByteConverter.SwapEndian((long)0x0102030405060708)).IsEqualTo((long)0x0807060504030201);
        await Assert.That(ByteConverter.SwapEndian((ulong)0x0102030405060708)).IsEqualTo((ulong)0x0807060504030201);

        await Assert.That(ByteConverter.SwapEndian(unchecked((short)0xFEFD))).IsEqualTo(unchecked((short)0xFDFE));
        await Assert.That(ByteConverter.SwapEndian((ushort)0xFEFD)).IsEqualTo((ushort)0xFDFE);
        await Assert.That(ByteConverter.SwapEndian(unchecked((int)0xFEFDFCFB))).IsEqualTo(unchecked((int)0xFBFCFDFE));
        await Assert.That(ByteConverter.SwapEndian((uint)0xFEFDFCFB)).IsEqualTo((uint)0xFBFCFDFE);
        await Assert.That(ByteConverter.SwapEndian(unchecked((long)0xFEFDFCFBFAF9F8F7))).IsEqualTo(unchecked((long)0xF7F8F9FAFBFCFDFE));
        await Assert.That(ByteConverter.SwapEndian((ulong)0xFEFDFCFBFAF9F8F7)).IsEqualTo((ulong)0xF7F8F9FAFBFCFDFE);
    }

    #endregion SwapEndian

    #region CopyTo byte[] -> byte[] (Positive Bytes)

    [Test]
    public async Task CopyTo_LittleEndianPositiveByteWithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..1], sourceCount);
        byte[] destination = new byte[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new byte[] { 0x01, 0x01, 0x01, 0 });
    }

    [Test]
    public async Task CopyTo_LittleEndianPositiveByteWithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..1], sourceCount);
        byte[] destination = new byte[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new byte[] { 0x01, 0x01, 0x01, 0 });
    }

    [Test]
    public async Task CopyTo_BigEndianPositiveByteWithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..1], sourceCount);
        byte[] destination = new byte[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new byte[] { 0x01, 0x01, 0x01, 0 });
    }

    [Test]
    public async Task CopyTo_BigEndianPositiveByteWithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..1], sourceCount);
        byte[] destination = new byte[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new byte[] { 0x01, 0x01, 0x01, 0 });
    }

    #endregion CopyTo byte[] -> byte[] (Positive Bytes)

    #region CopyTo byte[] -> byte[] (Negative Bytes)

    [Test]
    public async Task CopyTo_LittleEndianNegativeByteWithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..1], sourceCount);
        byte[] destination = new byte[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new byte[] { unchecked((byte)0xFE), unchecked((byte)0xFE), unchecked((byte)0xFE), 0 });
    }

    [Test]
    public async Task CopyTo_LittleEndianNegativeByteWithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..1], sourceCount);
        byte[] destination = new byte[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new byte[] { unchecked((byte)0xFE), unchecked((byte)0xFE), unchecked((byte)0xFE), 0 });
    }

    [Test]
    public async Task CopyTo_BigEndianNegativeByteWithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..1], sourceCount);
        byte[] destination = new byte[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new byte[] { unchecked((byte)0xFE), unchecked((byte)0xFE), unchecked((byte)0xFE), 0 });
    }

    [Test]
    public async Task CopyTo_BigEndianNegativeByteWithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..1], sourceCount);
        byte[] destination = new byte[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new byte[] { unchecked((byte)0xFE), unchecked((byte)0xFE), unchecked((byte)0xFE), 0 });
    }

    #endregion CopyTo byte[] -> byte[] (Negative Bytes)

    #region CopyTo bool[] -> byte[]

    [Test]
    public async Task CopyTo_ToBytesLittleEndianBooleanFalseAndTrue_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo([false, true, false, true, false, true, false], buffer),
            [0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianBooleanFalseAndTrue_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo([false, true, false, true, false, true, false], buffer),
            [0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00]);
    }

    [Test]
    public async Task CopyTo_ToBytesLittleEndianBooleanFalseAndTrueMaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo([false, true, false, true, false, true, false], buffer, 2),
            [0x00, 0x01]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianBooleanFalseAndTrueMaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo([false, true, false, true, false, true, false], buffer, 2),
            [0x00, 0x01]);
    }

    #endregion CopyTo bool[] -> byte[]

    #region CopyTo sbyte[] -> byte[] (Positive Bytes)

    [Test]
    public async Task CopyTo_ToBytesLittleEndianPositiveSByte_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new sbyte[] { 0x01, 0x02, 0x03 }, buffer),
            [0x01, 0x02, 0x03]);
    }

    [Test]
    public async Task CopyTo_ToBytesLittleEndianPositiveSByteMaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new sbyte[] { 0x01, 0x02, 0x03 }, buffer, 2),
            [0x01, 0x02]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianPositiveSByte_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new sbyte[] { 0x01, 0x02, 0x03 }, buffer),
            [0x01, 0x02, 0x03]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianPositiveSByteMaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new sbyte[] { 0x01, 0x02, 0x03 }, buffer, 2),
            [0x01, 0x02]);
    }

    #endregion CopyTo sbyte[] -> byte[] (Positive Bytes)

    #region CopyTo sbyte[] -> byte[] (Negative Bytes)

    [Test]
    public async Task CopyTo_ToBytesLittleEndianNegativeSByte_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo([unchecked((sbyte)0xFE), unchecked((sbyte)0xFD), unchecked((sbyte)0xFC)], buffer),
            [0xFE, 0xFD, 0xFC]);
    }

    [Test]
    public async Task CopyTo_ToBytesLittleEndianNegativeSByteMaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo([unchecked((sbyte)0xFE), unchecked((sbyte)0xFD), unchecked((sbyte)0xFC)], buffer, 2),
            [0xFE, 0xFD]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianNegativeSByte_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo([unchecked((sbyte)0xFE), unchecked((sbyte)0xFD), unchecked((sbyte)0xFC)], buffer),
            [0xFE, 0xFD, 0xFC]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianNegativeSByteMaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo([unchecked((sbyte)0xFE), unchecked((sbyte)0xFD), unchecked((sbyte)0xFC)], buffer, 2),
            [0xFE, 0xFD]);
    }

    #endregion CopyTo sbyte[] -> byte[] (Negative Bytes)

    #region CopyTo ushort[] -> byte[] (Positive Bytes)

    [Test]
    public async Task CopyTo_ToBytesLittleEndianPositiveUInt16_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new ushort[] { 0x0102, 0x0304, 0x0506 }, buffer),
            [0x02, 0x01, 0x04, 0x03, 0x06, 0x05]);
    }

    [Test]
    public async Task CopyTo_ToBytesLittleEndianPositiveUInt16MaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new ushort[] { 0x0102, 0x0304, 0x0506 }, buffer, 2),
            [0x02, 0x01, 0x04, 0x03]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianPositiveUInt16_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new ushort[] { 0x0102, 0x0304, 0x0506 }, buffer),
            [0x01, 0x02, 0x03, 0x04, 0x05, 0x06]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianPositiveUInt16MaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new ushort[] { 0x0102, 0x0304, 0x0506 }, buffer, 2),
            [0x01, 0x02, 0x03, 0x04]);
    }

    #endregion CopyTo ushort[] -> byte[] (Positive Bytes)

    #region CopyTo ushort[] -> byte[] (Negative Bytes)

    [Test]
    public async Task CopyTo_ToBytesLittleEndianNegativeUInt16_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new ushort[] { 0xFEFD, 0xFCFB, 0xFAF9 }, buffer),
            [0xFD, 0xFE, 0xFB, 0xFC, 0xF9, 0xFA]);
    }

    [Test]
    public async Task CopyTo_ToBytesLittleEndianNegativeUInt16MaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new ushort[] { 0xFEFD, 0xFCFB, 0xFAF9 }, buffer, 2),
            [0xFD, 0xFE, 0xFB, 0xFC]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianNegativeUInt16_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new ushort[] { 0xFEFD, 0xFCFB, 0xFAF9 }, buffer),
            [0xFE, 0xFD, 0xFC, 0xFB, 0xFA, 0xF9]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianNegativeUInt16MaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new ushort[] { 0xFEFD, 0xFCFB, 0xFAF9 }, buffer, 2),
            [0xFE, 0xFD, 0xFC, 0xFB]);
    }

    #endregion CopyTo ushort[] -> byte[] (Negative Bytes)

    #region CopyTo short[] -> byte[] (Positive Bytes)

    [Test]
    public async Task CopyTo_ToBytesLittleEndianPositiveInt16_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new short[] { 0x0102, 0x0304, 0x0506 }, buffer),
            [0x02, 0x01, 0x04, 0x03, 0x06, 0x05]);
    }

    [Test]
    public async Task CopyTo_ToBytesLittleEndianPositiveInt16MaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new short[] { 0x0102, 0x0304, 0x0506 }, buffer, 2),
            [0x02, 0x01, 0x04, 0x03]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianPositiveInt16_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new short[] { 0x0102, 0x0304, 0x0506 }, buffer),
            [0x01, 0x02, 0x03, 0x04, 0x05, 0x06]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianPositiveInt16MaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new short[] { 0x0102, 0x0304, 0x0506 }, buffer, 2),
            [0x01, 0x02, 0x03, 0x04]);
    }

    #endregion CopyTo short[] -> byte[] (Positive Bytes)

    #region CopyTo short[] -> byte[] (Negative Bytes)

    [Test]
    public async Task CopyTo_ToBytesLittleEndianNegativeInt16_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo([unchecked((short)0xFEFD), unchecked((short)0xFCFB), unchecked((short)0xFAF9)], buffer),
            [0xFD, 0xFE, 0xFB, 0xFC, 0xF9, 0xFA]);
    }

    [Test]
    public async Task CopyTo_ToBytesLittleEndianNegativeInt16MaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo([unchecked((short)0xFEFD), unchecked((short)0xFCFB), unchecked((short)0xFAF9)], buffer, 2),
            [0xFD, 0xFE, 0xFB, 0xFC]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianNegativeInt16_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo([unchecked((short)0xFEFD), unchecked((short)0xFCFB), unchecked((short)0xFAF9)], buffer),
            [0xFE, 0xFD, 0xFC, 0xFB, 0xFA, 0xF9]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianNegativeInt16MaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[10];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo([unchecked((short)0xFEFD), unchecked((short)0xFCFB), unchecked((short)0xFAF9)], buffer, 2),
            [0xFE, 0xFD, 0xFC, 0xFB]);
    }

    #endregion CopyTo short[] -> byte[] (Negative Bytes)

    #region CopyTo uint[] -> byte[] (Positive Bytes)

    [Test]
    public async Task CopyTo_ToBytesLittleEndianPositiveUInt32_YieldExpectedValue()
    {
        byte[] buffer = new byte[20];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new uint[] { 0x01020304, 0x05060708, 0x090A0B0C }, buffer),
            [0x04, 0x03, 0x02, 0x01, 0x08, 0x07, 0x06, 0x05, 0x0C, 0x0B, 0x0A, 0x09]);
    }

    [Test]
    public async Task CopyTo_ToBytesLittleEndianPositiveUInt32MaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[20];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new uint[] { 0x01020304, 0x05060708, 0x090A0B0C }, buffer, 2),
            [0x04, 0x03, 0x02, 0x01, 0x08, 0x07, 0x06, 0x05]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianPositiveUInt32_YieldExpectedValue()
    {
        byte[] buffer = new byte[20];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new uint[] { 0x01020304, 0x05060708, 0x090A0B0C }, buffer),
            [0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianPositiveUInt32MaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[20];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new uint[] { 0x01020304, 0x05060708, 0x090A0B0C }, buffer, 2),
            [0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08]);
    }

    #endregion CopyTo uint[] -> byte[] (Positive Bytes)

    #region CopyTo uint[] -> byte[] (Negative Bytes)

    [Test]
    public async Task CopyTo_ToBytesLittleEndianNegativeUInt32_YieldExpectedValue()
    {
        byte[] buffer = new byte[20];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new uint[] { 0xFEFDFCFB, 0xFAF9F8F7, 0xF6F5F4F3 }, buffer),
            [0xFB, 0xFC, 0xFD, 0xFE, 0xF7, 0xF8, 0xF9, 0xFA, 0xF3, 0xF4, 0xF5, 0xF6]);
    }

    [Test]
    public async Task CopyTo_ToBytesLittleEndianNegativeUInt32MaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[20];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new uint[] { 0xFEFDFCFB, 0xFAF9F8F7, 0xF6F5F4F3 }, buffer, 2),
            [0xFB, 0xFC, 0xFD, 0xFE, 0xF7, 0xF8, 0xF9, 0xFA]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianNegativeUInt32_YieldExpectedValue()
    {
        byte[] buffer = new byte[20];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new uint[] { 0xFEFDFCFB, 0xFAF9F8F7, 0xF6F5F4F3 }, buffer),
            [0xFE, 0xFD, 0xFC, 0xFB, 0xFA, 0xF9, 0xF8, 0xF7, 0xF6, 0xF5, 0xF4, 0xF3]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianNegativeUInt32MaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[20];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new uint[] { 0xFEFDFCFB, 0xFAF9F8F7, 0xF6F5F4F3 }, buffer, 2),
            [0xFE, 0xFD, 0xFC, 0xFB, 0xFA, 0xF9, 0xF8, 0xF7]);
    }

    #endregion CopyTo uint[] -> byte[] (Negative Bytes)

    #region CopyTo int[] -> byte[] (Positive Bytes)

    [Test]
    public async Task CopyTo_ToBytesLittleEndianPositiveInt32_YieldExpectedValue()
    {
        byte[] buffer = new byte[20];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new int[] { 0x01020304, 0x05060708, 0x090A0B0C }, buffer),
            [0x04, 0x03, 0x02, 0x01, 0x08, 0x07, 0x06, 0x05, 0x0C, 0x0B, 0x0A, 0x09]);
    }

    [Test]
    public async Task CopyTo_ToBytesLittleEndianPositiveInt32MaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[20];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new int[] { 0x01020304, 0x05060708, 0x090A0B0C }, buffer, 2),
            [0x04, 0x03, 0x02, 0x01, 0x08, 0x07, 0x06, 0x05]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianPositiveInt32_YieldExpectedValue()
    {
        byte[] buffer = new byte[20];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new int[] { 0x01020304, 0x05060708, 0x090A0B0C }, buffer),
            [0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianPositiveInt32MaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[20];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new int[] { 0x01020304, 0x05060708, 0x090A0B0C }, buffer, 2),
            [0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08]);
    }

    #endregion CopyTo int[] -> byte[] (Positive Bytes)

    #region CopyTo int[] -> byte[] (Negative Bytes)

    [Test]
    public async Task CopyTo_ToBytesLittleEndianNegativeInt32_YieldExpectedValue()
    {
        byte[] buffer = new byte[20];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new int[] { unchecked((int)0xFEFDFCFB), unchecked((int)0xFAF9F8F7), unchecked((int)0xF6F5F4F3) }, buffer),
            [0xFB, 0xFC, 0xFD, 0xFE, 0xF7, 0xF8, 0xF9, 0xFA, 0xF3, 0xF4, 0xF5, 0xF6]);
    }

    [Test]
    public async Task CopyTo_ToBytesLittleEndianNegativeInt32MaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[20];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new int[] { unchecked((int)0xFEFDFCFB), unchecked((int)0xFAF9F8F7), unchecked((int)0xF6F5F4F3) }, buffer, 2),
            [0xFB, 0xFC, 0xFD, 0xFE, 0xF7, 0xF8, 0xF9, 0xFA]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianNegativeInt32_YieldExpectedValue()
    {
        byte[] buffer = new byte[20];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new int[] { unchecked((int)0xFEFDFCFB), unchecked((int)0xFAF9F8F7), unchecked((int)0xF6F5F4F3) }, buffer),
            [0xFE, 0xFD, 0xFC, 0xFB, 0xFA, 0xF9, 0xF8, 0xF7, 0xF6, 0xF5, 0xF4, 0xF3]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianNegativeInt32MaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[20];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new int[] { unchecked((int)0xFEFDFCFB), unchecked((int)0xFAF9F8F7), unchecked((int)0xF6F5F4F3) }, buffer, 2),
            [0xFE, 0xFD, 0xFC, 0xFB, 0xFA, 0xF9, 0xF8, 0xF7]);
    }

    #endregion CopyTo int[] -> byte[] (Negative Bytes)

    #region CopyTo ulong[] -> byte[] (Positive Bytes)

    [Test]
    public async Task CopyTo_ToBytesLittleEndianPositiveUInt64_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new ulong[] { 0x0102030405060708, 0x090A0B0C0D0E0F10, 0x1112131415161718 }, buffer),
            [0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01, 0x10, 0x0F, 0x0E, 0x0D, 0x0C, 0x0B, 0x0A, 0x09, 0x18, 0x17, 0x16, 0x15, 0x14, 0x13, 0x12, 0x11]);
    }

    [Test]
    public async Task CopyTo_ToBytesLittleEndianPositiveUInt64MaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new ulong[] { 0x0102030405060708, 0x090A0B0C0D0E0F10, 0x1112131415161718 }, buffer, 2),
            [0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01, 0x10, 0x0F, 0x0E, 0x0D, 0x0C, 0x0B, 0x0A, 0x09]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianPositiveUInt64_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new ulong[] { 0x0102030405060708, 0x090A0B0C0D0E0F10, 0x1112131415161718 }, buffer),
            [0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianPositiveUInt64MaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new ulong[] { 0x0102030405060708, 0x090A0B0C0D0E0F10, 0x1112131415161718 }, buffer, 2),
            [0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10]);
    }

    #endregion CopyTo ulong[] -> byte[] (Positive Bytes)

    #region CopyTo ulong[] -> byte[] (Negative Bytes)

    [Test]
    public async Task CopyTo_ToBytesLittleEndianNegativeUInt64_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new ulong[] { 0xFEFDFCFBFAF9F8F7, 0xF6F5F4F3F2F1F0EF, 0xF0EEEDECEBEAE9E8 }, buffer),
            [0xF7, 0xF8, 0xF9, 0xFA, 0xFB, 0xFC, 0xFD, 0xFE, 0xEF, 0xF0, 0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xE8, 0xE9, 0xEA, 0xEB, 0xEC, 0xED, 0xEE, 0xF0]);
    }

    [Test]
    public async Task CopyTo_ToBytesLittleEndianNegativeUInt64MaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new ulong[] { 0xFEFDFCFBFAF9F8F7, 0xF6F5F4F3F2F1F0EF, 0xF0EEEDECEBEAE9E8 }, buffer, 2),
            [0xF7, 0xF8, 0xF9, 0xFA, 0xFB, 0xFC, 0xFD, 0xFE, 0xEF, 0xF0, 0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianNegativeUInt64_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new ulong[] { 0xFEFDFCFBFAF9F8F7, 0xF6F5F4F3F2F1F0EF, 0xF0EEEDECEBEAE9E8 }, buffer),
            [0xFE, 0xFD, 0xFC, 0xFB, 0xFA, 0xF9, 0xF8, 0xF7, 0xF6, 0xF5, 0xF4, 0xF3, 0xF2, 0xF1, 0xF0, 0xEF, 0xF0, 0xEE, 0xED, 0xEC, 0xEB, 0xEA, 0xE9, 0xE8]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianNegativeUInt64MaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new ulong[] { 0xFEFDFCFBFAF9F8F7, 0xF6F5F4F3F2F1F0EF, 0xF0EEEDECEBEAE9E8 }, buffer, 2),
            [0xFE, 0xFD, 0xFC, 0xFB, 0xFA, 0xF9, 0xF8, 0xF7, 0xF6, 0xF5, 0xF4, 0xF3, 0xF2, 0xF1, 0xF0, 0xEF]);
    }

    #endregion CopyTo ulong[] -> byte[] (Negative Bytes)

    #region CopyTo long[] -> byte[] (Positive Bytes)

    [Test]
    public async Task CopyTo_ToBytesLittleEndianPositiveInt64_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new long[] { 0x0102030405060708, 0x090A0B0C0D0E0F10, 0x1112131415161718 }, buffer),
            [0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01, 0x10, 0x0F, 0x0E, 0x0D, 0x0C, 0x0B, 0x0A, 0x09, 0x18, 0x17, 0x16, 0x15, 0x14, 0x13, 0x12, 0x11]);
    }

    [Test]
    public async Task CopyTo_ToBytesLittleEndianPositiveInt64MaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new long[] { 0x0102030405060708, 0x090A0B0C0D0E0F10, 0x1112131415161718 }, buffer, 2),
            [0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01, 0x10, 0x0F, 0x0E, 0x0D, 0x0C, 0x0B, 0x0A, 0x09]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianPositiveInt64_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new long[] { 0x0102030405060708, 0x090A0B0C0D0E0F10, 0x1112131415161718 }, buffer),
            [0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianPositiveInt64MaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new long[] { 0x0102030405060708, 0x090A0B0C0D0E0F10, 0x1112131415161718 }, buffer, 2),
            [0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10]);
    }

    #endregion CopyTo long[] -> byte[] (Positive Bytes)

    #region CopyTo long[] -> byte[] (Negative Bytes)

    [Test]
    public async Task CopyTo_ToBytesLittleEndianNegativeInt64_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new long[] { unchecked((long)0xFEFDFCFBFAF9F8F7), unchecked((long)0xF6F5F4F3F2F1F0EF), unchecked((long)0xF0EEEDECEBEAE9E8) }, buffer),
            [0xF7, 0xF8, 0xF9, 0xFA, 0xFB, 0xFC, 0xFD, 0xFE, 0xEF, 0xF0, 0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xE8, 0xE9, 0xEA, 0xEB, 0xEC, 0xED, 0xEE, 0xF0]);
    }

    [Test]
    public async Task CopyTo_ToBytesLittleEndianNegativeInt64MaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new long[] { unchecked((long)0xFEFDFCFBFAF9F8F7), unchecked((long)0xF6F5F4F3F2F1F0EF), unchecked((long)0xF0EEEDECEBEAE9E8) }, buffer, 2),
            [0xF7, 0xF8, 0xF9, 0xFA, 0xFB, 0xFC, 0xFD, 0xFE, 0xEF, 0xF0, 0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianNegativeInt64_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new long[] { unchecked((long)0xFEFDFCFBFAF9F8F7), unchecked((long)0xF6F5F4F3F2F1F0EF), unchecked((long)0xF0EEEDECEBEAE9E8) }, buffer),
            [0xFE, 0xFD, 0xFC, 0xFB, 0xFA, 0xF9, 0xF8, 0xF7, 0xF6, 0xF5, 0xF4, 0xF3, 0xF2, 0xF1, 0xF0, 0xEF, 0xF0, 0xEE, 0xED, 0xEC, 0xEB, 0xEA, 0xE9, 0xE8]);
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianNegativeInt64MaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new long[] { unchecked((long)0xFEFDFCFBFAF9F8F7), unchecked((long)0xF6F5F4F3F2F1F0EF), unchecked((long)0xF0EEEDECEBEAE9E8) }, buffer, 2),
            [0xFE, 0xFD, 0xFC, 0xFB, 0xFA, 0xF9, 0xF8, 0xF7, 0xF6, 0xF5, 0xF4, 0xF3, 0xF2, 0xF1, 0xF0, 0xEF]);
    }

    #endregion CopyTo long[] -> byte[] (Negative Bytes)

    #region CopyTo float[] -> byte[] (Positive Bytes)

    [Test]
    public async Task CopyTo_ToBytesLittleEndianPositiveSingle_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new float[] { 1.23456789f, 1.23456789f, 1.23456789f }, buffer),
            s_positiveSingleLittleEndian.Concat(s_positiveSingleLittleEndian).Concat(s_positiveSingleLittleEndian).ToArray());
    }

    [Test]
    public async Task CopyTo_ToBytesLittleEndianPositiveSingleMaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new float[] { 1.23456789f, 1.23456789f, 1.23456789f }, buffer, 2),
            s_positiveSingleLittleEndian.Concat(s_positiveSingleLittleEndian).ToArray());
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianPositiveSingle_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        byte[] bytesReversed = s_positiveSingleLittleEndian.Reverse().ToArray();
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new float[] { 1.23456789f, 1.23456789f, 1.23456789f }, buffer),
            bytesReversed.Concat(bytesReversed).Concat(bytesReversed).ToArray());
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianPositiveSingleMaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        byte[] bytesReversed = s_positiveSingleLittleEndian.Reverse().ToArray();
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new float[] { 1.23456789f, 1.23456789f, 1.23456789f }, buffer, 2),
            bytesReversed.Concat(bytesReversed).ToArray());
    }

    #endregion CopyTo float[] -> byte[] (Positive Bytes)

    #region CopyTo float[] -> byte[] (Negative Bytes)

    [Test]
    public async Task CopyTo_ToBytesLittleEndianNegativeSingle_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new float[] { -1.23456789f, -1.23456789f, -1.23456789f }, buffer),
            s_negativeSingleLittleEndian.Concat(s_negativeSingleLittleEndian).Concat(s_negativeSingleLittleEndian).ToArray());
    }

    [Test]
    public async Task CopyTo_ToBytesLittleEndianNegativeSingleMaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new float[] { -1.23456789f, -1.23456789f, -1.23456789f }, buffer, 2),
            s_negativeSingleLittleEndian.Concat(s_negativeSingleLittleEndian).ToArray());
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianNegativeSingle_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        byte[] bytesReversed = s_negativeSingleLittleEndian.Reverse().ToArray();
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new float[] { -1.23456789f, -1.23456789f, -1.23456789f }, buffer),
            bytesReversed.Concat(bytesReversed).Concat(bytesReversed).ToArray());
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianNegativeSingleMaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        byte[] bytesReversed = s_negativeSingleLittleEndian.Reverse().ToArray();
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new float[] { -1.23456789f, -1.23456789f, -1.23456789f }, buffer, 2),
            bytesReversed.Concat(bytesReversed).ToArray());
    }

    #endregion CopyTo float[] -> byte[] (Negative Bytes)

    #region CopyTo double[] -> byte[] (Positive Bytes)

    [Test]
    public async Task CopyTo_ToBytesLittleEndianPositiveDouble_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new double[] { 1.23456789d, 1.23456789d, 1.23456789d }, buffer),
            s_positiveDoubleLittleEndian.Concat(s_positiveDoubleLittleEndian).Concat(s_positiveDoubleLittleEndian).ToArray());
    }

    [Test]
    public async Task CopyTo_ToBytesLittleEndianPositiveDoubleMaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new double[] { 1.23456789d, 1.23456789d, 1.23456789d }, buffer, 2),
            s_positiveDoubleLittleEndian.Concat(s_positiveDoubleLittleEndian).ToArray());
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianPositiveDouble_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        byte[] bytesReversed = s_positiveDoubleLittleEndian.Reverse().ToArray();
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new double[] { 1.23456789d, 1.23456789d, 1.23456789d }, buffer),
            bytesReversed.Concat(bytesReversed).Concat(bytesReversed).ToArray());
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianPositiveDoubleMaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        byte[] bytesReversed = s_positiveDoubleLittleEndian.Reverse().ToArray();
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new double[] { 1.23456789d, 1.23456789d, 1.23456789d }, buffer, 2),
            bytesReversed.Concat(bytesReversed).ToArray());
    }

    #endregion CopyTo double[] -> byte[] (Positive Bytes)

    #region CopyTo double[] -> byte[] (Negative Bytes)

    [Test]
    public async Task CopyTo_ToBytesLittleEndianNegativeDouble_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new double[] { -1.23456789d, -1.23456789d, -1.23456789d }, buffer),
            s_negativeDoubleLittleEndian.Concat(s_negativeDoubleLittleEndian).Concat(s_negativeDoubleLittleEndian).ToArray());
    }

    [Test]
    public async Task CopyTo_ToBytesLittleEndianNegativeDoubleMaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        await RunToBytesTest(
            buffer,
            () => ByteConverter.LittleEndian.CopyTo(new double[] { -1.23456789d, -1.23456789d, -1.23456789d }, buffer, 2),
            s_negativeDoubleLittleEndian.Concat(s_negativeDoubleLittleEndian).ToArray());
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianNegativeDouble_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        byte[] bytesReversed = s_negativeDoubleLittleEndian.Reverse().ToArray();
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new double[] { -1.23456789d, -1.23456789d, -1.23456789d }, buffer),
            bytesReversed.Concat(bytesReversed).Concat(bytesReversed).ToArray());
    }

    [Test]
    public async Task CopyTo_ToBytesBigEndianNegativeDoubleMaxBytes_YieldExpectedValue()
    {
        byte[] buffer = new byte[30];
        byte[] bytesReversed = s_negativeDoubleLittleEndian.Reverse().ToArray();
        await RunToBytesTest(
            buffer,
            () => ByteConverter.BigEndian.CopyTo(new double[] { -1.23456789d, -1.23456789d, -1.23456789d }, buffer, 2),
            bytesReversed.Concat(bytesReversed).ToArray());
    }

    #endregion CopyTo double[] -> byte[] (Negative Bytes)

    #region CopyTo byte[] -> bool[] (False and True)

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveBooleanFalseAndTrue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat([0x00, 0x01], sourceCount);
        bool[] destination = new bool[sourceCount * 2 + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination, sourceCount * 2)).IsEqualTo(sourceCount * 2);
        await Assert.That(destination).IsEquivalentTo(new bool[] { false, true, false, true, false, true, false });
    }

    #endregion CopyTo byte[] -> bool[] (False and True)

    #region CopyTo byte[] -> bool[] (Positive Bytes)

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveBooleanWithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..1], sourceCount);
        bool[] destination = new bool[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new bool[] { true, true, true, false });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveBooleanWithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..1], sourceCount);
        bool[] destination = new bool[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new bool[] { true, true, true, false });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveBooleanWithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..1], sourceCount);
        bool[] destination = new bool[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new bool[] { true, true, true, false });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveBooleanWithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..1], sourceCount);
        bool[] destination = new bool[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new bool[] { true, true, true, false });
    }

    #endregion CopyTo byte[] -> bool[] (Positive Bytes)

    #region CopyTo byte[] -> bool[] (Negative Bytes)

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeBooleanWithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..1], sourceCount);
        bool[] destination = new bool[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new bool[] { true, true, true, false });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeBooleanWithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..1], sourceCount);
        bool[] destination = new bool[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new bool[] { true, true, true, false });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeBooleanWithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..1], sourceCount);
        bool[] destination = new bool[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new bool[] { true, true, true, false });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeBooleanWithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..1], sourceCount);
        bool[] destination = new bool[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new bool[] { true, true, true, false });
    }

    #endregion CopyTo byte[] -> bool[] (Negative Bytes)

    #region CopyTo byte[] -> sbyte[] (Positive Bytes)

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveSByteWithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..1], sourceCount);
        sbyte[] destination = new sbyte[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new sbyte[] { 0x01, 0x01, 0x01, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveSByteWithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..1], sourceCount);
        sbyte[] destination = new sbyte[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new sbyte[] { 0x01, 0x01, 0x01, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveSByteWithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..1], sourceCount);
        sbyte[] destination = new sbyte[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new sbyte[] { 0x01, 0x01, 0x01, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveSByteWithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..1], sourceCount);
        sbyte[] destination = new sbyte[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new sbyte[] { 0x01, 0x01, 0x01, 0 });
    }

    #endregion CopyTo byte[] -> sbyte[] (Positive Bytes)

    #region CopyTo byte[] -> sbyte[] (Negative Bytes)

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeSByteWithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..1], sourceCount);
        sbyte[] destination = new sbyte[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new sbyte[] { unchecked((sbyte)0xFE), unchecked((sbyte)0xFE), unchecked((sbyte)0xFE), 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeSByteWithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..1], sourceCount);
        sbyte[] destination = new sbyte[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new sbyte[] { unchecked((sbyte)0xFE), unchecked((sbyte)0xFE), unchecked((sbyte)0xFE), 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeSByteWithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..1], sourceCount);
        sbyte[] destination = new sbyte[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new sbyte[] { unchecked((sbyte)0xFE), unchecked((sbyte)0xFE), unchecked((sbyte)0xFE), 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeSByteWithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..1], sourceCount);
        sbyte[] destination = new sbyte[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new sbyte[] { unchecked((sbyte)0xFE), unchecked((sbyte)0xFE), unchecked((sbyte)0xFE), 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeSBytePartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..1], sourceCount)[0..^1];
        sbyte[] destination = new sbyte[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new sbyte[] { unchecked((sbyte)0xFE), unchecked((sbyte)0xFE), 0, 0 });
    }

    #endregion CopyTo byte[] -> sbyte[] (Negative Bytes)

    #region CopyTo byte[] -> ushort[] (Positive Bytes)

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveUInt16WithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..2], sourceCount);
        ushort[] destination = new ushort[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new ushort[] { 0x0201, 0x0201, 0x0201, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveUInt16WithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..2], sourceCount);
        ushort[] destination = new ushort[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new ushort[] { 0x0201, 0x0201, 0x0201, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveUInt16PartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..2], sourceCount)[0..^1];
        ushort[] destination = new ushort[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new ushort[] { 0x0201, 0x0201, 0, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveUInt16WithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..2], sourceCount);
        ushort[] destination = new ushort[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new ushort[] { 0x0102, 0x0102, 0x0102, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveUInt16WithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..2], sourceCount);
        ushort[] destination = new ushort[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new ushort[] { 0x0102, 0x0102, 0x0102, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveUInt16PartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..2], sourceCount)[0..^1];
        ushort[] destination = new ushort[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new ushort[] { 0x0102, 0x0102, 0, 0 });
    }

    #endregion CopyTo byte[] -> ushort[] (Positive Bytes)

    #region CopyTo byte[] -> ushort[] (Negative Bytes)

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeUInt16WithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..2], sourceCount);
        ushort[] destination = new ushort[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new ushort[] { 0xFDFE, 0xFDFE, 0xFDFE, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeUInt16WithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..2], sourceCount);
        ushort[] destination = new ushort[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new ushort[] { 0xFDFE, 0xFDFE, 0xFDFE, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeUInt16PartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..2], sourceCount)[0..^1];
        ushort[] destination = new ushort[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new ushort[] { 0xFDFE, 0xFDFE, 0, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeUInt16WithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..2], sourceCount);
        ushort[] destination = new ushort[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new ushort[] { 0xFEFD, 0xFEFD, 0xFEFD, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeUInt16WithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..2], sourceCount);
        ushort[] destination = new ushort[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new ushort[] { 0xFEFD, 0xFEFD, 0xFEFD, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeUInt16PartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..2], sourceCount)[0..^1];
        ushort[] destination = new ushort[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new ushort[] { 0xFEFD, 0xFEFD, 0, 0 });
    }

    #endregion CopyTo byte[] -> ushort[] (Negative Bytes)

    #region CopyTo byte[] -> short[] (Positive Bytes)

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveInt16WithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..2], sourceCount);
        short[] destination = new short[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new short[] { 0x0201, 0x0201, 0x0201, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveInt16WithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..2], sourceCount);
        short[] destination = new short[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new short[] { 0x0201, 0x0201, 0x0201, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveInt16PartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..2], sourceCount)[0..^1];
        short[] destination = new short[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new short[] { 0x0201, 0x0201, 0, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveInt16WithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..2], sourceCount);
        short[] destination = new short[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new short[] { 0x0102, 0x0102, 0x0102, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveInt16WithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..2], sourceCount);
        short[] destination = new short[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new short[] { 0x0102, 0x0102, 0x0102, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveInt16PartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..2], sourceCount)[0..^1];
        short[] destination = new short[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new short[] { 0x0102, 0x0102, 0, 0 });
    }

    #endregion CopyTo byte[] -> short[] (Positive Bytes)

    #region CopyTo byte[] -> short[] (Negative Bytes)

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeInt16WithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..2], sourceCount);
        short[] destination = new short[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new short[] { unchecked((short)0xFDFE), unchecked((short)0xFDFE), unchecked((short)0xFDFE), 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeInt16WithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..2], sourceCount);
        short[] destination = new short[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new short[] { unchecked((short)0xFDFE), unchecked((short)0xFDFE), unchecked((short)0xFDFE), 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeInt16PartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..2], sourceCount)[0..^1];
        short[] destination = new short[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new short[] { unchecked((short)0xFDFE), unchecked((short)0xFDFE), 0, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeInt16WithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..2], sourceCount);
        short[] destination = new short[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new short[] { unchecked((short)0xFEFD), unchecked((short)0xFEFD), unchecked((short)0xFEFD), 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeInt16WithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..2], sourceCount);
        short[] destination = new short[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new short[] { unchecked((short)0xFEFD), unchecked((short)0xFEFD), unchecked((short)0xFEFD), 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeInt16PartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..2], sourceCount)[0..^1];
        short[] destination = new short[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new short[] { unchecked((short)0xFEFD), unchecked((short)0xFEFD), 0, 0 });
    }

    #endregion CopyTo byte[] -> short[] (Negative Bytes)

    #region CopyTo byte[] -> uint[] (Positive Bytes)

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveUInt32WithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..4], sourceCount);
        uint[] destination = new uint[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new uint[] { 0x04030201, 0x04030201, 0x04030201, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveUInt32WithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..4], sourceCount);
        uint[] destination = new uint[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new uint[] { 0x04030201, 0x04030201, 0x04030201, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveUInt32PartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..4], sourceCount)[0..^1];
        uint[] destination = new uint[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new uint[] { 0x04030201, 0x04030201, 0, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveUInt32WithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..4], sourceCount);
        uint[] destination = new uint[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new uint[] { 0x01020304, 0x01020304, 0x01020304, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveUInt32WithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..4], sourceCount);
        uint[] destination = new uint[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new uint[] { 0x01020304, 0x01020304, 0x01020304, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveUInt32PartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..4], sourceCount)[0..^1];
        uint[] destination = new uint[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new uint[] { 0x01020304, 0x01020304, 0, 0 });
    }

    #endregion CopyTo byte[] -> uint[] (Positive Bytes)

    #region CopyTo byte[] -> uint[] (Negative Bytes)

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeUInt32WithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..4], sourceCount);
        uint[] destination = new uint[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new uint[] { 0xFBFCFDFE, 0xFBFCFDFE, 0xFBFCFDFE, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeUInt32WithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..4], sourceCount);
        uint[] destination = new uint[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new uint[] { 0xFBFCFDFE, 0xFBFCFDFE, 0xFBFCFDFE, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeUInt32PartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..4], sourceCount)[0..^1];
        uint[] destination = new uint[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new uint[] { 0xFBFCFDFE, 0xFBFCFDFE, 0, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeUInt32WithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..4], sourceCount);
        uint[] destination = new uint[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new uint[] { 0xFEFDFCFB, 0xFEFDFCFB, 0xFEFDFCFB, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeUInt32WithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..4], sourceCount);
        uint[] destination = new uint[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new uint[] { 0xFEFDFCFB, 0xFEFDFCFB, 0xFEFDFCFB, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeUInt32PartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..4], sourceCount)[0..^1];
        uint[] destination = new uint[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new uint[] { 0xFEFDFCFB, 0xFEFDFCFB, 0, 0 });
    }

    #endregion CopyTo byte[] -> uint[] (Negative Bytes)

    #region CopyTo byte[] -> int[] (Positive Bytes)

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveInt32WithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..4], sourceCount);
        int[] destination = new int[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new int[] { 0x04030201, 0x04030201, 0x04030201, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveInt32WithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..4], sourceCount);
        int[] destination = new int[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new int[] { 0x04030201, 0x04030201, 0x04030201, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveInt32PartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..4], sourceCount)[0..^1];
        int[] destination = new int[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new int[] { 0x04030201, 0x04030201, 0, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveInt32WithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..4], sourceCount);
        int[] destination = new int[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new int[] { 0x01020304, 0x01020304, 0x01020304, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveInt32WithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..4], sourceCount);
        int[] destination = new int[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new int[] { 0x01020304, 0x01020304, 0x01020304, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveInt32PartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..4], sourceCount)[0..^1];
        int[] destination = new int[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new int[] { 0x01020304, 0x01020304, 0, 0 });
    }

    #endregion CopyTo byte[] -> int[] (Positive Bytes)

    #region CopyTo byte[] -> int[] (Negative Bytes)

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeInt32WithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..4], sourceCount);
        int[] destination = new int[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new int[] { unchecked((int)0xFBFCFDFE), unchecked((int)0xFBFCFDFE), unchecked((int)0xFBFCFDFE), 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeInt32WithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..4], sourceCount);
        int[] destination = new int[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new int[] { unchecked((int)0xFBFCFDFE), unchecked((int)0xFBFCFDFE), unchecked((int)0xFBFCFDFE), 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeInt32PartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..4], sourceCount)[0..^1];
        int[] destination = new int[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new int[] { unchecked((int)0xFBFCFDFE), unchecked((int)0xFBFCFDFE), 0, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeInt32WithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..4], sourceCount);
        int[] destination = new int[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new int[] { unchecked((int)0xFEFDFCFB), unchecked((int)0xFEFDFCFB), unchecked((int)0xFEFDFCFB), 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeInt32WithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..4], sourceCount);
        int[] destination = new int[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new int[] { unchecked((int)0xFEFDFCFB), unchecked((int)0xFEFDFCFB), unchecked((int)0xFEFDFCFB), 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeInt32PartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..4], sourceCount)[0..^1];
        int[] destination = new int[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new int[] { unchecked((int)0xFEFDFCFB), unchecked((int)0xFEFDFCFB), 0, 0 });
    }

    #endregion CopyTo byte[] -> int[] (Negative Bytes)

    #region CopyTo byte[] -> ulong[] (Positive Bytes)

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveUInt64WithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..8], sourceCount);
        ulong[] destination = new ulong[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new ulong[] { 0x0807060504030201, 0x0807060504030201, 0x0807060504030201, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveUInt64WithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..8], sourceCount);
        ulong[] destination = new ulong[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new ulong[] { 0x0807060504030201, 0x0807060504030201, 0x0807060504030201, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveUInt64PartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..8], sourceCount)[0..^1];
        ulong[] destination = new ulong[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new ulong[] { 0x0807060504030201, 0x0807060504030201, 0, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveUInt64WithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..8], sourceCount);
        ulong[] destination = new ulong[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new ulong[] { 0x0102030405060708, 0x0102030405060708, 0x0102030405060708, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveUInt64WithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..8], sourceCount);
        ulong[] destination = new ulong[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new ulong[] { 0x0102030405060708, 0x0102030405060708, 0x0102030405060708, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveUInt64PartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..8], sourceCount)[0..^1];
        ulong[] destination = new ulong[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new ulong[] { 0x0102030405060708, 0x0102030405060708, 0, 0 });
    }

    #endregion CopyTo byte[] -> ulong[] (Positive Bytes)

    #region CopyTo byte[] -> ulong[] (Negative Bytes)

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeUInt64WithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..8], sourceCount);
        ulong[] destination = new ulong[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new ulong[] { 0xF7F8F9FAFBFCFDFE, 0xF7F8F9FAFBFCFDFE, 0xF7F8F9FAFBFCFDFE, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeUInt64WithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..8], sourceCount);
        ulong[] destination = new ulong[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new ulong[] { 0xF7F8F9FAFBFCFDFE, 0xF7F8F9FAFBFCFDFE, 0xF7F8F9FAFBFCFDFE, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeUInt64PartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..8], sourceCount)[0..^1];
        ulong[] destination = new ulong[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new ulong[] { 0xF7F8F9FAFBFCFDFE, 0xF7F8F9FAFBFCFDFE, 0, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeUInt64WithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..8], sourceCount);
        ulong[] destination = new ulong[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new ulong[] { 0xFEFDFCFBFAF9F8F7, 0xFEFDFCFBFAF9F8F7, 0xFEFDFCFBFAF9F8F7, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeUInt64WithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..8], sourceCount);
        ulong[] destination = new ulong[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new ulong[] { 0xFEFDFCFBFAF9F8F7, 0xFEFDFCFBFAF9F8F7, 0xFEFDFCFBFAF9F8F7, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeUInt64PartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..8], sourceCount)[0..^1];
        ulong[] destination = new ulong[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new ulong[] { 0xFEFDFCFBFAF9F8F7, 0xFEFDFCFBFAF9F8F7, 0, 0 });
    }

    #endregion CopyTo byte[] -> ulong[] (Negative Bytes)

    #region CopyTo byte[] -> long[] (Positive Bytes)

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveInt64WithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..8], sourceCount);
        long[] destination = new long[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new long[] { 0x0807060504030201, 0x0807060504030201, 0x0807060504030201, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveInt64WithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..8], sourceCount);
        long[] destination = new long[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new long[] { 0x0807060504030201, 0x0807060504030201, 0x0807060504030201, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveInt64PartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..8], sourceCount)[0..^1];
        long[] destination = new long[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new long[] { 0x0807060504030201, 0x0807060504030201, 0, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveInt64WithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..8], sourceCount);
        long[] destination = new long[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new long[] { 0x0102030405060708, 0x0102030405060708, 0x0102030405060708, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveInt64WithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..8], sourceCount);
        long[] destination = new long[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new long[] { 0x0102030405060708, 0x0102030405060708, 0x0102030405060708, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveInt64PartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveIntegerData[0..8], sourceCount)[0..^1];
        long[] destination = new long[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new long[] { 0x0102030405060708, 0x0102030405060708, 0, 0 });
    }

    #endregion CopyTo byte[] -> long[] (Positive Bytes)

    #region CopyTo byte[] -> long[] (Negative Bytes)

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeInt64WithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..8], sourceCount);
        long[] destination = new long[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new long[] { unchecked((long)0xF7F8F9FAFBFCFDFE), unchecked((long)0xF7F8F9FAFBFCFDFE), unchecked((long)0xF7F8F9FAFBFCFDFE), 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeInt64WithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..8], sourceCount);
        long[] destination = new long[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new long[] { unchecked((long)0xF7F8F9FAFBFCFDFE), unchecked((long)0xF7F8F9FAFBFCFDFE), unchecked((long)0xF7F8F9FAFBFCFDFE), 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeInt64PartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..8], sourceCount)[0..^1];
        long[] destination = new long[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new long[] { unchecked((long)0xF7F8F9FAFBFCFDFE), unchecked((long)0xF7F8F9FAFBFCFDFE), 0, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeInt64WithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..8], sourceCount);
        long[] destination = new long[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new long[] { unchecked((long)0xFEFDFCFBFAF9F8F7), unchecked((long)0xFEFDFCFBFAF9F8F7), unchecked((long)0xFEFDFCFBFAF9F8F7), 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeInt64WithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..8], sourceCount);
        long[] destination = new long[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new long[] { unchecked((long)0xFEFDFCFBFAF9F8F7), unchecked((long)0xFEFDFCFBFAF9F8F7), unchecked((long)0xFEFDFCFBFAF9F8F7), 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeInt64PartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeIntegerData[0..8], sourceCount)[0..^1];
        long[] destination = new long[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new long[] { unchecked((long)0xFEFDFCFBFAF9F8F7), unchecked((long)0xFEFDFCFBFAF9F8F7), 0, 0 });
    }

    #endregion CopyTo byte[] -> long[] (Negative Bytes)

    #region CopyTo byte[] -> float[] (Positive Bytes)

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveSingleWithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveSingleLittleEndian, sourceCount);
        float[] destination = new float[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new float[] { 1.23456789f, 1.23456789f, 1.23456789f, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveSingleWithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveSingleLittleEndian, sourceCount);
        float[] destination = new float[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new float[] { 1.23456789f, 1.23456789f, 1.23456789f, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveSinglePartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveSingleLittleEndian, sourceCount)[0..^1];
        float[] destination = new float[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new float[] { 1.23456789f, 1.23456789f, 0, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveSingleWithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveSingleLittleEndian.Reverse().ToArray(), sourceCount);
        float[] destination = new float[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new float[] { 1.23456789f, 1.23456789f, 1.23456789f, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveSingleWithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveSingleLittleEndian.Reverse().ToArray(), sourceCount);
        float[] destination = new float[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new float[] { 1.23456789f, 1.23456789f, 1.23456789f, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveSinglePartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveSingleLittleEndian.Reverse().ToArray(), sourceCount)[0..^1];
        float[] destination = new float[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new float[] { 1.23456789f, 1.23456789f, 0, 0 });
    }

    #endregion CopyTo byte[] -> float[] (Positive Bytes)

    #region CopyTo byte[] -> float[] (Negative Bytes)

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeSingleWithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeSingleLittleEndian, sourceCount);
        float[] destination = new float[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new float[] { -1.23456789f, -1.23456789f, -1.23456789f, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeSingleWithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeSingleLittleEndian, sourceCount);
        float[] destination = new float[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new float[] { -1.23456789f, -1.23456789f, -1.23456789f, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeSinglePartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeSingleLittleEndian, sourceCount)[0..^1];
        float[] destination = new float[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new float[] { -1.23456789f, -1.23456789f, 0, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeSingleWithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeSingleLittleEndian.Reverse().ToArray(), sourceCount);
        float[] destination = new float[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new float[] { -1.23456789f, -1.23456789f, -1.23456789f, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeSingleWithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeSingleLittleEndian.Reverse().ToArray(), sourceCount);
        float[] destination = new float[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new float[] { -1.23456789f, -1.23456789f, -1.23456789f, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeSinglePartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeSingleLittleEndian.Reverse().ToArray(), sourceCount)[0..^1];
        float[] destination = new float[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new float[] { -1.23456789f, -1.23456789f, 0, 0 });
    }

    #endregion CopyTo byte[] -> float[] (Negative Bytes)

    #region CopyTo byte[] -> double[] (Positive Bytes)

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveDoubleWithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveDoubleLittleEndian, sourceCount);
        double[] destination = new double[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new double[] { 1.23456789d, 1.23456789d, 1.23456789d, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveDoubleWithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveDoubleLittleEndian, sourceCount);
        double[] destination = new double[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new double[] { 1.23456789d, 1.23456789d, 1.23456789d, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianPositiveDoublePartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveDoubleLittleEndian, sourceCount)[0..^1];
        double[] destination = new double[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new double[] { 1.23456789d, 1.23456789d, 0, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveDoubleWithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveDoubleLittleEndian.Reverse().ToArray(), sourceCount);
        double[] destination = new double[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new double[] { 1.23456789d, 1.23456789d, 1.23456789d, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveDoubleWithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveDoubleLittleEndian.Reverse().ToArray(), sourceCount);
        double[] destination = new double[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new double[] { 1.23456789d, 1.23456789d, 1.23456789d, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianPositiveDoublePartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_positiveDoubleLittleEndian.Reverse().ToArray(), sourceCount)[0..^1];
        double[] destination = new double[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new double[] { 1.23456789d, 1.23456789d, 0, 0 });
    }

    #endregion CopyTo byte[] -> double[] (Positive Bytes)

    #region CopyTo byte[] -> double[] (Negative Bytes)

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeDoubleWithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeDoubleLittleEndian, sourceCount);
        double[] destination = new double[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new double[] { -1.23456789d, -1.23456789d, -1.23456789d, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeDoubleWithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeDoubleLittleEndian, sourceCount);
        double[] destination = new double[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new double[] { -1.23456789d, -1.23456789d, -1.23456789d, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesLittleEndianNegativeDoublePartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeDoubleLittleEndian, sourceCount)[0..^1];
        double[] destination = new double[sourceCount + 1];
        await Assert.That(ByteConverter.LittleEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new double[] { -1.23456789d, -1.23456789d, 0, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeDoubleWithCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeDoubleLittleEndian.Reverse().ToArray(), sourceCount);
        double[] destination = new double[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination, sourceCount)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new double[] { -1.23456789d, -1.23456789d, -1.23456789d, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeDoubleWithoutCount_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeDoubleLittleEndian.Reverse().ToArray(), sourceCount);
        double[] destination = new double[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount);
        await Assert.That(destination).IsEquivalentTo(new double[] { -1.23456789d, -1.23456789d, -1.23456789d, 0 });
    }

    [Test]
    public async Task CopyTo_FromBytesBigEndianNegativeDoublePartialValue_YieldExpectedValue()
    {
        const int sourceCount = 3;
        byte[] source = Repeat(s_negativeDoubleLittleEndian.Reverse().ToArray(), sourceCount)[0..^1];
        double[] destination = new double[sourceCount + 1];
        await Assert.That(ByteConverter.BigEndian.CopyTo(source, destination)).IsEqualTo(sourceCount - 1);
        await Assert.That(destination).IsEquivalentTo(new double[] { -1.23456789d, -1.23456789d, 0, 0 });
    }

    #endregion CopyTo byte[] -> double[] (Negative Bytes)

    private static byte[] Repeat(byte[] buffer, int count)
    {
        byte[] newBuffer = new byte[buffer.Length * count];
        for (int i = 0; i < count; i++)
        {
            buffer.CopyTo(newBuffer, i * buffer.Length);
        }
        return newBuffer;
    }

    private static async Task RunToBytesTest(byte[] buffer, Action convertAction, byte[] expectedBuffer)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = 0;
        }

        convertAction();

        byte[] testBuffer = new byte[buffer.Length];

        Buffer.BlockCopy(expectedBuffer, 0, testBuffer, 0, expectedBuffer.Length);

        await Assert.That(buffer).IsEquivalentTo(testBuffer);
    }
}

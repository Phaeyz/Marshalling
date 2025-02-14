using System.Text;

namespace Phaeyz.Marshalling.Test;

internal class EncodingExtensionsTest
{
    [Test]
    public async Task GetMinimumCodeUnitSize_CommonEncodings_YieldExpectedValues()
    {
        await Assert.That(Encoding.ASCII.GetMinimumCodeUnitSize()).IsEqualTo(1);
#pragma warning disable SYSLIB0001 // Type or member is obsolete
        await Assert.That(Encoding.UTF7.GetMinimumCodeUnitSize()).IsEqualTo(1);
#pragma warning restore SYSLIB0001 // Type or member is obsolete
        await Assert.That(Encoding.UTF8.GetMinimumCodeUnitSize()).IsEqualTo(1);
        await Assert.That(Encoding.Unicode.GetMinimumCodeUnitSize()).IsEqualTo(2);
        await Assert.That(Encoding.UTF32.GetMinimumCodeUnitSize()).IsEqualTo(4);
    }

    [Test]
    public async Task GetMinimumCodeUnitSize_Latin1_Returns1()
    {
        const int codePage_ISO_8859_1 = 28591; // Latin1
        await Assert.That(new TestEncoding(codePage_ISO_8859_1, 0).GetMinimumCodeUnitSize()).IsEqualTo(1);
    }

    [Test]
    public async Task GetMinimumCodeUnitSize_CustomEncoding_ReturnsExpectedValue()
    {
        await Assert.That(new TestEncoding(-1, 5).GetMinimumCodeUnitSize()).IsEqualTo(5);
    }

    [Test]
    public async Task GetNullTerminatedString_NullTerminatorNotFound_UsesWholeBufferForString()
    {
        ReadOnlySpan<byte> buffer = [0x61, 0x62, 0x63, 0x64, 0x65];
        await Assert.That(Encoding.ASCII.GetNullTerminatedString(buffer, out int nullTerminatorIndex)).IsEqualTo("abcde");
        await Assert.That(nullTerminatorIndex).IsEqualTo(-1);
    }

    [Test]
    public async Task GetNullTerminatedString_NullTerminatorInMiddleOfBuffer_ReturnsStringUntilNil()
    {
        ReadOnlySpan<byte> buffer = [0x61, 0x62, 0, 0x63, 0x64, 0x65];
        await Assert.That(Encoding.ASCII.GetNullTerminatedString(buffer, out int nullTerminatorIndex)).IsEqualTo("ab");
        await Assert.That(nullTerminatorIndex).IsEqualTo(2);
    }

    [Test]
    public async Task GetNullTerminatedString_NullTerminatorAtStartOfBuffer_ReturnsEmptyString()
    {
        ReadOnlySpan<byte> buffer = [0, 0x61, 0x62, 0x63, 0x64, 0x65];
        await Assert.That(Encoding.ASCII.GetNullTerminatedString(buffer, out int nullTerminatorIndex)).IsEqualTo(string.Empty);
        await Assert.That(nullTerminatorIndex).IsEqualTo(0);
    }

    [Test]
    public async Task GetNullTerminatedString_EmptyBuffer_ReturnsEmptyString()
    {
        await Assert.That(Encoding.ASCII.GetNullTerminatedString(ReadOnlySpan<byte>.Empty, out int nullTerminatorIndex)).IsEqualTo(string.Empty);
        await Assert.That(nullTerminatorIndex).IsEqualTo(-1);
    }
}

file class TestEncoding(int codePage, int byteCountToReturn) : Encoding
{
    public override int CodePage => codePage;
    public override int GetByteCount(char[] chars, int index, int count) => byteCountToReturn;
    public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex) => throw new NotImplementedException();
    public override int GetCharCount(byte[] bytes, int index, int count) => throw new NotImplementedException();
    public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex) => throw new NotImplementedException();
    public override int GetMaxByteCount(int charCount) => throw new NotImplementedException();
    public override int GetMaxCharCount(int byteCount) => throw new NotImplementedException();
}
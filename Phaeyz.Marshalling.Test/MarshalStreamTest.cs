using System.Text;

namespace Phaeyz.Marshalling.Test;

internal class MarshalStreamTest
{
    private static readonly byte[] s_sourceBuffer = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
    private const int c_defaultStreamBufferSize = 3;

    private static MarshalStream CreateForFixedBuffer() => new(s_sourceBuffer);

    private static MarshalStream CreateForStream(bool canRead, bool canSeek, bool canWrite, int bufferSize = c_defaultStreamBufferSize) =>
        new(new CustomMemoryStream(s_sourceBuffer, canRead, canSeek, canWrite), true, bufferSize);

    private static IEnumerable<MarshalStream> Iterate(params MarshalStream[] streams)
    {
        foreach (var stream in streams)
        {
            yield return stream;
            stream.Dispose();
        }
    }

    private static byte[] Slice(byte[] source, int startIndex, int? length = null) =>
        source[startIndex..(startIndex + (length ?? (source.Length - startIndex)))];

    #region AddReadProcessor

    [Test]
    public async Task AddReadProcessor_ForFixedBuffer_BecomesNonSeekableReadOnly()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.CanRead).IsTrue();
        await Assert.That(stream.CanWrite).IsFalse();
        await Assert.That(stream.CanSeek).IsTrue();
        using (stream.AddReadProcessor(new CustomProcessor(bytes => { })))
        {
            await Assert.That(stream.CanRead).IsTrue();
            await Assert.That(stream.CanWrite).IsFalse();
            await Assert.That(stream.CanSeek).IsFalse();
        }
        await Assert.That(stream.CanRead).IsTrue();
        await Assert.That(stream.CanWrite).IsFalse();
        await Assert.That(stream.CanSeek).IsTrue();
    }

    [Test]
    public async Task AddReadProcessor_ForFixedBufferCopyTo_ProcessesBytesCorrectly()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        ValidationProcessor processor = new(s_sourceBuffer.AsMemory());
        using (stream.AddReadProcessor(processor))
        {
            stream.CopyTo(new MemoryStream());
        }
        await Assert.That(processor.Success).IsTrue();
    }

    [Test]
    public async Task AddReadProcessor_ForFixedBufferCopyToAsync_ProcessesBytesCorrectly()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        ValidationProcessor processor = new(s_sourceBuffer.AsMemory());
        using (stream.AddReadProcessor(processor))
        {
            await stream.CopyToAsync(new MemoryStream());
        }
        await Assert.That(processor.Success).IsTrue();
    }

    [Test]
    public async Task AddReadProcessor_ForFixedBufferRead_ProcessesBytesCorrectly()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        ValidationProcessor processor = new(s_sourceBuffer.AsMemory());
        using (stream.AddReadProcessor(processor))
        {
            byte[] buffer = new byte[s_sourceBuffer.Length];
            stream.ReadExactly(buffer.AsSpan());
        }
        await Assert.That(processor.Success).IsTrue();
    }

    [Test]
    public async Task AddReadProcessor_ForFixedBufferReadAsync_ProcessesBytesCorrectly()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        ValidationProcessor processor = new(s_sourceBuffer.AsMemory());
        using (stream.AddReadProcessor(processor))
        {
            byte[] buffer = new byte[s_sourceBuffer.Length];
            await stream.ReadExactlyAsync(buffer.AsMemory());
        }
        await Assert.That(processor.Success).IsTrue();
    }

    [Test]
    public async Task AddReadProcessor_ForFixedBufferReadString_ProcessesBytesCorrectly()
    {
        const string testString = "testing123";
        byte[] bytes = Encoding.UTF8.GetBytes(testString + "\0\0\0");
        using MarshalStream stream = new(bytes);
        ValidationProcessor processor = new(bytes.AsMemory()[..(testString.Length + 1)]);
        using (stream.AddReadProcessor(processor))
        {
            byte[] buffer = new byte[bytes.Length];
            stream.ReadString(Encoding.UTF8, -1, MarshalStreamNullTerminatorBehavior.Stop);
        }
        await Assert.That(processor.Success).IsTrue();
    }

    [Test]
    public async Task AddReadProcessor_ForFixedBufferReadStringAsync_ProcessesBytesCorrectly()
    {
        const string testString = "testing123";
        byte[] bytes = Encoding.UTF8.GetBytes(testString + "\0\0\0");
        using MarshalStream stream = new(bytes);
        ValidationProcessor processor = new(bytes.AsMemory()[..(testString.Length + 1)]);
        using (stream.AddReadProcessor(processor))
        {
            byte[] buffer = new byte[bytes.Length];
            await stream.ReadStringAsync(Encoding.UTF8, -1, MarshalStreamNullTerminatorBehavior.Stop);
        }
        await Assert.That(processor.Success).IsTrue();
    }

    [Test]
    public async Task AddReadProcessor_ForFixedBufferScan_ProcessesBytesCorrectly()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        ValidationProcessor processor = new(s_sourceBuffer.AsMemory());
        using (stream.AddReadProcessor(processor))
        {
            stream.Scan(1, -1, b => 1);
        }
        await Assert.That(processor.Success).IsTrue();
    }

    [Test]
    public async Task AddReadProcessor_ForFixedBufferScanAsync_ProcessesBytesCorrectly()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        ValidationProcessor processor = new(s_sourceBuffer.AsMemory());
        using (stream.AddReadProcessor(processor))
        {
            await stream.ScanAsync(1, -1, b => 1);
        }
        await Assert.That(processor.Success).IsTrue();
    }

    [Test]
    public async Task AddReadProcessor_ForFixedBufferSkip_ProcessesBytesCorrectly()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        ValidationProcessor processor = new(s_sourceBuffer.AsMemory());
        using (stream.AddReadProcessor(processor))
        {
            stream.Skip(s_sourceBuffer.Length);
        }
        await Assert.That(processor.Success).IsTrue();
    }

    [Test]
    public async Task AddReadProcessor_ForFixedBufferSkipAsync_ProcessesBytesCorrectly()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        ValidationProcessor processor = new(s_sourceBuffer.AsMemory());
        using (stream.AddReadProcessor(processor))
        {
            await stream.SkipAsync(s_sourceBuffer.Length);
        }
        await Assert.That(processor.Success).IsTrue();
    }

    [Test]
    public async Task AddReadProcessor_ForStream_BecomesNonSeekableReadOnly()
    {
        using MarshalStream stream = CreateForStream(true, true, true);
        await Assert.That(stream.CanRead).IsTrue();
        await Assert.That(stream.CanWrite).IsTrue();
        await Assert.That(stream.CanSeek).IsTrue();
        using (stream.AddReadProcessor(new CustomProcessor(bytes => { })))
        {
            await Assert.That(stream.CanRead).IsTrue();
            await Assert.That(stream.CanWrite).IsFalse();
            await Assert.That(stream.CanSeek).IsFalse();
        }
        await Assert.That(stream.CanRead).IsTrue();
        await Assert.That(stream.CanWrite).IsTrue();
        await Assert.That(stream.CanSeek).IsTrue();
    }

    [Test]
    public void AddReadProcessor_ForStreamCalledTwice_Throws()
    {
        using MarshalStream stream = CreateForStream(true, true, true);
        CustomProcessor processor = new(bytes => { });
        stream.AddReadProcessor(processor);
        Assert.Throws<ArgumentException>(() => stream.AddReadProcessor(processor));
    }

    [Test]
    public async Task AddReadProcessor_ForStreamCopyTo_ProcessesBytesCorrectly()
    {
        using MarshalStream stream = CreateForStream(true, true, true);
        ValidationProcessor processor = new(s_sourceBuffer.AsMemory());
        using (stream.AddReadProcessor(processor))
        {
            stream.CopyTo(new MemoryStream());
        }
        await Assert.That(processor.Success).IsTrue();
    }

    [Test]
    public async Task AddReadProcessor_ForStreamCopyToAsync_ProcessesBytesCorrectly()
    {
        using MarshalStream stream = CreateForStream(true, true, true);
        ValidationProcessor processor = new(s_sourceBuffer.AsMemory());
        using (stream.AddReadProcessor(processor))
        {
            await stream.CopyToAsync(new MemoryStream());
        }
        await Assert.That(processor.Success).IsTrue();
    }

    [Test]
    public async Task AddReadProcessor_ForStreamRead_ProcessesBytesCorrectly()
    {
        using MarshalStream stream = CreateForStream(true, true, true);
        ValidationProcessor processor = new(s_sourceBuffer.AsMemory());
        using (stream.AddReadProcessor(processor))
        {
            byte[] buffer = new byte[s_sourceBuffer.Length];
            stream.ReadExactly(buffer.AsSpan());
        }
        await Assert.That(processor.Success).IsTrue();
    }

    [Test]
    public async Task AddReadProcessor_ForStreamReadAsync_ProcessesBytesCorrectly()
    {
        using MarshalStream stream = CreateForStream(true, true, true);
        ValidationProcessor processor = new(s_sourceBuffer.AsMemory());
        using (stream.AddReadProcessor(processor))
        {
            byte[] buffer = new byte[s_sourceBuffer.Length];
            await stream.ReadExactlyAsync(buffer.AsMemory());
        }
        await Assert.That(processor.Success).IsTrue();
    }

    [Test]
    public async Task AddReadProcessor_ForStreamReadString_ProcessesBytesCorrectly()
    {
        const string testString = "testing123";
        byte[] bytes = Encoding.UTF8.GetBytes(testString + "\0\0\0");
        using MarshalStream stream = new(new CustomMemoryStream(bytes, true, true, true), true);
        ValidationProcessor processor = new(bytes.AsMemory()[..(testString.Length + 1)]);
        using (stream.AddReadProcessor(processor))
        {
            byte[] buffer = new byte[bytes.Length];
            stream.ReadString(Encoding.UTF8, -1, MarshalStreamNullTerminatorBehavior.Stop);
        }
        await Assert.That(processor.Success).IsTrue();
    }

    [Test]
    public async Task AddReadProcessor_ForStreamReadStringAsync_ProcessesBytesCorrectly()
    {
        const string testString = "testing123";
        byte[] bytes = Encoding.UTF8.GetBytes(testString + "\0\0\0");
        using MarshalStream stream = new(new CustomMemoryStream(bytes, true, true, true), true);
        ValidationProcessor processor = new(bytes.AsMemory()[..(testString.Length + 1)]);
        using (stream.AddReadProcessor(processor))
        {
            byte[] buffer = new byte[bytes.Length];
            await stream.ReadStringAsync(Encoding.UTF8, -1, MarshalStreamNullTerminatorBehavior.Stop);
        }
        await Assert.That(processor.Success).IsTrue();
    }

    [Test]
    public async Task AddReadProcessor_ForStreamScan_ProcessesBytesCorrectly()
    {
        using MarshalStream stream = CreateForStream(true, true, true);
        ValidationProcessor processor = new(s_sourceBuffer.AsMemory());
        using (stream.AddReadProcessor(processor))
        {
            stream.Scan(1, -1, b => 1);
        }
        await Assert.That(processor.Success).IsTrue();
    }

    [Test]
    public async Task AddReadProcessor_ForStreamScanAsync_ProcessesBytesCorrectly()
    {
        using MarshalStream stream = CreateForStream(true, true, true);
        ValidationProcessor processor = new(s_sourceBuffer.AsMemory());
        using (stream.AddReadProcessor(processor))
        {
            await stream.ScanAsync(1, -1, b => 1);
        }
        await Assert.That(processor.Success).IsTrue();
    }

    [Test]
    public async Task AddReadProcessor_ForStreamSkip_ProcessesBytesCorrectly()
    {
        using MarshalStream stream = CreateForStream(true, true, true);
        ValidationProcessor processor = new(s_sourceBuffer.AsMemory());
        using (stream.AddReadProcessor(processor))
        {
            stream.Skip(s_sourceBuffer.Length);
        }
        await Assert.That(processor.Success).IsTrue();
    }

    [Test]
    public async Task AddReadProcessor_ForStreamSkipAsync_ProcessesBytesCorrectly()
    {
        using MarshalStream stream = CreateForStream(true, true, true);
        ValidationProcessor processor = new(s_sourceBuffer.AsMemory());
        using (stream.AddReadProcessor(processor))
        {
            await stream.SkipAsync(s_sourceBuffer.Length);
        }
        await Assert.That(processor.Success).IsTrue();
    }

    [Test]
    public async Task AddWriteProcessor_ForFixedBuffer_ThrowsBecauseCannotWriteToFixedBuffer()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.CanRead).IsTrue();
        await Assert.That(stream.CanWrite).IsFalse();
        await Assert.That(stream.CanSeek).IsTrue();
        Assert.Throws<NotSupportedException>(() => stream.AddWriteProcessor(new CustomProcessor(bytes => { })));
    }

    [Test]
    public async Task AddWriteProcessor_ForStream_BecomesNonSeekableWriteOnly()
    {
        using MarshalStream stream = CreateForStream(true, true, true);
        await Assert.That(stream.CanRead).IsTrue();
        await Assert.That(stream.CanWrite).IsTrue();
        await Assert.That(stream.CanSeek).IsTrue();
        using (stream.AddWriteProcessor(new CustomProcessor(bytes => { })))
        {
            await Assert.That(stream.CanRead).IsFalse();
            await Assert.That(stream.CanWrite).IsTrue();
            await Assert.That(stream.CanSeek).IsFalse();
        }
        await Assert.That(stream.CanRead).IsTrue();
        await Assert.That(stream.CanWrite).IsTrue();
        await Assert.That(stream.CanSeek).IsTrue();
    }

    [Test]
    public void AddWriteProcessor_ForStreamCalledTwice_Throws()
    {
        using MarshalStream stream = CreateForStream(true, true, true);
        CustomProcessor processor = new(bytes => { });
        stream.AddWriteProcessor(processor);
        Assert.Throws<ArgumentException>(() => stream.AddWriteProcessor(processor));
    }

    [Test]
    public async Task AddWriteProcessor_ForStreamWriteBytes_ProcessesBytesCorrectly()
    {
        using MarshalStream stream = CreateForStream(true, true, true);
        ValidationProcessor processor = new(s_sourceBuffer.AsMemory());
        using (stream.AddWriteProcessor(processor))
        {
            stream.Write(s_sourceBuffer, 0, s_sourceBuffer.Length);
        }
        await Assert.That(processor.Success).IsTrue();
    }

    [Test]
    public async Task AddWriteProcessor_ForStreamWriteSpan_ProcessesBytesCorrectly()
    {
        using MarshalStream stream = CreateForStream(true, true, true);
        ValidationProcessor processor = new(s_sourceBuffer.AsMemory());
        using (stream.AddWriteProcessor(processor))
        {
            stream.Write(s_sourceBuffer);
        }
        await Assert.That(processor.Success).IsTrue();
    }

    [Test]
    public async Task AddWriteProcessor_ForStreamWriteAsyncBytes_ProcessesBytesCorrectly()
    {
        using MarshalStream stream = CreateForStream(true, true, true);
        ValidationProcessor processor = new(s_sourceBuffer.AsMemory());
        using (stream.AddWriteProcessor(processor))
        {
            await stream.WriteAsync(s_sourceBuffer, 0, s_sourceBuffer.Length);
        }
        await Assert.That(processor.Success).IsTrue();
    }

    [Test]
    public async Task AddWriteProcessor_ForStreamWriteAsyncMemory_ProcessesBytesCorrectly()
    {
        using MarshalStream stream = CreateForStream(true, true, true);
        ValidationProcessor processor = new(s_sourceBuffer.AsMemory());
        using (stream.AddWriteProcessor(processor))
        {
            await stream.WriteAsync(s_sourceBuffer.AsMemory());
        }
        await Assert.That(processor.Success).IsTrue();
    }

    [Test]
    public async Task AddWriteProcessor_ForStreamWriteString_ProcessesBytesCorrectly()
    {
        const string testString = "testing123";
        byte[] bytes = Encoding.UTF8.GetBytes(testString + '\0');
        using MarshalStream stream = CreateForStream(true, true, true);
        ValidationProcessor processor = new(bytes.AsMemory());
        using (stream.AddWriteProcessor(processor))
        {
            stream.WriteString(Encoding.UTF8, testString, true);
        }
        await Assert.That(processor.Success).IsTrue();
    }

    [Test]
    public async Task AddWriteProcessor_ForStreamWriteStringAsync_ProcessesBytesCorrectly()
    {
        const string testString = "testing123";
        byte[] bytes = Encoding.UTF8.GetBytes(testString + '\0');
        using MarshalStream stream = CreateForStream(true, true, true);
        ValidationProcessor processor = new(bytes.AsMemory());
        using (stream.AddWriteProcessor(processor))
        {
            await stream.WriteStringAsync(Encoding.UTF8, testString, true);
        }
        await Assert.That(processor.Success).IsTrue();
    }

    #endregion

    #region BufferedReadableBytes

    [Test]
    public async Task BufferedReadableBytes_InitialValueForFixedBuffer_EqualsFixedBuffer()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.BufferedReadableBytes.ToArray()).IsEquivalentTo(s_sourceBuffer);
    }

    [Test]
    public async Task BufferedReadableBytes_InitialValueForStream_EqualsEmptyBuffer()
    {
        using MarshalStream stream = CreateForStream(true, false, false);
        await Assert.That(stream.BufferedReadableBytes.ToArray()).IsEquivalentTo(Array.Empty<byte>());
    }

    [Test]
    public async Task BufferedReadableBytes_AfterReadPartialForFixedBuffer_EqualsRemainingBuffer()
    {
        const int readSize = 4;
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.Read(stackalloc byte[readSize])).IsEqualTo(readSize);
        await Assert.That(stream.BufferedReadableBytes.ToArray()).IsEquivalentTo(Slice(s_sourceBuffer, readSize));
    }

    [Test]
    public async Task BufferedReadableBytes_AfterReadTotalForFixedBuffer_EqualsEmptyBuffer()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.Read(stackalloc byte[s_sourceBuffer.Length])).IsEqualTo(s_sourceBuffer.Length);
        await Assert.That(stream.BufferedReadableBytes.ToArray()).IsEquivalentTo(Array.Empty<byte>());
    }

    [Test]
    public async Task BufferedReadableBytes_AfterReadExtraForFixedBuffer_EqualsEmptyBuffer()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.Read(stackalloc byte[s_sourceBuffer.Length + 1])).IsEqualTo(s_sourceBuffer.Length);
        await Assert.That(stream.BufferedReadableBytes.ToArray()).IsEquivalentTo(Array.Empty<byte>());
    }

    [Test]
    public async Task BufferedReadableBytes_AfterReadLessThanBufferSizeForStream_EqualsOneByteLeft()
    {
        using MarshalStream stream = CreateForStream(true, false, false);
        await Assert.That(stream.Read(stackalloc byte[c_defaultStreamBufferSize - 1])).IsEqualTo(c_defaultStreamBufferSize - 1);
        await Assert.That(stream.BufferedReadableBytes.ToArray()).IsEquivalentTo(Slice(s_sourceBuffer, c_defaultStreamBufferSize - 1, 1));
    }

    [Test]
    public async Task BufferedReadableBytes_AfterReadMoreThanBufferSizeForStream_EqualsEmptyBuffer()
    {
        using MarshalStream stream = CreateForStream(true, false, false);
        await Assert.That(stream.Read(stackalloc byte[c_defaultStreamBufferSize + 1])).IsEqualTo(c_defaultStreamBufferSize);
        await Assert.That(stream.BufferedReadableBytes.ToArray()).IsEquivalentTo(Array.Empty<byte>());
    }

    [Test]
    public async Task BufferedReadableBytes_AfterSeekLessThanStreamSizeForStream_EqualsEmptyBuffer()
    {
        using MarshalStream stream = CreateForStream(true, true, false);
        await Assert.That(stream.Seek(s_sourceBuffer.Length - 1, SeekOrigin.Begin)).IsEqualTo(s_sourceBuffer.Length - 1);
        await Assert.That(stream.BufferedReadableBytes.ToArray()).IsEquivalentTo(Array.Empty<byte>());
    }

    [Test]
    public async Task BufferedReadableBytes_AfterSeekMoreThanStreamSizeForStream_EqualsEmptyBuffer()
    {
        using MarshalStream stream = CreateForStream(true, true, false);
        await Assert.That(stream.Seek(s_sourceBuffer.Length + 1, SeekOrigin.Begin)).IsEqualTo(s_sourceBuffer.Length + 1);
        await Assert.That(stream.BufferedReadableBytes.ToArray()).IsEquivalentTo(Array.Empty<byte>());
    }

    [Test]
    public async Task BufferedReadableBytes_AfterSeekIncreaseAndReduceBufferSizeForFixedBuffer_EqualsExpectedBuffer()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.BufferedReadableBytes.ToArray()).IsEquivalentTo(s_sourceBuffer);
        await Assert.That(stream.Seek(5, SeekOrigin.Begin)).IsEqualTo(5);
        await Assert.That(stream.BufferedReadableBytes.ToArray()).IsEquivalentTo(Slice(s_sourceBuffer, 5));
        await Assert.That(stream.Seek(6, SeekOrigin.Begin)).IsEqualTo(6);
        await Assert.That(stream.BufferedReadableBytes.ToArray()).IsEquivalentTo(Slice(s_sourceBuffer, 6));
        await Assert.That(stream.Seek(3, SeekOrigin.Begin)).IsEqualTo(3);
        await Assert.That(stream.BufferedReadableBytes.ToArray()).IsEquivalentTo(Slice(s_sourceBuffer, 3));
        await Assert.That(stream.Seek(0, SeekOrigin.Begin)).IsEqualTo(0);
        await Assert.That(stream.BufferedReadableBytes.ToArray()).IsEquivalentTo(s_sourceBuffer);
    }

    [Test]
    public async Task BufferedReadableBytes_AfterSeekIncreaseAndReduceBufferSizeForStream_EqualsExpectedBuffer()
    {
        int bufferSize = 7;
        using MarshalStream stream = CreateForStream(true, true, false, bufferSize);
        await Assert.That(stream.BufferedReadableBytes.ToArray()).IsEquivalentTo(Array.Empty<byte>());
        await Assert.That(stream.EnsureByteCountAvailableInBuffer(1)).IsEqualTo(true);
        await Assert.That(stream.BufferedReadableBytes.ToArray()).IsEquivalentTo(Slice(s_sourceBuffer, 0, bufferSize));
        await Assert.That(stream.Seek(5, SeekOrigin.Begin)).IsEqualTo(5);
        await Assert.That(stream.BufferedReadableBytes.ToArray()).IsEquivalentTo(Slice(s_sourceBuffer, 5, bufferSize - 5));
        await Assert.That(stream.Seek(6, SeekOrigin.Begin)).IsEqualTo(6);
        await Assert.That(stream.BufferedReadableBytes.ToArray()).IsEquivalentTo(Slice(s_sourceBuffer, 6, bufferSize - 6));
        await Assert.That(stream.Seek(3, SeekOrigin.Begin)).IsEqualTo(3);
        await Assert.That(stream.BufferedReadableBytes.ToArray()).IsEquivalentTo(Slice(s_sourceBuffer, 3, bufferSize - 3));
        await Assert.That(stream.Seek(0, SeekOrigin.Begin)).IsEqualTo(0);
        await Assert.That(stream.BufferedReadableBytes.ToArray()).IsEquivalentTo(Slice(s_sourceBuffer, 0, bufferSize));
    }

    #endregion // BufferedReadableBytes

    #region BufferedReadableByteCount

    [Test]
    public async Task BufferedReadableByteCount_InitialValueForFixedBuffer_EqualsBufferLength()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(s_sourceBuffer.Length);
    }

    [Test]
    public async Task BufferedReadableByteCount_InitialValueForStream_EqualsBufferLength()
    {
        using MarshalStream stream = CreateForStream(true, false, false);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(0);
    }

    [Test]
    public async Task BufferedReadableByteCount_AfterReadPartialForFixedBuffer_EqualsRemainingBufferLength()
    {
        const int readSize = 4;
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.Read(stackalloc byte[readSize])).IsEqualTo(readSize);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(s_sourceBuffer.Length - readSize);
    }

    [Test]
    public async Task BufferedReadableByteCount_AfterReadTotalForFixedBuffer_EqualsZero()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.Read(stackalloc byte[s_sourceBuffer.Length])).IsEqualTo(s_sourceBuffer.Length);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(0);
    }

    [Test]
    public async Task BufferedReadableByteCount_AfterReadExtraForFixedBuffer_EqualsZero()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.Read(stackalloc byte[s_sourceBuffer.Length + 1])).IsEqualTo(s_sourceBuffer.Length);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(0);
    }

    [Test]
    public async Task BufferedReadableByteCount_AfterReadLessThanBufferSizeForStream_EqualsUnreadBufferLength()
    {
        using MarshalStream stream = CreateForStream(true, false, false);
        await Assert.That(stream.Read(stackalloc byte[c_defaultStreamBufferSize - 1])).IsEqualTo(c_defaultStreamBufferSize - 1);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(1);
    }

    [Test]
    public async Task BufferedReadableByteCount_AfterReadMoreThanBufferSizeForStream_EqualsZero()
    {
        using MarshalStream stream = CreateForStream(true, false, false);
        await Assert.That(stream.Read(stackalloc byte[c_defaultStreamBufferSize + 1])).IsEqualTo(c_defaultStreamBufferSize);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(0);
    }

    [Test]
    public async Task BufferedReadableByteCount_AfterSeekLessThanStreamSizeForStream_EqualsZero()
    {
        using MarshalStream stream = CreateForStream(true, true, false);
        await Assert.That(stream.Seek(s_sourceBuffer.Length - 1, SeekOrigin.Begin)).IsEqualTo(s_sourceBuffer.Length - 1);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(0);
    }

    [Test]
    public async Task BufferedReadableByteCount_AfterSeekMoreThanStreamSizeForStream_EqualsZero()
    {
        using MarshalStream stream = CreateForStream(true, true, false);
        await Assert.That(stream.Seek(s_sourceBuffer.Length + 1, SeekOrigin.Begin)).IsEqualTo(s_sourceBuffer.Length + 1);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(0);
    }

    [Test]
    public async Task BufferedReadableByteCount_AfterSeekIncreaseAndReduceBufferSizeForFixedBuffer_EqualsExpectedSize()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(s_sourceBuffer.Length);
        await Assert.That(stream.Seek(5, SeekOrigin.Begin)).IsEqualTo(5);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(s_sourceBuffer.Length - 5);
        await Assert.That(stream.Seek(6, SeekOrigin.Begin)).IsEqualTo(6);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(s_sourceBuffer.Length - 6);
        await Assert.That(stream.Seek(3, SeekOrigin.Begin)).IsEqualTo(3);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(s_sourceBuffer.Length - 3);
        await Assert.That(stream.Seek(0, SeekOrigin.Begin)).IsEqualTo(0);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(s_sourceBuffer.Length);
    }

    [Test]
    public async Task BufferedReadableByteCount_AfterSeekIncreaseAndReduceBufferSizeForStream_EqualsExpectedSize()
    {
        int bufferSize = 7;
        using MarshalStream stream = CreateForStream(true, true, false, bufferSize);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(0);
        await Assert.That(stream.EnsureByteCountAvailableInBuffer(1)).IsEqualTo(true);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(bufferSize);
        await Assert.That(stream.Seek(5, SeekOrigin.Begin)).IsEqualTo(5);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(bufferSize - 5);
        await Assert.That(stream.Seek(6, SeekOrigin.Begin)).IsEqualTo(6);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(bufferSize - 6);
        await Assert.That(stream.Seek(3, SeekOrigin.Begin)).IsEqualTo(3);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(bufferSize - 3);
        await Assert.That(stream.Seek(0, SeekOrigin.Begin)).IsEqualTo(0);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(bufferSize);
    }

    #endregion // BufferedReadableByteCount

    #region CopyTo

    [Test]
    public async Task CopyTo_ValidSourceAndDestination_ShouldCopyDataCorrectly()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, false, false)))
        {
            using MemoryStream destinationStream = new();
            stream.CopyTo(destinationStream);
            await Assert.That(destinationStream.ToArray()).IsEquivalentTo(s_sourceBuffer);
        }
    }

    [Test]
    public async Task CopyToAsync_ValidSourceAndDestination_ShouldCopyDataCorrectly()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, false, false)))
        {
            using MemoryStream destinationStream = new();
            await stream.CopyToAsync(destinationStream);
            await Assert.That(destinationStream.ToArray()).IsEquivalentTo(s_sourceBuffer);
        }
    }

    [Test]
    public void CopyTo_NonWritableDestination_Throws()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, false, false)))
        {
            using CustomMemoryStream destinationStream = new([], true, false, false);
            Assert.Throws<NotSupportedException>(() => stream.CopyTo(destinationStream));
        }
    }

    [Test]
    public async Task CopyToAsync_NonWritableDestination_Throws()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, false, false)))
        {
            using CustomMemoryStream destinationStream = new([], true, false, false);
            await Assert.ThrowsAsync<NotSupportedException>(() => stream.CopyToAsync(destinationStream));
        }
    }

    #endregion // CopyTo

    #region EnsureByteCountAvailableInBuffer

    [Test]
    public async Task EnsureByteCountAvailableInBuffer_StartingWithEmptyBuffer_ShouldFillBuffer()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, false, false, s_sourceBuffer.Length)))
        {
            await Assert.That(stream.EnsureByteCountAvailableInBuffer(1)).IsEqualTo(true);
            await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(s_sourceBuffer.Length);
        }
    }

    [Test]
    public async Task EnsureByteCountAvailableInBufferAsync_StartingWithEmptyBuffer_ShouldFillBuffer()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, false, false, s_sourceBuffer.Length)))
        {
            await Assert.That(async () => await stream.EnsureByteCountAvailableInBufferAsync(1)).IsEqualTo(true);
            await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(s_sourceBuffer.Length);
        }
    }

    [Test]
    public async Task EnsureByteCountAvailableInBuffer_PartiallyReadBuffer_ShouldFillBuffer()
    {
        using MarshalStream stream = CreateForStream(true, false, false);
        await Assert.That(stream.Read(stackalloc byte[1])).IsEqualTo(1);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(c_defaultStreamBufferSize - 1);
        await Assert.That(stream.EnsureByteCountAvailableInBuffer(c_defaultStreamBufferSize)).IsEqualTo(true);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(c_defaultStreamBufferSize);
    }

    [Test]
    public async Task EnsureByteCountAvailableInBufferAsync_PartiallyReadBuffer_ShouldFillBuffer()
    {
        using MarshalStream stream = CreateForStream(true, false, false);
        await Assert.That(stream.Read(stackalloc byte[1])).IsEqualTo(1);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(c_defaultStreamBufferSize - 1);
        await Assert.That(async () => await stream.EnsureByteCountAvailableInBufferAsync(c_defaultStreamBufferSize)).IsEqualTo(true);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(c_defaultStreamBufferSize);
    }

    [Test]
    public async Task EnsureByteCountAvailableInBuffer_CannotFillDueToEndOfStream_ReturnsFalse()
    {
        Memory<byte> readBuffer = new byte[s_sourceBuffer.Length - 1];
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, false, false, s_sourceBuffer.Length)))
        {
            await Assert.That(stream.Read(readBuffer.Span)).IsEqualTo(s_sourceBuffer.Length - 1);
            await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(1);
            await Assert.That(stream.EnsureByteCountAvailableInBuffer(s_sourceBuffer.Length)).IsEqualTo(false);
            await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(1);
        }
    }

    [Test]
    public async Task EnsureByteCountAvailableInBufferAsync_CannotFillDueToEndOfStream_ReturnsFalse()
    {
        Memory<byte> readBuffer = new byte[s_sourceBuffer.Length - 1];
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, false, false, s_sourceBuffer.Length)))
        {
            await Assert.That(stream.Read(readBuffer.Span)).IsEqualTo(s_sourceBuffer.Length - 1);
            await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(1);
            await Assert.That(async () => await stream.EnsureByteCountAvailableInBufferAsync(s_sourceBuffer.Length)).IsEqualTo(false);
            await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(1);
        }
    }

    [Test]
    public async Task EnsureByteCountAvailableInBuffer_PartiallyReadFixedBuffer_ReturnsFalse()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.Read(stackalloc byte[1])).IsEqualTo(1);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(s_sourceBuffer.Length - 1);
        await Assert.That(stream.EnsureByteCountAvailableInBuffer(s_sourceBuffer.Length)).IsEqualTo(false);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(s_sourceBuffer.Length - 1);
    }

    [Test]
    public async Task EnsureByteCountAvailableInBufferAsync_PartiallyReadFixedBuffer_ReturnsFalse()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.Read(stackalloc byte[1])).IsEqualTo(1);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(s_sourceBuffer.Length - 1);
        await Assert.That(async () => await stream.EnsureByteCountAvailableInBufferAsync(s_sourceBuffer.Length)).IsEqualTo(false);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(s_sourceBuffer.Length - 1);
    }

    [Test]
    public async Task EnsureByteCountAvailableInBuffer_HasSufficientBytes_ReturnsTrue()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, false, false, s_sourceBuffer.Length)))
        {
            await Assert.That(stream.EnsureByteCountAvailableInBuffer(1)).IsEqualTo(true);
            await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(s_sourceBuffer.Length);
            await Assert.That(stream.EnsureByteCountAvailableInBuffer(s_sourceBuffer.Length - 1)).IsEqualTo(true);
            await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(s_sourceBuffer.Length);
        }
    }

    [Test]
    public async Task EnsureByteCountAvailableInBufferAsync_HasSufficientBytes_ReturnsTrue()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, false, false, s_sourceBuffer.Length)))
        {
            await Assert.That(stream.EnsureByteCountAvailableInBuffer(1)).IsEqualTo(true);
            await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(s_sourceBuffer.Length);
            await Assert.That(async () => await stream.EnsureByteCountAvailableInBufferAsync(s_sourceBuffer.Length - 1)).IsEqualTo(true);
            await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(s_sourceBuffer.Length);
        }
    }

    #endregion // EnsureByteCountAvailableInBuffer

    #region Flush

    [Test]
    public async Task Flush_InitialValueForFixedBuffer_NoChange()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        stream.Flush();
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(s_sourceBuffer.Length);
        await Assert.That(stream.Position).IsEqualTo(0);
    }

    [Test]
    public async Task FlushAsync_InitialValueForFixedBuffer_NoChange()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        await stream.FlushAsync();
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(s_sourceBuffer.Length);
        await Assert.That(stream.Position).IsEqualTo(0);
    }

    [Test]
    public async Task Flush_EmptyBufferForStream_NoChange()
    {
        using MarshalStream stream = CreateForStream(true, true, true);
        stream.Flush();
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(0);
        await Assert.That(stream.Position).IsEqualTo(0);
    }

    [Test]
    public async Task FlushAsync_EmptyBufferForStream_NoChange()
    {
        using MarshalStream stream = CreateForStream(true, true, true);
        await stream.FlushAsync();
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(0);
        await Assert.That(stream.Position).IsEqualTo(0);
    }

    [Test]
    public async Task Flush_FullReadBufferForFixedBuffer_ReadBufferNotCleared()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(s_sourceBuffer.Length);
        stream.Flush();
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(s_sourceBuffer.Length);
        await Assert.That(stream.Position).IsEqualTo(0);
    }

    [Test]
    public async Task FlushAsync_FullReadBufferForFixedBuffer_ReadBufferNotCleared()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(s_sourceBuffer.Length);
        await stream.FlushAsync();
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(s_sourceBuffer.Length);
        await Assert.That(stream.Position).IsEqualTo(0);
    }

    [Test]
    public async Task Flush_FullReadBufferForStream_ReadBufferCleared()
    {
        using MarshalStream stream = CreateForStream(true, true, true, s_sourceBuffer.Length);
        await Assert.That(stream.EnsureByteCountAvailableInBuffer(1)).IsEqualTo(true);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(s_sourceBuffer.Length);
        stream.Flush();
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(0);
        await Assert.That(stream.Position).IsEqualTo(0);
    }

    [Test]
    public async Task FlushAsync_FullReadBufferForStream_ReadBufferCleared()
    {
        using MarshalStream stream = CreateForStream(true, true, true, s_sourceBuffer.Length);
        await Assert.That(stream.EnsureByteCountAvailableInBuffer(1)).IsEqualTo(true);
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(s_sourceBuffer.Length);
        await stream.FlushAsync();
        await Assert.That(stream.BufferedReadableByteCount).IsEqualTo(0);
        await Assert.That(stream.Position).IsEqualTo(0);
    }

    [Test]
    public async Task Flush_HasUnpersistedWrites_WriteBufferPersisted()
    {
        CustomMemoryStream destinationStream = new(s_sourceBuffer, true, true, true);
        using MarshalStream stream = new(destinationStream);
        await Assert.That(stream.Position).IsEqualTo(0);
        stream.Write([11, 12]);
        await Assert.That(stream.Position).IsEqualTo(2);
        await Assert.That(destinationStream.ToArray()[0..2]).IsEquivalentTo(new byte[] { 1, 2 });
        stream.Flush();
        await Assert.That(stream.Position).IsEqualTo(2);
        await Assert.That(destinationStream.ToArray()[0..2]).IsEquivalentTo(new byte[] { 11, 12 });
    }

    [Test]
    public async Task FlushAsync_HasUnpersistedWrites_WriteBufferPersisted()
    {
        CustomMemoryStream destinationStream = new(s_sourceBuffer, true, true, true);
        using MarshalStream stream = new(destinationStream);
        await Assert.That(stream.Position).IsEqualTo(0);
        stream.Write([11, 12]);
        await Assert.That(stream.Position).IsEqualTo(2);
        await Assert.That(destinationStream.ToArray()[0..2]).IsEquivalentTo(new byte[] { 1, 2 });
        await stream.FlushAsync();
        await Assert.That(stream.Position).IsEqualTo(2);
        await Assert.That(destinationStream.ToArray()[0..2]).IsEquivalentTo(new byte[] { 11, 12 });
    }

    #endregion // Flush

    #region IsFixedBuffer

    [Test]
    public async Task IsFixedBuffer_ActuallyIsFixedBuffer_ReturnsTrue()
    {
        using MarshalStream stream = new(new byte[1]);
        await Assert.That(stream.IsFixedBuffer).IsTrue();
    }

    [Test]
    public async Task IsFixedBuffer_WrappingStream_ReturnsFalse()
    {
        using MarshalStream stream = new(new MemoryStream());
        await Assert.That(stream.IsFixedBuffer).IsFalse();
    }

    #endregion IsFixedBuffer

    #region Match

    [Test]
    public async Task Match_ForFixedBufferEmptyMatch_MatchIsTrue()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.Match([])).IsEqualTo(new(true, 0, false));
        await Assert.That(stream.Position).IsEqualTo(0);
    }

    [Test]
    public async Task Match_ForFixedBufferFirstCharMatch_MatchIsFalse()
    {
        int byteCountToCheck = 4;
        byte[] match = s_sourceBuffer.AsSpan(0, byteCountToCheck).ToArray();
        match[0]++;
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.Match(match.AsSpan())).IsEqualTo(new(false, 0, false));
        await Assert.That(stream.Position).IsEqualTo(0);
    }

    [Test]
    public async Task Match_ForFixedBufferFullStream_MatchIsTrue()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.Match(s_sourceBuffer.AsSpan())).IsEqualTo(new(true, s_sourceBuffer.Length, false));
        await Assert.That(stream.Position).IsEqualTo(s_sourceBuffer.Length);
    }

    [Test]
    public async Task Match_ForFixedBufferLastCharMatch_MatchIsFalse()
    {
        int byteCountToCheck = 4;
        byte[] match = s_sourceBuffer.AsSpan(0, byteCountToCheck).ToArray();
        match[byteCountToCheck - 1]++;
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.Match(match.AsSpan())).IsEqualTo(new(false, 0, false));
        await Assert.That(stream.Position).IsEqualTo(0);
    }

    [Test]
    public async Task Match_ForFixedBufferMatchBiggerThanStream_MatchIsFalse()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.Match([..s_sourceBuffer, ..s_sourceBuffer])).IsEqualTo(new(false, 0, true));
        await Assert.That(stream.Position).IsEqualTo(0);
    }

    [Test]
    public async Task Match_ForFixedBufferPartialStream_MatchIsTrue()
    {
        int byteCountToCheck = 4;
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.Match(s_sourceBuffer.AsSpan(0, byteCountToCheck))).IsEqualTo(new(true, byteCountToCheck, false));
        await Assert.That(stream.Position).IsEqualTo(byteCountToCheck);
    }

    [Test]
    public async Task Match_ForStreamEmptyMatch_MatchIsTrue()
    {
        using MarshalStream stream = CreateForStream(true, true, true);
        await Assert.That(stream.Match(Span<byte>.Empty)).IsEqualTo(new(true, 0, false));
        await Assert.That(stream.Position).IsEqualTo(0);
    }

    [Test]
    public async Task Match_ForStreamFirstCharMatch_MatchIsFalse()
    {
        int byteCountToCheck = 4;
        byte[] match = s_sourceBuffer.AsSpan(0, byteCountToCheck).ToArray();
        match[0]++;
        using MarshalStream stream = CreateForStream(true, true, true);
        await Assert.That(stream.Match(match.AsSpan())).IsEqualTo(new(false, 0, false));
        await Assert.That(stream.Position).IsEqualTo(0);
    }

    [Test]
    public async Task Match_ForStreamFullStream_MatchIsTrue()
    {
        using MarshalStream stream = CreateForStream(true, true, true);
        await Assert.That(stream.Match(s_sourceBuffer.AsSpan())).IsEqualTo(new(true, s_sourceBuffer.Length, false));
        await Assert.That(stream.Position).IsEqualTo(s_sourceBuffer.Length);
    }

    [Test]
    public async Task Match_ForStreamLastCharMatch_MatchIsFalse()
    {
        int byteCountToCheck = 4;
        byte[] match = s_sourceBuffer.AsSpan(0, byteCountToCheck).ToArray();
        match[byteCountToCheck - 1]++;
        using MarshalStream stream = CreateForStream(true, true, true);
        await Assert.That(stream.Match(match.AsSpan())).IsEqualTo(new(false, c_defaultStreamBufferSize, false));
        await Assert.That(stream.Position).IsEqualTo(c_defaultStreamBufferSize);
    }

    [Test]
    public async Task Match_ForStreamMatchBiggerThanStream_MatchIsFalse()
    {
        using MarshalStream stream = CreateForStream(true, true, true);
        await Assert.That(stream.Match([..s_sourceBuffer, ..s_sourceBuffer])).IsEqualTo(new(false, s_sourceBuffer.Length, true));
        await Assert.That(stream.Position).IsEqualTo(s_sourceBuffer.Length);
    }

    [Test]
    public async Task Match_ForStreamPartialStream_MatchIsTrue()
    {
        int byteCountToCheck = 4;
        using MarshalStream stream = CreateForStream(true, true, true);
        await Assert.That(stream.Match(s_sourceBuffer.AsSpan(0, byteCountToCheck))).IsEqualTo(new(true, byteCountToCheck, false));
        await Assert.That(stream.Position).IsEqualTo(byteCountToCheck);
    }

    [Test]
    public async Task MatchAsync_ForFixedBufferEmptyMatch_MatchIsTrue()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.MatchAsync(Memory<byte>.Empty)).IsEqualTo(new(true, 0, false));
        await Assert.That(stream.Position).IsEqualTo(0);
    }

    [Test]
    public async Task MatchAsync_ForFixedBufferFirstCharMatch_MatchIsFalse()
    {
        int byteCountToCheck = 4;
        byte[] match = s_sourceBuffer.AsSpan(0, byteCountToCheck).ToArray();
        match[0]++;
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.MatchAsync(match.AsMemory())).IsEqualTo(new(false, 0, false));
        await Assert.That(stream.Position).IsEqualTo(0);
    }

    [Test]
    public async Task MatchAsync_ForFixedBufferFullStream_MatchIsTrue()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.MatchAsync(s_sourceBuffer.AsMemory())).IsEqualTo(new(true, s_sourceBuffer.Length, false));
        await Assert.That(stream.Position).IsEqualTo(s_sourceBuffer.Length);
    }

    [Test]
    public async Task MatchAsync_ForFixedBufferLastCharMatch_MatchIsFalse()
    {
        int byteCountToCheck = 4;
        byte[] match = s_sourceBuffer.AsSpan(0, byteCountToCheck).ToArray();
        match[byteCountToCheck - 1]++;
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.MatchAsync(match.AsMemory())).IsEqualTo(new(false, 0, false)); ;
        await Assert.That(stream.Position).IsEqualTo(0);
    }

    [Test]
    public async Task MatchAsync_ForFixedBufferMatchBiggerThanStream_MatchIsFalse()
    {
        using MarshalStream stream = CreateForFixedBuffer();
        List<byte> matchBytes = [..s_sourceBuffer, ..s_sourceBuffer];
        await Assert.That(stream.MatchAsync(matchBytes.ToArray().AsMemory())).IsEqualTo(new(false, 0, true));
        await Assert.That(stream.Position).IsEqualTo(0);
    }

    [Test]
    public async Task MatchAsync_ForFixedBufferPartialStream_MatchIsTrue()
    {
        int byteCountToCheck = 4;
        using MarshalStream stream = CreateForFixedBuffer();
        await Assert.That(stream.MatchAsync(s_sourceBuffer.AsMemory(0, byteCountToCheck))).IsEqualTo(new(true, byteCountToCheck, false));
        await Assert.That(stream.Position).IsEqualTo(byteCountToCheck);
    }

    [Test]
    public async Task MatchAsync_ForStreamEmptyMatch_MatchIsTrue()
    {
        using MarshalStream stream = CreateForStream(true, true, true);
        await Assert.That(stream.MatchAsync(Memory<byte>.Empty)).IsEqualTo(new(true, 0, false));
        await Assert.That(stream.Position).IsEqualTo(0);
    }

    [Test]
    public async Task MatchAsync_ForStreamFirstCharMatch_MatchIsFalse()
    {
        int byteCountToCheck = 4;
        byte[] match = s_sourceBuffer.AsSpan(0, byteCountToCheck).ToArray();
        match[0]++;
        using MarshalStream stream = CreateForStream(true, true, true);
        await Assert.That(stream.MatchAsync(match.AsMemory())).IsEqualTo(new(false, 0, false));
        await Assert.That(stream.Position).IsEqualTo(0);
    }

    [Test]
    public async Task MatchAsync_ForStreamFullStream_MatchIsTrue()
    {
        using MarshalStream stream = CreateForStream(true, true, true);
        await Assert.That(stream.MatchAsync(s_sourceBuffer.AsMemory())).IsEqualTo(new(true, s_sourceBuffer.Length, false));
        await Assert.That(stream.Position).IsEqualTo(s_sourceBuffer.Length);
    }

    [Test]
    public async Task MatchAsync_ForStreamLastCharMatch_MatchIsTrue()
    {
        int byteCountToCheck = 4;
        byte[] match = s_sourceBuffer.AsSpan(0, byteCountToCheck).ToArray();
        match[byteCountToCheck - 1]++;
        using MarshalStream stream = CreateForStream(true, true, true);
        await Assert.That(stream.MatchAsync(match.AsMemory())).IsEqualTo(new(false, c_defaultStreamBufferSize, false));
        await Assert.That(stream.Position).IsEqualTo(c_defaultStreamBufferSize);
    }

    [Test]
    public async Task MatchAsync_ForStreamMatchBiggerThanStream_MatchIsFalse()
    {
        using MarshalStream stream = CreateForStream(true, true, true);
        List<byte> matchBytes = [..s_sourceBuffer, ..s_sourceBuffer];
        await Assert.That(stream.MatchAsync(matchBytes.ToArray().AsMemory())).IsEqualTo(new(false, s_sourceBuffer.Length, true));
        await Assert.That(stream.Position).IsEqualTo(s_sourceBuffer.Length);
    }

    [Test]
    public async Task MatchAsync_ForStreamPartialStream_MatchIsTrue()
    {
        int byteCountToCheck = 4;
        using MarshalStream stream = CreateForStream(true, true, true);
        await Assert.That(stream.MatchAsync(s_sourceBuffer.AsMemory(0, byteCountToCheck))).IsEqualTo(new(true, byteCountToCheck, false));
        await Assert.That(stream.Position).IsEqualTo(byteCountToCheck);
    }

    #endregion Match

    #region Length

    [Test]
    public async Task Length_Seekable_ReturnsExpectedValue()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(false, true, false)))
        {
            await Assert.That(stream.Length).IsEqualTo(s_sourceBuffer.Length);
        }
    }

    [Test]
    public async Task Length_HasUnpersistedWrites_LengthIncludesUnpersistedWrites()
    {
        using MarshalStream stream = CreateForStream(true, true, true);
        stream.Seek(0, SeekOrigin.End);
        stream.Write([11, 12]);
        await Assert.That(stream.Length).IsEqualTo(s_sourceBuffer.Length + 2);
    }

    #endregion // Length

    #region Position

    [Test]
    public async Task Position_InitialValue_ReturnsZero()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(false, true, false)))
        {
            await Assert.That(stream.Position).IsEqualTo(0);
        }
    }

    [Test]
    public async Task Position_SeekToEndOfStream_ReturnsStreamLength()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(false, true, false)))
        {
            await Assert.That(stream.Seek(0, SeekOrigin.End)).IsEqualTo(s_sourceBuffer.Length);
            await Assert.That(stream.Position).IsEqualTo(s_sourceBuffer.Length);
        }
    }

    [Test]
    public async Task Position_SeekToAfterEndOfStream_ReturnsAfterStreamLength()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(false, true, false)))
        {
            await Assert.That(stream.Seek(10, SeekOrigin.End)).IsEqualTo(s_sourceBuffer.Length + 10);
            await Assert.That(stream.Position).IsEqualTo(s_sourceBuffer.Length + 10);
        }
    }

    [Test]
    public async Task Position_HasUnpersistedWrites_ReturnsAfterCachedWritePosition()
    {
        using MarshalStream stream = CreateForStream(true, true, true);
        stream.Write(stackalloc byte[c_defaultStreamBufferSize - 1]);
        await Assert.That(stream.Position).IsEqualTo(c_defaultStreamBufferSize - 1);
    }

    #endregion // Position

    #region Read

    [Test]
    public async Task Read_ByteArrayForStream_ReturnsExpectedValues()
    {
        var destinationBuffer = new byte[c_defaultStreamBufferSize];
        using MarshalStream stream = CreateForStream(true, true, false);
        while (true)
        {
            if ((int)stream.Position + c_defaultStreamBufferSize > s_sourceBuffer.Length)
            {
                int remainingLength = s_sourceBuffer.Length - (int)stream.Position;
                await Assert.That(stream.Read(destinationBuffer, 0, remainingLength))
                    .IsEqualTo(remainingLength);
                await Assert.That(Slice(destinationBuffer, 0, 1))
                    .IsEquivalentTo(Slice(s_sourceBuffer, (int)stream.Position - remainingLength, remainingLength));
                await Assert.That(stream.Read(destinationBuffer, 0, remainingLength))
                    .IsEqualTo(0);
                break;
            }
            await Assert.That(stream.Read(destinationBuffer, 0, c_defaultStreamBufferSize - 1))
                .IsEqualTo(c_defaultStreamBufferSize - 1);
            await Assert.That(Slice(destinationBuffer, 0, c_defaultStreamBufferSize - 1))
                .IsEquivalentTo(Slice(s_sourceBuffer, (int)stream.Position - (c_defaultStreamBufferSize - 1), c_defaultStreamBufferSize - 1));
            await Assert.That(stream.Read(destinationBuffer, 0, c_defaultStreamBufferSize - 1))
                .IsEqualTo(1);
            await Assert.That(Slice(destinationBuffer, 0, 1))
                .IsEquivalentTo(Slice(s_sourceBuffer, (int)stream.Position - 1, 1));
        }
    }

    [Test]
    public async Task Read_SpanForStream_ReturnsExpectedValues()
    {
        var destinationBuffer = new byte[c_defaultStreamBufferSize];
        using MarshalStream stream = CreateForStream(true, true, false);
        while (true)
        {
            if ((int)stream.Position + c_defaultStreamBufferSize > s_sourceBuffer.Length)
            {
                int remainingLength = s_sourceBuffer.Length - (int)stream.Position;
                await Assert.That(stream.Read(destinationBuffer.AsSpan(0, remainingLength)))
                    .IsEqualTo(remainingLength);
                await Assert.That(Slice(destinationBuffer, 0, 1))
                    .IsEquivalentTo(Slice(s_sourceBuffer, (int)stream.Position - remainingLength, remainingLength));
                await Assert.That(stream.Read(destinationBuffer.AsSpan(0, remainingLength)))
                    .IsEqualTo(0);
                break;
            }
            await Assert.That(stream.Read(destinationBuffer.AsSpan(0, c_defaultStreamBufferSize - 1)))
                .IsEqualTo(c_defaultStreamBufferSize - 1);
            await Assert.That(Slice(destinationBuffer, 0, c_defaultStreamBufferSize - 1))
                .IsEquivalentTo(Slice(s_sourceBuffer, (int)stream.Position - (c_defaultStreamBufferSize - 1), c_defaultStreamBufferSize - 1));
            await Assert.That(stream.Read(destinationBuffer.AsSpan(0, c_defaultStreamBufferSize - 1)))
                .IsEqualTo(1);
            await Assert.That(Slice(destinationBuffer, 0, 1))
                .IsEquivalentTo(Slice(s_sourceBuffer, (int)stream.Position - 1, 1));
        }
    }

    [Test]
    public async Task Read_ByteArrayForFixedBuffer_ReturnsExpectedValues()
    {
        var destinationBuffer = new byte[c_defaultStreamBufferSize];
        using MarshalStream stream = CreateForFixedBuffer();
        while (true)
        {
            if ((int)stream.Position + c_defaultStreamBufferSize > s_sourceBuffer.Length)
            {
                int remainingLength = s_sourceBuffer.Length - (int)stream.Position;
                await Assert.That(stream.Read(destinationBuffer, 0, remainingLength))
                    .IsEqualTo(remainingLength);
                await Assert.That(Slice(destinationBuffer, 0, 1))
                    .IsEquivalentTo(Slice(s_sourceBuffer, (int)stream.Position - remainingLength, remainingLength));
                await Assert.That(stream.Read(destinationBuffer, 0, remainingLength))
                    .IsEqualTo(0);
                break;
            }
            await Assert.That(stream.Read(destinationBuffer, 0, c_defaultStreamBufferSize))
                .IsEqualTo(c_defaultStreamBufferSize);
            await Assert.That(Slice(destinationBuffer, 0, c_defaultStreamBufferSize))
                .IsEquivalentTo(Slice(s_sourceBuffer, (int)stream.Position - c_defaultStreamBufferSize, c_defaultStreamBufferSize));
        }
    }

    [Test]
    public async Task Read_SpanForFixedBuffer_ReturnsExpectedValues()
    {
        var destinationBuffer = new byte[c_defaultStreamBufferSize];
        using MarshalStream stream = CreateForFixedBuffer();
        while (true)
        {
            if ((int)stream.Position + c_defaultStreamBufferSize > s_sourceBuffer.Length)
            {
                int remainingLength = s_sourceBuffer.Length - (int)stream.Position;
                await Assert.That(stream.Read(destinationBuffer.AsSpan(0, remainingLength)))
                    .IsEqualTo(remainingLength);
                await Assert.That(Slice(destinationBuffer, 0, 1))
                    .IsEquivalentTo(Slice(s_sourceBuffer, (int)stream.Position - remainingLength, remainingLength));
                await Assert.That(stream.Read(destinationBuffer.AsSpan(0, remainingLength)))
                    .IsEqualTo(0);
                break;
            }
            await Assert.That(stream.Read(destinationBuffer.AsSpan(0, c_defaultStreamBufferSize)))
                .IsEqualTo(c_defaultStreamBufferSize);
            await Assert.That(Slice(destinationBuffer, 0, c_defaultStreamBufferSize))
                .IsEquivalentTo(Slice(s_sourceBuffer, (int)stream.Position - c_defaultStreamBufferSize, c_defaultStreamBufferSize));
        }
    }

    [Test]
    public async Task ReadAsync_ByteArrayForStream_ReturnsExpectedValues()
    {
        var destinationBuffer = new byte[c_defaultStreamBufferSize];
        using MarshalStream stream = CreateForStream(true, true, false);
        while (true)
        {
            if ((int)stream.Position + c_defaultStreamBufferSize > s_sourceBuffer.Length)
            {
                int remainingLength = s_sourceBuffer.Length - (int)stream.Position;
                await Assert.That(stream.ReadAsync(destinationBuffer, 0, remainingLength))
                    .IsEqualTo(remainingLength);
                await Assert.That(Slice(destinationBuffer, 0, 1))
                    .IsEquivalentTo(Slice(s_sourceBuffer, (int)stream.Position - remainingLength, remainingLength));
                await Assert.That(stream.ReadAsync(destinationBuffer, 0, remainingLength))
                    .IsEqualTo(0);
                break;
            }
            await Assert.That(stream.ReadAsync(destinationBuffer, 0, c_defaultStreamBufferSize - 1))
                .IsEqualTo(c_defaultStreamBufferSize - 1);
            await Assert.That(Slice(destinationBuffer, 0, c_defaultStreamBufferSize - 1))
                .IsEquivalentTo(Slice(s_sourceBuffer, (int)stream.Position - (c_defaultStreamBufferSize - 1), c_defaultStreamBufferSize - 1));
            await Assert.That(stream.ReadAsync(destinationBuffer, 0, c_defaultStreamBufferSize - 1))
                .IsEqualTo(1);
            await Assert.That(Slice(destinationBuffer, 0, 1))
                .IsEquivalentTo(Slice(s_sourceBuffer, (int)stream.Position - 1, 1));
        }
    }

    [Test]
    public async Task ReadAsync_SpanForStream_ReturnsExpectedValues()
    {
        var destinationBuffer = new byte[c_defaultStreamBufferSize];
        using MarshalStream stream = CreateForStream(true, true, false);
        while (true)
        {
            if ((int)stream.Position + c_defaultStreamBufferSize > s_sourceBuffer.Length)
            {
                int remainingLength = s_sourceBuffer.Length - (int)stream.Position;
                await Assert.That(async () => await stream.ReadAsync(destinationBuffer.AsMemory(0, remainingLength)))
                    .IsEqualTo(remainingLength);
                await Assert.That(Slice(destinationBuffer, 0, 1))
                    .IsEquivalentTo(Slice(s_sourceBuffer, (int)stream.Position - remainingLength, remainingLength));
                await Assert.That(async () => await stream.ReadAsync(destinationBuffer.AsMemory(0, remainingLength)))
                    .IsEqualTo(0);
                break;
            }
            await Assert.That(async () => await stream.ReadAsync(destinationBuffer.AsMemory(0, c_defaultStreamBufferSize - 1)))
                .IsEqualTo(c_defaultStreamBufferSize - 1);
            await Assert.That(Slice(destinationBuffer, 0, c_defaultStreamBufferSize - 1))
                .IsEquivalentTo(Slice(s_sourceBuffer, (int)stream.Position - (c_defaultStreamBufferSize - 1), c_defaultStreamBufferSize - 1));
            await Assert.That(async () => await stream.ReadAsync(destinationBuffer.AsMemory(0, c_defaultStreamBufferSize - 1)))
                .IsEqualTo(1);
            await Assert.That(Slice(destinationBuffer, 0, 1))
                .IsEquivalentTo(Slice(s_sourceBuffer, (int)stream.Position - 1, 1));
        }
    }

    [Test]
    public async Task ReadAsync_ByteArrayForFixedBuffer_ReturnsExpectedValues()
    {
        var destinationBuffer = new byte[c_defaultStreamBufferSize];
        using MarshalStream stream = CreateForFixedBuffer();
        while (true)
        {
            if ((int)stream.Position + c_defaultStreamBufferSize > s_sourceBuffer.Length)
            {
                int remainingLength = s_sourceBuffer.Length - (int)stream.Position;
                await Assert.That(stream.ReadAsync(destinationBuffer, 0, remainingLength))
                    .IsEqualTo(remainingLength);
                await Assert.That(Slice(destinationBuffer, 0, 1))
                    .IsEquivalentTo(Slice(s_sourceBuffer, (int)stream.Position - remainingLength, remainingLength));
                await Assert.That(stream.ReadAsync(destinationBuffer, 0, remainingLength))
                    .IsEqualTo(0);
                break;
            }
            await Assert.That(stream.ReadAsync(destinationBuffer, 0, c_defaultStreamBufferSize))
                .IsEqualTo(c_defaultStreamBufferSize);
            await Assert.That(Slice(destinationBuffer, 0, c_defaultStreamBufferSize))
                .IsEquivalentTo(Slice(s_sourceBuffer, (int)stream.Position - c_defaultStreamBufferSize, c_defaultStreamBufferSize));
        }
    }

    [Test]
    public async Task ReadAsync_SpanForFixedBuffer_ReturnsExpectedValues()
    {
        var destinationBuffer = new byte[c_defaultStreamBufferSize];
        using MarshalStream stream = CreateForFixedBuffer();
        while (true)
        {
            if ((int)stream.Position + c_defaultStreamBufferSize > s_sourceBuffer.Length)
            {
                int remainingLength = s_sourceBuffer.Length - (int)stream.Position;
                await Assert.That(async () => await stream.ReadAsync(destinationBuffer.AsMemory(0, remainingLength)))
                    .IsEqualTo(remainingLength);
                await Assert.That(Slice(destinationBuffer, 0, 1))
                    .IsEquivalentTo(Slice(s_sourceBuffer, (int)stream.Position - remainingLength, remainingLength));
                await Assert.That(async () => await stream.ReadAsync(destinationBuffer.AsMemory(0, remainingLength)))
                    .IsEqualTo(0);
                break;
            }
            await Assert.That(async () => await stream.ReadAsync(destinationBuffer.AsMemory(0, c_defaultStreamBufferSize)))
                .IsEqualTo(c_defaultStreamBufferSize);
            await Assert.That(Slice(destinationBuffer, 0, c_defaultStreamBufferSize))
                .IsEquivalentTo(Slice(s_sourceBuffer, (int)stream.Position - c_defaultStreamBufferSize, c_defaultStreamBufferSize));
        }
    }

    [Test]
    public async Task Read_AfterStreamEnd_ReturnsZero()
    {
        var destinationBuffer = new byte[1];
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, true, false)))
        {
            foreach (var seekOffset in new int[0, 1])
            {
                stream.Seek(seekOffset, SeekOrigin.End);
                await Assert.That(stream.Read(destinationBuffer, 0, 1)).IsEqualTo(0);
                await Assert.That(stream.Read(destinationBuffer.AsSpan())).IsEqualTo(0);
                await Assert.That(stream.ReadAsync(destinationBuffer, 0, 1)).IsEqualTo(0);
                await Assert.That(async () => await stream.ReadAsync(destinationBuffer.AsMemory())).IsEqualTo(0);
                await Assert.That(stream.ReadByte()).IsEqualTo(-1);
                await Assert.That(async () => await stream.ReadByteAsync()).IsEqualTo(-1);
            }
        }
    }

    #endregion // Read

    #region ReadByte

    [Test]
    public async Task ReadByte_AllBytesAndEnd_ReturnsExpectedValue()
    {
        var expectedResults = s_sourceBuffer.Select(x => (int)x).Concat([-1]).ToArray();
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, false, false)))
        {
            for (int i = 0; i < expectedResults.Length; i++)
            {
                await Assert.That(stream.ReadByte()).IsEqualTo(expectedResults[i]);
            }
        }
    }

    [Test]
    public async Task ReadByteAsync_AllBytesAndEnd_ReturnsExpectedValue()
    {
        var expectedResults = s_sourceBuffer.Select(x => (int)x).Concat([-1]).ToArray();
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, false, false)))
        {
            for (int i = 0; i < expectedResults.Length; i++)
            {
                await Assert.That(async () => await stream.ReadByteAsync()).IsEqualTo(expectedResults[i]);
            }
        }
    }

    #endregion // ReadByte

    #region ReadString

    [Test]
    public async Task ReadString_MultiByteCharacterStopOnEmbeddedNull_RoundTripsCorrectly()
    {
        const string expectedString = "dকাঁañ";
        const string expectedStringPlusNullTerminator = expectedString + "\0";
        const string testString = expectedStringPlusNullTerminator + "test";
        foreach (Encoding encoding in new[] { Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32 })
        {
            byte[] testBuffer = encoding.GetBytes(testString);
            int expectedBytesRead = encoding.GetByteCount(expectedStringPlusNullTerminator);
            using MarshalStream stream = new(testBuffer);
            MarshalStreamReadStringResult result = stream.ReadString(encoding.GetDecoder(), -1, MarshalStreamNullTerminatorBehavior.Stop);
            await Assert.That(result).IsEqualTo(new MarshalStreamReadStringResult(expectedString, expectedBytesRead, true, false));
        }
    }

    [Test]
    public async Task ReadString_SingleCharacterBufferStopOnEmbeddedNull_RoundTripsCorrectly()
    {
        const string expectedString = "dকাঁañ";
        const string expectedStringPlusNullTerminator = expectedString + "\0";
        const string testString = expectedStringPlusNullTerminator + "test";
        foreach (Encoding encoding in new[] { Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32 })
        {
            byte[] testBuffer = encoding.GetBytes(testString);
            int expectedBytesRead = encoding.GetByteCount(expectedStringPlusNullTerminator);
            using MarshalStream stream = new(new MemoryStream(testBuffer), true, 1);
            MarshalStreamReadStringResult result = stream.ReadString(encoding.GetDecoder(), -1, MarshalStreamNullTerminatorBehavior.Stop);
            await Assert.That(result).IsEqualTo(new MarshalStreamReadStringResult(expectedString, expectedBytesRead, true, false));
        }
    }

    [Test]
    public async Task ReadString_MultiByteCharacterDoNotStopOnEmbeddedNull_RoundTripsCorrectly()
    {
        const string expectedString = "dকাঁañ\0test";
        foreach (Encoding encoding in new[] { Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32 })
        {
            byte[] testBuffer = encoding.GetBytes(expectedString);
            using MarshalStream stream = new(testBuffer);
            MarshalStreamReadStringResult result = stream.ReadString(encoding.GetDecoder(), -1, MarshalStreamNullTerminatorBehavior.Ignore);
            await Assert.That(result).IsEqualTo(new MarshalStreamReadStringResult(expectedString, testBuffer.Length, false, true));
        }
    }

    [Test]
    public async Task ReadString_SingleCharacterBufferDoNotStopOnEmbeddedNull_RoundTripsCorrectly()
    {
        const string expectedString = "dকাঁañ\0test";
        foreach (Encoding encoding in new[] { Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32 })
        {
            byte[] testBuffer = encoding.GetBytes(expectedString);
            using MarshalStream stream = new(new MemoryStream(testBuffer), true, 1);
            MarshalStreamReadStringResult result = stream.ReadString(encoding.GetDecoder(), -1, MarshalStreamNullTerminatorBehavior.Ignore);
            await Assert.That(result).IsEqualTo(new MarshalStreamReadStringResult(expectedString, testBuffer.Length, false, true));
        }
    }

    [Test]
    public async Task ReadString_FallbackCausingGrowingBuffer_RoundTripsCorrectly()
    {
        const string testString = "dকাঁañ";
        const string fallbackString = "?123456789123456789123456789?";
        byte[] testBuffer = Encoding.UTF8.GetBytes(testString);
        Decoder decoder = Encoding.ASCII.GetDecoder();
        decoder.Fallback = new DecoderReplacementFallback(fallbackString);
        string expectedString;
        {
            char[] characterBuffer = new char[1024];
            int charactersWritten = decoder.GetChars(testBuffer, characterBuffer, true);
            expectedString = new string(characterBuffer, 0, charactersWritten);
        }
        using MarshalStream stream = new(testBuffer);
        MarshalStreamReadStringResult result = stream.ReadString(decoder, -1, MarshalStreamNullTerminatorBehavior.Ignore);
        await Assert.That(result).IsEqualTo(new MarshalStreamReadStringResult(expectedString, testBuffer.Length, false, true));
    }

    [Test]
    public async Task ReadString_MaxBytesOnStandardString_StringTruncated()
    {
        const string testString = "test";
        const int maxBytesToRead = 2;
        const string expectedString = "te";
        byte[] testBuffer = Encoding.UTF8.GetBytes(testString);
        using MarshalStream stream = new(testBuffer);
        MarshalStreamReadStringResult result = stream.ReadString(Encoding.UTF8.GetDecoder(), maxBytesToRead, MarshalStreamNullTerminatorBehavior.Ignore);
        await Assert.That(result).IsEqualTo(new MarshalStreamReadStringResult(expectedString, maxBytesToRead, false, false));
    }

    [Test]
    public async Task ReadString_MaxBytesOnNullTerminatedString_NullTerminatorWins()
    {
        const string testString = "test\0abcdef";
        const int maxBytesToRead = 8;
        const string expectedString = "test";
        byte[] testBuffer = Encoding.UTF8.GetBytes(testString);
        using MarshalStream stream = new(testBuffer);
        MarshalStreamReadStringResult result = stream.ReadString(Encoding.UTF8.GetDecoder(), maxBytesToRead, MarshalStreamNullTerminatorBehavior.Stop);
        await Assert.That(result).IsEqualTo(new MarshalStreamReadStringResult(expectedString, expectedString.Length + 1, true, false));
    }

    [Test]
    public async Task ReadString_MaxBytesOnIncompleteSequence_IncompleteBytesFlushed()
    {
        const string testString = "dকাঁañ";
        const int maxBytesToRead = 2;
        const string expectedString = "d�";
        byte[] testBuffer = Encoding.UTF8.GetBytes(testString);
        using MarshalStream stream = new(testBuffer);
        MarshalStreamReadStringResult result = stream.ReadString(Encoding.UTF8.GetDecoder(), maxBytesToRead, MarshalStreamNullTerminatorBehavior.Ignore);
        await Assert.That(result).IsEqualTo(new MarshalStreamReadStringResult(expectedString, maxBytesToRead, false, false));
    }

    [Test]
    public async Task ReadString_TrimTrailingNullTerminators_EmbeddedNullsStillExist()
    {
        const string testString = "test\0abc\0\0";
        const string expectedString = "test\0abc";
        byte[] testBuffer = Encoding.UTF8.GetBytes(testString);
        using MarshalStream stream = new(testBuffer);
        MarshalStreamReadStringResult result = stream.ReadString(Encoding.UTF8.GetDecoder(), -1, MarshalStreamNullTerminatorBehavior.TrimTrailing);
        await Assert.That(result).IsEqualTo(new MarshalStreamReadStringResult(expectedString, testString.Length, false, true));
    }

    [Test]
    public async Task ReadString_TrimTrailingNullTerminatorsWithMaxLength_EmbeddedNullsStillExist()
    {
        const string testString = "test\0abc\0\0foo";
        const int maxBytesToRead = 10;
        const string expectedString = "test\0abc";
        byte[] testBuffer = Encoding.UTF8.GetBytes(testString);
        using MarshalStream stream = new(testBuffer);
        MarshalStreamReadStringResult result = stream.ReadString(Encoding.UTF8.GetDecoder(), maxBytesToRead, MarshalStreamNullTerminatorBehavior.TrimTrailing);
        await Assert.That(result).IsEqualTo(new MarshalStreamReadStringResult(expectedString, maxBytesToRead, false, false));
    }

    [Test]
    public async Task ReadString_TrimTrailingNullTerminatorsOnlyNullsWithMaxLengthAndOneByteBuffer_YieldsEmptyString()
    {
        const string testString = "\0\0\0\0\0\0\0";
        const int maxBytesToRead = 4;
        const string expectedString = "";
        byte[] testBuffer = Encoding.UTF8.GetBytes(testString);
        using MarshalStream stream = new(new MemoryStream(testBuffer), true, 1);
        MarshalStreamReadStringResult result = stream.ReadString(Encoding.UTF8.GetDecoder(), maxBytesToRead, MarshalStreamNullTerminatorBehavior.TrimTrailing);
        await Assert.That(result).IsEqualTo(new MarshalStreamReadStringResult(expectedString, maxBytesToRead, false, false));
    }

    [Test]
    public async Task ReadStringAsync_MultiByteCharacterStopOnEmbeddedNull_RoundTripsCorrectly()
    {
        const string expectedString = "dকাঁañ";
        const string expectedStringPlusNullTerminator = expectedString + "\0";
        const string testString = expectedStringPlusNullTerminator + "test";
        foreach (Encoding encoding in new[] { Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32 })
        {
            byte[] testBuffer = encoding.GetBytes(testString);
            int expectedBytesRead = encoding.GetByteCount(expectedStringPlusNullTerminator);
            using MarshalStream stream = new(testBuffer);
            MarshalStreamReadStringResult result = await stream.ReadStringAsync(encoding.GetDecoder(), -1, MarshalStreamNullTerminatorBehavior.Stop);
            await Assert.That(result).IsEqualTo(new MarshalStreamReadStringResult(expectedString, expectedBytesRead, true, false));
        }
    }

    [Test]
    public async Task ReadStringAsync_SingleCharacterBufferStopOnEmbeddedNull_RoundTripsCorrectly()
    {
        const string expectedString = "dকাঁañ";
        const string expectedStringPlusNullTerminator = expectedString + "\0";
        const string testString = expectedStringPlusNullTerminator + "test";
        foreach (Encoding encoding in new[] { Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32 })
        {
            byte[] testBuffer = encoding.GetBytes(testString);
            int expectedBytesRead = encoding.GetByteCount(expectedStringPlusNullTerminator);
            using MarshalStream stream = new(new MemoryStream(testBuffer), true, 1);
            MarshalStreamReadStringResult result = await stream.ReadStringAsync(encoding.GetDecoder(), -1, MarshalStreamNullTerminatorBehavior.Stop);
            await Assert.That(result).IsEqualTo(new MarshalStreamReadStringResult(expectedString, expectedBytesRead, true, false));
        }
    }

    [Test]
    public async Task ReadStringAsync_MultiByteCharacterDoNotStopOnEmbeddedNull_RoundTripsCorrectly()
    {
        const string expectedString = "dকাঁañ\0test";
        foreach (Encoding encoding in new[] { Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32 })
        {
            byte[] testBuffer = encoding.GetBytes(expectedString);
            using MarshalStream stream = new(testBuffer);
            MarshalStreamReadStringResult result = await stream.ReadStringAsync(encoding.GetDecoder(), -1, MarshalStreamNullTerminatorBehavior.Ignore);
            await Assert.That(result).IsEqualTo(new MarshalStreamReadStringResult(expectedString, testBuffer.Length, false, true));
        }
    }

    [Test]
    public async Task ReadStringAsync_SingleCharacterBufferDoNotStopOnEmbeddedNull_RoundTripsCorrectly()
    {
        const string expectedString = "dকাঁañ\0test";
        foreach (Encoding encoding in new[] { Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32 })
        {
            byte[] testBuffer = encoding.GetBytes(expectedString);
            using MarshalStream stream = new(new MemoryStream(testBuffer), true, 1);
            MarshalStreamReadStringResult result = await stream.ReadStringAsync(encoding.GetDecoder(), -1, MarshalStreamNullTerminatorBehavior.Ignore);
            await Assert.That(result).IsEqualTo(new MarshalStreamReadStringResult(expectedString, testBuffer.Length, false, true));
        }
    }

    [Test]
    public async Task ReadStringAsync_FallbackCausingGrowingBuffer_RoundTripsCorrectly()
    {
        const string testString = "dকাঁañ";
        const string fallbackString = "?123456789123456789123456789?";
        byte[] testBuffer = Encoding.UTF8.GetBytes(testString);
        Decoder decoder = Encoding.ASCII.GetDecoder();
        decoder.Fallback = new DecoderReplacementFallback(fallbackString);
        string expectedString;
        {
            char[] characterBuffer = new char[1024];
            int charactersWritten = decoder.GetChars(testBuffer, characterBuffer, true);
            expectedString = new string(characterBuffer, 0, charactersWritten);
        }
        using MarshalStream stream = new(testBuffer);
        MarshalStreamReadStringResult result = await stream.ReadStringAsync(decoder, -1, MarshalStreamNullTerminatorBehavior.Ignore);
        await Assert.That(result).IsEqualTo(new MarshalStreamReadStringResult(expectedString, testBuffer.Length, false, true));
    }

    [Test]
    public async Task ReadStringAsync_MaxBytesOnStandardString_StringTruncated()
    {
        const string testString = "test";
        const int maxBytesToRead = 2;
        const string expectedString = "te";
        byte[] testBuffer = Encoding.UTF8.GetBytes(testString);
        using MarshalStream stream = new(testBuffer);
        MarshalStreamReadStringResult result = await stream.ReadStringAsync(Encoding.UTF8.GetDecoder(), maxBytesToRead, MarshalStreamNullTerminatorBehavior.Ignore);
        await Assert.That(result).IsEqualTo(new MarshalStreamReadStringResult(expectedString, maxBytesToRead, false, false));
    }

    [Test]
    public async Task ReadStringAsync_MaxByteUnalignedWithBuffer_MaxBytesIsHonored()
    {
        const string testString = "testing";
        const int maxBytesToRead = 3;
        const string expectedString = "tes";
        byte[] testBuffer = Encoding.UTF8.GetBytes(testString);
        using MarshalStream stream = new(new MemoryStream(testBuffer), true, 2);
        MarshalStreamReadStringResult result = await stream.ReadStringAsync(Encoding.UTF8.GetDecoder(), maxBytesToRead, MarshalStreamNullTerminatorBehavior.Ignore);
        await Assert.That(result).IsEqualTo(new MarshalStreamReadStringResult(expectedString, maxBytesToRead, false, false));
    }

    [Test]
    public async Task ReadStringAsync_MaxBytesOnNullTerminatedString_NullTerminatorWins()
    {
        const string testString = "test\0abcdef";
        const int maxBytesToRead = 8;
        const string expectedString = "test";
        byte[] testBuffer = Encoding.UTF8.GetBytes(testString);
        using MarshalStream stream = new(testBuffer);
        MarshalStreamReadStringResult result = await stream.ReadStringAsync(Encoding.UTF8.GetDecoder(), maxBytesToRead, MarshalStreamNullTerminatorBehavior.Stop);
        await Assert.That(result).IsEqualTo(new MarshalStreamReadStringResult(expectedString, expectedString.Length + 1, true, false));
    }

    [Test]
    public async Task ReadStringAsync_MaxBytesOnIncompleteSequence_IncompleteBytesFlushed()
    {
        const string testString = "dকাঁañ";
        const int maxBytesToRead = 2;
        const string expectedString = "d�";
        byte[] testBuffer = Encoding.UTF8.GetBytes(testString);
        using MarshalStream stream = new(testBuffer);
        MarshalStreamReadStringResult result = await stream.ReadStringAsync(Encoding.UTF8.GetDecoder(), maxBytesToRead, MarshalStreamNullTerminatorBehavior.Ignore);
        await Assert.That(result).IsEqualTo(new MarshalStreamReadStringResult(expectedString, maxBytesToRead, false, false));
    }

    [Test]
    public async Task ReadStringAsync_TrimTrailingNullTerminators_EmbeddedNullsStillExist()
    {
        const string testString = "test\0abc\0\0";
        const string expectedString = "test\0abc";
        byte[] testBuffer = Encoding.UTF8.GetBytes(testString);
        using MarshalStream stream = new(testBuffer);
        MarshalStreamReadStringResult result = await stream.ReadStringAsync(Encoding.UTF8.GetDecoder(), -1, MarshalStreamNullTerminatorBehavior.TrimTrailing);
        await Assert.That(result).IsEqualTo(new MarshalStreamReadStringResult(expectedString, testString.Length, false, true));
    }

    [Test]
    public async Task ReadStringAsync_TrimTrailingNullTerminatorsWithMaxLength_EmbeddedNullsStillExist()
    {
        const string testString = "test\0abc\0\0foo";
        const int maxBytesToRead = 10;
        const string expectedString = "test\0abc";
        byte[] testBuffer = Encoding.UTF8.GetBytes(testString);
        using MarshalStream stream = new(testBuffer);
        MarshalStreamReadStringResult result = await stream.ReadStringAsync(Encoding.UTF8.GetDecoder(), maxBytesToRead, MarshalStreamNullTerminatorBehavior.TrimTrailing);
        await Assert.That(result).IsEqualTo(new MarshalStreamReadStringResult(expectedString, maxBytesToRead, false, false));
    }

    [Test]
    public async Task ReadStringAsync_TrimTrailingNullTerminatorsOnlyNullsWithMaxLengthAndOneByteBuffer_YieldsEmptyString()
    {
        const string testString = "\0\0\0\0\0\0\0";
        const int maxBytesToRead = 4;
        const string expectedString = "";
        byte[] testBuffer = Encoding.UTF8.GetBytes(testString);
        using MarshalStream stream = new(new MemoryStream(testBuffer), true, 1);
        MarshalStreamReadStringResult result = await stream.ReadStringAsync(Encoding.UTF8.GetDecoder(), maxBytesToRead, MarshalStreamNullTerminatorBehavior.TrimTrailing);
        await Assert.That(result).IsEqualTo(new MarshalStreamReadStringResult(expectedString, maxBytesToRead, false, false));
    }

    #endregion

    #region Scan

    [Test]
    public async Task Scan_MatchTwoBytesAtThirdByte_PositionOnThirdByte()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, true, false)))
        {
            static int scanFunc(ReadOnlyMemory<byte> bytes) => bytes.Span[0] == 3 && bytes.Span[1] == 4 ? 0 : 1;
            await Assert.That(stream.Scan(2, -1, scanFunc)).IsEqualTo(new MarshalStreamScanResult(2, true, false));
            await Assert.That(stream.Position).IsEqualTo(2);
        }
    }

    [Test]
    public async Task Scan_MatchAtEndOfStream_PositionOnLastByte()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, true, false)))
        {
            static int scanFunc(ReadOnlyMemory<byte> bytes) => bytes.Span[0] == 10 ? 0 : 1;
            await Assert.That(stream.Scan(1, -1, scanFunc)).IsEqualTo(new MarshalStreamScanResult(9, true, false));
            await Assert.That(stream.Position).IsEqualTo(9);
        }
    }

    [Test]
    public async Task Scan_NoMatch_PositionAtEndOfStream()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, true, false)))
        {
            static int scanFunc(ReadOnlyMemory<byte> bytes) => 1;
            await Assert.That(stream.Scan(1, -1, scanFunc)).IsEqualTo(new MarshalStreamScanResult(10, false, true));
            await Assert.That(stream.Position).IsEqualTo(10);
        }
    }

    [Test]
    public async Task Scan_NoMatchAfterThreeMaxBytes_PositionAtFourthByte()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, true, false)))
        {
            static int scanFunc(ReadOnlyMemory<byte> bytes) => 1;
            await Assert.That(stream.Scan(1, 3, scanFunc)).IsEqualTo(new MarshalStreamScanResult(3, false, false));
            await Assert.That(stream.Position).IsEqualTo(3);
        }
    }

    [Test]
    public async Task Scan_CopyReadBitsToDestinationShortBuffer_DestinationBufferIsCorrect()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, true, false)))
        {
            var destinationBuffer = new byte[3];
            static int scanFunc(ReadOnlyMemory<byte> bytes) => 1;
            await Assert.That(stream.Scan(destinationBuffer, 1, scanFunc)).IsEqualTo(new MarshalStreamScanResult(3, false, false));
            await Assert.That(stream.Position).IsEqualTo(3);
            await Assert.That(destinationBuffer).IsEquivalentTo(Slice(s_sourceBuffer, 0, 3));
        }
    }

    [Test]
    public async Task Scan_CopyReadBitsToDestinationStream_DestinationStreamIsCorrect()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, true, false)))
        {
            using MemoryStream destinationStream = new();
            static int scanFunc(ReadOnlyMemory<byte> bytes) => 1;
            await Assert.That(stream.Scan(destinationStream, 1, 3, scanFunc)).IsEqualTo(new MarshalStreamScanResult(3, false, false));
            await Assert.That(stream.Position).IsEqualTo(3);
            await Assert.That(destinationStream.ToArray()).IsEquivalentTo(Slice(s_sourceBuffer, 0, 3));
        }
    }

    [Test]
    public async Task ScanAsync_MatchTwoBytesAtThirdByte_PositionOnThirdByte()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, true, false)))
        {
            static int scanFunc(ReadOnlyMemory<byte> bytes) => bytes.Span[0] == 3 && bytes.Span[1] == 4 ? 0 : 1;
            await Assert.That(async () => await stream.ScanAsync(2, -1, scanFunc)).IsEqualTo(new MarshalStreamScanResult(2, true, false));
            await Assert.That(stream.Position).IsEqualTo(2);
        }
    }

    [Test]
    public async Task ScanAsync_MatchAtEndOfStream_PositionOnLastByte()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, true, false)))
        {
            static int scanFunc(ReadOnlyMemory<byte> bytes) => bytes.Span[0] == 10 ? 0 : 1;
            await Assert.That(async () => await stream.ScanAsync(1, -1, scanFunc)).IsEqualTo(new MarshalStreamScanResult(9, true, false));
            await Assert.That(stream.Position).IsEqualTo(9);
        }
    }

    [Test]
    public async Task ScanAsync_NoMatch_PositionAtEndOfStream()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, true, false)))
        {
            static int scanFunc(ReadOnlyMemory<byte> bytes) => 1;
            await Assert.That(async () => await stream.ScanAsync(1, -1, scanFunc)).IsEqualTo(new MarshalStreamScanResult(10, false, true));
            await Assert.That(stream.Position).IsEqualTo(10);
        }
    }

    [Test]
    public async Task ScanAsync_NoMatchAfterThreeMaxBytes_PositionAtFourthByte()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, true, false)))
        {
            static int scanFunc(ReadOnlyMemory<byte> bytes) => 1;
            await Assert.That(async () => await stream.ScanAsync(1, 3, scanFunc)).IsEqualTo(new MarshalStreamScanResult(3, false, false));
            await Assert.That(stream.Position).IsEqualTo(3);
        }
    }

    [Test]
    public async Task ScanAsync_CopyReadBitsToDestinationShortBuffer_DestinationBufferIsCorrect()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, true, false)))
        {
            var destinationBuffer = new byte[3];
            static int scanFunc(ReadOnlyMemory<byte> bytes) => 1;
            await Assert.That(async () => await stream.ScanAsync(destinationBuffer, 1, scanFunc)).IsEqualTo(new MarshalStreamScanResult(3, false, false));
            await Assert.That(stream.Position).IsEqualTo(3);
            await Assert.That(destinationBuffer).IsEquivalentTo(Slice(s_sourceBuffer, 0, 3));
        }
    }

    [Test]
    public async Task ScanAsync_CopyReadBitsToDestinationStream_DestinationStreamIsCorrect()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, true, false)))
        {
            using MemoryStream destinationStream = new();
            static int scanFunc(ReadOnlyMemory<byte> bytes) => 1;
            await Assert.That(async () => await stream.ScanAsync(destinationStream, 1, 3, scanFunc)).IsEqualTo(new MarshalStreamScanResult(3, false, false));
            await Assert.That(stream.Position).IsEqualTo(3);
            await Assert.That(destinationStream.ToArray()).IsEquivalentTo(Slice(s_sourceBuffer, 0, 3));
        }
    }

    #endregion // Scan

    #region Seek

    [Test]
    public async Task Seek_OriginBeginToLastByte_AtLastByte()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, true, false)))
        {
            await Assert.That(stream.Seek(s_sourceBuffer.Length - 1, SeekOrigin.Begin)).IsEqualTo(s_sourceBuffer.Length - 1);
            await Assert.That(stream.ReadByte()).IsEqualTo(s_sourceBuffer.Length);
        }
    }

    [Test]
    public async Task SeekAsync_OriginBeginToLastByte_AtLastByte()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, true, false)))
        {
            await Assert.That(async () => await stream.SeekAsync(s_sourceBuffer.Length - 1, SeekOrigin.Begin)).IsEqualTo(s_sourceBuffer.Length - 1);
            await Assert.That(stream.ReadByte()).IsEqualTo(s_sourceBuffer[s_sourceBuffer.Length - 1]);
        }
    }

    [Test]
    public async Task Seek_OriginEndToFirstByte_AtFirstByte()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, true, false)))
        {
            await Assert.That(stream.Seek(-s_sourceBuffer.Length, SeekOrigin.End)).IsEqualTo(0);
            await Assert.That(stream.ReadByte()).IsEqualTo(s_sourceBuffer[0]);
        }
    }

    [Test]
    public async Task SeekAsync_OriginEndToFirstByte_AtFirstByte()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, true, false)))
        {
            await Assert.That(async () => await stream.SeekAsync(-s_sourceBuffer.Length, SeekOrigin.End)).IsEqualTo(0);
            await Assert.That(stream.ReadByte()).IsEqualTo(s_sourceBuffer[0]);
        }
    }

    [Test]
    public async Task Seek_OriginCurrentGoingBackwardAndForward_AtExpectedBytes()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, true, false)))
        {
            await Assert.That(stream.Seek(s_sourceBuffer.Length - 1, SeekOrigin.Current)).IsEqualTo(s_sourceBuffer.Length - 1);
            await Assert.That(stream.ReadByte()).IsEqualTo(s_sourceBuffer[s_sourceBuffer.Length - 1]);
            await Assert.That(stream.Seek(-(s_sourceBuffer.Length - 1), SeekOrigin.Current)).IsEqualTo(1);
            await Assert.That(stream.ReadByte()).IsEqualTo(s_sourceBuffer[1]);
            await Assert.That(stream.Seek(1, SeekOrigin.Current)).IsEqualTo(3);
            await Assert.That(stream.ReadByte()).IsEqualTo(s_sourceBuffer[3]);
        }
    }

    [Test]
    public async Task SeekAsync_OriginCurrentGoingBackwardAndForward_AtExpectedBytes()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, true, false)))
        {
            await Assert.That(async () => await stream.SeekAsync(s_sourceBuffer.Length - 1, SeekOrigin.Current)).IsEqualTo(s_sourceBuffer.Length - 1);
            await Assert.That(stream.ReadByte()).IsEqualTo(s_sourceBuffer[s_sourceBuffer.Length - 1]);
            await Assert.That(async () => await stream.SeekAsync(-(s_sourceBuffer.Length - 1), SeekOrigin.Current)).IsEqualTo(1);
            await Assert.That(stream.ReadByte()).IsEqualTo(s_sourceBuffer[1]);
            await Assert.That(async () => await stream.SeekAsync(1, SeekOrigin.Current)).IsEqualTo(3);
            await Assert.That(stream.ReadByte()).IsEqualTo(s_sourceBuffer[3]);
        }
    }

    [Test]
    public async Task Seek_AfterStream_AtEndOfStream()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, true, false)))
        {
            await Assert.That(stream.Seek(s_sourceBuffer.Length + 1, SeekOrigin.Current)).IsEqualTo(s_sourceBuffer.Length + 1);
            await Assert.That(stream.ReadByte()).IsEqualTo(-1);
            await Assert.That(stream.Seek(-1, SeekOrigin.Current)).IsEqualTo(s_sourceBuffer.Length);
            await Assert.That(stream.ReadByte()).IsEqualTo(-1);
            await Assert.That(stream.Seek(s_sourceBuffer.Length + 10, SeekOrigin.Begin)).IsEqualTo(s_sourceBuffer.Length + 10);
            await Assert.That(stream.ReadByte()).IsEqualTo(-1);
            await Assert.That(stream.Seek(3, SeekOrigin.End)).IsEqualTo(s_sourceBuffer.Length + 3);
            await Assert.That(stream.ReadByte()).IsEqualTo(-1);
        }
    }

    [Test]
    public async Task SeekAsync_AfterStream_AtEndOfStream()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, true, false)))
        {
            await Assert.That(async () => await stream.SeekAsync(s_sourceBuffer.Length + 1, SeekOrigin.Current)).IsEqualTo(s_sourceBuffer.Length + 1);
            await Assert.That(stream.ReadByte()).IsEqualTo(-1);
            await Assert.That(async () => await stream.SeekAsync(-1, SeekOrigin.Current)).IsEqualTo(s_sourceBuffer.Length);
            await Assert.That(stream.ReadByte()).IsEqualTo(-1);
            await Assert.That(async () => await stream.SeekAsync(s_sourceBuffer.Length + 10, SeekOrigin.Begin)).IsEqualTo(s_sourceBuffer.Length + 10);
            await Assert.That(stream.ReadByte()).IsEqualTo(-1);
            await Assert.That(async () => await stream.SeekAsync(3, SeekOrigin.End)).IsEqualTo(s_sourceBuffer.Length + 3);
            await Assert.That(stream.ReadByte()).IsEqualTo(-1);
        }
    }

    #endregion // Seek

    #region SetLength

    [Test]
    public async Task SetLength_ShrinkAndGrow_UnderlyingBufferSizeChanged()
    {
        MemoryStream underlyingStream = new();
        using MarshalStream stream = new(underlyingStream);
        stream.SetLength(1);
        await Assert.That(underlyingStream.Length).IsEqualTo(1);
        stream.SetLength(5);
        await Assert.That(underlyingStream.Length).IsEqualTo(5);
    }

    [Test]
    public async Task SetLength_ShrinkLessThanPosition_PositionAfterEndOfStream()
    {
        MemoryStream underlyingStream = new(s_sourceBuffer, true);
        using MarshalStream stream = new(underlyingStream);
        stream.Position = s_sourceBuffer.Length - 1;
        stream.SetLength(s_sourceBuffer.Length - 2);
        await Assert.That(stream.Position).IsEqualTo(s_sourceBuffer.Length - 2);
        await Assert.That(stream.ReadByte()).IsEqualTo(-1);
    }

    #endregion // SetLength

    #region Skip

    [Test]
    public async Task Skip_LessThanWholeStreamWithExistingBuffer_PositionBeforeEndOfStream()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, true, false)))
        {
            await Assert.That(stream.ReadByte()).IsEqualTo(1);
            await Assert.That(stream.Skip(s_sourceBuffer.Length - 3)).IsEqualTo(s_sourceBuffer.Length - 3);
            await Assert.That(stream.Position).IsEqualTo(s_sourceBuffer.Length - 2);
        }
    }

    [Test]
    public async Task Skip_MoreThanWholeStreamWithNothingBuffered_PositionAtEndOfStream()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, true, false)))
        {
            await Assert.That(stream.Skip(s_sourceBuffer.Length + 1)).IsEqualTo(s_sourceBuffer.Length);
            await Assert.That(stream.Position).IsEqualTo(s_sourceBuffer.Length);
        }
    }

    [Test]
    public async Task SkipAsync_LessThanWholeStreamWithExistingBuffer_PositionBeforeEndOfStream()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, true, false)))
        {
            await Assert.That(stream.ReadByte()).IsEqualTo(1);
            await Assert.That(async () => await stream.SkipAsync(s_sourceBuffer.Length - 3)).IsEqualTo(s_sourceBuffer.Length - 3);
            await Assert.That(stream.Position).IsEqualTo(s_sourceBuffer.Length - 2);
        }
    }

    [Test]
    public async Task SkipAsync_MoreThanWholeStreamWithNothingBuffered_PositionAtEndOfStream()
    {
        foreach (MarshalStream stream in Iterate(
            CreateForFixedBuffer(),
            CreateForStream(true, true, false)))
        {
            await Assert.That(async () => await stream.SkipAsync(s_sourceBuffer.Length + 1)).IsEqualTo(s_sourceBuffer.Length);
            await Assert.That(stream.Position).IsEqualTo(s_sourceBuffer.Length);
        }
    }

    #endregion // Skip

    #region Write

    [Test]
    public async Task Write_GrowStreamWithBuffer_UnderlyingStreamIsGrownOnlyAfterFlush()
    {
        var expectedFlushedBytes = s_sourceBuffer.Concat(new byte[] { 11, 12 }).ToArray();
        MemoryStream underlyingStream = new();
        underlyingStream.Write(s_sourceBuffer);
        using MarshalStream stream = new(underlyingStream);
        stream.Write([ 11, 12 ], 0, 2);
        await Assert.That(stream.Position).IsEqualTo(s_sourceBuffer.Length + 2);
        await Assert.That(stream.Length).IsEqualTo(s_sourceBuffer.Length + 2);
        await Assert.That(underlyingStream.ToArray()).IsEquivalentTo(s_sourceBuffer);
        stream.Flush();
        await Assert.That(stream.Position).IsEqualTo(s_sourceBuffer.Length + 2);
        await Assert.That(stream.Length).IsEqualTo(s_sourceBuffer.Length + 2);
        await Assert.That(underlyingStream.ToArray()).IsEquivalentTo(expectedFlushedBytes);
    }

    [Test]
    public async Task Write_GrowStreamWithSpan_UnderlyingStreamIsGrownOnlyAfterFlush()
    {
        var expectedFlushedBytes = s_sourceBuffer.Concat(new byte[] { 11, 12 }).ToArray();
        MemoryStream underlyingStream = new();
        underlyingStream.Write(s_sourceBuffer);
        using MarshalStream stream = new(underlyingStream);
        stream.Write([11, 12]);
        await Assert.That(stream.Position).IsEqualTo(s_sourceBuffer.Length + 2);
        await Assert.That(stream.Length).IsEqualTo(s_sourceBuffer.Length + 2);
        await Assert.That(underlyingStream.ToArray()).IsEquivalentTo(s_sourceBuffer);
        stream.Flush();
        await Assert.That(stream.Position).IsEqualTo(s_sourceBuffer.Length + 2);
        await Assert.That(stream.Length).IsEqualTo(s_sourceBuffer.Length + 2);
        await Assert.That(underlyingStream.ToArray()).IsEquivalentTo(expectedFlushedBytes);
    }

    [Test]
    public async Task WriteAsync_GrowStreamWithBuffer_UnderlyingStreamIsGrownOnlyAfterFlush()
    {
        var expectedFlushedBytes = s_sourceBuffer.Concat(new byte[] { 11, 12 }).ToArray();
        MemoryStream underlyingStream = new();
        underlyingStream.Write(s_sourceBuffer);
        using MarshalStream stream = new(underlyingStream);
        await stream.WriteAsync([11, 12], 0, 2);
        await Assert.That(stream.Position).IsEqualTo(s_sourceBuffer.Length + 2);
        await Assert.That(stream.Length).IsEqualTo(s_sourceBuffer.Length + 2);
        await Assert.That(underlyingStream.ToArray()).IsEquivalentTo(s_sourceBuffer);
        stream.Flush();
        await Assert.That(stream.Position).IsEqualTo(s_sourceBuffer.Length + 2);
        await Assert.That(stream.Length).IsEqualTo(s_sourceBuffer.Length + 2);
        await Assert.That(underlyingStream.ToArray()).IsEquivalentTo(expectedFlushedBytes);
    }

    [Test]
    public async Task WriteAsync_GrowStreamWithMemory_UnderlyingStreamIsGrownOnlyAfterFlush()
    {
        var expectedFlushedBytes = s_sourceBuffer.Concat(new byte[] { 11, 12 }).ToArray();
        MemoryStream underlyingStream = new();
        underlyingStream.Write(s_sourceBuffer);
        using MarshalStream stream = new(underlyingStream);
        await stream.WriteAsync(new byte[] { 11, 12 });
        await Assert.That(stream.Position).IsEqualTo(s_sourceBuffer.Length + 2);
        await Assert.That(stream.Length).IsEqualTo(s_sourceBuffer.Length + 2);
        await Assert.That(underlyingStream.ToArray()).IsEquivalentTo(s_sourceBuffer);
        stream.Flush();
        await Assert.That(stream.Position).IsEqualTo(s_sourceBuffer.Length + 2);
        await Assert.That(stream.Length).IsEqualTo(s_sourceBuffer.Length + 2);
        await Assert.That(underlyingStream.ToArray()).IsEquivalentTo(expectedFlushedBytes);
    }

    [Test]
    public async Task Write_OverwriteOldBytesWithBuffer_UnderlyingBufferChanged()
    {
        byte[] reversedBytes = s_sourceBuffer.Reverse().ToArray();
        CustomMemoryStream underlyingStream = new(s_sourceBuffer, true, true, true);
        using MarshalStream stream = new(underlyingStream);
        stream.Write(reversedBytes, 0, reversedBytes.Length);
        stream.Flush();
        await Assert.That(underlyingStream.ToArray()).IsEquivalentTo(reversedBytes);
    }

    [Test]
    public async Task Write_OverwriteOldBytesWithSpan_UnderlyingBufferChanged()
    {
        byte[] reversedBytes = s_sourceBuffer.Reverse().ToArray();
        CustomMemoryStream underlyingStream = new(s_sourceBuffer, true, true, true);
        using MarshalStream stream = new(underlyingStream);
        stream.Write(reversedBytes.AsSpan());
        stream.Flush();
        await Assert.That(underlyingStream.ToArray()).IsEquivalentTo(reversedBytes);
    }

    [Test]
    public async Task WriteAsync_OverwriteOldBytesWithBuffer_UnderlyingBufferChanged()
    {
        byte[] reversedBytes = s_sourceBuffer.Reverse().ToArray();
        CustomMemoryStream underlyingStream = new(s_sourceBuffer, true, true, true);
        using MarshalStream stream = new(underlyingStream);
        await stream.WriteAsync(reversedBytes, 0, reversedBytes.Length);
        stream.Flush();
        await Assert.That(underlyingStream.ToArray()).IsEquivalentTo(reversedBytes);
    }

    [Test]
    public async Task WriteAsync_OverwriteOldBytesWithMemory_UnderlyingBufferChanged()
    {
        byte[] reversedBytes = s_sourceBuffer.Reverse().ToArray();
        CustomMemoryStream underlyingStream = new(s_sourceBuffer, true, true, true);
        using MarshalStream stream = new(underlyingStream);
        await stream.WriteAsync(reversedBytes.AsMemory());
        stream.Flush();
        await Assert.That(underlyingStream.ToArray()).IsEquivalentTo(reversedBytes);
    }

    #endregion // Write

    #region WriteByte

    [Test]
    public async Task WriteByte_GrowStreamWith_UnderlyingStreamIsGrownOnlyAfterFlush()
    {
        var expectedFlushedBytes = s_sourceBuffer.Concat(new byte[] { 11 }).ToArray();
        MemoryStream underlyingStream = new();
        underlyingStream.Write(s_sourceBuffer);
        using MarshalStream stream = new(underlyingStream);
        stream.WriteByte(11);
        await Assert.That(stream.Position).IsEqualTo(s_sourceBuffer.Length + 1);
        await Assert.That(stream.Length).IsEqualTo(s_sourceBuffer.Length + 1);
        await Assert.That(underlyingStream.ToArray()).IsEquivalentTo(s_sourceBuffer);
        stream.Flush();
        await Assert.That(stream.Position).IsEqualTo(s_sourceBuffer.Length + 1);
        await Assert.That(stream.Length).IsEqualTo(s_sourceBuffer.Length + 1);
        await Assert.That(underlyingStream.ToArray()).IsEquivalentTo(expectedFlushedBytes);
    }

    [Test]
    public async Task WriteByteAsync_GrowStreamWith_UnderlyingStreamIsGrownOnlyAfterFlush()
    {
        var expectedFlushedBytes = s_sourceBuffer.Concat(new byte[] { 11 }).ToArray();
        MemoryStream underlyingStream = new();
        underlyingStream.Write(s_sourceBuffer);
        using MarshalStream stream = new(underlyingStream);
        await stream.WriteByteAsync(11);
        await Assert.That(stream.Position).IsEqualTo(s_sourceBuffer.Length + 1);
        await Assert.That(stream.Length).IsEqualTo(s_sourceBuffer.Length + 1);
        await Assert.That(underlyingStream.ToArray()).IsEquivalentTo(s_sourceBuffer);
        stream.Flush();
        await Assert.That(stream.Position).IsEqualTo(s_sourceBuffer.Length + 1);
        await Assert.That(stream.Length).IsEqualTo(s_sourceBuffer.Length + 1);
        await Assert.That(underlyingStream.ToArray()).IsEquivalentTo(expectedFlushedBytes);
    }

    [Test]
    public async Task WriteByte_OverwriteOldByteWithBuffer_UnderlyingBufferChanged()
    {
        byte[] expectedBytes = [.. s_sourceBuffer];
        expectedBytes[0] = 77;
        CustomMemoryStream underlyingStream = new(s_sourceBuffer, true, true, true);
        using MarshalStream stream = new(underlyingStream);
        stream.WriteByte(expectedBytes[0]);
        stream.Flush();
        await Assert.That(underlyingStream.ToArray()).IsEquivalentTo(expectedBytes);
    }

    [Test]
    public async Task WriteByteAsync_OverwriteOldByteWithBuffer_UnderlyingBufferChanged()
    {
        byte[] expectedBytes = [.. s_sourceBuffer];
        expectedBytes[0] = 77;
        CustomMemoryStream underlyingStream = new(s_sourceBuffer, true, true, true);
        using MarshalStream stream = new(underlyingStream);
        await stream.WriteByteAsync(expectedBytes[0]);
        stream.Flush();
        await Assert.That(underlyingStream.ToArray()).IsEquivalentTo(expectedBytes);
    }

    #endregion // WriteByte

    #region WriteString

    [Test]
    public async Task WriteString_MinimumBuffer_StringEncodedToStream()
    {
        const string testString = "dকাঁañ";
        foreach (Encoding encoding in new[] { Encoding.ASCII, Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32 })
        {
            using MemoryStream memoryStream = new();
            using MarshalStream stream = new(memoryStream, false, 1);
            int bytesWritten = stream.WriteString(encoding.GetEncoder(), testString, false);
            stream.Flush();
            await Assert.That(bytesWritten).IsEqualTo((int)memoryStream.Length);
            await Assert.That(memoryStream.ToArray()).IsEquivalentTo(encoding.GetBytes(testString));
        }
    }

    [Test]
    public async Task WriteString_StreamBuffer_StringEncodedToStream()
    {
        const string testString = "dকাঁañ";
        foreach (Encoding encoding in new[] { Encoding.ASCII, Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32 })
        {
            using MemoryStream memoryStream = new();
            using MarshalStream stream = new(memoryStream, false);
            int bytesWritten = stream.WriteString(encoding.GetEncoder(), testString, false);
            stream.Flush();
            await Assert.That(bytesWritten).IsEqualTo((int)memoryStream.Length);
            await Assert.That(memoryStream.ToArray()).IsEquivalentTo(encoding.GetBytes(testString));
        }
    }

    [Test]
    public async Task WriteString_WriteNullTerminatorAndMinimumBuffer_NullTerminatorIsPersisted()
    {
        const string testString = "dকাঁañ";
        foreach (Encoding encoding in new[] { Encoding.ASCII, Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32 })
        {
            int nullTerminatorSize = encoding.GetByteCount(['\0']);
            int testStringSize = encoding.GetByteCount(testString);
            using MemoryStream memoryStream = new();
            using MarshalStream stream = new(memoryStream, false, 1);
            int bytesWritten = stream.WriteString(encoding.GetEncoder(), testString, true);
            stream.Flush();
            byte[] result = memoryStream.ToArray();
            await Assert.That(bytesWritten).IsEqualTo(result.Length);
            await Assert.That(result.Length).IsEqualTo(testStringSize + nullTerminatorSize);
            await Assert.That(Slice(result, testStringSize)).IsEquivalentTo(new byte[nullTerminatorSize]);
        }
    }

    [Test]
    public async Task WriteString_WriteNullTerminatorAndMinimumBufferSmallishStreamBuffer_NullTerminatorIsPersisted()
    {
        const string testString = "dকাঁañ";
        foreach (Encoding encoding in new[] { Encoding.ASCII, Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32 })
        {
            int nullTerminatorSize = encoding.GetByteCount(['\0']);
            int testStringSize = encoding.GetByteCount(testString);
            using MemoryStream memoryStream = new();
            using MarshalStream stream = new(memoryStream, false, 100);
            int bytesWritten = stream.WriteString(encoding.GetEncoder(), testString, true);
            stream.Flush();
            byte[] result = memoryStream.ToArray();
            await Assert.That(bytesWritten).IsEqualTo(result.Length);
            await Assert.That(result.Length).IsEqualTo(testStringSize + nullTerminatorSize);
            await Assert.That(Slice(result, testStringSize)).IsEquivalentTo(new byte[nullTerminatorSize]);
        }
    }

    [Test]
    public async Task WriteString_WriteNullTerminatorAndUsingStreamBuffer_NullTerminatorIsPersisted()
    {
        const string testString = "dকাঁañ";
        foreach (Encoding encoding in new[] { Encoding.ASCII, Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32 })
        {
            int nullTerminatorSize = encoding.GetByteCount(['\0']);
            int testStringSize = encoding.GetByteCount(testString);
            using MemoryStream memoryStream = new();
            using MarshalStream stream = new(memoryStream, false);
            int bytesWritten = stream.WriteString(encoding.GetEncoder(), testString, true);
            stream.Flush();
            byte[] result = memoryStream.ToArray();
            await Assert.That(bytesWritten).IsEqualTo(result.Length);
            await Assert.That(result.Length).IsEqualTo(testStringSize + nullTerminatorSize);
            await Assert.That(Slice(result, testStringSize)).IsEquivalentTo(new byte[nullTerminatorSize]);
        }
    }

    [Test]
    public async Task WriteString_OnlyNullTerminatorAndMinimumBuffer_OnlyNullTerminatorPersisted()
    {
        foreach (Encoding encoding in new[] { Encoding.ASCII, Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32 })
        {
            int nullTerminatorSize = encoding.GetByteCount(['\0']);
            using MemoryStream memoryStream = new();
            using MarshalStream stream = new(memoryStream, false, 1);
            int bytesWritten = stream.WriteString(encoding.GetEncoder(), string.Empty, true);
            stream.Flush();
            byte[] result = memoryStream.ToArray();
            await Assert.That(bytesWritten).IsEqualTo(result.Length);
            await Assert.That(memoryStream.ToArray()).IsEquivalentTo(new byte[nullTerminatorSize]);
        }
    }

    [Test]
    public async Task WriteString_OnlyNullTerminatorAndUsingStreamBuffer_OnlyNullTerminatorPersisted()
    {
        foreach (Encoding encoding in new[] { Encoding.ASCII, Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32 })
        {
            int nullTerminatorSize = encoding.GetByteCount(['\0']);
            using MemoryStream memoryStream = new();
            using MarshalStream stream = new(memoryStream, false);
            int bytesWritten = stream.WriteString(encoding.GetEncoder(), string.Empty, true);
            stream.Flush();
            byte[] result = memoryStream.ToArray();
            await Assert.That(bytesWritten).IsEqualTo(result.Length);
            await Assert.That(memoryStream.ToArray()).IsEquivalentTo(new byte[nullTerminatorSize]);
        }
    }

    [Test]
    public async Task WriteString_EmptyStringAndMinimumBuffer_NoBytesPersisted()
    {
        foreach (Encoding encoding in new[] { Encoding.ASCII, Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32 })
        {
            using MemoryStream memoryStream = new();
            using MarshalStream stream = new(memoryStream, false, 1);
            int bytesWritten = stream.WriteString(encoding.GetEncoder(), string.Empty, false);
            stream.Flush();
            await Assert.That(bytesWritten).IsEqualTo(0);
            await Assert.That(memoryStream.Length).IsEqualTo(0);
        }
    }

    [Test]
    public async Task WriteString_EmptyStringUsingStreamBuffer_NoBytesPersisted()
    {
        foreach (Encoding encoding in new[] { Encoding.ASCII, Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32 })
        {
            using MemoryStream memoryStream = new();
            using MarshalStream stream = new(memoryStream, false);
            int bytesWritten = stream.WriteString(encoding.GetEncoder(), string.Empty, false);
            stream.Flush();
            await Assert.That(bytesWritten).IsEqualTo(0);
            await Assert.That(memoryStream.Length).IsEqualTo(0);
        }
    }

    [Test]
    public async Task WriteStringAsync_MinimumBuffer_StringEncodedToStream()
    {
        const string testString = "dকাঁañ";
        foreach (Encoding encoding in new[] { Encoding.ASCII, Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32 })
        {
            using MemoryStream memoryStream = new();
            using MarshalStream stream = new(memoryStream, false, 1);
            int bytesWritten = await stream.WriteStringAsync(encoding.GetEncoder(), testString.AsMemory(), false);
            await stream.FlushAsync();
            await Assert.That(bytesWritten).IsEqualTo((int)memoryStream.Length);
            await Assert.That(memoryStream.ToArray()).IsEquivalentTo(encoding.GetBytes(testString));
        }
    }

    [Test]
    public async Task WriteStringAsync_StreamBuffer_StringEncodedToStream()
    {
        const string testString = "dকাঁañ";
        foreach (Encoding encoding in new[] { Encoding.ASCII, Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32 })
        {
            using MemoryStream memoryStream = new();
            using MarshalStream stream = new(memoryStream, false);
            int bytesWritten = await stream.WriteStringAsync(encoding.GetEncoder(), testString.AsMemory(), false);
            await stream.FlushAsync();
            await Assert.That(bytesWritten).IsEqualTo((int)memoryStream.Length);
            await Assert.That(memoryStream.ToArray()).IsEquivalentTo(encoding.GetBytes(testString));
        }
    }

    [Test]
    public async Task WriteStringAsync_WriteNullTerminatorAndMinimumBuffer_NullTerminatorIsPersisted()
    {
        const string testString = "dকাঁañ";
        foreach (Encoding encoding in new[] { Encoding.ASCII, Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32 })
        {
            int nullTerminatorSize = encoding.GetByteCount(['\0']);
            int testStringSize = encoding.GetByteCount(testString);
            using MemoryStream memoryStream = new();
            using MarshalStream stream = new(memoryStream, false, 1);
            int bytesWritten = await stream.WriteStringAsync(encoding.GetEncoder(), testString.AsMemory(), true);
            await stream.FlushAsync();
            byte[] result = memoryStream.ToArray();
            await Assert.That(bytesWritten).IsEqualTo(result.Length);
            await Assert.That(result.Length).IsEqualTo(testStringSize + nullTerminatorSize);
            await Assert.That(Slice(result, testStringSize)).IsEquivalentTo(new byte[nullTerminatorSize]);
        }
    }

    [Test]
    public async Task WriteStringAsync_WriteNullTerminatorAndMinimumBufferSmallishStreamBuffer_NullTerminatorIsPersisted()
    {
        const string testString = "dকাঁañ";
        foreach (Encoding encoding in new[] { Encoding.ASCII, Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32 })
        {
            int nullTerminatorSize = encoding.GetByteCount(['\0']);
            int testStringSize = encoding.GetByteCount(testString);
            using MemoryStream memoryStream = new();
            using MarshalStream stream = new(memoryStream, false, 100);
            int bytesWritten = await stream.WriteStringAsync(encoding.GetEncoder(), testString.AsMemory(), true);
            await stream.FlushAsync();
            byte[] result = memoryStream.ToArray();
            await Assert.That(bytesWritten).IsEqualTo(result.Length);
            await Assert.That(result.Length).IsEqualTo(testStringSize + nullTerminatorSize);
            await Assert.That(Slice(result, testStringSize)).IsEquivalentTo(new byte[nullTerminatorSize]);
        }
    }

    [Test]
    public async Task WriteStringAsync_WriteNullTerminatorAndUsingStreamBuffer_NullTerminatorIsPersisted()
    {
        const string testString = "dকাঁañ";
        foreach (Encoding encoding in new[] { Encoding.ASCII, Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32 })
        {
            int nullTerminatorSize = encoding.GetByteCount(['\0']);
            int testStringSize = encoding.GetByteCount(testString);
            using MemoryStream memoryStream = new();
            using MarshalStream stream = new(memoryStream, false);
            int bytesWritten = await stream.WriteStringAsync(encoding.GetEncoder(), testString.AsMemory(), true);
            await stream.FlushAsync();
            byte[] result = memoryStream.ToArray();
            await Assert.That(bytesWritten).IsEqualTo(result.Length);
            await Assert.That(result.Length).IsEqualTo(testStringSize + nullTerminatorSize);
            await Assert.That(Slice(result, testStringSize)).IsEquivalentTo(new byte[nullTerminatorSize]);
        }
    }

    [Test]
    public async Task WriteStringAsync_OnlyNullTerminatorAndMinimumBuffer_OnlyNullTerminatorPersisted()
    {
        foreach (Encoding encoding in new[] { Encoding.ASCII, Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32 })
        {
            int nullTerminatorSize = encoding.GetByteCount(['\0']);
            using MemoryStream memoryStream = new();
            using MarshalStream stream = new(memoryStream, false, 1);
            int bytesWritten = await stream.WriteStringAsync(encoding.GetEncoder(), string.Empty.AsMemory(), true);
            await stream.FlushAsync();
            byte[] result = memoryStream.ToArray();
            await Assert.That(bytesWritten).IsEqualTo(result.Length);
            await Assert.That(memoryStream.ToArray()).IsEquivalentTo(new byte[nullTerminatorSize]);
        }
    }

    [Test]
    public async Task WriteStringAsync_OnlyNullTerminatorAndUsingStreamBuffer_OnlyNullTerminatorPersisted()
    {
        foreach (Encoding encoding in new[] { Encoding.ASCII, Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32 })
        {
            int nullTerminatorSize = encoding.GetByteCount(['\0']);
            using MemoryStream memoryStream = new();
            using MarshalStream stream = new(memoryStream, false);
            int bytesWritten = await stream.WriteStringAsync(encoding.GetEncoder(), string.Empty.AsMemory(), true);
            await stream.FlushAsync();
            byte[] result = memoryStream.ToArray();
            await Assert.That(bytesWritten).IsEqualTo(result.Length);
            await Assert.That(memoryStream.ToArray()).IsEquivalentTo(new byte[nullTerminatorSize]);
        }
    }

    [Test]
    public async Task WriteStringAsync_EmptyStringAndMinimumBuffer_NoBytesPersisted()
    {
        foreach (Encoding encoding in new[] { Encoding.ASCII, Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32 })
        {
            using MemoryStream memoryStream = new();
            using MarshalStream stream = new(memoryStream, false, 1);
            int bytesWritten = await stream.WriteStringAsync(encoding.GetEncoder(), string.Empty.AsMemory(), false);
            await stream.FlushAsync();
            await Assert.That(bytesWritten).IsEqualTo(0);
            await Assert.That(memoryStream.Length).IsEqualTo(0);
        }
    }

    [Test]
    public async Task WriteStringAsync_EmptyStringUsingStreamBuffer_NoBytesPersisted()
    {
        foreach (Encoding encoding in new[] { Encoding.ASCII, Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, Encoding.UTF32 })
        {
            using MemoryStream memoryStream = new();
            using MarshalStream stream = new(memoryStream, false);
            int bytesWritten = await stream.WriteStringAsync(encoding.GetEncoder(), string.Empty.AsMemory(), false);
            await stream.FlushAsync();
            await Assert.That(bytesWritten).IsEqualTo(0);
            await Assert.That(memoryStream.Length).IsEqualTo(0);
        }
    }

    #endregion WriteString
}

#region CustomMemoryStream

file class CustomMemoryStream : MemoryStream
{
    private bool _canRead;
    private bool _canSeek;
    private bool _canWrite;

    public CustomMemoryStream(byte[]? buffer, bool canRead, bool canSeek, bool canWrite) : base()
    {
        if (buffer is not null)
        {
            _canWrite = true;
            _canSeek = true;
            Write(buffer.AsSpan());
            Position = 0;
        }
        _canRead = canRead;
        _canSeek = canSeek;
        _canWrite = canWrite;
    }

    public override bool CanRead => _canRead && base.CanRead;

    public override bool CanSeek => _canSeek && base.CanSeek;

    public override bool CanWrite => _canWrite && base.CanWrite;

    public override long Length => _canSeek ? base.Length : throw new NotSupportedException("Cannot seek.");

    public override long Position
    {
        get => _canSeek ? base.Position : throw new NotSupportedException("Cannot seek.");
        set
        {
            if (_canSeek)
            {
                base.Position = value;
            }
            else
            {
                throw new NotSupportedException("Cannot seek.");
            }
        }
    }

    public override int Read(byte[] buffer, int offset, int count) =>
        _canRead ? base.Read(buffer, offset, count) : throw new NotSupportedException("Cannot read.");

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken) =>
        _canRead ? base.ReadAsync(buffer, cancellationToken) : throw new NotSupportedException("Cannot read.");

    public override long Seek(long offset, SeekOrigin loc) =>
        _canSeek ? base.Seek(offset, loc) : throw new NotSupportedException("Cannot seek.");

    public override void SetLength(long value)
    {
        if (_canSeek && _canWrite)
        {
            base.SetLength(value);
        }
        else
        {
            throw new NotSupportedException("Cannot seek and write.");
        }
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        if (_canWrite)
        {
            base.Write(buffer, offset, count);
        }
        else
        {
            throw new NotSupportedException("Cannot write.");
        }
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        if (_canWrite)
        {
            base.Write(buffer);
        }
        else
        {
            throw new NotSupportedException("Cannot write.");
        }
    }

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        _canWrite ? base.WriteAsync(buffer, offset, count, cancellationToken) : throw new NotSupportedException("Cannot write.");

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) =>
        _canWrite ? base.WriteAsync(buffer, cancellationToken) : throw new NotSupportedException("Cannot write.");
}

#endregion CustomMemoryStream

#region CustomProcessor

file class CustomProcessor(Action<ReadOnlySpan<byte>> process) : IMarshalStreamProcessor
{
    public void Process(ReadOnlySpan<byte> bytes) => process(bytes);
}

#endregion CustomProcessor

#region ValidationProcessor

file class ValidationProcessor(Memory<byte> expectedBytes) : IMarshalStreamProcessor
{
    private bool _foundMatch = false;

    public void Process(ReadOnlySpan<byte> bytes)
    {
        if (_foundMatch ||
            bytes.Length > expectedBytes.Length ||
            !bytes.SequenceEqual(expectedBytes.Span[..bytes.Length]))
        {
            _foundMatch = true;
            return;
        }

        expectedBytes = expectedBytes[bytes.Length..];
    }

    public bool Success => !_foundMatch && expectedBytes.Length == 0;
}

#endregion ValidationProcessor
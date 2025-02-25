namespace Phaeyz.Marshalling.Test;

internal class ScopedReadStreamTest
{
    private static readonly byte[] s_streamData = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

    private static int ReadUntilEnd(Stream stream, int chunkSize)
    {
        byte[] chunk = new byte[chunkSize];
        int nextExpectedByte = 1;
        int totalBytesRead = 0;
        while (true)
        {
            int bytesRead = stream.Read(chunk, 0, chunkSize);
            if (bytesRead == 0)
            {
                return totalBytesRead;
            }
            totalBytesRead += bytesRead;
            for (int i = 0; i < bytesRead; i++)
            {
                if (nextExpectedByte++ != chunk[i])
                {
                    throw new Exception("Unexpected data returned.");
                }
            }
        }
    }

    [Test]
    public async Task Read_ChunkBiggerThanScope_FullStreamRead()
    {
        using MemoryStream stream = new(s_streamData, false);
        using (ScopedReadStream scopedReadStream = new(stream, s_streamData.Length))
        {
            await Assert.That(ReadUntilEnd(scopedReadStream, s_streamData.Length * 2)).IsEqualTo(s_streamData.Length);
        }
        await Assert.That(stream.Position).IsEqualTo(s_streamData.Length);
    }

    [Test]
    public async Task Read_OneByteChunkOnFullStream_FullStreamRead()
    {
        using MemoryStream stream = new(s_streamData, false);
        using (ScopedReadStream scopedReadStream = new(stream, s_streamData.Length))
        {
            await Assert.That(ReadUntilEnd(scopedReadStream, 1)).IsEqualTo(s_streamData.Length);
        }
        await Assert.That(stream.Position).IsEqualTo(s_streamData.Length);
    }

    [Test]
    public async Task Read_ScopeBiggerThanStream_FullStreamRead()
    {
        using MemoryStream stream = new(s_streamData, false);
        using (ScopedReadStream scopedReadStream = new(stream, s_streamData.Length * 2))
        {
            await Assert.That(ReadUntilEnd(scopedReadStream, 3)).IsEqualTo(s_streamData.Length);
        }
        await Assert.That(stream.Position).IsEqualTo(s_streamData.Length);
    }

    [Test]
    public async Task Read_ThreeByteChunkOnHalfStream_HalfStreamRead()
    {
        using MemoryStream stream = new(s_streamData, false);
        using (ScopedReadStream scopedReadStream = new(stream, s_streamData.Length / 2))
        {
            await Assert.That(ReadUntilEnd(scopedReadStream, 3)).IsEqualTo(s_streamData.Length / 2);
        }
        await Assert.That(stream.Position).IsEqualTo(s_streamData.Length / 2);
    }

    [Test]
    public async Task Read_ZeroSizeScope_ZeroBytesRead()
    {
        using MemoryStream stream = new(s_streamData, false);
        using (ScopedReadStream scopedReadStream = new(stream, 0))
        {
            await Assert.That(ReadUntilEnd(scopedReadStream, 1)).IsEqualTo(0);
        }
        await Assert.That(stream.Position).IsEqualTo(0);
    }
}
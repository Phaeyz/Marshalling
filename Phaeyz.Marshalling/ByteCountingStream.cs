namespace Phaeyz.Marshalling;

/// <summary>
/// An extremely light-weight stream that counts the number of bytes written to it.
/// The written bytes are discarded after being counted. The stream is not readable or seekable.
/// </summary>
public class ByteCountingStream : Stream
{
    /// <inheritdoc />
    public override bool CanRead => false;

    /// <inheritdoc />
    public override bool CanSeek => false;

    /// <inheritdoc />
    public override bool CanWrite => true;

    /// <summary>
    /// The total number of bytes written to the stream.
    /// </summary>
    public long BytesWritten { get; private set; }

    /// <inheritdoc />
    public override long Length => throw new NotSupportedException("The stream is not seekable.");

    /// <inheritdoc />
    public override long Position
    {
        get => throw new NotSupportedException("The stream is not seekable.");
        set => throw new NotSupportedException("The stream is not seekable.");
    }

    /// <inheritdoc />
    public override void Flush() { }

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count) =>
        throw new NotSupportedException("The stream is not readable.");

    /// <inheritdoc />
    public override long Seek(long offset, System.IO.SeekOrigin origin) =>
        throw new NotSupportedException("The stream is not seekable.");

    /// <inheritdoc />
    public override void SetLength(long value) =>
        throw new NotSupportedException("The stream is not seekable.");

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count) => BytesWritten += count;
}
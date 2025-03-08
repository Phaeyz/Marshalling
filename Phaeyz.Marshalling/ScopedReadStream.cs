namespace Phaeyz.Marshalling;

/// <summary>
/// A stream for wrapping another stream, and limiting the number of bytes which may be read.
/// </summary>
/// <remarks>
/// The stream is not seekable or writable.
/// </remarks>
public class ScopedReadStream : Stream
{
    private readonly Stream _stream;
    private readonly bool _ownStream;
    private bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="Phaeyz.Marshalling.ScopedReadStream"/> instance.
    /// </summary>
    /// <param name="stream">
    /// The stream the current instance wraps.
    /// </param>
    /// <param name="maxReadableBytes">
    /// The maximum number of bytes allowed to be read from the underlying stream.
    /// </param>
    /// <param name="ownStream">
    /// If <c>true</c> when the current instance is disposed, the underlying stream is also disposed.
    /// Defaults to <c>false</c>.
    /// </param>
    /// <exception cref="System.ArgumentException">
    /// <paramref name="stream"/> is not readable.
    /// </exception>
    public ScopedReadStream(Stream stream, int maxReadableBytes, bool ownStream = false)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentOutOfRangeException.ThrowIfNegative(maxReadableBytes);

        if (!stream.CanRead)
        {
            throw new ArgumentException("The stream is not readable.", nameof(stream));
        }

        _stream = stream;
        _ownStream = ownStream;
        MaxReadableBytes = maxReadableBytes;
    }

    /// <summary>
    /// Indicates whether the current stream supports reading. Always returns <c>true></c>.
    /// </summary>
    public override bool CanRead => true;

    /// <summary>
    /// Indicates whether the current stream supports seeking. Always returns <c>false</c>.
    /// </summary>
    public override bool CanSeek => false;

    /// <summary>
    /// Indicates whether the current stream supports writing. Always returns <c>false</c>.
    /// </summary>
    public override bool CanWrite => false;

    /// <summary>
    /// Gets the length in bytes of the stream. An exception is always thrown.
    /// </summary>
    /// <exception cref="System.NotSupportedException">
    /// A class derived from stream does not support seeking and the length is unknown.
    /// </exception>
    public override long Length => throw new NotSupportedException(
        "A class derived from Stream does not support seeking and the length is unknown.");

    /// <summary>
    /// The max number of bytes allowed to be read from the stream.
    /// </summary>
    public int MaxReadableBytes { get; private set; }

    /// <summary>
    /// Gets or sets the position within the current stream. An exception is always thrown.
    /// </summary>
    /// <exception cref="System.NotSupportedException">
    /// The stream does not support seeking.
    /// </exception>
    public override long Position
    {
        get => throw new NotSupportedException("The stream does not support seeking.");
        set => throw new NotSupportedException("The stream does not support seeking.");
    }

    /// <summary>
    /// The total number of bytes read from the stream since the instance was created.
    /// </summary>
    public int TotalBytesRead { get; private set; }

    /// <summary>
    /// Closes the stream.
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
    /// </param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && !_disposed && _ownStream)
        {
            _stream.Dispose();
        }
        _disposed = true;
        base.Dispose(disposing);
    }

    /// <summary>
    /// Clears all buffers for this stream and causes any buffered data to be written to the underlying
    /// device. This method is a no-op for <see cref="Phaeyz.Marshalling.ScopedReadStream"/>.
    /// </summary>
    public override void Flush() { }

    /// <summary>
    /// Reads a sequence of bytes from the current stream and advances the position within the stream
    /// by the number of bytes read.
    /// </summary>
    /// <param name="buffer">
    /// An array of bytes. When this method returns, the buffer contains the specified byte array with
    /// the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - <c>1</c>)
    /// replaced by the bytes read from the current source.
    /// </param>
    /// <param name="offset">
    /// The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read
    /// from the current stream.
    /// </param>
    /// <param name="count">
    /// The maximum number of bytes to be read from the current stream.
    /// </param>
    /// <returns>
    /// The total number of bytes read into the buffer. This can be less than the number of bytes requested
    /// if that many bytes are not currently available, or zero (<c>0</c>) if count is <c>0</c> or the end
    /// of the stream has been reached.
    /// </returns>
    /// <exception cref="System.ArgumentException">
    /// The sum of offset and count is larger than the buffer length.
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    /// <paramref name="buffer"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="offset"/> or <paramref name="count"/> is negative.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream has been closed.
    /// </exception>
    public override int Read(byte[] buffer, int offset, int count)
    {
        ValidateBufferArguments(buffer, offset, count);
        ObjectDisposedException.ThrowIf(_disposed, this);
        int totalBytesAvailableForReading = MaxReadableBytes - TotalBytesRead;
        if (count == 0 || totalBytesAvailableForReading == 0)
        {
            return 0;
        }
        int bytesRead = _stream.Read(buffer, offset, Math.Min(totalBytesAvailableForReading, count));
        TotalBytesRead += bytesRead;
        return bytesRead;
    }

    /// <summary>
    /// Sets the position within the current stream. An exception is always thrown.
    /// </summary>
    /// <param name="offset">
    /// A byte offset relative to the <paramref name="origin"/> parameter.
    /// </param>
    /// <param name="origin">
    /// A value of type <see cref="System.IO.SeekOrigin"/> indicating the reference point used to obtain the new position.
    /// </param>
    /// <returns>
    /// The new position within the current stream.
    /// </returns>
    /// <exception cref="System.NotSupportedException">
    /// The stream does not support seeking, such as if the stream is constructed from a pipe or console output.
    /// </exception>
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException(
        "The stream does not support seeking, such as if the stream is constructed from a pipe or console output.");

    /// <summary>
    /// Sets the length of the current stream. An exception is always thrown.
    /// </summary>
    /// <param name="value">
    /// The desired length of the current stream in bytes.
    /// </param>
    /// <exception cref="System.NotSupportedException">
    /// The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output.
    /// </exception>
    public override void SetLength(long value) => throw new NotSupportedException(
        "The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output.");

    /// <summary>
    /// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
    /// </summary>
    /// <param name="buffer">
    /// An array of bytes. This method copies <paramref name="count"/> bytes from <paramref name="buffer"/> to the current stream.
    /// </param>
    /// <param name="offset">
    /// The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current stream.
    /// </param>
    /// <param name="count">
    /// The number of bytes to be written to the current stream.
    /// </param>
    /// <exception cref="System.NotSupportedException">
    /// The stream does not support writing.
    /// </exception>
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException("The stream does not support writing.");
}
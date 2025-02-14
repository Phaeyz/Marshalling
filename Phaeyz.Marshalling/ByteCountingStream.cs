namespace Phaeyz.Marshalling;

/// <summary>
/// An extremely light-weight stream that counts the number of bytes written to it.
/// The written bytes are discarded after being counted. The stream is not readable or seekable.
/// </summary>
public class ByteCountingStream : Stream
{
    /// <summary>
    /// Gets a value indicating whether the current stream supports reading. Always returns <c>false</c>.
    /// </summary>
    public override bool CanRead => false;

    /// <summary>
    /// Gets a value indicating whether the current stream supports seeking. Always returns <c>false</c>.
    /// </summary>
    public override bool CanSeek => false;

    /// <summary>
    /// Gets a value indicating whether the current stream supports writing. Always returns <c>true</c>.
    /// </summary>
    public override bool CanWrite => true;

    /// <summary>
    /// The total number of bytes written to the stream.
    /// </summary>
    public long BytesWritten { get; private set; }

    /// <summary>
    /// Always throws an exception as the stream does not support seeking and the length is unknown.
    /// </summary>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not seekable.
    /// </exception>
    public override long Length => throw new NotSupportedException("The stream is not seekable.");

    /// <summary>
    /// Always throws an exception as the stream does not support seeking.
    /// </summary>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not seekable.
    /// </exception>
    public override long Position
    {
        get => throw new NotSupportedException("The stream is not seekable.");
        set => throw new NotSupportedException("The stream is not seekable.");
    }

    /// <summary>
    /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
    /// This method is a no-op as there is no buffering.
    /// </summary>
    public override void Flush() { }

    /// <summary>
    /// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
    /// Currently this method always throws an exception as the stream is not readable.
    /// </summary>
    /// <param name="buffer">
    /// An array of bytes. When this method returns, the buffer contains the specified byte array
    /// with the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced
    /// by the bytes read from the current source.
    /// </param>
    /// <param name="offset">
    /// The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream.
    /// </param>
    /// <param name="count">
    /// The maximum number of bytes to be read from the current stream.
    /// </param>
    /// <returns>
    /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are
    /// not currently available, or zero (0) if <paramref name="count"/> is 0 or the end of the stream has been reached.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// The stream is not readable.
    /// </exception>
    public override int Read(byte[] buffer, int offset, int count) =>
        throw new NotSupportedException("The stream is not readable.");

    /// <summary>
    /// Sets the position within the current stream. Currently this method always throws an exception as the stream is not seekable.
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
    /// <exception cref="NotSupportedException">
    /// The stream is not seekable.
    /// </exception>
    public override long Seek(long offset, SeekOrigin origin) =>
        throw new NotSupportedException("The stream is not seekable.");

    /// <summary>
    /// Sets the length of the current stream. Currently this method always throws an exception as the stream is not seekable.
    /// </summary>
    /// <param name="value">
    /// The desired length of the current stream in bytes.
    /// </param>
    /// <exception cref="NotSupportedException">
    /// The stream is not seekable.
    /// </exception>
    public override void SetLength(long value) =>
        throw new NotSupportedException("The stream is not seekable.");

    /// <summary>
    /// Increments the <see cref="Phaeyz.Marshalling.ByteCountingStream.BytesWritten"/> property by the count
    /// of bytes requested to be written.
    /// </summary>
    /// <param name="buffer">
    /// A buffer requested to be written. Ignored.
    /// </param>
    /// <param name="offset">
    /// The offset of the buffer containing the bytes to be written. Ignored.
    /// </param>
    /// <param name="count">
    /// The count of bytes to increment the <see cref="Phaeyz.Marshalling.ByteCountingStream.BytesWritten"/> property.
    /// </param>
    public override void Write(byte[] buffer, int offset, int count) => BytesWritten += count;
}
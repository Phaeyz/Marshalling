using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;

namespace Phaeyz.Marshalling;

/// <summary>
/// A stream similar to <see cref="System.IO.BufferedStream"/> but provides access to the read buffer, making the stream
/// ideal for parsing and scanning.
/// </summary>
/// <remarks>
/// This class is not thread safe. Like any class which is not thread safe, usage of this class concurrently from multiple
/// threads is not supported and will result in corrupt state.
/// </remarks>
public class MarshalStream : Stream
{
    /// <summary>
    /// The default buffer size created when wrapping a stream (16KB).
    /// </summary>
    public const int DefaultBufferSize = 1024 * 16; // 16KB

    // A byte array used to test for null terminators in the stream.
    private static readonly byte[] s_nullTerminatorBytes = new byte[8];

    // During a stream write operation, for efficiency a minimum buffer size is computed based
    // on string length and encoding size. This is the minimum number of characters is required
    // for such a buffer.
    const int c_minimumCharactersForStringWriteBuffer = 100;

    // During a stream write operation, for efficiency a minimum buffer size is computed based
    // on string length and encoding size. Each character may take multiple code units, and
    // each code unit multiple bytes. Typically the size of each code unit may be computed
    // based on the size of a null terminator. This value determines the number of code units
    // per character on the high side. There is not a single correct answer for all encodings,
    // and the string write operation is resilient to recover if this value is too small.
    // However a high number is chosen here to balance efficiency of memory vs speed. Typically,
    // most Unicode encodings use just 1 code unit, especially for English characters.
    const int c_codeUnitsToReservePerCharacterForStringWriteBuffer = 3;

    private Stream? _stream;
    private readonly bool _ownStream;
    private byte[]? _buffer;
    private bool _bufferHasUnpersistedBytes = false;
    private long _currentReadOffset = 0;
    private int _bufferedByteCount = 0;
    private ReadOnlyMemory<byte>? _fixedBuffer;
    private readonly bool _canRead;
    private readonly bool _canSeek;
    private readonly bool _canWrite;
    private readonly HashSet<IMarshalStreamProcessor> _readProcessors = [];
    private readonly HashSet<IMarshalStreamProcessor> _writeProcessors = [];

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="Phaeyz.Marshalling.MarshalStream"/> which wraps another underlying stream.
    /// </summary>
    /// <param name="stream">
    /// The underlying stream to wrap.
    /// </param>
    /// <param name="ownStream">
    /// If <c>true</c>, when the <see cref="Phaeyz.Marshalling.MarshalStream"/> is disposed, the underlying stream is also disposed.
    /// </param>
    /// <param name="bufferSize">
    /// The buffer size to use. If unspecified <see cref="Phaeyz.Marshalling.MarshalStream.DefaultBufferSize"/> is used.
    /// </param>
    public MarshalStream(Stream stream, bool ownStream = true, int bufferSize = DefaultBufferSize)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentOutOfRangeException.ThrowIfNegative(bufferSize);
        _stream = stream;
        _ownStream = ownStream;
        _buffer = new byte[bufferSize == 0 ? DefaultBufferSize : bufferSize];
        _canRead = _stream.CanRead;
        _canSeek = _stream.CanSeek;
        _canWrite = _stream.CanWrite;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Phaeyz.Marshalling.MarshalStream"/> which wraps the specified read-only buffer.
    /// </summary>
    /// <param name="buffer">
    /// The read-only buffer to wrap.
    /// </param>
    public MarshalStream(ReadOnlyMemory<byte> buffer)
    {
        _fixedBuffer = buffer;
        _bufferedByteCount = buffer.Length;
        _canRead = true;
        _canSeek = true;
        _canWrite = false;
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Provides access to the unread part of the read buffer.
    /// </summary>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    public ReadOnlyMemory<byte> BufferedReadableBytes
    {
        get
        {
            VerifyNotDisposed();
            return _fixedBuffer is not null
                ? _currentReadOffset >= _fixedBuffer.Value.Length
                    ? ReadOnlyMemory<byte>.Empty
                    : _fixedBuffer.Value[(int)_currentReadOffset.._bufferedByteCount]
                : _bufferHasUnpersistedBytes
                    ? ReadOnlyMemory<byte>.Empty
                    : new(_buffer, (int)_currentReadOffset, _bufferedByteCount - (int)_currentReadOffset);
        }
    }

    /// <summary>
    /// Gets the number of unread bytes in the read buffer.
    /// </summary>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    public int BufferedReadableByteCount
    {
        get
        {
            VerifyNotDisposed();
            return _bufferHasUnpersistedBytes || _currentReadOffset >= _bufferedByteCount
                ? 0
                : _bufferedByteCount - (int)_currentReadOffset;
        }
    }

    /// <summary>
    /// Returns <c>true</c> if the stream may be read from, <c>false</c> otherwise.
    /// </summary>
    public override bool CanRead => !IsDisposed && _canRead && _writeProcessors.Count == 0;

    /// <summary>
    /// Returns <c>true</c> if the stream may seeked, <c>false</c> otherwise.
    /// </summary>
    public override bool CanSeek => !IsDisposed && _canSeek && _readProcessors.Count == 0 && _writeProcessors.Count == 0;

    /// <summary>
    /// Returns <c>true</c> if the stream may be written to, <c>false</c> otherwise.
    /// </summary>
    public override bool CanWrite => !IsDisposed && _canWrite && _readProcessors.Count == 0;

    /// <summary>
    /// Returns <c>true</c> if the stream is disposed, <c>false</c> otherwise.
    /// </summary>
    public bool IsDisposed => _buffer is null && _fixedBuffer is null;

    /// <summary>
    /// Returns <c>true</c> if the stream wraps a fixed buffer; <c>false</c> otherwise.
    /// </summary>
    public bool IsFixedBuffer => _fixedBuffer is not null;

    /// <summary>
    /// Gets the length of the stream. The stream must wrap a fixed buffer or a seekable stream.
    /// </summary>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    public override long Length
    {
        get
        {
            VerifyNotDisposed();
            // If there are unpersisted bytes, choose the max of the position or the length
            // of the underlying stream. Don't do this without unpersisted bytes because seeking
            // past the end of the stream is supported.
            return _fixedBuffer?.Length ??
                (_bufferHasUnpersistedBytes
                ? Math.Max(_stream!.Length, Position)
                : _stream!.Length);
        }
    }

    /// <summary>
    /// Gets the read or write position of the stream. The stream must wrap a fixed buffer or a seekable stream.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Attempting to seek before the beginning of the stream.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not seekable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    public override long Position
    {
        get
        {
            VerifyNotDisposed();
            return _fixedBuffer is not null
                ? _currentReadOffset
                : _bufferHasUnpersistedBytes
                    ? _stream!.Position + _bufferedByteCount
                    : _stream!.Position - _bufferedByteCount + _currentReadOffset;
        }
        set => Seek(value, SeekOrigin.Begin);
    }

    /// <summary>
    /// Gets the size of the fixed buffer or the buffer for the underlying stream.
    /// </summary>
    public int TotalBufferSize => _fixedBuffer?.Length ?? _buffer?.Length ?? 0;

    #endregion Properties

    #region AddProcessor

    /// <summary>
    /// Registers a new read processor.
    /// </summary>
    /// <param name="processor">
    /// The read processor to register.
    /// </param>
    /// <returns>
    /// A <see cref="System.IDisposable"/> which may be used to remove the read processor.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// The read processor is already registered.
    /// </exception>
    /// <remarks>
    /// When the read processor is registered, the stream will become unseekable and read-only.
    /// Multiple read processors may be concurrently registered.
    /// </remarks>
    public IDisposable AddReadProcessor(IMarshalStreamProcessor processor)
    {
        VerifyReadableStream();
        if (!_readProcessors.Add(processor))
        {
            throw new ArgumentException("The processor is already registered.", nameof(processor));
        }
        return new ProcessorRemover(processor, _readProcessors);
    }

    /// <summary>
    /// Registers a new write processor.
    /// </summary>
    /// <param name="processor">
    /// The write processor to register.
    /// </param>
    /// <returns>
    /// A <see cref="System.IDisposable"/> which may be used to remove the write processor.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// The write processor is already registered.
    /// </exception>
    /// <remarks>
    /// When the write processor is registered, the stream will become unseekable and write-only.
    /// Multiple write processors may be concurrently registered.
    /// </remarks>
    public IDisposable AddWriteProcessor(IMarshalStreamProcessor processor)
    {
        VerifyWritableStream();
        if (!_writeProcessors.Add(processor))
        {
            throw new ArgumentException("The processor is already registered.", nameof(processor));
        }
        return new ProcessorRemover(processor, _writeProcessors);
    }

    #endregion AddProcessor

    #region AlignBytesAvailableToBufferStart

    /// <summary>
    /// Maximizes the available read buffer by moving all unread bytes to the beginning of the read buffer.
    /// </summary>
    /// <remarks>
    /// This does nothing when wrapping a fixed buffer. This will cause the already-read buffer to be flushed.
    /// </remarks>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    private void AlignBytesAvailableToBufferStart()
    {
        VerifyNotDisposed();
        if (_fixedBuffer is null && _currentReadOffset > 0)
        {
            _bufferedByteCount = BufferedReadableByteCount;
            if (_bufferedByteCount > 0)
            {
                Buffer.BlockCopy(_buffer!, (int)_currentReadOffset, _buffer!, 0, _bufferedByteCount);
            }
            _currentReadOffset = 0;
        }
    }

    #endregion AlignBytesAvailableToBufferStart

    #region CopyTo

    /// <summary>
    /// Copies the remainder of the current stream to the specified destination stream.
    /// </summary>
    /// <param name="destination">
    /// The destination stream.
    /// </param>
    /// <param name="bufferSize">
    /// The minimum buffer size to use during the copy. If the internal buffer is larger
    /// it will be used instead. This parameter is not used when wrapping a fixed buffer.
    /// </param>
    /// <exception cref="System.ArgumentNullException">
    /// <paramref name="destination"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="bufferSize"/> is negative.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The current stream is not readable, or <paramref name="destination"/> is not writable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    public override void CopyTo(Stream destination, int bufferSize)
    {
        ArgumentNullException.ThrowIfNull(destination);
        ArgumentOutOfRangeException.ThrowIfNegative(bufferSize);
        VerifyReadableStream();

        if (!destination.CanWrite)
        {
            throw new NotSupportedException("The destination stream is not writable.");
        }

        if (_fixedBuffer is not null)
        {
            if (BufferedReadableByteCount > 0)
            {
                // Try and get the fixed buffer segment so we can avoid calling the Span version of Write.
                // The default implementation of the Span version of Write reserves a buffer and copies to it.
                if (MemoryMarshal.TryGetArray(BufferedReadableBytes, out ArraySegment<byte> fixedBufferSegment))
                {
                    Process(_readProcessors, fixedBufferSegment.Array!, fixedBufferSegment.Offset, fixedBufferSegment.Count);
                    destination.Write(fixedBufferSegment.Array!, fixedBufferSegment.Offset, fixedBufferSegment.Count);
                }
                else
                {
                    ReadOnlySpan<byte> bytes = BufferedReadableBytes.Span;
                    Process(_readProcessors, bytes);
                    destination.Write(bytes);
                }
                _currentReadOffset = _bufferedByteCount;
            }
            return;
        }

        FlushWrite();

        if (BufferedReadableByteCount > 0)
        {
            int byteCount = BufferedReadableByteCount;
            Process(_readProcessors, _buffer!, (int)_currentReadOffset, byteCount);
            destination.Write(_buffer!, (int)_currentReadOffset, byteCount); // Don't use the Span version because it does an extra buffer copy.
            _currentReadOffset = 0;
            _bufferedByteCount = 0;
        }

        byte[] buffer = (bufferSize > 0) ? ArrayPool<byte>.Shared.Rent(bufferSize) : _buffer!;
        try
        {
            int bytesRead;
            while ((bytesRead = _stream!.Read(buffer, 0, buffer.Length)) != 0) // Don't use the Span version because it does an extra buffer copy.
            {
                Process(_readProcessors, buffer, 0, bytesRead);
                destination.Write(buffer, 0, bytesRead); // Don't use the Span version because it does an extra buffer copy.
            }
        }
        finally
        {
            if (bufferSize > 0)
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }

    /// <summary>
    /// Copies the remainder of the current stream to the specified destination stream.
    /// </summary>
    /// <param name="destination">
    /// The destination stream.
    /// </param>
    /// <param name="bufferSize">
    /// The minimum buffer size to use during the copy. If the internal buffer is larger
    /// it will be used instead. This parameter is not used when wrapping a fixed buffer.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task which is completed when the operation is complete.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// <paramref name="destination"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="bufferSize"/> is negative.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The current stream is not readable, or the destination stream is not writable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    /// <exception cref="System.OperationCanceledException">
    /// The cancellation token was canceled.
    /// </exception>
    public override async Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(destination);
        ArgumentOutOfRangeException.ThrowIfNegative(bufferSize);
        VerifyReadableStream();

        if (!destination.CanWrite)
        {
            throw new NotSupportedException("The destination stream is not writable.");
        }

        if (_fixedBuffer is not null)
        {
            if (BufferedReadableByteCount > 0)
            {
                ReadOnlyMemory<byte> bytes = BufferedReadableBytes;
                Process(_readProcessors, bytes.Span);
                await destination.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
                _currentReadOffset = _bufferedByteCount;
            }
            return;
        }

        await FlushWriteAsync(cancellationToken).ConfigureAwait(false);

        if (BufferedReadableByteCount > 0)
        {
            ReadOnlyMemory<byte> bytes = BufferedReadableBytes;
            Process(_readProcessors, bytes.Span);
            await destination.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
            _currentReadOffset = 0;
            _bufferedByteCount = 0;
        }

        byte[] buffer = (bufferSize > 0) ? ArrayPool<byte>.Shared.Rent(bufferSize) : _buffer!;
        try
        {
            var memoryBuffer = new Memory<byte>(buffer);
            int bytesRead;
            while ((bytesRead = await _stream!.ReadAsync(memoryBuffer, cancellationToken).ConfigureAwait(false)) != 0)
            {
                var bytes = memoryBuffer[..bytesRead];
                Process(_readProcessors, bytes.Span);
                await destination.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
            }
        }
        finally
        {
            if (bufferSize > 0)
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }

    #endregion CopyTo

    #region Dispose

    /// <summary>
    /// Releases all resources used by the <see cref="Phaeyz.Marshalling.MarshalStream"/>.
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
    /// </param>
    protected override void Dispose(bool disposing)
    {
        Stream? stream = Interlocked.Exchange(ref _stream, null);
        if (stream is not null)
        {
            FlushWrite(stream);
            if (_ownStream)
            {
                stream.Dispose();
            }
        }
        _fixedBuffer = null;
        _buffer = null;
#pragma warning disable CA1816
        GC.SuppressFinalize(this);
#pragma warning restore CA1816
    }

    /// <summary>
    /// Asynchronously releases the unmanaged resources used by the <see cref="Phaeyz.Marshalling.MarshalStream"/>.
    /// </summary>
    /// <returns>
    /// A task which is completed when the asynchronous dispose operation completes.
    /// </returns>
    public override async ValueTask DisposeAsync()
    {
        Stream? stream = Interlocked.Exchange(ref _stream, null);
        if (stream is not null)
        {
            await FlushWriteAsync(stream, default).ConfigureAwait(false);
            if (_ownStream)
            {
                await stream.DisposeAsync().ConfigureAwait(false);
            }
        }
        _fixedBuffer = null;
        _buffer = null;
#pragma warning disable CA1816
        GC.SuppressFinalize(this);
#pragma warning restore CA1816
    }

    #endregion Dispose

    #region EnsureByteCountAvailableInBuffer

    /// <summary>
    /// Ensures at minimum the specified number of unread bytes is available in the read buffer.
    /// </summary>
    /// <param name="byteCount">
    /// The minimum number of unread bytes which must be available in the read buffer.
    /// </param>
    /// <returns>
    /// Returns <c>true</c> if the specified number of unread bytes is available in the read buffer.
    /// </returns>
    /// <remarks>
    /// To make the entire read buffer available with unread bytes, use TotalBufferSize as the byte count.
    /// A value of zero always returns <c>true</c>.
    /// </remarks>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="byteCount"/> was negative or greater than the total buffer size.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not readable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    public bool EnsureByteCountAvailableInBuffer(int byteCount)
    {
        VerifyReadableStream();
        ArgumentOutOfRangeException.ThrowIfNegative(byteCount);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(byteCount, TotalBufferSize);

        if (byteCount == 0)
        {
            return true;
        }

        if (_fixedBuffer is not null)
        {
            return byteCount <= BufferedReadableByteCount;
        }

        FlushWrite();

        if (byteCount > BufferedReadableByteCount)
        {
            FlushReadIfBufferIsEmpty();

            // Shift the buffer over if there is not enough room.
            if (byteCount > _buffer!.Length - _currentReadOffset)
            {
                AlignBytesAvailableToBufferStart();
            }

            do
            {
                // Do not use the Span version of Read because default implementation does an extra buffer copy.
                int bytesRead = _stream!.Read(_buffer!, _bufferedByteCount, _buffer!.Length - _bufferedByteCount);
                if (bytesRead == 0)
                {
                    return false;
                }
                else
                {
                    _bufferedByteCount += bytesRead;
                }
            } while (byteCount > BufferedReadableByteCount);
        }

        return true;
    }

    /// <summary>
    /// Ensures at minimum the specified number of unread bytes is available in the read buffer.
    /// </summary>
    /// <param name="byteCount">
    /// The minimum number of unread bytes which must be available in the read buffer.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// Returns a task yielding <c>true</c> if the specified number of unread bytes is available in the read buffer.
    /// </returns>
    /// <remarks>
    /// To make the entire read buffer available with unread bytes, use TotalBufferSize as the byte count.
    /// A value of zero always returns <c>true</c>.
    /// </remarks>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="byteCount"/> was negative or greater than the total buffer size.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not readable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    /// <exception cref="System.OperationCanceledException">
    /// The cancellation token was canceled.
    /// </exception>
    public async ValueTask<bool> EnsureByteCountAvailableInBufferAsync(int byteCount, CancellationToken cancellationToken = default)
    {
        VerifyReadableStream();
        ArgumentOutOfRangeException.ThrowIfNegative(byteCount);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(byteCount, TotalBufferSize);

        if (byteCount == 0)
        {
            return true;
        }

        if (_fixedBuffer is not null)
        {
            return byteCount <= BufferedReadableByteCount;
        }

        await FlushWriteAsync(cancellationToken).ConfigureAwait(false);

        if (byteCount > BufferedReadableByteCount)
        {
            FlushReadIfBufferIsEmpty();

            // Shift the buffer over if there is not enough room.
            if (byteCount > _buffer!.Length - _currentReadOffset)
            {
                AlignBytesAvailableToBufferStart();
            }

            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                int bytesRead = await _stream!.ReadAsync(
                    _buffer!.AsMemory(_bufferedByteCount, _buffer!.Length - _bufferedByteCount),
                    cancellationToken).ConfigureAwait(false);
                if (bytesRead == 0)
                {
                    return false;
                }
                else
                {
                    _bufferedByteCount += bytesRead;
                }
            } while (byteCount > BufferedReadableByteCount);
        }

        return true;
    }

    #endregion EnsureByteCountAvailableInBuffer

    #region Flush

    /// <summary>
    /// Ensures there is no unread bytes in the read buffer by updating the underlying stream's
    /// seek pointer, and ensuring all unwritten bytes are persisted to the underlying stream.
    /// </summary>
    /// <remarks>
    /// This method has no effect when wrapping a fixed buffer.
    /// </remarks>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not seekable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    public override void Flush()
    {
        FlushRead();
        FlushWrite();
    }

    /// <summary>
    /// Ensures there is no unread bytes in the read buffer by updating the underlying stream's
    /// seek pointer, and ensures all unwritten bytes are persisted to the underlying stream.
    /// </summary>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task which is completed when the operation is complete.
    /// </returns>
    /// <remarks>
    /// This method has no effect when wrapping a fixed buffer.
    /// </remarks>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not seekable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    /// <exception cref="System.OperationCanceledException">
    /// The cancellation token was canceled.
    /// </exception>
    public override async Task FlushAsync(CancellationToken cancellationToken)
    {
        FlushRead();
        cancellationToken.ThrowIfCancellationRequested();
        await FlushWriteAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Ensures there is no unread bytes in the read buffer by updating the underlying stream's
    /// seek pointer.
    /// </summary>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not seekable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    private void FlushRead()
    {
        FlushReadIfBufferIsEmpty();
        if (_stream is not null && BufferedReadableByteCount > 0)
        {
            VerifySeekableStream();
            _stream!.Seek(-BufferedReadableByteCount, SeekOrigin.Current);
            _currentReadOffset = 0;
            _bufferedByteCount = 0;
        }
    }

    /// <summary>
    /// Before a write operation, ensures there is no unread bytes in the read buffer by updating the underlying stream's
    /// seek pointer.
    /// </summary>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The current stream is not writable, or the read buffer cannot be flushed because the current stream is not seekable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    private void FlushReadBeforeWrite()
    {
        VerifyWritableStream();
        if (!_bufferHasUnpersistedBytes)
        {
            // Provide a more friendly exception in this scenario.
            if (BufferedReadableByteCount > 0 && !CanSeek)
            {
                throw new NotSupportedException("Cannot write to the underlying stream if the read buffer cannot be flushed.");
            }
            FlushRead();
        }
    }

    /// <summary>
    /// If there is no cached data, the read buffer variables are reset.
    /// </summary>
    private void FlushReadIfBufferIsEmpty()
    {
        if (!_bufferHasUnpersistedBytes && _fixedBuffer is null && _currentReadOffset > 0 && _currentReadOffset == _bufferedByteCount)
        {
            _currentReadOffset = 0;
            _bufferedByteCount = 0;
        }
    }

    /// <summary>
    /// Ensures all unwritten bytes are persisted to the underlying stream.
    /// </summary>
    /// <param name="underlyingStream">
    /// The underlying stream to flush. If unspecified, the current underlying stream is used.
    /// </param>
    private void FlushWrite(Stream? underlyingStream = null)
    {
        if (_bufferHasUnpersistedBytes)
        {
            if (_bufferedByteCount > 0)
            {
                (underlyingStream ?? _stream!).Write(_buffer!.AsSpan(0, _bufferedByteCount));
                _bufferedByteCount = 0;
            }
            _bufferHasUnpersistedBytes = false;
        }
    }

    /// <summary>
    /// Ensures all unwritten bytes are persisted to the underlying stream.
    /// </summary>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task which is completed when the operation is complete.
    /// </returns>
    /// <exception cref="System.OperationCanceledException">
    /// The cancellation token was canceled.
    /// </exception>
    private ValueTask FlushWriteAsync(CancellationToken cancellationToken) => FlushWriteAsync(null, cancellationToken);

    /// <summary>
    /// Ensures all unwritten bytes are persisted to the underlying stream.
    /// </summary>
    /// <param name="underlyingStream">
    /// The underlying stream to flush. If unspecified, the current underlying stream is used.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task which is completed when the operation is complete.
    /// </returns>
    /// <exception cref="System.OperationCanceledException">
    /// The cancellation token was canceled.
    /// </exception>
    private async ValueTask FlushWriteAsync(Stream? underlyingStream, CancellationToken cancellationToken)
    {
        if (_bufferHasUnpersistedBytes)
        {
            if (_bufferedByteCount > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await (underlyingStream ?? _stream!)!.WriteAsync(_buffer!.AsMemory(0, _bufferedByteCount), cancellationToken).ConfigureAwait(false);
                _bufferedByteCount = 0;
            }
            _bufferHasUnpersistedBytes = false;
        }
    }

    #endregion Flush

    #region IsMatch

    /// <summary>
    /// Check to see if the next bytes to be read from the stream match the provided sequence of bytes.
    /// </summary>
    /// <param name="match">
    /// The sequence of bytes to compare against the next bytes to be read from the stream.
    /// </param>
    /// <param name="bytesRead">
    /// Receives the number of bytes read from the stream when attempting to do a compare. This may be less than the
    /// full length of <paramref name="match"/> if the buffer prevents having to increment the read pointer.
    /// If <c>true</c> is returned, <paramref name="bytesRead"/> will be the number of bytes in <paramref name="match"/>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the provided sequence of bytes matches the next bytes to be read from the stream.
    /// </returns>
    public bool IsMatch(ReadOnlySpan<byte> match, out int bytesRead)
    {
        VerifyReadableStream();

        if (match.Length == 0)
        {
            bytesRead = 0;
            return true;
        }

        if (_fixedBuffer is not null)
        {
            var fixedBuffer = _fixedBuffer.Value.Span;
            if (fixedBuffer.Length - _currentReadOffset < match.Length)
            {
                bytesRead = 0;
                return false;
            }

            for (int i = 0; i < match.Length; i++)
            {
                if (fixedBuffer[(int)_currentReadOffset + i] != match[i])
                {
                    bytesRead = 0;
                    return false;
                }
            }

            _currentReadOffset += match.Length;
            bytesRead = match.Length;
            return true;
        }

        FlushWrite();

        int totalBytesProcessed = 0;
        bytesRead = 0;
        while (true)
        {
            for (long currentReadOffset = _currentReadOffset;
                currentReadOffset < _bufferedByteCount && totalBytesProcessed < match.Length;
                totalBytesProcessed++, currentReadOffset++)
            {
                if (_buffer![currentReadOffset] != match[totalBytesProcessed])
                {
                    return false;
                }
            }

            _currentReadOffset += (totalBytesProcessed - bytesRead);
            bytesRead = totalBytesProcessed;

            if (totalBytesProcessed == match.Length)
            {
                return true;
            }

            if (!EnsureByteCountAvailableInBuffer(1))
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Check to see if the next bytes to be read from the stream match the provided sequence of bytes.
    /// </summary>
    /// <param name="match">
    /// The sequence of bytes to compare against the next bytes to be read from the stream.
    /// </param>
    /// <returns>
    /// A tuple where the first item is a boolean where it's value is <c>true</c> if the provided sequence of bytes matches
    /// the next bytes to be read from the stream. The second item of the tuple receives the number of bytes read from the
    /// stream when attempting to do a compare. This may be less than the full length of <paramref name="match"/> if the buffer
    /// prevents having to increment the read pointer. If the first item of the tuple is <c>true</c>, the second item will be
    /// the number of bytes in <paramref name="match"/>.
    /// </returns>
    public async Task<(bool matched, int bytesRead)> IsMatchAsync(ReadOnlyMemory<byte> match)
    {
        VerifyReadableStream();

        if (match.Length == 0)
        {
            return (true, 0);
        }

        if (_fixedBuffer is not null)
        {
            var fixedBuffer = _fixedBuffer.Value.Span;
            if (fixedBuffer.Length - _currentReadOffset < match.Length)
            {
                return (false, 0);
            }

            ReadOnlySpan<byte> matchSpan = match.Span;
            for (int i = 0; i < match.Length; i++)
            {
                if (fixedBuffer[(int)_currentReadOffset + i] != matchSpan[i])
                {
                    return (false, 0);
                }
            }

            _currentReadOffset += match.Length;
            return (true, match.Length);
        }

        FlushWrite();

        int totalBytesProcessed = 0;
        int bytesRead = 0;
        while (true)
        {
            ReadOnlySpan<byte> matchSpan = match.Span;
            for (long currentReadOffset = _currentReadOffset;
                currentReadOffset < _bufferedByteCount && totalBytesProcessed < match.Length;
                totalBytesProcessed++, currentReadOffset++)
            {
                if (_buffer![currentReadOffset] != matchSpan[totalBytesProcessed])
                {
                    return (false, bytesRead);
                }
            }

            _currentReadOffset += (totalBytesProcessed - bytesRead);
            bytesRead = totalBytesProcessed;

            if (totalBytesProcessed == match.Length)
            {
                return (true, bytesRead);
            }

            if (!await EnsureByteCountAvailableInBufferAsync(1).ConfigureAwait(false))
            {
                return (false, bytesRead);
            }
        }
    }

    #endregion IsMatch

    #region Process

    /// <summary>
    /// Passes the buffer data to a collection of processors.
    /// </summary>
    /// <param name="processors">
    /// The processors to process the buffer data.
    /// </param>
    /// <param name="buffer">
    /// The buffer to process.
    /// </param>
    /// <param name="offset">
    /// The offset within the buffer to begin processing.
    /// </param>
    /// <param name="count">
    /// The number of bytes after the offset to begin processing.
    /// </param>
    private static void Process(HashSet<IMarshalStreamProcessor> processors, byte[] buffer, int offset, int count)
    {
        if (count > 0 && processors.Count > 0)
        {
            Process(processors, buffer.AsSpan(offset, count));
        }
    }

    /// <summary>
    /// Passes the buffer data to a collection of processors.
    /// </summary>
    /// <param name="processors">
    /// The processors to process the buffer data.
    /// </param>
    /// <param name="bytes">
    /// The buffer to process.
    /// </param>
    private static void Process(HashSet<IMarshalStreamProcessor> processors, ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length > 0 && processors.Count > 0)
        {
            foreach (IMarshalStreamProcessor processor in processors)
            {
                processor.Process(bytes);
            }
        }
    }

    #endregion Process

    #region Read

    /// <summary>
    /// Reads a sequence of bytes from the current stream and advances the position within the stream by the
    /// number of bytes read.
    /// </summary>
    /// <param name="buffer">
    /// The buffer which receives the bytes to read.
    /// </param>
    /// <param name="offset">
    /// The starting offset within <paramref name="buffer"/> which may receive to bytes to read.
    /// </param>
    /// <param name="count">
    /// The number of bytes after <paramref name="offset"/> within <paramref name="buffer"/> may receive the bytes to read.
    /// </param>
    /// <returns>
    /// The number of bytes read into within <paramref name="buffer"/>. This might be less than the number of bytes requested.
    /// If the return value is zero, the end of the stream has been reached.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// <paramref name="buffer"/> was null.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="offset"/> was outside the bounds of <paramref name="buffer"/>, or
    /// <paramref name="count"/> was negative, or the range specified by the combination of
    /// <paramref name="offset"/> and <paramref name="count"/> exceed the length of <paramref name="buffer"/>.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not readable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    public override int Read(byte[] buffer, int offset, int count)
    {
        ValidateBufferArguments(buffer, offset, count);
        return Read(buffer.AsSpan(offset, count));
    }

    /// <summary>
    /// Reads a sequence of bytes from the current stream and advances the position within the stream by the
    /// number of bytes read.
    /// </summary>
    /// <param name="buffer">
    /// The buffer which receives the bytes to read.
    /// </param>
    /// <returns>
    /// The number of bytes read into the buffer. This might be less than the number of bytes requested.
    /// If the return value is zero, the end of the stream has been reached.
    /// </returns>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not readable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    public override int Read(Span<byte> buffer)
    {
        VerifyReadableStream();
        FlushWrite();

        if (buffer.Length == 0)
        {
            return 0;
        }

        // Ensure at least 1 byte is available for reading -- if not it will be filled.
        if (!EnsureByteCountAvailableInBuffer(1))
        {
            return 0;
        }

        int bytesToRead = Math.Min(buffer.Length, BufferedReadableByteCount);
        ReadOnlySpan<byte> bytes = BufferedReadableBytes[..bytesToRead].Span;
        Process(_readProcessors, bytes);
        bytes.CopyTo(buffer);
        _currentReadOffset += bytesToRead;
        return bytesToRead;
    }

    /// <summary>
    /// Reads a sequence of bytes from the current stream and advances the position within the stream by the
    /// number of bytes read.
    /// </summary>
    /// <param name="buffer">
    /// The buffer which receives the bytes to read.
    /// </param>
    /// <param name="offset">
    /// The starting offset within <paramref name="buffer"/> which may receive to bytes to read.
    /// </param>
    /// <param name="count">
    /// The number of bytes after <paramref name="offset"/> within <paramref name="buffer"/> may receive the bytes to read.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task which yields the number of bytes read into <paramref name="buffer"/>. This might be less than the number of bytes requested.
    /// If the return value is zero, the end of the stream has been reached.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// <paramref name="buffer"/> was null.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="offset"/> was outside the bounds of <paramref name="buffer"/>, or
    /// <paramref name="count"/> was negative, or the range specified by the combination of
    /// <paramref name="offset"/> and <paramref name="count"/> exceed the length of <paramref name="buffer"/>.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not readable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    /// <exception cref="System.OperationCanceledException">
    /// The cancellation token was canceled.
    /// </exception>
    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        ValidateBufferArguments(buffer, offset, count);
        return await ReadAsync(buffer.AsMemory(offset, count), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Reads a sequence of bytes from the current stream and advances the position within the stream by the
    /// number of bytes read.
    /// </summary>
    /// <param name="buffer">
    /// The buffer which receives the bytes to read.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task which yields the number of bytes read into the buffer. This might be less than the number of bytes requested.
    /// If the return value is zero, the end of the stream has been reached.
    /// </returns>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not readable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    /// <exception cref="System.OperationCanceledException">
    /// The cancellation token was canceled.
    /// </exception>
    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        VerifyReadableStream();
        await FlushWriteAsync(cancellationToken).ConfigureAwait(false);

        if (buffer.Length == 0)
        {
            return 0;
        }

        // Ensure at least 1 byte is available for reading -- if not it will be filled.
        if (!await EnsureByteCountAvailableInBufferAsync(1, cancellationToken).ConfigureAwait(false))
        {
            return 0;
        }

        int bytesToRead = Math.Min(buffer.Length, BufferedReadableByteCount);
        ReadOnlySpan<byte> bytes = BufferedReadableBytes[..bytesToRead].Span;
        Process(_readProcessors, bytes);
        bytes.CopyTo(buffer.Span);
        _currentReadOffset += bytesToRead;
        return bytesToRead;
    }

    #endregion Read

    #region ReadByte

    /// <summary>
    /// Reads a byte from the current stream and advances the position within the stream by one byte.
    /// </summary>
    /// <returns>
    /// Returns the byte which has been read in integer form, or <c>-1</c> if the end of the stream has been reached.
    /// </returns>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not readable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    public override int ReadByte()
    {
        VerifyReadableStream();
        FlushWrite();

        // Ensure at least 1 byte is available for reading -- if not it will be filled.
        if (!EnsureByteCountAvailableInBuffer(1))
        {
            return -1;
        }

        ReadOnlySpan<byte> bytes = BufferedReadableBytes.Span;
        if (_readProcessors.Count > 0)
        {
            Process(_readProcessors, bytes[..1]);
        }
        int result = bytes[0];
        _currentReadOffset++;
        return result;
    }

    /// <summary>
    /// Reads a byte from the current stream and advances the position within the stream by one byte.
    /// </summary>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// Returns a task which yields the byte which has been read in integer form, or <c>-1</c> if the end of the stream has been reached.
    /// </returns>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not readable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    /// <exception cref="System.OperationCanceledException">
    /// The cancellation token was canceled.
    /// </exception>
    public async ValueTask<int> ReadByteAsync(CancellationToken cancellationToken = default)
    {
        VerifyReadableStream();
        await FlushWriteAsync(cancellationToken).ConfigureAwait(false);

        // Ensure at least 1 byte is available for reading -- if not it will be filled.
        if (!await EnsureByteCountAvailableInBufferAsync(1, cancellationToken).ConfigureAwait(false))
        {
            return -1;
        }

        ReadOnlySpan<byte> bytes = BufferedReadableBytes.Span;
        if (_readProcessors.Count > 0)
        {
            Process(_readProcessors, bytes[..1]);
        }
        int result = bytes[0];
        _currentReadOffset++;
        return result;
    }

    #endregion ReadByte

    #region ReadString

    /// <summary>
    /// Reads a string from the stream.
    /// </summary>
    /// <param name="encoding">
    /// The <see cref="System.Text.Encoding"/> object to use for decoding the bytes of the stream into characters.
    /// </param>
    /// <param name="maxBytesToRead">
    /// The maximum number of bytes to read from the stream. If -1, then read until the end of the stream.
    /// </param>
    /// <param name="nullTerminatorBehavior">
    /// Defines the behavior when null terminators are encountered.
    /// </param>
    /// <returns>
    /// A task yielding a <see cref="Phaeyz.Marshalling.MarshalStreamReadStringResult"/> object.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// The maximum number of bytes to read is less than zero.
    /// </exception>
    /// <exception cref="System.Text.DecoderFallbackException">
    /// The <paramref name="encoding"/>'s <see cref="System.Text.Encoding.DecoderFallback"/> property is set
    /// to <see cref="System.Text.DecoderExceptionFallback"/> and a fallback occurred. When this happens, the
    /// stream's new position is undefined.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not readable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    public MarshalStreamReadStringResult ReadString(
        Encoding encoding,
        int maxBytesToRead,
        MarshalStreamNullTerminatorBehavior nullTerminatorBehavior) =>
            ReadString(encoding.GetDecoder(), maxBytesToRead, nullTerminatorBehavior);

    /// <summary>
    /// Reads a string from the stream.
    /// </summary>
    /// <param name="decoder">
    /// The <see cref="System.Text.Decoder"/> object to use for decoding the bytes of the stream into characters.
    /// </param>
    /// <param name="maxBytesToRead">
    /// The maximum number of bytes to read from the stream. If -1, then read until the end of the stream.
    /// </param>
    /// <param name="nullTerminatorBehavior">
    /// Defines the behavior when null terminators are encountered.
    /// </param>
    /// <returns>
    /// A task yielding a <see cref="Phaeyz.Marshalling.MarshalStreamReadStringResult"/> object.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// The maximum number of bytes to read is less than zero.
    /// </exception>
    /// <exception cref="System.Text.DecoderFallbackException">
    /// The <paramref name="decoder"/>'s <see cref="System.Text.Decoder.Fallback"/> property (or the owning
    /// <see cref="System.Text.Encoding"/>'s <see cref="System.Text.Encoding.DecoderFallback"/> property) is set
    /// to <see cref="System.Text.DecoderExceptionFallback"/> and a fallback occurred. When this happens, the
    /// stream's new position is undefined.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not readable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    public MarshalStreamReadStringResult ReadString(
        Decoder decoder,
        int maxBytesToRead,
        MarshalStreamNullTerminatorBehavior nullTerminatorBehavior)
    {
        using StringReadOperation stringReadOperation = new(this, decoder, maxBytesToRead, nullTerminatorBehavior);

        while (stringReadOperation.ContinueProcessing)
        {
            if (!EnsureByteCountAvailableInBuffer(1))
            {
                stringReadOperation.EndOfStreamReached = true;
                break;
            }

            stringReadOperation.ProcessBuffer();
        };

        return stringReadOperation.GetResult();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringReadOperation"/> class.
    /// </summary>
    /// <param name="encoding">
    /// The <see cref="System.Text.Encoding"/> object to use for decoding the bytes of the stream into characters.
    /// </param>
    /// <param name="maxBytesToRead">
    /// The maximum number of bytes to read from the stream. If -1, then read until the end of the stream.
    /// </param>
    /// <param name="nullTerminatorBehavior">
    /// Defines the behavior when null terminators are encountered.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task yielding a <see cref="Phaeyz.Marshalling.MarshalStreamReadStringResult"/> object.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// The maximum number of bytes to read is less than zero.
    /// </exception>
    /// <exception cref="System.Text.DecoderFallbackException">
    /// The <paramref name="encoding"/>'s <see cref="System.Text.Encoding.DecoderFallback"/> property is set
    /// to <see cref="System.Text.DecoderExceptionFallback"/> and a fallback occurred. When this happens, the
    /// stream's new position is undefined.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not readable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    /// <exception cref="System.OperationCanceledException">
    /// The cancellation token was canceled.
    /// </exception>
    public ValueTask<MarshalStreamReadStringResult> ReadStringAsync(
        Encoding encoding,
        int maxBytesToRead,
        MarshalStreamNullTerminatorBehavior nullTerminatorBehavior,
        CancellationToken cancellationToken = default) =>
            ReadStringAsync(encoding.GetDecoder(), maxBytesToRead, nullTerminatorBehavior, cancellationToken);

    /// <summary>
    /// Initializes a new instance of the <see cref="Phaeyz.Marshalling.MarshalStreamReadStringResult"/> class.
    /// </summary>
    /// <param name="decoder">
    /// The <see cref="System.Text.Decoder"/> object to use for decoding the bytes of the stream into characters.
    /// </param>
    /// <param name="maxBytesToRead">
    /// The maximum number of bytes to read from the stream. If -1, then read until the end of the stream.
    /// </param>
    /// <param name="nullTerminatorBehavior">
    /// Defines the behavior when null terminators are encountered.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task yielding a <see cref="Phaeyz.Marshalling.MarshalStreamReadStringResult"/> object.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// The maximum number of bytes to read is less than zero.
    /// </exception>
    /// <exception cref="System.Text.DecoderFallbackException">
    /// The <paramref name="decoder"/>'s <see cref="System.Text.Decoder.Fallback"/> property (or the owning
    /// <see cref="System.Text.Encoding"/>'s <see cref="System.Text.Encoding.DecoderFallback"/> property) is set
    /// to <see cref="System.Text.DecoderExceptionFallback"/> and a fallback occurred. When this happens, the
    /// stream's new position is undefined.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not readable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    /// <exception cref="System.OperationCanceledException">
    /// The cancellation token was canceled.
    /// </exception>
    public async ValueTask<MarshalStreamReadStringResult> ReadStringAsync(
        Decoder decoder,
        int maxBytesToRead,
        MarshalStreamNullTerminatorBehavior nullTerminatorBehavior,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using StringReadOperation stringReadOperation = new(this, decoder, maxBytesToRead, nullTerminatorBehavior);

        while (stringReadOperation.ContinueProcessing)
        {
            if (!await EnsureByteCountAvailableInBufferAsync(1, cancellationToken).ConfigureAwait(false))
            {
                stringReadOperation.EndOfStreamReached = true;
                break;
            }

            stringReadOperation.ProcessBuffer();
        };

        return stringReadOperation.GetResult();
    }

    #endregion ReadString

    #region RemoveProcessor

    /// <summary>
    /// Unregisters a read processor.
    /// </summary>
    /// <param name="processor">
    /// The read processor to unregister.
    /// </param>
    /// <returns>
    /// <c>true</c> if the read processor was successfully unregistered; <c>false</c> if the read processor was not registered.
    /// </returns>
    public bool RemoveReadProcessor(IMarshalStreamProcessor processor) => _readProcessors.Remove(processor);

    /// <summary>
    /// Unregisters a write processor.
    /// </summary>
    /// <param name="processor">
    /// The write processor to unregister.
    /// </param>
    /// <returns>
    /// <c>true</c> if the write processor was successfully unregistered; <c>false</c> if the write processor was not registered.
    /// </returns>
    public bool RemoveWriteProcessor(IMarshalStreamProcessor processor) => _writeProcessors.Remove(processor);

    #endregion RemoveProcessor

    #region Scan

    /// <summary>
    /// Efficiently reads each byte of a stream until a scanning function instructs to stop reading.
    /// </summary>
    /// <param name="minBytesNeededForScan">
    /// The minimum number of bytes to pass to the scanning function. See remarks regarding the relationship between
    /// <paramref name="minBytesNeededForScan"/> and <paramref name="maxBytesToRead"/>.
    /// </param>
    /// <param name="maxBytesToRead">
    /// The maximum number of bytes to read. This may be <c>-1</c> to allow reading the entire stream.
    /// See remarks regarding the relationship between <paramref name="minBytesNeededForScan"/> and <paramref name="maxBytesToRead"/>.
    /// </param>
    /// <param name="scanFunc">
    /// A function which is called for each read byte. The function must look at the bytes at the current position and determine if
    /// scanning should be stopped. If scanning should be stopped, the function must return 0. Otherwise the function must return the
    /// number of bytes processed, which also indicates the number of bytes to skip before the next call of the scan function.
    /// </param>
    /// <returns>
    /// A <see cref="Phaeyz.Marshalling.MarshalStreamScanResult"/> object.
    /// </returns>
    /// <remarks>
    /// If <paramref name="minBytesNeededForScan"/> is more than <c>1</c>, <paramref name="maxBytesToRead"/> can never be read
    /// because reading the minimum of bytes needed for scan may cause us to read beyond the maximum bytes to read.
    /// <br/>
    /// For performance reasons, the scan function should scan the entire memory buffer provided to it until a match is found, instead of
    /// scanning only the top of the buffer before returning. While both strategies work, scanning the entire memory buffer is far more efficient.
    /// </remarks>
    /// <exception cref="System.ArgumentNullException">
    /// The <paramref name="scanFunc"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="minBytesNeededForScan"/> is zero or less, <paramref name="minBytesNeededForScan"/> is greater than the total buffer size,
    /// or <paramref name="maxBytesToRead"/> is less than <c>-1</c>.
    /// </exception>
    /// <exception cref="System.InvalidOperationException">
    /// The scan function returned a negative value, or the scan function returned more bytes than it was provided.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not readable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    public MarshalStreamScanResult Scan(
        int minBytesNeededForScan,
        long maxBytesToRead,
        Func<ReadOnlyMemory<byte>, int> scanFunc) => Scan(
            false,
            [],
            null,
            minBytesNeededForScan,
            maxBytesToRead,
            scanFunc);

    /// <summary>
    /// Efficiently reads each byte of a stream until a scanning function instructs to stop reading.
    /// </summary>
    /// <param name="destinationBuffer">
    /// Each read byte which did not match the scan function is copied to this buffer. The maximum bytes to read is defined
    /// by the length of this buffer.
    /// </param>
    /// <param name="minBytesNeededForScan">
    /// The minimum number of bytes to pass to the scanning function.
    /// </param>
    /// <param name="scanFunc">
    /// A function which is called for each read byte. The function must look at the bytes at the current position and determine if
    /// scanning should be stopped. If scanning should be stopped, the function must return 0. Otherwise the function must return the
    /// number of bytes processed, which also indicates the number of bytes to skip before the next call of the scan function.
    /// </param>
    /// <returns>
    /// A <see cref="Phaeyz.Marshalling.MarshalStreamScanResult"/> object.
    /// </returns>
    /// <remarks>
    /// For performance reasons, the scan function should scan the entire memory buffer provided to it until a match is found, instead of
    /// scanning only the top of the buffer before returning. While both strategies work, scanning the entire memory buffer is far more efficient.
    /// </remarks>
    /// <exception cref="System.ArgumentNullException">
    /// The <paramref name="scanFunc"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="minBytesNeededForScan"/> is zero or less, or <paramref name="minBytesNeededForScan"/> is greater than the total buffer size.
    /// </exception>
    /// <exception cref="System.InvalidOperationException">
    /// The scan function returned a negative value, or the scan function returned more bytes than it was provided.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not readable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    public MarshalStreamScanResult Scan(
        Span<byte> destinationBuffer,
        int minBytesNeededForScan,
        Func<ReadOnlyMemory<byte>, int> scanFunc) => Scan(
            true,
            destinationBuffer,
            null,
            minBytesNeededForScan,
            -1,
            scanFunc);

    /// <summary>
    /// Efficiently reads each byte of a stream until a scanning function instructs to stop reading.
    /// </summary>
    /// <param name="destinationStream">
    /// Optionally, each read byte which did not match the scan function is written to this stream.
    /// Specify <c>null</c> to not store read bytes.
    /// </param>
    /// <param name="minBytesNeededForScan">
    /// The minimum number of bytes to pass to the scanning function. See remarks regarding the relationship between
    /// <paramref name="minBytesNeededForScan"/> and <paramref name="maxBytesToRead"/>.
    /// </param>
    /// <param name="maxBytesToRead">
    /// The maximum number of bytes to read. This may be <c>-1</c> to allow reading the entire stream.
    /// See remarks regarding the relationship between <paramref name="minBytesNeededForScan"/> and <paramref name="maxBytesToRead"/>.
    /// </param>
    /// <param name="scanFunc">
    /// A function which is called for each read byte. The function must look at the bytes at the current position and determine if
    /// scanning should be stopped. If scanning should be stopped, the function must return 0. Otherwise the function must return the
    /// number of bytes processed, which also indicates the number of bytes to skip before the next call of the scan function.
    /// </param>
    /// <returns>
    /// A <see cref="Phaeyz.Marshalling.MarshalStreamScanResult"/> object.
    /// </returns>
    /// <remarks>
    /// If <paramref name="minBytesNeededForScan"/> is more than <c>1</c>, <paramref name="maxBytesToRead"/> can never be read
    /// because reading the minimum of bytes needed for scan may cause us to read beyond the maximum bytes to read.
    /// <br/>
    /// For performance reasons, the scan function should scan the entire memory buffer provided to it until a match is found, instead of
    /// scanning only the top of the buffer before returning. While both strategies work, scanning the entire memory buffer is far more efficient.
    /// </remarks>
    /// <exception cref="System.ArgumentException">
    /// The <paramref name="destinationStream"/> is not writable.
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    /// The <paramref name="scanFunc"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="minBytesNeededForScan"/> is zero or less, <paramref name="minBytesNeededForScan"/> is greater than the total buffer size,
    /// or <paramref name="maxBytesToRead"/> is less than <c>-1</c>.
    /// </exception>
    /// <exception cref="System.InvalidOperationException">
    /// The scan function returned a negative value, or the scan function returned more bytes than it was provided.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not readable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    public MarshalStreamScanResult Scan(
        Stream destinationStream,
        int minBytesNeededForScan,
        long maxBytesToRead,
        Func<ReadOnlyMemory<byte>, int> scanFunc) => Scan(
            false,
            [],
            destinationStream,
            minBytesNeededForScan,
            maxBytesToRead,
            scanFunc);

    /// <summary>
    /// Efficiently reads each byte of a stream until a scanning function instructs to stop reading.
    /// </summary>
    /// <param name="useDestinationBuffer">
    /// <c>true</c> if the <paramref name="destinationBuffer"/> is provided, otherwise <c>false</c>.
    /// </param>
    /// <param name="destinationBuffer">
    /// Optionally, each read byte which did not match the scan function is copied to this buffer.
    /// Specify <c>null</c> to not store read bytes.
    /// </param>
    /// <param name="destinationStream">
    /// Optionally, each read byte which did not match the scan function is written to this stream.
    /// Specify <c>null</c> to not store read bytes.
    /// </param>
    /// <param name="minBytesNeededForScan">
    /// The minimum number of bytes to pass to the scanning function. See remarks regarding the relationship between
    /// <paramref name="minBytesNeededForScan"/> and <paramref name="maxBytesToRead"/>.
    /// </param>
    /// <param name="maxBytesToRead">
    /// The maximum number of bytes to read. This may be <c>-1</c> to allow reading the entire stream.
    /// See remarks regarding the relationship between <paramref name="minBytesNeededForScan"/> and <paramref name="maxBytesToRead"/>.
    /// If <paramref name="destinationBuffer"/> is provided, <paramref name="maxBytesToRead"/> is reduced to the length of that buffer.
    /// </param>
    /// <param name="scanFunc">
    /// A function which is called for each read byte. The function must look at the bytes at the current position and determine if
    /// scanning should be stopped. If scanning should be stopped, the function must return 0. Otherwise the function must return the
    /// number of bytes processed, which also indicates the number of bytes to skip before the next call of the scan function.
    /// </param>
    /// <returns>
    /// A <see cref="Phaeyz.Marshalling.MarshalStreamScanResult"/> object.
    /// </returns>
    /// <remarks>
    /// If <paramref name="minBytesNeededForScan"/> is more than <c>1</c>, <paramref name="maxBytesToRead"/> can never be read
    /// because reading the minimum of bytes needed for scan may cause us to read beyond the maximum bytes to read.
    /// <br/>
    /// For performance reasons, the scan function should scan the entire memory buffer provided to it until a match is found, instead of
    /// scanning only the top of the buffer before returning. While both strategies work, scanning the entire memory buffer is far more efficient.
    /// </remarks>
    /// <exception cref="System.ArgumentException">
    /// The <paramref name="destinationStream"/> is not writable.
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    /// The <paramref name="scanFunc"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="minBytesNeededForScan"/> is zero or less, <paramref name="minBytesNeededForScan"/> is greater than the total buffer size,
    /// or <paramref name="maxBytesToRead"/> is less than <c>-1</c>.
    /// </exception>
    /// <exception cref="System.InvalidOperationException">
    /// The scan function returned a negative value, or the scan function returned more bytes than it was provided.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not readable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    private MarshalStreamScanResult Scan(
        bool useDestinationBuffer,
        Span<byte> destinationBuffer,
        Stream? destinationStream,
        int minBytesNeededForScan,
        long maxBytesToRead,
        Func<ReadOnlyMemory<byte>, int> scanFunc)
    {
        ArgumentNullException.ThrowIfNull(scanFunc);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(minBytesNeededForScan);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(minBytesNeededForScan, TotalBufferSize);
        ArgumentOutOfRangeException.ThrowIfLessThan(maxBytesToRead, -1);

        if (destinationStream is not null && !destinationStream.CanWrite)
        {
            throw new ArgumentException("The destination stream is not writable.", nameof(destinationStream));
        }

        VerifyReadableStream();

        maxBytesToRead = Math.Min(
            useDestinationBuffer ? destinationBuffer.Length : long.MaxValue,
            maxBytesToRead == -1 ? long.MaxValue : maxBytesToRead);

        long totalBytesRead = 0;
        while (true)
        {
            if (!EnsureByteCountAvailableInBuffer(minBytesNeededForScan))
            {
                return new MarshalStreamScanResult(totalBytesRead, false, true);
            }

            int scanOffset = 0;
            bool scanHasPositiveMatch = false;
            while (Math.Min(maxBytesToRead, BufferedReadableByteCount - scanOffset) >= minBytesNeededForScan)
            {
                ReadOnlyMemory<byte> scanBytes = BufferedReadableBytes[scanOffset..];
                int scannedBytes = scanFunc(scanBytes);
                if (scannedBytes == 0)
                {
                    scanHasPositiveMatch = true;
                    break;
                }
                if (scannedBytes < 0)
                {
                    throw new InvalidOperationException("The scan function returned a negative value.");
                }
                if (scannedBytes > scanBytes.Length)
                {
                    throw new InvalidOperationException("The scan function indicated it scanned more bytes than provided.");
                }
                scanOffset += scannedBytes;
                maxBytesToRead -= scannedBytes;
                totalBytesRead += scannedBytes;
            }

            if (scanOffset > 0)
            {
                if (_readProcessors.Count > 0)
                {
                    Process(_readProcessors, BufferedReadableBytes[..scanOffset].Span);
                }
                if (useDestinationBuffer)
                {
                    // Using totalBytesRead would reduce this to one line, but totalBytesRead is a long and Slice() requires an int.
                    // So getting around that by recreating the destination buffer so that we copy to the first element.
                    BufferedReadableBytes[..scanOffset].Span.CopyTo(destinationBuffer);
                    destinationBuffer = destinationBuffer[scanOffset..];
                }
                destinationStream?.Write(BufferedReadableBytes[..scanOffset].Span);
            }

            _currentReadOffset += scanOffset;

            if (scanHasPositiveMatch || scanOffset == 0 || maxBytesToRead < minBytesNeededForScan)
            {
                return new MarshalStreamScanResult(
                    totalBytesRead,
                    scanHasPositiveMatch,
                    _fixedBuffer is not null && _currentReadOffset >= _bufferedByteCount);
            }
        }
    }

    /// <summary>
    /// Efficiently reads each byte of a stream until a scanning function instructs to stop reading.
    /// </summary>
    /// <param name="minBytesNeededForScan">
    /// The minimum number of bytes to pass to the scanning function. See remarks regarding the relationship between
    /// <paramref name="minBytesNeededForScan"/> and <paramref name="maxBytesToRead"/>.
    /// </param>
    /// <param name="maxBytesToRead">
    /// The maximum number of bytes to read. This may be <c>-1</c> to allow reading the entire stream.
    /// See remarks regarding the relationship between <paramref name="minBytesNeededForScan"/> and <paramref name="maxBytesToRead"/>.
    /// </param>
    /// <param name="scanFunc">
    /// A function which is called for each read byte. The function must look at the bytes at the current position and determine if
    /// scanning should be stopped. If scanning should be stopped, the function must return 0. Otherwise the function must return the
    /// number of bytes processed, which also indicates the number of bytes to skip before the next call of the scan function.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task yielding a <see cref="Phaeyz.Marshalling.MarshalStreamScanResult"/> object.
    /// </returns>
    /// <remarks>
    /// If <paramref name="minBytesNeededForScan"/> is more than <c>1</c>, <paramref name="maxBytesToRead"/> can never be read
    /// because reading the minimum of bytes needed for scan may cause us to read beyond the maximum bytes to read.
    /// <br/>
    /// For performance reasons, the scan function should scan the entire memory buffer provided to it until a match is found, instead of
    /// scanning only the top of the buffer before returning. While both strategies work, scanning the entire memory buffer is far more efficient.
    /// </remarks>
    /// <exception cref="System.ArgumentNullException">
    /// The <paramref name="scanFunc"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="minBytesNeededForScan"/> is zero or less, <paramref name="minBytesNeededForScan"/> is greater than the total buffer size,
    /// or <paramref name="maxBytesToRead"/> is less than <c>-1</c>.
    /// </exception>
    /// <exception cref="System.InvalidOperationException">
    /// The scan function returned a negative value, or the scan function returned more bytes than it was provided.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not readable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    /// <exception cref="System.OperationCanceledException">
    /// The cancellation token was canceled.
    /// </exception>
    public ValueTask<MarshalStreamScanResult> ScanAsync(
        int minBytesNeededForScan,
        long maxBytesToRead,
        Func<ReadOnlyMemory<byte>, int> scanFunc,
        CancellationToken cancellationToken = default) => ScanAsync(
            null,
            null,
            minBytesNeededForScan,
            maxBytesToRead,
            scanFunc,
            cancellationToken);

    /// <summary>
    /// Efficiently reads each byte of a stream until a scanning function instructs to stop reading.
    /// </summary>
    /// <param name="destinationBuffer">
    /// Each read byte which did not match the scan function is copied to this buffer. The maximum bytes to read is defined
    /// by the length of this buffer.
    /// </param>
    /// <param name="minBytesNeededForScan">
    /// The minimum number of bytes to pass to the scanning function.
    /// </param>
    /// <param name="scanFunc">
    /// A function which is called for each read byte. The function must look at the bytes at the current position and determine if
    /// scanning should be stopped. If scanning should be stopped, the function must return 0. Otherwise the function must return the
    /// number of bytes processed, which also indicates the number of bytes to skip before the next call of the scan function.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task yielding a <see cref="Phaeyz.Marshalling.MarshalStreamScanResult"/> object.
    /// </returns>
    /// <remarks>
    /// For performance reasons, the scan function should scan the entire memory buffer provided to it until a match is found, instead of
    /// scanning only the top of the buffer before returning. While both strategies work, scanning the entire memory buffer is far more efficient.
    /// </remarks>
    /// <exception cref="System.ArgumentNullException">
    /// The <paramref name="scanFunc"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="minBytesNeededForScan"/> is zero or less, or <paramref name="minBytesNeededForScan"/> is greater than the total buffer size.
    /// </exception>
    /// <exception cref="System.InvalidOperationException">
    /// The scan function returned a negative value, or the scan function returned more bytes than it was provided.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not readable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    /// <exception cref="System.OperationCanceledException">
    /// The cancellation token was canceled.
    /// </exception>
    public ValueTask<MarshalStreamScanResult> ScanAsync(
        Memory<byte> destinationBuffer,
        int minBytesNeededForScan,
        Func<ReadOnlyMemory<byte>, int> scanFunc,
        CancellationToken cancellationToken = default) => ScanAsync(
            destinationBuffer,
            null,
            minBytesNeededForScan,
            -1,
            scanFunc,
            cancellationToken);

    /// <summary>
    /// Efficiently reads each byte of a stream until a scanning function instructs to stop reading.
    /// </summary>
    /// <param name="destinationStream">
    /// Optionally, each read byte which did not match the scan function is written to this stream.
    /// Specify <c>null</c> to not store read bytes.
    /// </param>
    /// <param name="minBytesNeededForScan">
    /// The minimum number of bytes to pass to the scanning function. See remarks regarding the relationship between
    /// <paramref name="minBytesNeededForScan"/> and <paramref name="maxBytesToRead"/>.
    /// </param>
    /// <param name="maxBytesToRead">
    /// The maximum number of bytes to read. This may be <c>-1</c> to allow reading the entire stream.
    /// See remarks regarding the relationship between <paramref name="minBytesNeededForScan"/> and <paramref name="maxBytesToRead"/>.
    /// </param>
    /// <param name="scanFunc">
    /// A function which is called for each read byte. The function must look at the bytes at the current position and determine if
    /// scanning should be stopped. If scanning should be stopped, the function must return 0. Otherwise the function must return the
    /// number of bytes processed, which also indicates the number of bytes to skip before the next call of the scan function.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task yielding a <see cref="Phaeyz.Marshalling.MarshalStreamScanResult"/> object.
    /// </returns>
    /// <remarks>
    /// If <paramref name="minBytesNeededForScan"/> is more than <c>1</c>, <paramref name="maxBytesToRead"/> can never be read
    /// because reading the minimum of bytes needed for scan may cause us to read beyond the maximum bytes to read.
    /// <br/>
    /// For performance reasons, the scan function should scan the entire memory buffer provided to it until a match is found, instead of
    /// scanning only the top of the buffer before returning. While both strategies work, scanning the entire memory buffer is far more efficient.
    /// </remarks>
    /// <exception cref="System.ArgumentException">
    /// The <paramref name="destinationStream"/> is not writable.
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    /// The <paramref name="scanFunc"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="minBytesNeededForScan"/> is zero or less, <paramref name="minBytesNeededForScan"/> is greater than the total buffer size,
    /// or <paramref name="maxBytesToRead"/> is less than <c>-1</c>.
    /// </exception>
    /// <exception cref="System.InvalidOperationException">
    /// The scan function returned a negative value, or the scan function returned more bytes than it was provided.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not readable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    /// <exception cref="System.OperationCanceledException">
    /// The cancellation token was canceled.
    /// </exception>
    public ValueTask<MarshalStreamScanResult> ScanAsync(
        Stream destinationStream,
        int minBytesNeededForScan,
        long maxBytesToRead,
        Func<ReadOnlyMemory<byte>, int> scanFunc,
        CancellationToken cancellationToken = default) => ScanAsync(
            null,
            destinationStream,
            minBytesNeededForScan,
            maxBytesToRead,
            scanFunc,
            cancellationToken);

    /// <summary>
    /// Efficiently reads each byte of a stream until a scanning function instructs to stop reading.
    /// </summary>
    /// <param name="destinationBuffer">
    /// Optionally, each read byte which did not match the scan function is copied to this buffer.
    /// Specify <c>null</c> to not store read bytes.
    /// </param>
    /// <param name="destinationStream">
    /// Optionally, each read byte which did not match the scan function is written to this stream.
    /// Specify <c>null</c> to not store read bytes.
    /// </param>
    /// <param name="minBytesNeededForScan">
    /// The minimum number of bytes to pass to the scanning function. See remarks regarding the relationship between
    /// <paramref name="minBytesNeededForScan"/> and <paramref name="maxBytesToRead"/>.
    /// </param>
    /// <param name="maxBytesToRead">
    /// The maximum number of bytes to read. This may be <c>-1</c> to allow reading the entire stream.
    /// See remarks regarding the relationship between <paramref name="minBytesNeededForScan"/> and <paramref name="maxBytesToRead"/>.
    /// If <paramref name="destinationBuffer"/> is provided, <paramref name="maxBytesToRead"/> is reduced to the length of that buffer.
    /// </param>
    /// <param name="scanFunc">
    /// A function which is called for each read byte. The function must look at the bytes at the current position and determine if
    /// scanning should be stopped. If scanning should be stopped, the function must return 0. Otherwise the function must return the
    /// number of bytes processed, which also indicates the number of bytes to skip before the next call of the scan function.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task yielding a <see cref="Phaeyz.Marshalling.MarshalStreamScanResult"/> object.
    /// </returns>
    /// <remarks>
    /// If <paramref name="minBytesNeededForScan"/> is more than <c>1</c>, <paramref name="maxBytesToRead"/> can never be read
    /// because reading the minimum of bytes needed for scan may cause us to read beyond the maximum bytes to read.
    /// <br/>
    /// For performance reasons, the scan function should scan the entire memory buffer provided to it until a match is found, instead of
    /// scanning only the top of the buffer before returning. While both strategies work, scanning the entire memory buffer is far more efficient.
    /// </remarks>
    /// <exception cref="System.ArgumentException">
    /// The <paramref name="destinationStream"/> is not writable.
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    /// The <paramref name="scanFunc"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="minBytesNeededForScan"/> is zero or less, <paramref name="minBytesNeededForScan"/> is greater than the total buffer size,
    /// or <paramref name="maxBytesToRead"/> is less than <c>-1</c>.
    /// </exception>
    /// <exception cref="System.InvalidOperationException">
    /// The scan function returned a negative value, or the scan function returned more bytes than it was provided.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not readable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    /// <exception cref="System.OperationCanceledException">
    /// The cancellation token was canceled.
    /// </exception>
    private async ValueTask<MarshalStreamScanResult> ScanAsync(
        Memory<byte>? destinationBuffer,
        Stream? destinationStream,
        int minBytesNeededForScan,
        long maxBytesToRead,
        Func<ReadOnlyMemory<byte>, int> scanFunc,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scanFunc);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(minBytesNeededForScan);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(minBytesNeededForScan, TotalBufferSize);
        ArgumentOutOfRangeException.ThrowIfLessThan(maxBytesToRead, -1);

        if (destinationStream is not null && !destinationStream.CanWrite)
        {
            throw new ArgumentException("The destination stream is not writable.", nameof(destinationStream));
        }

        VerifyReadableStream();

        maxBytesToRead = Math.Min(
            destinationBuffer?.Length ?? long.MaxValue,
            maxBytesToRead == -1 ? long.MaxValue : maxBytesToRead);

        long totalBytesRead = 0;
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!await EnsureByteCountAvailableInBufferAsync(minBytesNeededForScan, cancellationToken).ConfigureAwait(false))
            {
                return new MarshalStreamScanResult(totalBytesRead, false, true);
            }

            int scanOffset = 0;
            bool scanHasPositiveMatch = false;
            while (Math.Min(maxBytesToRead, BufferedReadableByteCount - scanOffset) >= minBytesNeededForScan)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ReadOnlyMemory<byte> scanBytes = BufferedReadableBytes[scanOffset..];
                int scannedBytes = scanFunc(scanBytes);
                if (scannedBytes == 0)
                {
                    scanHasPositiveMatch = true;
                    break;
                }
                if (scannedBytes < 0)
                {
                    throw new InvalidOperationException("The scan function returned a negative value.");
                }
                if (scannedBytes > scanBytes.Length)
                {
                    throw new InvalidOperationException("The scan function indicated it scanned more bytes than provided.");
                }
                scanOffset += scannedBytes;
                maxBytesToRead -= scannedBytes;
                totalBytesRead += scannedBytes;
            }

            if (scanOffset > 0)
            {
                if (_readProcessors.Count > 0)
                {
                    Process(_readProcessors, BufferedReadableBytes[..scanOffset].Span);
                }
                if (destinationBuffer is not null)
                {
                    // Using totalBytesRead would reduce this to one line, but totalBytesRead is a long and Slice() requires an int.
                    // So getting around that by recreating the destination buffer so that we copy to the first element.
                    BufferedReadableBytes[..scanOffset].CopyTo(destinationBuffer.Value);
                    destinationBuffer = destinationBuffer.Value[scanOffset..];
                }
                if (destinationStream is not null)
                {
                    await destinationStream.WriteAsync(BufferedReadableBytes[..scanOffset], cancellationToken).ConfigureAwait(false);
                }
            }

            _currentReadOffset += scanOffset;

            if (scanHasPositiveMatch || scanOffset == 0 || maxBytesToRead < minBytesNeededForScan)
            {
                return new MarshalStreamScanResult(
                    totalBytesRead,
                    scanHasPositiveMatch,
                    _fixedBuffer is not null && _currentReadOffset >= _bufferedByteCount);
            }
        }
    }

    #endregion Scan

    #region Seek

    /// <summary>
    /// Sets the position within the current stream.
    /// </summary>
    /// <param name="offset">
    /// The number of bytes to move the position relative to <paramref name="origin"/>.
    /// </param>
    /// <param name="origin">
    /// The origin to use for <paramref name="offset"/>.
    /// </param>
    /// <returns>
    /// The position within the stream.
    /// </returns>
    /// <remarks>
    /// Seeking passed the end of the stream is supported.
    /// </remarks>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="offset"/> is attempting to seek before the beginning of the stream.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not seekable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    public override long Seek(long offset, SeekOrigin origin)
    {
        VerifySeekableStream();
        long oldPosition = Position;
        if (origin == SeekOrigin.Begin)
        {
            offset -= oldPosition;
        }
        else if (origin == SeekOrigin.End)
        {
            offset = Length + offset - oldPosition;
        }
        if (offset == 0)
        {
            return oldPosition;
        }
        FlushWrite();
        if ((offset < 0 && Math.Abs(offset) > _currentReadOffset) || offset > BufferedReadableByteCount)
        {
            long newPosition;
            if (_fixedBuffer is null)
            {
                newPosition = _stream!.Seek(oldPosition + offset, SeekOrigin.Begin);
                _currentReadOffset = 0;
            }
            else
            {
                // Stream.Seek() does support seeking beyond the end of the stream.
                newPosition = _currentReadOffset + offset;
                ArgumentOutOfRangeException.ThrowIfNegative(newPosition, nameof(offset));
                _currentReadOffset = newPosition;
            }
            _bufferedByteCount = 0;
            return newPosition;
        }
        _currentReadOffset += (int)offset;
        return oldPosition + offset;
    }

    /// <summary>
    /// Sets the position within the current stream.
    /// </summary>
    /// <param name="offset">
    /// The number of bytes to move the position relative to <paramref name="origin"/>.
    /// </param>
    /// <param name="origin">
    /// The origin to use for <paramref name="offset"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// The position within the stream.
    /// </returns>
    /// <remarks>
    /// Seeking passed the end of the stream is supported.
    /// </remarks>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="offset"/> is attempting to seek before the beginning of the stream.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not seekable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    /// <exception cref="System.OperationCanceledException">
    /// The cancellation token was canceled.
    /// </exception>
    public async ValueTask<long> SeekAsync(long offset, SeekOrigin origin, CancellationToken cancellationToken = default)
    {
        VerifySeekableStream();
        long oldPosition = Position;
        if (origin == SeekOrigin.Begin)
        {
            offset -= oldPosition;
        }
        else if (origin == SeekOrigin.End)
        {
            offset = Length + offset - oldPosition;
        }
        if (offset == 0)
        {
            return oldPosition;
        }
        await FlushWriteAsync(cancellationToken).ConfigureAwait(false);
        if ((offset < 0 && Math.Abs(offset) > _currentReadOffset) || offset > BufferedReadableByteCount)
        {
            long newPosition;
            if (_fixedBuffer is null)
            {
                newPosition = _stream!.Seek(oldPosition + offset, SeekOrigin.Begin); // There is no Stream.SeekAsync, unfortunately.
                _currentReadOffset = 0;
            }
            else
            {
                // Stream.Seek() does support seeking beyond the end of the stream.
                newPosition = _currentReadOffset + offset;
                ArgumentOutOfRangeException.ThrowIfNegative(newPosition, nameof(offset));
                _currentReadOffset = newPosition;
            }
            _bufferedByteCount = 0;
            return newPosition;
        }
        _currentReadOffset += (int)offset;
        return oldPosition + offset;
    }

    #endregion Seek

    #region SetLength

    /// <summary>
    /// Sets the length of the current stream.
    /// </summary>
    /// <param name="value">
    /// The number of bytes to set the length of the stream to.
    /// </param>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// Either the current stream does not support seeking or writing.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    public override void SetLength(long value)
    {
        VerifyNotDisposed();
        if (!CanSeek || !CanWrite)
        {
            throw new NotSupportedException(
                "The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output.");
        }
        Flush();
        _stream!.SetLength(value);
    }

    #endregion SetLength

    #region Skip

    /// <summary>
    /// Reads and discards a sequence of bytes from the current stream and advances the position within the stream by the
    /// specified number of bytes.
    /// </summary>
    /// <param name="byteCount">
    /// A positive number indicating the number of bytes to skip.
    /// </param>
    /// <returns>
    /// The number of bytes actually skipped, which may be less than the requested number of bytes if the
    /// end of the stream was reached.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="byteCount"/> is negative.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not readable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    public long Skip(long byteCount)
    {
        VerifyReadableStream();
        ArgumentOutOfRangeException.ThrowIfNegative(byteCount);

        // Do not try to seek because seeking may go beyond the end of the stream.
        long bytesSkipped = 0;
        while (byteCount > 0)
        {
            // If the existing buffer has enough bytes to skip, just update buffer references.
            int bufferedCount = BufferedReadableByteCount;
            if (byteCount <= bufferedCount)
            {
                if (_readProcessors.Count > 0)
                {
                    Process(_readProcessors, BufferedReadableBytes.Span[..(int)byteCount]);
                }
                _currentReadOffset += byteCount;
                bytesSkipped += byteCount;
                break;
            }

            // If there are any buffered bytes, skip them so we can fill the buffer back up.
            if (bufferedCount > 0)
            {
                if (_readProcessors.Count > 0)
                {
                    Process(_readProcessors, BufferedReadableBytes.Span);
                }
                _currentReadOffset = _bufferedByteCount;
                byteCount -= bufferedCount;
                bytesSkipped += bufferedCount;
            }

            // Refill the buffer.
            if (!EnsureByteCountAvailableInBuffer(1))
            {
                // Since only 1 was passed in, the end of the stream was reached.
                break;
            }
        }

        return bytesSkipped;
    }

    /// <summary>
    /// Reads and discards a sequence of bytes from the current stream and advances the position within the stream by the
    /// specified number of bytes.
    /// </summary>
    /// <param name="byteCount">
    /// A positive number indicating the number of bytes to skip.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// The number of bytes actually skipped, which may be less than the requested number of bytes if the
    /// end of the stream was reached.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="byteCount"/> is negative.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not readable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    public async ValueTask<long> SkipAsync(long byteCount, CancellationToken cancellationToken = default)
    {
        VerifyReadableStream();
        ArgumentOutOfRangeException.ThrowIfNegative(byteCount);

        // Do not try to seek because seeking may go beyond the end of the stream.
        long bytesSkipped = 0;
        while (byteCount > 0)
        {
            // If the existing buffer has enough bytes to skip, just update buffer references.
            int bufferedCount = BufferedReadableByteCount;
            if (byteCount <= bufferedCount)
            {
                if (_readProcessors.Count > 0)
                {
                    Process(_readProcessors, BufferedReadableBytes.Span[..(int)byteCount]);
                }
                _currentReadOffset += byteCount;
                bytesSkipped += byteCount;
                break;
            }

            // If there are any buffered bytes, skip them so we can fill the buffer back up.
            if (bufferedCount > 0)
            {
                if (_readProcessors.Count > 0)
                {
                    Process(_readProcessors, BufferedReadableBytes.Span);
                }
                _currentReadOffset = _bufferedByteCount;
                byteCount -= bufferedCount;
                bytesSkipped += bufferedCount;
            }

            // Refill the buffer.
            if (!await EnsureByteCountAvailableInBufferAsync(1, cancellationToken).ConfigureAwait(false))
            {
                // Since only 1 was passed in, the end of the stream was reached.
                break;
            }
        }

        return bytesSkipped;
    }

    #endregion Skip

    #region Verify

    /// <summary>
    /// Throws an exception if the stream is disposed.
    /// </summary>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    private void VerifyNotDisposed() => ObjectDisposedException.ThrowIf(IsDisposed, this);

    /// <summary>
    /// Throws an exception if the stream is not readable.
    /// </summary>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not readable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    private void VerifyReadableStream()
    {
        VerifyNotDisposed();
        if (!CanRead)
        {
            throw new NotSupportedException("The stream is not readable.");
        }
    }

    /// <summary>
    /// Throws an exception if the stream is not seekable.
    /// </summary>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not seekable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    private void VerifySeekableStream()
    {
        VerifyNotDisposed();
        if (!CanSeek)
        {
            throw new NotSupportedException("The stream is not seekable.");
        }
    }

    /// <summary>
    /// Throws an exception if the stream is not writable.
    /// </summary>
    /// <exception cref="System.NotSupportedException">
    /// The stream is not writable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    private void VerifyWritableStream()
    {
        VerifyNotDisposed();
        if (!CanWrite)
        {
            throw new NotSupportedException("The stream is not writable.");
        }
    }

    #endregion Verify

    #region Write

    /// <summary>
    /// Writes a sequence of bytes to the current stream and advances the current position within
    /// the stream by the number of bytes written.
    /// </summary>
    /// <param name="buffer">
    /// A buffer containing the bytes to write.
    /// </param>
    /// <param name="offset">
    /// The starting offset within <paramref name="buffer"/> containing the bytes to write.
    /// </param>
    /// <param name="count">
    /// The count of bytes to write within <paramref name="buffer"/> starting at <paramref name="offset"/>.
    /// </param>
    /// <exception cref="System.ArgumentNullException">
    /// <paramref name="buffer"/> was null.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="offset"/> was outside the bounds of <paramref name="buffer"/>, or
    /// <paramref name="count"/> was negative, or the range specified by the combination of
    /// <paramref name="offset"/> and <paramref name="count"/> exceed the length of <paramref name="buffer"/>.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The current stream is not writable, or the read buffer cannot be flushed because the current stream is not seekable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    public override void Write(byte[] buffer, int offset, int count)
    {
        ValidateBufferArguments(buffer, offset, count);
        FlushReadBeforeWrite();

        Process(_writeProcessors, buffer, offset, count);

        // Loop until the entire input buffer has been processed.
        int endOffset = offset + count;
        while (offset < endOffset)
        {
            // Copy the input buffer to the backing buffer under two conditions:
            // 1) There is already data in the backing buffer, so we can try to fill it up and reduce the number of stream writes.
            // 2) The remaining input buffer has less bytes than the backing buffer. If the remaining input buffer has the same
            //    or more bytes as the backing buffer, we know that we are going to write to the stream directly, and we can do
            //    that without having an extra copy to the backing buffer. If the remaining input buffer has less bytes than the
            //    full backing buffer, are not going to write directly to the stream, so fill up the backing buffer the most we can.
            if (_bufferedByteCount > 0 || (endOffset - offset) < _buffer!.Length)
            {
                int bufferSizeToCopy = Math.Min(endOffset - offset, _buffer!.Length - _bufferedByteCount);
                if (bufferSizeToCopy > 0)
                {
                    Buffer.BlockCopy(buffer, offset, _buffer!, _bufferedByteCount, bufferSizeToCopy);
                    _bufferedByteCount += bufferSizeToCopy;
                    offset += bufferSizeToCopy;
                    _bufferHasUnpersistedBytes = true;
                }
            }
            // If we filled up the backing buffer, write it to the underlying stream.
            if (_bufferedByteCount == _buffer!.Length)
            {
                _stream!.Write(_buffer, 0, _bufferedByteCount); // Don't use the Span version because it does an extra buffer copy.
                _bufferedByteCount = 0;
                _bufferHasUnpersistedBytes = false;
            }
            // If the input buffer has the same or more remaining bytes as the size of the backing buffer, write the whole buffer
            // directly to the underlying stream. It is guaranteed by this point that the backing buffer has been flushed.
            if ((endOffset - offset) >= _buffer!.Length)
            {
                _stream!.Write(buffer, offset, endOffset - offset); // Don't use the Span version because it does an extra buffer copy.
                break;
            }
        }
    }

    /// <summary>
    /// Writes a sequence of bytes to the current stream and advances the current position within
    /// the stream by the number of bytes written.
    /// </summary>
    /// <param name="buffer">
    /// A buffer containing the bytes to write.
    /// </param>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The current stream is not writable, or the read buffer cannot be flushed because the current stream is not seekable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    public override void Write(ReadOnlySpan<byte> buffer)
    {
        FlushReadBeforeWrite();

        Process(_writeProcessors, buffer);

        // Loop until the entire input buffer has been processed.
        for (int offset = 0; offset < buffer.Length;)
        {
            // Copy the input buffer to the backing buffer under two conditions:
            // 1) There is already data in the backing buffer, so we can try to fill it up and reduce the number of stream writes.
            // 2) The remaining input buffer has less bytes than the backing buffer. If the remaining input buffer has the same
            //    or more bytes as the backing buffer, we know that we are going to write to the stream directly, and we can do
            //    that without having an extra copy to the backing buffer. If the remaining input buffer has less bytes than the
            //    full backing buffer, are not going to write directly to the stream, so fill up the backing buffer the most we can.
            if (_bufferedByteCount > 0 || (buffer.Length - offset) < _buffer!.Length)
            {
                int bufferSizeToCopy = Math.Min(buffer.Length - offset, _buffer!.Length - _bufferedByteCount);
                if (bufferSizeToCopy > 0)
                {
                    buffer.Slice(offset, bufferSizeToCopy).CopyTo(_buffer.AsSpan(_bufferedByteCount));
                    _bufferedByteCount += bufferSizeToCopy;
                    offset += bufferSizeToCopy;
                    _bufferHasUnpersistedBytes = true;
                }
            }
            // If we filled up the backing buffer, write it to the underlying stream.
            if (_bufferedByteCount == _buffer!.Length)
            {
                _stream!.Write(_buffer, 0, _bufferedByteCount); // Don't use the Span version because it does an extra buffer copy.
                _bufferedByteCount = 0;
                _bufferHasUnpersistedBytes = false;
            }
            // If the input buffer has the same or more remaining bytes as the size of the backing buffer, write the whole buffer
            // directly to the underlying stream. It is guaranteed by this point that the backing buffer has been flushed.
            if ((buffer.Length - offset) >= _buffer!.Length)
            {
                _stream!.Write(buffer[offset..]);
                break;
            }
        }
    }

    /// <summary>
    /// Writes a sequence of bytes to the current stream and advances the current position within
    /// the stream by the number of bytes written.
    /// </summary>
    /// <param name="buffer">
    /// A buffer containing the bytes to write.
    /// </param>
    /// <param name="offset">
    /// The starting offset within <paramref name="buffer"/> containing the bytes to write.
    /// </param>
    /// <param name="count">
    /// The count of bytes to write within <paramref name="buffer"/> starting at <paramref name="offset"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task which is completed when the operation is complete.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// <paramref name="buffer"/> was null.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="offset"/> was outside the bounds of <paramref name="buffer"/>, or
    /// <paramref name="count"/> was negative, or the range specified by the combination of
    /// <paramref name="offset"/> and <paramref name="count"/> exceed the length of <paramref name="buffer"/>.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The current stream is not writable, or the read buffer cannot be flushed because the current stream is not seekable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    /// <exception cref="System.OperationCanceledException">
    /// The cancellation token was canceled.
    /// </exception>
    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        ValidateBufferArguments(buffer, offset, count);
        await WriteAsync(buffer.AsMemory(offset, count), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Writes a sequence of bytes to the current stream and advances the current position within
    /// the stream by the number of bytes written.
    /// </summary>
    /// <param name="buffer">
    /// A buffer containing the bytes to write.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task which is completed when the operation is complete.
    /// </returns>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The current stream is not writable, or the read buffer cannot be flushed because the current stream is not seekable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    /// <exception cref="System.OperationCanceledException">
    /// The cancellation token was canceled.
    /// </exception>
    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        FlushReadBeforeWrite();

        if (_writeProcessors.Count > 0)
        {
            Process(_writeProcessors, buffer.Span);
        }

        // Loop until the entire input buffer has been processed.
        for (int offset = 0; offset < buffer.Length;)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Copy the input buffer to the backing buffer under two conditions:
            // 1) There is already data in the backing buffer, so we can try to fill it up and reduce the number of stream writes.
            // 2) The remaining input buffer has less bytes than the backing buffer. If the remaining input buffer has the same
            //    or more bytes as the backing buffer, we know that we are going to write to the stream directly, and we can do
            //    that without having an extra copy to the backing buffer. If the remaining input buffer has less bytes than the
            //    full backing buffer, are not going to write directly to the stream, so fill up the backing buffer the most we can.
            if (_bufferedByteCount > 0 || (buffer.Length - offset) < _buffer!.Length)
            {
                int bufferSizeToCopy = Math.Min(buffer.Length - offset, _buffer!.Length - _bufferedByteCount);
                if (bufferSizeToCopy > 0)
                {
                    buffer.Slice(offset, bufferSizeToCopy).CopyTo(_buffer.AsMemory(_bufferedByteCount));
                    _bufferedByteCount += bufferSizeToCopy;
                    offset += bufferSizeToCopy;
                    _bufferHasUnpersistedBytes = true;
                }
            }
            // If we filled up the backing buffer, write it to the underlying stream.
            if (_bufferedByteCount == _buffer!.Length)
            {
                await _stream!.WriteAsync(_buffer.AsMemory(0, _bufferedByteCount), cancellationToken).ConfigureAwait(false);
                _bufferedByteCount = 0;
                _bufferHasUnpersistedBytes = false;
            }
            // If the input buffer has the same or more remaining bytes as the size of the backing buffer, write the whole buffer
            // directly to the underlying stream. It is guaranteed by this point that the backing buffer has been flushed.
            if ((buffer.Length - offset) >= _buffer!.Length)
            {
                await _stream!.WriteAsync(buffer[offset..], cancellationToken).ConfigureAwait(false);
                break;
            }
        }
    }

    #endregion Write

    #region WriteByte

    /// <summary>
    /// Writes a byte to the current stream and advances the current position within the stream by one byte.
    /// </summary>
    /// <param name="value">
    /// The byte to write to the stream.
    /// </param>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The current stream is not writable, or the read buffer cannot be flushed because the current stream is not seekable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    public override void WriteByte(byte value)
    {
        byte[] buffer = ArrayPool<byte>.Shared.Rent(1);
        try
        {
            buffer[0] = value;
            Write(buffer, 0, 1);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    /// <summary>
    /// Writes a byte to the current stream and advances the current position within the stream by one byte.
    /// </summary>
    /// <param name="value">
    /// The byte to write to the stream.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task which is completed when the operation is complete.
    /// </returns>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The current stream is not writable, or the read buffer cannot be flushed because the current stream is not seekable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    /// <exception cref="System.OperationCanceledException">
    /// The cancellation token was canceled.
    /// </exception>
    public async ValueTask WriteByteAsync(byte value, CancellationToken cancellationToken = default)
    {
        byte[] buffer = ArrayPool<byte>.Shared.Rent(1);
        try
        {
            buffer[0] = value;
            await WriteAsync(new(buffer, 0, 1), cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    #endregion WriteByte

    #region WriteString

    /// <summary>
    /// Writes a string to the stream.
    /// </summary>
    /// <param name="encoding">
    /// The <see cref="System.Text.Encoding"/> object to use for encoding the characters of the string to bytes onto the stream.
    /// </param>
    /// <param name="value">
    /// The string value to write to the stream.
    /// </param>
    /// <param name="writeNullTerminator">
    /// If <c>true</c> a null terminator will be persisted after the string. The default value is <c>false</c>.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task yielding the number of bytes written to the stream.
    /// </returns>
    /// <exception cref="System.Text.EncoderFallbackException">
    /// The <paramref name="encoding"/>'s <see cref="System.Text.Encoding.EncoderFallback"/> property is set
    /// to <see cref="System.Text.EncoderExceptionFallback"/> and a fallback occurred. When this happens, the
    /// stream's new position is undefined.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The current stream is not writable, or the read buffer cannot be flushed because the current stream is not seekable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    public int WriteString(
        Encoding encoding,
        ReadOnlySpan<char> value,
        bool writeNullTerminator = false,
        CancellationToken cancellationToken = default) =>
            WriteString(encoding.GetEncoder(), value, writeNullTerminator, cancellationToken);

    /// <summary>
    /// Writes a string to the stream.
    /// </summary>
    /// <param name="encoder">
    /// The <see cref="System.Text.Encoder"/> object to use for encoding the characters of the string to bytes onto the stream.
    /// </param>
    /// <param name="value">
    /// The string value to write to the stream.
    /// </param>
    /// <param name="writeNullTerminator">
    /// If <c>true</c> a null terminator will be persisted after the string. The default value is <c>false</c>.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task yielding the number of bytes written to the stream.
    /// </returns>
    /// <exception cref="System.Text.EncoderFallbackException">
    /// The <paramref name="encoder"/>'s <see cref="System.Text.Encoder.Fallback"/> property (or the owning
    /// <see cref="System.Text.Encoding"/>'s <see cref="System.Text.Encoding.EncoderFallback"/> property) is set
    /// to <see cref="System.Text.EncoderExceptionFallback"/> and a fallback occurred. When this happens, the
    /// stream's new position is undefined.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The current stream is not writable, or the read buffer cannot be flushed because the current stream is not seekable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    public int WriteString(
        Encoder encoder,
        ReadOnlySpan<char> value,
        bool writeNullTerminator = false,
        CancellationToken cancellationToken = default)
    {
        FlushReadBeforeWrite();

        // This may cause some allocations, unfortunately, but there is no other strategy here.
        encoder.Reset();

        // Get the null terminator size. We will need this for both computing code unit size,
        // and persisting a null terminator at the end if the caller wanted that.
        int nullTerminatorSize = encoder.GetByteCount(['\0'], true);

        // Compute the minimum number of bytes per character required for the target buffer.
        int minBytesPerCharacter = nullTerminatorSize * c_codeUnitsToReservePerCharacterForStringWriteBuffer;

        // Compute the minimum buffer for the string write operation.
        int minimumBufferSize = minBytesPerCharacter * c_minimumCharactersForStringWriteBuffer;

        // These track the progress.
        int charactersProcessed = 0;
        int totalBytesWritten = 0;

        byte[]? minimumBuffer = null;
        try
        {
            // Bypass all the heavy work if the input string is empty. Do not exit as
            // the null terminator may need to be written.
            if (value.Length > 0)
            {
                // If the built-in buffer is insufficient, rent a buffer from the pool.
                if (minimumBufferSize > _buffer!.Length)
                {
                    minimumBuffer = ArrayPool<byte>.Shared.Rent(minimumBufferSize);
                }

                // Compute the maximum number of characters to process per pass. If in the unlikely case
                // the buffer cannot handle this, it will be reduced.
                int maxCharactersPerPass = (minimumBuffer?.Length ?? _buffer!.Length) / minBytesPerCharacter;

                // Loop until all characters are processed.
                while (charactersProcessed < value.Length)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // Flush the stream buffer as necessary.
                    if (minimumBuffer is null && _buffer!.Length - _bufferedByteCount < minBytesPerCharacter)
                    {
                        FlushWrite();
                    }

                    // Write characters to the buffer. A loop is needed in the instance the number of characters
                    // would produce too many bytes, and we need to reduce the number of characters to process.
                    // However, this would be very unlikely to ever happen.
                    int bytesEncoded;
                    while (true)
                    {
                        try
                        {
                            // Compute the number of characters to process in this pass.
                            int charactersToProcess = Math.Min(maxCharactersPerPass, value.Length - charactersProcessed);

                            // If this is the last of the characters, we need to flush.
                            bool flush = charactersToProcess == value.Length - charactersProcessed;

                            // Encode.
                            bytesEncoded = minimumBuffer is not null
                                ? encoder.GetBytes(value.Slice(charactersProcessed, charactersToProcess), minimumBuffer, flush)
                                : encoder.GetBytes(value.Slice(charactersProcessed, charactersToProcess), _buffer.AsSpan(_bufferedByteCount), flush);

                            // Track the characters processed.
                            charactersProcessed += charactersToProcess;
                            break;
                        }
                        catch (ArgumentException)
                        {
                            // If the buffer is too small, reduce the number of characters to process.
                            // This will also impact future encode passes. However, this is very unlikely
                            // the buffer reserved was very generous.
                            if ((maxCharactersPerPass /= 2) == 0)
                            {
                                throw;
                            }
                        }
                    }

                    // If the minimumBuffer was used, we now need to move the bytes to the stream buffer.
                    if (minimumBuffer is not null)
                    {
                        // If there just happens to be enough space on the stream buffer, simply copy.
                        if (_buffer!.Length - _bufferedByteCount >= bytesEncoded)
                        {
                            Span<byte> sourceBuffer = minimumBuffer.AsSpan(0, bytesEncoded);
                            Process(_writeProcessors, sourceBuffer);
                            sourceBuffer.CopyTo(_buffer.AsSpan(_bufferedByteCount));
                            _bufferedByteCount += bytesEncoded;
                            _bufferHasUnpersistedBytes = true;
                        }
                        else
                        {
                            // Otherwise smart logic is necessary to handle the new bytes.
                            Write(minimumBuffer.AsSpan(0, bytesEncoded));
                        }
                    }
                    else
                    {
                        // If a minimum buffer was not used, we already wrote the bytes to the stream buffer.
                        if (_writeProcessors.Count > 0)
                        {
                            Process(_writeProcessors, _buffer.AsSpan(_bufferedByteCount, bytesEncoded));
                        }
                        _bufferedByteCount += bytesEncoded;
                        _bufferHasUnpersistedBytes = true;
                    }

                    // Track the number of bytes written.
                    totalBytesWritten += bytesEncoded;
                }
            }

            // Writing the string is done, now check to see if the caller wants to write a null terminator.
            if (writeNullTerminator)
            {
                // See if the buffer is big enough to write the null terminator directly.
                if (_buffer!.Length >= nullTerminatorSize)
                {
                    // If the buffer already contains content, we need to make sure there sufficient
                    // space remaining for a null terminator. If there is not, flush the buffer.
                    if (_buffer!.Length - _bufferedByteCount < nullTerminatorSize)
                    {
                        FlushWrite();
                    }
                    // Simply copy the null terminator to the buffer.
                    Span<byte> nullTerminatorBytes = s_nullTerminatorBytes.AsSpan(0, nullTerminatorSize);
                    Process(_writeProcessors, nullTerminatorBytes);
                    nullTerminatorBytes.CopyTo(_buffer.AsSpan(_bufferedByteCount));
                    _bufferedByteCount += nullTerminatorSize;
                    _bufferHasUnpersistedBytes = true;
                }
                else
                {
                    // The buffer is too small even for the null terminator, so leverage all the logic in WriteAsync.
                    Write(s_nullTerminatorBytes.AsSpan(0, nullTerminatorSize));
                }

                // Record the bytes written for the null terminator.
                totalBytesWritten += nullTerminatorSize;
            }

            return totalBytesWritten;
        }
        finally
        {
            // If a minimumBuffer was rented, return it back to the pool.
            if (minimumBuffer is not null)
            {
                ArrayPool<byte>.Shared.Return(minimumBuffer);
            }
        }
    }

    /// <summary>
    /// Writes a string to the stream.
    /// </summary>
    /// <param name="encoding">
    /// The <see cref="System.Text.Encoding"/> object to use for encoding the characters of the string to bytes onto the stream.
    /// </param>
    /// <param name="value">
    /// The string value to write to the stream.
    /// </param>
    /// <param name="writeNullTerminator">
    /// If <c>true</c> a null terminator will be persisted after the string. The default value is <c>false</c>.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task yielding the number of bytes written to the stream.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// The input string value is <c>null</c>.
    /// </exception>
    /// <exception cref="System.Text.EncoderFallbackException">
    /// The <paramref name="encoding"/>'s <see cref="System.Text.Encoding.EncoderFallback"/> property is set
    /// to <see cref="System.Text.EncoderExceptionFallback"/> and a fallback occurred. When this happens, the
    /// stream's new position is undefined.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The current stream is not writable, or the read buffer cannot be flushed because the current stream is not seekable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    /// <exception cref="System.OperationCanceledException">
    /// The cancellation token was canceled.
    /// </exception>
    public ValueTask<int> WriteStringAsync(
        Encoding encoding,
        string value,
        bool writeNullTerminator = false,
        CancellationToken cancellationToken = default) => WriteStringAsync(
            encoding.GetEncoder(),
            value?.AsMemory() ?? throw new ArgumentNullException(nameof(value)),
            writeNullTerminator,
            cancellationToken);

    /// <summary>
    /// Writes a string to the stream.
    /// </summary>
    /// <param name="encoding">
    /// The <see cref="System.Text.Encoding"/> object to use for encoding the characters of the string to bytes onto the stream.
    /// </param>
    /// <param name="value">
    /// The string value to write to the stream.
    /// </param>
    /// <param name="writeNullTerminator">
    /// If <c>true</c> a null terminator will be persisted after the string. The default value is <c>false</c>.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task yielding the number of bytes written to the stream.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// The input string value is <c>null</c>.
    /// </exception>
    /// <exception cref="System.Text.EncoderFallbackException">
    /// The <paramref name="encoding"/>'s <see cref="System.Text.Encoding.EncoderFallback"/> property is set
    /// to <see cref="System.Text.EncoderExceptionFallback"/> and a fallback occurred. When this happens, the
    /// stream's new position is undefined.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The current stream is not writable, or the read buffer cannot be flushed because the current stream is not seekable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    /// <exception cref="System.OperationCanceledException">
    /// The cancellation token was canceled.
    /// </exception>
    public ValueTask<int> WriteStringAsync(
        Encoding encoding,
        ReadOnlyMemory<char> value,
        bool writeNullTerminator = false,
        CancellationToken cancellationToken = default) =>
            WriteStringAsync(encoding.GetEncoder(), value, writeNullTerminator, cancellationToken);

    /// <summary>
    /// Writes a string to the stream.
    /// </summary>
    /// <param name="encoder">
    /// The <see cref="System.Text.Encoder"/> object to use for encoding the characters of the string to bytes onto the stream.
    /// </param>
    /// <param name="value">
    /// The string value to write to the stream.
    /// </param>
    /// <param name="writeNullTerminator">
    /// If <c>true</c> a null terminator will be persisted after the string. The default value is <c>false</c>.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task yielding the number of bytes written to the stream.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// The input string value is <c>null</c>.
    /// </exception>
    /// <exception cref="System.Text.EncoderFallbackException">
    /// The <paramref name="encoder"/>'s <see cref="System.Text.Encoder.Fallback"/> property (or the owning
    /// <see cref="System.Text.Encoding"/>'s <see cref="System.Text.Encoding.EncoderFallback"/> property) is set
    /// to <see cref="System.Text.EncoderExceptionFallback"/> and a fallback occurred. When this happens, the
    /// stream's new position is undefined.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The current stream is not writable, or the read buffer cannot be flushed because the current stream is not seekable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    /// <exception cref="System.OperationCanceledException">
    /// The cancellation token was canceled.
    /// </exception>
    public ValueTask<int> WriteStringAsync(
        Encoder encoder,
        string value,
        bool writeNullTerminator = false,
        CancellationToken cancellationToken = default) => WriteStringAsync(
            encoder,
            value?.AsMemory() ?? throw new ArgumentNullException(nameof(value)),
            writeNullTerminator,
            cancellationToken);

    /// <summary>
    /// Writes a string to the stream.
    /// </summary>
    /// <param name="encoder">
    /// The <see cref="System.Text.Encoder"/> object to use for encoding the characters of the string to bytes onto the stream.
    /// </param>
    /// <param name="value">
    /// The string value to write to the stream.
    /// </param>
    /// <param name="writeNullTerminator">
    /// If <c>true</c> a null terminator will be persisted after the string. The default value is <c>false</c>.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token which may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task yielding the number of bytes written to the stream.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// The input string value is <c>null</c>.
    /// </exception>
    /// <exception cref="System.Text.EncoderFallbackException">
    /// The <paramref name="encoder"/>'s <see cref="System.Text.Encoder.Fallback"/> property (or the owning
    /// <see cref="System.Text.Encoding"/>'s <see cref="System.Text.Encoding.EncoderFallback"/> property) is set
    /// to <see cref="System.Text.EncoderExceptionFallback"/> and a fallback occurred. When this happens, the
    /// stream's new position is undefined.
    /// </exception>
    /// <exception cref="System.IO.IOException">
    /// An I/O error occurred.
    /// </exception>
    /// <exception cref="System.NotSupportedException">
    /// The current stream is not writable, or the read buffer cannot be flushed because the current stream is not seekable.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The stream is disposed.
    /// </exception>
    /// <exception cref="System.OperationCanceledException">
    /// The cancellation token was canceled.
    /// </exception>
    public async ValueTask<int> WriteStringAsync(
        Encoder encoder,
        ReadOnlyMemory<char> value,
        bool writeNullTerminator = false,
        CancellationToken cancellationToken = default)
    {
        FlushReadBeforeWrite();

        // This may cause some allocations, unfortunately, but there is no other strategy here.
        encoder.Reset();

        // Get the null terminator size. We will need this for both computing code unit size,
        // and persisting a null terminator at the end if the caller wanted that.
        int nullTerminatorSize = encoder.GetByteCount(['\0'], true);

        // Compute the minimum number of bytes per character required for the target buffer.
        int minBytesPerCharacter = nullTerminatorSize * c_codeUnitsToReservePerCharacterForStringWriteBuffer;

        // Compute the minimum buffer for the string write operation.
        int minimumBufferSize = minBytesPerCharacter * c_minimumCharactersForStringWriteBuffer;

        // These track the progress.
        int charactersProcessed = 0;
        int totalBytesWritten = 0;

        byte[]? minimumBuffer = null;
        try
        {
            // Bypass all the heavy work if the input string is empty. Do not exit as
            // the null terminator may need to be written.
            if (value.Length > 0)
            {
                // If the built-in buffer is insufficient, rent a buffer from the pool.
                if (minimumBufferSize > _buffer!.Length)
                {
                    minimumBuffer = ArrayPool<byte>.Shared.Rent(minimumBufferSize);
                }

                // Compute the maximum number of characters to process per pass. If in the unlikely case
                // the buffer cannot handle this, it will be reduced.
                int maxCharactersPerPass = (minimumBuffer?.Length ?? _buffer!.Length) / minBytesPerCharacter;

                // Loop until all characters are processed.
                while (charactersProcessed < value.Length)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // Flush the stream buffer as necessary.
                    if (minimumBuffer is null && _buffer!.Length - _bufferedByteCount < minBytesPerCharacter)
                    {
                        await FlushWriteAsync(cancellationToken).ConfigureAwait(false);
                    }

                    // Write characters to the buffer. A loop is needed in the instance the number of characters
                    // would produce too many bytes, and we need to reduce the number of characters to process.
                    // However, this would be very unlikely to ever happen.
                    int bytesEncoded;
                    while (true)
                    {
                        try
                        {
                            // Compute the number of characters to process in this pass.
                            int charactersToProcess = Math.Min(maxCharactersPerPass, value.Length - charactersProcessed);

                            // If this is the last of the characters, we need to flush.
                            bool flush = charactersToProcess == value.Length - charactersProcessed;

                            // Encode.
                            bytesEncoded = minimumBuffer is not null
                                ? encoder.GetBytes(value.Span.Slice(charactersProcessed, charactersToProcess), minimumBuffer, flush)
                                : encoder.GetBytes(value.Span.Slice(charactersProcessed, charactersToProcess), _buffer.AsSpan(_bufferedByteCount), flush);

                            // Track the characters processed.
                            charactersProcessed += charactersToProcess;
                            break;
                        }
                        catch (ArgumentException)
                        {
                            // If the buffer is too small, reduce the number of characters to process.
                            // This will also impact future encode passes. However, this is very unlikely
                            // the buffer reserved was very generous.
                            if ((maxCharactersPerPass /= 2) == 0)
                            {
                                throw;
                            }
                        }
                    }

                    // If the minimumBuffer was used, we now need to move the bytes to the stream buffer.
                    if (minimumBuffer is not null)
                    {
                        // If there just happens to be enough space on the stream buffer, simply copy.
                        if (_buffer!.Length - _bufferedByteCount >= bytesEncoded)
                        {
                            Span<byte> sourceBuffer = minimumBuffer.AsSpan(0, bytesEncoded);
                            Process(_writeProcessors, sourceBuffer);
                            sourceBuffer.CopyTo(_buffer.AsSpan(_bufferedByteCount));
                            _bufferedByteCount += bytesEncoded;
                            _bufferHasUnpersistedBytes = true;
                        }
                        else
                        {
                            // Otherwise smart logic is necessary to handle the new bytes.
                            await WriteAsync(minimumBuffer.AsMemory(0, bytesEncoded), cancellationToken).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        // If a minimum buffer was not used, we already wrote the bytes to the stream buffer.
                        if (_writeProcessors.Count > 0)
                        {
                            Process(_writeProcessors, _buffer.AsSpan(_bufferedByteCount, bytesEncoded));
                        }
                        _bufferedByteCount += bytesEncoded;
                        _bufferHasUnpersistedBytes = true;
                    }

                    // Track the number of bytes written.
                    totalBytesWritten += bytesEncoded;
                }
            }

            // Writing the string is done, now check to see if the caller wants to write a null terminator.
            if (writeNullTerminator)
            {
                // See if the buffer is big enough to write the null terminator directly.
                if (_buffer!.Length >= nullTerminatorSize)
                {
                    // If the buffer already contains content, we need to make sure there sufficient
                    // space remaining for a null terminator. If there is not, flush the buffer.
                    if (_buffer!.Length - _bufferedByteCount < nullTerminatorSize)
                    {
                        await FlushWriteAsync(cancellationToken).ConfigureAwait(false);
                    }
                    // Simply copy the null terminator to the buffer.
                    Span<byte> nullTerminatorBytes = s_nullTerminatorBytes.AsSpan(0, nullTerminatorSize);
                    Process(_writeProcessors, nullTerminatorBytes);
                    nullTerminatorBytes.CopyTo(_buffer.AsSpan(_bufferedByteCount));
                    _bufferedByteCount += nullTerminatorSize;
                    _bufferHasUnpersistedBytes = true;
                }
                else
                {
                    // The buffer is too small even for the null terminator, so leverage all the logic in WriteAsync.
                    await WriteAsync(s_nullTerminatorBytes.AsMemory(0, nullTerminatorSize), cancellationToken).ConfigureAwait(false);
                }

                // Record the bytes written for the null terminator.
                totalBytesWritten += nullTerminatorSize;
            }

            return totalBytesWritten;
        }
        finally
        {
            // If a minimumBuffer was rented, return it back to the pool.
            if (minimumBuffer is not null)
            {
                ArrayPool<byte>.Shared.Return(minimumBuffer);
            }
        }
    }

    #endregion WriteString

    #region StringReadOperation

    /// <summary>
    /// Creates a state machine which reads a string from the stream.
    /// </summary>
    private class StringReadOperation : IDisposable
    {
        // Determines the maximum size of a buffer (in characters) available for decoding bytes into characters.
        private const int MaxReadStringBufferChars = 1024 * 8;

        // Typically a decoded character array will be the same or shorter than the encoded byte array,
        // however this may not be true if the caller provided a fallback buffer which yields more characters
        // than bytes. There is no way to know this without decoding the bytes, so we must assume the worse case.
        // A number of characters is reserved on top of read string character buffer to protect against this scenario.
        // The reservation size is computed by dividing the target buffer size by ReadStringFallbackExcessCharsDivisor.
        // For example, if ReadStringFallbackExcessCharsDivisor is 4, the buffer size is increased by 25%.
        private const int ReadStringFallbackExcessCharsDivisor = 4;

        // The stream to read from.
        private readonly MarshalStream _stream;

        // The decoder to use for decoding the bytes of the stream into characters.
        private readonly Decoder _decoder;

        // Holds the decoded string.
        private readonly StringBuilder _sb = new();

        // A character buffer which is the destination for each decode operations.
        private char[]? _charBuffer;

        // The maximum number of bytes to read from the stream.
        private readonly int _maxBytesToRead;

        // The character buffer may grow in an extremely unlikely fallback scenario, however, the input bytes may never grow.
        private readonly int _maxBytesToProcessEachPass;

        // Holds the total number of bytes currently read from the stream.
        private int _totalBytesRead;

        // Defines the behavior of the operation when a null terminator is encountered.
        private readonly MarshalStreamNullTerminatorBehavior _nullTerminatorBehavior;

        // The size of the null terminator for the decoder.
        private readonly int _nullTerminatorSize;

        // A flag indicating if a null terminator was found.
        private bool _nullTerminatorFound;

        // Tracks the number of zeros currently found at the end of the byte buffer while probing for a null terminator.
        private int _zeroCount;

        // Used to track known trailing zeros when the null terminator behavior is to trim trailing zeros.
        private int _trailingZeros;

        /// <summary>
        /// Initializes a new instance of the <see cref="Phaeyz.Marshalling.MarshalStreamReadStringResult"/> class.
        /// </summary>
        /// <param name="stream">
        /// The <see cref="Phaeyz.Marshalling.MarshalStream"/> object to use for the read string operation.
        /// </param>
        /// <param name="decoder">
        /// The <see cref="System.Text.Decoder"/> object to use for decoding the bytes of the stream into characters.
        /// </param>
        /// <param name="maxBytesToRead">
        /// The maximum number of bytes to read from the stream. If -1, then read until the end of the stream.
        /// </param>
        /// <param name="nullTerminatorBehavior">
        /// Defines the behavior when null terminators are encountered.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The maximum number of bytes to read is less than zero.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// The stream is not readable.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The stream is disposed.
        /// </exception>
        public StringReadOperation(
            MarshalStream stream,
            Decoder decoder,
            int maxBytesToRead,
            MarshalStreamNullTerminatorBehavior nullTerminatorBehavior)
        {
            stream.VerifyReadableStream();
            stream.FlushWrite();

            ArgumentOutOfRangeException.ThrowIfLessThan(maxBytesToRead, -1);

            _stream = stream;
            _decoder = decoder;
            _maxBytesToRead = maxBytesToRead;
            _nullTerminatorBehavior = nullTerminatorBehavior;

            // Short circuit if we are reading nothing.
            if (maxBytesToRead == 0)
            {
                return;
            }

            // This may cause some allocations, unfortunately, but there is no other strategy here.
            decoder.Reset();

            // Compute the size of the null terminator for the decoder.
            _nullTerminatorSize = _nullTerminatorBehavior == MarshalStreamNullTerminatorBehavior.Stop ? ComputeNullTerminatorSize() : 0;

            // We will create a buffer of one char for every encoded byte -- this is the worse case scenario.
            int maxCharacterBufferSize = Math.Min(stream._fixedBuffer?.Length ?? stream._buffer!.Length, MaxReadStringBufferChars);

            // The character buffer may grow in an extremely unlikely fallback scenario, however, the input bytes
            // should never grow.
            _maxBytesToProcessEachPass = maxBytesToRead == -1 ? maxCharacterBufferSize : Math.Min(maxBytesToRead, maxCharacterBufferSize);

            // Get a buffer with an excess reservation.
            _charBuffer = ArrayPool<char>.Shared.Rent(ComputeIncreasedBufferWithReservation(_maxBytesToProcessEachPass));
        }

        /// <summary>
        /// Gets or sets a value indicating if the read operation has reached the end of the stream.
        /// This value must be set to <c>true</c> if the end of the stream was reached while adding more bytes to the buffer.
        /// </summary>
        public bool EndOfStreamReached { get; set; }

        /// <summary>
        /// Gets a value if the read operation is complete. If <c>true</c> the caller should continue adding
        /// bytes to the buffer and processing it. If <c>false</c> the caller should end processing and create a result.
        /// </summary>
        public bool ContinueProcessing =>
            !_nullTerminatorFound && !EndOfStreamReached && _charBuffer is not null &&
            (_maxBytesToRead == -1 || _totalBytesRead < _maxBytesToRead);

        /// <summary>
        /// Appends characters to the string builder, tracking trailing zeros if necessary.
        /// </summary>
        /// <param name="startIndex">
        /// The start index within the character buffer.
        /// </param>
        /// <param name="count">
        /// The count of characters to append.
        /// </param>
        private void AppendCharacters(int startIndex, int count)
        {
            if (count == 0)
            {
                return;
            }
            // If null terminators are to be trimmed, track the number of trailing zeros without adding them to the buffer.
            if (_nullTerminatorBehavior == MarshalStreamNullTerminatorBehavior.TrimTrailing)
            {
                // Count the trailing nulls.
                int lastIndex = startIndex + count - 1;
                int lastNonNullIndex = lastIndex;
                for (; lastNonNullIndex >= startIndex && _charBuffer![lastNonNullIndex] == '\0'; lastNonNullIndex--);
                int nullsFound = lastIndex - lastNonNullIndex;
                // If there are non-nulls, append the preexisting known trailing zeros.
                if (count - nullsFound > 0)
                {
                    while (_trailingZeros > 0)
                    {
                        _sb.Append('\0');
                        _trailingZeros--;
                    }
                    _trailingZeros = nullsFound;
                    _sb.Append(_charBuffer, startIndex, count - nullsFound);
                }
                else
                {
                    // All characters are zeros, so just add to the known trailing zeros.
                    _trailingZeros += nullsFound;
                }
            }
            else
            {
                // Normal behavior is to just append the characters.
                _sb.Append(_charBuffer, startIndex, count);
            }
        }

        /// <summary>
        /// Computes a new character buffer size accounting for an excess reserve size.
        /// </summary>
        /// <param name="charBufferSize">
        /// The character buffer size.
        /// </param>
        /// <returns>
        /// A new character buffer size accounting for an excess reserve size.
        /// </returns>
        private static int ComputeIncreasedBufferWithReservation(int charBufferSize) =>
            charBufferSize + Math.Max(
                (int)(charBufferSize / ReadStringFallbackExcessCharsDivisor),
                ReadStringFallbackExcessCharsDivisor);

        /// <summary>
        /// Computes the size of a null terminator for a character encoding for the
        /// <see cref="System.Text.Decoder"/> object.
        /// </summary>
        /// <returns>
        /// The number of bytes of encoded text which make up a null terminator.
        /// </returns>
        private int ComputeNullTerminatorSize()
        {
            for (int i = 1; i <= s_nullTerminatorBytes.Length; i++)
            {
                // There is a Span version of this method, but it is not used here because the default implementations
                // currently do not take advantage of Spans, and an internal buffer copy occurs and the array method is called.
                if (_decoder.GetCharCount(s_nullTerminatorBytes, 0, i, true) == 1)
                {
                    return i;
                }
            }
            // Should never happen...
            throw new InvalidOperationException("Failed to compute the null terminator size.");
        }

        /// <summary>
        /// A utility to count the number of trailing zeros in a byte span.
        /// </summary>
        /// <param name="span">
        /// The byte span to count trailing zeros.
        /// </param>
        /// <returns>
        /// The number of trailing zeros in the byte span.
        /// </returns>
        private static int CountTrailingZeros(ReadOnlySpan<byte> span)
        {
            int count = 0;
            for (int i = span.Length - 1; i >= 0 && span[i] == 0; i--, count++) ;
            return count;
        }

        /// <summary>
        /// Creates a result based on the state of the read string operation. Calling this method
        /// will prevent further processing.
        /// </summary>
        /// <returns>
        /// A <see cref="Phaeyz.Marshalling.MarshalStreamReadStringResult"/> object.
        /// </returns>
        public MarshalStreamReadStringResult GetResult()
        {
            // Tell the decoder to flush
            if (_charBuffer is not null)
            {
                int charsFlushed = _decoder.GetChars(s_nullTerminatorBytes, 0, 0, _charBuffer, 0, true);
                if (charsFlushed > 0 && !_nullTerminatorFound)
                {
                    AppendCharacters(0, charsFlushed);
                }

                Dispose();
            }

            return new(_sb.ToString(), _totalBytesRead, _nullTerminatorFound, EndOfStreamReached);
        }

        /// <summary>
        /// Returns the allocated character buffer back to it's array pool.
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (_charBuffer is not null)
                {
                    ArrayPool<char>.Shared.Return(_charBuffer);
                }
            }
            finally
            {
                _charBuffer = null;
            }
        }

        /// <summary>
        /// Processes the current stream's read buffer by converting bytes to characters.
        /// </summary>
        /// <exception cref="System.Text.DecoderFallbackException">
        /// The <see cref="System.Text.Decoder"/>'s <see cref="System.Text.Decoder.Fallback"/> property (or the owning
        /// <see cref="System.Text.Encoding"/>'s <see cref="System.Text.Encoding.DecoderFallback"/> property) is set
        /// to <see cref="System.Text.DecoderExceptionFallback"/> and a fallback occurred. When this happens, the
        /// stream's new position is undefined.
        /// </exception>
        public void ProcessBuffer()
        {
            if (!ContinueProcessing)
            {
                return;
            }

            // Compute the maximum number of bytes to read this attempt.
            int byteCountToProcess = Math.Min(_stream.BufferedReadableByteCount, _maxBytesToProcessEachPass);
            if (_maxBytesToRead != -1)
            {
                byteCountToProcess = Math.Min(_maxBytesToRead - _totalBytesRead, byteCountToProcess);
            }

            // Scan for null terminator if desired.
            if (_nullTerminatorBehavior == MarshalStreamNullTerminatorBehavior.Stop && byteCountToProcess > 0)
            {
                // For detection of null terminator there are three options when all that we have is a Decoder:
                // 1) Use the decoder to decode the entire buffer into characters, and then scan the characters for
                //    the null terminator. This is not efficient because it requires decoding the entire buffer.
                //    But it also presents a more challenging problem in that we don't know where in the buffer the
                //    null terminator was found.
                // 2) Process bytes one byte at a time. This takes advantage of the fact that GetChars from the Decoder
                //    keeps track of the state of previous bytes in the current code unit. When a character is complete
                //    the character may be examined for the null terminator. This is by far the simplest approach, however
                //    internally the Decoders may do allocations each time GetChars is called, making this approach
                //    inefficient. Furthermore the existing Decoders are optimized to handle a bulk of bytes at a time.
                //    If the Decoders were optimized for one byte at a time, this would be the best approach.
                // 3) Detect the size of the null terminator for the decoder (easily done using GetCharCount), and
                //    detect the null terminator in the buffer before decoding. This is challenging with the following issues:
                //    a) Some encodings may end with zeros in subsequent code units. For example, UTF16 or UTF32.
                //       For example, this is the word "hello" in UTF16:
                //           0  1  2  3  4  5  6  7  8  9  10 11
                //           68 00 65 00 6C 00 6C 00 6F 00 00 00
                //       If we have knowledge of the encoding, we know in UTF16's case the minimum code unit size is two bytes,
                //       and we could keep track of the first byte and skip every other byte looking for the null terminator.
                //       Unfortunately we cannot assume this as the Decoder may be for any other decoding. So if we just
                //       searched for the null terminator, we would first detect it at index 9 where it is actually at index 10.
                //    b) The null terminator may be split across two buffer reads. Therefore logic must be introduced to
                //       check for the right amount of zeros at either the beginning middle, or end of the buffer. Furthermore
                //       the logic must take into account the provided buffer may be very small as well. We want to do this
                //       as efficiently as possible.
                // The strategy chosen is the third. This is the technique of how to achieve it:
                // 1) Detect the size of the null terminator.
                // 2) Each buffer read, attempt to look for the null terminator.
                //    a) If a sufficient zero sequence is found, only process one less than the null terminator size before
                //       processing one byte at a time. This is to account for the fact some of the zeros may be part of
                //       a preceding code unit. This technique takes advantage of the fact that GetChars from the Decoder
                //       keeps track of the state, and when processing one byte at a time while we have hit the number of
                //       zeros in a null terminator allows us to check for each output of GetChars. While we continue to
                //       get zero bytes in this state, when GetChars yields a null terminator we know we have reached the
                //       end of the string.
                //    b) If a sufficient zero sequence is not found, check for zeros at the end of the buffer which may
                //       indicate a partial null terminator. Keep track of the number of zeros, and when processing the
                //       next buffer read, check the start of the buffer if the zeros complete to a null terminator size.
                //       If it does, we can execute step 'a' above.

                // First, if this is a continuation, see if the start of the buffer is the null terminator.
                if (_zeroCount > 0)
                {
                    // Try to look for the rest of zeros for the null terminator (if the buffer permits).
                    // But for reasons listed above, we may be more zeros than the size of the null terminator,
                    // so we must look at zeros one byte at a time until we find the true null terminator.
                    int zeroSearchSize = Math.Clamp(_nullTerminatorSize - _zeroCount, 1, byteCountToProcess);
                    // Does the buffer start with these zeros?
                    if (_stream.BufferedReadableBytes.Span.StartsWith(s_nullTerminatorBytes.AsSpan(0, zeroSearchSize)))
                    {
                        // The buffer started with the requested zeros. But due to buffer size it still may not be
                        // the full null terminator. The number of bytes we want to process will always be one less
                        // than the size of the null terminator, but always at least one so we can process one
                        // byte at a time when we have discovered zeros at the null terminator size.
                        int zeroProcessSize = Math.Max(
                            _zeroCount + zeroSearchSize < _nullTerminatorSize ? zeroSearchSize : zeroSearchSize - 1,
                            1);
                        byteCountToProcess = zeroProcessSize;
                        _zeroCount += zeroProcessSize;
                    }
                    else
                    {
                        // If the buffer does not start with the zeros, it means we never found the true null terminator.
                        _zeroCount = 0;
                    }
                }
                // Otherwise, scan the whole buffer for the null terminator.
                bool encounteredPotentialNewNullTerminator = false;
                if (_zeroCount == 0)
                {
                    // Try to find the null terminator anywhere in the buffer. If the buffer is too small, it
                    // may be less than the size of the null terminator (size of the readable buffer).
                    int zeroSearchSize = Math.Clamp(_nullTerminatorSize, 1, byteCountToProcess);
                    int zeroOffset = _stream.BufferedReadableBytes[..byteCountToProcess]
                        .Span
                        .IndexOf(s_nullTerminatorBytes.AsSpan(0, zeroSearchSize));
                    if (zeroOffset != -1)
                    {
                        // Process the zeros we found for the null terminator up to one less than the null terminator.
                        // We begin to process one byte at a time after that.
                        int zeroProcessSize = zeroSearchSize < _nullTerminatorSize ? zeroSearchSize : zeroSearchSize - 1;
                        // It is possible to end up with an empty process size if either 1) the buffer is small enough
                        // where it only contains a partial null terminator or 2) this is a single byte code unit encoding
                        // and the null terminator appears at the beginning of the buffer.
                        if (zeroOffset + zeroProcessSize == 0)
                        {
                            zeroProcessSize = 1;
                        }
                        // Set the number of bytes to process.
                        byteCountToProcess = zeroOffset + zeroProcessSize;
                        _zeroCount = zeroProcessSize;
                        // This is important for one byte code units, as zeroCount may be zero until the next read.
                        encounteredPotentialNewNullTerminator = true;
                    }
                }
                // If a potential null terminator still wasn't found, it is possible only part of it is in the buffer.
                // Keep track of the number of zeros at the end of the buffer.
                if (!encounteredPotentialNewNullTerminator && _zeroCount == 0)
                {
                    _zeroCount = CountTrailingZeros(_stream.BufferedReadableBytes[..byteCountToProcess].Span);
                }
            }

            // Decode the bytes to characters.
            if (byteCountToProcess > 0)
            {
                // Loop because the character buffer may need to grow (very unlikely for this to happen, though).
                int charsDecoded;
                bool processedBytes = false;
                while (true)
                {
                    try
                    {
                        // Favor the byte array version because the default underlying implementation of the Span method creates
                        // a new buffer and does a buffer copy and calls the byte array version anyway.
                        if (MemoryMarshal.TryGetArray(_stream.BufferedReadableBytes, out ArraySegment<byte> segment) && segment.Array is not null)
                        {
                            // Only process bytes once!
                            if (!processedBytes)
                            {
                                MarshalStream.Process(_stream._readProcessors, segment.Array, segment.Offset, byteCountToProcess);
                                processedBytes = true;
                            }
                            charsDecoded = _decoder.GetChars(segment.Array, segment.Offset, byteCountToProcess, _charBuffer!, 0, false);
                        }
                        else
                        {
                            ReadOnlySpan<byte> bytes = _stream.BufferedReadableBytes[..byteCountToProcess].Span;
                            // Only process bytes once!
                            if (!processedBytes)
                            {
                                MarshalStream.Process(_stream._readProcessors, bytes);
                                processedBytes = true;
                            }
                            charsDecoded = _decoder.GetChars(bytes, _charBuffer!.AsSpan(), false);
                        }
                        break;
                    }
                    catch (ArgumentException)
                    {
                        // Note that in a fallback situation, especially for single byte code unit encodings, the decoder may
                        // throw an exception if the character buffer is too small. This is an edge case and we provide a
                        // very generous ReadStringFallbackExcessChars number of characters, and this will likely never happen.
                        // This strategy is chosen to prevent having to call GetCharCount every pass. However if an ArgumentException
                        // has been reached, we did encounter this very unlikely scenario. To be super resilient, we will compute the
                        // number of required characters for this buffer, and grow our character array.
                        int charsRequired = (MemoryMarshal.TryGetArray(_stream.BufferedReadableBytes, out ArraySegment<byte> segment) && segment.Array is not null)
                            ? _decoder.GetCharCount(segment.Array, segment.Offset, byteCountToProcess, false)
                            : _decoder.GetCharCount(_stream.BufferedReadableBytes[..byteCountToProcess].Span, false);

                        if (charsRequired <= _charBuffer!.Length)
                        {
                            // Why was this thrown at all then? This should never happen.
                            throw;
                        }

                        // Grow the character buffer.
                        ArrayPool<char>.Shared.Return(_charBuffer!);
                        _charBuffer = null; // Just in case the Rent call fails...
                        _charBuffer = ArrayPool<char>.Shared.Rent(ComputeIncreasedBufferWithReservation(charsRequired));
                    }
                }

                // Advance the read pointer and byte counter by number of bytes processed.
                _stream._currentReadOffset += byteCountToProcess;
                _totalBytesRead += byteCountToProcess;

                // Move the decoded characters to the string builder.
                if (charsDecoded > 0)
                {
                    // If we have detected the number of zeros for a null terminator at the end of the processed
                    // buffer, check to see if a null terminator has been reached.
                    if (_nullTerminatorBehavior == MarshalStreamNullTerminatorBehavior.Stop && _zeroCount >= _nullTerminatorSize)
                    {
                        // If a fallback buffer was used, it is possible for the decoder to return more characters than input bytes.
                        // But we should still stop accumulating characters if we have found the null terminator.
                        for (int charIndex = 0; charIndex < charsDecoded; charIndex++)
                        {
                            if (_charBuffer![charIndex] == '\0')
                            {
                                // A null terminator has been reached, don't add this character to the string builder.
                                // Note that when detecting zeros in this state, we did not read more bytes
                                // beyond this null terminator so it is safe to assume the stream position is correct.
                                _nullTerminatorFound = true;
                                break;
                            }
                            AppendCharacters(charIndex, 1);
                        }
                    }
                    else
                    {
                        AppendCharacters(0, charsDecoded);

                        if (byteCountToProcess == 1)
                        {
                            _zeroCount = 0;
                        }
                    }
                }
            }
        }
    }

    #endregion StringReadOperation

    #region ProcessorRemover

    /// <summary>
    /// A class which can be used to remove a processor using <see cref="System.IDisposable"/>.
    /// </summary>
    /// <param name="processor">
    /// The processor to remove.
    /// </param>
    /// <param name="processors">
    /// The set to remove the processors from.
    /// </param>
    private class ProcessorRemover(
        IMarshalStreamProcessor processor,
        HashSet<IMarshalStreamProcessor> processors) : IDisposable
    {
        private bool _disposed = false;

        public void Dispose()
        {
            try
            {
                if (!_disposed)
                {
                    processors.Remove(processor);
                }
            }
            finally
            {
                _disposed = false;
            }
        }
    }

    #endregion ProcessorRemover
}
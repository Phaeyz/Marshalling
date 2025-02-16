# MarshalStream class

A stream similar to BufferedStream but provides access to the read buffer, making the stream ideal for parsing and scanning.

```csharp
public class MarshalStream : Stream
```

## Public Members

| name | description |
| --- | --- |
| [MarshalStream](MarshalStream/MarshalStream.md)(…) | Initializes a new instance of [`MarshalStream`](./MarshalStream.md) which wraps another underlying stream. (2 constructors) |
| [BufferedReadableByteCount](MarshalStream/BufferedReadableByteCount.md) { get; } | Gets the number of unread bytes in the read buffer. |
| [BufferedReadableBytes](MarshalStream/BufferedReadableBytes.md) { get; } | Provides access to the unread part of the read buffer. |
| override [CanRead](MarshalStream/CanRead.md) { get; } | Returns `true` if the stream may be read from, `false` otherwise. |
| override [CanSeek](MarshalStream/CanSeek.md) { get; } | Returns `true` if the stream may seeked, `false` otherwise. |
| override [CanWrite](MarshalStream/CanWrite.md) { get; } | Returns `true` if the stream may be written to, `false` otherwise. |
| [IsDisposed](MarshalStream/IsDisposed.md) { get; } | Returns `true` if the stream is disposed, `false` otherwise. |
| [IsFixedBuffer](MarshalStream/IsFixedBuffer.md) { get; } | Returns `true` if the stream wraps a fixed buffer; `false` otherwise. |
| override [Length](MarshalStream/Length.md) { get; } | Gets the length of the stream. The stream must wrap a fixed buffer or a seekable stream. |
| override [Position](MarshalStream/Position.md) { get; set; } | Gets the read or write position of the stream. The stream must wrap a fixed buffer or a seekable stream. |
| [TotalBufferSize](MarshalStream/TotalBufferSize.md) { get; } | Gets the size of the fixed buffer or the buffer for the underlying stream. |
| [AddReadProcessor](MarshalStream/AddReadProcessor.md)(…) | Registers a new read processor. |
| [AddWriteProcessor](MarshalStream/AddWriteProcessor.md)(…) | Registers a new write processor. |
| override [CopyTo](MarshalStream/CopyTo.md)(…) | Copies the remainder of the current stream to the specified destination stream. |
| override [CopyToAsync](MarshalStream/CopyToAsync.md)(…) | Copies the remainder of the current stream to the specified destination stream. |
| override [DisposeAsync](MarshalStream/DisposeAsync.md)() | Asynchronously releases the unmanaged resources used by the [`MarshalStream`](./MarshalStream.md). |
| [EnsureByteCountAvailableInBuffer](MarshalStream/EnsureByteCountAvailableInBuffer.md)(…) | Ensures at minimum the specified number of unread bytes is available in the read buffer. |
| [EnsureByteCountAvailableInBufferAsync](MarshalStream/EnsureByteCountAvailableInBufferAsync.md)(…) | Ensures at minimum the specified number of unread bytes is available in the read buffer. |
| override [Flush](MarshalStream/Flush.md)() | Ensures there is no unread bytes in the read buffer by updating the underlying stream's seek pointer, and ensuring all unwritten bytes are persisted to the underlying stream. |
| override [FlushAsync](MarshalStream/FlushAsync.md)(…) | Ensures there is no unread bytes in the read buffer by updating the underlying stream's seek pointer, and ensures all unwritten bytes are persisted to the underlying stream. |
| [IsMatch](MarshalStream/IsMatch.md)(…) | Check to see if the next bytes to be read from the stream match the provided sequence of bytes. |
| [IsMatchAsync](MarshalStream/IsMatchAsync.md)(…) | Check to see if the next bytes to be read from the stream match the provided sequence of bytes. |
| override [Read](MarshalStream/Read.md)(…) | Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read. (2 methods) |
| override [ReadAsync](MarshalStream/ReadAsync.md)(…) | Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read. (2 methods) |
| override [ReadByte](MarshalStream/ReadByte.md)() | Reads a byte from the current stream and advances the position within the stream by one byte. |
| [ReadByteAsync](MarshalStream/ReadByteAsync.md)(…) | Reads a byte from the current stream and advances the position within the stream by one byte. |
| [ReadString](MarshalStream/ReadString.md)(…) | Reads a string from the stream. (2 methods) |
| [ReadStringAsync](MarshalStream/ReadStringAsync.md)(…) | Initializes a new instance of the StringReadOperation class. (2 methods) |
| [RemoveReadProcessor](MarshalStream/RemoveReadProcessor.md)(…) | Unregisters a read processor. |
| [RemoveWriteProcessor](MarshalStream/RemoveWriteProcessor.md)(…) | Unregisters a write processor. |
| [Scan](MarshalStream/Scan.md)(…) | Efficiently reads each byte of a stream until a scanning function instructs to stop reading. (3 methods) |
| [ScanAsync](MarshalStream/ScanAsync.md)(…) | Efficiently reads each byte of a stream until a scanning function instructs to stop reading. (3 methods) |
| override [Seek](MarshalStream/Seek.md)(…) | Sets the position within the current stream. |
| [SeekAsync](MarshalStream/SeekAsync.md)(…) | Sets the position within the current stream. |
| override [SetLength](MarshalStream/SetLength.md)(…) | Sets the length of the current stream. |
| [Skip](MarshalStream/Skip.md)(…) | Reads and discards a sequence of bytes from the current stream and advances the position within the stream by the specified number of bytes. |
| [SkipAsync](MarshalStream/SkipAsync.md)(…) | Reads and discards a sequence of bytes from the current stream and advances the position within the stream by the specified number of bytes. |
| override [Write](MarshalStream/Write.md)(…) | Writes a sequence of bytes to the current stream and advances the current position within the stream by the number of bytes written. (2 methods) |
| override [WriteAsync](MarshalStream/WriteAsync.md)(…) | Writes a sequence of bytes to the current stream and advances the current position within the stream by the number of bytes written. (2 methods) |
| override [WriteByte](MarshalStream/WriteByte.md)(…) | Writes a byte to the current stream and advances the current position within the stream by one byte. |
| [WriteByteAsync](MarshalStream/WriteByteAsync.md)(…) | Writes a byte to the current stream and advances the current position within the stream by one byte. |
| [WriteString](MarshalStream/WriteString.md)(…) | Writes a string to the stream. (2 methods) |
| [WriteStringAsync](MarshalStream/WriteStringAsync.md)(…) | Writes a string to the stream. (4 methods) |
| const [DefaultBufferSize](MarshalStream/DefaultBufferSize.md) | The default buffer size created when wrapping a stream (16KB). |

## Protected Members

| name | description |
| --- | --- |
| override [Dispose](MarshalStream/Dispose.md)(…) | Releases all resources used by the [`MarshalStream`](./MarshalStream.md). |

## Remarks

This class is not thread safe. Like any class which is not thread safe, usage of this class concurrently from multiple threads is not supported and will result in corrupt state.

## See Also

* namespace [Phaeyz.Marshalling](../Phaeyz.Marshalling.md)
* [MarshalStream.cs](https://github.com/Phaeyz/Marshalling/blob/main/Phaeyz.Marshalling/MarshalStream.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Phaeyz.Marshalling.dll -->

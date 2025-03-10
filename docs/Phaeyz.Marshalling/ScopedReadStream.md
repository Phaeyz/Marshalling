# ScopedReadStream class

A stream for wrapping another stream, and limiting the number of bytes which may be read.

```csharp
public class ScopedReadStream : Stream
```

## Public Members

| name | description |
| --- | --- |
| [ScopedReadStream](ScopedReadStream/ScopedReadStream.md)(…) | Initializes a new [`ScopedReadStream`](./ScopedReadStream.md) instance. |
| override [CanRead](ScopedReadStream/CanRead.md) { get; } | Indicates whether the current stream supports reading. Always returns `true>`. |
| override [CanSeek](ScopedReadStream/CanSeek.md) { get; } | Indicates whether the current stream supports seeking. Always returns `false`. |
| override [CanWrite](ScopedReadStream/CanWrite.md) { get; } | Indicates whether the current stream supports writing. Always returns `false`. |
| override [Length](ScopedReadStream/Length.md) { get; } | Gets the length in bytes of the stream. An exception is always thrown. |
| [MaxReadableBytes](ScopedReadStream/MaxReadableBytes.md) { get; } | The max number of bytes allowed to be read from the stream. |
| override [Position](ScopedReadStream/Position.md) { get; set; } | Gets or sets the position within the current stream. An exception is always thrown. |
| [TotalBytesRead](ScopedReadStream/TotalBytesRead.md) { get; } | The total number of bytes read from the stream since the instance was created. |
| override [Flush](ScopedReadStream/Flush.md)() | Clears all buffers for this stream and causes any buffered data to be written to the underlying device. This method is a no-op for [`ScopedReadStream`](./ScopedReadStream.md). |
| override [Read](ScopedReadStream/Read.md)(…) | Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read. |
| override [Seek](ScopedReadStream/Seek.md)(…) | Sets the position within the current stream. An exception is always thrown. |
| override [SetLength](ScopedReadStream/SetLength.md)(…) | Sets the length of the current stream. An exception is always thrown. |
| override [Write](ScopedReadStream/Write.md)(…) | Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written. |

## Protected Members

| name | description |
| --- | --- |
| override [Dispose](ScopedReadStream/Dispose.md)(…) | Closes the stream. |

## Remarks

The stream is not seekable or writable.

## See Also

* namespace [Phaeyz.Marshalling](../Phaeyz.Marshalling.md)
* [ScopedReadStream.cs](https://github.com/Phaeyz/Marshalling/blob/main/Phaeyz.Marshalling/ScopedReadStream.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Phaeyz.Marshalling.dll -->

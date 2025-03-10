# MarshalStream.WriteStringAsync method (1 of 4)

Writes a string to the stream.

```csharp
public ValueTask<int> WriteStringAsync(Encoder encoder, ReadOnlyMemory<char> value, 
    bool writeNullTerminator = false, CancellationToken cancellationToken = default)
```

| parameter | description |
| --- | --- |
| encoder | The Encoder object to use for encoding the characters of the string to bytes onto the stream. |
| value | The string value to write to the stream. |
| writeNullTerminator | If `true` a null terminator will be persisted after the string. The default value is `false`. |
| cancellationToken | A cancellation token which may be used to cancel the operation. |

## Return Value

A task yielding the number of bytes written to the stream.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentNullException | The input string value is `null`. |
| EncoderFallbackException | The *encoder*'s Fallback property (or the owning Encoding's EncoderFallback property) is set to EncoderExceptionFallback and a fallback occurred. When this happens, the stream's new position is undefined. |
| IOException | An I/O error occurred. |
| NotSupportedException | The current stream is not writable, or the read buffer cannot be flushed because the current stream is not seekable. |
| ObjectDisposedException | The stream is disposed. |
| OperationCanceledException | The cancellation token was canceled. |

## See Also

* class [MarshalStream](../MarshalStream.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

---

# MarshalStream.WriteStringAsync method (2 of 4)

Writes a string to the stream.

```csharp
public ValueTask<int> WriteStringAsync(Encoder encoder, string value, 
    bool writeNullTerminator = false, CancellationToken cancellationToken = default)
```

| parameter | description |
| --- | --- |
| encoder | The Encoder object to use for encoding the characters of the string to bytes onto the stream. |
| value | The string value to write to the stream. |
| writeNullTerminator | If `true` a null terminator will be persisted after the string. The default value is `false`. |
| cancellationToken | A cancellation token which may be used to cancel the operation. |

## Return Value

A task yielding the number of bytes written to the stream.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentNullException | The input string value is `null`. |
| EncoderFallbackException | The *encoder*'s Fallback property (or the owning Encoding's EncoderFallback property) is set to EncoderExceptionFallback and a fallback occurred. When this happens, the stream's new position is undefined. |
| IOException | An I/O error occurred. |
| NotSupportedException | The current stream is not writable, or the read buffer cannot be flushed because the current stream is not seekable. |
| ObjectDisposedException | The stream is disposed. |
| OperationCanceledException | The cancellation token was canceled. |

## See Also

* class [MarshalStream](../MarshalStream.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

---

# MarshalStream.WriteStringAsync method (3 of 4)

Writes a string to the stream.

```csharp
public ValueTask<int> WriteStringAsync(Encoding encoding, ReadOnlyMemory<char> value, 
    bool writeNullTerminator = false, CancellationToken cancellationToken = default)
```

| parameter | description |
| --- | --- |
| encoding | The Encoding object to use for encoding the characters of the string to bytes onto the stream. |
| value | The string value to write to the stream. |
| writeNullTerminator | If `true` a null terminator will be persisted after the string. The default value is `false`. |
| cancellationToken | A cancellation token which may be used to cancel the operation. |

## Return Value

A task yielding the number of bytes written to the stream.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentNullException | The input string value is `null`. |
| EncoderFallbackException | The *encoding*'s EncoderFallback property is set to EncoderExceptionFallback and a fallback occurred. When this happens, the stream's new position is undefined. |
| IOException | An I/O error occurred. |
| NotSupportedException | The current stream is not writable, or the read buffer cannot be flushed because the current stream is not seekable. |
| ObjectDisposedException | The stream is disposed. |
| OperationCanceledException | The cancellation token was canceled. |

## See Also

* class [MarshalStream](../MarshalStream.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

---

# MarshalStream.WriteStringAsync method (4 of 4)

Writes a string to the stream.

```csharp
public ValueTask<int> WriteStringAsync(Encoding encoding, string value, 
    bool writeNullTerminator = false, CancellationToken cancellationToken = default)
```

| parameter | description |
| --- | --- |
| encoding | The Encoding object to use for encoding the characters of the string to bytes onto the stream. |
| value | The string value to write to the stream. |
| writeNullTerminator | If `true` a null terminator will be persisted after the string. The default value is `false`. |
| cancellationToken | A cancellation token which may be used to cancel the operation. |

## Return Value

A task yielding the number of bytes written to the stream.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentNullException | The input string value is `null`. |
| EncoderFallbackException | The *encoding*'s EncoderFallback property is set to EncoderExceptionFallback and a fallback occurred. When this happens, the stream's new position is undefined. |
| IOException | An I/O error occurred. |
| NotSupportedException | The current stream is not writable, or the read buffer cannot be flushed because the current stream is not seekable. |
| ObjectDisposedException | The stream is disposed. |
| OperationCanceledException | The cancellation token was canceled. |

## See Also

* class [MarshalStream](../MarshalStream.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

<!-- DO NOT EDIT: generated by xmldocmd for Phaeyz.Marshalling.dll -->

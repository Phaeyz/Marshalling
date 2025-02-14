# StreamExtensions.WriteUInt8Async method

Writes an unsigned 8-bit value to the stream.

```csharp
public static Task WriteUInt8Async(this Stream @this, byte value, 
    CancellationToken cancellationToken = default)
```

| parameter | description |
| --- | --- |
| this | The stream to write to. |
| value | The value to write. |
| cancellationToken | A cancellation token which may be used to cancel the request. |

## Remarks

This is not named `WriteByteAsync` to avoid confusion with the existing `WriteByte` method contract on Stream.

## See Also

* class [StreamExtensions](../StreamExtensions.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

<!-- DO NOT EDIT: generated by xmldocmd for Phaeyz.Marshalling.dll -->

# StreamExtensions.WriteInt16Async method

Writes a signed 16-bit value to the stream.

```csharp
public static Task WriteInt16Async(this Stream @this, short value, 
    ByteConverter? byteConverter = null, CancellationToken cancellationToken = default)
```

| parameter | description |
| --- | --- |
| this | The stream to write to. |
| value | The value to write. |
| byteConverter | The byte converter to use when writing the value. If `null`, the system's endianness is used. |
| cancellationToken | A cancellation token which may be used to cancel the request. |

## See Also

* class [ByteConverter](../ByteConverter.md)
* class [StreamExtensions](../StreamExtensions.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

<!-- DO NOT EDIT: generated by xmldocmd for Phaeyz.Marshalling.dll -->

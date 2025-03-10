# StreamExtensions.WriteUInt16 method

Writes an unsigned 16-bit value to the stream.

```csharp
public static void WriteUInt16(this Stream @this, ushort value, ByteConverter? byteConverter = null)
```

| parameter | description |
| --- | --- |
| this | The stream to write to. |
| value | The value to write. |
| byteConverter | The byte converter to use when writing the value. If `null`, the system's endianness is used. |

## See Also

* class [ByteConverter](../ByteConverter.md)
* class [StreamExtensions](../StreamExtensions.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

<!-- DO NOT EDIT: generated by xmldocmd for Phaeyz.Marshalling.dll -->

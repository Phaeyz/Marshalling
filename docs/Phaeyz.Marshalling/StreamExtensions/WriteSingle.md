# StreamExtensions.WriteSingle method

Writes a 32-bit floating-point value to the stream.

```csharp
public static void WriteSingle(this Stream @this, float value, ByteConverter? byteConverter = null)
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

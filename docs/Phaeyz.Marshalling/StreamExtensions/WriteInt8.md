# StreamExtensions.WriteInt8 method

Writes a signed 8-bit value to the stream.

```csharp
public static void WriteInt8(this Stream @this, sbyte value)
```

| parameter | description |
| --- | --- |
| this | The stream to write to. |
| value | The value to write. |

## Remarks

This is not named `WriteSByte` to avoid confusion with the existing `WriteByte` method contract on Stream.

## See Also

* class [StreamExtensions](../StreamExtensions.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

<!-- DO NOT EDIT: generated by xmldocmd for Phaeyz.Marshalling.dll -->

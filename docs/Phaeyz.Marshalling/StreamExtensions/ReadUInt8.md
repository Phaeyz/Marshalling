# StreamExtensions.ReadUInt8 method

Reads an unsigned 8-bit value from the stream.

```csharp
public static byte ReadUInt8(this Stream @this)
```

| parameter | description |
| --- | --- |
| this | The stream to read from. |

## Return Value

An unsigned 8-bit value read from the stream.

## Exceptions

| exception | condition |
| --- | --- |
| EndOfStreamException | The end of the stream is reached before reading the required number of bytes. |

## Remarks

This is not named `ReadByte` to avoid confusion with the existing `ReadByte` method contract on Stream.

## See Also

* class [StreamExtensions](../StreamExtensions.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

<!-- DO NOT EDIT: generated by xmldocmd for Phaeyz.Marshalling.dll -->

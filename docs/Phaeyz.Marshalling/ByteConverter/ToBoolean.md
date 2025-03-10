# ByteConverter.ToBoolean method

Reads the zero offset of a span as a boolean (a one-byte value where zero is `false`, non-zero is `true`).

```csharp
public bool ToBoolean(ReadOnlySpan<byte> value)
```

| parameter | description |
| --- | --- |
| value | The span which contains the value at the zero offset. |

## Return Value

A boolean value read from the span.

## See Also

* class [ByteConverter](../ByteConverter.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

<!-- DO NOT EDIT: generated by xmldocmd for Phaeyz.Marshalling.dll -->

# MarshalStream.CopyTo method

Copies the remainder of the current stream to the specified destination stream.

```csharp
public override void CopyTo(Stream destination, int bufferSize)
```

| parameter | description |
| --- | --- |
| destination | The destination stream. |
| bufferSize | The minimum buffer size to use during the copy. If the internal buffer is larger it will be used instead. This parameter is not used when wrapping a fixed buffer. |

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentNullException | *destination* is `null`. |
| ArgumentOutOfRangeException | *bufferSize* is negative. |
| IOException | An I/O error occurred. |
| NotSupportedException | The current stream is not readable, or *destination* is not writable. |
| ObjectDisposedException | The stream is disposed. |

## See Also

* class [MarshalStream](../MarshalStream.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

<!-- DO NOT EDIT: generated by xmldocmd for Phaeyz.Marshalling.dll -->

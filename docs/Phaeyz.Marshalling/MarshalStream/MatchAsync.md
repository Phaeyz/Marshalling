# MarshalStream.MatchAsync method

Check to see if the next bytes to be read from the stream match the provided sequence of bytes.

```csharp
public Task<MarshalStreamMatchResult> MatchAsync(ReadOnlyMemory<byte> matchBytes, 
    CancellationToken cancellationToken = default)
```

| parameter | description |
| --- | --- |
| matchBytes | The sequence of bytes to compare against the next bytes to be read from the stream. |
| cancellationToken | A cancellation token which may be used to cancel the operation. |

## Return Value

A [`MarshalStreamMatchResult`](../MarshalStreamMatchResult.md) instance which describes the result of the match operation.

## Exceptions

| exception | condition |
| --- | --- |
| IOException | An I/O error occurred. |
| NotSupportedException | The stream is not readable. |
| ObjectDisposedException | The stream is disposed. |
| OperationCanceledException | The cancellation token was canceled. |

## See Also

* struct [MarshalStreamMatchResult](../MarshalStreamMatchResult.md)
* class [MarshalStream](../MarshalStream.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

<!-- DO NOT EDIT: generated by xmldocmd for Phaeyz.Marshalling.dll -->

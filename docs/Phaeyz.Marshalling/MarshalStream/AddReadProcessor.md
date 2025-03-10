# MarshalStream.AddReadProcessor method

Registers a new read processor.

```csharp
public IDisposable AddReadProcessor(IMarshalStreamProcessor processor)
```

| parameter | description |
| --- | --- |
| processor | The read processor to register. |

## Return Value

A IDisposable which may be used to remove the read processor.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentException | The read processor is already registered. |

## Remarks

When the read processor is registered, the stream will become unseekable and read-only. Multiple read processors may be concurrently registered.

## See Also

* interface [IMarshalStreamProcessor](../IMarshalStreamProcessor.md)
* class [MarshalStream](../MarshalStream.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

<!-- DO NOT EDIT: generated by xmldocmd for Phaeyz.Marshalling.dll -->

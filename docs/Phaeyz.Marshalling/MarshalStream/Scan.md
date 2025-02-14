# MarshalStream.Scan method (1 of 3)

Efficiently reads each byte of a stream until a scanning function instructs to stop reading.

```csharp
public MarshalStreamScanResult Scan(int minBytesNeededForScan, long maxBytesToRead, 
    Func<ReadOnlyMemory<byte>, int> scanFunc)
```

| parameter | description |
| --- | --- |
| minBytesNeededForScan | The minimum number of bytes to pass to the scanning function. See remarks regarding the relationship between *minBytesNeededForScan* and *maxBytesToRead*. |
| maxBytesToRead | The maximum number of bytes to read. This may be `-1` to allow reading the entire stream. See remarks regarding the relationship between *minBytesNeededForScan* and *maxBytesToRead*. |
| scanFunc | A function which is called for each read byte. The function must look at the bytes at the current position and determine if scanning should be stopped. If scanning should be stopped, the function must return 0. Otherwise the function must return the number of bytes processed, which also indicates the number of bytes to skip before the next call of the scan function. |

## Return Value

A [`MarshalStreamScanResult`](../MarshalStreamScanResult.md) object.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentNullException | The *scanFunc* is `null`. |
| ArgumentOutOfRangeException | *minBytesNeededForScan* is zero or less, *minBytesNeededForScan* is greater than the total buffer size, or *maxBytesToRead* is less than `-1`. |
| InvalidOperationException | The scan function returned a negative value, or the scan function returned more bytes than it was provided. |
| IOException | An I/O error occurred. |
| NotSupportedException | The stream is not readable. |
| ObjectDisposedException | The stream is disposed. |

## Remarks

If *minBytesNeededForScan* is more than `1`, *maxBytesToRead* can never be read because reading the minimum of bytes needed for scan may cause us to read beyond the maximum bytes to read.  For performance reasons, the scan function should scan the entire memory buffer provided to it until a match is found, instead of scanning only the top of the buffer before returning. While both strategies work, scanning the entire memory buffer is far more efficient.

## See Also

* struct [MarshalStreamScanResult](../MarshalStreamScanResult.md)
* class [MarshalStream](../MarshalStream.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

---

# MarshalStream.Scan method (2 of 3)

Efficiently reads each byte of a stream until a scanning function instructs to stop reading.

```csharp
public MarshalStreamScanResult Scan(Span<byte> destinationBuffer, int minBytesNeededForScan, 
    Func<ReadOnlyMemory<byte>, int> scanFunc)
```

| parameter | description |
| --- | --- |
| destinationBuffer | Each read byte which did not match the scan function is copied to this buffer. The maximum bytes to read is defined by the length of this buffer. |
| minBytesNeededForScan | The minimum number of bytes to pass to the scanning function. |
| scanFunc | A function which is called for each read byte. The function must look at the bytes at the current position and determine if scanning should be stopped. If scanning should be stopped, the function must return 0. Otherwise the function must return the number of bytes processed, which also indicates the number of bytes to skip before the next call of the scan function. |

## Return Value

A [`MarshalStreamScanResult`](../MarshalStreamScanResult.md) object.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentNullException | The *scanFunc* is `null`. |
| ArgumentOutOfRangeException | *minBytesNeededForScan* is zero or less, or *minBytesNeededForScan* is greater than the total buffer size. |
| InvalidOperationException | The scan function returned a negative value, or the scan function returned more bytes than it was provided. |
| IOException | An I/O error occurred. |
| NotSupportedException | The stream is not readable. |
| ObjectDisposedException | The stream is disposed. |

## Remarks

For performance reasons, the scan function should scan the entire memory buffer provided to it until a match is found, instead of scanning only the top of the buffer before returning. While both strategies work, scanning the entire memory buffer is far more efficient.

## See Also

* struct [MarshalStreamScanResult](../MarshalStreamScanResult.md)
* class [MarshalStream](../MarshalStream.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

---

# MarshalStream.Scan method (3 of 3)

Efficiently reads each byte of a stream until a scanning function instructs to stop reading.

```csharp
public MarshalStreamScanResult Scan(Stream destinationStream, int minBytesNeededForScan, 
    long maxBytesToRead, Func<ReadOnlyMemory<byte>, int> scanFunc)
```

| parameter | description |
| --- | --- |
| destinationStream | Optionally, each read byte which did not match the scan function is written to this stream. Specify `null` to not store read bytes. |
| minBytesNeededForScan | The minimum number of bytes to pass to the scanning function. See remarks regarding the relationship between *minBytesNeededForScan* and *maxBytesToRead*. |
| maxBytesToRead | The maximum number of bytes to read. This may be `-1` to allow reading the entire stream. See remarks regarding the relationship between *minBytesNeededForScan* and *maxBytesToRead*. |
| scanFunc | A function which is called for each read byte. The function must look at the bytes at the current position and determine if scanning should be stopped. If scanning should be stopped, the function must return 0. Otherwise the function must return the number of bytes processed, which also indicates the number of bytes to skip before the next call of the scan function. |

## Return Value

A [`MarshalStreamScanResult`](../MarshalStreamScanResult.md) object.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentException | The *destinationStream* is not writable. |
| ArgumentNullException | The *scanFunc* is `null`. |
| ArgumentOutOfRangeException | *minBytesNeededForScan* is zero or less, *minBytesNeededForScan* is greater than the total buffer size, or *maxBytesToRead* is less than `-1`. |
| InvalidOperationException | The scan function returned a negative value, or the scan function returned more bytes than it was provided. |
| IOException | An I/O error occurred. |
| NotSupportedException | The stream is not readable. |
| ObjectDisposedException | The stream is disposed. |

## Remarks

If *minBytesNeededForScan* is more than `1`, *maxBytesToRead* can never be read because reading the minimum of bytes needed for scan may cause us to read beyond the maximum bytes to read.  For performance reasons, the scan function should scan the entire memory buffer provided to it until a match is found, instead of scanning only the top of the buffer before returning. While both strategies work, scanning the entire memory buffer is far more efficient.

## See Also

* struct [MarshalStreamScanResult](../MarshalStreamScanResult.md)
* class [MarshalStream](../MarshalStream.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

<!-- DO NOT EDIT: generated by xmldocmd for Phaeyz.Marshalling.dll -->

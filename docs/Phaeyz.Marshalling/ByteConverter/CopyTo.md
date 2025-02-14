# ByteConverter.CopyTo method (1 of 21)

Copies a span of bool values to a target byte sequence.

```csharp
public int CopyTo(ReadOnlySpan<bool> source, Span<byte> dest, int? count = null)
```

| parameter | description |
| --- | --- |
| source | The source span of bool values to copy. |
| dest | The target byte sequence. |
| count | The number of elements to copy. If null the entire source is copied. |

## Return Value

The number of elements copied to the target byte sequence.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentOutOfRangeException | Either source length is less than the count, or the destination length is less than the count. |

## See Also

* class [ByteConverter](../ByteConverter.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

---

# ByteConverter.CopyTo method (2 of 21)

Copies a byte sequence to a target span of bool values.

```csharp
public int CopyTo(ReadOnlySpan<byte> source, Span<bool> dest, int? count = null)
```

| parameter | description |
| --- | --- |
| source | The source byte sequence to copy. |
| dest | The target span of bool values. |
| count | The number of elements to copy. If null the entire source is copied. |

## Return Value

The number of elements copied to the target span.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentOutOfRangeException | Either source length is less than the count, or the destination length is less than the count. |

## See Also

* class [ByteConverter](../ByteConverter.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

---

# ByteConverter.CopyTo method (3 of 21)

Copies a byte sequence to a target byte sequence.

```csharp
public int CopyTo(ReadOnlySpan<byte> source, Span<byte> dest, int? count = null)
```

| parameter | description |
| --- | --- |
| source | The source byte sequence to copy. |
| dest | The target byte sequence. |
| count | The number of elements to copy. If null the entire source is copied. |

## Return Value

The number of elements copied to the target span.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentOutOfRangeException | Either source length is less than the count, or the destination length is less than the count. |

## See Also

* class [ByteConverter](../ByteConverter.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

---

# ByteConverter.CopyTo method (4 of 21)

Copies a byte sequence to a target span of 64-bit floating-point values.

```csharp
public int CopyTo(ReadOnlySpan<byte> source, Span<double> dest, int? count = null)
```

| parameter | description |
| --- | --- |
| source | The source byte sequence to copy. |
| dest | The target span of 64-bit floating-point values. |
| count | The number of elements to copy. If null the entire source is copied. |

## Return Value

The number of elements copied to the target span.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentOutOfRangeException | Either source length is less than the count, or the destination length is less than the count. |

## See Also

* class [ByteConverter](../ByteConverter.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

---

# ByteConverter.CopyTo method (5 of 21)

Copies a byte sequence to a target span of 32-bit floating-point values.

```csharp
public int CopyTo(ReadOnlySpan<byte> source, Span<float> dest, int? count = null)
```

| parameter | description |
| --- | --- |
| source | The source byte sequence to copy. |
| dest | The target span of 32-bit floating-point values. |
| count | The number of elements to copy. If null the entire source is copied. |

## Return Value

The number of elements copied to the target span.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentOutOfRangeException | Either source length is less than the count, or the destination length is less than the count. |

## See Also

* class [ByteConverter](../ByteConverter.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

---

# ByteConverter.CopyTo method (6 of 21)

Copies a byte sequence to a target span of 32-bit signed values.

```csharp
public int CopyTo(ReadOnlySpan<byte> source, Span<int> dest, int? count = null)
```

| parameter | description |
| --- | --- |
| source | The source byte sequence to copy. |
| dest | The target span of 32-bit signed values. |
| count | The number of elements to copy. If null the entire source is copied. |

## Return Value

The number of elements copied to the target span.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentOutOfRangeException | Either source length is less than the count, or the destination length is less than the count. |

## See Also

* class [ByteConverter](../ByteConverter.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

---

# ByteConverter.CopyTo method (7 of 21)

Copies a byte sequence to a target span of 64-bit signed values.

```csharp
public int CopyTo(ReadOnlySpan<byte> source, Span<long> dest, int? count = null)
```

| parameter | description |
| --- | --- |
| source | The source byte sequence to copy. |
| dest | The target span of 64-bit signed values. |
| count | The number of elements to copy. If null the entire source is copied. |

## Return Value

The number of elements copied to the target span.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentOutOfRangeException | Either source length is less than the count, or the destination length is less than the count. |

## See Also

* class [ByteConverter](../ByteConverter.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

---

# ByteConverter.CopyTo method (8 of 21)

Copies a byte sequence to a target span of 8-bit signed values.

```csharp
public int CopyTo(ReadOnlySpan<byte> source, Span<sbyte> dest, int? count = null)
```

| parameter | description |
| --- | --- |
| source | The source byte sequence to copy. |
| dest | The target span of 8-bit signed values. |
| count | The number of elements to copy. If null the entire source is copied. |

## Return Value

The number of elements copied to the target span.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentOutOfRangeException | Either source length is less than the count, or the destination length is less than the count. |

## See Also

* class [ByteConverter](../ByteConverter.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

---

# ByteConverter.CopyTo method (9 of 21)

Copies a byte sequence to a target span of 16-bit signed values.

```csharp
public int CopyTo(ReadOnlySpan<byte> source, Span<short> dest, int? count = null)
```

| parameter | description |
| --- | --- |
| source | The source byte sequence to copy. |
| dest | The target span of 16-bit signed values. |
| count | The number of elements to copy. If null the entire source is copied. |

## Return Value

The number of elements copied to the target span.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentOutOfRangeException | Either source length is less than the count, or the destination length is less than the count. |

## See Also

* class [ByteConverter](../ByteConverter.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

---

# ByteConverter.CopyTo method (10 of 21)

Copies a byte sequence to a target span of 32-bit unsigned values.

```csharp
public int CopyTo(ReadOnlySpan<byte> source, Span<uint> dest, int? count = null)
```

| parameter | description |
| --- | --- |
| source | The source byte sequence to copy. |
| dest | The target span of 32-bit unsigned values. |
| count | The number of elements to copy. If null the entire source is copied. |

## Return Value

The number of elements copied to the target span.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentOutOfRangeException | Either source length is less than the count, or the destination length is less than the count. |

## See Also

* class [ByteConverter](../ByteConverter.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

---

# ByteConverter.CopyTo method (11 of 21)

Copies a byte sequence to a target span of 64-bit unsigned values.

```csharp
public int CopyTo(ReadOnlySpan<byte> source, Span<ulong> dest, int? count = null)
```

| parameter | description |
| --- | --- |
| source | The source byte sequence to copy. |
| dest | The target span of 64-bit unsigned values. |
| count | The number of elements to copy. If null the entire source is copied. |

## Return Value

The number of elements copied to the target span.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentOutOfRangeException | Either source length is less than the count, or the destination length is less than the count. |

## See Also

* class [ByteConverter](../ByteConverter.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

---

# ByteConverter.CopyTo method (12 of 21)

Copies a byte sequence to a target span of 16-bit unsigned values.

```csharp
public int CopyTo(ReadOnlySpan<byte> source, Span<ushort> dest, int? count = null)
```

| parameter | description |
| --- | --- |
| source | The source byte sequence to copy. |
| dest | The target span of 16-bit unsigned values. |
| count | The number of elements to copy. If null the entire source is copied. |

## Return Value

The number of elements copied to the target span.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentOutOfRangeException | Either source length is less than the count, or the destination length is less than the count. |

## See Also

* class [ByteConverter](../ByteConverter.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

---

# ByteConverter.CopyTo method (13 of 21)

Copies a span of 64-bit floating-point values to a target byte sequence.

```csharp
public int CopyTo(ReadOnlySpan<double> source, Span<byte> dest, int? count = null)
```

| parameter | description |
| --- | --- |
| source | The source span of 64-bit floating-point values to copy. |
| dest | The target byte sequence. |
| count | The number of elements to copy. If null the entire source is copied. |

## Return Value

The number of elements copied to the target byte sequence.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentOutOfRangeException | Either source length is less than the count, or the destination length is less than the count. |

## See Also

* class [ByteConverter](../ByteConverter.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

---

# ByteConverter.CopyTo method (14 of 21)

Copies a span of 32-bit floating-point values to a target byte sequence.

```csharp
public int CopyTo(ReadOnlySpan<float> source, Span<byte> dest, int? count = null)
```

| parameter | description |
| --- | --- |
| source | The source span of 32-bit floating-point values to copy. |
| dest | The target byte sequence. |
| count | The number of elements to copy. If null the entire source is copied. |

## Return Value

The number of elements copied to the target byte sequence.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentOutOfRangeException | Either source length is less than the count, or the destination length is less than the count. |

## See Also

* class [ByteConverter](../ByteConverter.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

---

# ByteConverter.CopyTo method (15 of 21)

Copies a span of signed 32-bit values to a target byte sequence.

```csharp
public int CopyTo(ReadOnlySpan<int> source, Span<byte> dest, int? count = null)
```

| parameter | description |
| --- | --- |
| source | The source span of signed 32-bit values to copy. |
| dest | The target byte sequence. |
| count | The number of elements to copy. If null the entire source is copied. |

## Return Value

The number of elements copied to the target byte sequence.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentOutOfRangeException | Either source length is less than the count, or the destination length is less than the count. |

## See Also

* class [ByteConverter](../ByteConverter.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

---

# ByteConverter.CopyTo method (16 of 21)

Copies a span of signed 64-bit values to a target byte sequence.

```csharp
public int CopyTo(ReadOnlySpan<long> source, Span<byte> dest, int? count = null)
```

| parameter | description |
| --- | --- |
| source | The source span of signed 64-bit values to copy. |
| dest | The target byte sequence. |
| count | The number of elements to copy. If null the entire source is copied. |

## Return Value

The number of elements copied to the target byte sequence.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentOutOfRangeException | Either source length is less than the count, or the destination length is less than the count. |

## See Also

* class [ByteConverter](../ByteConverter.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

---

# ByteConverter.CopyTo method (17 of 21)

Copies a span of signed 8-bit values to a target byte sequence.

```csharp
public int CopyTo(ReadOnlySpan<sbyte> source, Span<byte> dest, int? count = null)
```

| parameter | description |
| --- | --- |
| source | The source span of signed 8-bit values to copy. |
| dest | The target byte sequence. |
| count | The number of elements to copy. If null the entire source is copied. |

## Return Value

The number of elements copied to the target byte sequence.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentOutOfRangeException | Either source length is less than the count, or the destination length is less than the count. |

## See Also

* class [ByteConverter](../ByteConverter.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

---

# ByteConverter.CopyTo method (18 of 21)

Copies a span of signed 16-bit values to a target byte sequence.

```csharp
public int CopyTo(ReadOnlySpan<short> source, Span<byte> dest, int? count = null)
```

| parameter | description |
| --- | --- |
| source | The source span of signed 16-bit values to copy. |
| dest | The target byte sequence. |
| count | The number of elements to copy. If null the entire source is copied. |

## Return Value

The number of elements copied to the target byte sequence.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentOutOfRangeException | Either source length is less than the count, or the destination length is less than the count. |

## See Also

* class [ByteConverter](../ByteConverter.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

---

# ByteConverter.CopyTo method (19 of 21)

Copies a span of unsigned 32-bit values to a target byte sequence.

```csharp
public int CopyTo(ReadOnlySpan<uint> source, Span<byte> dest, int? count = null)
```

| parameter | description |
| --- | --- |
| source | The source span of unsigned 32-bit values to copy. |
| dest | The target byte sequence. |
| count | The number of elements to copy. If null the entire source is copied. |

## Return Value

The number of elements copied to the target byte sequence.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentOutOfRangeException | Either source length is less than the count, or the destination length is less than the count. |

## See Also

* class [ByteConverter](../ByteConverter.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

---

# ByteConverter.CopyTo method (20 of 21)

Copies a span of unsigned 64-bit values to a target byte sequence.

```csharp
public int CopyTo(ReadOnlySpan<ulong> source, Span<byte> dest, int? count = null)
```

| parameter | description |
| --- | --- |
| source | The source span of unsigned 64-bit values to copy. |
| dest | The target byte sequence. |
| count | The number of elements to copy. If null the entire source is copied. |

## Return Value

The number of elements copied to the target byte sequence.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentOutOfRangeException | Either source length is less than the count, or the destination length is less than the count. |

## See Also

* class [ByteConverter](../ByteConverter.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

---

# ByteConverter.CopyTo method (21 of 21)

Copies a span of unsigned 16-bit values to a target byte sequence.

```csharp
public int CopyTo(ReadOnlySpan<ushort> source, Span<byte> dest, int? count = null)
```

| parameter | description |
| --- | --- |
| source | The source span of unsigned 16-bit values to copy. |
| dest | The target byte sequence. |
| count | The number of elements to copy. If null the entire source is copied. |

## Return Value

The number of elements copied to the target byte sequence.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentOutOfRangeException | Either source length is less than the count, or the destination length is less than the count. |

## See Also

* class [ByteConverter](../ByteConverter.md)
* namespace [Phaeyz.Marshalling](../../Phaeyz.Marshalling.md)

<!-- DO NOT EDIT: generated by xmldocmd for Phaeyz.Marshalling.dll -->

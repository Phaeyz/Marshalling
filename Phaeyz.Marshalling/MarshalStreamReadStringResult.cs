namespace Phaeyz.Marshalling;

/// <summary>
/// Used by all <c>ReadStringAsync</c> overloads in <see cref="Phaeyz.Marshalling.MarshalStream"/> to return the result of
/// a string read operation.
/// </summary>
/// <param name="value">
/// The string value which has been read.
/// </param>
/// <param name="bytesRead">
/// The number of bytes read from the stream.
/// </param>
/// <param name="isEndedDueToNullTerminator">
/// Indicates whether or not a null terminator was encountered while reading from the stream which caused reading to stop.
/// This may only be <c>true</c> is the read operation was requested to stop on null terminators.
/// </param>
/// <param name="isEndOfStream">
/// Indicates whether or not the end of the stream was reached while reading from the stream.
/// </param>
public readonly struct MarshalStreamReadStringResult(
    string value,
    int bytesRead,
    bool isEndedDueToNullTerminator,
    bool isEndOfStream) : IEquatable<MarshalStreamReadStringResult>
{
    /// <summary>
    /// The string value which has been read.
    /// </summary>
    public string Value { get; } = value;

    /// <summary>
    /// The number of bytes read from the stream.
    /// </summary>
    /// <remarks>
    /// This may be zero if <c>maxBytesToRead</c> if also zero which in that case <c>EndOfStreamReached</c> will also be false.
    /// If reading started at the end of stream and the maximum number of bytes is greater than zero, this will be zero and <c>EndOfStreamReached</c> will be true.
    /// </remarks>
    public int BytesRead { get; } = bytesRead;

    /// <summary>
    /// Indicates whether or not a null terminator was encountered while reading from the stream which caused reading to stop.
    /// This may only be <c>true</c> is the read operation was requested to stop on null terminators.
    /// </summary>
    public bool IsEndedDueToNullTerminator { get; } = isEndedDueToNullTerminator;

    /// <summary>
    /// Indicates whether or not the end of the stream was reached while reading from the stream.
    /// </summary>
    /// <remarks>
    /// The value of <c>EndedDueToNullTerminator</c> will always be false if this is true.
    /// </remarks>
    public bool IsEndOfStream { get; } = isEndOfStream;

    /// <summary>
    /// Determines whether two object instances are equal.
    /// </summary>
    /// <param name="other">
    /// The object to compare with the current object.
    /// </param>
    /// <returns>
    /// <c>true</c>> if the specified object is equal to the current object; otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(MarshalStreamReadStringResult other) =>
        Value == other.Value && BytesRead == other.BytesRead &&
        IsEndedDueToNullTerminator == other.IsEndedDueToNullTerminator && IsEndOfStream == other.IsEndOfStream;

    /// <summary>
    /// Determines whether two object instances are equal.
    /// </summary>
    /// <param name="obj">
    /// The object to compare with the current object.
    /// </param>
    /// <returns>
    /// <c>true</c>> if the specified object is equal to the current object; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj) =>
        obj is MarshalStreamReadStringResult other && Equals(other);

    /// <summary>
    /// Computes a hash code for the current instance.
    /// </summary>
    /// <returns>
    /// A hash code for the current instance.
    /// </returns>
    public override int GetHashCode() =>
        HashCode.Combine(Value, BytesRead, IsEndedDueToNullTerminator, IsEndOfStream);

    /// <summary>
    /// Creates a friendly string for the current instance.
    /// </summary>
    /// <returns>
    /// A friendly string for the current instance.
    /// </returns>
    public override string ToString() =>
        $"{{ Value = {Value}, ReadByteCount = {BytesRead}, IsEndedDueToNullTerminator = {IsEndedDueToNullTerminator}, IsEndOfStream = {IsEndOfStream} }}";

    /// <summary>
    /// Determines if the two objects are equal.
    /// </summary>
    /// <param name="left">
    /// The first object to compare.
    /// </param>
    /// <param name="right">
    /// The second object to compare.
    /// </param>
    /// <returns>
    /// <c>true</c>> if the first object is equal to the second object; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(MarshalStreamReadStringResult left, MarshalStreamReadStringResult right) => left.Equals(right);

    /// <summary>
    /// Determines if the two objects are not equal.
    /// </summary>
    /// <param name="left">
    /// The first object to compare.
    /// </param>
    /// <param name="right">
    /// The second object to compare.
    /// </param>
    /// <returns>
    /// <c>true</c>> if the first object is not equal to the second object; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(MarshalStreamReadStringResult left, MarshalStreamReadStringResult right) => !left.Equals(right);
}
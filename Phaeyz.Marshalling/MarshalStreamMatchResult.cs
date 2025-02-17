namespace Phaeyz.Marshalling;

/// <summary>
/// Used by all <c>Match</c> overloads in <see cref="Phaeyz.Marshalling.MarshalStream"/> to return the result of
/// a match operation.
/// </summary>
/// <param name="success">
/// If <c>true</c> the match was successful; <c>false</c> otherwise.
/// </param>
/// <param name="bytesRead">
/// The number of bytes read from the stream, indicating the number of bytes the stream pointer has been advanced.
/// </param>
/// <param name="isEndOfStream">
/// Indicates whether or not the end of the stream was reached while matching from the stream.
/// </param>
public readonly struct MarshalStreamMatchResult(
    bool success,
    int bytesRead,
    bool isEndOfStream) : IEquatable<MarshalStreamMatchResult>
{
    /// <summary>
    /// If <c>true</c> the match was successful; <c>false</c> otherwise.
    /// </summary>
    public bool Success { get; } = success;

    /// <summary>
    /// The number of bytes read from the stream, indicating the number of bytes the stream pointer has been advanced.
    /// </summary>
    /// <remarks>
    /// The only times this value will be greater than zero is upon a successful match (and then the value will always
    /// be the length of byte sequence being matched), or if the stream is encapsulating another stream (not a fixed buffer)
    /// and the stream buffer is shorter than the byte sequence being matched. For unsuccessful matches when encapsulating
    /// another stream, you can guarantee this will always be zero by ensuring the stream buffer is at least the length of
    /// the byte sequence being matched.
    /// </remarks>
    public int BytesRead { get; } = bytesRead;

    /// <summary>
    /// Indicates whether or not the end of the stream was reached while matching from the stream.
    /// </summary>
    /// <remarks>
    /// If <c>IsEndOfStream</c>> is <c>true</c> the value of <c>Success</c> will always be false.
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
    public bool Equals(MarshalStreamMatchResult other) =>
        Success == other.Success && BytesRead == other.BytesRead && IsEndOfStream == other.IsEndOfStream;

    /// <summary>
    /// Determines whether two object instances are equal.
    /// </summary>
    /// <param name="obj">
    /// The object to compare with the current object.
    /// </param>
    /// <returns>
    /// <c>true</c>> if the specified object is equal to the current object; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj) => obj is MarshalStreamMatchResult other && Equals(other);

    /// <summary>
    /// Computes a hash code for the current instance.
    /// </summary>
    /// <returns>
    /// A hash code for the current instance.
    /// </returns>
    public override int GetHashCode() => HashCode.Combine(Success, BytesRead, IsEndOfStream);

    /// <summary>
    /// Creates a friendly string for the current instance.
    /// </summary>
    /// <returns>
    /// A friendly string for the current instance.
    /// </returns>
    public override string ToString() =>
        $"{{ Success = {Success}, ReadByteCount = {BytesRead}, IsEndOfStream = {IsEndOfStream} }}";

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
    public static bool operator ==(MarshalStreamMatchResult left, MarshalStreamMatchResult right) => left.Equals(right);

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
    public static bool operator !=(MarshalStreamMatchResult left, MarshalStreamMatchResult right) => !left.Equals(right);
}
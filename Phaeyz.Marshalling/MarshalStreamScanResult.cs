namespace Phaeyz.Marshalling
{
    /// <summary>
    /// Used by scan methods in <see cref="Phaeyz.Marshalling.MarshalStream"/> to return the result of a scan operation.
    /// </summary>
    /// <param name="bytesRead">
    /// The total number of bytes read during the scan operation.
    /// </param>
    /// <param name="isPositiveMatch">
    /// <c>true</c> if the scan found a successful match, <c>false</c> otherwise.
    /// </param>
    /// <param name="isEndOfStream">
    /// <c>true</c> if the end of the stream was reached during scanning, <c>false</c> otherwise.
    /// </param>
    public readonly struct MarshalStreamScanResult(
        long bytesRead,
        bool isPositiveMatch,
        bool isEndOfStream) : IEquatable<MarshalStreamScanResult>
    {
        /// <summary>
        /// The total number of bytes read during the scan operation.
        /// </summary>
        public long BytesRead { get; } = bytesRead;

        /// <summary>
        /// Returns <c>true</c> if the scan found a successful match, <c>false</c> otherwise.
        /// </summary>
        public bool IsPositiveMatch { get; } = isPositiveMatch;

        /// <summary>
        /// Returns <c>true</c> if the end of the stream was reached during scanning, <c>false</c> otherwise.
        /// </summary>
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
        public bool Equals(MarshalStreamScanResult other) =>
            BytesRead == other.BytesRead && IsPositiveMatch == other.IsPositiveMatch && IsEndOfStream == other.IsEndOfStream;

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
            obj is MarshalStreamScanResult other && Equals(other);

        /// <summary>
        /// Computes a hash code for the current instance.
        /// </summary>
        /// <returns>
        /// A hash code for the current instance.
        /// </returns>
        public override int GetHashCode() =>
            HashCode.Combine(BytesRead, IsPositiveMatch, IsEndOfStream);

        /// <summary>
        /// Creates a friendly string for the current instance.
        /// </summary>
        /// <returns>
        /// A friendly string for the current instance.
        /// </returns>
        public override string ToString() =>
            $"{{ ReadByteCount = {BytesRead}, IsPositiveMatch = {IsPositiveMatch}, IsEndOfStream = {IsEndOfStream} }}";

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
        public static bool operator ==(MarshalStreamScanResult left, MarshalStreamScanResult right) => left.Equals(right);

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
        public static bool operator !=(MarshalStreamScanResult left, MarshalStreamScanResult right) => !left.Equals(right);
    }
}
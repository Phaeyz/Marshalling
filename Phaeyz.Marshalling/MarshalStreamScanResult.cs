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

        /// <inheritdoc/>
        public bool Equals(MarshalStreamScanResult other) =>
            BytesRead == other.BytesRead && IsPositiveMatch == other.IsPositiveMatch && IsEndOfStream == other.IsEndOfStream;

        /// <inheritdoc/>
        public override bool Equals(object? obj) =>
            obj is MarshalStreamScanResult other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() =>
            HashCode.Combine(BytesRead, IsPositiveMatch, IsEndOfStream);

        /// <inheritdoc/>
        public override string ToString() =>
            $"{{ ReadByteCount = {BytesRead}, IsPositiveMatch = {IsPositiveMatch}, IsEndOfStream = {IsEndOfStream} }}";

        /// <inheritdoc/>
        public static bool operator ==(MarshalStreamScanResult left, MarshalStreamScanResult right) => left.Equals(right);

        /// <inheritdoc/>
        public static bool operator !=(MarshalStreamScanResult left, MarshalStreamScanResult right) => !left.Equals(right);
    }
}
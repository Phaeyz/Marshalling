namespace Phaeyz.Marshalling;

/// <summary>
/// This interface is usable with <see cref="Phaeyz.Marshalling.MarshalStream"/> to process bytes
/// being read and written.
/// </summary>
/// <remarks>
/// The purpose of this interface is to support capabilities such as computing a CRC on a buffered stream,
/// while still keeping stream marshalling as efficient as possible.
/// </remarks>
public interface IMarshalStreamProcessor
{
    /// <summary>
    /// Processes bytes being read or written to a <see cref="Phaeyz.Marshalling.MarshalStream"/> instance.
    /// </summary>
    /// <param name="bytes">
    /// The bytes being processed.
    /// </param>
    /// <remarks>
    /// If this method throws an exception, the state of the <see cref="Phaeyz.Marshalling.MarshalStream"/>
    /// will be corrupted and invalid.
    /// </remarks>
    void Process(ReadOnlySpan<byte> bytes);
}
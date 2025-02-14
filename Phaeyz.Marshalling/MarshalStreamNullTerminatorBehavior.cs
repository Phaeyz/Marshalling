namespace Phaeyz.Marshalling;

/// <summary>
/// Specifies the behavior of a <see cref="Phaeyz.Marshalling.MarshalStream"/> when it encounters a null terminator.
/// </summary>
public enum MarshalStreamNullTerminatorBehavior
{
    /// <summary>
    /// Null terminators are ignored.
    /// </summary>
    Ignore,

    /// <summary>
    /// The stream read operation is stopped after the first null terminator is encountered.
    /// The null terminator is omitted from the resulting string.
    /// </summary>
    Stop,

    /// <summary>
    /// Null terminators are read, however all trailing null terminators are omitted from
    /// the resulting string. This is useful when reading a fixed length string with a max
    /// number of bytes.
    /// </summary>
    TrimTrailing,
}
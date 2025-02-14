using System.Text;

namespace Phaeyz.Marshalling;

/// <summary>
/// Utility methods for text encoding.
/// </summary>
public static class EncodingExtensions
{
    private static readonly char[] s_getByteCountCharBuffer = ['A'];

    /// <summary>
    /// Computes the minimum number of bytes required to store a single code unit in the current character encoding.
    /// </summary>
    /// <param name="this">
    /// The current <see cref="System.Text.Encoding"/> object.
    /// </param>
    /// <returns>
    /// The minimum number of bytes required to store a single code unit in the current character encoding.
    /// </returns>
    public static int GetMinimumCodeUnitSize(this Encoding @this)
    {
        ArgumentNullException.ThrowIfNull(@this);

        int bytesPerCodeUnit
            = @this is ASCIIEncoding ? 1
            : @this is UTF7Encoding ? 1
            : @this is UTF8Encoding ? 1
            : @this is UnicodeEncoding ? 2
            : @this is UTF32Encoding ? 4
            : Enum.IsDefined((CodePage1ByteMinimum)@this.CodePage) ? 1
            : @this.GetByteCount(s_getByteCountCharBuffer);

        return bytesPerCodeUnit;
    }

    /// <summary>
    /// Given a buffer, reads a null terminated string beginning at the index zero.
    /// </summary>
    /// <param name="this">
    /// The current Encoding object.
    /// </param>
    /// <param name="buffer">
    /// A buffer containing the null terminated string data beginning at index zero.
    /// </param>
    /// <param name="nullTerminatorIndex">
    /// On output, receives the index within the buffer where the null terminator was found. This may be
    /// -1 if a null terminator was not found, and the entire buffer contains the string.
    /// </param>
    /// <returns>
    /// A string read from the buffer.
    /// </returns>
    public static string GetNullTerminatedString(
        this Encoding @this,
        ReadOnlySpan<byte> buffer,
        out int nullTerminatorIndex)
    {
        // First, create a null terminator for the size of the current encoding.
        Span<byte> nullTerminator = stackalloc byte[@this.GetMinimumCodeUnitSize()];
        nullTerminator.Clear(); // Spec says initialization of stack bytes is undefined.

        // Find the null terminator in the buffer. This also sets the output parameter which defines
        // the index within the input buffer where the null terminator was found.
        nullTerminatorIndex = buffer.IndexOf(nullTerminator);

        // Now get the string from the buffer using all bytes leading up to the null terminator.
        // If a null terminator wasn't found, use the entire buffer.
        return @this.GetString(nullTerminatorIndex == -1 ? buffer : buffer[..nullTerminatorIndex]);
    }
}

/// <summary>
/// Uncommon code pages which have a minimum code unit size of 1 byte.
/// </summary>
file enum CodePage1ByteMinimum : int
{
    EUCCN               = 936,
    Windows1252         = 1252,
    MacKorean           = 10003,
    MacGB2312           = 10008,
    GB2312              = 20936,
    DLLKorean           = 20949,
    ISO_8859_1          = 28591, // Latin1
    ISO_8859_8_Visual   = 28598,
    ISO_8859_8I         = 38598,
    ISO2022JP           = 50220,
    ISO2022JPESC        = 50221,
    ISO2022JPSISO       = 50222,
    ISOKorean           = 50225,
    ISOSimplifiedCN     = 50227,
    EUCJP               = 51932,
    ChineseHZ           = 52936,
    DuplicateEUCCN      = 51936,
    EUCKR               = 51949,
    GB18030             = 54936,
    ISCIIAssemese       = 57006,
    ISCIIBengali        = 57003,
    ISCIIDevanagari     = 57002,
    ISCIIGujarathi      = 57010,
    ISCIIKannada        = 57008,
    ISCIIMalayalam      = 57009,
    ISCIIOriya          = 57007,
    ISCIIPanjabi        = 57011,
    ISCIITamil          = 57004,
    ISCIITelugu         = 57005,
}
using System.Text;

namespace Phaeyz.Marshalling.Test;

internal class Crc32Test
{
    private static readonly Dictionary<string, uint> s_knownValues = new()
    {
        ["a"]       = 0xE8B7BE43,
        ["ab"]      = 0x9E83486D,
        ["abc"]     = 0x352441C2,
        ["abcd"]    = 0xED82CD11,
        ["abcde"]   = 0x8587D865,
    };

    [Test]
    public async Task Update_StartingValue_KnownValueMatches()
    {
        foreach (KeyValuePair<string, uint> kvp in s_knownValues)
        {
            Span<byte> inputBytes = Encoding.UTF8.GetBytes(kvp.Key).AsSpan();
            Crc32 crc32 = new();
            crc32.Update(inputBytes[..^1]);
            crc32 = new(crc32.Value);
            crc32.Update(inputBytes[^1]);
            await Assert.That(crc32.Value).IsEqualTo(kvp.Value);
        }
    }

    [Test]
    public async Task Update_IndividualBytes_KnownValueMatches()
    {
        foreach (KeyValuePair<string, uint> kvp in s_knownValues)
        {
            Crc32 crc32 = new();
            foreach (byte value in Encoding.UTF8.GetBytes(kvp.Key))
            {
                crc32.Update(value);
            }
            await Assert.That(crc32.Value).IsEqualTo(kvp.Value);
        }
    }

    [Test]
    public async Task Update_IndividualBytesWithReset_KnownValueMatches()
    {
        Crc32 crc32 = new();
        foreach (KeyValuePair<string, uint> kvp in s_knownValues)
        {
            crc32.Reset();
            foreach (byte value in Encoding.UTF8.GetBytes(kvp.Key))
            {
                crc32.Update(value);
            }
            await Assert.That(crc32.Value).IsEqualTo(kvp.Value);
        }
    }

    [Test]
    public async Task Update_SeriesOfBytes_KnownValueMatches()
    {
        foreach (KeyValuePair<string, uint> kvp in s_knownValues)
        {
            Crc32 crc32 = new();
            crc32.Update(Encoding.UTF8.GetBytes(kvp.Key));
            await Assert.That(crc32.Value).IsEqualTo(kvp.Value);
        }
    }

    [Test]
    public async Task Update_SeriesOfBytesWithReset_KnownValueMatches()
    {
        Crc32 crc32 = new();
        foreach (KeyValuePair<string, uint> kvp in s_knownValues)
        {
            crc32.Reset();
            crc32.Update(Encoding.UTF8.GetBytes(kvp.Key));
            await Assert.That(crc32.Value).IsEqualTo(kvp.Value);
        }
    }

    [Test]
    public async Task Value_NoUpdates_ZeroValue()
    {
        Crc32 crc32 = new();
        await Assert.That(crc32.Value).IsZero();
    }

    [Test]
    public async Task Value_ZeroStartingValueAndNoUpdates_ZeroValue()
    {
        Crc32 crc32 = new(0);
        await Assert.That(crc32.Value).IsZero();
    }

    [Test]
    public async Task Value_OneStartingValueAndNoUpdates_OneValue()
    {
        Crc32 crc32 = new(1);
        await Assert.That(crc32.Value).IsEqualTo(1u);;
    }
}

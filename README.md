# Phaeyz

Phaeyz is a set of libraries created and polished over time for use with other projects, and made available here for convenience.

All Phaeyz libraries may be found [here](https://github.com/Phaeyz).

# Phaeyz.Marshalling

API documentation for **Phaeyz.Marshalling** library is [here](https://github.com/Phaeyz/Marshalling/blob/main/docs/Phaeyz.Marshalling.md).

This library contains classes which are convenient for marshalling data from one format to another. Here are some highlights.

## ByteConverter

```C#
var byteConverter = ByteConverter.BigEndian; // Byte converter is an instance and can be passed around.
var buffer = new byte[10];
byteConverter.FromInt32(42, buffer.AsSpan(10)); // Writes an int, optimized for Span<T>.
```

## MarshalStream

```C#
var buffer = new byte[100];
var stream = new MarshalStream(buffer.AsMemory()); // Can wrap a stream or a buffer.
stream.EnsureByteCountAvailableInBuffer(1); // Control over the internal buffer.
int value = ByteConverter.ToInt32(stream.BufferedReadableBytes.AsSpan()); // Can directly access the read buffer.
// Efficient scanning of a stream or buffer.
static int ScanBuffer(ReadOnlyMemory<byte> scanBuffer)
{
    ReadOnlySpan<byte> scanSpan = scanBuffer.Span;
    for (int i = 0; i < scanSpan.Length - 1; i++)
    {
        if (scanSpan[i] == 0 && scanSpan[i + 1] == 0)
        {
            return i;
        }
    }
    return scanSpan.Length - 1;
}
var result = stream.Scan(2, 50, ScanBuffer);
```

# Licensing

This project is licensed under GNU General Public License v3.0, which means you can use it for personal or educational purposes for free. However, donations are always encouraged to support the ongoing development of adding new features and resolving issues.

If you plan to use this code for commercial purposes or within an organization, we kindly ask for a donation to support the project's development. Any reasonably sized donation amount which reflects the perceived value of using Phaeyz in your product or service is accepted.

## Donation Options

There are several ways to support Phaeyz with a donation. Perhaps the best way is to use Patreon so that recurring small donations continue to support the development of Phaeyz.

- **Patreon**: [https://www.patreon.com/phaeyz](https://www.patreon.com/phaeyz)
- **Bitcoin**: Send funds to address: ```bc1qdzdahz8d7jkje09fg7s7e8xedjsxm6kfhvsgsw```
- **PayPal**: Send funds to ```phaeyz@pm.me``` ([directions](https://www.paypal.com/us/cshelp/article/how-do-i-send-money-help293))

Your support is greatly appreciated and helps me continue to improve and maintain Phaeyz!
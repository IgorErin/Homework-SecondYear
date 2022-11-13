namespace FTP.Extensions;

using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

public static class NetworkStreamExtensions
{
    private const int CharCodeLength = 2;

    private static readonly byte[] FalseInBytes = { Convert.ToByte(false) };
    private static readonly byte[] TrueInBytes = { Convert.ToByte(true) };

    public static ConfiguredTaskAwaitable ConfigureWriteAsync(this NetworkStream stream, byte[] buffer, int offset, int count)
        => stream.WriteAsync(buffer, offset, count).ConfigureAwait(false);

    public static ConfiguredTaskAwaitable ConfigureWriteAsync(this NetworkStream stream, byte[] buffer)
        => stream.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

    public static ConfiguredTaskAwaitable ConfigureWriteWhiteSpaceAsync(this NetworkStream stream)
        => stream.WriteAsync(Encoding.Unicode.GetBytes(" "), 0, CharCodeLength).ConfigureAwait(false);

    public static async Task ConfigureReadAsyncWhithCheck(this NetworkStream stream, byte[] buffer)
    {
        if (await stream.ReadAsync(buffer).ConfigureAwait(false) != buffer.Length)
        {
            throw new Exception("read size not compatible"); //TODO()
        }
    }

    public static ConfiguredTaskAwaitable ConfigureWriteTrue(this NetworkStream stream)
        => stream.WriteAsync(TrueInBytes, 0, FalseInBytes.Length).ConfigureAwait(false);

    public static ConfiguredTaskAwaitable ConfigureWriteFalse(this NetworkStream stream)
        => stream.WriteAsync(TrueInBytes, 0, TrueInBytes.Length).ConfigureAwait(false);

    public static ConfiguredTaskAwaitable ConfigureFlushAsync(this NetworkStream stream)
        => stream.FlushAsync().ConfigureAwait(false);

    public static ConfiguredTaskAwaitable<string?> ConfigureReadLineAsync(this StreamReader stream)
        => stream.ReadLineAsync().ConfigureAwait(false);
}

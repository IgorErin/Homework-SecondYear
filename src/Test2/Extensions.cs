namespace Test2;

using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using Exceptions;

/// <summary>
/// Some <see cref="NetworkStream"/> and <see cref="StreamReader"/> extensions.
/// </summary>
public static class NetworkStreamExtensions
{
    private const int CharCodeLength = 2;

    private static readonly byte[] FalseInBytes = { Convert.ToByte(false) };
    private static readonly byte[] TrueInBytes = { Convert.ToByte(true) };

    private static readonly byte[] WhiteSpaceBytes = Encoding.Unicode.GetBytes(" ");

    /// <summary>
    /// Method for reading from a <see cref="NetworkStream"/>, with a predefined offset = 0 and .ConfigureAwait(false).
    /// e. g.
    /// <code>
    /// stream.WriteAsync(buffer, 0, count).ConfigureAwait(false)
    /// </code>
    /// </summary>
    /// <param name="stream">Read stream.</param>
    /// <param name="buffer">Read buffer.</param>
    /// <param name="count">Buffer read count.</param>
    /// <returns><see cref="ConfiguredTaskAwaitable"/>.</returns>
    public static ConfiguredTaskAwaitable ConfigureWriteAsyncFromZero(this NetworkStream stream, byte[] buffer, int count)
        => stream.WriteAsync(buffer, 0, count).ConfigureAwait(false);

    /// <summary>
    /// Method for writing to a <see cref="NetworkStream"/>, with a predefined offset = 0 and .ConfigureAwait(false).
    /// e. g.
    /// <code>
    /// stream.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false)
    /// </code>
    /// </summary>
    /// <param name="stream">stream to write.</param>
    /// <param name="buffer">buffer to write from.</param>
    /// <returns><see cref="ConfiguredTaskAwaitable"/>.</returns>
    public static ConfiguredTaskAwaitable ConfigureWriteAsync(this NetworkStream stream, byte[] buffer)
        => stream.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

    /// <summary>
    /// Method for writing white space to a <see cref="NetworkStream"/> by <see cref="Convert.ToByte(bool)"/> ,
    /// with a predefined offset = 0 and .ConfigureAwait(false).
    /// e. g.
    /// <code>
    /// stream.WriteAsync(WhiteSpaceBytes, 0, CharCodeLength).ConfigureAwait(false)
    /// </code>
    /// </summary>
    /// <param name="stream">stream to write.</param>
    /// <returns><see cref="ConfiguredTaskAwaitable"/>.</returns>
    public static ConfiguredTaskAwaitable ConfigureWriteWhiteSpaceAsync(this NetworkStream stream)
        => stream.WriteAsync(WhiteSpaceBytes, 0, CharCodeLength).ConfigureAwait(false);

    /// <summary>
    /// Method for writing to a <see cref="NetworkStream"/>, with a predefined offset = 0 and .ConfigureAwait(false).
    /// And checking for the number of bytes in the response.
    /// </summary>
    /// <param name="stream">stream to write.</param>
    /// <param name="buffer">buffer to write from.</param>
    /// <exception cref="DataLossException">
    /// An exception will be thrown if a smaller number of bytes are written to the buffer than its size.
    /// </exception>
    /// <returns>
    /// Byte writing <see cref="Task"/>.
    /// </returns>
    public static async Task ConfigureReadAsyncWithCheck(this NetworkStream stream, byte[] buffer)
    {
        if (await stream.ReadAsync(buffer).ConfigureAwait(false) != buffer.Length)
        {
            throw new DataLossException("data loss during transmission, incomplete specifier");
        }
    }

    /// <summary>
    /// Method for writing true <see cref="bool"/> value by
    /// <see cref="Convert.ToByte(bool)"/> to a <see cref="NetworkStream"/>,
    /// with a predefined offset = 0 and .ConfigureAwait(false).
    /// </summary>
    /// <param name="stream">stream to write.</param>
    /// <returns><see cref="ConfiguredTaskAwaitable"/>.</returns>
    public static ConfiguredTaskAwaitable ConfigureWriteTrueAsync(this NetworkStream stream)
        => stream.WriteAsync(TrueInBytes, 0, FalseInBytes.Length).ConfigureAwait(false);

    /// <summary>
    /// Method for writing false <see cref="bool"/> value by
    /// <see cref="Convert.ToByte(bool)"/> to a <see cref="NetworkStream"/>,
    /// with a predefined offset = 0 and .ConfigureAwait(false).
    /// </summary>
    /// <param name="stream">stream to write.</param>
    /// <returns><see cref="ConfiguredTaskAwaitable"/>.</returns>
    public static ConfiguredTaskAwaitable ConfigureWriteFalseAsync(this NetworkStream stream)
        => stream.WriteAsync(FalseInBytes, 0, FalseInBytes.Length).ConfigureAwait(false);

    /// <summary>
    /// Method for <see cref="NetworkStream.FlushAsync()"/> to a <see cref="NetworkStream"/>, with .ConfigureAwait(false).
    /// </summary>
    /// <param name="stream">stream to flush.</param>
    /// <returns><see cref="ConfiguredTaskAwaitable"/>.</returns>
    public static ConfiguredTaskAwaitable ConfigureFlushAsync(this NetworkStream stream)
        => stream.FlushAsync().ConfigureAwait(false);

    /// <summary>
    /// Method for reading a string from <see cref="NetworkStream"/> with predefined .ConfigureAwait(false).
    /// </summary>
    /// <param name="stream">read stream.</param>
    /// <returns><see cref="ConfiguredTaskAwaitable"/> with string value.</returns>
    public static ConfiguredTaskAwaitable<string?> ConfigureReadLineAsync(this StreamReader stream)
        => stream.ReadLineAsync().ConfigureAwait(false);
}
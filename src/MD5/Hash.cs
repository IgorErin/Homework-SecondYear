namespace MD5;

using System.Text;

public class Hash
{
    public static async Task<byte[]> getSum(string path)
    {
        if (File.Exists(path))
        {
            return await GetFromFile(new FileInfo(path));
        }

        var DirInfo = new DirectoryInfo(path);

        if (DirInfo.Exists)
        {
            return await GetFromDir(DirInfo);
        }

        throw new DirectoryNotFoundException("file or directory not found");
    }

    private static async Task<byte[]> GetFromFile(FileInfo fileInfo)
    {
        try
        {
            var size = fileInfo.Length;

            var buffer = new byte[size + 1];

            await fileInfo.Open(FileMode.Open).WriteAsync(buffer);

            return System.Security.Cryptography.MD5.HashData(buffer);
        }
        catch (System.IO.IOException exception)
        {
            Console.WriteLine($"Cannot get hash from {fileInfo.Name}");
        }

        return new Byte[0];
    }

    private static async Task<byte[]> GetFromDir(DirectoryInfo directoryInfo)
    {
        var result = Encoding.ASCII.GetBytes(directoryInfo.Name);

        foreach (var file in directoryInfo.EnumerateFiles())
        {
            var fileBytes = await GetFromFile(new FileInfo(file.FullName));

            result = Join(result, fileBytes);
        }

        foreach (var directory in directoryInfo.GetDirectories())
        {
            var directoryResult = await GetFromDir(new DirectoryInfo(directory.FullName));

            result = Join(result, directoryResult);
        }

        return result;
    }

    private static byte[] Join(byte[] left, byte[] right)
    {
        var result = new byte[left.Length + right.Length];
        left.CopyTo(result, 0);
        right.CopyTo(result, left.Length);

        return result;
    }
}

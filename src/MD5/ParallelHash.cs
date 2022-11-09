namespace MD5;

using System.Text;

public class ParallelHash
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

    private static Task<byte[]> GetFromFile(FileInfo fileInfo)
    {
        return Hash.getSum(fileInfo.FullName);
    }

    private static async Task<byte[]> GetFromDir(DirectoryInfo directoryInfo)
    {
        var result = Encoding.ASCII.GetBytes(directoryInfo.Name);

        var size = directoryInfo.GetFiles().Length + directoryInfo.GetDirectories().Length;

        var taskList = new Task<byte[]>[directoryInfo.GetFiles().Length + directoryInfo.GetDirectories().Length];

        for (var i = 0; i < directoryInfo.GetFiles().Length; i++)
        {
            taskList[i] = GetFromFile(directoryInfo.GetFiles()[i]);
        }

        for (var i = directoryInfo.GetFiles().Length; i < size; i++)
        {
           taskList[i] = GetFromDir(directoryInfo.GetDirectories()[i]);
        }

        foreach (var task in taskList)
        {
            result = Join(result, await task);
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
namespace MyNunit.Extensions;

public static class ArrayExtensions
{
    public static T[] Fill<T>(this T[] array, T value)
    {
        for (var i = 0; i < array.Length; i++)
        {
            array[i] = value;
        }

        return array;
    }
}
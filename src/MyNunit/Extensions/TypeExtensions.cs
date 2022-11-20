namespace MyNunit.Extensions;

public static class TypeExtensions
{
    public static bool IsEqual(this Type firstType, Type secondType)
    {
        if (firstType.FullName != secondType.FullName)
        {
            return false;
        }

        return true;
    }
}
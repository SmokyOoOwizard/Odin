namespace Odin.Utils;

public static class ArrayExtensions
{
    public static bool IsSubArray<T>(this T[] a, T[] b) where T : IComparable<T>
    {
        var n = a.Length;
        var m = b.Length;

        for (var i = 0; i <= n - m; i++)
        {
            int j;
            for (j = 0; j < m; j++)
            {
                if (!EqualityComparer<T>.Default.Equals(a[i + j], b[j]))
                {
                    break;
                }
            }

            if (j == m)
            {
                return true;
            }
        }

        return false;
    }
}
﻿namespace Odin.Utils;

public class ArrayEqualityComparer<T> : IEqualityComparer<T[]>
{
    public bool Equals(T[] x, T[] y)
    {
        if (x.Length != y.Length)
        {
            return false;
        }
        for (var i = 0; i < x.Length; i++)
        {
            if (!x[i]!.Equals(y[i]))
            {
                return false;
            }
        }
        return true;
    }

    public int GetHashCode(T[] obj)
    {
        int result = 17;
        for (var i = 0; i < obj.Length; i++)
        {
            unchecked
            {
                result = result * 23 + obj[i]!.GetHashCode();
            }
        }
        return result;
    }
}
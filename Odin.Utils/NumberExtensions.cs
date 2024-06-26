namespace Odin.Utils;

public static class NumberExtensions
{
    public static sbyte MapToSbyte(this byte uValue)
    {
        return (sbyte)unchecked((sbyte)uValue + sbyte.MinValue);
    }

    public static byte MapToByte(this sbyte value)
    {
        return (byte)unchecked(value - sbyte.MinValue);
    }

    public static short MapToShort(this ushort uValue)
    {
        return (short)unchecked(uValue + short.MinValue);
    }

    public static ushort MapToUshort(this short value)
    {
        return (ushort)unchecked((value - short.MinValue));
    }

    public static int MapToInt(this uint uValue)
    {
        return unchecked((int)uValue + int.MinValue);
    }

    public static uint MapToUint(this int value)
    {
        return unchecked((uint)(value - int.MinValue));
    }

    public static long MapToLong(this ulong uValue)
    {
        return unchecked((long)uValue + long.MinValue);
    }

    public static ulong MapToUlong(this long value)
    {
        return unchecked((ulong)(value - long.MinValue));
    }
}
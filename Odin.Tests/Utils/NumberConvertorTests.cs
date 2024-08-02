using Odin.Utils;

namespace Odin.Tests.Utils;

public class NumberConvertorTests
{
    [Fact]
    public void UlongToLong()
    {
        var valueMin = ulong.MinValue;
        var lValueMin = valueMin.MapToLong();

        Assert.Equal(long.MinValue, lValueMin);

        var valueMax = ulong.MaxValue;
        var lValueMax = valueMax.MapToLong();

        Assert.Equal(long.MaxValue, lValueMax);
    }

    [Fact]
    public void LongToULong()
    {
        var valueMin = long.MinValue;
        var uValueMin = valueMin.MapToUlong();

        Assert.Equal(0ul, uValueMin);

        var valueMax = long.MaxValue;
        var uValueMax = valueMax.MapToUlong();

        Assert.Equal(ulong.MaxValue, uValueMax);
    }

    [Fact]
    public void UintToInt()
    {
        var valueMin = uint.MinValue;
        var lValueMin = valueMin.MapToInt();

        Assert.Equal(int.MinValue, lValueMin);

        var valueMax = uint.MaxValue;
        var lValueMax = valueMax.MapToInt();

        Assert.Equal(int.MaxValue, lValueMax);
    }

    [Fact]
    public void IntToUint()
    {
        var valueMin = int.MinValue;
        var uValueMin = valueMin.MapToUint();

        Assert.Equal(0u, uValueMin);

        var valueMax = int.MaxValue;
        var uValueMax = valueMax.MapToUint();

        Assert.Equal(uint.MaxValue, uValueMax);
    }
    
    [Fact]
    public void UshortToShort()
    {
        var valueMin = ushort.MinValue;
        var lValueMin = valueMin.MapToShort();

        Assert.Equal(short.MinValue, lValueMin);

        var valueMax = ushort.MaxValue;
        var lValueMax = valueMax.MapToShort();

        Assert.Equal(short.MaxValue, lValueMax);
    }

    [Fact]
    public void ShortToUshort()
    {
        var valueMin = short.MinValue;
        var uValueMin = valueMin.MapToUshort();

        Assert.Equal(0u, uValueMin);

        var valueMax = short.MaxValue;
        var uValueMax = valueMax.MapToUshort();

        Assert.Equal(ushort.MaxValue, uValueMax);
    }
    
    [Fact]
    public void ByteToSbyte()
    {
        var valueMin = byte.MinValue;
        var lValueMin = valueMin.MapToSbyte();

        Assert.Equal(sbyte.MinValue, lValueMin);

        var valueMax = byte.MaxValue;
        var lValueMax = valueMax.MapToSbyte();

        Assert.Equal(sbyte.MaxValue, lValueMax);
    }

    [Fact]
    public void SbyteToBytet()
    {
        var valueMin = sbyte.MinValue;
        var uValueMin = valueMin.MapToByte();

        Assert.Equal(0u, uValueMin);

        var valueMax = sbyte.MaxValue;
        var uValueMax = valueMax.MapToByte();

        Assert.Equal(byte.MaxValue, uValueMax);
    }
}
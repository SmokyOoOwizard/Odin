namespace Odin.Core.Abstractions.Components.Declarations
{
    [Flags]
    public enum EFieldType : ushort
    {
        String = 0,
        Int = 0b_0001_0000,
        Int8 = 0b_0001_0001,
        Int16 = 0b_0001_0010,
        Int32 = 0b_0001_0011,
        Int64 = 0b_0001_0100,
        UInt8 = 0b_0001_1001,
        UInt16 = 0b_0001_1010,
        UInt32 = 0b_0001_1011,
        UInt64 = 0b_0001_1100,
        Float = 0b_0010_0000,
        Double = 0b_0010_0001,
        Bool = 0b_0011_0000,
        Complex = ushort.MaxValue
    }
}
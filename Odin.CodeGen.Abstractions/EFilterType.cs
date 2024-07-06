namespace Odin.CodeGen.Abstractions;

public enum EFilterType
{
    Unknown,
    All,
    Any,
    Not,
    Has,
    NotHas,
    Added,
    Removed,
    AnyChanges
}
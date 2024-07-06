namespace Odin.CodeGen.Abstractions;

[Serializable]
public struct FilterComponent
{
    public EFilterType Type;
    public string GenericArg;
    public FilterComponent[] Children;
}
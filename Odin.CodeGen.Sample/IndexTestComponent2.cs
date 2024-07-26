using Odin.Abstractions.Components;

namespace Odin.Component.CodeGen.Sample;

public struct IndexTestComponent2 : IComponent
{
    [IndexBy]
    public ulong[] TestField;
}
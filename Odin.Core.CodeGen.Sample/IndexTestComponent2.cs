using Odin.Core.Abstractions.Components;
using Odin.Core.Abstractions.Components.Attributes;

namespace Odin.Core.CodeGen.Sample;

public struct IndexTestComponent2 : IComponent
{
    [IndexBy]
    public ulong[] TestField;
    
    [IndexBy]
    public ulong[] TestField2;
}
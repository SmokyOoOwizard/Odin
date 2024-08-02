using Odin.Core.Abstractions.Components;

namespace Odin.Core.CodeGen.Sample;

public struct SubStructTestComponent : IComponent
{
    public struct SubStruct
    {
        public int TestSubValue;
    }
    
    
    public SubStruct TestSubStruct; 
}
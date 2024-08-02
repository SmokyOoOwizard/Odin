using System;
using Odin.Core.Abstractions.Components;
using Odin.Core.Abstractions.Components.Attributes;

namespace Odin.Core.CodeGen.Sample;

public class ComponentClassOwner
{
    [IndexBy]
    public struct IndexTestComponent : IComponent
    {
        public Int32 TestField;
    }
}
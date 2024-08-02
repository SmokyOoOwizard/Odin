using System;
using Odin.Core.Abstractions.Components;
using Odin.Core.Abstractions.Components.Attributes;

namespace Odin.Core.CodeGen.Sample;

[IndexBy]
internal struct InternalIndexTestComponent : IComponent
{
    public Int32 TestField;
}
using System;
using Odin.Abstractions.Components;

namespace Odin.Component.CodeGen.Sample;

public struct SecondTestComponent : IComponent
{
    [IndexBy]
    public Int32 TestField;
}
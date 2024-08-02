using System;
using Odin.Abstractions.Components;

namespace Odin.Component.CodeGen.Sample;

[IndexBy]
public struct IndexTestComponent : IComponent
{
    public Int32 TestField;
}
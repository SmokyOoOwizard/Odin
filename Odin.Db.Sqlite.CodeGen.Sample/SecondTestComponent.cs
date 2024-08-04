using System;
using Odin.Core.Abstractions.Components;
using Odin.Core.Abstractions.Components.Attributes;

namespace Odin.Db.Sqlite.CodeGen.Sample;

public struct SecondTestComponent : IComponent
{
    [IndexBy]
    public Int32 TestField;
}
using System;
using Odin.Abstractions.Components;

namespace Odin.Db.Sqlite.CodeGen.Sample;

public struct SecondTestComponent : IComponent
{
    [IndexBy]
    public Int32 TestField;
}
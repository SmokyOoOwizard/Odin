using System;
using Odin.Abstractions.Components;

namespace Odin.Component.CodeGen.Sample;

public struct TestComponent : IComponent
{
    public int TestField { get; set; }

    public int CustomSetterGetter
    {
        get => TestField2;
        set => TestField2 = value;
    }

    [IndexBy]
    public Int32 TestField2;

    public long TestField22;
    public ulong TestField122;
    public float TestField3;
    public double TestField4;
    public bool TestField5;

    public string TestField6;

    public int[] TestArray;
}
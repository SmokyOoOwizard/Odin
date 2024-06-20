using System;
using System.Collections.Generic;
using Odin.Abstractions.Components;

namespace Odin.Component.CodeGen.Sample;

public struct TestComponent : IComponent
{
    public int TestField { get; set; }

    [IndexBy]
    public Int32 TestField2;

    public long TestField22;
    public ulong TestField122;
    public float TestField3;
    public double TestField4;
    public bool TestField5;

    public string TestField6;

    public int[] TestArray;
    public List<int> TestList;
    public Dictionary<int, int> TestDictionary;

    public TestInterComplexStruct ComplexField;
}

public struct TestInterComplexStruct
{
    public int TestField;
    public string TestField2;
}
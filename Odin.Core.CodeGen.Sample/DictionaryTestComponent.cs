using System.Collections.Immutable;
using Odin.Core.Abstractions.Components;

namespace Odin.Core.CodeGen.Sample;

public struct DictionaryTestComponent : IComponent
{
    public ImmutableDictionary<int, string> TestDictionary;
}
using Odin.Core.Contexts.Impl;
using Odin.Tests.Abstractions.Contexts;

namespace Odin.Core.Tests.Contexts;

public class InMemoryMultiContextChangesTests : AMultiContextChangesTests
{
    public InMemoryMultiContextChangesTests() :
        base(
            new InMemoryEntityContext(nameof(InMemoryMultiContextChangesTests)),
            new InMemoryEntityContext(nameof(InMemoryMultiContextChangesTests) + "2")
        )
    {
    }
}
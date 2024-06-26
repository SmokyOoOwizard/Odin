using Odin.Tests.Abstractions.Contexts;
using OdinSdk.Contexts;

namespace OdinSdk.Tests.Contexts;

public class InMemoryMultiContextChangesTests : AMultiContextChangesTests
{
    public InMemoryMultiContextChangesTests() :
        base(
            new InMemoryEntityContext(nameof(InMemoryContextChangesTests)),
            new InMemoryEntityContext(nameof(InMemoryContextChangesTests) + "2")
        )
    {
    }
}
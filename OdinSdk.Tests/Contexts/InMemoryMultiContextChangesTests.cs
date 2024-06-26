using Odin.Tests.Abstractions.Contexts;
using OdinSdk.Contexts;

namespace OdinSdk.Tests.Contexts;

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
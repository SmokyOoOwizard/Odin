using Odin.Tests.Abstractions.Contexts;
using OdinSdk.Contexts;

namespace OdinSdk.Tests.Contexts;

public class InMemoryAContextChangesATests : AContextChangesTests
{
    public InMemoryAContextChangesATests() : base(new InMemoryEntityContext(nameof(InMemoryAContextChangesATests)))
    {
    }
}
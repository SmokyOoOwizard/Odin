using Odin.Tests.Abstractions.Contexts;
using OdinSdk.Contexts;

namespace OdinSdk.Tests.Contexts;

public class InMemoryContextChangesATests : AContextChangesTests
{
    public InMemoryContextChangesATests() : base(new InMemoryEntityContext(nameof(InMemoryContextChangesATests)))
    {
    }
}
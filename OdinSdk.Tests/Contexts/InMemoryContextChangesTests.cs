using Odin.Tests.Abstractions.Contexts;
using OdinSdk.Contexts;

namespace OdinSdk.Tests.Contexts;

public class InMemoryContextChangesTests : AContextChangesTests
{
    public InMemoryContextChangesTests() : base(new InMemoryEntityContext(nameof(InMemoryContextChangesTests)))
    {
    }
}
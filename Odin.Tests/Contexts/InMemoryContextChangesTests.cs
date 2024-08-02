using Odin.Contexts;
using Odin.Tests.Abstractions.Contexts;

namespace Odin.Tests.Contexts;

public class InMemoryContextChangesTests : AContextChangesTests
{
    public InMemoryContextChangesTests() : base(new InMemoryEntityContext(nameof(InMemoryContextChangesTests)))
    {
    }
}
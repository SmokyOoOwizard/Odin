using Odin.Core.Contexts.Impl;
using Odin.Tests.Abstractions.Contexts;

namespace Odin.Core.Tests.Contexts;

public class InMemoryContextChangesTests : AContextChangesTests
{
    public InMemoryContextChangesTests() : base(new InMemoryEntityContext(nameof(InMemoryContextChangesTests)))
    {
    }
}
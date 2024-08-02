using Odin.Contexts;
using Odin.Tests.Abstractions.Entities;

namespace Odin.Tests;

public class InMemoryContextCollectionFilterTests : AEntityCollectionFilterTests
{
    public InMemoryContextCollectionFilterTests() : base(new InMemoryEntityContext(nameof(InMemoryContextCollectionFilterTests)))
    {
    }
}
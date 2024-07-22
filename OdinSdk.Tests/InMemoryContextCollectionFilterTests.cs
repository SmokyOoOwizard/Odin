using Odin.Tests.Abstractions.Entities;
using OdinSdk.Contexts;

namespace OdinSdk.Tests;

public class InMemoryContextCollectionFilterTests : AEntityCollectionFilterTests
{
    public InMemoryContextCollectionFilterTests() : base(new InMemoryEntityContext(nameof(InMemoryContextCollectionFilterTests)))
    {
    }
}
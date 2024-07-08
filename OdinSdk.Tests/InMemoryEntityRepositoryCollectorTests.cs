using Odin.Tests.Abstractions.Entities;
using OdinSdk.Contexts;

namespace OdinSdk.Tests;

public class InMemoryContextCollectorTests : AEntityRepositoryCollectorTests
{
    public InMemoryContextCollectorTests() : base(new InMemoryEntityContext(nameof(InMemoryContextCollectorTests)))
    {
    }
}
using Odin.Contexts;
using Odin.Tests.Abstractions.Entities;

namespace Odin.Tests;

public class InMemoryContextCollectorTests : AEntityRepositoryCollectorTests
{
    public InMemoryContextCollectorTests() : base(new InMemoryEntityContext(nameof(InMemoryContextCollectorTests)))
    {
    }
}
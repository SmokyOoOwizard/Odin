using Odin.Core.Contexts.Impl;
using Odin.Tests.Abstractions.Entities;

namespace Odin.Core.Tests;

public class InMemoryContextCollectorTests : AEntityRepositoryCollectorTests
{
    public InMemoryContextCollectorTests() : base(new InMemoryEntityContext(nameof(InMemoryContextCollectorTests)))
    {
    }
}
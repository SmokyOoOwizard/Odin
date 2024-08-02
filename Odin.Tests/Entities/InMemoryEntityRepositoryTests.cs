using Odin.Entities.Repository.Impl;
using Odin.Tests.Abstractions.Entities;

namespace Odin.Tests.Entities;

public class InMemoryEntityRepositoryTests : AEntityRepositoryTests
{
    public InMemoryEntityRepositoryTests() : base(
        new InMemoryEntitiesRepository(
            (ulong)typeof(InMemoryEntityRepositoryTests).GetHashCode()
        )
    )
    {
    }
}
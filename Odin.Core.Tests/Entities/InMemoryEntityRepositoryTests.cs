using Odin.Core.Repositories.Entities.Impl;
using Odin.Tests.Abstractions.Entities;

namespace Odin.Core.Tests.Entities;

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
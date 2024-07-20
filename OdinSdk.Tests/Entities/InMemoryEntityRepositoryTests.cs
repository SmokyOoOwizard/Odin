using Odin.Tests.Abstractions.Entities;
using OdinSdk.Entities;
using OdinSdk.Entities.Repository.Impl;

namespace OdinSdk.Tests.Entities;

public class InMemoryEntityRepositoryTests : AEntityRepositoryTests
{
    public InMemoryEntityRepositoryTests() : base(new InMemoryEntitiesRepository())
    {
    }
}
using Odin.Tests.Abstractions.Entities;
using OdinSdk.Entities;

namespace OdinSdk.Tests;

public class InMemoryEntityRepositoryCollectorTests : AEntityRepositoryCollectorTests
{
    public InMemoryEntityRepositoryCollectorTests() : base(new InMemoryEntityRepository())
    {
    }
}
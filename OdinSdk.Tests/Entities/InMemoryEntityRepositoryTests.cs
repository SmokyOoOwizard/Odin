using Odin.Tests.Abstractions.Entities;
using OdinSdk.Entities;

namespace OdinSdk.Tests.Entities;

public class InMemoryEntityRepositoryTests : AEntityRepositoryTests
{
    public InMemoryEntityRepositoryTests() : base(new InMemoryEntityRepository())
    {
    }
}
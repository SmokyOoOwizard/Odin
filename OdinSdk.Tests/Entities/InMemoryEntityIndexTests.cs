using Odin.Tests.Abstractions.Entities;
using OdinSdk.Contexts;

namespace OdinSdk.Tests.Entities;

public class InMemoryEntityIndexTests : AEntityIndexTests
{
    public InMemoryEntityIndexTests() : base(
        new InMemoryEntityContext(nameof(InMemoryEntityIndexTests))
    )
    {
    }
}
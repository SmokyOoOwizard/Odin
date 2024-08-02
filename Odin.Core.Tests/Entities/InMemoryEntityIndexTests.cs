using Odin.Contexts;
using Odin.Tests.Abstractions.Entities;

namespace Odin.Tests.Entities;

public class InMemoryEntityIndexTests : AEntityIndexTests
{
    public InMemoryEntityIndexTests() : base(
        new InMemoryEntityContext(nameof(InMemoryEntityIndexTests))
    )
    {
    }
}
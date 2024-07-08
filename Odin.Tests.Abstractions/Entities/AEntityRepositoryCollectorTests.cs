using Odin.Abstractions.Collectors.Matcher;
using Odin.Abstractions.Components;
using Odin.Abstractions.Entities;
using Xunit;

namespace Odin.Tests.Abstractions.Entities;

public abstract class AEntityRepositoryCollectorTests
{
    private readonly IEntityRepository _repository;

    protected AEntityRepositoryCollectorTests(IEntityRepository repository)
    {
        _repository = repository;
    }

    public struct TestComponent : IComponent
    {
        public ulong TestData;
    }

    public class HasMatcher : AComponentMatcher
    {
        public override void Configure()
        {
            Filter().Has<TestComponent>();
        }
    }

    public class NotMatcher : AComponentMatcher
    {
        public override void Configure()
        {
            Filter().Has<TestComponent>();
        }
    }

    [Fact]
    public void HasMatcherTest()
    {
        var collector = _repository.CreateCollector<HasMatcher>("Test");
        _repository.CreateEntity();
        var entity = _repository.CreateEntity();

        _repository.Replace(entity, new TestComponent());

        var entities = collector.GetBatch()
                                .GetEntities()
                                .ToArray();

        Assert.Equal(1, entities.Length);
    }
}
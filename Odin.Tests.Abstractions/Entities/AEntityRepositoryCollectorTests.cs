using Odin.Abstractions.Collectors.Matcher;
using Odin.Abstractions.Components;
using Odin.Abstractions.Contexts;
using OdinSdk.Components;
using OdinSdk.Contexts;
using Xunit;

namespace Odin.Tests.Abstractions.Entities;

public abstract class AEntityRepositoryCollectorTests : ATestsWithContext
{
    protected AEntityRepositoryCollectorTests(AEntityContext context) : base(context)
    {
    }

    public struct TestComponent : IComponent
    {
        public ulong TestData;
    }

    public struct TestComponent2 : IComponent
    {
        public ulong TestData;
    }
    
    public struct TestComponent3 : IComponent
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

    public class NotHasMatcher : AComponentMatcher
    {
        public override void Configure()
        {
            Filter().NotHas<TestComponent>();
        }
    }

    public class AllHasMatcher : AComponentMatcher
    {
        public override void Configure()
        {
            Filter().Has<TestComponent>().Has<TestComponent2>();
        }
    }

    public class AnyHasMatcher : AComponentMatcher
    {
        public override void Configure()
        {
            Filter().Any(c => c.Has<TestComponent>(),
                         c => c.Has<TestComponent2>());
        }
    }
    
    public class NotMatcher : AComponentMatcher
    {
        public override void Configure()
        {
            Filter().Not(c => c.Has<TestComponent>(),
                         c => c.Has<TestComponent2>());
        }
    }
    
    public class AddedMatcher : AComponentMatcher
    {
        public override void Configure()
        {
            Filter().Added<TestComponent>();
        }
    }
    
    public class RemovedMatcher : AComponentMatcher
    {
        public override void Configure()
        {
            Filter().Removed<TestComponent>();
        }
    }
    
    public class AnyChangesMatcher : AComponentMatcher
    {
        public override void Configure()
        {
            Filter().AnyChanges<TestComponent>();
        }
    }

    [Fact]
    public void HasMatcherTest()
    {
        var collector = Context.CreateCollector<HasMatcher>("Test");
        
        Context.CreateEntity();
        var entity = Context.CreateEntity();
        var entity2 = Context.CreateEntity();

        entity.Replace(new TestComponent());
        entity2.Replace(new TestComponent2());

        EntityContexts.Save();

        var entities = collector.GetBatch()
                                .GetEntities()
                                .ToArray();

        Assert.Equal(1, entities.Length);
    }

    [Fact]
    public void HasMatcherTriggeredByNotSameComponentTest()
    {
        Context.CreateEntity();
        var entity = Context.CreateEntity();
        var entity2 = Context.CreateEntity();

        entity.Replace(new TestComponent());
        entity2.Replace(new TestComponent2());

        EntityContexts.Save();
        var collector = Context.CreateCollector<HasMatcher>("Test");

        entity2.Replace(new TestComponent());
        EntityContexts.Save();

        var entities = collector.GetBatch()
                                .GetEntities()
                                .ToArray();

        Assert.Equal(1, entities.Length);
    }

    [Fact]
    public void NotHasMatcherTest()
    {
        var collector = Context.CreateCollector<NotHasMatcher>("Test");
        
        var entity = Context.CreateEntity();
        var entity2 = Context.CreateEntity();

        entity.Replace(new TestComponent());
        entity2.Replace(new TestComponent2());

        EntityContexts.Save();

        var entities = collector.GetBatch()
                                .GetEntities()
                                .ToArray();

        Assert.Equal(1, entities.Length);
    }
    
    [Fact]
    public void AllMatcherTest()
    {
        var collector = Context.CreateCollector<AllHasMatcher>("Test");
        
        var entity = Context.CreateEntity();
        var entity2 = Context.CreateEntity();

        entity.Replace(new TestComponent());
        entity2.Replace(new TestComponent2());
        entity2.Replace(new TestComponent());

        EntityContexts.Save();

        var entities = collector.GetBatch()
                                .GetEntities()
                                .ToArray();

        Assert.Equal(1, entities.Length);
    }
    
    [Fact]
    public void AnyMatcherTest()
    {
        var collector = Context.CreateCollector<AnyHasMatcher>("Test");
        
        var entity = Context.CreateEntity();
        var entity2 = Context.CreateEntity();

        entity.Replace(new TestComponent());
        entity2.Replace(new TestComponent2());

        EntityContexts.Save();

        var entities = collector.GetBatch()
                                .GetEntities()
                                .ToArray();

        Assert.Equal(2, entities.Length);
    }
    
    [Fact]
    public void NotMatcherTest()
    {
        var collector = Context.CreateCollector<NotMatcher>("Test");
        
        var entity = Context.CreateEntity();
        var entity2 = Context.CreateEntity();
        var entity3 = Context.CreateEntity();

        entity.Replace(new TestComponent());
        entity2.Replace(new TestComponent2());
        entity3.Replace(new TestComponent3());

        EntityContexts.Save();

        var entities = collector.GetBatch()
                                .GetEntities()
                                .ToArray();

        Assert.Equal(1, entities.Length);
    }
    
    [Fact]
    public void AddedMatcherTest()
    {
        
        var entity = Context.CreateEntity();
        var entity2 = Context.CreateEntity();
        var entity3 = Context.CreateEntity();

        entity.Replace(new TestComponent());
        entity2.Replace(new TestComponent2());
        entity3.Replace(new TestComponent3());

        EntityContexts.Save();
        
        var collector = Context.CreateCollector<AddedMatcher>("Test");
        
        entity2.Replace(new TestComponent());
        entity.Remove<TestComponent>();
        
        EntityContexts.Save();

        var entities = collector.GetBatch()
                                .GetEntities()
                                .ToArray();

        Assert.Equal(1, entities.Length);
    }
    
    [Fact]
    public void RemovedMatcherTest()
    {
        
        var entity = Context.CreateEntity();
        var entity2 = Context.CreateEntity();
        var entity3 = Context.CreateEntity();

        entity.Replace(new TestComponent());
        entity2.Replace(new TestComponent2());
        entity3.Replace(new TestComponent3());

        EntityContexts.Save();
        
        var collector = Context.CreateCollector<RemovedMatcher>("Test");
        
        entity2.Replace(new TestComponent());
        entity.Remove<TestComponent>();
        
        EntityContexts.Save();

        var entities = collector.GetBatch()
                                .GetEntities()
                                .ToArray();

        Assert.Equal(1, entities.Length);
    }
    
    [Fact]
    public void AnyChangesMatcherTest()
    {
        
        var entity = Context.CreateEntity();
        var entity2 = Context.CreateEntity();
        var entity3 = Context.CreateEntity();

        entity.Replace(new TestComponent());
        entity2.Replace(new TestComponent2());
        entity3.Replace(new TestComponent3());

        EntityContexts.Save();
        
        var collector = Context.CreateCollector<AnyChangesMatcher>("Test");
        
        entity2.Replace(new TestComponent());
        entity.Remove<TestComponent>();
        
        EntityContexts.Save();

        var entities = collector.GetBatch()
                                .GetEntities()
                                .ToArray();

        Assert.Equal(2, entities.Length);
    }
}
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

    public struct Component : IComponent
    {
        public ulong TestData;
    }

    public struct Component2 : IComponent
    {
        public ulong TestData;
    }

    public struct Component3 : IComponent
    {
        public ulong TestData;
    }

    public class HasMatcher : AComponentMatcher
    {
        public override void Configure()
        {
            Filter().Has<Component>();
        }
    }

    public class NotHasMatcher : AComponentMatcher
    {
        public override void Configure()
        {
            Filter().NotHas<Component>();
        }
    }

    public class AllHasMatcher : AComponentMatcher
    {
        public override void Configure()
        {
            Filter().Has<Component>().Has<Component2>();
        }
    }

    public class AnyHasMatcher : AComponentMatcher
    {
        public override void Configure()
        {
            Filter().Any(c => c.Has<Component>(),
                         c => c.Has<Component2>());
        }
    }

    public class NotMatcher : AComponentMatcher
    {
        public override void Configure()
        {
            Filter().Not(c => c.Has<Component>(),
                         c => c.Has<Component2>());
        }
    }

    public class AddedMatcher : AComponentMatcher
    {
        public override void Configure()
        {
            Filter().Added<Component>();
        }
    }

    public class RemovedMatcher : AComponentMatcher
    {
        public override void Configure()
        {
            Filter().Removed<Component>();
        }
    }

    public class AnyChangesMatcher : AComponentMatcher
    {
        public override void Configure()
        {
            Filter().AnyChanges<Component>();
        }
    }

    public class ComplexMatcher : AComponentMatcher
    {
        public override void Configure()
        {
            Filter().Any(
                c => c.Has<Component3>().Added<Component>(),
                c => c.Has<Component2>().NotHas<Component3>().Removed<Component>()
            );
        }
    }
    
    [Fact]
    public void DisableMatcherTest()
    {
        var collector = Context.CreateCollector<HasMatcher>("Test");

        var entity = Context.CreateEntity();
        var entity2 = Context.CreateEntity();
        var entity3 = Context.CreateEntity();

        entity.Replace(new Component());
        entity2.Replace(new Component2());

        EntityContexts.Save();
        
        Context.DisableCollector("Test");
        
        entity3.Replace(new Component());
        
        EntityContexts.Save();

        var entities = collector.GetBatch()
                                .GetEntities()
                                .ToArray();

        Assert.Equal(1, entities.Length);
    }

    [Fact]
    public void DeleteMatcherTest()
    {
        var collector = Context.CreateCollector<HasMatcher>("Test");

        var entity = Context.CreateEntity();
        var entity2 = Context.CreateEntity();
        var entity3 = Context.CreateEntity();

        entity.Replace(new Component());
        entity2.Replace(new Component2());

        EntityContexts.Save();
        
        Context.DeleteCollector("Test");
        
        entity3.Replace(new Component());
        
        EntityContexts.Save();

        var entities = collector.GetBatch()
                                .GetEntities()
                                .ToArray();

        Assert.Equal(0, entities.Length);
    }
    
    [Fact]
    public void HasMatcherTest()
    {
        var collector = Context.CreateCollector<HasMatcher>("Test");

        Context.CreateEntity();
        var entity = Context.CreateEntity();
        var entity2 = Context.CreateEntity();

        entity.Replace(new Component());
        entity2.Replace(new Component2());

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

        entity.Replace(new Component());
        entity2.Replace(new Component2());

        EntityContexts.Save();
        var collector = Context.CreateCollector<HasMatcher>("Test");

        entity2.Replace(new Component());
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

        entity.Replace(new Component());
        entity2.Replace(new Component2());

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

        entity.Replace(new Component());
        entity2.Replace(new Component2());
        entity2.Replace(new Component());

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

        entity.Replace(new Component());
        entity2.Replace(new Component2());

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

        entity.Replace(new Component());
        entity2.Replace(new Component2());
        entity3.Replace(new Component3());

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

        entity.Replace(new Component());
        entity2.Replace(new Component2());
        entity3.Replace(new Component3());

        EntityContexts.Save();

        var collector = Context.CreateCollector<AddedMatcher>("Test");

        entity2.Replace(new Component());
        entity.Remove<Component>();

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

        entity.Replace(new Component());
        entity2.Replace(new Component2());
        entity3.Replace(new Component3());

        EntityContexts.Save();

        var collector = Context.CreateCollector<RemovedMatcher>("Test");

        entity2.Replace(new Component());
        entity.Remove<Component>();

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

        entity.Replace(new Component());
        entity2.Replace(new Component2());
        entity3.Replace(new Component3());

        EntityContexts.Save();

        var collector = Context.CreateCollector<AnyChangesMatcher>("Test");

        entity2.Replace(new Component());
        entity.Remove<Component>();

        EntityContexts.Save();

        var entities = collector.GetBatch()
                                .GetEntities()
                                .ToArray();

        Assert.Equal(2, entities.Length);
    }

    [Fact]
    public void ComplexMatcherTest()
    {
        var entity = Context.CreateEntity();
        var entity2 = Context.CreateEntity();
        var entity3 = Context.CreateEntity();

        entity.Replace(new Component3());
        entity2.Replace(new Component());
        entity2.Replace(new Component2());
        entity3.Replace(new Component3());

        EntityContexts.Save();

        var collector = Context.CreateCollector<ComplexMatcher>("Test");

        entity.Replace(new Component());
        entity2.Remove<Component>();

        EntityContexts.Save();

        var entities = collector.GetBatch()
                                .GetEntities()
                                .ToArray();

        Assert.Equal(2, entities.Length);
    }
}
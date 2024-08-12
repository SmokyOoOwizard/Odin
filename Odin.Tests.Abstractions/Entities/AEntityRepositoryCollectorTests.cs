using Odin.Core.Abstractions.Components;
using Odin.Core.Abstractions.Matchers;
using Odin.Core.Components;
using Odin.Core.Contexts;
using Odin.Core.Matchers.Extensions;
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

    public struct Component4 : IComponent
    {
        public ulong TestData;
    }

    public class HasMatcher : AReactiveComponentMatcher
    {
        public override void Configure()
        {
            Filter().Has<Component>();
        }
    }

    public class NotHasMatcher : AReactiveComponentMatcher
    {
        public override void Configure()
        {
            Filter().NotHas<Component>();
        }
    }

    public class AllHasMatcher : AReactiveComponentMatcher
    {
        public override void Configure()
        {
            Filter().Has<Component>().Has<Component2>();
        }
    }

    public class AnyHasMatcher : AReactiveComponentMatcher
    {
        public override void Configure()
        {
            Filter().Any(c => c.Has<Component>(),
                         c => c.Has<Component2>());
        }
    }

    public class NotMatcher : AReactiveComponentMatcher
    {
        public override void Configure()
        {
            Filter().Not(c => c.Has<Component>(),
                         c => c.Has<Component2>());
        }
    }

    public class AddedMatcher : AReactiveComponentMatcher
    {
        public override void Configure()
        {
            Filter().Added<Component>();
        }
    }

    public class RemovedMatcher : AReactiveComponentMatcher
    {
        public override void Configure()
        {
            Filter().Removed<Component>();
        }
    }

    public class AnyChangesMatcher : AReactiveComponentMatcher
    {
        public override void Configure()
        {
            Filter().AnyChanges<Component>();
        }
    }

    public class ComplexMatcher : AReactiveComponentMatcher
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
        using var collector = Context.CreateCollector<HasMatcher>("Test");

        var entity = Context.CreateEntity();
        var entity2 = Context.CreateEntity();
        var entity3 = Context.CreateEntity();

        entity.Replace(new Component());
        entity2.Replace(new Component2());

        EntityContexts.Save();

        Context.DisableCollector("Test");

        entity3.Replace(new Component());

        EntityContexts.Save();

        var entities = collector.GetEntities()
            .ToArray();

        Assert.Equal(1, entities.Length);
    }

    [Fact]
    public void DeleteMatcherTest()
    {
        using var collector = Context.CreateCollector<HasMatcher>("Test");

        var entity = Context.CreateEntity();
        var entity2 = Context.CreateEntity();
        var entity3 = Context.CreateEntity();

        entity.Replace(new Component());
        entity2.Replace(new Component2());

        EntityContexts.Save();

        Context.DeleteCollector("Test");

        entity3.Replace(new Component());

        EntityContexts.Save();

        var entities = collector.GetEntities()
            .ToArray();

        Assert.Equal(0, entities.Length);
    }


    [Fact]
    public void GetAlreadyCreatedMatcherTest()
    {
        Context.CreateCollector<HasMatcher>("Test");

        var entity = Context.CreateEntity();
        var entity2 = Context.CreateEntity();
        var entity3 = Context.CreateEntity();

        entity.Replace(new Component());
        entity2.Replace(new Component2());

        EntityContexts.Save();

        Context.DeleteCollector("Test");

        entity3.Replace(new Component());

        EntityContexts.Save();

        using var collector = Context.CreateCollector<HasMatcher>("Test");

        var entities = collector.GetEntities()
            .ToArray();

        Assert.Equal(0, entities.Length);
    }


    [Fact]
    public void HasMatcherTest()
    {
        Context.CreateEntity();
        var entity = Context.CreateEntity();
        var entity2 = Context.CreateEntity();

        entity.Replace(new Component());
        entity2.Replace(new Component2());

        EntityContexts.Save();

        using var collector = Context.CreateCollector<HasMatcher>("Test");
        entity.Replace(new Component2());
        entity2.Replace(new Component3());

        EntityContexts.Save();

        var entities = collector.GetEntities()
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
        using var collector = Context.CreateCollector<HasMatcher>("Test");

        entity2.Replace(new Component());
        entity.Replace(new Component2());

        EntityContexts.Save();

        var entities = collector.GetEntities()
            .ToArray();

        Assert.Equal(1, entities.Length);
    }

    [Fact]
    public void NotHasMatcherTest()
    {
        var entity = Context.CreateEntity();
        var entity2 = Context.CreateEntity();

        entity.Replace(new Component());
        entity2.Replace(new Component2());

        EntityContexts.Save();

        using var collector = Context.CreateCollector<NotHasMatcher>("Test");

        entity.Replace(new Component2());
        entity2.Replace(new Component3());

        EntityContexts.Save();

        var entities = collector.GetEntities()
            .ToArray();

        Assert.Equal(1, entities.Length);
    }

    [Fact]
    public void AllMatcherTest()
    {
        using var collector = Context.CreateCollector<AllHasMatcher>("Test");

        var entity = Context.CreateEntity();
        var entity2 = Context.CreateEntity();

        entity.Replace(new Component());
        entity2.Replace(new Component2());
        entity2.Replace(new Component());

        EntityContexts.Save();

        entity.Replace(new Component4());
        entity2.Replace(new Component4());
        entity2.Replace(new Component4());

        EntityContexts.Save();

        var entities = collector.GetEntities()
            .ToArray();

        Assert.Equal(1, entities.Length);
    }

    [Fact]
    public void AnyMatcherTest()
    {
        using var collector = Context.CreateCollector<AnyHasMatcher>("Test");

        var entity = Context.CreateEntity();
        var entity2 = Context.CreateEntity();

        entity.Replace(new Component());
        entity2.Replace(new Component2());

        EntityContexts.Save();

        entity.Replace(new Component4());
        entity2.Replace(new Component4());

        EntityContexts.Save();

        var entities = collector.GetEntities()
            .ToArray();

        Assert.Equal(2, entities.Length);
    }

    [Fact]
    public void NotMatcherTest()
    {
        var entity = Context.CreateEntity();
        var entity2 = Context.CreateEntity();
        var entity3 = Context.CreateEntity();

        entity.Replace(new Component());
        entity2.Replace(new Component2());
        entity3.Replace(new Component3());

        EntityContexts.Save();

        using var collector = Context.CreateCollector<NotMatcher>("Test");

        entity.Replace(new Component3());
        entity2.Replace(new Component3());
        entity3.Replace(new Component4());

        EntityContexts.Save();

        var entities = collector.GetEntities()
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

        using var collector = Context.CreateCollector<AddedMatcher>("Test");

        entity2.Replace(new Component());
        entity.Remove<Component>();

        EntityContexts.Save();

        var entities = collector.GetEntities()
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

        using var collector = Context.CreateCollector<RemovedMatcher>("Test");

        entity2.Replace(new Component());
        entity.Remove<Component>();

        EntityContexts.Save();

        var entities = collector.GetEntities()
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

        using var collector = Context.CreateCollector<AnyChangesMatcher>("Test");

        entity2.Replace(new Component());
        entity.Remove<Component>();

        EntityContexts.Save();

        var entities = collector.GetEntities()
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

        using var collector = Context.CreateCollector<ComplexMatcher>("Test");

        entity.Replace(new Component());
        entity2.Remove<Component>();

        EntityContexts.Save();

        var entities = collector.GetEntities()
            .ToArray();

        Assert.Equal(2, entities.Length);
    }

    [Fact]
    public void FewCollectorsWithSameMatcherTest()
    {
        using var collector = Context.CreateCollector<HasMatcher>("Test");
        using var collector2 = Context.CreateCollector<HasMatcher>("Test2");

        Context.CreateEntity();
        var entity = Context.CreateEntity();
        var entity2 = Context.CreateEntity();

        entity.Replace(new Component());
        entity2.Replace(new Component4());

        EntityContexts.Save();

        entity.Replace(new Component4());
        entity2.Replace(new Component2());

        EntityContexts.Save();

        var entities = collector.GetEntities()
            .ToArray();

        var entities2 = collector2.GetEntities()
            .ToArray();

        Assert.Equal(1, entities.Length);
        Assert.Equal(1, entities2.Length);
    }
}
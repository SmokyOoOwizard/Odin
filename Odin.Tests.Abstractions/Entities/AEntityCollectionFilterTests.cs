using Odin.Abstractions.Collectors.Matcher;
using Odin.Abstractions.Collectors.Matcher.Extensions;
using Odin.Abstractions.Components;
using Odin.Abstractions.Contexts;
using OdinSdk.Components;
using OdinSdk.Contexts;
using Xunit;

namespace Odin.Tests.Abstractions.Entities;

public abstract class AEntityCollectionFilterTests : ATestsWithContext
{
    protected AEntityCollectionFilterTests(AEntityContext context) : base(context)
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

    public class ComplexMatcher : AComponentMatcher
    {
        public override void Configure()
        {
            Filter().Any(
                c => c.Has<Component3>(),
                c => c.Has<Component2>().NotHas<Component3>()
            );
        }
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

        var group = Context.GetEntities().Filter<HasMatcher>().Build();

        var entities = group.ToArray();

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

        var collector = Context.GetEntities().Filter<NotHasMatcher>().Build();

        var entities = collector.ToArray();

        Assert.Equal(1, entities.Length);
    }

    [Fact]
    public void AllMatcherTest()
    {
        var entity = Context.CreateEntity();
        var entity2 = Context.CreateEntity();

        entity.Replace(new Component());
        entity2.Replace(new Component2());
        entity2.Replace(new Component());

        EntityContexts.Save();
        var collector = Context.GetEntities().Filter<AllHasMatcher>().Build();

        var entities = collector.ToArray();

        Assert.Equal(1, entities.Length);
    }

    [Fact]
    public void AnyMatcherTest()
    {
        var entity = Context.CreateEntity();
        var entity2 = Context.CreateEntity();

        entity.Replace(new Component());
        entity2.Replace(new Component2());

        EntityContexts.Save();

        var collector = Context.GetEntities().Filter<AnyHasMatcher>().Build();

        var entities = collector.ToArray();

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

        var collector = Context.GetEntities().Filter<NotMatcher>().Build();

        var entities = collector.ToArray();

        Assert.Equal(1, entities.Length);
    }

    [Fact]
    public void ComplexMatcherTest()
    {
        var entity = Context.CreateEntity();
        var entity2 = Context.CreateEntity();
        var entity3 = Context.CreateEntity();
        var entity4 = Context.CreateEntity();
        
        entity.Replace(new Component3());
        entity2.Replace(new Component());
        entity2.Replace(new Component2());
        entity3.Replace(new Component3());
        entity4.Replace(new Component4());

        EntityContexts.Save();

        var collector = Context.GetEntities().Filter<ComplexMatcher>().Build();

        var entities = collector.ToArray();

        Assert.Equal(3, entities.Length);
    }
}
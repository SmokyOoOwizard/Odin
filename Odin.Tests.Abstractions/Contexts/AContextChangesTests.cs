using Odin.Abstractions.Contexts;
using OdinSdk.Components;
using OdinSdk.Contexts;
using Xunit;

namespace Odin.Tests.Abstractions.Contexts;

public abstract class AContextChangesTests : ATestsWithContext
{
    protected AContextChangesTests(AEntityContext context) : base(context)
    {
    }

    [Fact]
    public void CreateEntity()
    {
        var entity = Context.CreateEntity();
        EntityContexts.Save();

        var id = entity.Id;

        Assert.True(id.Id > 0);
        Assert.True(id.ContextId > 0);

        var entities = Context.GetEntities().ToArray();

        Assert.Single(entities);
        Assert.Equal(entity.Id, entities[0].Id);
    }

    [Fact]
    public void DeleteEntity()
    {
        var entity = Context.CreateEntity();

        EntityContexts.Save();

        entity.Destroy();

        EntityContexts.Save();

        var entities = Context.GetEntities().ToArray();

        Assert.Empty(entities);
    }
}
using Odin.Core.Components;
using Odin.Core.Contexts;
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
    public void CreateManyEntities()
    {
        for (int i = 0; i < 10; i++)
        {
            Context.CreateEntity();
            EntityContexts.Save();
        }

        var entities = Context.GetEntities().ToArray();
        Assert.Equal(10, entities.Length);

        for (int i = 0; i < 10; i++)
        {
            Context.CreateEntity();
        }

        EntityContexts.Save();
        var entities2 = Context.GetEntities().ToArray();
        Assert.Equal(20, entities2.Length);
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

    [Fact]
    public void ClearContext()
    {
        Context.CreateEntity();
        Context.CreateEntity();

        EntityContexts.Save();

        var entities = Context.GetEntities().ToArray();

        Assert.Equal(2, entities.Length);

        Context.Clear();

        var newEntities = Context.GetEntities().ToArray();

        Assert.Empty(newEntities);
    }
}
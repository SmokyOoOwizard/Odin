using OdinSdk.Components;
using OdinSdk.Contexts;

namespace OdinSdk.Tests.Contexts;

public class ContextChanges : TestsWithContext
{
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
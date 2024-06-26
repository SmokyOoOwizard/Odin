using Odin.Abstractions.Contexts;
using OdinSdk.Components;
using OdinSdk.Contexts;
using Xunit;

namespace Odin.Tests.Abstractions.Contexts;

public abstract class AMultiContextChangesTests : IDisposable
{
    private readonly AEntityContext _firstContext;
    private readonly AEntityContext _secondContext;

    protected AMultiContextChangesTests(AEntityContext firstContext, AEntityContext secondContext)
    {
        _firstContext = firstContext;
        _secondContext = secondContext;
    }

    [Fact]
    public void CreateEntity()
    {
        var entity = _firstContext.CreateEntity();
        EntityContexts.Save();

        var id = entity.Id;

        Assert.True(id.Id > 0);
        Assert.True(id.ContextId > 0);

        var fEntities = _firstContext.GetEntities().ToArray();

        Assert.Single(fEntities);
        Assert.Equal(entity.Id, fEntities[0].Id);

        var sEntities = _secondContext.GetEntities().ToArray();

        Assert.Empty(sEntities);
    }

    [Fact]
    public void DeleteEntity()
    {
        var fEntity = _firstContext.CreateEntity();
        _secondContext.CreateEntity();

        EntityContexts.Save();

        var fEntities = _firstContext.GetEntities().ToArray();

        Assert.Single(fEntities);

        var sEntities = _secondContext.GetEntities().ToArray();

        Assert.Single(sEntities);

        fEntity.Destroy();

        EntityContexts.Save();

        var fEntities2 = _firstContext.GetEntities().ToArray();

        Assert.Empty(fEntities2);

        var sEntities2 = _secondContext.GetEntities().ToArray();

        Assert.Single(sEntities2);
    }

    [Fact]
    public void ClearContext()
    {
        _firstContext.CreateEntity();
        _firstContext.CreateEntity();
        _secondContext.CreateEntity();

        EntityContexts.Save();

        var entities = _firstContext.GetEntities().ToArray();

        Assert.Equal(2, entities.Length);

        _firstContext.Clear();

        var fEntities =_firstContext.GetEntities().ToArray();

        Assert.Empty(fEntities);
        
        var sEntities = _secondContext.GetEntities().ToArray();
        
        Assert.Single(sEntities);
    }

    public void Dispose()
    {
        _firstContext.Dispose();
        _secondContext.Dispose();
    }
}
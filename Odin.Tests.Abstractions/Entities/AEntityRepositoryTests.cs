using Odin.Abstractions.Components;
using Odin.Abstractions.Entities;
using OdinSdk.Contexts;
using Xunit;

namespace Odin.Tests.Abstractions.Entities;

public abstract class AEntityRepositoryTests
{
    private readonly IEntityRepository _repository;

    protected AEntityRepositoryTests(IEntityRepository repository)
    {
        _repository = repository;
    }

    public struct TestComponent : IComponent
    {
        public ulong TestData;
    }

    [Fact]
    public void CreateComponent()
    {
        var entity = _repository.CreateEntity();

        _repository.Get<TestComponent>(entity, out var old);
        Assert.Equal(old, default);

        var component = new TestComponent
        {
            TestData = 123
        };

        _repository.Replace(entity, component);

        _repository.Get<TestComponent>(entity, out var newComponent);
        Assert.Equal(component, newComponent);
        Assert.Equal(newComponent.TestData, component.TestData);
    }

    [Fact]
    public void GetOldComponent()
    {
        var entity = _repository.CreateEntity();


        var component = new TestComponent
        {
            TestData = 123
        };

        _repository.Replace(entity, component);

        _repository.GetOld<TestComponent>(entity, out var old);
        Assert.Equal(old, default);

        var newComponent = new TestComponent() { TestData = 444 };
        _repository.Replace(entity, newComponent);

        EntityContexts.Save();

        _repository.Get<TestComponent>(entity, out var getNewComponent);
        Assert.Equal(getNewComponent, newComponent);

        _repository.GetOld(entity, out old);
        Assert.Equal(old, component);
    }

    [Fact]
    public void DeleteComponent()
    {
        Thread.Sleep(100);
        var entity = _repository.CreateEntity();

        var component = new TestComponent
        {
            TestData = 123
        };

        _repository.Replace(entity, component);


        _repository.Remove<TestComponent>(entity);

        Assert.False(_repository.Get<TestComponent>(entity, out _));

        _repository.Get<TestComponent>(entity, out var old);

        Assert.Equal(old, default);
    }

    [Fact]
    public void CreateEntity()
    {
        var entity = _repository.CreateEntity();

        var id = entity.Id.Id;

        Assert.True(id > 0);

        var entities = _repository.GetEntities().ToArray();

        Assert.Single(entities);
        Assert.Equal(entity, entities[0]);
    }

    [Fact]
    public void DeleteEntity()
    {
        var entity = _repository.CreateEntity();

        _repository.DestroyEntity(entity);

        var entities = _repository.GetEntities().ToArray();

        Assert.Empty(entities);
    }

    [Fact]
    public void ClearContext()
    {
        _repository.CreateEntity();
        _repository.CreateEntity();

        var entities = _repository.GetEntities().ToArray();

        Assert.Equal(2, entities.Length);

        _repository.Clear();

        var newEntities = _repository.GetEntities().ToArray();

        Assert.Empty(newEntities);
    }
}
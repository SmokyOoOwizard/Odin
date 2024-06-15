using OdinSdk.Components;
using OdinSdk.Contexts;

namespace OdinSdk.Tests.Components;

public class ComponentChanges : TestsWithContext
{
    private struct TestComponent : IComponent
    {
        public ulong TestData;
    }

    [Fact]
    public void CreateComponent()
    {
        var entity = Context.CreateEntity();
        EntityContexts.Save();

        var old = entity.Get<TestComponent>();
        Assert.Equal(old, default);

        var component = new TestComponent
        {
            TestData = 123
        };

        entity.Replace(component);

        EntityContexts.Save();

        var newComponent = entity.Get<TestComponent>();
        Assert.Equal(component, newComponent);
        Assert.Equal(newComponent.TestData, component.TestData);
    }
    
    [Fact]
    public void HasComponent()
    {
        var entity = Context.CreateEntity();
        EntityContexts.Save();
        
        Assert.False(entity.Has<TestComponent>());

        var component = new TestComponent
        {
            TestData = 123
        };

        entity.Replace(component);

        EntityContexts.Save();
        
        Assert.True(entity.Has<TestComponent>());
    }
    
    [Fact]
    public void GetOldComponent()
    {
        var entity = Context.CreateEntity();
        EntityContexts.Save();


        var component = new TestComponent
        {
            TestData = 123
        };

        entity.Replace(component);

        EntityContexts.Save();
        
        var old = entity.GetOld<TestComponent>();
        Assert.Equal(old, default);

        var newComponent = new TestComponent() { TestData = 444 };
        entity.Replace(newComponent);

        EntityContexts.Save();
        
        var getNewComponent = entity.Get<TestComponent>();
        Assert.Equal(getNewComponent, newComponent);
        
        old = entity.GetOld<TestComponent>();
        Assert.Equal(old, component);
    }

    [Fact]
    public void DeleteComponent()
    {
        Thread.Sleep(100);
        var entity = Context.CreateEntity();
        EntityContexts.Save();

        var component = new TestComponent
        {
            TestData = 123
        };

        entity.Replace(component);

        EntityContexts.Save();

        entity.Remove<TestComponent>();

        EntityContexts.Save();
        
        Assert.False(entity.Has<TestComponent>());

        var old = entity.Get<TestComponent>();
        
        Assert.Equal(old, default);
    }
}
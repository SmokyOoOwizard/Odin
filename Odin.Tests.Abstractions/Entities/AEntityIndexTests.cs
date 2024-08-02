using Odin.Abstractions.Components;
using Odin.Abstractions.Contexts;
using Odin.Components;
using Odin.Contexts;
using Xunit;

namespace Odin.Tests.Abstractions.Entities;

public abstract class AEntityIndexTests : ATestsWithContext
{
    protected AEntityIndexTests(AEntityContext context) : base(context)
    {
    }

    [IndexBy]
    public struct Component : IComponent
    {
        public ulong TestData;
        public ulong TestData2;
    }

    public struct Component2 : IComponent
    {
        [IndexBy]
        public ulong TestData;
        
        [IndexBy]
        public ulong TestData2;
    }
    
    public struct Component3 : IComponent
    {
        [IndexBy]
        public ulong[] TestData;
    }
    
    [IndexBy]
    public struct Component4 : IComponent
    {
        public ulong[] TestData;
    }

    [Fact]
    public void GetEntityByIndex()
    {
        var entity = Context.CreateEntity();

        var component = new Component() { TestData = 1, TestData2 = 2 }; 
        entity.Replace(component);
        
        var entity2 = Context.CreateEntity();
        entity2.Replace(new Component() { TestData = 2, TestData2 = 1 });
        
        EntityContexts.Save();

        var entities = Context.Index<AEntityIndexTests_ComponentIndex>().GetEntities(component).ToArray();
        var entitiesIds = entities.Select(c=>c.Id).ToArray();
        
        Assert.Equal(1, entities.Length);
        Assert.Contains(entity.Id, entitiesIds);
        Assert.DoesNotContain(entity2.Id, entitiesIds);
        
        Assert.Equal(component, entities[0].Get<Component>());
    }
    
    [Fact]
    public void GetEntityByArrayIndex()
    {
        var entity = Context.CreateEntity();

        var component = new Component4() { TestData = new[] { 1ul, 2ul } }; 
        entity.Replace(component);
        
        var entity2 = Context.CreateEntity();
        entity2.Replace(new Component4() { TestData = new[] { 2ul, 1ul } });
        
        EntityContexts.Save();

        var entities = Context.Index<AEntityIndexTests_Component4Index>().GetEntities(component).ToArray();
        var entitiesIds = entities.Select(c=>c.Id).ToArray();
        
        Assert.Equal(1, entities.Length);
        Assert.Contains(entity.Id, entitiesIds);
        Assert.DoesNotContain(entity2.Id, entitiesIds);
        
        Assert.Equal(component, entities[0].Get<Component4>());
    }
    
    [Fact]
    public void GetEntityByArrayFieldIndex()
    {
        var entity = Context.CreateEntity();

        var component = new Component3() { TestData = new[] { 1ul, 2ul } }; 
        entity.Replace(component);
        
        var entity2 = Context.CreateEntity();
        entity2.Replace(new Component3() { TestData = new[] { 2ul, 1ul } });
        
        EntityContexts.Save();

        var entities = Context.Index<AEntityIndexTests_Component3Index>().TestData(1, 2).ToArray();
        var entitiesIds = entities.Select(c=>c.Id).ToArray();
        
        Assert.Equal(1, entities.Length);
        Assert.Contains(entity.Id, entitiesIds);
        Assert.DoesNotContain(entity2.Id, entitiesIds);
        
        Assert.Equal(component, entities[0].Get<Component3>());
        
        var entities2 = Context.Index<AEntityIndexTests_Component3Index>().TestDataContains(1).ToArray();
        Assert.Equal(2, entities2.Length);
        
        var entities3 = Context.Index<AEntityIndexTests_Component3Index>().TestDataContainsAny(1, 2).ToArray();
        Assert.Equal(2, entities3.Length);
    }
    
    [Fact]
    public void GetEntitiesByFieldIndex()
    {
        var entity = Context.CreateEntity();

        var component = new Component2() { TestData = 1ul, TestData2 = 10ul}; 
        entity.Replace(component);
        
        var entity2 = Context.CreateEntity();
        entity2.Replace(new Component2() { TestData = 1ul, TestData2 = 2ul });
        
        EntityContexts.Save();

        var entities = Context.Index<AEntityIndexTests_Component2Index>().TestData(1).ToArray();
        
        Assert.Equal(2, entities.Length);
        
        var entities2 = Context.Index<AEntityIndexTests_Component2Index>().TestData2(2).ToArray();
        Assert.Equal(1, entities2.Length);
        
        var entities3 = Context.Index<AEntityIndexTests_Component2Index>().TestData2(10).ToArray();
        Assert.Equal(1, entities3.Length);
    }
}
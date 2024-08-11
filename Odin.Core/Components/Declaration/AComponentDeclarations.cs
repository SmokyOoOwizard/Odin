using Odin.Core.Abstractions.Components;
using Odin.Core.Components.Declaration.Builder;
using Odin.Core.Components.Declaration.Builder.States;

namespace Odin.Core.Components.Declaration;

public abstract class AComponentDeclarations : IComponentDeclarations
{
    // key - component id, value - component declaration
    private readonly Dictionary<ulong, ComponentDeclaration> _componentDeclarations = new();

    // key - component name, value - component id
    private readonly Dictionary<string, ulong> _componentNames = new();

    public AComponentDeclarations()
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        Configure();
    }

    protected abstract void Configure();

    public IEnumerable<ComponentDeclaration> GetComponentDeclarations()
    {
        return _componentDeclarations.Values;
    }

    public ulong? GetComponentTypeId<TComponent>() where TComponent : IComponent
    {
        var componentName = typeof(TComponent).FullName;
        if (string.IsNullOrWhiteSpace(componentName))
            return null;

        if (!_componentNames.TryGetValue(componentName, out var componentTypeId))
            return null;

        return componentTypeId;
    }

    public bool TryGet(ulong componentTypeId, out ComponentDeclaration componentDeclaration)
    {
        return _componentDeclarations.TryGetValue(componentTypeId, out componentDeclaration);
    }

    protected ComponentBuilder<TComponent, ComponentDeclarationState> Component<TComponent>()
        where TComponent : IComponent
    {
        return new ComponentBuilder<TComponent, ComponentDeclarationState>(AddComponentDescription);
    }

    protected void AddComponentDescription(ComponentDeclaration componentDeclaration)
    {
        _componentDeclarations[componentDeclaration.Id] = componentDeclaration;

        _componentNames[componentDeclaration.Name] = componentDeclaration.Id;
    }
}
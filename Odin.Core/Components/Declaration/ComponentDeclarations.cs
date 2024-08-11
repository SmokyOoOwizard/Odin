using Odin.Core.Abstractions.Components;

namespace Odin.Core.Components.Declaration;

public static class ComponentDeclarations
{
    private static readonly List<IComponentDeclarations> _componentDeclarations = new();

    static ComponentDeclarations()
    {
        Reload();
    }

    public static void Reload()
    {
        var type = typeof(IComponentDeclarations);
        var existsType = AppDomain.CurrentDomain
                                  .GetAssemblies()
                                  .SelectMany(s => s.GetTypes())
                                  .Where(p => type.IsAssignableFrom(p) && p.IsClass);

        foreach (var declarations in existsType)
        {
            var instance = (IComponentDeclarations)Activator.CreateInstance(declarations)!;

            _componentDeclarations.Add(instance);
        }
    }


    public static ulong? GetComponentTypeId<TComponent>() where TComponent : IComponent
    {
        foreach (var declaration in _componentDeclarations)
        {
            var componentTypeId = declaration.GetComponentTypeId<TComponent>();
            if (componentTypeId.HasValue)
                return componentTypeId;
        }

        return null;
    }

    public static bool TryGet(ulong componentTypeId, out ComponentDeclaration componentDeclaration)
    {
        componentDeclaration = default;

        foreach (var declaration in _componentDeclarations)
        {
            if (declaration.TryGet(componentTypeId, out componentDeclaration))
                return true;
        }

        return false;
    }
}
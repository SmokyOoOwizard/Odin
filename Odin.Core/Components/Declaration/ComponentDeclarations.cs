using Odin.Core.Abstractions.Components;

namespace Odin.Core.Components.Declaration;

public static class ComponentDeclarations
{
    private static readonly Dictionary<ulong, ComponentDeclaration> Components = new();

    private static readonly Dictionary<string, ulong> ComponentNames = new();

    static ComponentDeclarations()
    {
        Reload();
    }

    public static IEnumerable<ComponentDeclaration> GetComponentDeclarations()
    {
        return Components.Values;
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

            foreach (var declaration in instance.GetComponentDeclarations())
            {
                if (Components.ContainsKey(declaration.Id))
                    throw new Exception($"Component with id {declaration.Id} already exists");

                Components[declaration.Id] = declaration;

                ComponentNames[declaration.Name] = declaration.Id;
            }
        }
    }

    public static ulong? GetComponentTypeId<TComponent>() where TComponent : IComponent
    {
        var componentName = typeof(TComponent).FullName;
        if (string.IsNullOrWhiteSpace(componentName))
            return null;

        if (!ComponentNames.TryGetValue(componentName, out var componentTypeId))
            return null;

        return componentTypeId;
    }

    public static bool TryGet(ulong componentTypeId, out ComponentDeclaration componentDeclaration)
    {
        return Components.TryGetValue(componentTypeId, out componentDeclaration);
    }
}
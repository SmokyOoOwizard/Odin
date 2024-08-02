namespace Odin.Abstractions.Components.Declaration.Builder;

public interface IComponentBuilderState
{
    ComponentDeclaration Add<T>(ComponentDeclaration declaration) where T : IComponent; 
}
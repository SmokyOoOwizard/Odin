using Odin.Core.Abstractions.Components;

namespace Odin.Core.Components.Declaration.Builder;

public interface IComponentBuilderState
{
    ComponentDeclaration Add<T>(ComponentDeclaration declaration) where T : IComponent; 
}
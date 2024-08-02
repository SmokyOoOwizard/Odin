using Odin.Core.Abstractions.Components;

namespace Odin.Core.Components.Declaration.Builder;

public class ComponentBuilder<T, TState> where T : IComponent where TState : struct, IComponentBuilderState
{
    private readonly Action<ComponentDeclaration> _onDone;

    internal ComponentDeclaration ComponentDeclaration;
    internal TState State = default;

    internal ComponentBuilder(Action<ComponentDeclaration> onDone, ComponentDeclaration declaration = default)
    {
        _onDone = onDone;
        ComponentDeclaration = declaration;
    }

    internal void CompleteState()
    {
        ComponentDeclaration = State.Add<T>(ComponentDeclaration);
    }

    public void Build()
    {
        ComponentDeclaration = State.Add<T>(ComponentDeclaration);
        _onDone.Invoke(ComponentDeclaration);
    }

    public ComponentBuilder<T, TNewState> NewState<TNewState>() where TNewState : struct, IComponentBuilderState
    {
        return new ComponentBuilder<T, TNewState>(_onDone, ComponentDeclaration);
    }
}
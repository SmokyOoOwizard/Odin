using Odin.Core.Abstractions.Components;
using Odin.Core.Abstractions.Components.Declarations;

namespace Odin.Core.Components.Declaration.Builder.States;

public static class ComponentFieldDeclarationStateExtensions
{
    public static ComponentBuilder<T, ComponentFieldDeclarationState> AddField<T, TState>(
        this ComponentBuilder<T, TState> builder,
        string name
    ) where T : IComponent where TState : struct, IComponentBuilderState
    {
        builder.CompleteState();

        var newBuilder = builder.NewState<ComponentFieldDeclarationState>();

        var state = newBuilder.State;
        state.Name = name;

        newBuilder.UpdateState(state);

        return newBuilder;
    }

    public static ComponentBuilder<T, ComponentFieldDeclarationState> Index<T>(
        this ComponentBuilder<T, ComponentFieldDeclarationState> builder
    ) where T : IComponent
    {
        var state = builder.State;
        state.IsIndex = true;
        
        builder.UpdateState(state);

        return builder;
    }

    public static ComponentBuilder<T, ComponentFieldDeclarationState> Collection<T>(
        this ComponentBuilder<T, ComponentFieldDeclarationState> builder,
        ECollectionType type
    ) where T : IComponent
    {
        var state = builder.State;
        state.CollectionType = type;
        
        builder.UpdateState(state);

        return builder;
    }

    public static ComponentBuilder<T, ComponentFieldDeclarationState> Type<T>(
        this ComponentBuilder<T, ComponentFieldDeclarationState> builder,
        EFieldType type
    ) where T : IComponent
    {
        var state = builder.State;
        state.FieldType = type;
        
        builder.UpdateState(state);

        return builder;
    }

    public static ComponentBuilder<T, ComponentFieldDeclarationState> Offset<T>(
        this ComponentBuilder<T, ComponentFieldDeclarationState> builder,
        int offset
    ) where T : IComponent
    {
        var state = builder.State;
        state.Offset = offset;
        
        builder.UpdateState(state);

        return builder;
    }
}
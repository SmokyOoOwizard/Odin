namespace Odin.Abstractions.Components.Declaration.Builder.States;

public static class ComponentFieldDeclarationStateExtensions
{
    public static ComponentBuilder<T, ComponentFieldDeclarationState> AddField<T, TState>(
        this ComponentBuilder<T, TState> builder,
        string name
    ) where T : IComponent where TState : struct, IComponentBuilderState
    {
        builder.CompleteState();

        var newBuilder = builder.NewState<ComponentFieldDeclarationState>();

        newBuilder.State.Name = name;

        return newBuilder;
    }

    public static ComponentBuilder<T, ComponentFieldDeclarationState> Index<T>(
        this ComponentBuilder<T, ComponentFieldDeclarationState> builder
    ) where T : IComponent
    {
        builder.CompleteState();

        var newBuilder = builder.NewState<ComponentFieldDeclarationState>();

        newBuilder.State.IsIndex = true;

        return newBuilder;
    }

    public static ComponentBuilder<T, ComponentFieldDeclarationState> Collection<T>(
        this ComponentBuilder<T, ComponentFieldDeclarationState> builder,
        ECollectionType type
    ) where T : IComponent
    {
        builder.CompleteState();

        var newBuilder = builder.NewState<ComponentFieldDeclarationState>();

        newBuilder.State.CollectionType = type;

        return newBuilder;
    }

    public static ComponentBuilder<T, ComponentFieldDeclarationState> Type<T>(
        this ComponentBuilder<T, ComponentFieldDeclarationState> builder,
        EFieldType type
    ) where T : IComponent
    {
        builder.CompleteState();

        var newBuilder = builder.NewState<ComponentFieldDeclarationState>();

        newBuilder.State.FieldType = type;

        return newBuilder;
    }
}
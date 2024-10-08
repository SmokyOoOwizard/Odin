﻿using Odin.Core.Abstractions.Components;

namespace Odin.Core.Components.Declaration.Builder.States;

public static class ComponentDeclarationStateExtensions
{
    public static ComponentBuilder<T, ComponentDeclarationState> WithName<T>(
        this ComponentBuilder<T, ComponentDeclarationState> builder,
        string name
    ) where T : IComponent
    {
        builder.State.Name = name;
        return builder;
    }
    
    public static ComponentBuilder<T, ComponentDeclarationState> WithId<T>(
        this ComponentBuilder<T, ComponentDeclarationState> builder,
        ulong id
    ) where T : IComponent
    {
        builder.State.Id = id;
        return builder;
    }
    
    public static ComponentBuilder<T, ComponentDeclarationState> Size<T>(
        this ComponentBuilder<T, ComponentDeclarationState> builder,
        ulong id
    ) where T : IComponent
    {
        builder.State.Size = id;
        return builder;
    }
}
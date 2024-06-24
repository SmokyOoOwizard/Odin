using Odin.Abstractions.Components;

namespace Odin.Abstractions.Entities;

public record struct ComponentWrapper(int TypeId, IComponent? Component);
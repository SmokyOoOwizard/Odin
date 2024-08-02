using Odin.Abstractions.Components;

namespace Odin.Abstractions.Entities;

public record struct ComponentWrapper(ulong TypeId, IComponent? Component);
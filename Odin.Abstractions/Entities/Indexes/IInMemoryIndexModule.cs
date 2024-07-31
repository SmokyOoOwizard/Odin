using Odin.Abstractions.Components;

namespace Odin.Abstractions.Entities.Indexes;

public interface IInMemoryIndexModule : IIndexModule
{
}

public interface IInMemoryIndexModule<T> : IInMemoryIndexModule, IIndexModule<T> where T : struct, IComponent
{
}
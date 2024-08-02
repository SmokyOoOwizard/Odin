using Odin.Abstractions.Entities;

namespace Odin.Systems;

public interface IForeachSystem : ISystem
{
    Task Do(Entity entity);
}
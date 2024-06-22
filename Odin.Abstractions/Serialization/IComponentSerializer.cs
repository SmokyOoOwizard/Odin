using Odin.Abstractions.Components;

namespace Odin.Abstractions.Serialization;

public interface IComponentSerializer<T> where T : IComponent
{
    SerializedComponent Serialize(T component);
    SerializedComponent[] Serialize(T[] components);

    T Deserialize(SerializedComponent serializedComponent);
    T[] Deserialize(SerializedComponent[] serializedComponents);
}
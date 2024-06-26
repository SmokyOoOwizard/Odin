namespace Odin.Systems.Features;

public class Feature
{
    public readonly string Name;
    public readonly uint Priority;

    public Feature(string name, uint priority)
    {
        Name = name;
        Priority = priority;
    }
}